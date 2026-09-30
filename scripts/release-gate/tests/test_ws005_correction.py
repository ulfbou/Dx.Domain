import sys, unittest
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT))
from release_gate.model import GateDecision
from release_gate.aggregation import exit_code
class CorrectionTests(unittest.TestCase):
    def test_accept_ready_exit_codes(self):
        self.assertEqual(0, exit_code(GateDecision.ACCEPT_READY))
        self.assertEqual(1, exit_code(GateDecision.NOT_ACCEPT_READY))
        self.assertEqual(2, exit_code(GateDecision.ACCEPT_READY_NOT_PROVEN))
    def test_run_dispatches_accept_ready(self):
        text=(ROOT/'run.py').read_text(encoding='utf-8')
        self.assertIn('execute_accept_ready_profile', text)
if __name__=='__main__': unittest.main()
