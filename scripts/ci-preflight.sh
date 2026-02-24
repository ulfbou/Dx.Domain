#!/usr/bin/env bash
set -euo pipefail
. "$(dirname "$0")/lib.sh"

ensure_git_root
REPO="$(gh_repo)"
BRANCH="${1:-$(git rev-parse --abbrev-ref HEAD)}"

echo "Repo: $REPO"
echo "Branch: $BRANCH"
echo

echo "▶ Workflows (enabled):"
gh workflow list --repo "$REPO" --all || true
echo

echo "▶ Latest Build & Validate runs:"
gh run list --repo "$REPO" --workflow "Build & Validate" --limit 5 || true
echo

echo "▶ Latest CI (Analyzers & Docs Governance) runs:"
gh run list --repo "$REPO" --workflow "CI (Analyzers & Docs Governance)" --limit 5 || true
echo

echo "▶ Labeler config (this branch):"
if gh api "repos/$REPO/contents/.github/labeler.yml?ref=$BRANCH" >/dev/null 2>&1; then
  ok ".github/labeler.yml present on $BRANCH"
else
  err ".github/labeler.yml is missing on $BRANCH"
fi

echo "▶ Labels catalog (this branch):"
if gh api "repos/$REPO/contents/.github/labels.yml?ref=$BRANCH" --jq .download_url | xargs -I{} curl -s {} | yaml_ok_py >/dev/null 2>&1; then
  ok ".github/labels.yml valid YAML on $BRANCH"
else
  warn ".github/labels.yml missing or invalid YAML on $BRANCH (or Python PyYAML missing)"
fi

echo
echo "▶ Conclusion:"
ok "Preflight complete. Address any red items above before opening PR."
