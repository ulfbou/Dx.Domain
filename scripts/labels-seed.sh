# path: scripts/labels-seed.sh
#!/usr/bin/env bash
set -euo pipefail

# Purpose:
#   Seed or update GitHub labels for this repository using labels.json
#
# Requirements:
#   - bash, curl, jq
#   - A GitHub token with "repo" scope in GH_TOKEN or GITHUB_TOKEN
#   - REPO environment variable in the form "owner/repo" (e.g., "ulfbou/dx.domain")
#
# Usage:
#   REPO="owner/repo" GH_TOKEN="..." scripts/labels-seed.sh
#
# Notes:
#   - Existing labels with the same name are updated (color/description).
#   - New labels are created when missing.
#   - Script is idempotent.

if [[ -z "${REPO:-}" ]]; then
  echo "ERROR: REPO environment variable must be set (e.g., 'owner/repo')." >&2
  exit 1
fi

TOKEN="${GH_TOKEN:-${GITHUB_TOKEN:-}}"
if [[ -z "${TOKEN}" ]]; then
  echo "ERROR: GH_TOKEN or GITHUB_TOKEN must be set." >&2
  exit 1
fi

if [[ ! -f "labels.json" ]]; then
  echo "ERROR: labels.json not found in current directory." >&2
  exit 1
fi

API="https://api.github.com/repos/${REPO}/labels"
AUTH_HEADER="Authorization: Bearer ${TOKEN}"
ACCEPT_HEADER="Accept: application/vnd.github+json"

echo "Fetching existing labels from ${REPO}..."
existing="$(curl -fsSL -H "${AUTH_HEADER}" -H "${ACCEPT_HEADER}" "${API}?per_page=100")"

# Iterate over labels.json entries and upsert
jq -c '.[]' labels.json | while read -r label; do
  name=$(jq -r '.name' <<<"$label")
  color=$(jq -r '.color' <<<"$label")
  desc=$(jq -r '.description' <<<"$label")

  # Check if label exists
  exists=$(jq --arg n "$name" -r '.[] | select(.name==$n) | .name' <<<"$existing" || true)

  if [[ -n "${exists}" ]]; then
    echo "Updating label: ${name}"
    # GitHub API: PATCH /repos/{owner}/{repo}/labels/{name}
    curl -fsSL -X PATCH \
      -H "${AUTH_HEADER}" -H "${ACCEPT_HEADER}" \
      -d "$(jq -n --arg newname "$name" --arg color "$color" --arg desc "$desc" '{new_name:$newname, color:$color, description:$desc}')" \
      "${API}/$(jq -rn --arg s "$name" '$s|@uri')" >/dev/null
  else
    echo "Creating label: ${name}"
    # GitHub API: POST /repos/{owner}/{repo}/labels
    curl -fsSL -X POST \
      -H "${AUTH_HEADER}" -H "${ACCEPT_HEADER}" \
      -d "$(jq -n --arg name "$name" --arg color "$color" --arg desc "$desc" '{name:$name, color:$color, description:$desc}')" \
      "${API}" >/dev/null
  fi
done

echo "Done. Labels synced to ${REPO}."
