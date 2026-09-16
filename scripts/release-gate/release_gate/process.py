from __future__ import annotations

import os
import signal
import subprocess
import time
from datetime import datetime, timezone
from pathlib import Path
from typing import Mapping, Sequence

from .model import ExecutionClassification, ProcessResult


def utc_now() -> str:
    return datetime.now(timezone.utc).isoformat()


def _terminate_process_tree(
    process: subprocess.Popen[bytes],
) -> list[str]:
    actions: list[str] = []
    if process.poll() is not None:
        return actions
    if os.name == "nt":
        completed = subprocess.run(
            ("taskkill", "/PID", str(process.pid), "/T"),
            stdout=subprocess.DEVNULL,
            stderr=subprocess.DEVNULL,
            check=False,
        )
        actions.append(f"taskkill:{completed.returncode}")
        if process.poll() is None:
            completed = subprocess.run(
                (
                    "taskkill",
                    "/PID",
                    str(process.pid),
                    "/T",
                    "/F",
                ),
                stdout=subprocess.DEVNULL,
                stderr=subprocess.DEVNULL,
                check=False,
            )
            actions.append(f"taskkill-force:{completed.returncode}")
    else:
        try:
            os.killpg(process.pid, signal.SIGTERM)
            actions.append("sigterm-process-group")
        except ProcessLookupError:
            return actions
        try:
            process.wait(timeout=5)
        except subprocess.TimeoutExpired:
            try:
                os.killpg(process.pid, signal.SIGKILL)
                actions.append("sigkill-process-group")
            except ProcessLookupError:
                pass
    return actions


def run_process(
    *,
    command_id: str,
    argv: Sequence[str],
    cwd: Path,
    evidence_directory: Path,
    timeout_seconds: int,
    environment: Mapping[str, str] | None = None,
) -> ProcessResult:
    command_directory = evidence_directory / "commands" / command_id
    command_directory.mkdir(parents=True, exist_ok=True)
    stdout_path = command_directory / "stdout.txt"
    stderr_path = command_directory / "stderr.txt"
    started = utc_now()
    monotonic_start = time.monotonic()
    termination_actions: list[str] = []
    timed_out = False
    merged_environment = os.environ.copy()
    if environment:
        merged_environment.update(environment)
    popen_options: dict[str, object] = {}
    if os.name == "nt":
        popen_options["creationflags"] = subprocess.CREATE_NEW_PROCESS_GROUP
    else:
        popen_options["start_new_session"] = True
    try:
        with (
            stdout_path.open("wb") as stdout_stream,
            stderr_path.open("wb") as stderr_stream,
        ):
            process = subprocess.Popen(
                list(argv),
                cwd=cwd,
                env=merged_environment,
                stdout=stdout_stream,
                stderr=stderr_stream,
                **popen_options,
            )
            try:
                exit_code = process.wait(timeout=timeout_seconds)
            except subprocess.TimeoutExpired:
                timed_out = True
                termination_actions.extend(_terminate_process_tree(process))
                try:
                    exit_code = process.wait(timeout=10)
                except subprocess.TimeoutExpired:
                    exit_code = None
    except FileNotFoundError:
        exit_code = None
        classification = ExecutionClassification.EXECUTABLE_NOT_FOUND
    except OSError:
        exit_code = None
        classification = ExecutionClassification.START_FAILURE
    else:
        if timed_out:
            classification = ExecutionClassification.TIMEOUT
        elif exit_code == 0:
            classification = ExecutionClassification.SUCCESS
        else:
            classification = ExecutionClassification.NONZERO_EXIT
    finished = utc_now()
    duration = time.monotonic() - monotonic_start
    return ProcessResult(
        command_id=command_id,
        argv=list(argv),
        cwd=str(cwd),
        started_utc=started,
        finished_utc=finished,
        duration_seconds=round(duration, 6),
        exit_code=exit_code,
        classification=classification,
        timed_out=timed_out,
        termination_actions=termination_actions,
        stdout_path=stdout_path.as_posix(),
        stderr_path=stderr_path.as_posix(),
    )
