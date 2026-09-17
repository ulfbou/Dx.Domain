[CmdletBinding()]
param(
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]] $RemainingArgs
)

$ErrorActionPreference = "Stop"
$ScriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$Runner = Join-Path $ScriptDirectory "run.py"

$Python = Get-Command python3 -ErrorAction SilentlyContinue
if ($null -eq $Python) {
    $Python = Get-Command python -ErrorAction SilentlyContinue
}

if ($null -eq $Python) {
    Write-Error "Python 3 was not found on PATH."
    exit 3
}

& $Python.Source $Runner @RemainingArgs
exit $LASTEXITCODE
