!/usr/bin/env bash
set -euo pipefail

BASE_REF="${1:-origin/main}"
PRBODYFILE="${2:-/dev/stdin}"

Fetch and compute changed files relative to base
git fetch "${BASE_REF}" --depth=1 >/dev/null 2>&1 || true
CHANGED=$(git diff --name-only "${BASE_REF}"...HEAD || true)

if echo "$CHANGED" | grep -qE '(^|/)Dx\.Domain\.Core/|(^|/)src/Dx\.Domain\.Core/'; then
  PRBODY=$(cat "$PRBODY_FILE" || true)

  required_items=(
    "Enforces an invariant"
    "Removes accidental complexity"
    "Increases what the compiler can prove"
    "Makes misuse impossible"
    "Upholds the Manifesto"
    "Violates no Non-Goals"
    "Correct placement"
  )

  for item in "${required_items[@]}"; do
    if ! printf '%s\n' "$PR_BODY" | grep -Fq "$item"; then
      echo "ERROR: Missing DPI checklist item: $item"
      exit 1
    fi
  done

  echo "DPI checklist present. OK."
else
  echo "No Dx.Domain.Core changes detected."
fi
