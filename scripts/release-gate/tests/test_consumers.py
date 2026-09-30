import sys
import tempfile
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))

from release_gate.consumers import build_workspace, create_isolated_workspace


class ConsumerTests(unittest.TestCase):
    def manifest(self):
        return {
            "packages": [
                {"packageId": "Dx.Domain.Annotations", "version": "0.1.0-alpha"}
            ]
        }

    def case(self, identifier, *, action="build", expects=None):
        return {
            "id": identifier,
            "carrier": "Dx.Domain.Annotations",
            "targetFramework": "net8.0",
            "action": action,
            "fixture": "annotations_valid",
            "expects": expects or {},
        }

    def test_workspace_is_package_only_and_isolated(self):
        with tempfile.TemporaryDirectory() as value:
            root = Path(value)
            feed = root / "feed"
            feed.mkdir()
            workspace = create_isolated_workspace(
                root,
                self.case("annotations"),
                feed,
                self.manifest(),
            )
            content = workspace.csproj_path.read_text(encoding="utf-8")
            self.assertNotIn("ProjectReference", content)
            self.assertIn(str(feed.resolve()), workspace.nuget_config_path.read_text())
            self.assertEqual(
                root / "consumers" / "annotations" / "packages",
                workspace.global_packages_folder,
            )

    def test_workspaces_share_only_run_scoped_http_cache(self):
        with tempfile.TemporaryDirectory() as value:
            root = Path(value)
            feed = root / "feed"
            feed.mkdir()
            first = create_isolated_workspace(root, self.case("first"), feed, self.manifest())
            second = create_isolated_workspace(root, self.case("second"), feed, self.manifest())
            self.assertNotEqual(first.path, second.path)
            self.assertNotEqual(first.global_packages_folder, second.global_packages_folder)
            self.assertNotEqual(first.environment["NUGET_PACKAGES"], second.environment["NUGET_PACKAGES"])
            self.assertNotEqual(first.environment["DOTNET_CLI_HOME"], second.environment["DOTNET_CLI_HOME"])
            self.assertEqual(first.environment["NUGET_HTTP_CACHE_PATH"], second.environment["NUGET_HTTP_CACHE_PATH"])

    def capture_build_argv(self, workspace):
        from unittest.mock import patch

        with patch("release_gate.consumers._run") as execute:
            build_workspace(workspace)
        return execute.call_args.args[2]

    def test_runtime_builds_use_minimal_logging_without_diagnostic_file_logger(self):
        with tempfile.TemporaryDirectory() as value:
            root = Path(value)
            feed = root / "feed"
            feed.mkdir()
            workspace = create_isolated_workspace(
                root,
                self.case("runtime", action="run"),
                feed,
                self.manifest(),
            )
            argv = self.capture_build_argv(workspace)
            self.assertIn("--verbosity", argv)
            self.assertEqual("minimal", argv[argv.index("--verbosity") + 1])
            self.assertFalse(any(value.startswith("-flp:") for value in argv))

    def test_analyzer_builds_retain_diagnostic_file_logging(self):
        with tempfile.TemporaryDirectory() as value:
            root = Path(value)
            feed = root / "feed"
            feed.mkdir()
            case = self.case(
                "analyzer",
                expects={"analyzer": "must_report"},
            )
            case["fixture"] = "analyzer_invalid_usage"
            workspace = create_isolated_workspace(root, case, feed, self.manifest())
            argv = self.capture_build_argv(workspace)
            self.assertIn("--verbosity", argv)
            self.assertEqual("minimal", argv[argv.index("--verbosity") + 1])
            self.assertTrue(any(value.startswith("-flp:") and "verbosity=diagnostic" in value for value in argv))



if __name__ == "__main__":
    unittest.main()
