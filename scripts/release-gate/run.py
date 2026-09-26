#!/usr/bin/env python3
import argparse
import sys
import traceback
from datetime import datetime, timezone
from pathlib import Path

from release_gate.aggregation import exit_code
from release_gate.configuration import ConfigurationError
from release_gate.evidence import write_json_atomic
from release_gate.feedback import (
    FeedbackResult,
    build_error_feedback,
    cli_result,
    collect_feedback,
    transport_feedback,
)
from release_gate.orchestrator import execute_accept_ready_profile, execute_candidate_profile, execute_consumers_profile, execute_gate
from release_gate.repository import RepositoryDiscoveryError, discover_repository_root

EXPECTED_OPERATIONAL_ERRORS = (ConfigurationError, RepositoryDiscoveryError, OSError, RuntimeError)


def parse_args(argv=None):
    parser = argparse.ArgumentParser(description="Run the Dx.Domain WS-003 release gate")
    parser.add_argument("--profile", choices=("local", "ci", "accept-ready", "candidate", "consumers"), default="local")
    parser.add_argument("--feedback", choices=("dx",), default="dx")
    parser.add_argument("--repo", type=Path)
    parser.add_argument("--evidence-dir", type=Path)
    parser.add_argument("--candidate-dir", type=Path)
    parser.add_argument("--command-timeout", type=int, default=1800)
    args = parser.parse_args(argv)
    if args.command_timeout < 1:
        parser.error("--command-timeout must be positive")
    return args


def execute(args, repository_root, script_root):
    common = {
        "evidence_base": args.evidence_dir,
        "timeout_seconds": args.command_timeout,
    }
    if args.profile == "candidate":
        return execute_candidate_profile(repository_root, script_root, **common)
    if args.profile == "consumers":
        return execute_consumers_profile(repository_root, script_root,
                                         candidate_dir=args.candidate_dir, **common)
    if args.profile == "accept-ready":
        return execute_accept_ready_profile(repository_root, script_root,
                                            candidate_dir=args.candidate_dir, **common)
    return execute_gate(repository_root, script_root, profile=args.profile, **common)


def _error_evidence_root(args, repository_root):
    base = args.evidence_dir or repository_root / ".dx" / "verification" / "release-gate" / "errors"
    stamp = datetime.now(timezone.utc).strftime("%Y%m%dT%H%M%SZ")
    root = Path(base).resolve() / f"{stamp}-{args.profile}"
    suffix = 1
    while root.exists():
        root = root.with_name(f"{stamp}-{args.profile}-{suffix}")
        suffix += 1
    root.mkdir(parents=True)
    return root


def run(argv=None):
    args = parse_args(argv)
    script_root = Path(__file__).resolve().parent
    starts = (args.repo,) if args.repo else (Path.cwd(), script_root)
    repository_root = None
    evidence_root = None
    decision_value = "ERROR"
    gate_code = 3
    feedback_result = FeedbackResult("dx", "NOT_CREATED", "FAILED")
    try:
        repository_root = discover_repository_root(*starts)
        decision, evidence_root, report = execute(args, repository_root, script_root)
        decision_value = decision.value
        gate_code = exit_code(decision)
        feedback_result = collect_feedback(
            mode=args.feedback, profile=args.profile, decision=decision_value,
            gate_exit_code=gate_code, repository_root=repository_root,
            evidence_root=evidence_root, report=report,
        )
        print("Dx.Domain release gate", file=sys.stderr)
        print(f"Profile: {args.profile}", file=sys.stderr)
        print(f"Commit: {report['head']}", file=sys.stderr)
        print(f"Decision: {decision_value}", file=sys.stderr)
        print(f"Evidence: {evidence_root}", file=sys.stderr)
        if feedback_result.transport_status == "FAILED":
            print(f"WARNING: feedback transport failed: {feedback_result.transport_error}", file=sys.stderr)
    except EXPECTED_OPERATIONAL_ERRORS as exc:
        repository_root = repository_root or Path.cwd().resolve()
        try:
            evidence_root = _error_evidence_root(args, repository_root)
            write_json_atomic(evidence_root / "error.json", {"schema": "dx-domain.release-gate-error.v1", "profile": args.profile,
                "error_type": type(exc).__name__, "message": str(exc), "traceback": traceback.format_exc().splitlines()})
            feedback = build_error_feedback(profile=args.profile, repository_root=repository_root, evidence_root=evidence_root, error=exc)
            feedback_result = transport_feedback(profile=args.profile, decision="ERROR", repository_root=repository_root, evidence_root=evidence_root, dossier=feedback)
        except (OSError, RuntimeError, ValueError) as collection_error:
            feedback_result = FeedbackResult("dx", "FAILED", "FAILED", transport_error=str(collection_error))
        print(f"ERROR: {exc}", file=sys.stderr)
    process_code = 3 if feedback_result.transport_status != "CREATED" or feedback_result.carrier_verification != "PASS" else gate_code
    print(cli_result(
        profile=args.profile, decision=decision_value,
        gate_exit_code=gate_code, process_exit_code=process_code,
        evidence_root=evidence_root, repository_root=repository_root,
        result=feedback_result,
    ))
    return process_code


if __name__ == "__main__":
    sys.exit(run())
