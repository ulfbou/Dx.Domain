import hashlib
import json
import os
import platform
import shutil
import sys
from datetime import datetime, timezone
from pathlib import Path

from .aggregation import aggregate, exit_code
from .configuration import load_contract, validate_contracts
from .criteria import process_criterion, test_criterion
from .dotnet import build_command, restore_command, test_command
from .evidence import write_json_atomic, write_text_atomic
from .git_state import capture_git_state
from .model import CriterionResult, CriterionStatus, ExecutionClassification, GateDecision
from .process import run_process
from .trx import TrxError, parse_trx


def utc_now():
    return datetime.now(timezone.utc).isoformat()


def make_run_id(head):
    stamp = datetime.now(timezone.utc).strftime("%Y%m%dT%H%M%SZ")
    suffix = os.urandom(2).hex()
    return f"{stamp}-{head[:8]}-{suffix}"


def read_output(path):
    try:
        return Path(path).read_text(encoding="utf-8", errors="replace")
    except OSError:
        return ""


def command_record(result, repository_root):
    value = result.to_dict()
    for key in ("stdout_path", "stderr_path"):
        path = Path(value[key])
        try:
            value[key] = path.resolve().relative_to(repository_root.resolve()).as_posix()
        except ValueError:
            value[key] = path.as_posix()
    return value


def prerequisite_result(criterion_id, title, passed, motivation, expected, observed):
    return CriterionResult(
        criterion_id=criterion_id,
        title=title,
        status=CriterionStatus.PASS if passed else CriterionStatus.NOT_PROVEN,
        motivation=motivation,
        verifier="release_gate.orchestrator.prerequisite_result",
        expected=expected,
        observed=observed,
        corrective_action=None if passed else "Install or correct the required prerequisite and rerun the gate.",
    )


def error_result(criterion_id, title, motivation, evidence=None):
    return CriterionResult(
        criterion_id=criterion_id,
        title=title,
        status=CriterionStatus.ERROR,
        motivation=motivation,
        verifier="release_gate.orchestrator.error_result",
        evidence=evidence or [],
        corrective_action="Inspect the retained evidence, correct the verifier or environment, and rerun.",
    )


def execute_gate(repository_root, script_root, profile="local", evidence_base=None, timeout_seconds=1800):
    repository_root = repository_root.resolve()
    script_root = script_root.resolve()
    contracts_dir = script_root / "contracts"
    release = load_contract(contracts_dir / "release-contract.json")
    mandatory = load_contract(contracts_dir / "mandatory-projects.json")
    validate_contracts(release, mandatory, repository_root)

    initial_git = capture_git_state(repository_root)
    run_id = make_run_id(initial_git["head"])
    if evidence_base is None:
        evidence_base = repository_root / ".dx" / "verification" / "release-gate"
    evidence_root = Path(evidence_base).resolve() / run_id
    evidence_root.mkdir(parents=True, exist_ok=False)

    run_manifest = {
        "schema": "dx-domain.release-gate-run.v1",
        "run_id": run_id,
        "profile": profile,
        "phase": "DEVELOPMENT" if profile == "local" else "ACCEPT_READY",
        "created_utc": utc_now(),
        "repository_root": repository_root.as_posix(),
        "head": initial_git["head"],
        "branch": initial_git["branch"],
        "detached": initial_git["detached"],
        "initial_status": initial_git["status"],
        "platform": {"system": platform.system(), "release": platform.release(), "machine": platform.machine()},
        "python": sys.version,
        "contracts": {
            "release_contract_sha256": release.sha256,
            "mandatory_projects_sha256": mandatory.sha256,
        },
    }
    write_json_atomic(evidence_root / "run.json", run_manifest)
    write_json_atomic(evidence_root / "git-before.json", initial_git)

    commands = []
    criteria = []

    git_path = shutil.which("git")
    dotnet_path = shutil.which("dotnet")
    criteria.append(prerequisite_result("WS003-PREFLIGHT-GIT", "Git executable", bool(git_path), "Git was located on PATH." if git_path else "Git was not located on PATH.", "git executable", git_path))
    criteria.append(prerequisite_result("WS003-PREFLIGHT-DOTNET", ".NET executable", bool(dotnet_path), ".NET was located on PATH." if dotnet_path else ".NET was not located on PATH.", "dotnet executable", dotnet_path))

    if dotnet_path:
        sdk = run_process(command_id="preflight-dotnet-list-sdks", argv=("dotnet", "--list-sdks"), cwd=repository_root, evidence_directory=evidence_root, timeout_seconds=min(timeout_seconds, 120))
        commands.append(sdk)
        sdk_text = read_output(sdk.stdout_path)
        installed = sorted({line.split(".", 1)[0].strip() for line in sdk_text.splitlines() if "." in line})
        required = [str(value) for value in release.value.get("required_sdk_majors", [])]
        missing = [value for value in required if value not in installed]
        criteria.append(prerequisite_result("WS003-PREFLIGHT-SDKS", "Required .NET SDKs", sdk.classification is ExecutionClassification.SUCCESS and not missing, "All required .NET SDK major versions were found." if not missing else "Required .NET SDK major versions are missing.", required, {"installed_majors": installed, "missing": missing}))
    else:
        criteria.append(prerequisite_result("WS003-PREFLIGHT-SDKS", "Required .NET SDKs", False, "SDK inventory could not run because dotnet is unavailable.", release.value.get("required_sdk_majors", []), []))

    can_execute = dotnet_path and all(item.status is CriterionStatus.PASS for item in criteria)
    if can_execute:
        solution = release.value["solution"]
        restore = run_process(command_id="restore-solution", argv=restore_command(solution), cwd=repository_root, evidence_directory=evidence_root, timeout_seconds=timeout_seconds)
        commands.append(restore)
        criteria.append(process_criterion("WS003-RESTORE", "Strict restore", restore))

        build = None
        if restore.classification is ExecutionClassification.SUCCESS:
            build = run_process(command_id="build-solution-release-strict", argv=build_command(solution), cwd=repository_root, evidence_directory=evidence_root, timeout_seconds=timeout_seconds)
            commands.append(build)
            criteria.append(process_criterion("WS003-BUILD", "Strict Release build", build))
        else:
            criteria.append(prerequisite_result("WS003-BUILD", "Strict Release build", False, "Build was blocked by restore failure.", "successful restore", restore.classification.value))

        if build is not None and build.classification is ExecutionClassification.SUCCESS:
            for project in mandatory.value["projects"]:
                for framework in project["target_frameworks"]:
                    command_id = f"test-{project['id']}-{framework}".lower().replace(".", "-")
                    results_dir = evidence_root / "tests" / project["id"] / framework
                    trx_name = f"{project['id']}-{framework}.trx"
                    result = run_process(command_id=command_id, argv=test_command(project["path"], framework, results_dir, trx_name), cwd=repository_root, evidence_directory=evidence_root, timeout_seconds=timeout_seconds)
                    commands.append(result)
                    process_check = process_criterion(f"WS003-TEST-COMMAND-{project['id']}-{framework}", f"Mandatory test command {project['id']} {framework}", result)
                    criteria.append(process_check)
                    trx_path = results_dir / trx_name
                    if result.classification is ExecutionClassification.SUCCESS:
                        try:
                            parsed = parse_trx(trx_path, project=project["path"], target_framework=framework)
                            criteria.append(test_criterion(f"WS003-TEST-RESULT-{project['id']}-{framework}", f"Mandatory test result {project['id']} {framework}", parsed, project["minimum_discovered_tests"], project["allow_skipped"]))
                        except TrxError as exc:
                            criteria.append(error_result(f"WS003-TEST-RESULT-{project['id']}-{framework}", f"Mandatory test result {project['id']} {framework}", str(exc), [trx_path.as_posix()]))
        else:
            for project in mandatory.value["projects"]:
                for framework in project["target_frameworks"]:
                    criteria.append(prerequisite_result(f"WS003-TEST-{project['id']}-{framework}", f"Mandatory tests {project['id']} {framework}", False, "Mandatory tests were blocked by build failure.", "successful strict build", None))

    final_git = capture_git_state(repository_root)
    write_json_atomic(evidence_root / "git-after.json", final_git)
    unchanged = initial_git["head"] == final_git["head"] and initial_git["tracked_diff"] == final_git["tracked_diff"] and initial_git["status"] == final_git["status"]
    criteria.append(CriterionResult(criterion_id="WS003-TRACKED-IMMUTABILITY", title="Tracked repository immutability", status=CriterionStatus.PASS if unchanged else CriterionStatus.FAIL, motivation="Tracked repository state remained unchanged." if unchanged else "Verification changed tracked repository state.", verifier="release_gate.orchestrator.execute_gate", evidence=["git-before.json", "git-after.json"], expected={"head": initial_git["head"], "status": initial_git["status"]}, observed={"head": final_git["head"], "status": final_git["status"]}, corrective_action=None if unchanged else "Revert verification-produced tracked changes and correct the responsible command."))

    decision = aggregate(criteria)
    report = {
        "schema": "dx-domain.release-gate-report.v1",
        "run_id": run_id,
        "profile": profile,
        "head": initial_git["head"],
        "decision": decision.value,
        "commands": [command_record(item, repository_root) for item in commands],
        "criteria": [item.to_dict() for item in criteria],
    }
    write_json_atomic(evidence_root / "criteria.json", report["criteria"])
    write_json_atomic(evidence_root / "report.json", report)
    lines = ["# Dx.Domain Release Gate", "", f"- Run: `{run_id}`", f"- Profile: `{profile}`", f"- Commit: `{initial_git['head']}`", f"- Decision: **{decision.value}**", ""]
    for item in criteria:
        lines.append(f"- **{item.status.value}** `{item.criterion_id}`: {item.motivation}")
    write_text_atomic(evidence_root / "report.md", "\n".join(lines) + "\n")
    return decision, evidence_root, report

def _candidate_result(cid, ok, expected, observed, evidence=None, error=False):
    return CriterionResult(cid,cid,CriterionStatus.ERROR if error else (CriterionStatus.PASS if ok else CriterionStatus.FAIL),"Candidate evidence matched the release contract." if ok else "Candidate evidence did not match the release contract.","release_gate.orchestrator.execute_candidate_profile",evidence or [],expected,observed,None if ok else "Correct the candidate mechanism and rerun from a clean tree.")

def execute_candidate_profile(repository_root, script_root, evidence_base=None, timeout_seconds=1800):
    from .manifests import compute_sha256, generate_manifest, verify_manifest, write_manifest
    from .msbuild import assert_build_order_matches_contract, get_project_references
    from .packages import is_valid_zip, read_nuspec, validate_analyzer_placement, validate_dependencies, validate_framework_groups, validate_nuspec, validate_readme_presence, extract_analyzer_bytes
    repository_root=Path(repository_root).resolve(); script_root=Path(script_root).resolve()
    release=load_contract(script_root/'contracts/release-contract.json'); package=load_contract(script_root/'contracts/package-contract.json')
    initial=capture_git_state(repository_root); run_id=make_run_id(initial['head']); evidence_root=Path(evidence_base or repository_root/'.dx/verification/release-gate').resolve()/run_id; evidence_root.mkdir(parents=True,exist_ok=False)
    write_json_atomic(evidence_root/'run.json',{"schema":"dx-domain.release-gate-run.v1","run_id":run_id,"profile":"candidate","phase":"ACCEPT_READY","created_utc":utc_now(),"repository_root":repository_root.as_posix(),"head":initial['head'],"branch":initial['branch'],"contracts":{"release_contract_sha256":release.sha256,"package_contract_sha256":package.sha256}})
    write_json_atomic(evidence_root/'environment.json',{"platform":{"system":platform.system(),"release":platform.release(),"machine":platform.machine()},"python":sys.version})
    write_json_atomic(evidence_root/'git-before.json',initial)
    criteria=[]; commands=[]; dirty=bool(initial['status']); criteria.append(_candidate_result('candidate-clean-tree',not dirty,[],initial['status'],['git-before.json']))
    if not dirty:
      cleanup=[]
      allowed=set(release.value['candidate']['cleanup_directories'])
      for path in repository_root.rglob('*'):
        if path.is_dir() and path.name in allowed and '.git' not in path.parts and '.dx' not in path.parts: cleanup.append(path)
      cleanup=sorted(set(cleanup),key=lambda p:len(p.parts),reverse=True); write_json_atomic(evidence_root/'cleanup.json',{"affected":[p.relative_to(repository_root).as_posix() for p in cleanup]})
      for p in cleanup: shutil.rmtree(p,ignore_errors=False)
      refs={}
      try:
       for project in package.value['build_order']: refs[project]=get_project_references(project,repository_root=repository_root,evidence_directory=evidence_root,timeout_seconds=min(timeout_seconds,300))
       assert_build_order_matches_contract(package.value,refs); criteria.append(_candidate_result('candidate-build-order',True,package.value['build_order'],package.value['build_order']))
      except Exception as exc: criteria.append(_candidate_result('candidate-build-order',False,package.value['build_order'],str(exc),error=True))
      if criteria[-1].status is CriterionStatus.PASS:
       for i,project in enumerate(package.value['build_order']):
        res=run_process(command_id=f"candidate-build-{i+1}-{Path(project).stem}",argv=("dotnet","build",project,"-c","Release","--nologo","-p:TreatWarningsAsErrors=true","-p:ContinuousIntegrationBuild=true","-p:PublicRelease=true"),cwd=repository_root,evidence_directory=evidence_root,timeout_seconds=timeout_seconds); commands.append(res); criteria.append(process_criterion('candidate-build-order',f'Build {project}',res))
        if res.classification is not ExecutionClassification.SUCCESS: break
      if all(c.status is CriterionStatus.PASS for c in criteria):
       analyzer=repository_root/'src/Dx.Domain.Analyzers/bin/Release/netstandard2.0/Dx.Domain.Analyzers.dll'; authoritative=compute_sha256(analyzer)
       package_dir=evidence_root/'packages'; package_dir.mkdir()
       for item in package.value['allowlist']:
        res=run_process(command_id=f"candidate-pack-{item['id'].split('.')[-1].lower()}",argv=("dotnet","pack",item['project'],"-c","Release","--no-build","--nologo","-o",str(package_dir),f"-p:PackageVersion={package.value['version']}","-p:TreatWarningsAsErrors=true","-p:ContinuousIntegrationBuild=true","-p:PublicRelease=true"),cwd=repository_root,evidence_directory=evidence_root,timeout_seconds=timeout_seconds); commands.append(res); criteria.append(process_criterion('candidate-exact-four',f"Pack {item['id']}",res))
       paths=sorted(package_dir.glob('*.nupkg')); expected={x['filename'] for x in package.value['allowlist']}; observed={p.name for p in paths}; criteria.append(_candidate_result('candidate-exact-four',len(paths)==4 and observed==expected,sorted(expected),sorted(observed)))
       prohibited=[p.name for p in paths if any(x.lower() in p.name.lower() for x in package.value['prohibited_ids'])]; criteria.append(_candidate_result('candidate-no-prohibited',not prohibited,[],prohibited))
       analyzer_hashes=[]
       for item in package.value['allowlist']:
        p=package_dir/item['filename']; valid=is_valid_zip(p); criteria.append(_candidate_result('candidate-zip-valid',valid,True,valid,[p.as_posix()]))
        if not valid: continue
        try:
         n=read_nuspec(p); criteria.extend([validate_nuspec(n,item['id'],package.value['version'],p),validate_framework_groups(n,item['frameworks'],p),validate_dependencies(n,item['dependencies'],package.value['version'],p),validate_readme_presence(p,item['readme']),validate_analyzer_placement(p,package.value['analyzer_asset_path'],package.value['forbidden_runtime_paths'])]); analyzer_hashes.append(compute_sha256_bytes(extract_analyzer_bytes(p,package.value['analyzer_asset_path'])))
        except Exception as exc: criteria.append(_candidate_result('candidate-zip-valid',False,'valid independently inspectable package',str(exc),[p.as_posix()],error=True))
       same=len(analyzer_hashes)==4 and len(set(analyzer_hashes))==1 and analyzer_hashes[0]==authoritative; criteria.append(_candidate_result('candidate-analyzer-byte-identity',same,authoritative,analyzer_hashes))
       if all(c.status is CriterionStatus.PASS for c in criteria):
        manifest=generate_manifest(paths,release.value['candidate']['signing_disposition']); manifest_path=evidence_root/'candidate-manifest.json'; write_manifest(manifest,manifest_path); criteria.append(_candidate_result('candidate-manifest-generated',True,4,len(manifest['packages']),[manifest_path.as_posix()])); criteria.extend(verify_manifest(manifest_path,package_dir))
    final=capture_git_state(repository_root); write_json_atomic(evidence_root/'git-after.json',final); unchanged=initial['head']==final['head'] and initial['tracked_diff']==final['tracked_diff']; criteria.append(_candidate_result('candidate-source-immutability',unchanged,initial['tracked_diff'],final['tracked_diff'],['git-before.json','git-after.json']))
    decision=aggregate(criteria); report={"schema":"dx-domain.release-gate-report.v1","run_id":run_id,"profile":"candidate","head":initial['head'],"decision":decision.value,"commands":[command_record(x,repository_root) for x in commands],"criteria":[x.to_dict() for x in criteria]}; write_json_atomic(evidence_root/'criteria.json',report['criteria']); write_json_atomic(evidence_root/'report.json',report); write_text_atomic(evidence_root/'report.md','# Dx.Domain Candidate Gate\n\n'+f'- Decision: **{decision.value}**\n'+''.join(f"- **{x.status.value}** `{x.criterion_id}`: {x.motivation}\n" for x in criteria)); return decision,evidence_root,report

def compute_sha256_bytes(value): return hashlib.sha256(value).hexdigest()


def _consumer_result(identifier, passed, expected, observed, evidence, status=None):
    return CriterionResult(identifier, identifier, status or (CriterionStatus.PASS if passed else CriterionStatus.FAIL), "Consumer evidence matched the contract." if passed else "Consumer evidence did not match the contract.", "release_gate.orchestrator.execute_consumers_profile", evidence, expected, observed, None if passed else "Correct the consumer mechanism and rerun.")

def _discover_candidate(repository_root, candidate_dir=None):
    if candidate_dir:
        directory = Path(candidate_dir).resolve(); manifest = directory.parent / "candidate-manifest.json"
        if not manifest.is_file(): manifest = directory / "candidate-manifest.json"
        return manifest, directory
    manifests = sorted((repository_root / ".dx/verification/release-gate").glob("*/candidate-manifest.json"), key=lambda path:path.stat().st_mtime, reverse=True)
    return (None, None) if not manifests else (manifests[0], manifests[0].parent / "packages")

def execute_consumers_profile(repository_root, script_root, evidence_base=None, timeout_seconds=1800, candidate_dir=None):
    from .configuration import load_consumer_contracts
    from .consumers import create_isolated_workspace, restore_workspace, build_workspace, run_workspace, assert_no_project_references, assert_no_repo_props_import, validate_runtime_output
    from .diagnostics import parse_diagnostics_from_build_output, validate_analyzer_activation
    from .manifests import verify_manifest
    repository_root = Path(repository_root).resolve(); script_root = Path(script_root).resolve(); initial = capture_git_state(repository_root)
    run_id = make_run_id(initial["head"]); root = Path(evidence_base or repository_root/".dx/verification/release-gate").resolve()/run_id; root.mkdir(parents=True, exist_ok=False)
    matrix, behaviors = load_consumer_contracts(script_root); manifest_path, packages = _discover_candidate(repository_root, candidate_dir); criteria=[]; commands=[]
    write_json_atomic(root/"run.json", {"schema":"dx-domain.release-gate-run.v1","run_id":run_id,"profile":"consumers","phase":"ACCEPT_READY","created_utc":utc_now(),"repository_root":repository_root.as_posix(),"head":initial["head"],"contracts":{"consumer_matrix_sha256":matrix.sha256,"required_behaviors_sha256":behaviors.sha256}})
    write_json_atomic(root/"environment.json", {"platform":platform.platform(),"python":sys.version}); write_json_atomic(root/"git-before.json", initial)
    if manifest_path is None or not packages.is_dir():
        criteria.append(_consumer_result("consumer-manifest-correlation", False, "verified retained candidate", None, [], CriterionStatus.NOT_PROVEN))
    else:
        verified=verify_manifest(manifest_path, packages); verified_ok=bool(verified) and all(item.status is CriterionStatus.PASS for item in verified)
        criteria.append(_consumer_result("consumer-manifest-correlation", verified_ok, "all entries verified", [x.to_dict() for x in verified], [str(manifest_path),str(packages)], None if verified_ok else CriterionStatus.NOT_PROVEN))
        if verified_ok:
            manifest=json.loads(manifest_path.read_text(encoding="utf-8")); baseline=None; baseline_case=None
            import shutil
            shutil.copyfile(manifest_path, root / "candidate-manifest.json")
            for case in matrix.value["cases"]:
                try:
                    ws=create_isolated_workspace(root,case,packages,manifest); content=ws.csproj_path.read_text(encoding="utf-8"); isolated=assert_no_project_references(content) and assert_no_repo_props_import(content)
                    criteria.append(_consumer_result(case["id"]+":consumer-isolation", isolated, True, isolated, [str(ws.csproj_path),str(ws.nuget_config_path)]))
                    restore=restore_workspace(ws,timeout_seconds); commands.append(restore); criteria.append(process_criterion(case["id"]+":consumer-restore",case["id"]+" restore",restore))
                    if restore.classification is not ExecutionClassification.SUCCESS: continue
                    build=build_workspace(ws,timeout_seconds); commands.append(build); text=read_output(build.stdout_path)+read_output(build.stderr_path); diagnostics=parse_diagnostics_from_build_output(text,ws.path/"build.diagnostics.log"); write_json_atomic(ws.path/"diagnostics.json",[x.to_dict() for x in diagnostics])
                    criteria.append(process_criterion(case["id"]+":consumer-build",case["id"]+" build",build))
                    if case.get("expects",{}).get("analyzer") == "must_report":
                        criteria.extend(validate_analyzer_activation(case,diagnostics,build,baseline if case["carrier"]=="combined" else None,text));
                        if case.get("analyzerBaseline") is True: baseline=diagnostics; baseline_case=case["id"]
                    if case["action"]=="run" and build.classification is ExecutionClassification.SUCCESS:
                        run=run_workspace(ws,timeout_seconds); commands.append(run); criteria.append(process_criterion(case["id"]+":consumer-run",case["id"]+" run",run))
                        output=read_output(run.stdout_path); ok,expected,observed=validate_runtime_output(case,output)
                        criteria.append(_consumer_result(case["id"]+":consumer-runtime-output",ok,expected,observed,[run.stdout_path]))
                except Exception as exc:
                    criteria.append(_consumer_result(case["id"]+":consumer-isolation",False,True,str(exc),[],CriterionStatus.ERROR))
    final=capture_git_state(repository_root); write_json_atomic(root/"git-after.json",final); criteria.append(_consumer_result("consumer-source-immutability",initial["head"]==final["head"] and initial["tracked_diff"]==final["tracked_diff"],initial["tracked_diff"],final["tracked_diff"],["git-before.json","git-after.json"]))
    decision=aggregate(criteria); report={"schema":"dx-domain.release-gate-report.v1","run_id":run_id,"profile":"consumers","head":initial["head"],"decision":decision.value,"commands":[command_record(x,repository_root) for x in commands],"criteria":[x.to_dict() for x in criteria]}
    write_json_atomic(root/"criteria.json",report["criteria"]); write_json_atomic(root/"report.json",report); write_text_atomic(root/"report.md","# Dx.Domain Consumer Gate\n\n- Decision: **"+decision.value+"**\n"+"".join(f"- **{x.status.value}** `{x.criterion_id}`: {x.motivation}\n" for x in criteria)); return decision,root,report
def execute_accept_ready_profile(repository_root, script_root, evidence_base=None, timeout_seconds=1800, candidate_dir=None):
    """Execute WS-003, WS-004, and WS-005 in dependency order and aggregate evidence."""
    repository_root = Path(repository_root).resolve()
    script_root = Path(script_root).resolve()
    initial = capture_git_state(repository_root)
    run_id = make_run_id(initial["head"])
    root = Path(evidence_base or repository_root / ".dx/verification/release-gate").resolve() / run_id
    root.mkdir(parents=True, exist_ok=False)
    write_json_atomic(root / "run.json", {
        "schema": "dx-domain.release-gate-run.v1", "run_id": run_id,
        "profile": "accept-ready", "phase": "ACCEPT_READY", "created_utc": utc_now(),
        "repository_root": repository_root.as_posix(), "head": initial["head"],
        "branch": initial["branch"],
    })
    write_json_atomic(root / "git-before.json", initial)
    stages = []

    def record(name, outcome):
        decision, evidence, report = outcome
        stages.append({"name": name, "decision": decision.value,
                       "evidence_directory": evidence.as_posix(), "report": report})
        return decision

    ci = record("strict-ci", execute_gate(repository_root, script_root, profile="ci",
                                           evidence_base=root / "stages" / "strict-ci",
                                           timeout_seconds=timeout_seconds))
    if ci is GateDecision.PASS:
        candidate = record("candidate", execute_candidate_profile(
            repository_root, script_root, evidence_base=root / "stages" / "candidate",
            timeout_seconds=timeout_seconds))
    else:
        candidate = None
        stages.append({"name": "candidate", "decision": "NOT_PROVEN",
                       "blocked_by": "strict-ci", "report": None})
    if candidate is GateDecision.PASS:
        candidate_report = stages[-1]["report"]
        generated = Path(stages[-1]["evidence_directory"])
        consumers = record("consumers", execute_consumers_profile(
            repository_root, script_root, evidence_base=root / "stages" / "consumers",
            timeout_seconds=timeout_seconds, candidate_dir=generated / "packages"))
    else:
        consumers = None
        stages.append({"name": "consumers", "decision": "NOT_PROVEN",
                       "blocked_by": "candidate", "report": None})

    stage_values = [item["decision"] for item in stages]
    if "ERROR" in stage_values:
        decision = GateDecision.ERROR
    elif "FAIL" in stage_values:
        decision = GateDecision.NOT_ACCEPT_READY
    elif stage_values == ["PASS", "PASS", "PASS"]:
        decision = GateDecision.ACCEPT_READY
    else:
        decision = GateDecision.ACCEPT_READY_NOT_PROVEN
    final = capture_git_state(repository_root)
    write_json_atomic(root / "git-after.json", final)
    report = {"schema": "dx-domain.release-gate-report.v1", "run_id": run_id,
              "profile": "accept-ready", "head": initial["head"],
              "decision": decision.value, "stages": stages,
              "source_unchanged": initial["head"] == final["head"] and
                                  initial["tracked_diff"] == final["tracked_diff"]}
    write_json_atomic(root / "criteria.json", [
        {"criterion_id": "accept-ready-stage-" + item["name"],
         "status": "PASS" if item["decision"] == "PASS" else item["decision"],
         "observed": item["decision"], "evidence": [item.get("evidence_directory", "")],
         "blocked_by": item.get("blocked_by")}
        for item in stages
    ])
    write_json_atomic(root / "report.json", report)
    write_text_atomic(root / "report.md", "# Dx.Domain ACCEPT READY Gate\n\n" +
                      f"- Decision: **{decision.value}**\n" +
                      "".join(f"- {item['name']}: **{item['decision']}**\n" for item in stages))
    return decision, root, report
