import sys,unittest
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT))
from release_gate.msbuild import assert_build_order_matches_contract
class MsbuildTests(unittest.TestCase):
 def test_order(self):self.assertTrue(assert_build_order_matches_contract({'build_order':['a.csproj','b.csproj']},{'a.csproj':[],'b.csproj':[{'Identity':'a.csproj'}]}))
 def test_duplicate(self):
  with self.assertRaises(ValueError):assert_build_order_matches_contract({'build_order':['a','a']},{})
if __name__=='__main__':unittest.main()
