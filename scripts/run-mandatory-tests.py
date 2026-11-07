#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import subprocess
import sys
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any
from xml.etree import ElementTree as ET

TRX_NAMESPACE = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"
SKIPPED_OUTCOMES = {"NotExecuted", "NotRunnable", "Skipped", "Ignored", "Inconclusive"}


@dataclass(frozen=True)
class TestRunResult:
    project: str
    target_framework: str
    command: str
    exit_code: int
    discovered_count: int
    passed_count: int
    failed_count: int
    skipped_count: int
    trx_file: str
    timed_out: bool


def parse_target_framework(project_path: Path) -> str:
    text = project_path.read_text(encoding="utf-8")
    root = ET.fromstring(text)
    tf = root.find(".//TargetFramework")
    if tf is not None and (tf.text or "").strip():
        return (tf.text or "").strip()
    tfs = root.find(".//TargetFrameworks")
    if tfs is not None and (tfs.text or "").strip():
        return (tfs.text or "").split(";")[0].strip()
    return "unknown"


def parse_trx(trx_path: Path) -> tuple[int, int, int, int]:
    root = ET.parse(trx_path).getroot()
    ns = {"trx": TRX_NAMESPACE}
    results = root.findall(".//trx:UnitTestResult", ns)
    discovered = len(results)
    passed = sum(1 for result in results if result.attrib.get("outcome") == "Passed")
    failed = sum(1 for result in results if result.attrib.get("outcome") == "Failed")
    skipped = sum(1 for result in results if result.attrib.get("outcome") in SKIPPED_OUTCOMES)
    return discovered, passed, failed, skipped


def run_test_project(project_path: Path, configuration: str, results_dir: Path, no_build: bool, no_restore: bool, timeout_seconds: int) -> TestRunResult:
    target_framework = parse_target_framework(project_path)
    trx_name = f"{project_path.stem}-{target_framework}.trx"
    command = [
        "dotnet",
        "test",
        str(project_path),
        "-c",
        configuration,
        "--logger",
        f"trx;LogFileName={trx_name}",
        "--results-directory",
        str(results_dir),
    ]
    if no_build:
        command.append("--no-build")
    if no_restore:
        command.append("--no-restore")

    print(f"+ {' '.join(command)}", flush=True)
    timed_out = False
    try:
        completed = subprocess.run(
            command,
            text=True,
            encoding="utf-8",
            errors="replace",
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            check=False,
            timeout=timeout_seconds,
        )
        print(completed.stdout, end="")
        exit_code = completed.returncode
    except subprocess.TimeoutExpired as exc:
        timed_out = True
        exit_code = 124
        output = exc.stdout or ""
        print(output, end="")
        print(f"ERROR: test command timed out after {timeout_seconds} seconds: {' '.join(command)}", file=sys.stderr)

    trx_path = results_dir / trx_name
    if not trx_path.exists():
        discovered = passed = failed = skipped = 0
    else:
        discovered, passed, failed, skipped = parse_trx(trx_path)

    return TestRunResult(
        project=str(project_path),
        target_framework=target_framework,
        command=" ".join(command),
        exit_code=exit_code,
        discovered_count=discovered,
        passed_count=passed,
        failed_count=failed,
        skipped_count=skipped,
        trx_file=str(trx_path),
        timed_out=timed_out,
    )


def main() -> int:
    parser = argparse.ArgumentParser(description="Run mandatory tests and record TRX counts")
    parser.add_argument("--project", action="append", required=True, help="Test project path (repeatable)")
    parser.add_argument("--configuration", default="Release")
    parser.add_argument("--results-directory", type=Path, default=Path("artifacts/test-results/release-validation"))
    parser.add_argument("--output", type=Path, required=True, help="Path to write JSON summary")
    parser.add_argument("--no-build", action="store_true")
    parser.add_argument("--no-restore", action="store_true")
    parser.add_argument("--timeout-seconds", type=int, default=900, help="Per-project timeout in seconds")
    args = parser.parse_args()

    results_dir = args.results_directory.resolve()
    results_dir.mkdir(parents=True, exist_ok=True)

    summary: dict[str, Any] = {
        "schema": "dx-domain.test-run-summary.v1",
        "created_utc": datetime.now(timezone.utc).isoformat(),
        "configuration": args.configuration,
        "timeout_seconds": args.timeout_seconds,
        "results_directory": str(results_dir),
        "runs": [],
        "totals": {
            "discovered": 0,
            "passed": 0,
            "failed": 0,
            "skipped": 0,
        },
        "errors": [],
        "success": True,
    }

    for project_arg in args.project:
        project_path = Path(project_arg.strip()).resolve()
        result = run_test_project(project_path, args.configuration, results_dir, args.no_build, args.no_restore, args.timeout_seconds)
        summary["runs"].append(asdict(result))
        summary["totals"]["discovered"] += result.discovered_count
        summary["totals"]["passed"] += result.passed_count
        summary["totals"]["failed"] += result.failed_count
        summary["totals"]["skipped"] += result.skipped_count

        if result.timed_out:
            summary["errors"].append(f"{project_path}: timed out after {args.timeout_seconds} seconds")
        if result.exit_code != 0:
            summary["errors"].append(f"{project_path}: exit code {result.exit_code}")
        if result.discovered_count == 0:
            summary["errors"].append(f"{project_path}: no tests discovered")
        if result.failed_count != 0:
            summary["errors"].append(f"{project_path}: {result.failed_count} failed")
        if result.skipped_count != 0:
            summary["errors"].append(f"{project_path}: {result.skipped_count} skipped")

    summary["success"] = not summary["errors"]
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(summary, indent=2, sort_keys=True) + "\n", encoding="utf-8", newline="\n")

    print(json.dumps(summary, indent=2, sort_keys=True))
    return 0 if summary["success"] else 1


if __name__ == "__main__":
    sys.exit(main())
