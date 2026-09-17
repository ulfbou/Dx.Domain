from __future__ import annotations
import json
from pathlib import Path
from .model import ExecutionClassification
from .process import run_process

PROPERTIES=("PackageId","Version","PackageVersion","IsPackable","TargetFramework","TargetFrameworks","PackageReadmeFile","PackageOutputPath")

def _json_output(result):
    if result.classification is not ExecutionClassification.SUCCESS:
        stdout = Path(result.stdout_path).read_text(encoding="utf-8", errors="replace")
        stderr = Path(result.stderr_path).read_text(encoding="utf-8", errors="replace")
        raise RuntimeError(
            "MSBuild evaluation failed: "
            f"{result.classification.value}; exit_code={result.exit_code}; "
            f"stdout={result.stdout_path}; stderr={result.stderr_path}; "
            f"detail={(stderr or stdout).strip()}"
        )
    text=Path(result.stdout_path).read_text(encoding="utf-8",errors="replace")
    try: return json.loads(text)
    except json.JSONDecodeError as exc: raise RuntimeError(f"Invalid MSBuild JSON output: {exc}") from exc

def get_evaluated_properties(project_path, configuration="Release", *, repository_root=None, evidence_directory=None, timeout_seconds=300):
    project=Path(project_path); root=Path(repository_root or Path.cwd()); evidence=Path(evidence_directory or root/".dx/verification/release-gate/msbuild")
    result=run_process(command_id=f"msbuild-properties-{project.stem}",argv=("dotnet","msbuild",str(project),f"-property:Configuration={configuration}","-property:TreatWarningsAsErrors=true","-property:ContinuousIntegrationBuild=true",*(f"-getProperty:{x}" for x in PROPERTIES)),cwd=root,evidence_directory=evidence,timeout_seconds=timeout_seconds)
    return _json_output(result).get("Properties",{})

def get_project_references(project_path, *, repository_root=None, evidence_directory=None, timeout_seconds=300):
    project=Path(project_path); root=Path(repository_root or Path.cwd()); evidence=Path(evidence_directory or root/".dx/verification/release-gate/msbuild")
    result=run_process(command_id=f"msbuild-references-{project.stem}",argv=("dotnet","msbuild",str(project),"-property:Configuration=Release","-getItem:ProjectReference"),cwd=root,evidence_directory=evidence,timeout_seconds=timeout_seconds)
    return _json_output(result).get("Items",{}).get("ProjectReference",[])

def assert_build_order_matches_contract(contract, references_by_project):
    order=contract["build_order"]; positions={Path(x).as_posix():i for i,x in enumerate(order)}
    if len(positions)!=len(order): raise ValueError("Package contract build order contains duplicates")
    for project, refs in references_by_project.items():
        current=positions[Path(project).as_posix()]
        for ref in refs:
            identity=ref.get("FullPath") or ref.get("Identity","")
            normalized=Path(identity).resolve().as_posix() if Path(identity).is_absolute() else (Path(project).parent/identity).resolve().as_posix()
            matches=[p for p in order if normalized.endswith(Path(p).as_posix())]
            if matches and positions[matches[0]]>=current: raise ValueError(f"Build order places dependency {matches[0]} after {project}")
    return True
