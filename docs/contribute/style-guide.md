# Documentation Style Guide

Use the canonical product narrative. Address the reader directly, define acronyms once, and prefer short task-oriented sections. Label enforcement as compiler, analyzer, runtime, process, planned, or not guaranteed. Do not use “ensures,” “prevents,” “impossible,” or “frozen” without verified scope. Use repository-relative links, fenced code with a language, exact package IDs, and exact diagnostic IDs. Public pages must not require internal governance knowledge.

## Release-gate documentation
State the reader and intended outcome for every substantive release-gate document. Write the current contract directly without document revision labels, amendment narratives, or comparisons with discarded designs. Exact executable identifiers such as DX v2.0 and schema identifiers remain unchanged.

Use `carrier` for the one final `.dx.txt` handoff, `local evidence` for retained files under `.dx/verification/`, `handoff` for transported continuation information, and `result envelope` for the single `DX_RELEASE_GATE_RESULT=` stdout line. Distinguish primary findings, consequential findings, and operational failures.

Use repository-relative paths and relevant variables including `$REPO_ROOT`, `$DX`, `$DX_FILE`, and `$OUTPUT_DIR`. Never use `/path/to`.