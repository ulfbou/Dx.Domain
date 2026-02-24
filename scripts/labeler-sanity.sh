#!/usr/bin/env bash
set -euo pipefail
. "$(dirname "$0")/lib.sh"

ensure_git_root
REPO="$(gh_repo)"
BRANCH="${1:-$(git rev-parse --abbrev-ref HEAD)}"

echo "Repo: $REPO"
echo "Branch: $BRANCH"

echo "▶ Check config on $BRANCH"
if gh api "repos/$REPO/contents/.github/labeler.yml?ref=$BRANCH" >/dev/null 2>&1; then
  ok ".github/labeler.yml present on $BRANCH"
else
  err "Missing .github/labeler.yml on $BRANCH"
fi

echo "▶ Check target workflow location (pr-labeler):"
if gh api "repos/$REPO/contents/.github/workflows/pr-labeler.yml?ref=$BRANCH" >/dev/null 2>&1; then
  ok ".github/workflows/pr-labeler.yml present on $BRANCH"
else
  err "Missing .github/workflows/pr-labeler.yml on $BRANCH"
fi

echo "▶ Check obsolete workflow on main:"
if gh api "repos/$REPO/contents/.github/workflows/labeler.yml?ref=main" >/dev/null 2>&1; then
  warn "Obsolete workflow found on main: .github/workflows/labeler.yml"
  echo "  To remove it through a PR:"
  echo "    git switch -c chore/ci/remove-obsolete-labeler"
  echo "    git checkout origin/main -- .github/workflows/labeler.yml"
  echo "    git rm -f .github/workflows/labeler.yml"
  echo "    git commit -m 'ci: remove obsolete labeler workflow in favor of pr-labeler + .github/labeler.yml'"
  echo "    git push --set-upstream origin chore/ci/remove-obsolete-labeler"
  echo "    gh pr create -f -t 'ci: remove obsolete labeler workflow' -b 'Replaces misplaced labeler with pr-labeler and .github/labeler.yml.'"
else
  ok "No obsolete .github/workflows/labeler.yml on main"
fi
