#!/usr/bin/env bash
set -euo pipefail
root=$(git rev-parse --show-toplevel)
cd "$root"
project="samples/Documentation.Quickstart/Documentation.Quickstart.csproj"

dotnet restore "$project"

dotnet build "$project" \
  -c Release \
  --no-restore \
  -p:TreatWarningsAsErrors=false
printf 'documentation-examples: compiled Quickstart against repository projects (net8.0)\n'
