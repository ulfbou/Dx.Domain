#!/usr/bin/env bash
set -euo pipefail
python3 - <<'PY'
from pathlib import Path
import re, sys, posixpath
root=Path('.')
public=root/'docs/public'
errors=[]
tracked=[root/'readme.md', root/'CHANGELOG.md', root/'CONTRIBUTING.md', root/'SECURITY.md', *list((root/'docs').rglob('*.md'))]
for p in tracked:
    try: text=p.read_text(encoding='utf-8')
    except UnicodeDecodeError: continue
    if any(m in text for m in ('<<<<<<< ', '>>>>>>> ')):
        errors.append(f'{p}: merge marker')
for p in public.rglob('*.md'):
    text=p.read_text(encoding='utf-8')
    if not text.strip(): errors.append(f'{p}: empty page')
    if re.search(r'placeholder|example\.com', text, re.I): errors.append(f'{p}: placeholder text')
    for target in re.findall(r'\[[^\]]*\]\(([^)]+)\)', text):
        link=target.split('#',1)[0]
        if not link or '://' in link or link.startswith(('mailto:','#')): continue
        q=(p.parent/link).resolve()
        if not q.exists(): errors.append(f'{p}: broken link {target}')
for p in public.rglob('toc.yml'):
    for href in re.findall(r'^\s*href:\s*["\']?([^"\'\s]+)', p.read_text(), re.M):
        link=href.split('#',1)[0]
        if link and not (p.parent/link).exists(): errors.append(f'{p}: broken toc href {href}')
if errors:
    print('\n'.join(errors), file=sys.stderr); raise SystemExit(1)
print(f'docs-lint: ok ({len(list(public.rglob("*.md")))} public pages)')
PY
