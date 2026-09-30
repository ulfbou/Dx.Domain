#!/usr/bin/env python3
from pathlib import Path
import json,re,sys
R=Path(__file__).resolve().parents[1]; errors=[]
def fail(s): errors.append(s)
def txt(p): return (R/p).read_text(encoding="utf-8",errors="replace")
cfg=json.loads(txt("docs/docfx.json")); raw=json.dumps(cfg)
if ".generated/**" not in raw: fail("DocFX configuration does not exclude generated docs")
if "docs/public" in raw: fail("DocFX configuration still references docs/public")
required=["docs/index.md","docs/use/index.md","docs/use/getting-started.md","docs/use/quickstart.md","docs/reference/index.md","docs/reference/packages/index.md","docs/reference/diagnostics/index.md","docs/use/stability.md","docs/use/limitations.md","docs/use/troubleshooting.md"]
for s in required:
 if not (R/s).exists(): fail("missing canonical route: "+s)
for p in (R/"docs").rglob("*.md"):
 if not p.read_text(encoding="utf-8").strip(): fail("blank docs page: "+str(p.relative_to(R)))
 link=re.compile(r"(?<!!)\[[^]]*\]\(([^)]+)\)")
 for t in link.findall(txt(p.relative_to(R))):
  t=t.strip().split()[0].strip("<>").split("#",1)[0]
  if t and not re.match(r"^[a-z]+:",t,re.I) and not t.startswith("/") and not (p.parent/t).exists(): fail(f"broken link: {p.relative_to(R)} -> {t}")
for p in (R/"docs").rglob("toc.yml"):
 for t in re.findall(r"^[ \t]*href:[ \t]*['\"]?([^'\" #\r\n]+)",txt(p.relative_to(R)),re.M):
  if not re.match(r"^[a-z]+:",t,re.I) and not (p.parent/t).exists(): fail(f"broken TOC: {p.relative_to(R)} -> {t}")
docs="\n".join(p.read_text(encoding="utf-8",errors="replace") for p in (R/"docs").rglob("*.md"))
if re.search(r"(?i)cannot be suppressed|cannot suppress",docs): fail("absolute suppression prohibition remains")
if re.search(r"(?i)(kernel|public api).{0,40}(is|are) frozen",docs): fail("frozen API claim remains")
g=json.dumps(json.loads(txt("docs/.generated/analyzers.json")))
if "DXA065" not in g: fail("DXA065 absent from generated inventory")
if "DXA090" in g: fail("DXA090 appears in generated shipped inventory")
if errors:
 print("FAIL: final conformance structural checks",file=sys.stderr)
 for e in errors: print(" - "+e,file=sys.stderr)
 raise SystemExit(1)
print("PASS: final conformance structural checks")
