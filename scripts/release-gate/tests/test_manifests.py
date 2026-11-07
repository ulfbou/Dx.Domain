import sys,tempfile,unittest,zipfile
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT))
from release_gate.manifests import *
class ManifestTests(unittest.TestCase):
 def test_roundtrip_and_mutation(self):
  with tempfile.TemporaryDirectory() as d:
   d=Path(d);p=d/'Dx.Domain.Kernel.0.1.0-alpha.nupkg'
   with zipfile.ZipFile(p,'w') as z:z.writestr('x.nuspec','<package><metadata><id>Dx.Domain.Kernel</id><version>0.1.0-alpha</version></metadata></package>')
   m=generate_manifest([p]);mp=d/'candidate-manifest.json';write_manifest(m,mp);self.assertTrue(all(x.status.value=='PASS' for x in verify_manifest(mp,d)));p.write_bytes(p.read_bytes()+b'x');self.assertTrue(any(x.status.value=='FAIL' for x in verify_manifest(mp,d)))
if __name__=='__main__':unittest.main()
