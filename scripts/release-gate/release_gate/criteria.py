from __future__ import annotations

from .model import (
    CriterionResult,
    CriterionStatus,
    ExecutionClassification,
    ProcessResult,
    TestResult,
)


def process_criterion(
    criterion_id: str,
    title: str,
    process: ProcessResult,
) -> CriterionResult:
    if process.classification is ExecutionClassification.SUCCESS:
        status = CriterionStatus.PASS
        motivation = "The authoritative command completed successfully."
        corrective_action = None
    elif process.classification is ExecutionClassification.NONZERO_EXIT:
        status = CriterionStatus.FAIL
        motivation = (
            f"The authoritative command exited with code {process.exit_code}."
        )
        corrective_action = "Inspect the retained stdout and stderr evidence."
    else:
        status = CriterionStatus.ERROR
        motivation = (
            "The verifier could not obtain valid command evidence: "
            f"{process.classification.value}."
        )
        corrective_action = "Correct the execution environment or verifier failure."

    return CriterionResult(
        criterion_id=criterion_id,
        title=title,
        status=status,
        motivation=motivation,
        verifier="release_gate.criteria.process_criterion",
        evidence=[process.stdout_path, process.stderr_path],
        expected={"classification": "SUCCESS", "exit_code": 0},
        observed={
            "classification": process.classification.value,
            "exit_code": process.exit_code,
        },
        corrective_action=corrective_action,
    )


def test_criterion(
    criterion_id: str,
    title: str,
    result: TestResult,
    minimum_discovered_tests: int,
    allow_skipped: bool,
) -> CriterionResult:
    failures: list[str] = []

    if result.total < minimum_discovered_tests:
        failures.append(
            f"expected at least {minimum_discovered_tests} discovered tests, "
            f"observed {result.total}"
        )

    if result.failed:
        failures.append(f"{result.failed} tests failed")

    if not allow_skipped and result.skipped:
        failures.append(f"{result.skipped} tests were skipped")

    if failures:
        return CriterionResult(
            criterion_id=criterion_id,
            title=title,
            status=CriterionStatus.FAIL,
            motivation="; ".join(failures) + ".",
            verifier="release_gate.criteria.test_criterion",
            evidence=[result.trx_path],
            expected={
                "minimum_discovered_tests": minimum_discovered_tests,
                "failed": 0,
                "skipped": 0 if not allow_skipped else "allowed",
            },
            observed=result.to_dict(),
            corrective_action="Correct the mandatory test project and rerun the gate.",
        )

    return CriterionResult(
        criterion_id=criterion_id,
        title=title,
        status=CriterionStatus.PASS,
        motivation="Mandatory tests were discovered, executed, and passed.",
        verifier="release_gate.criteria.test_criterion",
        evidence=[result.trx_path],
        expected={
            "minimum_discovered_tests": minimum_discovered_tests,
            "failed": 0,
            "skipped": 0 if not allow_skipped else "allowed",
        },
        observed=result.to_dict(),
    )
