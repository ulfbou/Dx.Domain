#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import re
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
SOURCE_DIR = ROOT / "src" / "Dx.Domain.Analyzers" / "Analyzers"
DEFAULT_OUTPUT = ROOT / "docs" / ".generated" / "analyzers.json"


PATTERNS = {
    "Id": re.compile(r'const string DiagnosticId\s*=\s*"(DXA\d{3})"', re.S),
    "Title": re.compile(r'(?:const string|LocalizableString)\s+Title\s*=\s*"((?:\\.|[^"\\])*)"\s*;', re.S),
    "Message": re.compile(r'(?:const string|LocalizableString)\s+MessageFormat\s*=\s*"((?:\\.|[^"\\])*)"\s*;', re.S),
    "Category": re.compile(r'(?:const string|LocalizableString)\s+Category\s*=\s*"((?:\\.|[^"\\])*)"\s*;', re.S),
    "DefaultSeverity": re.compile(r'DiagnosticSeverity\.(\w+)', re.S),
    "EnabledByDefault": re.compile(r'isEnabledByDefault:\s*(true|false)', re.S),
    "Description": re.compile(r'(?:const string|LocalizableString)\s+Description\s*=\s*"((?:\\.|[^"\\])*)"\s*;', re.S),
    "HelpLinkUri": re.compile(r'helpLinkUri:\s*"((?:\\.|[^"\\])*)"', re.S),
}


def decode(text: str) -> str:
    return bytes(text, "utf-8").decode("unicode_escape")


def capture(pattern: re.Pattern[str], source: str, field: str, filename: Path) -> str:
    match = pattern.search(source)
    if not match:
        raise SystemExit(f"{filename}: missing {field}")
    return decode(match.group(1))


def parse_descriptor(path: Path) -> dict[str, object]:
    text = path.read_text(encoding="utf-8")
    help_link = PATTERNS["HelpLinkUri"].search(text)
    return {
        "Id": capture(PATTERNS["Id"], text, "DiagnosticId", path),
        "Title": capture(PATTERNS["Title"], text, "Title", path),
        "Message": capture(PATTERNS["Message"], text, "MessageFormat", path),
        "Category": capture(PATTERNS["Category"], text, "Category", path),
        "DefaultSeverity": capture(PATTERNS["DefaultSeverity"], text, "DiagnosticSeverity", path),
        "EnabledByDefault": capture(PATTERNS["EnabledByDefault"], text, "isEnabledByDefault", path) == "true",
        "Description": capture(PATTERNS["Description"], text, "Description", path),
        "HelpLinkUri": decode(help_link.group(1)) if help_link else None,
    }


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--output", default=str(DEFAULT_OUTPUT))
    args = parser.parse_args()

    descriptors = []
    seen_ids: set[str] = set()

    for path in sorted(SOURCE_DIR.glob("DXA[0-9][0-9][0-9]_*.cs")):
        descriptor = parse_descriptor(path)
        diagnostic_id = descriptor["Id"]
        if diagnostic_id in seen_ids:
            raise SystemExit(f"duplicate DiagnosticId: {diagnostic_id}")
        seen_ids.add(diagnostic_id)
        descriptors.append(descriptor)

    if not descriptors:
        raise SystemExit(f"no analyzer descriptors found in {SOURCE_DIR}")

    expected_ids = [
        "DXA010",
        "DXA011",
        "DXA020",
        "DXA022",
        "DXA030",
        "DXA040",
        "DXA050",
        "DXA060",
        "DXA065",
        "DXA070",
        "DXA080",
    ]
    actual_ids = [descriptor["Id"] for descriptor in descriptors]
    missing = [diagnostic_id for diagnostic_id in expected_ids if diagnostic_id not in seen_ids]
    if missing:
        raise SystemExit(f"missing analyzer descriptor(s): {missing}")
    if actual_ids != expected_ids:
        raise SystemExit(f"descriptor ordering differs from contract: {actual_ids}")

    output = Path(args.output)
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(json.dumps(descriptors, indent=2) + "\n", encoding="utf-8")
    print(f"wrote {output}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
