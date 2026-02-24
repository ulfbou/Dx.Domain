#!/usr/bin/env bash
set -euo pipefail

# Colors
if [ -t 1 ]; then
  GREEN='\033[0;32m'; YELLOW='\033[0;33m'; RED='\033[0;31m'; NC='\033[0m'
else
  GREEN=''; YELLOW=''; RED=''; NC=''
fi

ok()   { echo -e "${GREEN}✓${NC} $*"; }
warn() { echo -e "${YELLOW}!${NC} $*"; }
err()  { echo -e "${RED}✗${NC} $*" 1>&2; }

# Ensure we’re in a git repo
ensure_git_root() {
  git rev-parse --show-toplevel >/dev/null 2>&1 || {
    err "Not inside a git repository"; exit 2;
  }
}

# Detect GH repo (e.g., owner/name) from remote origin
gh_repo() {
  local rem
  rem=$(git config --get remote.origin.url || true)
  if [[ "$rem" =~ github.com[:/]+([^/]+)/([^/.]+) ]]; then
    echo "${BASH_REMATCH[1]}/${BASH_REMATCH[2]}"
  else
    # fallback to GH’s current context
    gh repo view --json nameWithOwner -q .nameWithOwner 2>/dev/null || true
  fi
}

# JSON pretty printer if jq exists
jqp() {
  if command -v jq >/dev/null 2>&1; then jq "$@"; else cat; fi
}

# YAML validator using Python (no external yq needed)
yaml_ok_py() {
  python - <<'PY' 2>/dev/null
import sys
try:
    import yaml
    yaml.safe_load(sys.stdin.read())
    print("YAML OK")
except Exception as e:
    sys.exit(1)
PY
}

# Convenience: get latest run id for a workflow (by file path or name)
latest_run_id() {
  local wf="$1" repo="$2" limit="${3:-1}"
  gh run list --repo "$repo" --workflow "$wf" --limit "$limit" --json databaseId -q '.[0].databaseId' 2>/dev/null || true
}

# Wait for a run to complete (simple poll)
wait_for_run() {
  local repo="$1" run_id="$2"
  while true; do
    local c
    c=$(gh run view "$run_id" --repo "$repo" --json status,conclusion -q '.status + " " + ( .conclusion // "" )')
    [[ -z "$c" ]] && { sleep 3; continue; }
    echo "  status: $c"
    [[ "$c" == "completed "* ]] && break
    sleep 5
  done
}
