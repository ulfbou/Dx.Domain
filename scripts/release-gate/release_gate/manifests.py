from __future__ import annotations
import hashlib,json
from pathlib import Path
from .evidence import write_json_atomic
from .model import CriterionResult,CriterionStatus
from .packages import read_nuspec,_text

def compute_sha256(path):
 h=hashlib.sha256()
 with Path(path).open('rb') as f:
  for chunk in iter(lambda:f.read(1024*1024),b''):h.update(chunk)
 return h.hexdigest()
def generate_manifest(package_paths,signing_disposition="unsigned"):
 entries=[]
 for p in sorted(map(Path,package_paths),key=lambda x:x.name):
  n=read_nuspec(p); entries.append({"packageId":_text(n,"id"),"version":_text(n,"version"),"filename":p.name,"byteSize":p.stat().st_size,"sha256":compute_sha256(p),"signingDisposition":signing_disposition})
 return {"schema":"dx-domain.candidate-manifest.v1","packages":entries}
def write_manifest(manifest,output_path):write_json_atomic(Path(output_path),manifest)
def verify_manifest(manifest_path,package_dir):
 data=json.loads(Path(manifest_path).read_text(encoding='utf-8')); results=[]
 for e in data.get('packages',[]):
  p=Path(package_dir)/e['filename']; observed={"exists":p.is_file()}
  if p.is_file():
   n=read_nuspec(p); observed.update({"packageId":_text(n,'id'),"version":_text(n,'version'),"filename":p.name,"byteSize":p.stat().st_size,"sha256":compute_sha256(p)})
  expected={k:e[k] for k in ('packageId','version','filename','byteSize','sha256')}; ok=all(observed.get(k)==v for k,v in expected.items())
  results.append(CriterionResult("candidate-manifest-verified",f"Manifest {e['filename']}",CriterionStatus.PASS if ok else CriterionStatus.FAIL,"Retained package matches manifest." if ok else "Retained package does not match manifest.","release_gate.manifests.verify_manifest",[str(manifest_path),str(p)],expected,observed,None if ok else "Discard the candidate and produce it again."))
 return results
def get_authoritative_analyzer_sha256(build_output_path):
 root=Path(build_output_path); found=sorted(root.glob('**/Dx.Domain.Analyzers.dll'))
 candidates=[x for x in found if '/ref/' not in x.as_posix() and '/refint/' not in x.as_posix()]
 if len(candidates)!=1: raise ValueError(f"Expected one authoritative Analyzer output, observed {[x.as_posix() for x in candidates]}")
 return compute_sha256(candidates[0])
