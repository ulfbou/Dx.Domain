#!/usr/bin/env python3
from __future__ import annotations

import argparse
import collections
import hashlib
import json
import subprocess
import sys
import zipfile
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any
from xml.etree import ElementTree as ET

EXPECTED_PACKAGES = (
    "Dx.Domain.Annotations",
    "Dx.Domain.Kernel",
    "Dx.Domain.Primitives",
    "Dx.Domain.Facts",
)
EXPECTED_VERSION = "0.1.0-alpha"
EXPECTED_FILENAMES = tuple(f"{package}.{EXPECTED_VERSION}.nupkg" for package in EXPECTED_PACKAGES)
EXPECTED_ANALYZER_PATH = "analyzers/dotnet/cs/Dx.Domain.Analyzers.dll"
FORBIDDEN_ANALYZER_PREFIXES = ("lib/", "runtimes/")
FORBIDDEN_PACKAGE_PREFIXES = ("Dx.Domain.Analyzers.", "Dx.Domain.Generators.")
FORBIDDEN_DEPENDENCIES = {"Dx.Domain.Analyzers", "Dx.Domain.Generators"}
MANIFEST_SCHEMA = "dx-domain.release-manifest.v1"
REPOSITORY_URL = "https://github.com/ulfbou/Dx.Domain"
SIGNING_DISPOSITION = {
    "author_signing_required": False,
    "nuget_org_repository_signing_accepted": True,
}


class ValidationFailure(RuntimeError):
    pass


@dataclass(frozen=True)
class AnalyzerIdentity:
    path: str
    size: int
    sha256: str


@dataclass(frozen=True)
class PackageIdentity:
    file: str
    id: str
    version: str
    size: int
    sha256: str
    readme: str
    analyzer: AnalyzerIdentity
    dependencies: list[str]
    archive_entries: int
    duplicate_entries: list[str]
    errors: list[str]


def sha256_bytes(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def sha256_file(path: Path) -> str:
    return sha256_bytes(path.read_bytes())


def run_git(*args: str, cwd: Path) -> str:
    result = subprocess.run(
        ("git", *args),
        cwd=cwd,
        text=True,
        encoding="utf-8",
        errors="replace",
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        raise RuntimeError(f"git {' '.join(args)} failed: {result.stderr.strip() or result.stdout.strip()}")
    return result.stdout.strip()


def local_name(tag: str) -> str:
    return tag.rsplit("}", 1)[-1]


def parse_nuspec(archive: zipfile.ZipFile) -> tuple[str | None, str | None, str | None, list[str]]:
    nuspec_entries = [name for name in archive.namelist() if name.lower().endswith(".nuspec")]
    if len(nuspec_entries) != 1:
        raise ValidationFailure(f"expected exactly one nuspec, found {len(nuspec_entries)}")

    with archive.open(nuspec_entries[0]) as stream:
        root = ET.parse(stream).getroot()

    metadata = next((node for node in root.iter() if local_name(node.tag) == "metadata"), None)
    if metadata is None:
        raise ValidationFailure("nuspec metadata section missing")

    values: dict[str, str] = {}
    for node in metadata:
        values[local_name(node.tag)] = (node.text or "").strip()

    dependencies: list[str] = []
    for dep in root.iter():
        if local_name(dep.tag) == "dependency":
            dep_id = (dep.attrib.get("id") or "").strip()
            if dep_id:
                dependencies.append(dep_id)

    return values.get("id"), values.get("version"), values.get("readme"), dependencies


def inspect_package(path: Path, authoritative_analyzer_hash: str | None) -> PackageIdentity:
    errors: list[str] = []
    package_id: str | None = None
    version: str | None = None
    readme: str | None = None
    dependencies: list[str] = []
    analyzer_identity = AnalyzerIdentity(path=EXPECTED_ANALYZER_PATH, size=0, sha256="")
    duplicate_entries: list[str] = []
    entry_count = 0

    try:
        with zipfile.ZipFile(path, "r") as archive:
            entries = archive.infolist()
            entry_count = len(entries)
            names = [entry.filename.replace("\\", "/") for entry in entries]
            name_counts = collections.Counter(names)
            duplicate_entries = sorted(name for name, count in name_counts.items() if count > 1)

            if duplicate_entries:
                errors.append(f"duplicate archive entries: {duplicate_entries}")

            package_id, version, readme, dependencies = parse_nuspec(archive)

            if not package_id:
                errors.append("nuspec package id missing")
            if not version:
                errors.append("nuspec package version missing")
            if package_id and package_id not in EXPECTED_PACKAGES:
                errors.append(f"unexpected package id: {package_id}")
            if version and version != EXPECTED_VERSION:
                errors.append(f"incorrect package version: {version}")

            if package_id and version:
                expected_filename = f"{package_id}.{version}.nupkg"
                if path.name != expected_filename:
                    errors.append(f"filename/metadata mismatch: {path.name} != {expected_filename}")

            if not readme:
                errors.append("nuspec readme metadata missing")
            else:
                readme_matches = [name for name in names if name == readme or name.endswith(f"/{readme}")]
                if len(readme_matches) != 1:
                    errors.append(f"declared readme occurrences: {len(readme_matches)}")

            analyzer_matches = [name for name in names if name == EXPECTED_ANALYZER_PATH]
            if len(analyzer_matches) != 1:
                errors.append(f"required analyzer occurrences: {len(analyzer_matches)}")

            bad_analyzer_paths = [
                name for name in names
                if name.endswith("Dx.Domain.Analyzers.dll") and name != EXPECTED_ANALYZER_PATH
            ]
            bad_analyzer_paths.extend(
                name for name in names
                if any(name.startswith(prefix) and name.endswith("Dx.Domain.Analyzers.dll") for prefix in FORBIDDEN_ANALYZER_PREFIXES)
            )
            if bad_analyzer_paths:
                errors.append(f"analyzer outside accepted path: {sorted(set(bad_analyzer_paths))}")

            if analyzer_matches:
                analyzer_data = archive.read(EXPECTED_ANALYZER_PATH)
                analyzer_identity = AnalyzerIdentity(
                    path=EXPECTED_ANALYZER_PATH,
                    size=len(analyzer_data),
                    sha256=sha256_bytes(analyzer_data),
                )
                if authoritative_analyzer_hash and analyzer_identity.sha256 != authoritative_analyzer_hash:
                    errors.append(
                        f"embedded analyzer differs from authoritative output: {analyzer_identity.sha256} != {authoritative_analyzer_hash}"
                    )

            prohibited_dependencies = [dep for dep in dependencies if dep in FORBIDDEN_DEPENDENCIES]
            if prohibited_dependencies:
                errors.append(f"prohibited dependencies: {sorted(set(prohibited_dependencies))}")

    except zipfile.BadZipFile as exc:
        raise ValidationFailure(f"invalid ZIP archive: {exc}") from exc
    except ValidationFailure:
        raise
    except Exception as exc:  # noqa: BLE001
        raise ValidationFailure(f"archive verification failed: {type(exc).__name__}: {exc}") from exc

    if package_id is None:
        package_id = path.name.removesuffix(f".{EXPECTED_VERSION}.nupkg")
    if version is None:
        version = EXPECTED_VERSION

    return PackageIdentity(
        file=path.name,
        id=package_id,
        version=version,
        size=path.stat().st_size,
        sha256=sha256_file(path),
        readme=readme or "",
        analyzer=analyzer_identity,
        dependencies=dependencies,
        archive_entries=entry_count,
        duplicate_entries=duplicate_entries,
        errors=errors,
    )


def validate_manifest(manifest_path: Path, packages: list[PackageIdentity], repository_root: Path) -> dict[str, Any]:
    manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    errors: list[str] = []

    if manifest.get("schema") != MANIFEST_SCHEMA:
        errors.append(f"manifest schema mismatch: {manifest.get('schema')}")
    if manifest.get("version") != EXPECTED_VERSION:
        errors.append(f"manifest version mismatch: {manifest.get('version')}")
    if manifest.get("repository") != REPOSITORY_URL:
        errors.append(f"manifest repository mismatch: {manifest.get('repository')}")
    if manifest.get("signing_disposition") != SIGNING_DISPOSITION:
        errors.append("manifest signing disposition mismatch")

    manifest_branch = manifest.get("branch")
    manifest_head = manifest.get("head")
    try:
        current_branch = run_git("branch", "--show-current", cwd=repository_root)
        current_head = run_git("rev-parse", "HEAD", cwd=repository_root)
    except RuntimeError as exc:
        errors.append(str(exc))
        current_branch = None
        current_head = None

    if manifest_branch and current_branch and manifest_branch != current_branch:
        errors.append(f"manifest branch mismatch: {manifest_branch} != {current_branch}")
    if manifest_head and current_head and manifest_head != current_head:
        errors.append(f"manifest head mismatch: {manifest_head} != {current_head}")

    manifest_packages = manifest.get("packages") or []
    if len(manifest_packages) != len(packages):
        errors.append(f"manifest package count mismatch: {len(manifest_packages)} != {len(packages)}")

    packages_by_file = {package.file: package for package in packages}
    expected_files = {package.file for package in packages}
    manifest_files = {entry.get("file") for entry in manifest_packages}
    if manifest_files != expected_files:
        errors.append(f"manifest package filenames mismatch: {sorted(manifest_files)} != {sorted(expected_files)}")

    for entry in manifest_packages:
        filename = entry.get("file")
        package = packages_by_file.get(filename)
        if package is None:
            continue

        if entry.get("id") != package.id:
            errors.append(f"manifest id mismatch for {filename}: {entry.get('id')} != {package.id}")
        if entry.get("version") != package.version:
            errors.append(f"manifest version mismatch for {filename}: {entry.get('version')} != {package.version}")
        if entry.get("size") != package.size:
            errors.append(f"manifest size mismatch for {filename}: {entry.get('size')} != {package.size}")
        if entry.get("sha256") != package.sha256:
            errors.append(f"manifest sha256 mismatch for {filename}")

        manifest_analyzer = entry.get("analyzer") or {}
        if manifest_analyzer.get("path") != package.analyzer.path:
            errors.append(f"manifest analyzer path mismatch for {filename}")
        if manifest_analyzer.get("size") != package.analyzer.size:
            errors.append(f"manifest analyzer size mismatch for {filename}")
        if manifest_analyzer.get("sha256") != package.analyzer.sha256:
            errors.append(f"manifest analyzer hash mismatch for {filename}")

        manifest_readme = entry.get("readme")
        if manifest_readme != package.readme:
            errors.append(f"manifest readme mismatch for {filename}: {manifest_readme!r} != {package.readme!r}")

        manifest_dependencies = entry.get("dependencies") or []
        if sorted(manifest_dependencies) != sorted(package.dependencies):
            errors.append(f"manifest dependencies mismatch for {filename}")

        if entry.get("signing_disposition") and entry.get("signing_disposition") != SIGNING_DISPOSITION:
            errors.append(f"manifest signing disposition mismatch for {filename}")

    return {
        "manifest": manifest,
        "errors": errors,
        "current_branch": current_branch,
        "current_head": current_head,
    }


def build_report(args: argparse.Namespace) -> dict[str, Any]:
    repository_root = Path(args.repository_root or Path(__file__).resolve().parents[1]).resolve()
    package_directory = Path(args.packages).resolve()
    analyzer_path = Path(args.analyzer).resolve() if args.analyzer else None
    report_path = Path(args.report).resolve()
    command = args.command or ""

    report: dict[str, Any] = {
        "schema": "dx-domain.release-package-verification.v1",
        "created_utc": datetime.now(timezone.utc).isoformat(),
        "repository": {
            "remote": run_git("remote", "get-url", "origin", cwd=repository_root),
            "branch": run_git("branch", "--show-current", cwd=repository_root),
            "head": run_git("rev-parse", "HEAD", cwd=repository_root),
            "status": run_git("status", "--porcelain=v1", "-uall", cwd=repository_root).splitlines(),
        },
        "sdk": subprocess.run(("dotnet", "--version"), text=True, stdout=subprocess.PIPE, check=True).stdout.strip(),
        "command": command,
        "candidate_location": str(package_directory),
        "expected_version": EXPECTED_VERSION,
        "expected_filenames": list(EXPECTED_FILENAMES),
        "expected_packages": list(EXPECTED_PACKAGES),
        "expected_analyzer_path": EXPECTED_ANALYZER_PATH,
        "forbidden_package_prefixes": list(FORBIDDEN_PACKAGE_PREFIXES),
        "forbidden_analyzer_prefixes": list(FORBIDDEN_ANALYZER_PREFIXES),
        "errors": [],
        "packages": [],
        "authoritative_analyzer": {},
        "manifest": None,
    }

    if not package_directory.exists():
        raise ValidationFailure(f"candidate directory does not exist: {package_directory}")
    if analyzer_path is not None:
        if not analyzer_path.exists():
            raise ValidationFailure(f"authoritative analyzer output does not exist: {analyzer_path}")
        authoritative = analyzer_path.read_bytes()
        report["authoritative_analyzer"] = {
            "path": str(analyzer_path),
            "size": len(authoritative),
            "sha256": sha256_bytes(authoritative),
        }
    else:
        report["authoritative_analyzer"] = None

    package_files = sorted(package_directory.glob("*.nupkg"), key=lambda item: item.name)
    all_candidate_artifacts = sorted(
        [path.name for path in package_directory.iterdir() if path.is_file() and path.suffix.lower() in {".nupkg", ".snupkg"}],
        key=str.lower,
    )

    for filename in all_candidate_artifacts:
        if filename.lower().endswith(".snupkg"):
            report["errors"].append(f"unexpected symbol package: {filename}")

    if len(package_files) != len(EXPECTED_FILENAMES):
        report["errors"].append(f"package count mismatch: {len(package_files)} != {len(EXPECTED_FILENAMES)}")

    filename_counts = collections.Counter(path.name for path in package_files)
    for name, count in sorted(filename_counts.items()):
        if count > 1:
            report["errors"].append(f"duplicate filename: {name}")

    expected_filename_set = set(EXPECTED_FILENAMES)
    actual_filename_set = {path.name for path in package_files}
    for filename in sorted(expected_filename_set - actual_filename_set):
        report["errors"].append(f"missing package: {filename}")
    for filename in sorted(actual_filename_set - expected_filename_set):
        report["errors"].append(f"unexpected package: {filename}")

    packages: list[PackageIdentity] = []
    package_id_counts: collections.Counter[str] = collections.Counter()

    for package_path in package_files:
        authoritative_hash = report["authoritative_analyzer"]["sha256"] if report["authoritative_analyzer"] else None
        package = inspect_package(package_path, authoritative_hash)
        package_id_counts[package.id] += 1
        packages.append(package)
        report["packages"].append({
            "file": package.file,
            "id": package.id,
            "version": package.version,
            "size": package.size,
            "sha256": package.sha256,
            "readme": package.readme,
            "analyzer": asdict(package.analyzer),
            "dependencies": package.dependencies,
            "archive_entries": package.archive_entries,
            "duplicate_entries": package.duplicate_entries,
            "errors": package.errors,
        })
        report["errors"].extend(f"{package.file}: {error}" for error in package.errors)

    for package_id, count in sorted(package_id_counts.items()):
        if count > 1:
            report["errors"].append(f"duplicate package id: {package_id}")

    if set(package_id_counts) != set(EXPECTED_PACKAGES):
        report["errors"].append(f"package id set mismatch: {sorted(package_id_counts)}")

    analyzer_hashes = {package.analyzer.sha256 for package in packages if package.analyzer.sha256}
    if len(analyzer_hashes) != 1:
        report["errors"].append(f"analyzer hashes differ: {sorted(analyzer_hashes)}")
    elif report["authoritative_analyzer"] and analyzer_hashes != {report["authoritative_analyzer"]["sha256"]}:
        report["errors"].append("embedded analyzer does not match authoritative output")

    if args.manifest:
        manifest_result = validate_manifest(Path(args.manifest).resolve(), packages, repository_root)
        report["manifest"] = {
            "path": str(Path(args.manifest).resolve()),
            "schema": manifest_result["manifest"].get("schema"),
            "errors": manifest_result["errors"],
            "branch": manifest_result["manifest"].get("branch"),
            "head": manifest_result["manifest"].get("head"),
            "signing_disposition": manifest_result["manifest"].get("signing_disposition"),
        }
        report["errors"].extend(manifest_result["errors"])

    report["success"] = not report["errors"]
    report["packages"] = sorted(report["packages"], key=lambda item: item["file"])

    report_path.parent.mkdir(parents=True, exist_ok=True)
    report_path.write_text(json.dumps(report, indent=2, sort_keys=True) + "\n", encoding="utf-8", newline="\n")

    print(f"{report_path}: {'OK' if report['success'] else 'FAILED'} ({len(report['errors'])} errors)")
    return report


def main() -> int:
    parser = argparse.ArgumentParser(description="Verify release candidate packages")
    parser.add_argument("--packages", type=Path, required=True, help="Directory containing candidate .nupkg files")
    parser.add_argument("--analyzer", type=Path, help="Authoritative analyzer build output")
    parser.add_argument("--report", type=Path, required=True, help="Path to write the JSON verification report")
    parser.add_argument("--manifest", type=Path, help="Optional release manifest to verify against the package set")
    parser.add_argument("--command", default="", help="Command used to generate the candidate")
    parser.add_argument("--repository-root", type=Path, help="Repository root; defaults to the parent of this script")
    args = parser.parse_args()

    try:
        report = build_report(args)
    except Exception as exc:  # noqa: BLE001
        print(f"ERROR: {exc}", file=sys.stderr)
        return 1

    return 0 if report["success"] else 1


if __name__ == "__main__":
    sys.exit(main())
