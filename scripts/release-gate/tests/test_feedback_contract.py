import json
import sys
import tempfile
import unittest
from pathlib import Path
ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))
from release_gate.feedback import FeedbackValidationError, build_feedback, collect_feedback, validate_feedback

class FeedbackContractTests(unittest.TestCase):
    def base(self, root):
        evidence = root / '.dx/verification/release-gate/run-1'; evidence.mkdir(parents=True)
        (evidence / 'run.json').write_text(json.dumps({'run_id':'run-1','head':'a'*40,'branch':'test','initial_status':[]}), encoding='utf-8')
        (evidence / 'git-before.json').write_text(json.dumps({'head':'a'*40,'tracked_diff':''}), encoding='utf-8')
        (evidence / 'git-after.json').write_text(json.dumps({'head':'a'*40,'tracked_diff':''}), encoding='utf-8')
        return evidence
    def test_material_failure_is_self_contained(self):
        with tempfile.TemporaryDirectory() as value:
            root=Path(value); evidence=self.base(root)
            report={'run_id':'run-1','head':'a'*40,'branch':'test','criteria':[{'criterion_id':'x','title':'X','status':'FAIL','motivation':'Observed contradiction.','expected':{'value':1},'observed':{'value':2},'corrective_action':'Set value to 1 and rerun.','evidence':['commands/x/stdout.txt']}]}
            result=collect_feedback(mode='summary',profile='local',decision='FAIL',gate_exit_code=1,repository_root=root,evidence_root=evidence,report=report)
            self.assertEqual('PASS',result.validation_status)
            dossier=json.loads((evidence/'feedback.json').read_text(encoding='utf-8'))
            finding=dossier['findings'][0]
            self.assertEqual({'value':1},finding['expected']); self.assertEqual({'value':2},finding['observed'])
            self.assertTrue(finding['impact']); self.assertTrue(finding['next_action_code'])
    def test_source_unchanged_requires_positive_proof(self):
        dossier={'gate':{'decision':'PASS','exit_code':0,'profile':'local','achieved':True},'repository':{'source_unchanged':True,'positive_proof':False},'findings':[],'packages':{'identities':[]},'conclusion':{}}
        self.assertTrue(any('positive' in item for item in validate_feedback(dossier)))
    def test_candidate_success_requires_four_identities(self):
        dossier={'gate':{'decision':'PASS','exit_code':0,'profile':'candidate','achieved':True},'repository':{},'findings':[],'packages':{'identities':[]},'conclusion':{}}
        self.assertTrue(any('four-package' in item for item in validate_feedback(dossier)))
    def test_missing_expected_rejected(self):
        dossier={'gate':{'decision':'FAIL','exit_code':1,'profile':'local','achieved':False},'repository':{},'findings':[{'criterion_id':'x','status':'FAIL','expected':None,'observed':1,'impact':'blocked','next_action':'fix','next_action_code':'ACTION-FIX','kind':'PRIMARY'}],'packages':{'identities':[]},'conclusion':{}}
        self.assertTrue(any('expected' in item for item in validate_feedback(dossier)))
if __name__ == '__main__': unittest.main()
