import json
import os
import sys
import tempfile
import unittest
from pathlib import Path
from subprocess import CompletedProcess
from unittest.mock import patch

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))

from release_gate.feedback import (
    _package_feedback,
    build_feedback,
    cli_result,
    collect_feedback,
)


class FeedbackTests(unittest.TestCase):
    def report(self):
        return {
            "run_id": "run-1",
            "head": "a" * 40,
            "branch": "feat/release-gate-feedback",
            "criteria": [
                {
                    "criterion_id": "ok",
                    "title": "Passing criterion",
                    "status": "PASS",
                    "motivation": "The required fact was positively established.",
                    "verifier": "test.feedback",
                    "expected": {"result": "success"},
                    "observed": {"result": "success"},
                    "evidence": ["commands/ok/stdout.txt"],
                },
                {
                    "criterion_id": "na",
                    "title": "Non-applicable criterion",
                    "status": "NOT_APPLICABLE",
                    "motivation": "The criterion does not apply to this profile.",
                    "verifier": "test.feedback",
                    "expected": {"applicable": False},
                    "observed": {"applicable": False},
                    "evidence": [],
                },
                {
                    "criterion_id": "bad",
                    "title": "Failing criterion",
                    "status": "FAIL",
                    "motivation": "The observed value contradicted the contract.",
                    "verifier": "test.feedback",
                    "expected": {"value": "expected"},
                    "observed": {"value": "actual"},
                    "evidence": ["commands/bad/stderr.txt"],
                    "corrective_action": (
                        "Correct the value to match the contract and rerun."
                    ),
                },
            ],
        }

    def create_evidence(self, repository):
        evidence = (
            repository
            / ".dx"
            / "verification"
            / "release-gate"
            / "run-1"
        )
        evidence.mkdir(parents=True)

        (evidence / "run.json").write_text(
            json.dumps(
                {
                    "schema": "dx-domain.release-gate-run.v1",
                    "run_id": "run-1",
                    "head": "a" * 40,
                    "branch": "feat/release-gate-feedback",
                    "initial_status": [],
                }
            )
            + "\n",
            encoding="utf-8",
        )
        (evidence / "git-before.json").write_text(
            json.dumps({"head": "a" * 40, "tracked_diff": ""}) + "\n",
            encoding="utf-8",
        )
        (evidence / "git-after.json").write_text(
            json.dumps({"head": "a" * 40, "tracked_diff": ""}) + "\n",
            encoding="utf-8",
        )
        (evidence / "report.json").write_text(
            json.dumps(self.report()) + "\n",
            encoding="utf-8",
        )
        (evidence / "criteria.json").write_text(
            json.dumps(self.report()["criteria"]) + "\n",
            encoding="utf-8",
        )
        (evidence / "report.md").write_text(
            "# Fixture report\n",
            encoding="utf-8",
        )

        command = evidence / "commands" / "bad"
        command.mkdir(parents=True)
        (command / "stderr.txt").write_text(
            "observed contract contradiction\n",
            encoding="utf-8",
        )

        return evidence

    def dossier(self, repository, evidence):
        return build_feedback(
            profile="local",
            decision="FAIL",
            gate_exit_code=1,
            repository_root=repository,
            evidence_root=evidence,
            report=self.report(),
        )

    def test_summary_is_self_contained_and_prioritizes_findings(self):
        with tempfile.TemporaryDirectory() as temporary:
            repository = Path(temporary)
            evidence = self.create_evidence(repository)

            result = collect_feedback(
                mode="summary",
                profile="local",
                decision="FAIL",
                gate_exit_code=1,
                repository_root=repository,
                evidence_root=evidence,
                report=self.report(),
            )

            self.assertEqual("CREATED", result.feedback_status)
            self.assertEqual("NOT_REQUESTED", result.transport_status)
            self.assertEqual("PASS", result.validation_status)
            self.assertIsNotNone(result.feedback_sha256)

            loaded = json.loads(
                (evidence / "feedback.json").read_text(encoding="utf-8")
            )

            self.assertEqual("FAIL", loaded["gate"]["decision"])
            self.assertEqual(1, loaded["gate"]["exit_code"])
            self.assertEqual("PASS", loaded["validation"]["status"])

            actionable = [
                item
                for item in loaded["findings"]
                if item["status"] == "FAIL"
            ]
            self.assertEqual(["bad"], [
                item["criterion_id"] for item in actionable
            ])

            finding = actionable[0]
            self.assertEqual(
                {"value": "expected"},
                finding["expected"],
            )
            self.assertEqual(
                {"value": "actual"},
                finding["observed"],
            )
            self.assertTrue(finding["impact"])
            self.assertTrue(finding["next_action"])
            self.assertEqual(
                "ACTION-CORRECT-BAD",
                finding["next_action_code"],
            )

            self.assertTrue((evidence / "feedback.md").is_file())

    def test_none_writes_nothing(self):
        with tempfile.TemporaryDirectory() as temporary:
            repository = Path(temporary)
            evidence = repository / "evidence"
            evidence.mkdir()

            result = collect_feedback(
                mode="none",
                profile="local",
                decision="PASS",
                gate_exit_code=0,
                repository_root=repository,
                evidence_root=evidence,
                report=self.report(),
            )

            self.assertEqual("NOT_REQUESTED", result.feedback_status)
            self.assertFalse((evidence / "feedback.json").exists())
            self.assertFalse((evidence / "feedback.md").exists())

    def test_dx_transport_failure_preserves_validated_summary(self):
        with tempfile.TemporaryDirectory() as temporary:
            repository = Path(temporary)
            evidence = self.create_evidence(repository)

            with patch.dict(
                os.environ,
                {"DX": temporary},
                clear=False,
            ):
                result = collect_feedback(
                    mode="dx",
                    profile="local",
                    decision="FAIL",
                    gate_exit_code=1,
                    repository_root=repository,
                    evidence_root=evidence,
                    report=self.report(),
                )

            self.assertEqual("CREATED", result.feedback_status)
            self.assertEqual("CREATED", result.transport_status)
            self.assertEqual("PASS", result.validation_status)
            self.assertTrue((evidence / "feedback.json").is_file())
            self.assertTrue((evidence / "feedback.md").is_file())
            self.assertIsNone(result.transport_error)

    def test_dx_packages_validated_feedback_and_selected_attachments(self):
        with tempfile.TemporaryDirectory() as temporary:
            root = Path(temporary)
            repository = root / "repo"
            repository.mkdir()
            evidence = self.create_evidence(repository)

            large_binary = evidence / "consumers" / "case" / "build.binlog"
            large_binary.parent.mkdir(parents=True)
            large_binary.write_bytes(b"not selected feedback evidence")

            transfer = root / "transfer"
            transfer.mkdir()

            calls = []

            def fake_run(argv, **kwargs):
                calls.append(list(argv))

                if "pack" in argv:
                    self.assertEqual(sys.executable, argv[0])
                    self.assertEqual(str(ROOT / "dx.py"), argv[1])
                    self.assertEqual("pack", argv[2])

                    staging = Path(argv[3])
                    self.assertFalse(
                        staging.is_relative_to(repository)
                    )
                    self.assertFalse(
                        staging.is_relative_to(evidence)
                    )
                    self.assertEqual("--root", argv[4])
                    self.assertEqual(str(staging), argv[5])
                    self.assertEqual("-o", argv[6])

                    self.assertTrue(
                        (staging / "feedback.json").is_file()
                    )
                    self.assertTrue(
                        (staging / "feedback.md").is_file()
                    )
                    self.assertTrue(
                        (staging / "attachment-manifest.json").is_file()
                    )
                    self.assertFalse(
                        any(
                            item.name == "build.binlog"
                            for item in staging.rglob("*")
                        )
                    )

                    Path(argv[7]).write_text(
                        "carrier\n",
                        encoding="utf-8",
                    )
                    return CompletedProcess(
                        argv,
                        0,
                        stdout="packed",
                        stderr="",
                    )

                if "inspect" in argv:
                    self.assertEqual("--verify", argv[-1])
                    return CompletedProcess(
                        argv,
                        0,
                        stdout="verified",
                        stderr="",
                    )

                self.fail(f"Unexpected collector operation: {argv}")

            with (
                patch.dict(
                    os.environ,
                    {"DX": str(transfer)},
                    clear=True,
                ),
                patch(
                    "release_gate.feedback._collector_command",
                    return_value=[sys.executable, str(ROOT / "dx.py")],
                ),
                patch(
                    "release_gate.feedback.subprocess.run",
                    side_effect=fake_run,
                ),
            ):
                carrier = _package_feedback(
                    repository,
                    evidence,
                    "local",
                    "FAIL",
                    "run-1",
                    self.dossier(repository, evidence),
                )

            self.assertTrue(carrier.is_file())
            self.assertEqual(2, len(calls))
            self.assertIn("pack", calls[0])
            self.assertIn("inspect", calls[1])
            self.assertEqual("--verify", calls[1][-1])

    def test_cli_result_is_single_prefixed_json_line(self):
        with tempfile.TemporaryDirectory() as temporary:
            root = Path(temporary)

            result = collect_feedback(
                mode="none",
                profile="local",
                decision="PASS",
                gate_exit_code=0,
                repository_root=root,
                evidence_root=root,
                report=self.report(),
            )

            line = cli_result(
                profile="local",
                decision="PASS",
                gate_exit_code=0,
                process_exit_code=0,
                evidence_root=root,
                repository_root=root,
                result=result,
            )

            self.assertTrue(
                line.startswith("DX_RELEASE_GATE_RESULT=")
            )
            self.assertNotIn("\n", line)

            payload = json.loads(line.split("=", 1)[1])
            self.assertEqual(0, payload["process_exit_code"])
            self.assertEqual(
                "dx.release-gate.result-envelope/1.0",
                payload["schema"],
            )


if __name__ == "__main__":
    unittest.main()
