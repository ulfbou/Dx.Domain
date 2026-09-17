import sys,tempfile,unittest,zipfile
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT))
from release_gate.packages import *
class PackageTests(unittest.TestCase):
 def make(self,p,extra=(),version='0.1.0-alpha',analyzer=True):
  nus=f'<package><metadata><id>Dx.Domain.Kernel</id><version>{version}</version><readme>readme.md</readme><dependencies><group targetFramework="net8.0"><dependency id="Dx.Domain.Annotations" version="[0.1.0-alpha]"/></group></dependencies></metadata></package>'
  with zipfile.ZipFile(p,'w') as z:
   z.writestr('Dx.Domain.Kernel.nuspec',nus);z.writestr('readme.md','x');z.writestr('lib/net8.0/a.dll',b'x')
   if analyzer:z.writestr('analyzers/dotnet/cs/Dx.Domain.Analyzers.dll',b'a')
   for n,v in extra:z.writestr(n,v)
 def test_valid_and_contract(self):
  with tempfile.TemporaryDirectory() as d:
   p=Path(d)/'a.nupkg';self.make(p);self.assertTrue(is_valid_zip(p));n=read_nuspec(p);self.assertEqual('PASS',validate_nuspec(n,'Dx.Domain.Kernel','0.1.0-alpha').status.value);self.assertEqual('PASS',validate_readme_presence(p,'readme.md').status.value);self.assertEqual('PASS',validate_analyzer_placement(p).status.value)
 def test_invalid_and_wrong_analyzer(self):
  with tempfile.TemporaryDirectory() as d:
   p=Path(d)/'a.nupkg';p.write_bytes(b'bad');self.assertFalse(is_valid_zip(p));q=Path(d)/'b.nupkg';self.make(q,[('lib/net8.0/Dx.Domain.Analyzers.dll',b'a')]);self.assertEqual('FAIL',validate_analyzer_placement(q).status.value)
 def test_duplicate_rejected(self):
  with tempfile.TemporaryDirectory() as d:
   p=Path(d)/'a.nupkg';self.make(p,[('readme.md',b'y')]);self.assertFalse(is_valid_zip(p))
if __name__=='__main__':unittest.main()
