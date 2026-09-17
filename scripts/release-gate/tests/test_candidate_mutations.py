import sys,tempfile,unittest,zipfile
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT))
from release_gate.packages import *
class MutationTests(unittest.TestCase):
 def make(self,p,version='0.1.0-alpha',analyzers=('analyzers/dotnet/cs/Dx.Domain.Analyzers.dll',)):
  with zipfile.ZipFile(p,'w') as z:
   z.writestr('x.nuspec',f'<package><metadata><id>Dx.Domain.Kernel</id><version>{version}</version></metadata></package>')
   for a in analyzers:z.writestr(a,b'a')
 def test_mutations_rejected(self):
  with tempfile.TemporaryDirectory() as d:
   d=Path(d);p=d/'v.nupkg';self.make(p,'9.9.9');self.assertEqual('FAIL',validate_nuspec(read_nuspec(p),'Dx.Domain.Kernel','0.1.0-alpha').status.value)
   p=d/'none.nupkg';self.make(p,analyzers=());self.assertEqual('FAIL',validate_analyzer_placement(p).status.value)
   p=d/'two.nupkg';self.make(p,analyzers=('analyzers/dotnet/cs/Dx.Domain.Analyzers.dll','lib/Dx.Domain.Analyzers.dll'));self.assertEqual('FAIL',validate_analyzer_placement(p).status.value)
   p=d/'truncated.nupkg';p.write_bytes(b'x');self.assertFalse(is_valid_zip(p))
if __name__=='__main__':unittest.main()

class CandidateCommandContractTests(unittest.TestCase):
    def test_candidate_commands_force_public_release_versioning(self):
        orchestrator = (
            ROOT / "release_gate" / "orchestrator.py"
        ).read_text(encoding="utf-8")

        self.assertEqual(
            2,
            orchestrator.count('"-p:PublicRelease=true"'),
            "Both candidate build and pack commands must force public-release versioning.",
        )
        self.assertIn(
            'f"-p:PackageVersion={package.value[\'version\']}"',
            orchestrator,
        )
