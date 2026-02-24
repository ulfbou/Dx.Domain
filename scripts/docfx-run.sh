#!/usr/bin/env bash
set -euo pipefail
. "$(dirname "$0")/lib.sh"

ensure_git_root
REPO="$(gh_repo)"
BRANCH="${1:-$(git rev-parse --abbrev-ref HEAD)}"
WF="docfx.yml"

echo "Dispatching $WF on $BRANCH ..."
gh workflow run "$WF" --ref "$BRANCH" --repo "$REPO"
sleep 2

RUN_ID="$(gh run list --repo "$REPO" --workflow "$WF" --limit 1 --json databaseId -q '.[0].databaseId')"
echo "Run ID: $RUN_ID"

echo "Waiting for run to complete..."
wait_for_run "$REPO" "$RUN_ID"

echo "Logs:"
gh run view "$RUN_ID" --repo "$REPO" --log || true

echo "Try download site artifact (if any):"
mkdir -p "_artifacts/$RUN_ID"
# The artifact name in your workflow is `docfx-public-site`; keep a fallback pattern
gh run download "$RUN_ID" --repo "$REPO" -n "docfx-public-site" -D "_artifacts/$RUN_ID" \
  || gh run download "$RUN_ID" --repo "$REPO" -p "*site*" -D "_artifacts/$RUN_ID" \
  || warn "No artifacts found for run $RUN_ID"

ok "Done."
