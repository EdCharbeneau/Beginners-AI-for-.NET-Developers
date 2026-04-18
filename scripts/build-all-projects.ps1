[CmdletBinding()]
param(
    [string]$RootPath,
    [switch]$NoRestore,
    [switch]$StopOnFirstFailure
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-RelativePath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$BasePath,
        [Parameter(Mandatory = $true)]
        [string]$TargetPath
    )

    Push-Location -LiteralPath (Resolve-Path -LiteralPath $BasePath).Path
    try {
        return (Resolve-Path -LiteralPath $TargetPath -Relative).TrimStart(@('.', [char]92))
    }
    finally {
        Pop-Location
    }
}

function Get-BuildErrorLines {
    param(
        [AllowNull()]
        [AllowEmptyCollection()]
        [string[]]$OutputLines
    )

    $matches = $OutputLines | Where-Object {
        $_ -match ':\s*error\s+[A-Za-z]+\d+\s*:' -or
        $_ -match '\berror\s+[A-Za-z]+\d+\s*:' -or
        $_ -match '^\s*error\b'
    }

    if ($matches) {
        return $matches
    }

    return @()
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw 'dotnet CLI is required but was not found on PATH.'
}

if ([string]::IsNullOrWhiteSpace($RootPath)) {
    $RootPath = Split-Path -Parent $PSCommandPath
}

if (-not (Test-Path -LiteralPath $RootPath)) {
    throw "Root path not found: $RootPath"
}

$resolvedRoot = (Resolve-Path -LiteralPath $RootPath).Path
Write-Host "Root folder: $resolvedRoot"

$projectFiles = Get-ChildItem -LiteralPath $resolvedRoot -Recurse -File |
    Where-Object { $_.Extension -in @('.csproj', '.fsproj') } |
    Sort-Object FullName

if (-not $projectFiles) {
    Write-Host 'No .csproj or .fsproj files found. Nothing to build.'
    exit 0
}

Write-Host "Found $($projectFiles.Count) projects."
Write-Host ''

$results = New-Object System.Collections.Generic.List[object]
$index = 0

foreach ($project in $projectFiles) {
    $index++
    $relativePath = Get-RelativePath -BasePath $resolvedRoot -TargetPath $project.FullName

    Write-Host "[$index/$($projectFiles.Count)] Building $relativePath"

    $args = @('build', $project.FullName, '--nologo', '--verbosity', 'minimal')
    if ($NoRestore) {
        $args += '--no-restore'
    }

    $output = & dotnet @args 2>&1
    $exitCode = $LASTEXITCODE

    if ($exitCode -eq 0) {
        Write-Host "  SUCCESS: $relativePath"
        $results.Add([pscustomobject]@{
                Project = $relativePath
                ExitCode = 0
                Status = 'Success'
                Errors = @()
            }) | Out-Null
    }
    else {
        Write-Host "  FAILED:  $relativePath"

        $errorLines = Get-BuildErrorLines -OutputLines $output
        if (-not $errorLines) {
            $errorLines = $output | Select-Object -Last 20
        }

        $results.Add([pscustomobject]@{
                Project = $relativePath
                ExitCode = $exitCode
                Status = 'Failed'
                Errors = $errorLines
            }) | Out-Null

        if ($StopOnFirstFailure) {
            Write-Host ''
            Write-Host 'Stopping on first failure as requested.'
            break
        }
    }

    Write-Host ''
}

$failed = @($results | Where-Object { $_.Status -eq 'Failed' })
$passed = @($results | Where-Object { $_.Status -eq 'Success' })

Write-Host 'Build summary'
Write-Host "  Total attempted: $($results.Count)"
Write-Host "  Succeeded:       $($passed.Count)"
Write-Host "  Failed:          $($failed.Count)"
Write-Host ''

if ($failed.Count -gt 0) {
    Write-Host 'Errors by project:'
    foreach ($item in $failed) {
        Write-Host "- $($item.Project)"
        foreach ($line in ($item.Errors | Select-Object -First 30)) {
            Write-Host "    $line"
        }
        if ($item.Errors.Count -gt 30) {
            Write-Host '    ... (truncated)'
        }
        Write-Host ''
    }

    exit 1
}

exit 0
