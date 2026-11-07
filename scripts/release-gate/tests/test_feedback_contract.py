import json
import sys
import tempfile
import unittest
from pathlib import Path
ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))
from release_gate.feedback import FeedbackValidationError, build_feedback, validate_feedback

class FeedbackContractTests(unittest.TestCase):
    def base(self, root):
        evidence = root / '.dx/verification/release-gate/run-1'; evidence.mkdir(parents=True)
        (evidence / 'run.json').write_text(json.dumps({'run_id':'run-1','head':'a'*40,'branch':'test','initial_status':[]}), encoding='utf-8')
        (evidence / 'git-before.json').write_text(json.dumps({'head':'a'*40,'tracked_diff':''}), encoding='utf-8')
        (evidence / 'git-after.json').write_text(json.dumps({'head':'a'*40,'tracked_diff':''}), encoding='utf-8')
        return evidence
    def test_source_unchanged_requires_positive_proof(self):
        dossier={'gate':{'decision':'PASS','exit_code':0,'profile':'local','achieved':True},'repository':{'source_unchanged':True,'positive_proof':False},'findings':[],'packages':{'identities':[]},'conclusion':{}}
        self.assertTrue(any('positive' in item for item in validate_feedback(dossier)))
    def test_candidate_success_requires_four_identities(self):
        dossier={'gate':{'decision':'PASS','exit_code':0,'profile':'candidate','achieved':True},'repository':{},'findings':[],'packages':{'identities':[]},'conclusion':{}}
        self.assertTrue(any('four-package' in item for item in validate_feedback(dossier)))
    def test_missing_expected_rejected(self):
        dossier={'gate':{'decision':'FAIL','exit_code':1,'profile':'local','achieved':False},'repository':{},'findings':[{'criterion_id':'x','status':'FAIL','expected':None,'observed':1,'impact':'blocked','next_action':'fix','next_action_code':'ACTION-FIX','kind':'PRIMARY'}],'packages':{'identities':[]},'conclusion':{}}
        self.assertTrue(any('expected' in item for item in validate_feedback(dossier)))
    def test_empty_observed_collection_is_relevant_evidence(self):
        dossier = {
            'gate': {
                'decision': 'FAIL',
                'exit_code': 1,
                'profile': 'consumers',
                'achieved': False,
            },
            'repository': {},
            'findings': [
                {
                    'criterion_id': 'consumer-analyzer-activated',
                    'status': 'FAIL',
                    'expected': ['DXA065'],
                    'observed': [],
                    'impact': 'The expected diagnostic was not emitted.',
                    'next_action': (
                        'Correct Analyzer activation and rerun.'
                    ),
                    'next_action_code': (
                        'ACTION-CORRECT-CONSUMER-ANALYZER-ACTIVATED'
                    ),
                    'kind': 'PRIMARY',
                }
            ],
            'packages': {'identities': []},
            'conclusion': {},
        }

        errors = validate_feedback(dossier)

        self.assertFalse(
            any('lacks observed' in error for error in errors),
            errors,
        )

    def test_absent_observed_field_is_rejected(self):
        dossier = {
            'gate': {
                'decision': 'FAIL',
                'exit_code': 1,
                'profile': 'consumers',
                'achieved': False,
            },
            'repository': {},
            'findings': [
                {
                    'criterion_id': 'consumer-analyzer-activated',
                    'status': 'FAIL',
                    'expected': ['DXA065'],
                    'impact': 'The expected diagnostic was not emitted.',
                    'next_action': (
                        'Correct Analyzer activation and rerun.'
                    ),
                    'next_action_code': (
                        'ACTION-CORRECT-CONSUMER-ANALYZER-ACTIVATED'
                    ),
                    'kind': 'PRIMARY',
                }
            ],
            'packages': {'identities': []},
            'conclusion': {},
        }

        errors = validate_feedback(dossier)

        self.assertTrue(
            any('lacks observed' in error for error in errors),
            errors,
        )

if __name__ == '__main__': unittest.main()
