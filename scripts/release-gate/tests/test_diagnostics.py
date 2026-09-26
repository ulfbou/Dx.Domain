import sys,unittest
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT))
from release_gate.diagnostics import *
class DiagnosticsTests(unittest.TestCase):
 def test_parse_and_count(self):
  ds=parse_diagnostics_from_build_output("a.cs(3,2): warning DXA065: reference\na.cs(3,2): warning DXA065: reference\nwarning AD0001: failed"); counts=count_effective_diagnostics(ds); self.assertEqual(1,counts["duplicates"]["DXA065"]); self.assertIn("AD0001",[x.id for x in ds])
 def test_boundary(self):
  ds=parse_diagnostics_from_build_output("a.cs(3,2): warning DXA065: reference"); self.assertTrue(validate_no_duplicate_execution(ds,ds)); self.assertFalse(validate_no_duplicate_execution(ds+ds,ds))
if __name__=="__main__":unittest.main()
