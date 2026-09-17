import subprocess


class GitStateError(RuntimeError):
    pass


def git(repository_root, *args):
    try:
        completed = subprocess.run(
            ("git", *args),
            cwd=repository_root,
            text=True,
            encoding="utf-8",
            errors="replace",
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            check=False,
        )
    except OSError as exc:
        raise GitStateError(
            f"Could not execute Git: {exc}"
        ) from exc

    if completed.returncode != 0:
        detail = completed.stderr.strip() or completed.stdout.strip()
        raise GitStateError(
            f"git {' '.join(args)} failed with "
            f"{completed.returncode}: {detail}"
        )

    return completed.stdout


def capture_git_state(repository_root):
    head = git(
        repository_root,
        "rev-parse",
        "HEAD",
    ).strip()

    branch = git(
        repository_root,
        "branch",
        "--show-current",
    ).strip()

    status = git(
        repository_root,
        "status",
        "--porcelain=v1",
        "-uall",
    ).splitlines()

    tracked_diff = git(
        repository_root,
        "diff",
        "--no-ext-diff",
        "--binary",
        "HEAD",
        "--",
        ".",
    )

    return {
        "head": head,
        "branch": branch,
        "detached": not bool(branch),
        "status": status,
        "tracked_diff": tracked_diff,
    }


def tracked_source_status(repository_root):
    return git(
        repository_root,
        "status",
        "--porcelain=v1",
        "--untracked-files=no",
    ).splitlines()
