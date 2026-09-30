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
    cases = matrix.value.get("cases")
    inventory = behaviors.value.get("behaviors")
    if not isinstance(cases, list) or not cases: raise ConfigurationError("Consumer matrix contains no cases")
    if not isinstance(inventory, list) or not inventory: raise ConfigurationError("Required behaviors contains no behaviors")
    ids = [x.get("id") for x in cases if isinstance(x, dict)]
    if len(ids) != len(set(ids)) or any(not x for x in ids): raise ConfigurationError("Consumer case IDs must be non-empty and unique")
    fixtures = {x.get("fixture") for x in inventory if isinstance(x, dict)}
    allowed_carriers = {"Dx.Domain.Annotations", "Dx.Domain.Kernel", "Dx.Domain.Primitives", "Dx.Domain.Facts", "combined"}
    allowed_actions = {"build", "run"}
    for case in cases:
        cid = case.get("id", "<missing>")
        if case.get("fixture") not in fixtures: raise ConfigurationError(f"Unknown fixture for {cid}: {case.get('fixture')}")
        if case.get("carrier") not in allowed_carriers: raise ConfigurationError(f"Unknown carrier for {cid}: {case.get('carrier')}")
        if case.get("action") not in allowed_actions: raise ConfigurationError(f"Unsupported action for {cid}: {case.get('action')}")
        expects = case.get("expects")
        if not isinstance(expects, dict): raise ConfigurationError(f"Missing expectations for {cid}")
        if case.get("action") == "run" and not (isinstance(expects.get("output"), str) or isinstance(expects.get("outputPattern"), str)):
            raise ConfigurationError(f"Runnable case lacks expected output: {cid}")
    required_tfms = {"net8.0", "net9.0", "net10.0"}
    for carrier in ("Dx.Domain.Kernel", "Dx.Domain.Primitives", "Dx.Domain.Facts", "combined"):
        observed = {x.get("targetFramework") for x in cases if x.get("carrier") == carrier and x.get("action") == "run"}
        if observed != required_tfms: raise ConfigurationError(f"Incomplete framework coverage for {carrier}: {sorted(observed)}")
    baselines = [x for x in cases if x.get("analyzerBaseline") is True]
    if len(baselines) != 1 or baselines[0].get("expects", {}).get("analyzer") != "must_report":
        raise ConfigurationError("Exactly one explicit Analyzer baseline is required")
    return matrix, behaviors
