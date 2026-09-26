from __future__ import annotations

import hashlib
import json
import mimetypes
import os
import shutil
import subprocess
import sys
import tempfile
from dataclasses import dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any, Iterable

from .evidence import write_json_atomic, write_text_atomic

FEEDBACK_SCHEMA = "dx.release-feedback/1.0"
CLI_RESULT_SCHEMA = "dx.release-gate.result-envelope/1.0"
ATTACHMENT_MANIFEST_SCHEMA = "dx.release-feedback.attachments/1.0"
NON_ATTENTION = {"PASS", "NOT_APPLICABLE", "NOT_YET_APPLICABLE", "ACCEPTED_INPUT"}
MANDATORY_ATTENTION = {"FAIL", "NOT_PROVEN", "EXTERNAL_EVIDENCE_REQUIRED", "HUMAN_DECISION_REQUIRED", "ERROR"}
PACKAGE_IDS = (
    "Dx.Domain.Annotations",
    "Dx.Domain.Kernel",
    "Dx.Domain.Primitives",
    "Dx.Domain.Facts",
)
DECISION_EXIT_CODES = {
    "PASS": 0, "ACCEPT_READY": 0, "READY_FOR_PUBLICATION": 0, "DONE": 0,
    "FAIL": 1, "NOT_ACCEPT_READY": 1, "NOT_READY_FOR_PUBLICATION": 1, "NOT_DONE": 1,
    "INCOMPLETE": 2, "ACCEPT_READY_NOT_PROVEN": 2, "READINESS_NOT_PROVEN": 2,
    "COMPLETION_NOT_PROVEN": 2, "ERROR": 3,
}
PROFILE_PHASE = {
    "local": "DEVELOPMENT", "ci": "DEVELOPMENT", "candidate": "ACCEPT_READY",
    "consumers": "ACCEPT_READY", "accept-ready": "ACCEPT_READY",
    "prepublish": "PREPUBLICATION", "postpublish": "POSTPUBLICATION",
}
PROFILE_OBJECTIVE = {
    "local": "Prove release-equivalent local mechanical checks without publication.",
    "ci": "Prove strict CI for one immutable commit.",
    "candidate": "Build once and validate exactly four immutable candidate packages.",
    "consumers": "Prove isolated package-backed consumption and Analyzer behavior.",
    "accept-ready": "Prove that all acceptance mechanisms are implemented and executable.",
    "prepublish": "Authorize promotion of one tagged immutable candidate.",
    "postpublish": "Prove public release completion.",
}

class FeedbackValidationError(ValueError):
    pass

@dataclass(frozen=True)
class FeedbackResult:
    mode: str
    feedback_status: str
    transport_status: str
    feedback_path: str | None = None
    carrier_path: str | None = None
    transport_error: str | None = None
    feedback_sha256: str | None = None
    validation_status: str | None = None
    carrier_sha256: str | None = None
    carrier_size: int | None = None
    carrier_verification: str | None = None
    def to_dict(self) -> dict[str, Any]:
        return self.__dict__.copy()

def _utc_now() -> str:
    return datetime.now(timezone.utc).isoformat()

def _canonical_bytes(value: Any) -> bytes:
    return (json.dumps(value, ensure_ascii=False, sort_keys=True, separators=(",", ":")) + "\n").encode("utf-8")

def _sha256_bytes(value: bytes) -> str:
    return hashlib.sha256(value).hexdigest()

def _sha256_file(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()

def _relative(path: Path, repository_root: Path) -> str:
    try:
        return path.resolve().relative_to(repository_root.resolve()).as_posix()
    except ValueError:
        return path.resolve().as_posix()

def _status_counts(criteria: Iterable[dict[str, Any]]) -> dict[str, int]:
    counts: dict[str, int] = {}
    for item in criteria:
        status = str(item.get("status") or "ERROR")
        counts[status] = counts.get(status, 0) + 1
    return dict(sorted(counts.items()))

def _proof_completeness(criteria: list[dict[str, Any]]) -> str:
    statuses = {str(item.get("status")) for item in criteria}
    if "ERROR" in statuses: return "ERROR"
    if "HUMAN_DECISION_REQUIRED" in statuses: return "HUMAN_REQUIRED"
    if "EXTERNAL_EVIDENCE_REQUIRED" in statuses: return "EXTERNAL_REQUIRED"
    if "NOT_PROVEN" in statuses: return "INCOMPLETE"
    return "COMPLETE"

def _artifact_disposition(profile: str, decision: str) -> str:
    if profile == "candidate": return "RETAIN_IMMUTABLE" if decision == "PASS" else "DISCARD_CANDIDATE"
    if profile in {"consumers", "accept-ready", "prepublish"}:
        return "PROMOTION_ELIGIBLE" if decision in {"READY_FOR_PUBLICATION"} else "PROMOTION_PROHIBITED"
    if profile == "postpublish": return "RELEASE_COMPLETE" if decision == "DONE" else "RETRY_UNCHANGED_ONLY"
    return "NONE"

def _safe_subject(value: Any) -> dict[str, Any]:
    if isinstance(value, dict): return value
    if isinstance(value, list): return {"items": value}
    return {"value": value}

def _action_code(criterion_id: str, status: str) -> str:
    base = "".join(ch if ch.isalnum() else "-" for ch in criterion_id.upper()).strip("-") or "UNIDENTIFIED"
    prefix = {"ERROR": "CORRECT-VERIFIER", "NOT_PROVEN": "PROVE", "EXTERNAL_EVIDENCE_REQUIRED": "COLLECT-EXTERNAL", "HUMAN_DECISION_REQUIRED": "OBTAIN-DECISION"}.get(status, "CORRECT")
    return f"ACTION-{prefix}-{base}"

def _criterion_finding(item: dict[str, Any], report: dict[str, Any]) -> dict[str, Any]:
    criterion_id = str(item.get("criterion_id") or "unidentified-criterion")
    status = str(item.get("status") or "ERROR")
    expected = item.get("expected")
    observed = item.get("observed")
    action = item.get("corrective_action")
    if status not in NON_ATTENTION and not action:
        action = "Inspect the decisive facts, correct the identified cause, and rerun the same profile."
    evidence = [str(value).replace("\\", "/") for value in item.get("evidence", []) if value]
    blocked_by = item.get("blocked_by")
    impact = (
        "The selected profile cannot establish its objective."
        if status in MANDATORY_ATTENTION else
        "The criterion contributes positive or non-applicable evidence to the selected profile."
    )
    return {
        "finding_id": f"finding:{criterion_id}",
        "criterion_id": criterion_id,
        "stage": item.get("stage"),
        "title": str(item.get("title") or criterion_id),
        "kind": "CONSEQUENTIAL" if blocked_by else "PRIMARY",
        "status": status,
        "severity": "ERROR" if status in {"FAIL", "ERROR"} else ("WARNING" if status not in NON_ATTENTION else "INFO"),
        "subject": _safe_subject(item.get("subject") or {"profile": report.get("profile")}),
        "occurred": str(item.get("motivation") or "The criterion did not provide a motivation."),
        "expected": expected,
        "observed": observed,
        "decisive_facts": {"expected": expected, "observed": observed},
        "provenance": evidence,
        "impact": impact,
        "blocked_by": blocked_by,
        "artifact_disposition": item.get("artifact_disposition"),
        "next_action_code": None if status in NON_ATTENTION else _action_code(criterion_id, status),
        "next_action": action,
        "verifier": item.get("verifier"),
    }

def _load_json(path: Path) -> Any | None:
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except (OSError, UnicodeDecodeError, json.JSONDecodeError):
        return None

def _package_identity(evidence_root: Path) -> tuple[list[dict[str, Any]], str | None, dict[str, Any] | None]:
    candidates = [evidence_root / "candidate-manifest.json"] + list((evidence_root / "stages").glob("**/candidate-manifest.json"))
    manifest_path = next((path for path in candidates if path.is_file()), None)
    if manifest_path is None:
        return [], None, None
    manifest = _load_json(manifest_path)
    if not isinstance(manifest, dict): return [], None, None
    packages = []
    for item in manifest.get("packages", []):
        if not isinstance(item, dict): continue
        packages.append({
            "package_id": item.get("packageId"), "version": item.get("version"),
            "filename": item.get("filename"), "byte_size": item.get("byteSize"),
            "sha256": item.get("sha256"), "signing_disposition": item.get("signingDisposition"),
        })
    packages.sort(key=lambda value: str(value.get("package_id")))
    package_set = _sha256_bytes(_canonical_bytes(packages)) if packages else None
    return packages, package_set, {"path": manifest_path.as_posix(), "sha256": _sha256_file(manifest_path)}

def _source_proof(report: dict[str, Any], evidence_root: Path) -> dict[str, Any]:
    before = _load_json(evidence_root / "git-before.json")
    after = _load_json(evidence_root / "git-after.json")
    unchanged = report.get("source_unchanged")
    if unchanged is None and isinstance(before, dict) and isinstance(after, dict):
        unchanged = before.get("head") == after.get("head") and before.get("tracked_diff") == after.get("tracked_diff")
    return {
        "source_unchanged": unchanged,
        "positive_proof": bool(isinstance(before, dict) and isinstance(after, dict)),
        "before_head": before.get("head") if isinstance(before, dict) else None,
        "after_head": after.get("head") if isinstance(after, dict) else None,
        "before_tracked_diff_sha256": _sha256_bytes(str(before.get("tracked_diff", "")).encode()) if isinstance(before, dict) else None,
        "after_tracked_diff_sha256": _sha256_bytes(str(after.get("tracked_diff", "")).encode()) if isinstance(after, dict) else None,
    }

def build_feedback(*, profile: str, decision: str, gate_exit_code: int, repository_root: Path,
                   evidence_root: Path, report: dict[str, Any]) -> dict[str, Any]:
    criteria = [item for item in report.get("criteria", []) if isinstance(item, dict)]
    # Accept-ready owns one normalized criterion stream. Project child criteria
    # without embedding child reports in the final report or handoff.
    for stage in report.get("stages", []):
        if not isinstance(stage, dict):
            continue
        stage_name = str(stage.get("name") or "unknown")
        child_report = stage.get("report")
        if isinstance(child_report, dict):
            for child in child_report.get("criteria", []):
                if not isinstance(child, dict):
                    continue
                projected = json.loads(json.dumps(child))
                projected["criterion_id"] = f"{stage_name}:{projected.get('criterion_id', 'unidentified')}"
                projected["stage"] = stage_name
                criteria.append(projected)
    findings = [_criterion_finding(item, report) for item in criteria]
    packages, package_set_identity, manifest = _package_identity(evidence_root)
    source = _source_proof(report, evidence_root)
    run = _load_json(evidence_root / "run.json") or {}
    actions = []
    for finding in findings:
        if finding["next_action_code"]:
            actions.append({"action_code": finding["next_action_code"], "instruction": finding["next_action"], "criterion_id": finding["criterion_id"]})
    blocked_work = []
    for finding in findings:
        if finding["blocked_by"]:
            blocked_work.append({"operation": finding["criterion_id"], "blocked_by": finding["blocked_by"], "impact": finding["impact"], "resumption_condition": finding["next_action"]})
    dossier = {
        "schema": {"name": FEEDBACK_SCHEMA, "major": 1, "minor": 0},
        "producer": {"name": "Dx.Domain release gate", "component": "release_gate.feedback"},
        "run": {"run_id": report.get("run_id", evidence_root.name), "created_utc": _utc_now(), "platform": run.get("platform"), "python": run.get("python")},
        "repository": {"commit": report.get("head") or run.get("head"), "branch": report.get("branch") or run.get("branch"), "tag": run.get("tag"), "working_tree": run.get("initial_status"), **source},
        "gate": {"profile": profile, "phase": PROFILE_PHASE.get(profile, "UNKNOWN"), "release_state": decision, "decision": decision, "exit_code": gate_exit_code, "objective": PROFILE_OBJECTIVE.get(profile, "Execute the selected release-gate profile."), "achieved": gate_exit_code == 0, "proof_completeness": _proof_completeness(criteria)},
        "conclusion": {"summary": _summary(profile, decision), "impact": "The profile objective is established." if gate_exit_code == 0 else "The profile objective is not established.", "artifact_disposition": _artifact_disposition(profile, decision)},
        "status_counts": _status_counts(criteria),
        "findings": findings,
        "blocked_work": blocked_work,
        "actions": actions,
        "packages": {"required_package_ids": list(PACKAGE_IDS), "identities": packages, "package_set_identity": package_set_identity, "candidate_manifest": manifest},
        "continuity": [
            {key: value for key, value in stage.items() if key != "report"}
            for stage in report.get("stages", []) if isinstance(stage, dict)
        ],
        "attachments": [],
        "validation": {"status": "PENDING", "errors": [], "dossier_sha256": None},
    }
    return dossier

def _summary(profile: str, decision: str) -> str:
    subject = "Accept-ready" if profile == "accept-ready" else f"{profile.capitalize()} release-gate"
    disposition = "passed" if decision in {"PASS", "ACCEPT_READY", "READY_FOR_PUBLICATION", "DONE"} else ("was not proven" if "NOT_PROVEN" in decision or decision == "INCOMPLETE" else ("encountered a verifier or environment error" if decision == "ERROR" else "failed"))
    return f"{subject} verification {disposition}."

def validate_feedback(dossier: dict[str, Any]) -> list[str]:
    errors: list[str] = []
    gate = dossier.get("gate", {})
    decision = gate.get("decision")
    expected_exit = DECISION_EXIT_CODES.get(str(decision))
    if expected_exit is None: errors.append(f"unknown decision: {decision!r}")
    elif gate.get("exit_code") != expected_exit: errors.append(f"decision {decision} requires exit code {expected_exit}, observed {gate.get('exit_code')}")
    findings = dossier.get("findings")
    if not isinstance(findings, list): errors.append("findings must be an array"); findings = []
    for finding in findings:
        status = finding.get("status")
        if status in MANDATORY_ATTENTION:
            for field in ("expected", "observed"):
                if field not in finding or finding[field] is None:
                    errors.append(
                        f"{finding.get('criterion_id')}: "
                        f"material non-success lacks {field}"
                    )
            for field in ("impact", "next_action", "next_action_code"):
                if finding.get(field) in (None, ""):
                    errors.append(
                        f"{finding.get('criterion_id')}: "
                        f"material non-success lacks {field}"
                    )
        if finding.get("kind") == "CONSEQUENTIAL" and not finding.get("blocked_by"):
            errors.append(f"{finding.get('criterion_id')}: consequential finding lacks blocker")
    repo = dossier.get("repository", {})
    if repo.get("source_unchanged") is True and not repo.get("positive_proof"):
        errors.append("source_unchanged=true lacks positive before/after proof")
    profile = gate.get("profile")
    if gate.get("achieved") and profile in {"candidate", "consumers", "accept-ready", "prepublish", "postpublish"}:
        identities = dossier.get("packages", {}).get("identities", [])
        ids = {item.get("package_id") for item in identities}
        if ids != set(PACKAGE_IDS): errors.append("candidate-or-later success lacks the complete four-package identity set")
        if not dossier.get("packages", {}).get("package_set_identity"): errors.append("candidate-or-later success lacks package-set identity")
    if decision == "ACCEPT_READY" and gate.get("release_state") == "DONE": errors.append("ACCEPT_READY is presented as DONE")
    disposition = dossier.get("conclusion", {}).get("artifact_disposition")
    if disposition == "PARTIAL_PUBLICATION" and gate.get("achieved"): errors.append("partial publication is presented as success")
    return errors

def _finalize(dossier: dict[str, Any]) -> dict[str, Any]:
    candidate = json.loads(json.dumps(dossier))
    candidate["validation"] = {"status": "PENDING", "errors": [], "dossier_sha256": None}
    errors = validate_feedback(candidate)
    candidate["validation"] = {"status": "PASS" if not errors else "FAIL", "errors": errors, "dossier_sha256": None}
    unsigned = json.loads(json.dumps(candidate)); unsigned["validation"]["dossier_sha256"] = None
    candidate["validation"]["dossier_sha256"] = _sha256_bytes(_canonical_bytes(unsigned))
    if errors: raise FeedbackValidationError("; ".join(errors))
    return candidate

def render_markdown(dossier: dict[str, Any]) -> str:
    gate = dossier["gate"]; repo = dossier["repository"]; conclusion = dossier["conclusion"]
    lines = ["# Dx.Domain release-gate feedback", "", f"- Run: `{dossier['run']['run_id']}`", f"- Profile: `{gate['profile']}`", f"- Commit: `{repo.get('commit')}`", f"- Decision: **{gate['decision']}**", f"- Exit code: `{gate['exit_code']}`", f"- Proof completeness: `{gate['proof_completeness']}`", f"- Artifact disposition: `{conclusion['artifact_disposition']}`", "", "## Conclusion", "", conclusion["summary"], "", "## Decisive findings", ""]
    material = [f for f in dossier["findings"] if f["status"] not in NON_ATTENTION]
    if not material: lines.append("No actionable non-success findings.")
    for f in material:
        lines += [f"### {f['status']} `{f['criterion_id']}`", "", f["occurred"], "", f"- Expected: `{json.dumps(f['expected'], ensure_ascii=False, sort_keys=True)}`", f"- Observed: `{json.dumps(f['observed'], ensure_ascii=False, sort_keys=True)}`", f"- Impact: {f['impact']}", f"- Next action `{f['next_action_code']}`: {f['next_action']}", f"- Provenance: {', '.join(f['provenance']) if f['provenance'] else 'embedded facts only'}", ""]
    lines += ["## Package identity", "", f"- Package-set identity: `{dossier['packages']['package_set_identity']}`"]
    for item in dossier["packages"]["identities"]:
        lines.append(f"- `{item['package_id']}` `{item['version']}` `{item['filename']}` {item['byte_size']} bytes SHA-256 `{item['sha256']}` signing `{item['signing_disposition']}`")
    lines += ["", "## Validation", "", f"- Status: `{dossier['validation']['status']}`", f"- Dossier SHA-256: `{dossier['validation']['dossier_sha256']}`", ""]
    return "\n".join(lines)

def _attachment_candidates(evidence_root: Path, dossier: dict[str, Any]) -> list[tuple[str, str, Path]]:
    wanted: dict[Path, tuple[str, str]] = {}
    for name, purpose in (("run.json", "run identity"), ("report.json", "authoritative execution report"), ("criteria.json", "criterion execution facts"), ("git-before.json", "source immutability before state"), ("git-after.json", "source immutability after state"), ("candidate-manifest.json", "candidate identity")):
        path = evidence_root / name
        if path.is_file(): wanted[path] = (name.replace(".", "-"), purpose)
    for finding in dossier["findings"]:
        if finding["status"] in NON_ATTENTION: continue
        for ref in finding.get("provenance", []):
            path = Path(ref)
            if not path.is_absolute(): path = evidence_root / path
            if path.is_file() and path.stat().st_size <= 2 * 1024 * 1024:
                wanted[path] = (f"evidence-{_sha256_bytes(path.as_posix().encode())[:12]}", f"decisive evidence for {finding['criterion_id']}")
    return [(identity, purpose, path) for path, (identity, purpose) in sorted(wanted.items(), key=lambda item: item[0].as_posix())]

def _collector_command() -> list[str]:
    """Return the repository-owned DX v2.0 CLI command prefix."""
    collector = Path(__file__).resolve().parents[1] / "dx.py"
    if not collector.is_file():
        raise RuntimeError(f"Repository-owned DX collector is unavailable: {collector}")
    return [sys.executable, str(collector)]

def _carrier_path(profile: str, decision: str, run_id: str) -> Path:
    transfer = os.environ.get("DX")
    if not transfer: raise RuntimeError("DX is not set; cannot create transferable feedback carrier")
    return Path(transfer).resolve() / f"release-gate-{profile}-{decision.lower().replace('_','-')}-{run_id}.dx.txt"

def _resolve_evidence(reference: str, evidence_root: Path) -> Path | None:
    candidate = Path(reference)
    candidates = [candidate] if candidate.is_absolute() else [evidence_root / candidate, evidence_root.parent / candidate]
    return next((path for path in candidates if path.is_file()), None)

def _transport_path(staging: Path, finding: dict[str, Any], source: Path, ordinal: int) -> str:
    safe_id = "".join(ch if ch.isalnum() or ch in "-_" else "-" for ch in str(finding["finding_id"]))
    name = source.name if ordinal == 0 else f"{ordinal + 1}-{source.name}"
    target = staging / "release-gate" / "findings" / safe_id / name
    target.parent.mkdir(parents=True, exist_ok=True)
    shutil.copyfile(source, target)
    return target.relative_to(staging).as_posix()

def _stage_summaries(dossier: dict[str, Any]) -> list[dict[str, Any]]:
    return [dict(item) for item in dossier.get("continuity", []) if isinstance(item, dict)]

def _handoff(dossier: dict[str, Any], carrier_name: str) -> dict[str, Any]:
    material = [item for item in dossier["findings"] if item["status"] not in NON_ATTENTION]
    return {
        "schema": "dx-domain.release-gate-handoff/1.0",
        "carrier": {"format": "DX v2.0", "filename": carrier_name,
                    "payload_manifest_sha256": None, "verification": "VERIFIED"},
        "execution": {"run_id": dossier["run"]["run_id"], "profile": dossier["gate"]["profile"],
                      "operation_status": "COMPLETED", "decision": dossier["gate"]["decision"],
                      "exit_code": dossier["gate"]["exit_code"]},
        "repository": {key: dossier["repository"].get(key) for key in ("commit", "branch", "source_unchanged")},
        "summary": {"objective_achieved": dossier["gate"]["achieved"],
                    "primary_failure_count": sum(item["kind"] == "PRIMARY" for item in material),
                    "actionable_finding_count": len(material),
                    "blocked_operation_count": len(dossier["blocked_work"]),
                    "status_counts": dossier["status_counts"]},
        "findings": material,
        "blocked_work": dossier["blocked_work"], "next_actions": dossier["actions"],
        "artifacts": {"packages": dossier["packages"]},
        "evidence_index": sorted({ref for item in material for ref in item.get("provenance", [])}),
        "continuation": {"recommended_action": "CONTINUE" if dossier["gate"]["achieved"] else "CORRECT_AND_RERUN",
                         "rerun_profile": dossier["gate"]["profile"],
                         "preserve_candidate": dossier["conclusion"]["artifact_disposition"] in {"RETAIN_IMMUTABLE", "PROMOTION_PROHIBITED"}},
        "validation": dossier["validation"],
    }

def _payload_hash(release_root: Path) -> str:
    entries = []
    for path in sorted(item for item in release_root.rglob("*") if item.is_file() and item.name != "handoff.json"):
        entries.append({"path": path.relative_to(release_root.parent).as_posix(), "size": path.stat().st_size,
                        "sha256": _sha256_file(path)})
    return _sha256_bytes(_canonical_bytes(entries))

def _package_feedback(repository_root: Path, evidence_root: Path, profile: str, decision: str, run_id: str, dossier: dict[str, Any]) -> Path:
    command = _collector_command()
    carrier = _carrier_path(profile, decision, run_id)
    carrier.parent.mkdir(parents=True, exist_ok=True); carrier.unlink(missing_ok=True)
    with tempfile.TemporaryDirectory(prefix="dx-release-gate-feedback-") as temporary:
        staging = Path(temporary); release = staging / "release-gate"; release.mkdir(parents=True)
        transported = json.loads(json.dumps(dossier))
        for finding in transported["findings"]:
            if finding["status"] in NON_ATTENTION: continue
            paths = []
            for ordinal, reference in enumerate(finding.get("provenance", [])):
                source = _resolve_evidence(reference, evidence_root)
                if source is not None and source.stat().st_size <= 2 * 1024 * 1024:
                    paths.append(_transport_path(staging, finding, source, ordinal))
            finding["provenance"] = paths
            safe_id = "".join(ch if ch.isalnum() or ch in "-_" else "-" for ch in finding["finding_id"])
            write_json_atomic(release / "findings" / safe_id / "finding.json", finding)
        index = [{"finding_id": item["finding_id"], "path": "release-gate/findings/" +
                  "".join(ch if ch.isalnum() or ch in "-_" else "-" for ch in item["finding_id"]) + "/finding.json"}
                 for item in transported["findings"] if item["status"] not in NON_ATTENTION]
        if index: write_json_atomic(release / "findings" / "index.json", {"findings": index})
        for name in ("report.json", "criteria.json", "run.json"):
            source = evidence_root / name
            if source.is_file(): shutil.copyfile(source, release / name)
        for name in ("git-before.json", "git-after.json"):
            source = evidence_root / name
            if source.is_file():
                target = release / "source-state" / name; target.parent.mkdir(parents=True, exist_ok=True); shutil.copyfile(source, target)
        manifest = dossier.get("packages", {}).get("candidate_manifest")
        if isinstance(manifest, dict) and manifest.get("path"):
            source = Path(manifest["path"])
            if source.is_file():
                target = release / "candidate" / "candidate-manifest.json"; target.parent.mkdir(parents=True, exist_ok=True); shutil.copyfile(source, target)
        for stage in _stage_summaries(transported): write_json_atomic(release / "stages" / f"{stage['name']}.json", stage)
        write_text_atomic(release / "summary.md", render_markdown(transported))
        handoff = _handoff(transported, carrier.name); handoff["carrier"]["payload_manifest_sha256"] = _payload_hash(release)
        write_json_atomic(release / "handoff.json", handoff)
        completed = subprocess.run([*command, "pack", str(staging), "--root", str(staging), "-o", str(carrier)], cwd=repository_root, text=True, encoding="utf-8", errors="replace", stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False, shell=False)
        if completed.returncode != 0: raise RuntimeError(f"DX collector exited with {completed.returncode}: {(completed.stderr or completed.stdout).strip()}")
    if not carrier.is_file() or not carrier.stat().st_size: raise RuntimeError(f"DX collector did not create a non-empty carrier: {carrier}")
    inspect = subprocess.run([*command, "inspect", str(carrier), "--verify"], cwd=repository_root, text=True, encoding="utf-8", errors="replace", stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False, shell=False)
    if inspect.returncode != 0:
        carrier.unlink(missing_ok=True); raise RuntimeError(f"DX post-pack verification failed with {inspect.returncode}: {(inspect.stderr or inspect.stdout).strip()}")
    return carrier

def transport_feedback(
    *,
    profile: str,
    decision: str,
    repository_root: Path,
    evidence_root: Path,
    dossier: dict[str, Any],
) -> FeedbackResult:
    finalized = _finalize(dossier)
    feedback_path = evidence_root / "feedback.json"
    write_json_atomic(feedback_path, finalized)
    write_text_atomic(
        evidence_root / "feedback.md",
        render_markdown(finalized),
    )
    digest = _sha256_file(feedback_path)

    try:
        carrier = _package_feedback(
            repository_root,
            evidence_root,
            profile,
            decision,
            str(finalized["run"]["run_id"]),
            finalized,
        )
    except (OSError, RuntimeError, ValueError) as exc:
        return FeedbackResult(
            "dx",
            "CREATED",
            "FAILED",
            _relative(feedback_path, repository_root),
            transport_error=str(exc),
            feedback_sha256=digest,
            validation_status="PASS",
        )

    return FeedbackResult(
        "dx",
        "CREATED",
        "CREATED",
        _relative(feedback_path, repository_root),
        carrier.as_posix(),
        feedback_sha256=digest,
        validation_status="PASS",
        carrier_sha256=_sha256_file(carrier),
        carrier_size=carrier.stat().st_size,
        carrier_verification="PASS",
    )


def collect_feedback(*, mode: str, profile: str, decision: str, gate_exit_code: int, repository_root: Path, evidence_root: Path, report: dict[str, Any]) -> FeedbackResult:
    if mode == "none": return FeedbackResult(mode, "NOT_REQUESTED", "NOT_REQUESTED")
    dossier = _finalize(build_feedback(profile=profile, decision=decision, gate_exit_code=gate_exit_code, repository_root=repository_root, evidence_root=evidence_root, report=report))
    feedback_path = evidence_root / "feedback.json"; write_json_atomic(feedback_path, dossier)
    write_text_atomic(evidence_root / "feedback.md", render_markdown(dossier))
    digest = _sha256_file(feedback_path)
    if mode == "summary": return FeedbackResult(mode, "CREATED", "NOT_REQUESTED", _relative(feedback_path, repository_root), feedback_sha256=digest, validation_status="PASS")
    try:
        carrier = _package_feedback(repository_root, evidence_root, profile, decision, str(dossier["run"]["run_id"]), dossier)
    except (OSError, RuntimeError, ValueError) as exc:
        return FeedbackResult(mode, "CREATED", "FAILED", _relative(feedback_path, repository_root), transport_error=str(exc), feedback_sha256=digest, validation_status="PASS")
    return FeedbackResult(mode, "CREATED", "CREATED", _relative(feedback_path, repository_root), carrier.as_posix(), feedback_sha256=digest, validation_status="PASS", carrier_sha256=_sha256_file(carrier), carrier_size=carrier.stat().st_size, carrier_verification="PASS")

def build_error_feedback(*, profile: str, repository_root: Path, evidence_root: Path, error: BaseException) -> dict[str, Any]:
    report = {"run_id": evidence_root.name, "profile": profile, "head": None, "branch": None, "source_unchanged": None, "criteria": [{"criterion_id": "release-gate-operational-error", "title": "Release-gate operational error", "status": "ERROR", "motivation": f"{type(error).__name__}: {error}", "verifier": "release_gate.run", "expected": {"operation": "complete selected profile"}, "observed": {"error_type": type(error).__name__, "message": str(error)}, "evidence": ["error.json"], "corrective_action": "Inspect the embedded error facts and error.json, correct the verifier or environment failure, and rerun."}]}
    return _finalize(build_feedback(profile=profile, decision="ERROR", gate_exit_code=3, repository_root=repository_root, evidence_root=evidence_root, report=report))

def cli_feedback(feedback_path: Path) -> str:
    try:
        dossier = json.loads(
            feedback_path.read_text(encoding="utf-8")
        )
    except (OSError, UnicodeDecodeError, json.JSONDecodeError) as exc:
        raise RuntimeError(
            f"Could not read validated feedback from "
            f"{feedback_path}: {exc}"
        ) from exc

    if not isinstance(dossier, dict):
        raise RuntimeError(
            f"Validated feedback root must be an object: "
            f"{feedback_path}"
        )

    validation = dossier.get("validation")
    if not isinstance(validation, dict):
        raise RuntimeError(
            f"Validated feedback lacks validation metadata: "
            f"{feedback_path}"
        )
    if validation.get("status") != "PASS":
        raise RuntimeError(
            f"Feedback is not validated for console transport: "
            f"{feedback_path}"
        )

    return (
        "DX_RELEASE_GATE_FEEDBACK="
        + json.dumps(
            dossier,
            ensure_ascii=False,
            separators=(",", ":"),
            sort_keys=True,
        )
    )


def cli_result(*, profile: str, decision: str, gate_exit_code: int, process_exit_code: int, evidence_root: Path | None, repository_root: Path | None, result: FeedbackResult) -> str:
    payload = {"schema": CLI_RESULT_SCHEMA, "profile": profile, "decision": decision, "gate_exit_code": gate_exit_code, "process_exit_code": process_exit_code, "feedback": result.mode, "feedback_status": result.feedback_status, "feedback_validation": result.validation_status, "feedback_sha256": result.feedback_sha256, "transport_status": result.transport_status, "evidence_directory": _relative(evidence_root, repository_root) if evidence_root is not None and repository_root is not None else None, "feedback_path": result.feedback_path, "carrier": result.carrier_path, "carrier_sha256": result.carrier_sha256, "carrier_size": result.carrier_size, "carrier_verification": result.carrier_verification, "transport_error": result.transport_error}
    return "DX_RELEASE_GATE_RESULT=" + json.dumps(payload, ensure_ascii=False, separators=(",", ":"), sort_keys=True)
