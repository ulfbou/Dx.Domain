#!/usr/bin/env python3
from pathlib import Path
import sys
required={"builds/release/Dx.ReleaseTopology.props":["Dx.Domain.Annotations","Dx.Domain.Kernel","Dx.Domain.Primitives","Dx.Domain.Facts","Dx.Domain.Analyzers","Dx.Domain.Generators","<IsPackable>false</IsPackable>","<GeneratePackageOnBuild>false</GeneratePackageOnBuild>"],"builds/release/Dx.AnalyzerPackaging.targets":["analyzers/dotnet/cs/Dx.Domain.Analyzers.dll","DxBuildAuthoritativeAnalyzerForPackage"],"scripts/create-release-candidate.py":["PROJECTS=(","dotnet\",\"nbgv","verify-release-packages.py"],"scripts/verify-release-packages.py":["EXPECTED=(","ANALYZER=","all five analyzer identities"]}
errors=[]
for p,markers in required.items():
 text=Path(p).read_text(encoding="utf-8")
 for marker in markers:
  if marker not in text:errors.append(f"{p}: missing {marker}")
for name in ("Dx.Domain.Annotations","Dx.Domain.Kernel","Dx.Domain.Primitives","Dx.Domain.Facts"):
 p=Path("src")/name/f"{name}.csproj";text=p.read_text(encoding="utf-8")
 if "<PackageReadmeFile>readme.md</PackageReadmeFile>" not in text:errors.append(f"{p}: PackageReadmeFile")
 if 'Include="readme.md" Pack="true"' not in text:errors.append(f"{p}: packed readme")
solution=Path("Dx.Domain.sln").read_text(encoding="utf-8")
release_entry=Path("scripts/create-release-candidate.py").read_text(encoding="utf-8")
if "Dx.Domain.Generators.csproj" in solution:
 errors.append("generator must be absent from Dx.Domain.sln")
if "Dx.Domain.Generators" in release_entry:
 errors.append("generator must be absent from the authoritative release entry point")
print("WS-001/WS-002 static verification:","OK" if not errors else "FAILED")
for e in errors:print(e)
sys.exit(1 if errors else 0)
