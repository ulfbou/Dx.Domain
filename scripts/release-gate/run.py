#!/usr/bin/env python3
import argparse
import sys
from pathlib import Path

from release_gate.aggregation import exit_code
from release_gate.configuration import ConfigurationError
from release_gate.orchestrator import execute_candidate_profile, execute_gate
from release_gate.repository import RepositoryDiscoveryError, discover_repository_root


def main():
    parser = argparse.ArgumentParser(description="Run the Dx.Domain WS-003 release gate")
    parser.add_argument("--profile", choices=("local", "ci", "accept-ready", "candidate"), default="local")
    parser.add_argument("--repo", type=Path)
    parser.add_argument("--evidence-dir", type=Path)
    parser.add_argument("--command-timeout", type=int, default=1800)
    args = parser.parse_args()
    if args.command_timeout < 1:
        parser.error("--command-timeout must be positive")
    script_root = Path(__file__).resolve().parent
    starts = (args.repo,) if args.repo else (Path.cwd(), script_root)
    repository_root = discover_repository_root(*starts)
    decision, evidence_root, report = (execute_candidate_profile(repository_root, script_root, evidence_base=args.evidence_dir, timeout_seconds=args.command_timeout) if args.profile == "candidate" else execute_gate(repository_root, script_root, profile=args.profile, evidence_base=args.evidence_dir, timeout_seconds=args.command_timeout))
    print("Dx.Domain release gate")
    print(f"Profile: {args.profile}")
    print(f"Commit: {report['head']}")
    print(f"Decision: {decision.value}")
    print(f"Evidence: {evidence_root}")
    return exit_code(decision)


if __name__ == "__main__":
    try:
        sys.exit(main())
    except (ConfigurationError, RepositoryDiscoveryError, OSError, RuntimeError) as exc:
        print(f"ERROR: {exc}", file=sys.stderr)
        sys.exit(3)
