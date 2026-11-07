from __future__ import annotations

from dataclasses import asdict, dataclass, field
from enum import Enum
from pathlib import Path
from typing import Any


class Profile(str, Enum):
    LOCAL = "local"
    CI = "ci"
    ACCEPT_READY = "accept-ready"


class CriterionStatus(str, Enum):
    PASS = "PASS"
    FAIL = "FAIL"
    NOT_PROVEN = "NOT_PROVEN"
    NOT_APPLICABLE = "NOT_APPLICABLE"
    NOT_YET_APPLICABLE = "NOT_YET_APPLICABLE"
    EXTERNAL_EVIDENCE_REQUIRED = "EXTERNAL_EVIDENCE_REQUIRED"
    HUMAN_DECISION_REQUIRED = "HUMAN_DECISION_REQUIRED"
    ACCEPTED_INPUT = "ACCEPTED_INPUT"
    ERROR = "ERROR"


class ExecutionClassification(str, Enum):
    SUCCESS = "SUCCESS"
    NONZERO_EXIT = "NONZERO_EXIT"
    TIMEOUT = "TIMEOUT"
    INACTIVITY_TIMEOUT = "INACTIVITY_TIMEOUT"
    EXECUTABLE_NOT_FOUND = "EXECUTABLE_NOT_FOUND"
    START_FAILURE = "START_FAILURE"
    TERMINATION_FAILURE = "TERMINATION_FAILURE"
    OUTPUT_CAPTURE_FAILURE = "OUTPUT_CAPTURE_FAILURE"


class GateDecision(str, Enum):
    PASS = "PASS"
    FAIL = "FAIL"
    INCOMPLETE = "INCOMPLETE"
    ERROR = "ERROR"


@dataclass(frozen=True)
class ProcessResult:
    command_id: str
    argv: list[str]
    cwd: str
    started_utc: str
    finished_utc: str
    duration_seconds: float
    exit_code: int | None
    classification: ExecutionClassification
    timed_out: bool
    termination_actions: list[str]
    stdout_path: str
    stderr_path: str

    def to_dict(self) -> dict[str, Any]:
        result = asdict(self)
        result["classification"] = self.classification.value
        return result


@dataclass(frozen=True)
class CriterionResult:
    criterion_id: str
    title: str
    status: CriterionStatus
    motivation: str
    verifier: str
    evidence: list[str] = field(default_factory=list)
    expected: Any = None
    observed: Any = None
    corrective_action: str | None = None

    def to_dict(self) -> dict[str, Any]:
        result = asdict(self)
        result["status"] = self.status.value
        return result


@dataclass(frozen=True)
class TestResult:
    project: str
    target_framework: str
    trx_path: str
    total: int
    executed: int
    passed: int
    failed: int
    skipped: int
    duration: str | None

    def to_dict(self) -> dict[str, Any]:
        return asdict(self)


@dataclass
class RunContext:
    repository_root: Path
    evidence_root: Path
    run_id: str
    profile: Profile
    head: str
    branch: str
    initial_status: list[str]
    contracts: dict[str, Any]
