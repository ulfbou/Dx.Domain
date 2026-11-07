#!/usr/bin/env python3
from __future__ import annotations

import argparse
import hashlib
import json
import shutil
import subprocess
import sys
from datetime import datetime, timezone
from pathlib import Path
from typing import Any

VERSION = "0.1.0-alpha"
RELEASE_TAG = "v0.1.0-alpha"
PROJECTS = (
    ("Dx.Domain.Annotations", Path("src/Dx.Domain.Annotations/Dx.Domain.Annotations.csproj")),
    ("Dx.Domain.Kernel", Path("src/Dx.Domain.Kernel/Dx.Domain.Kernel.csproj")),
    ("Dx.Domain.Primitives", Path("src/Dx.Domain.Primitives/Dx.Domain.Primitives.csproj")),
    ("Dx.Domain.Facts", Path("src/Dx.Domain.Facts/Dx.Domain.Facts.csproj")),
)
EXPECTED_ANALYZER_PATH = Path("src/Dx.Domain.Analyzers/bin/Release/netstandard2.0/Dx.Domain.Analyzers.dll")
SIGNING_DISPOSITION = {
    "author_signing_required": False,
    "nuget_org_repository_signing_accepted": True,
}


def run(*args: str, cwd: Path | None = None) -> str:
    command = " ".join(args)
    print(f"+ {command}", flush=True)
    completed = subprocess.run(
        args,
        cwd=cwd,
        text=True,
        encoding="utf-8",
        errors="replace",
        stdout=subprocess.PIPE,
        stderr=subprocess.STDOUT,
        check=False,
    )
    print(completed.stdout, end="")
    if completed.returncode != 0:
        raise RuntimeError(f"command failed ({completed.returncode}): {command}")
    return completed.stdout


def git(*args: str, cwd: Path | None = None) -> str:
    completed = subprocess.run(
        ("git", *args),
        cwd=cwd,
        text=True,
        encoding="utf-8",
        errors="replace",
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if completed.returncode != 0:
        raise RuntimeError(f"git {' '.join(args)} failed: {completed.stderr.strip() or completed.stdout.strip()}")
    return completed.stdout.strip()


def git_status(cwd: Path) -> list[str]:
    return git("status", "--porcelain=v1", "-uall", cwd=cwd).splitlines()


def sha256_file(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def parse_metadata(report: dict[str, Any]) -> list[dict[str, Any]]:
    packages = []
    for package in report["packages"]:
        packages.append(
            {
                "file": package["file"],
                "id": package["id"],
                "version": package["version"],
                "size": package["size"],
                "sha256": package["sha256"],
                "readme": package["readme"],
                "analyzer": package["analyzer"],
                "dependencies": package["dependencies"],
                "signing_disposition": SIGNING_DISPOSITION,
            }
        )
    return packages


def main() -> int:
    parser = argparse.ArgumentParser(description="Build and verify the authoritative release candidate")
    parser.add_argument("--output", type=Path, default=Path("artifacts/release/0.1.0-alpha"), help="Release candidate output directory")
    parser.add_argument("--evidence", type=Path, default=Path(".dx/verification"), help="Directory for verification evidence")
    parser.add_argument("--allow-dirty", action="store_true", help="Allow an unclean working tree (not recommended for releases)")
    parser.add_argument(
        "--allow-local-global-json",
        action="store_true",
        help="Allow only a repository-local global.json modification required by the execution environment",
    )
    parser.add_argument("--release-tag", default=RELEASE_TAG, help="Expected immutable release tag")
    parser.add_argument("--expected-commit", help="Expected Git commit SHA for the release tag")
    parser.add_argument("--repository-root", type=Path, default=Path(__file__).resolve().parents[1], help="Repository root")
    args = parser.parse_args()

    repository_root = args.repository_root.resolve()
    branch = git("branch", "--show-current", cwd=repository_root)
    head = git("rev-parse", "HEAD", cwd=repository_root)
    status = git_status(repository_root)
    ignored_status = git("status", "--porcelain=v1", "--ignored=matching", cwd=repository_root).splitlines()

    if args.release_tag != RELEASE_TAG:
        raise RuntimeError(f"release tag must be {RELEASE_TAG!r}")

    resolved_tag_commit = git("rev-parse", f"{args.release_tag}^{{commit}}", cwd=repository_root)
    if args.expected_commit and resolved_tag_commit != args.expected_commit:
        raise RuntimeError(f"tag {args.release_tag!r} resolves to {resolved_tag_commit}, expected {args.expected_commit}")
    if resolved_tag_commit != head:
        raise RuntimeError(f"tag {args.release_tag!r} resolves to {resolved_tag_commit}, but HEAD is {head}")

    allowed_local_status = {" M global.json"} if args.allow_local_global_json else set()
    unexpected_status = [entry for entry in status if entry not in allowed_local_status]
    if unexpected_status and not args.allow_dirty:
        formatted_status = "\n".join(f"  {entry}" for entry in unexpected_status)
        raise RuntimeError(f"working tree contains unexpected changes:\n{formatted_status}")

    run("dotnet", "tool", "restore", cwd=repository_root)
    resolved_version = run("dotnet", "nbgv", "get-version", "-v", "NuGetPackageVersion", cwd=repository_root).strip().splitlines()[-1]
    if resolved_version != VERSION:
        raise RuntimeError(f"NBGV version {resolved_version!r} does not equal {VERSION!r}")

    if args.output.exists():
        shutil.rmtree(args.output)
    args.output.mkdir(parents=True, exist_ok=True)
    args.evidence.mkdir(parents=True, exist_ok=True)

    run("dotnet", "clean", "Dx.Domain.sln", "-c", "Release", "--nologo", cwd=repository_root)
    common = ("-c", "Release", "--nologo", "-p:ContinuousIntegrationBuild=true", "-p:TreatWarningsAsErrors=true")

    run("dotnet", "build", "src/Dx.Domain.Analyzers/Dx.Domain.Analyzers.csproj", *common, cwd=repository_root)
    run("dotnet", "build", "src/Dx.Domain.Annotations/Dx.Domain.Annotations.csproj", *common, cwd=repository_root)
    run("dotnet", "build", "src/Dx.Domain.Kernel/Dx.Domain.Kernel.csproj", *common, cwd=repository_root)
    run("dotnet", "build", "src/Dx.Domain.Primitives/Dx.Domain.Primitives.csproj", *common, cwd=repository_root)
    run("dotnet", "build", "src/Dx.Domain.Facts/Dx.Domain.Facts.csproj", *common, cwd=repository_root)

    for package_id, project_path in PROJECTS:
        run(
            "dotnet",
            "pack",
            str(project_path),
            *common,
            "--no-build",
            "--no-restore",
            "-o",
            str(args.output),
            cwd=repository_root,
        )

    report_path = args.evidence / "ws-001-002-package-verification.json"
    verification_command = " ".join(("python", "scripts/verify-release-packages.py", "--packages", str(args.output), "--analyzer", str(EXPECTED_ANALYZER_PATH), "--report", str(report_path), "--release-tag", args.release_tag, "--expected-commit", head))
    run(
        sys.executable,
        "scripts/verify-release-packages.py",
        "--packages",
        str(args.output),
        "--analyzer",
        str(EXPECTED_ANALYZER_PATH),
        "--report",
        str(report_path),
        "--command",
        verification_command,
        "--repository-root",
        str(repository_root),
        cwd=repository_root,
    )

    verification = json.loads(report_path.read_text(encoding="utf-8"))
    manifest = {
        "schema": "dx-domain.release-manifest.v1",
        "created_utc": datetime.now(timezone.utc).isoformat(),
        "release_tag": args.release_tag,
        "repository": "https://github.com/ulfbou/Dx.Domain",
        "branch": branch,
        "head": head,
        "working_tree_clean": not status,
        "working_tree_status": status,
        "ignored_status": ignored_status,
        "version": VERSION,
        "packages": parse_metadata(verification),
        "authoritative_analyzer": verification["authoritative_analyzer"],
        "verification_report": str(report_path),
        "verification_report_sha256": sha256_file(report_path),
        "signing_disposition": SIGNING_DISPOSITION,
    }
    manifest_path = args.output / "release-manifest.json"
    manifest_path.write_text(json.dumps(manifest, indent=2, sort_keys=True) + "\n", encoding="utf-8", newline="\n")

    run(
        sys.executable,
        "scripts/verify-release-packages.py",
        "--packages",
        str(args.output),
        "--analyzer",
        str(EXPECTED_ANALYZER_PATH),
        "--report",
        str(args.evidence / "ws-001-002-package-verification.manifest-check.json"),
        "--manifest",
        str(manifest_path),
        "--command",
        f"manifest verification for {args.release_tag} at {head}",
        "--repository-root",
        str(repository_root),
        cwd=repository_root,
    )

    print(manifest_path)
    return 0


if __name__ == "__main__":
    try:
        sys.exit(main())
    except Exception as exc:  # noqa: BLE001
        print(f"ERROR: {exc}", file=sys.stderr)
        sys.exit(1)
