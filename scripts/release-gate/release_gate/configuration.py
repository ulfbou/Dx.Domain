from __future__ import annotations

import hashlib
import json
from dataclasses import dataclass
from pathlib import Path
from typing import Any

from .model import Profile


PACKAGE_CONTRACT_PATH = Path("contracts/package-contract.json")
CANDIDATE_DIR = Path("artifacts/candidate")
MANIFEST_FILENAME = "candidate-manifest.json"

class ConfigurationError(ValueError):
    pass


@dataclass(frozen=True)
class LoadedContract:
    path: Path
    sha256: str
    value: dict[str, Any]


def load_contract(path: Path) -> LoadedContract:
    try:
        raw = path.read_bytes()
    except OSError as exc:
        raise ConfigurationError(f"Could not read contract {path}: {exc}") from exc

    try:
        value = json.loads(raw.decode("utf-8"))
    except (UnicodeDecodeError, json.JSONDecodeError) as exc:
        raise ConfigurationError(f"Invalid UTF-8 JSON contract {path}: {exc}") from exc

    if not isinstance(value, dict):
        raise ConfigurationError(f"Contract root must be an object: {path}")

    return LoadedContract(
        path=path,
        sha256=hashlib.sha256(raw).hexdigest(),
        value=value,
    )


def parse_profile(value: str) -> Profile:
    try:
        return Profile(value)
    except ValueError as exc:
        supported = ", ".join(profile.value for profile in Profile)
        raise ConfigurationError(
            f"Unsupported profile {value!r}. Supported profiles: {supported}"
        ) from exc


def validate_contracts(
    release_contract: LoadedContract,
    mandatory_projects: LoadedContract,
    repository_root: Path,
) -> None:
    release = release_contract.value
    projects = mandatory_projects.value

    if release.get("schema") != "dx-domain.release-contract.v1":
        raise ConfigurationError("Unsupported release-contract schema")

    if projects.get("schema") != "dx-domain.mandatory-projects.v1":
        raise ConfigurationError("Unsupported mandatory-projects schema")

    package_ids = release.get("package_ids")
    if not isinstance(package_ids, list) or len(package_ids) != 4:
        raise ConfigurationError("Release contract must authorize exactly four packages")

    project_items = projects.get("projects")
    if not isinstance(project_items, list) or not project_items:
        raise ConfigurationError("Mandatory-project contract contains no projects")

    seen: set[str] = set()
    for item in project_items:
        if not isinstance(item, dict):
            raise ConfigurationError("Mandatory project entry must be an object")

        path = item.get("path")
        if not isinstance(path, str) or not path:
            raise ConfigurationError("Mandatory project path is missing")

        if path in seen:
            raise ConfigurationError(f"Duplicate mandatory project: {path}")
        seen.add(path)

        if not (repository_root / path).is_file():
            raise ConfigurationError(f"Mandatory project does not exist: {path}")

        minimum = item.get("minimum_discovered_tests")
        if not isinstance(minimum, int) or minimum < 1:
            raise ConfigurationError(
                f"Mandatory project must require at least one test: {path}"
            )


CONSUMER_MATRIX_PATH = Path("contracts/consumer-matrix.json")
REQUIRED_BEHAVIORS_PATH = Path("contracts/required-behaviors.json")
def load_consumer_contracts(script_root):
    matrix = load_contract(Path(script_root) / CONSUMER_MATRIX_PATH)
    behaviors = load_contract(Path(script_root) / REQUIRED_BEHAVIORS_PATH)
    if matrix.value.get("schema") != "dx-domain.consumer-matrix.v1": raise ConfigurationError("Unsupported consumer-matrix schema")
    if behaviors.value.get("schema") != "dx-domain.required-behaviors.v1": raise ConfigurationError("Unsupported required-behaviors schema")
    if not matrix.value.get("cases"): raise ConfigurationError("Consumer matrix contains no cases")
    return matrix, behaviors
