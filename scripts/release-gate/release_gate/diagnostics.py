from __future__ import annotations
from collections import Counter
from pathlib import Path
import re
from .model import CriterionResult, CriterionStatus, Diagnostic

PATTERN = re.compile(r'(?:(?P<file>[^\r\n:(]+)\((?P<line>\d+)(?:,\d+)?\):\s*)?(?P<severity>warning|error|info)\s+(?P<id>DXA\d{3,}|AD0001)\s*:\s*(?P<message>.*?)(?:\s+\[[^\]]+\])?$', re.IGNORECASE | re.MULTILINE)

def parse_diagnostics_from_build_output(build_stdout: str, build_binlog_path: Path | None = None) -> list[Diagnostic]:
    text = build_stdout
    if build_binlog_path and Path(build_binlog_path).is_file():
        text += "\n" + Path(build_binlog_path).read_text(encoding="utf-8", errors="strict")
    return [Diagnostic(m.group("id").upper(), m.group("severity").lower(), m.group("message").strip(), m.group("file"), int(m.group("line")) if m.group("line") else None) for m in PATTERN.finditer(text)]

def _key(item): return (item.id, item.file or "", item.line or 0, item.message)

def count_effective_diagnostics(diagnostics):
    raw = Counter(item.id for item in diagnostics)
    unique = Counter(item.id for item in {_key(x):x for x in diagnostics}.values())
    return {"raw":dict(raw), "unique":dict(unique), "duplicates":{key:raw[key]-unique[key] for key in raw if raw[key] > unique[key]}}

def validate_no_duplicate_execution(combined_diagnostics, single_diagnostics):
    combined = count_effective_diagnostics(combined_diagnostics); single = count_effective_diagnostics(single_diagnostics)
    return not combined["duplicates"] and combined["unique"] == single["unique"]

def _result(identifier, passed, expected, observed):
    return CriterionResult(identifier, identifier, CriterionStatus.PASS if passed else CriterionStatus.FAIL, "Analyzer evidence matched the contract." if passed else "Analyzer evidence contradicted the contract.", "release_gate.diagnostics", [], expected, observed, None if passed else "Correct Analyzer packaging or fixture and rerun.")

def validate_analyzer_activation(case, diagnostics, build_result, single_diagnostics=None, build_text=""):
    expected = case.get("expects", {}); identifiers = [item.id for item in diagnostics]; results = []
    prefix = case.get("id", "unknown-case")
    scoped = lambda value: f"{prefix}:{value}"
    if expected.get("analyzer") == "must_report":
        wanted = expected.get("diagnosticIds", ["DXA065"]); results.append(_result(scoped("consumer-analyzer-activated"), all(x in identifiers for x in wanted), wanted, identifiers))
    load_failed = "AD0001" in identifiers or ("analyzer assembly" in build_text.lower() and "failed" in build_text.lower())
    results.append(_result(scoped("consumer-analyzer-load-failure-free"), not load_failed, False, load_failed))
    counts = count_effective_diagnostics(diagnostics); results.append(_result(scoped("consumer-effective-diagnostic-uniqueness"), not counts["duplicates"], {}, counts["duplicates"]))
    if case.get("carrier") == "combined" and single_diagnostics is not None:
        results.append(_result(scoped("consumer-analyzer-duplicate-boundary"), validate_no_duplicate_execution(diagnostics, single_diagnostics), count_effective_diagnostics(single_diagnostics), counts))
    return results
