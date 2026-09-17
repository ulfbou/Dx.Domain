import sys,tempfile,unittest
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT))
from release_gate.consumers import *
class ConsumerTests(unittest.TestCase):
 def test_workspace_is_package_only_and_isolated(self):
  with tempfile.TemporaryDirectory() as d:
   root=Path(d); feed=root/"feed"; feed.mkdir(); manifest={"packages":[{"packageId":"Dx.Domain.Annotations","version":"0.1.0-alpha"}]}; case={"id":"annotations","carrier":"Dx.Domain.Annotations","targetFramework":"netstandard2.0","action":"build","fixture":"annotations_valid"}; ws=create_isolated_workspace(root,case,feed,manifest); content=ws.csproj_path.read_text(); self.assertTrue(assert_no_project_references(content)); self.assertIn(str(feed.resolve()),ws.nuget_config_path.read_text()); self.assertEqual(ws.global_packages_folder,root/"consumers/annotations/packages")
if __name__=="__main__":unittest.main()
