import contextlib
import importlib.util
import io
import json
import os
import sys
import tempfile
import unittest
from pathlib import Path
from unittest.mock import patch


ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))

from release_gate.feedback import (
    FeedbackResult,
    _collector_command,
    build_error_feedback,
    transport_feedback,
)


def load_runner_module():
    path = ROOT / "run.py"
    spec = importlib.util.spec_from_file_location(
        "release_gate_test_runner",
        path,
    )
    if spec is None or spec.loader is None:
        raise RuntimeError(f"Could not load runner from {path}")

    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class FeedbackRelevanceTests(unittest.TestCase):
    def make_error_evidence(self, repository: Path) -> Path:
        evidence = repository / "evidence" / "operational-error"
        evidence.mkdir(parents=True)

        (evidence / "run.json").write_text(
            json.dumps(
                {
                    "schema": "dx-domain.release-gate-run.v1",
                    "run_id": "operational-error",
                    "profile": "local",
                    "head": "a" * 40,
                    "branch": "test",
                    "initial_status": [],
                    "platform": {
                        "system": "test",
                        "release": "test",
                        "machine": "test",
                    },
                    "python": sys.version,
                }
            )
            + "\n",
            encoding="utf-8",
        )
        (evidence / "git-before.json").write_text(
            json.dumps(
                {
                    "head": "a" * 40,
                    "tracked_diff": "",
                }
            )
            + "\n",
            encoding="utf-8",
        )
        (evidence / "git-after.json").write_text(
            json.dumps(
                {
                    "head": "a" * 40,
                    "tracked_diff": "",
                }
            )
            + "\n",
            encoding="utf-8",
        )
        (evidence / "error.json").write_text(
            json.dumps(
                {
                    "schema": "dx-domain.release-gate-error.v1",
                    "profile": "local",
                    "error_type": "RuntimeError",
                    "message": "candidate manifest is unreadable",
                }
            )
            + "\n",
            encoding="utf-8",
        )
        return evidence

    def test_operational_error_feedback_describes_exact_failure(self):
        with tempfile.TemporaryDirectory() as temporary:
            repository = Path(temporary)
            evidence = self.make_error_evidence(repository)
            error = RuntimeError("candidate manifest is unreadable")

            dossier = build_error_feedback(
                profile="local",
                repository_root=repository,
                evidence_root=evidence,
                error=error,
            )

            self.assertEqual("ERROR", dossier["gate"]["decision"])
            self.assertEqual(3, dossier["gate"]["exit_code"])
            self.assertFalse(dossier["gate"]["achieved"])
            self.assertEqual(
                "ERROR",
                dossier["gate"]["proof_completeness"],
            )
            self.assertEqual("PASS", dossier["validation"]["status"])

            self.assertEqual(1, len(dossier["findings"]))
            finding = dossier["findings"][0]

            self.assertEqual(
                "release-gate-operational-error",
                finding["criterion_id"],
            )
            self.assertEqual("ERROR", finding["status"])
            self.assertEqual("PRIMARY", finding["kind"])
            self.assertEqual("ERROR", finding["severity"])
            self.assertEqual(
                "RuntimeError: candidate manifest is unreadable",
                finding["occurred"],
            )
            self.assertEqual(
                {"operation": "complete selected profile"},
                finding["expected"],
            )
            self.assertEqual(
                {
                    "error_type": "RuntimeError",
                    "message": "candidate manifest is unreadable",
                },
                finding["observed"],
            )
            self.assertEqual(
                {
                    "expected": {
                        "operation": "complete selected profile",
                    },
                    "observed": {
                        "error_type": "RuntimeError",
                        "message": "candidate manifest is unreadable",
                    },
                },
                finding["decisive_facts"],
            )
            self.assertEqual(["error.json"], finding["provenance"])
            self.assertTrue(finding["impact"])
            self.assertEqual(
                "ACTION-CORRECT-VERIFIER-"
                "RELEASE-GATE-OPERATIONAL-ERROR",
                finding["next_action_code"],
            )
            self.assertIn(
                "error.json",
                finding["next_action"],
            )

            self.assertEqual(
                [
                    {
                        "action_code": finding["next_action_code"],
                        "instruction": finding["next_action"],
                        "criterion_id":
                            "release-gate-operational-error",
                    }
                ],
                dossier["actions"],
            )
            self.assertEqual([], dossier["blocked_work"])

            serialized = json.dumps(
                dossier,
                ensure_ascii=False,
                sort_keys=True,
            )
            self.assertNotIn("product failure", serialized.lower())
            self.assertNotIn("candidate package failed", serialized.lower())
            self.assertNotIn("consumer test failed", serialized.lower())

    def test_operational_error_transport_preserves_same_finding(self):
        with tempfile.TemporaryDirectory() as temporary:
            repository = Path(temporary) / "repo"
            repository.mkdir()
            evidence = self.make_error_evidence(repository)
            error = OSError("collector executable is unavailable")

            dossier = build_error_feedback(
                profile="consumers",
                repository_root=repository,
                evidence_root=evidence,
                error=error,
            )

            captured = {}

            def fake_package(
                repository_root,
                evidence_root,
                profile,
                decision,
                run_id,
                transported,
            ):
                captured["repository_root"] = repository_root
                captured["evidence_root"] = evidence_root
                captured["profile"] = profile
                captured["decision"] = decision
                captured["run_id"] = run_id
                captured["dossier"] = transported

                carrier = Path(temporary) / "feedback.dx.txt"
                carrier.write_text("carrier\n", encoding="utf-8")
                return carrier

            with patch(
                "release_gate.feedback._package_feedback",
                side_effect=fake_package,
            ):
                result = transport_feedback(
                    profile="consumers",
                    decision="ERROR",
                    repository_root=repository,
                    evidence_root=evidence,
                    dossier=dossier,
                )

            self.assertEqual("CREATED", result.feedback_status)
            self.assertEqual("CREATED", result.transport_status)
            self.assertEqual("PASS", result.validation_status)
            self.assertIsNotNone(result.feedback_sha256)

            transported = captured["dossier"]
            self.assertEqual("ERROR", captured["decision"])
            self.assertEqual(
                dossier["findings"],
                transported["findings"],
            )
            self.assertEqual(
                "release-gate-operational-error",
                transported["findings"][0]["criterion_id"],
            )
            self.assertEqual(
                {
                    "error_type": "OSError",
                    "message": "collector executable is unavailable",
                },
                transported["findings"][0]["observed"],
            )

            persisted = json.loads(
                (evidence / "feedback.json").read_text(
                    encoding="utf-8"
                )
            )
            self.assertEqual(
                transported["findings"],
                persisted["findings"],
            )

    def test_runner_summary_reports_relevant_operational_error(self):
        runner = load_runner_module()

        with tempfile.TemporaryDirectory() as temporary:
            repository = Path(temporary) / "repo"
            repository.mkdir()
            evidence_base = Path(temporary) / "evidence"

            output = io.StringIO()
            errors = io.StringIO()

            with (
                patch.object(
                    runner,
                    "discover_repository_root",
                    return_value=repository,
                ),
                patch.object(
                    runner,
                    "execute",
                    side_effect=RuntimeError(
                        "strict restore could not start"
                    ),
                ),
                contextlib.redirect_stdout(output),
                contextlib.redirect_stderr(errors),
            ):
                code = runner.run(
                    [
                        "--profile",
                        "local",
                        "--feedback",
                        "summary",
                        "--evidence-dir",
                        str(evidence_base),
                    ]
                )

            self.assertEqual(3, code)
            self.assertIn(
                "ERROR: strict restore could not start",
                errors.getvalue(),
            )

            feedback_files = list(
                evidence_base.glob("*/feedback.json")
            )
            self.assertEqual(1, len(feedback_files))

            dossier = json.loads(
                feedback_files[0].read_text(encoding="utf-8")
            )
            self.assertEqual("ERROR", dossier["gate"]["decision"])
            self.assertEqual(1, len(dossier["findings"]))
            self.assertEqual(
                "release-gate-operational-error",
                dossier["findings"][0]["criterion_id"],
            )
            self.assertEqual(
                {
                    "error_type": "RuntimeError",
                    "message": "strict restore could not start",
                },
                dossier["findings"][0]["observed"],
            )

            result_lines = [
                line
                for line in output.getvalue().splitlines()
                if line.startswith("DX_RELEASE_GATE_RESULT=")
            ]
            self.assertEqual(1, len(result_lines))
            envelope = json.loads(
                result_lines[0].split("=", 1)[1]
            )
            self.assertEqual("ERROR", envelope["decision"])
            self.assertEqual(3, envelope["gate_exit_code"])
            self.assertEqual(3, envelope["process_exit_code"])
            self.assertEqual("summary", envelope["feedback"])
            self.assertEqual("CREATED", envelope["feedback_status"])
            self.assertEqual(
                "NOT_REQUESTED",
                envelope["transport_status"],
            )

    def test_runner_dx_transports_finalized_error_dossier_directly(self):
        runner = load_runner_module()

        with tempfile.TemporaryDirectory() as temporary:
            repository = Path(temporary) / "repo"
            repository.mkdir()
            evidence_base = Path(temporary) / "evidence"
            captured = {}

            def fake_transport(**kwargs):
                captured.update(kwargs)
                finding = kwargs["dossier"]["findings"][0]
                self.assertEqual(
                    "release-gate-operational-error",
                    finding["criterion_id"],
                )
                self.assertEqual(
                    {
                        "error_type": "RuntimeError",
                        "message": "consumer workspace creation failed",
                    },
                    finding["observed"],
                )
                return FeedbackResult(
                    mode="dx",
                    feedback_status="CREATED",
                    transport_status="CREATED",
                    feedback_path="feedback.json",
                    carrier_path="feedback.dx.txt",
                    feedback_sha256="a" * 64,
                    validation_status="PASS",
                )

            with (
                patch.object(
                    runner,
                    "discover_repository_root",
                    return_value=repository,
                ),
                patch.object(
                    runner,
                    "execute",
                    side_effect=RuntimeError(
                        "consumer workspace creation failed"
                    ),
                ),
                patch.object(
                    runner,
                    "transport_feedback",
                    side_effect=fake_transport,
                ),
                patch.object(
                    runner,
                    "collect_feedback",
                ) as collect,
                contextlib.redirect_stdout(io.StringIO()),
                contextlib.redirect_stderr(io.StringIO()),
            ):
                code = runner.run(
                    [
                        "--profile",
                        "consumers",
                        "--feedback",
                        "dx",
                        "--evidence-dir",
                        str(evidence_base),
                    ]
                )

            self.assertEqual(3, code)
            self.assertFalse(collect.called)
            self.assertEqual("ERROR", captured["decision"])
            self.assertEqual(
                "ERROR",
                captured["dossier"]["gate"]["decision"],
            )

    def test_windows_collector_command_removes_matching_quotes(self):
        with tempfile.TemporaryDirectory() as temporary:
            root = Path(temporary)
            executable = root / "Python Runtime" / "python.exe"
            collector = root / "Collector Scripts" / "dx.py"
            executable.parent.mkdir()
            collector.parent.mkdir()
            executable.write_text("", encoding="utf-8")
            collector.write_text("", encoding="utf-8")

            configured = f'"{executable}" "{collector}"'

            with (
                patch.dict(
                    os.environ,
                    {"DX_RELEASE_GATE_COLLECTOR": configured},
                    clear=True,
                ),
                patch(
                    "release_gate.feedback.os.name",
                    "nt",
                ),
                patch(
                    "release_gate.feedback.shutil.which",
                    return_value=None,
                ),
            ):
                command = _collector_command()

            self.assertEqual(
                [str(executable), str(collector)],
                command,
            )
            self.assertFalse(command[0].startswith('"'))
            self.assertFalse(command[0].endswith('"'))
            self.assertFalse(command[1].startswith('"'))
            self.assertFalse(command[1].endswith('"'))

    def test_windows_collector_command_preserves_unmatched_quote(self):
        with tempfile.TemporaryDirectory() as temporary:
            executable = Path(temporary) / "python.exe"
            executable.write_text("", encoding="utf-8")
            configured = f'"{executable}" trailing"'

            with (
                patch.dict(
                    os.environ,
                    {"DX_RELEASE_GATE_COLLECTOR": configured},
                    clear=True,
                ),
                patch(
                    "release_gate.feedback.os.name",
                    "nt",
                ),
                patch(
                    "release_gate.feedback.shutil.which",
                    return_value=None,
                ),
            ):
                command = _collector_command()

            self.assertIsNotNone(command)
            self.assertEqual(str(executable), command[0])
            self.assertEqual('trailing"', command[1])


if __name__ == "__main__":
    unittest.main()
