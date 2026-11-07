#!/usr/bin/env sh

SCRIPT_DIR=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd) || return 3 2>/dev/null || exit 3

if command -v python3 >/dev/null 2>&1; then
    exec python3 "$SCRIPT_DIR/run.py" "$@"
fi

if command -v python >/dev/null 2>&1; then
    exec python "$SCRIPT_DIR/run.py" "$@"
fi

printf '%s\n' "ERROR: Python 3 was not found on PATH." >&2
exit 3
