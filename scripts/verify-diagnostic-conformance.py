#!/usr/bin/env python3
from __future__ import annotations
import json, re, sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
EXPECTED = ["DXA010","DXA011","DXA020","DXA022","DXA030","DXA040","DXA050","DXA060","DXA065","DXA070","DXA080"]
REQUIRED = {"Id","Title","Message","Category","DefaultSeverity","EnabledByDefault","Description","HelpLinkUri","Scope"}

def fail(message):
    print(f"ERROR: {message}", file=sys.stderr); raise SystemExit(1)

def ids_in(pattern, text): return sorted(set(re.findall(pattern, text)))

items = json.loads((ROOT / "docs/.generated/analyzers.json").read_text(encoding="utf-8"))
ids = [item.get("Id") for item in items]
if ids != EXPECTED: fail(f"generated IDs: {ids}")
for item in items:
    missing = REQUIRED - set(item)
    if missing: fail(f"{item.get('Id')} missing fields: {sorted(missing)}")
    if item["DefaultSeverity"] not in {"Warning","Error","Info","Hidden"}: fail(f"invalid severity for {item['Id']}")
    if not isinstance(item["EnabledByDefault"], bool): fail(f"invalid enabled state for {item['Id']}")
    if not item["Scope"]: fail(f"empty scope for {item['Id']}")

public_index = (ROOT / "docs/public/reference/diagnostics/index.md").read_text(encoding="utf-8")
if ids_in(r"\[?(DXA\d{3})\]?\(DXA\d{3}\.md\)", public_index) != EXPECTED: fail("public index ID set differs")
for item in items:
    page = (ROOT / f"docs/public/reference/diagnostics/{item['Id']}.md").read_text(encoding="utf-8")
    for marker in (f"# {item['Id']}: {item['Title']}", f"**Default severity:** {item['DefaultSeverity']}", f"**Category:** `{item['Category']}`"):
        if marker not in page: fail(f"{item['Id']} public page missing {marker}")

legacy_index = (ROOT / "docs/analyzers/index.md").read_text(encoding="utf-8")
legacy_toc = (ROOT / "docs/analyzers/toc.yml").read_text(encoding="utf-8")
router = (ROOT / "docs/when-the-compiler-fails.md").read_text(encoding="utf-8")
for label, text in (("legacy index",legacy_index),("legacy TOC",legacy_toc),("router",router)):
    shipped = [diagnostic_id for diagnostic_id in EXPECTED if diagnostic_id in text]
    if shipped != EXPECTED: fail(f"{label} ID set differs: {shipped}")

for file_name in ("CHANGELOG.md","docs/public/packages/analyzers.md","docs/public/release-notes/0.1.0-alpha.md"):
    text = (ROOT / file_name).read_text(encoding="utf-8")
    absent = [diagnostic_id for diagnostic_id in EXPECTED if diagnostic_id not in text]
    if absent: fail(f"{file_name} omits {absent}")

if "DXA090" in ids:
    fail("DXA090 appears in generated inventory")

print("PASS: descriptor-derived diagnostic metadata and inventories conform")
