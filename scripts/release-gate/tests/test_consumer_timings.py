import json
import sys
import tempfile
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))

from release_gate.consumer_timings import ConsumerStageTimings


class ConsumerStageTimingTests(unittest.TestCase):
    def case(self, identifier, action="run", target="net8.0"):
        return {"id": identifier, "targetFramework": target, "action": action}

    def test_aggregation_uses_process_result_durations(self):
        timings = ConsumerStageTimings()
        first = timings.begin_case(self.case("first"))
        first.restore_seconds = 1.25
        first.build_seconds = 2.5
        first.run_seconds = 0.75
        second = timings.begin_case(self.case("second", action="build"))
        second.restore_seconds = 3.0
        second.build_seconds = 4.0
        summary = timings.summary()
        self.assertEqual(4.25, summary["restore_seconds"])
        self.assertEqual(6.5, summary["build_seconds"])
        self.assertEqual(0.75, summary["run_seconds"])

    def test_build_only_run_duration_is_explicitly_absent(self):
        timings = ConsumerStageTimings()
        case = timings.begin_case(self.case("build-only", action="build"))
        case.restore_seconds = 1.0
        case.build_seconds = 2.0
        entry = timings.summary()["cases"][0]
        self.assertIsNone(entry["run_seconds"])
        self.assertEqual(0.0, timings.summary()["run_seconds"])

    def test_analyzer_parse_and_evidence_durations_are_recorded(self):
        timings = ConsumerStageTimings()
        case = timings.begin_case(self.case("analyzer", action="build"))
        case.diagnostic_parse_seconds = 0.125
        case.evidence_seconds = 0.25
        summary = timings.summary()
        self.assertEqual(0.125, summary["diagnostic_parse_seconds"])
        self.assertEqual(0.25, summary["evidence_seconds"])

    def test_runtime_case_has_no_analyzer_diagnostic_work(self):
        timings = ConsumerStageTimings()
        timings.begin_case(self.case("runtime"))
        entry = timings.summary()["cases"][0]
        self.assertEqual(0.0, entry["diagnostic_parse_seconds"])
        self.assertEqual(0.0, entry["evidence_seconds"])

    def test_every_case_appears_once_in_deterministic_order(self):
        timings = ConsumerStageTimings()
        timings.begin_case(self.case("z-case", target="net10.0"))
        timings.begin_case(self.case("a-case", action="build"))
        with self.assertRaises(ValueError):
            timings.begin_case(self.case("a-case", action="build"))
        summary = timings.summary()
        self.assertEqual(["a-case", "z-case"], [item["case_id"] for item in summary["cases"]])
        self.assertEqual(
            json.dumps(summary, sort_keys=True),
            json.dumps(timings.summary(), sort_keys=True),
        )

    def test_timings_are_observational_and_contain_no_decision(self):
        timings = ConsumerStageTimings()
        case = timings.begin_case(self.case("slow"))
        case.restore_seconds = 999999.0
        serialized = json.dumps(timings.summary(), sort_keys=True)
        self.assertNotIn("criterion", serialized.lower())
        self.assertNotIn("decision", serialized.lower())
        self.assertNotIn("pass", serialized.lower())
        self.assertNotIn("fail", serialized.lower())

    def test_written_json_has_required_schema_and_shape(self):
        timings = ConsumerStageTimings()
        timings.begin_case(self.case("case"))
        with tempfile.TemporaryDirectory() as value:
            path = Path(value) / "consumer-stage-timings.json"
            timings.write(path)
            loaded = json.loads(path.read_text(encoding="utf-8"))
        self.assertEqual("dx-domain.consumer-stage-timings/1.0", loaded["schema"])
        self.assertEqual(1, len(loaded["cases"]))


if __name__ == "__main__":
    unittest.main()
