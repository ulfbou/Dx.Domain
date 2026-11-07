#!/usr/bin/env python3
from __future__ import annotations
import argparse, hashlib, json, shutil, subprocess, sys
from datetime import datetime, timezone
from pathlib import Path
VERSION="0.1.0-alpha"
PROJECTS=("Dx.Domain.Annotations","Dx.Domain.Kernel","Dx.Domain.Primitives","Dx.Domain.Facts")
def run(*args:str)->str:
 print("+"," ".join(args),flush=True);r=subprocess.run(args,text=True,encoding="utf-8",stdout=subprocess.PIPE,stderr=subprocess.STDOUT);print(r.stdout,end="");
 if r.returncode:raise RuntimeError(f"command failed ({r.returncode}): {' '.join(args)}")
 return r.stdout
def git(*args:str)->str:return subprocess.run(("git",)+args,text=True,encoding="utf-8",stdout=subprocess.PIPE,check=True).stdout.strip()
def main()->int:
 ap=argparse.ArgumentParser();ap.add_argument("--output",type=Path,default=Path("artifacts/release/0.1.0-alpha"));ap.add_argument("--evidence",type=Path,default=Path(".dx/verification"));ap.add_argument("--allow-dirty",action="store_true");a=ap.parse_args()
 branch=git("branch","--show-current");head=git("rev-parse","HEAD");status=git("status","--porcelain=v1","-uall").splitlines()
 if branch!="release/0.1.0-alpha":raise RuntimeError(f"wrong branch: {branch}")
 if status and not a.allow_dirty:raise RuntimeError("working tree is not clean")
 run("dotnet","tool","restore");resolved=run("dotnet","nbgv","get-version","-v","NuGetPackageVersion").strip().splitlines()[-1]
 if resolved!=VERSION:raise RuntimeError(f"NBGV version {resolved!r} does not equal {VERSION!r}")
 if a.output.exists():shutil.rmtree(a.output)
 a.output.mkdir(parents=True);a.evidence.mkdir(parents=True,exist_ok=True)
 run("dotnet","clean","Dx.Domain.sln","-c","Release","--nologo")
 common=("-c","Release","--nologo","-p:ContinuousIntegrationBuild=true","-p:TreatWarningsAsErrors=true")
 run("dotnet","build","src/Dx.Domain.Analyzers/Dx.Domain.Analyzers.csproj",*common)
 run("dotnet","build","src/Dx.Domain.Annotations/Dx.Domain.Annotations.csproj",*common)
 run("dotnet","build","src/Dx.Domain.Kernel/Dx.Domain.Kernel.csproj",*common)
 run("dotnet","build","src/Dx.Domain.Primitives/Dx.Domain.Primitives.csproj",*common)
 run("dotnet","build","src/Dx.Domain.Facts/Dx.Domain.Facts.csproj",*common)
 for project in PROJECTS:run("dotnet","pack",f"src/{project}/{project}.csproj",*common,"--no-build","--no-restore","-o",str(a.output))
 report=a.evidence/"ws-001-002-package-verification.json";command=" ".join(sys.argv)
 run(sys.executable,"scripts/verify-release-packages.py","--packages",str(a.output),"--analyzer","src/Dx.Domain.Analyzers/bin/Release/netstandard2.0/Dx.Domain.Analyzers.dll","--report",str(report),"--command",command)
 verification=json.loads(report.read_text(encoding="utf-8"));manifest={"schema":"dx-domain.release-manifest.v1","created_utc":datetime.now(timezone.utc).isoformat(),"repository":"https://github.com/ulfbou/Dx.Domain","branch":branch,"head":head,"working_tree_clean":not status,"version":VERSION,"packages":verification["packages"],"authoritative_analyzer":verification["authoritative_analyzer"],"verification_report":str(report),"verification_report_sha256":hashlib.sha256(report.read_bytes()).hexdigest()};mp=a.evidence/"ws-001-002-release-manifest.json";mp.write_text(json.dumps(manifest,indent=2,sort_keys=True)+"\n",encoding="utf-8",newline="\n");print(mp);return 0
if __name__=="__main__":
 try:sys.exit(main())
 except Exception as e:print(f"ERROR: {e}",file=sys.stderr);sys.exit(1)
