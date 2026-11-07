import json
import os
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))

from release_gate.feedback import build_error_feedback, transport_feedback


class RealFinalCarrierTests(unittest.TestCase):
    def test_real_carrier_contains_authoritative_handoff(self):
        with tempfile.TemporaryDirectory() as temporary:
            root = Path(temporary)
            repository = root / "repository"
            evidence = (
                repository
                / ".dx"
                / "verification"
                / "release-gate"
                / "operational-error"
            )
            transfer = root / "transfer"

            repository.mkdir()
            evidence.mkdir(parents=True)
            transfer.mkdir()

            (evidence / "run.json").write_text(
                json.dumps(
                    {
                        "schema": "dx-domain.release-gate-run.v1",
                        "run_id": "operational-error",
                        "profile": "local",
                        "head": "a" * 40,
                        "branch": "test",
                        "initial_status": [],
                    }
                )
                + "\n",
                encoding="utf-8",
            )

            for name in ("git-before.json", "git-after.json"):
                (evidence / name).write_text(
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
                        "message": "fixture operational failure",
                    }
                )
                + "\n",
                encoding="utf-8",
            )

            dossier = build_error_feedback(
                profile="local",
                repository_root=repository,
                evidence_root=evidence,
                error=RuntimeError("fixture operational failure"),
            )

            previous = os.environ.get("DX")
            os.environ["DX"] = str(transfer)

            try:
                result = transport_feedback(
                    profile="local",
                    decision="ERROR",
                    repository_root=repository,
                    evidence_root=evidence,
                    dossier=dossier,
                )
            finally:
                if previous is None:
                    os.environ.pop("DX", None)
                else:
                    os.environ["DX"] = previous

            self.assertEqual("CREATED", result.transport_status)
            self.assertEqual(
                "STRUCTURE_VALID",
                result.carrier_verification,
            )
            self.assertIsNotNone(result.carrier_path)

            carrier = Path(result.carrier_path)
            self.assertTrue(carrier.is_file())
            self.assertTrue(carrier.name.endswith(".dx.txt"))

            inspect = subprocess.run(
                [
                    sys.executable,
                    str(ROOT / "dx.py"),
                    "inspect",
                    str(carrier),
                    "--verify",
                ],
                text=True,
                stdout=subprocess.PIPE,
                stderr=subprocess.PIPE,
                check=False,
            )

            self.assertEqual(
                0,
                inspect.returncode,
                inspect.stderr or inspect.stdout,
            )

            unpacked = root / "unpacked"
            unpacked.mkdir()

            unpack = subprocess.run(
                [
                    sys.executable,
                    str(ROOT / "dx.py"),
                    "unpack",
                    str(carrier),
                    str(unpacked),
                ],
                text=True,
                stdout=subprocess.PIPE,
                stderr=subprocess.PIPE,
                check=False,
            )

            self.assertEqual(
                0,
                unpack.returncode,
                unpack.stderr or unpack.stdout,
            )

            handoffs = list(
                unpacked.rglob("release-gate/handoff.json")
            )
            self.assertEqual(1, len(handoffs))

            handoff = json.loads(
                handoffs[0].read_text(encoding="utf-8")
            )

            self.assertEqual(
                "dx-domain.release-gate-handoff/1.0",
                handoff["schema"],
            )
            self.assertEqual(
                "ERROR",
                handoff["execution"]["decision"],
            )
            self.assertEqual(
                "release-gate-operational-error",
                handoff["findings"][0]["criterion_id"],
            )
            self.assertEqual(
                "STRUCTURE_VALID",
                handoff["carrier"]["verification"],
            )


if __name__ == "__main__":
    unittest.main()
