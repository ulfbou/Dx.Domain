from __future__ import annotations
import zipfile
import xml.etree.ElementTree as ET
from pathlib import Path
from .model import CriterionResult, CriterionStatus

def result(cid, ok, expected, observed, evidence):
    return CriterionResult(cid,cid,CriterionStatus.PASS if ok else CriterionStatus.FAIL,"Package evidence matched the contract." if ok else "Package evidence contradicted the contract.","release_gate.packages",[str(evidence)],expected,observed,None if ok else "Correct package production and rebuild the candidate once.")
def _names(z): return [x.filename.replace('\\','/') for x in z.infolist()]
def is_valid_zip(path):
    try:
        with zipfile.ZipFile(path) as z:
            names=_names(z); return len(names)==len(set(names)) and z.testzip() is None
    except (OSError,zipfile.BadZipFile): return False
def read_nuspec(path):
    with zipfile.ZipFile(path) as z:
        names=[n for n in _names(z) if n.lower().endswith('.nuspec')]
        if len(names)!=1: raise ValueError(f"Expected exactly one nuspec, observed {len(names)}")
        return ET.fromstring(z.read(names[0]))
def _local(e): return e.tag.rsplit('}',1)[-1]
def _text(root,name):
    for e in root.iter():
        if _local(e)==name:return (e.text or '').strip()
    return ''
def validate_nuspec(nuspec,expected_id,expected_version,evidence="nuspec"):
    observed={"id":_text(nuspec,"id"),"version":_text(nuspec,"version")}; expected={"id":expected_id,"version":expected_version}
    return result("candidate-nuspec",observed==expected,expected,observed,evidence)
def validate_framework_groups(nuspec,expected,evidence="nuspec"):
    observed=sorted({e.attrib.get("targetFramework","").lstrip('.') for e in nuspec.iter() if _local(e)=="group" and e.attrib.get("targetFramework")})
    # NuGet emits .NETStandard2.0 and net8.0 style names; normalize both.
    norm=lambda x:x.lower().replace(".netstandard","netstandard").replace(".netcoreapp","net")
    ok={norm(x) for x in observed}=={norm(x) for x in expected}
    return result("candidate-framework",ok,sorted(expected),observed,evidence)
def validate_dependencies(nuspec,expected,version,evidence="nuspec"):
    observed=sorted({e.attrib.get("id") for e in nuspec.iter() if _local(e)=="dependency" and e.attrib.get("id")})
    bad_versions=[e.attrib for e in nuspec.iter() if _local(e)=="dependency" and e.attrib.get("id") in expected and version not in e.attrib.get("version","")]
    return result("candidate-dependency",observed==sorted(expected) and not bad_versions,{"ids":sorted(expected),"version":version},{"ids":observed,"badVersions":bad_versions},evidence)
def validate_readme_presence(zip_path,expected_readme):
    with zipfile.ZipFile(zip_path) as z: names={n.lower().lstrip('/') for n in _names(z)}
    return result("candidate-readme",expected_readme.lower().lstrip('/') in names,expected_readme,sorted(names),zip_path)
def extract_analyzer_bytes(zip_path,analyzer_path):
    with zipfile.ZipFile(zip_path) as z:return z.read(analyzer_path)
def validate_analyzer_placement(zip_path,analyzer_path="analyzers/dotnet/cs/Dx.Domain.Analyzers.dll",forbidden_paths=("lib/","runtimes/")):
    with zipfile.ZipFile(zip_path) as z:names=_names(z)
    analyzer=[n for n in names if n.lower().endswith("dx.domain.analyzers.dll")]; forbidden=[n for n in analyzer if any(n.lower().startswith(x.lower()) for x in forbidden_paths)]
    ok=analyzer==[analyzer_path] and not forbidden
    return result("candidate-analyzer-placement",ok,{"exact":[analyzer_path],"forbidden":[]},{"analyzerEntries":analyzer,"forbidden":forbidden},zip_path)
