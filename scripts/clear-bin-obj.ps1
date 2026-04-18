[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'Medium')]
param(
    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string]$RootPath = (Split-Path -Parent $PSScriptRoot)
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path -LiteralPath $RootPath -PathType Container)) {
    throw "Root path does not exist or is not a directory: $RootPath"
}

Write-Host "Scanning for 'bin' and 'obj' folders under: $RootPath"

$targetFolders = Get-ChildItem -LiteralPath $RootPath -Directory -Recurse -Force |
    Where-Object { $_.Name -in @('bin', 'obj') }

if (-not $targetFolders) {
    Write-Host "No 'bin' or 'obj' folders found."
    return
}

Write-Host "Found $($targetFolders.Count) folder(s) to remove."

foreach ($folder in $targetFolders) {
    if ($PSCmdlet.ShouldProcess($folder.FullName, 'Remove folder recursively')) {
        Remove-Item -LiteralPath $folder.FullName -Recurse -Force
        Write-Host "Removed: $($folder.FullName)"
    }
}

Write-Host 'Cleanup complete.'
