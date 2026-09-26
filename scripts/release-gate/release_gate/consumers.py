from __future__ import annotations
from dataclasses import dataclass
from pathlib import Path
import html
import re
from .process import run_process

FIXTURES = {
    "annotations_valid": "using Dx.Domain.Annotations;\n[Entity] public sealed class ConsumerEntity { }\n",
    "kernel_valid": "using System;\nvar result = Dx.Result.Success(\"kernel-ok\"); if (!result.IsSuccess) throw new Exception(\"failure\"); Console.WriteLine(result.Value);\n",
    "primitives_valid": "using System; using Dx.Domain.Primitives;\nvar id = CorrelationId.New(); if (id.Value == Guid.Empty) throw new Exception(\"empty\"); Console.WriteLine(id);\n",
    "facts_valid": "using System; using Dx.Domain.Facts;\nvar fact = Fact<string>.Create(\"consumer.fact\", \"facts-ok\", default); if (fact.GetPayload() != \"facts-ok\") throw new Exception(\"payload\"); Console.WriteLine(fact.FactType);\n",
    "combined_valid": "using System; using Dx.Domain.Annotations; using Dx.Domain.Primitives; using Dx.Domain.Facts;\nvar id = CorrelationId.New(); var result = Dx.Result.Success(id); var fact = Fact<CorrelationId>.Create(\"consumer.combined\", result.Value, default); Console.WriteLine(fact.GetPayload());\n[Entity] sealed class ConsumerEntity { }\n",
    "analyzer_invalid_usage": "using Dx.Domain.Annotations;\n/// <summary>Uses Dx.Domain.Annotations.EntityAttribute directly.</summary>\n[Entity] public sealed class InvalidDocumentationReference { }\n",
}

@dataclass(frozen=True)
class ConsumerWorkspace:
    id: str
    path: Path
    nuget_config_path: Path
    csproj_path: Path
    global_packages_folder: Path
    action: str
    environment: dict[str, str]

def assert_no_project_references(content: str) -> bool:
    if re.search(r"<\s*ProjectReference\b", content, re.IGNORECASE):
        raise ValueError("ProjectReference is forbidden in a package consumer")
    return True

def assert_no_repo_props_import(content: str) -> bool:
    value = content.lower().replace("\\", "/")
    if "<import" in value or "directory.packages.props" in value or "../src/" in value:
        raise ValueError("Repository build inheritance is forbidden")
    return True

def create_isolated_workspace(base_evidence_dir: Path, case: dict, candidate_dir: Path, candidate_manifest: dict) -> ConsumerWorkspace:
    root = Path(base_evidence_dir) / "consumers" / case["id"]
    root.mkdir(parents=True, exist_ok=False)
    cache = root / "packages"; cache.mkdir()
    feed = Path(candidate_dir).resolve()
    config = root / "nuget.config"
    config.write_text('<?xml version="1.0" encoding="utf-8"?>\n<configuration><config><add key="globalPackagesFolder" value="' + html.escape(str(cache.resolve())) + '" /></config><packageSources><clear /><add key="candidate" value="' + html.escape(str(feed)) + '" /><add key="nuget.org" value="https://api.nuget.org/v3/index.json" /></packageSources><packageSourceMapping><packageSource key="candidate"><package pattern="Dx.Domain.*" /></packageSource><packageSource key="nuget.org"><package pattern="*" /></packageSource></packageSourceMapping></configuration>\n', encoding="utf-8")
    props = root / "Directory.Build.props"
    props.write_text("<Project><PropertyGroup><RepoRoot>$(MSBuildThisFileDirectory)</RepoRoot></PropertyGroup></Project>\n", encoding="utf-8")
    versions = {item["packageId"]: item["version"] for item in candidate_manifest["packages"]}
    wanted = case.get("packages") or [case["carrier"]]
    references = "".join(f'<PackageReference Include="{name}" Version="{versions[name]}" />' for name in wanted)
    output = "<OutputType>Exe</OutputType>" if case["action"] == "run" else ""
    project = f'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>{case["targetFramework"]}</TargetFramework>{output}<ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable><LangVersion>latest</LangVersion><RestorePackagesWithLockFile>false</RestorePackagesWithLockFile><DirectoryBuildPropsPath>{html.escape(str(props.resolve()))}</DirectoryBuildPropsPath><ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally></PropertyGroup><ItemGroup>{references}</ItemGroup></Project>\n'
    assert_no_project_references(project); assert_no_repo_props_import(project)
    csproj = root / "Consumer.csproj"; csproj.write_text(project, encoding="utf-8")
    (root / "Program.cs").write_text(FIXTURES[case["fixture"]], encoding="utf-8")
    env = {"NUGET_PACKAGES":str(cache.resolve()), "NUGET_HTTP_CACHE_PATH":str((root/"http-cache").resolve()), "DOTNET_CLI_HOME":str((root/"dotnet-home").resolve()), "DOTNET_NOLOGO":"1"}
    return ConsumerWorkspace(case["id"], root, config, csproj, cache, case["action"], env)

def _run(ws, name, argv, timeout_seconds):
    result = run_process(command_id=name, argv=argv, cwd=ws.path, evidence_directory=ws.path, timeout_seconds=timeout_seconds, environment=ws.environment)
    text = Path(result.stdout_path).read_text(encoding="utf-8", errors="replace") + Path(result.stderr_path).read_text(encoding="utf-8", errors="replace")
    (ws.path / f"{name}.log").write_text(text, encoding="utf-8")
    return result

def restore_workspace(ws, timeout_seconds=1800):
    return _run(ws, "restore", ("dotnet","restore",str(ws.csproj_path),"--configfile",str(ws.nuget_config_path),f"-p:DirectoryBuildPropsPath={ws.path/'Directory.Build.props'}"), timeout_seconds)

def build_workspace(ws, timeout_seconds=1800):
    return _run(ws, "build", ("dotnet","build",str(ws.csproj_path),"-c","Release","--no-restore",f"-p:DirectoryBuildPropsPath={ws.path/'Directory.Build.props'}",f"-flp:logfile={ws.path/'build.diagnostics.log'};verbosity=diagnostic"), timeout_seconds)

def run_workspace(ws, timeout_seconds=1800):
    return _run(ws, "run", ("dotnet","run","--project",str(ws.csproj_path),"-c","Release","--no-build",f"-p:DirectoryBuildPropsPath={ws.path/'Directory.Build.props'}"), timeout_seconds)

def validate_runtime_output(case: dict, stdout_text: str):
    expected = case.get("expects", {})
    observed = stdout_text.strip()
    if "output" in expected:
        return observed == expected["output"], expected["output"], observed
    pattern = expected.get("outputPattern")
    return bool(pattern and re.fullmatch(pattern, observed)), pattern, observed
