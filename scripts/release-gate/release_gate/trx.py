from __future__ import annotations

import xml.etree.ElementTree as ET
from pathlib import Path

from .model import TestResult


class TrxError(ValueError):
    pass


def _integer(value: str | None, name: str) -> int:
    if value is None:
        return 0

    try:
        parsed = int(value)
    except ValueError as exc:
        raise TrxError(f"Invalid TRX {name}: {value!r}") from exc

    if parsed < 0:
        raise TrxError(f"TRX {name} cannot be negative")

    return parsed


def parse_trx(
    path: Path,
    *,
    project: str,
    target_framework: str,
) -> TestResult:
    if not path.is_file():
        raise TrxError(f"TRX does not exist: {path}")

    try:
        root = ET.parse(path).getroot()
    except (ET.ParseError, OSError) as exc:
        raise TrxError(f"Malformed TRX {path}: {exc}") from exc

    counters = None
    times = None

    for element in root.iter():
        local_name = element.tag.rsplit("}", 1)[-1]
        if local_name == "Counters":
            counters = element
        elif local_name == "Times":
            times = element

    if counters is None:
        raise TrxError(f"TRX contains no result counters: {path}")

    total = _integer(counters.get("total"), "total")
    executed = _integer(counters.get("executed"), "executed")
    passed = _integer(counters.get("passed"), "passed")
    failed = _integer(counters.get("failed"), "failed")
    skipped = (
        _integer(counters.get("notExecuted"), "notExecuted")
        + _integer(counters.get("inconclusive"), "inconclusive")
        + _integer(counters.get("notRunnable"), "notRunnable")
        + _integer(counters.get("disconnected"), "disconnected")
        + _integer(counters.get("warning"), "warning")
    )

    if executed != passed + failed:
        raise TrxError(
            f"Inconsistent executed count in {path}: "
            f"executed={executed}, passed={passed}, failed={failed}"
        )

    if total < executed:
        raise TrxError(
            f"Inconsistent total count in {path}: total={total}, executed={executed}"
        )

    if total != executed + skipped:
        raise TrxError(
            f"Inconsistent total/skipped counts in {path}: "
            f"total={total}, executed={executed}, skipped={skipped}"
        )

    duration = times.get("finish") if times is not None else None

    return TestResult(
        project=project,
        target_framework=target_framework,
        trx_path=path.as_posix(),
        total=total,
        executed=executed,
        passed=passed,
        failed=failed,
        skipped=skipped,
        duration=duration,
    )
