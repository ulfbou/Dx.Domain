from __future__ import annotations

from dataclasses import dataclass
from pathlib import Path
from typing import Any

from .evidence import write_json_atomic

SCHEMA = "dx-domain.consumer-stage-timings/1.0"


@dataclass
class ConsumerCaseTiming:
    case_id: str
    target_framework: str
    action: str
    restore_seconds: float | None = None
    build_seconds: float | None = None
    run_seconds: float | None = None
    diagnostic_parse_seconds: float = 0.0
    evidence_seconds: float = 0.0

    def to_dict(self) -> dict[str, Any]:
        return {
            "case_id": self.case_id,
            "target_framework": self.target_framework,
            "action": self.action,
            "restore_seconds": self.restore_seconds,
            "build_seconds": self.build_seconds,
            "run_seconds": self.run_seconds,
            "diagnostic_parse_seconds": self.diagnostic_parse_seconds,
            "evidence_seconds": self.evidence_seconds,
        }


class ConsumerStageTimings:
    def __init__(self) -> None:
        self._cases: dict[str, ConsumerCaseTiming] = {}

    def begin_case(self, case: dict[str, Any]) -> ConsumerCaseTiming:
        case_id = str(case["id"])
        if case_id in self._cases:
            raise ValueError(f"Duplicate consumer timing case: {case_id}")
        timing = ConsumerCaseTiming(
            case_id=case_id,
            target_framework=str(case["targetFramework"]),
            action=str(case["action"]),
        )
        self._cases[case_id] = timing
        return timing

    def summary(self) -> dict[str, Any]:
        cases = [self._cases[key].to_dict() for key in sorted(self._cases)]
        return {
            "schema": SCHEMA,
            "restore_seconds": round(sum(item["restore_seconds"] or 0.0 for item in cases), 6),
            "build_seconds": round(sum(item["build_seconds"] or 0.0 for item in cases), 6),
            "run_seconds": round(sum(item["run_seconds"] or 0.0 for item in cases), 6),
            "diagnostic_parse_seconds": round(sum(item["diagnostic_parse_seconds"] for item in cases), 6),
            "evidence_seconds": round(sum(item["evidence_seconds"] for item in cases), 6),
            "cases": cases,
        }

    def write(self, path: Path) -> None:
        write_json_atomic(path, self.summary())
