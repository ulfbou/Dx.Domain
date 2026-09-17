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
from .model import CriterionResult, CriterionStatus, ExecutionClassification
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
