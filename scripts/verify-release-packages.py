#!/usr/bin/env python3
from __future__ import annotations
import argparse, hashlib, json, subprocess, sys, zipfile
from datetime import datetime, timezone
from pathlib import Path
from xml.etree import ElementTree as ET
EXPECTED=("Dx.Domain.Annotations","Dx.Domain.Kernel","Dx.Domain.Primitives","Dx.Domain.Facts")
VERSION="0.1.0-alpha"
ANALYZER="analyzers/dotnet/cs/Dx.Domain.Analyzers.dll"
FORBIDDEN_PREFIXES=("lib/","runtimes/","ref/","content/","contentFiles/","tools/","build/","buildTransitive/")
def sha(data:bytes)->str:return hashlib.sha256(data).hexdigest()
def git(*args:str)->str:
 r=subprocess.run(("git",)+args,text=True,encoding="utf-8",stdout=subprocess.PIPE,stderr=subprocess.PIPE)
 return r.stdout.rstrip("\n") if r.returncode==0 else f"ERROR: {r.stderr.strip()}"
def local(tag:str)->str:return tag.rsplit("}",1)[-1]
def main()->int:
 ap=argparse.ArgumentParser();ap.add_argument("--packages",type=Path,required=True);ap.add_argument("--analyzer",type=Path,required=True);ap.add_argument("--report",type=Path,required=True);ap.add_argument("--command",default="")
 a=ap.parse_args(); errors=[]; result={"schema":"dx-domain.release-package-verification.v1","created_utc":datetime.now(timezone.utc).isoformat(),"repository":{"remote":git("remote","get-url","origin"),"branch":git("branch","--show-current"),"head":git("rev-parse","HEAD"),"status":git("status","--porcelain=v1","-uall").splitlines()},"sdk":subprocess.run(("dotnet","--version"),text=True,stdout=subprocess.PIPE).stdout.strip(),"command":a.command,"candidate_location":str(a.packages.resolve()),"expected_version":VERSION,"errors":errors,"packages":[]}
 try:
  authoritative=a.analyzer.read_bytes(); auth={"path":str(a.analyzer),"size":len(authoritative),"sha256":sha(authoritative)};result["authoritative_analyzer"]=auth
 except Exception as e: errors.append(f"authoritative analyzer: {e}");authoritative=b"";auth={"size":-1,"sha256":""}
 found=sorted(a.packages.glob("*.nupkg")); expected_names={f"{x}.{VERSION}.nupkg" for x in EXPECTED}; actual_names={x.name for x in found}
 for name in sorted(expected_names-actual_names):errors.append(f"missing package: {name}")
 for name in sorted(actual_names-expected_names):errors.append(f"unexpected package: {name}")
 seen=set()
 for path in found:
  rec={"file":path.name,"size":path.stat().st_size,"sha256":sha(path.read_bytes()),"errors":[]};result["packages"].append(rec)
  try:
   with zipfile.ZipFile(path) as z:
    names=z.namelist(); nuspecs=[n for n in names if n.lower().endswith(".nuspec")]
    if len(nuspecs)!=1:raise ValueError(f"expected one nuspec, found {len(nuspecs)}")
    xml=ET.fromstring(z.read(nuspecs[0])); meta=next((x for x in xml.iter() if local(x.tag)=="metadata"),None)
    values={local(x.tag):(x.text or "").strip() for x in meta} if meta is not None else {}
    pid,version=values.get("id",""),values.get("version","");rec.update({"id":pid,"version":version,"readme":values.get("readme","")})
    if pid not in EXPECTED:rec["errors"].append(f"incorrect package id: {pid}")
    if pid in seen:rec["errors"].append(f"duplicate package id: {pid}")
    seen.add(pid)
    if version!=VERSION:rec["errors"].append(f"incorrect version: {version}")
    occurrences=[n for n in names if n.replace("\\","/").lower().endswith("dx.domain.analyzers.dll")]
    accepted=[n for n in occurrences if n.replace("\\","/")==ANALYZER]
    if len(accepted)!=1:rec["errors"].append(f"required analyzer occurrences: {len(accepted)}")
    outside=[n for n in occurrences if n.replace("\\","/")!=ANALYZER]
    if outside:rec["errors"].append(f"analyzer outside accepted path: {outside}")
    if accepted:
     embedded=z.read(accepted[0]);rec["analyzer"]={"path":accepted[0],"size":len(embedded),"sha256":sha(embedded)}
     if embedded!=authoritative:rec["errors"].append("embedded analyzer differs from authoritative output")
    dependencies=[(x.attrib.get("id") or "") for x in xml.iter() if local(x.tag)=="dependency"]
    prohibited=[x for x in dependencies if x in {"Dx.Domain.Analyzers","Dx.Domain.Generators"}]
    rec["dependencies"]=dependencies
    if prohibited:rec["errors"].append(f"prohibited dependencies: {prohibited}")
    readme=values.get("readme",""); matches=[n for n in names if n.lstrip("/")==readme.lstrip("/")]
    if not readme:rec["errors"].append("nuspec readme metadata missing")
    elif len(matches)!=1:rec["errors"].append(f"declared readme occurrences: {len(matches)}")
  except Exception as e:rec["errors"].append(f"archive verification: {type(e).__name__}: {e}")
  errors.extend(f"{path.name}: {e}" for e in rec["errors"])
 if seen!=set(EXPECTED):errors.append(f"package id set mismatch: {sorted(seen)}")
 hashes={p.get("analyzer",{}).get("sha256") for p in result["packages"] if p.get("analyzer")}
 if len(hashes)!=1 or hashes!={auth.get("sha256")}:errors.append("all five analyzer identities are not equal")
 result["success"]=not errors;a.report.parent.mkdir(parents=True,exist_ok=True);a.report.write_text(json.dumps(result,indent=2,sort_keys=True)+"\n",encoding="utf-8",newline="\n");print(f"{a.report}: {'OK' if not errors else 'FAILED'} ({len(errors)} errors)");return 0 if not errors else 1
if __name__=="__main__":sys.exit(main())
