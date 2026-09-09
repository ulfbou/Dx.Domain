#!/usr/bin/env bash
set -euo pipefail
root=$(git rev-parse --show-toplevel)
cd "$root"
dotnet build samples/Documentation.Quickstart/Documentation.Quickstart.csproj \
  -c Release \
  --no-restore \
  -p:TreatWarningsAsErrors=false
printf 'documentation-examples: compiled Quickstart against repository projects (net8.0)\n'
