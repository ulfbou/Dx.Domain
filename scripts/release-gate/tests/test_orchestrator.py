import json
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path
from unittest.mock import patch

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))

from release_gate.model import ExecutionClassification, ProcessResult
from release_gate.orchestrator import execute_gate


class OrchestratorTests(unittest.TestCase):
    def make_repo(self, root):
        subprocess.run(("git", "init", "-q", str(root)), check=True)
        subprocess.run(("git", "-C", str(root), "config", "user.email", "test@example.invalid"), check=True)
        subprocess.run(("git", "-C", str(root), "config", "user.name", "Test"), check=True)
        for name in ("Dx.Domain.sln", "Directory.Build.props", "version.json"):
            (root / name).write_text("{}\n" if name.endswith("json") else "\n", encoding="utf-8")
        for path in ("tests/Dx.Domain.Tests/Dx.Domain.Tests.csproj", "tests/Dx.Domain.Analyzers.Tests/Dx.Domain.Analyzers.Tests.csproj"):
            target = root / path
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_text("<Project />\n", encoding="utf-8")
        subprocess.run(("git", "-C", str(root), "add", "."), check=True)
        subprocess.run(("git", "-C", str(root), "commit", "-qm", "fixture"), check=True)

    def result(self, command_id, evidence, classification=ExecutionClassification.SUCCESS, exit_code=0, stdout=""):
        directory = evidence / "commands" / command_id
        directory.mkdir(parents=True, exist_ok=True)
        out = directory / "stdout.txt"
        err = directory / "stderr.txt"
        out.write_text(stdout, encoding="utf-8")
        err.write_text("", encoding="utf-8")
        return ProcessResult(command_id, ["fake"], str(evidence), "start", "finish", 0.1, exit_code, classification, classification is ExecutionClassification.TIMEOUT, [], out.as_posix(), err.as_posix())

    def test_missing_sdk_preserves_complete_report(self):
        with tempfile.TemporaryDirectory() as temporary:
            repo = Path(temporary) / "repo"
            repo.mkdir()
            self.make_repo(repo)
            evidence = Path(temporary) / "evidence"
            def fake_run(**kwargs):
                return self.result(kwargs["command_id"], kwargs["evidence_directory"], stdout="10.0.100 [fixture]\n")
            with patch("release_gate.orchestrator.shutil.which", return_value="fixture"), patch("release_gate.orchestrator.run_process", side_effect=fake_run):
                decision, run_dir, report = execute_gate(repo, ROOT, evidence_base=evidence, timeout_seconds=1)
            self.assertEqual("INCOMPLETE", decision.value)
            self.assertTrue((run_dir / "run.json").is_file())
            self.assertTrue((run_dir / "git-before.json").is_file())
            self.assertTrue((run_dir / "git-after.json").is_file())
            self.assertTrue((run_dir / "criteria.json").is_file())
            self.assertTrue((run_dir / "report.json").is_file())
            self.assertTrue((run_dir / "report.md").is_file())
            loaded = json.loads((run_dir / "report.json").read_text(encoding="utf-8"))
            self.assertEqual(report["decision"], loaded["decision"])

    def test_dirty_and_detached_git_state_is_recorded(self):
        with tempfile.TemporaryDirectory() as temporary:
            repo = Path(temporary) / "repo"
            repo.mkdir()
            self.make_repo(repo)
            subprocess.run(("git", "-C", str(repo), "checkout", "--detach", "-q"), check=True)
            (repo / "version.json").write_text('{"dirty":true}\n', encoding="utf-8")
            evidence = Path(temporary) / "evidence"
            with patch("release_gate.orchestrator.shutil.which", return_value=None):
                _, run_dir, _ = execute_gate(repo, ROOT, evidence_base=evidence, timeout_seconds=1)
            manifest = json.loads((run_dir / "run.json").read_text(encoding="utf-8"))
            self.assertTrue(manifest["detached"])
            self.assertEqual("", manifest["branch"])
            self.assertTrue(manifest["initial_status"])


if __name__ == "__main__":
    unittest.main()
