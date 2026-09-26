import sys
import tempfile
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))

from release_gate.consumers import FIXTURES, create_isolated_workspace


class ConsumerFixtureContractTests(unittest.TestCase):
    def manifest(self):
        return {
            "packages": [
                {"packageId": name, "version": "0.1.0-alpha"}
                for name in (
                    "Dx.Domain.Annotations",
                    "Dx.Domain.Kernel",
                    "Dx.Domain.Primitives",
                    "Dx.Domain.Facts",
                )
            ]
        }

    def workspace(self, case):
        temporary = tempfile.TemporaryDirectory()
        self.addCleanup(temporary.cleanup)
        root = Path(temporary.name)
        candidate = root / "candidate"
        candidate.mkdir()
        return create_isolated_workspace(
            root / "evidence",
            case,
            candidate,
            self.manifest(),
        )

    def test_build_only_fixture_is_a_library(self):
        workspace = self.workspace({
            "id": "analyzer-annotations-net8.0",
            "carrier": "Dx.Domain.Annotations",
            "targetFramework": "net8.0",
            "action": "build",
            "fixture": "analyzer_invalid_usage",
        })
        project = workspace.csproj_path.read_text(encoding="utf-8")
        self.assertNotIn("<OutputType>Exe</OutputType>", project)

    def test_runnable_fixture_is_an_executable(self):
        workspace = self.workspace({
            "id": "kernel-net8.0",
            "carrier": "Dx.Domain.Kernel",
            "targetFramework": "net8.0",
            "action": "run",
            "fixture": "kernel_valid",
        })
        project = workspace.csproj_path.read_text(encoding="utf-8")
        self.assertIn("<OutputType>Exe</OutputType>", project)

    def test_all_fixtures_use_explicit_modern_language_version(self):
        workspace = self.workspace({
            "id": "annotations-netstandard2.0",
            "carrier": "Dx.Domain.Annotations",
            "targetFramework": "netstandard2.0",
            "action": "build",
            "fixture": "annotations_valid",
        })
        project = workspace.csproj_path.read_text(encoding="utf-8")
        self.assertIn("<LangVersion>latest</LangVersion>", project)

    def test_kernel_fixture_uses_actual_public_namespace(self):
        self.assertNotIn("using Dx.Domain.Kernel;", FIXTURES["kernel_valid"])
        self.assertIn("Dx.Result.Success", FIXTURES["kernel_valid"])

    def test_combined_top_level_statements_precede_type_declaration(self):
        source = FIXTURES["combined_valid"]
        self.assertLess(source.index("var id ="), source.index("sealed class"))
        self.assertNotIn("using Dx.Domain.Kernel;", source)


if __name__ == "__main__":
    unittest.main()
