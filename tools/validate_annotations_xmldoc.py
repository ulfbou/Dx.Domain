#!/usr/bin/env python3
import re, sys, pathlib

ROOT = pathlib.Path('src/Dx.Domain.Annotations')
FAIL = 0
rules = []

SUMMARY_RE = re.compile(r"///\s*<summary>\s*(.*?)\s*</summary>", re.DOTALL)
REMARKS_RE = re.compile(r"///\s*<remarks>(.*?)</remarks>", re.DOTALL)
NON_PRESC_LABEL = "Example (Kernel realization, non‑prescriptive):"

FORBIDDEN_WORDS = [r"must", r"shall", r"required"]

for path in ROOT.rglob('*.cs'):
    text = path.read_text(encoding='utf-8')
    # extract xml doc blocks (naive but effective for CI)
    if '///' not in text:
        print(f"[WARN] No XML docs found: {path}")
        continue
    summ = SUMMARY_RE.search(text)
    rem = REMARKS_RE.search(text)
    if not summ:
        print(f"[ERROR] Missing <summary>: {path}")
        FAIL = 1
    else:
        if 'marker' not in summ.group(1).lower():
            print(f"[ERROR] <summary> must state it is a marker: {path}")
            FAIL = 1
    if not rem:
        print(f"[ERROR] Missing <remarks>: {path}")
        FAIL = 1
    else:
        rtxt = rem.group(1).lower()
        if 'no runtime semantics' not in rtxt and 'does not impose' not in rtxt:
            print(f"[ERROR] <remarks> must state no runtime semantics: {path}")
            FAIL = 1
        # forbid normative words
        for w in FORBIDDEN_WORDS:
            if re.search(w, rtxt):
                print(f"[ERROR] Normative language found in <remarks>: {path}")
                FAIL = 1
        # require example label if code block present
        if '<code>' in rem.group(1) and 'Example (Kernel realization, non' not in rem.group(1):
            print(f"[ERROR] Example code block must be labeled non-prescriptive: {path}")
            FAIL = 1

sys.exit(FAIL)
