from pathlib import Path

from .model import ExecutionClassification, ProcessResult
from .process import run_process


STRICT_PROPERTIES = (
    "-p:TreatWarningsAsErrors=true",
    "-p:ContinuousIntegrationBuild=true",
)


def restore_command(solution):
    return [
        "dotnet",
        "restore",
        solution,
        "--nologo",
        *STRICT_PROPERTIES,
    ]


def build_command(solution):
    return [
        "dotnet",
        "build",
        solution,
        "-c",
        "Release",
        "--no-restore",
        "--nologo",
        *STRICT_PROPERTIES,
    ]


def test_command(
    project,
    target_framework,
    results_directory,
    log_file_name,
):
    return [
        "dotnet",
        "test",
        project,
        "-c",
        "Release",
        "-f",
        target_framework,
        "--no-build",
        "--no-restore",
        "--nologo",
        "--logger",
        f"trx;LogFileName={log_file_name}",
        "--results-directory",
        str(results_directory),
        *STRICT_PROPERTIES,
    ]


def sdk_inventory(
    repository_root,
    evidence_directory,
    timeout_seconds,
):
    return run_process(
        command_id="dotnet-list-sdks",
        argv=("dotnet", "--list-sdks"),
        cwd=repository_root,
        evidence_directory=evidence_directory,
        timeout_seconds=timeout_seconds,
    )


def has_required_sdk(
    inventory_result,
    stdout,
    required_major,
):
    if inventory_result.classification is not ExecutionClassification.SUCCESS:
        return False

    prefix = f"{required_major}."
    return any(
        line.strip().startswith(prefix)
        for line in stdout.splitlines()
    )
