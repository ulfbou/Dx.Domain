import sys,unittest
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT))
from release_gate.consumers import *
from release_gate.diagnostics import *
class ConsumerMutations(unittest.TestCase):
 def test_project_reference_rejected(self):
  with self.assertRaises(ValueError): assert_no_project_references('<ProjectReference Include="../../src/x"/>')
 def test_repo_props_rejected(self):
  with self.assertRaises(ValueError): assert_no_repo_props_import('<Import Project="../../Directory.Build.props"/>')
 def test_missing_diagnostic_and_ad0001_fail(self):
  case={"expects":{"analyzer":"must_report","diagnosticIds":["DXA065"]}}; result=object(); self.assertEqual("FAIL",validate_analyzer_activation(case,[],result)[0].status.value); ds=parse_diagnostics_from_build_output("warning AD0001: failed"); self.assertTrue(any(x.status.value=="FAIL" for x in validate_analyzer_activation(case,ds,result)))
 def test_duplicate_execution_rejected(self):
  ds=parse_diagnostics_from_build_output("a.cs(1,1): warning DXA065: reference"); self.assertFalse(validate_no_duplicate_execution(ds+ds,ds))
if __name__=="__main__":unittest.main()
