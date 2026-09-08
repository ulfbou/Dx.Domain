#!/usr/bin/env bash
set -euo pipefail
if [ "$#" -ne 2 ]; then
  printf 'usage: %s DOCS_DIR TARGET_FRAMEWORK\n' "$0" >&2
  return 2 2>/dev/null || false
fi
docs_dir=$1
tfm=$2
python3 - "$docs_dir" "$tfm" <<'PY'
from pathlib import Path
import re,sys
root=Path(sys.argv[1]); tfm=sys.argv[2]
blocks=[]
for p in root.rglob('*.md'):
    for block in re.findall(r'```csharp\n(.*?)\n```',p.read_text(),re.S): blocks.append((p,block))
if not blocks: raise SystemExit('no C# snippets found')
for p,b in blocks:
    if b.count('{') != b.count('}'): raise SystemExit(f'{p}: unbalanced braces')
print(f'snippet-structure: ok ({len(blocks)} C# blocks, target {tfm})')
print('Full compilation is performed by the release clean-project smoke test.')
PY
