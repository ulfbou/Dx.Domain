from __future__ import annotations

from .model import CriterionResult, CriterionStatus, GateDecision


def aggregate(results: list[CriterionResult]) -> GateDecision:
    statuses = {result.status for result in results}

    if CriterionStatus.ERROR in statuses:
        return GateDecision.ERROR

    if CriterionStatus.FAIL in statuses:
        return GateDecision.FAIL

    incomplete = {
        CriterionStatus.NOT_PROVEN,
        CriterionStatus.EXTERNAL_EVIDENCE_REQUIRED,
        CriterionStatus.HUMAN_DECISION_REQUIRED,
    }
    if statuses.intersection(incomplete):
        return GateDecision.INCOMPLETE

    return GateDecision.PASS


def exit_code(decision: GateDecision) -> int:
    return {
        GateDecision.PASS: 0,
        GateDecision.FAIL: 1,
        GateDecision.INCOMPLETE: 2,
        GateDecision.ERROR: 3,
    }[decision]
