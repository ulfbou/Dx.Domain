#!/usr/bin/env bash
# Ready-for-PR gate: quick, repeatable checks before opening a PR.
#
# Requirements:
#   - GitHub CLI (gh) logged in: gh auth status
#   - Bash (works in Git Bash on Windows, macOS, Linux)
#   - Optional: Python 3 + PyYAML for YAML validation
#
# Usage:
#   scripts/ready-for-pr.sh [--repo owner/name] [--branch BRANCH]
#                           [--with-docfx] [--strict]
#
#   --repo       Explicit GitHub repo (owner/name); otherwise auto-detected
#   --branch     Branch to validate; default = current branch
#   --with-docfx Also dispatch docfx.yml on the branch and fail on CI errors
#   --strict     Treat warnings as failures (exit non-zero)

set -euo pipefail

SELF_DIR="$(cd "$(dirname "$0")" && pwd)"
# shellcheck source=lib.sh
. "$SELF_DIR/lib.sh"

usage() {
  cat <<USAGE
Ready-for-PR gate

Usage: $0 [--repo owner/name] [--branch BRANCH] [--with-docfx] [--strict]
USAGE
}

REPO_FLAG=""
BRANCH_FLAG=""
WITH_DOCFX=0
STRICT=0

while [[ $# -gt 0 ]]; do
  case "$1" in
    -r|--repo)   REPO_FLAG="$2"; shift 2;;
    -b|--branch) BRANCH_FLAG="$2"; shift 2;;
    --with-docfx) WITH_DOCFX=1; shift;;
    --strict)   STRICT=1; shift;;
    -h|--help)  usage; exit 0;;
    *) err "Unknown argument: $1"; usage; exit 2;;
  esac
done

ensure_git_root
REPO="$(resolve_repo "$REPO_FLAG")"
BRANCH="$(resolve_branch "$BRANCH_FLAG")"

# Counters
FAIL=0
WARN_C=0

note_ok()  { ok   "$@"; }
note_warn(){ warn "$@"; WARN_C=$((WARN_C+1)); }
note_err() { err  "$@"; FAIL=$((FAIL+1)); }

say() { echo "$@"; }

say "Repo:   $REPO"
say "Branch: $BRANCH"
echo

# ---------------------------------------------
# 1) Workflows present (by file path for stability)
# ---------------------------------------------
check_wf() {
  local path="$1" display="$2"
  if gh workflow view "$path" --repo "$REPO" --yaml >/dev/null 2>&1; then
    note_ok "Workflow present: $display ($path)"
  else
    note_err "Workflow missing or not on default branch yet: $display ($path)"
  fi
}

check_wf ".github/workflows/build-validate.yml" "Build & Validate"
check_wf ".github/workflows/ci-analyzers.yml"  "CI (Analyzers & Docs Governance)"
# docfx workflow may live on main only; we don't fail if absent but warn
if gh workflow view ".github/workflows/docfx.yml" --repo "$REPO" --yaml >/dev/null 2>&1; then
  note_ok "Workflow present: Build & Publish DocFX Documentation (.github/workflows/docfx.yml)"
else
  note_warn "DocFX workflow not found on default branch; skip docfx checks"
fi

echo
# ---------------------------------------------
# 2) Foundation files on the target BRANCH
# ---------------------------------------------
branch_has() {
  local p="$1"
  if gh api "repos/$REPO/contents/$p?ref=$BRANCH" >/dev/null 2>&1; then
    return 0
  fi
  return 1
}

if branch_has ".github/labeler.yml"; then
  note_ok ".github/labeler.yml present on $BRANCH"
else
  note_err ".github/labeler.yml missing on $BRANCH"
fi

# labels.yml YAML check (optional)
DL_URL=$(gh api "repos/$REPO/contents/.github/labels.yml?ref=$BRANCH" -q .download_url 2>/dev/null || true)
if [ -n "$DL_URL" ]; then
  if python -c "import yaml" >/dev/null 2>&1; then
    if curl -s "$DL_URL" | yaml_ok_py >/dev/null 2>&1; then
      note_ok ".github/labels.yml valid YAML on $BRANCH"
    else
      note_warn ".github/labels.yml present but YAML parse failed on $BRANCH"
    fi
  else
    note_warn ".github/labels.yml present; PyYAML not installed — skipped parse check"
  fi
else
  note_err ".github/labels.yml missing on $BRANCH"
fi

if branch_has ".github/workflows/pr-labeler.yml"; then
  note_ok ".github/workflows/pr-labeler.yml present on $BRANCH"
else
  note_err ".github/workflows/pr-labeler.yml missing on $BRANCH"
fi

if branch_has ".github/workflows/labels-sync.yml"; then
  note_ok ".github/workflows/labels-sync.yml present on $BRANCH"
else
  note_err ".github/workflows/labels-sync.yml missing on $BRANCH"
fi

echo
# ---------------------------------------------
# 3) Obsolete labeler workflow on main (informational)
# ---------------------------------------------
if gh api "repos/$REPO/contents/.github/workflows/labeler.yml?ref=main" >/dev/null 2>&1; then
  note_warn "Obsolete workflow on main: .github/workflows/labeler.yml — replace with pr-labeler + .github/labeler.yml"
fi

echo
# ---------------------------------------------
# 4) DocFX sanity (yaml content on BRANCH); optional CI run
# ---------------------------------------------
if gh api "repos/$REPO/contents/.github/workflows/docfx.yml?ref=$BRANCH" >/dev/null 2>&1; then
  # Download and inspect quickly
  YAML_URL=$(gh api "repos/$REPO/contents/.github/workflows/docfx.yml?ref=$BRANCH" -q .download_url)
  CONTENT=$(curl -s "$YAML_URL" || true)
  if echo "$CONTENT" | grep -qE "(^|\\s)run:\\s*docfx\\s"; then
    note_warn "docfx.yml on $BRANCH uses 'docfx ...'; prefer 'dotnet docfx ...' on runners"
  else
    note_ok "docfx.yml uses 'dotnet docfx' (good)"
  fi
  if echo "$CONTENT" | grep -q "permissions:"; then
    if echo "$CONTENT" | grep -q "contents:\\s*write"; then
      note_ok "docfx.yml grants permissions.contents: write (push to gh-pages possible)"
    else
      note_warn "docfx.yml lacks 'permissions: contents: write' — gh-pages push likely 403"
    fi
  else
    note_warn "docfx.yml has no top-level 'permissions:' — consider adding contents: write"
  fi
else
  note_warn "docfx.yml not present on $BRANCH; skipping content checks"
fi

if (( WITH_DOCFX == 1 )); then
  say
  say "▶ Dispatching docfx.yml on $BRANCH (this may take a minute)..."
  gh workflow run docfx.yml --ref "$BRANCH" --repo "$REPO" || note_err "Failed to dispatch docfx.yml"
  sleep 2
  RUN_ID=$(latest_run_id "$REPO" "docfx.yml")
  if [ -z "$RUN_ID" ]; then
    note_err "Could not determine docfx run id"
  else
    wait_for_run "$REPO" "$RUN_ID"
    CONC=$(gh run view "$RUN_ID" --repo "$REPO" --json conclusion -q .conclusion 2>/dev/null || true)
    if [ "$CONC" != "success" ]; then
      note_err "DocFX run conclusion: ${CONC:-unknown}"
    else
      note_ok "DocFX run succeeded"
    fi
  fi
fi

echo
# ---------------------------------------------
# 5) Decision
# ---------------------------------------------
if (( STRICT == 1 )); then
  TOTAL=$((FAIL + WARN_C))
  if (( TOTAL == 0 )); then
    note_ok "Ready for PR (strict mode)"
    exit 0
  else
    note_err "Not ready for PR (strict mode). FAIL=$FAIL WARN=$WARN_C"
    exit 1
  fi
else
  if (( FAIL == 0 )); then
    note_ok "Ready for PR"
    if (( WARN_C > 0 )); then
      warn "With warnings: $WARN_C (use --strict to enforce)"
    fi
    exit 0
  else
    note_err "Not ready for PR. FAIL=$FAIL WARN=$WARN_C"
    exit 1
  fi
fi
