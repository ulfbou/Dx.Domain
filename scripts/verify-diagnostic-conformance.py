#!/usr/bin/env python3
from __future__ import annotations
import json, re, sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
EXPECTED=["DXA010","DXA011","DXA020","DXA022","DXA030","DXA040","DXA050","DXA060","DXA065","DXA070","DXA080"]
FIELDS={"Id","Title","Message","Category","DefaultSeverity","EnabledByDefault","Description","HelpLinkUri"}
def fail(m): print("ERROR: "+m,file=sys.stderr); raise SystemExit(1)
def one(pattern,text,label):
    found=re.findall(pattern,text,re.S)
    if len(found)!=1: fail(f"expected one {label}, found {len(found)}")
    return found[0]
def value(name,text):
    raw=one(rf'(?:const string|LocalizableString)\s+{name}\s*=\s*"((?:\\.|[^"\\])*)"\s*;',text,name)
    return bytes(raw,"utf-8").decode("unicode_escape")
def descriptor(path):
    text=path.read_text(encoding="utf-8")
    links=re.findall(r'helpLinkUri:\s*"((?:\\.|[^"\\])*)"',text)
    return {"Id":one(r'const string DiagnosticId\s*=\s*"(DXA\d{3})"',text,"ID"),"Title":value("Title",text),"Message":value("MessageFormat",text),"Category":value("Category",text),"DefaultSeverity":one(r'DiagnosticSeverity\.(\w+)',text,"severity"),"EnabledByDefault":one(r'isEnabledByDefault:\s*(true|false)',text,"enabled")=="true","Description":value("Description",text),"HelpLinkUri":bytes(links[0],"utf-8").decode("unicode_escape") if links else None}
items=json.loads((ROOT/"docs/.generated/analyzers.json").read_text(encoding="utf-8"))
if [x.get("Id") for x in items]!=EXPECTED: fail("generated ID set differs")
for item in items:
    if set(item)!=FIELDS: fail(f"{item.get('Id')} fields differ: {sorted(set(item)^FIELDS)}")
sources=[]
for path in sorted((ROOT/"src/Dx.Domain.Analyzers/Analyzers").glob("DXA[0-9][0-9][0-9]_*.cs")): sources.append(descriptor(path))
sources.sort(key=lambda x:x["Id"])
if sources!=items:
    for actual,generated in zip(sources,items):
        if actual!=generated: print(f"MISMATCH {actual['Id']}: source={actual!r} generated={generated!r}",file=sys.stderr)
    fail("generated metadata differs from descriptors")
for item in items:
    did=item["Id"]
    page=(ROOT/f"docs/public/reference/diagnostics/{did}.md").read_text(encoding="utf-8")
    for marker in (f"# {did}: {item['Title']}",f"**Default severity:** {item['DefaultSeverity']}",f"**Category:** `{item['Category']}`"):
        if marker not in page: fail(f"{did} public page missing {marker}")
for name in ("docs/public/reference/diagnostics/index.md","docs/analyzers/index.md","docs/analyzers/toc.yml","docs/when-the-compiler-fails.md","CHANGELOG.md","docs/public/packages/analyzers.md","docs/public/release-notes/0.1.0-alpha.md"):
    text=(ROOT/name).read_text(encoding="utf-8")
    missing=[did for did in EXPECTED if did not in text]
    if missing: fail(f"{name} omits {missing}")
if any(x["Id"]=="DXA090" for x in items): fail("DXA090 is shipped")
print("PASS: descriptor metadata and diagnostic inventories conform")
