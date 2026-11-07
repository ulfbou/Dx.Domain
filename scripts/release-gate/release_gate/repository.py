from __future__ import annotations

from pathlib import Path


MARKERS = (
    ".git",
    "Dx.Domain.sln",
    "Directory.Build.props",
    "version.json",
)


class RepositoryDiscoveryError(RuntimeError):
    pass


def is_repository_root(path: Path) -> bool:
    return all((path / marker).exists() for marker in MARKERS)


def discover_repository_root(*starts: Path) -> Path:
    examined: set[Path] = set()

    for start in starts:
        current = start.resolve()
        if current.is_file():
            current = current.parent

        for candidate in (current, *current.parents):
            if candidate in examined:
                continue
            examined.add(candidate)

            if is_repository_root(candidate):
                return candidate

    locations = ", ".join(str(path) for path in starts)
    raise RepositoryDiscoveryError(
        f"Could not locate the Dx.Domain repository from: {locations}"
    )
