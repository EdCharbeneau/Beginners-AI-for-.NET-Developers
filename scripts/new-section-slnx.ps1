[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory = $true)]
    [string]$TargetFolder,

    [string]$SectionPattern = '^\d+\.\d+\s-\s',

    [string]$StartFolderSuffix = ' - Start',

    [string]$EndFolderSuffix = ' - End'
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

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw 'dotnet CLI is required but was not found on PATH.'
}

$resolvedTarget = Resolve-Path -LiteralPath $TargetFolder
$targetPath = $resolvedTarget.Path

$sections = Get-ChildItem -LiteralPath $targetPath -Directory |
    Where-Object { $_.Name -match $SectionPattern } |
    Sort-Object Name

if (-not $sections) {
    Write-Host "No section folders matched pattern '$SectionPattern' under '$targetPath'."
    exit 0
}

$results = New-Object System.Collections.Generic.List[object]

foreach ($section in $sections) {
    $sectionName = $section.Name
    $sectionPath = $section.FullName
    $slnxPath = Join-Path $sectionPath ("$sectionName.slnx")

    $startProject = Get-ChildItem -LiteralPath $sectionPath -Recurse -Filter *.csproj |
        Where-Object { $_.Directory.Name.EndsWith($StartFolderSuffix) } |
        Select-Object -First 1

    $endProject = Get-ChildItem -LiteralPath $sectionPath -Recurse -Filter *.csproj |
        Where-Object { $_.Directory.Name.EndsWith($EndFolderSuffix) } |
        Select-Object -First 1

    if (-not $startProject -or -not $endProject) {
        $slnState = if (Test-Path -LiteralPath $slnxPath) { 'reused' } else { 'not-created' }
        $reason = 'missing Start or End project'
        Write-Host "[SKIP] $sectionName | slnx: $slnState | $reason"
        $results.Add([pscustomobject]@{
                Section = $sectionName
                Solution = $slnState
                StartProject = if ($startProject) { $startProject.FullName } else { $null }
                EndProject = if ($endProject) { $endProject.FullName } else { $null }
                Added = @()
                SkippedReason = $reason
            }) | Out-Null
        continue
    }

    $slnState = 'reused'
    if (-not (Test-Path -LiteralPath $slnxPath)) {
        if ($PSCmdlet.ShouldProcess($slnxPath, 'Create .slnx file')) {
            dotnet new sln --format slnx --name "$sectionName" --output "$sectionPath" | Out-Null
        }
        $slnState = 'created'
    }

    if (-not (Test-Path -LiteralPath $slnxPath)) {
        throw "Expected solution file not found after creation: $slnxPath"
    }

    $added = New-Object System.Collections.Generic.List[string]
    $alreadyPresent = New-Object System.Collections.Generic.List[string]

    foreach ($proj in @($startProject, $endProject)) {
        $relativeProjectPath = (Get-RelativePath -BasePath $sectionPath -TargetPath $proj.FullName).Replace('\', '/')
        $slnxContent = Get-Content -LiteralPath $slnxPath -Raw

        $pathToken = "Path=`"$relativeProjectPath`""
        if ($slnxContent -match [regex]::Escape($pathToken)) {
            $alreadyPresent.Add($relativeProjectPath) | Out-Null
            continue
        }

        if ($PSCmdlet.ShouldProcess($slnxPath, "Add project $relativeProjectPath")) {
            dotnet sln "$slnxPath" add "$($proj.FullName)" | Out-Null
        }
        $added.Add($relativeProjectPath) | Out-Null
    }

    Write-Host "[OK]   $sectionName | slnx: $slnState | added: $($added.Count) | already present: $($alreadyPresent.Count)"
    $results.Add([pscustomobject]@{
            Section = $sectionName
            Solution = $slnState
            StartProject = (Get-RelativePath -BasePath $sectionPath -TargetPath $startProject.FullName).Replace('\', '/')
            EndProject = (Get-RelativePath -BasePath $sectionPath -TargetPath $endProject.FullName).Replace('\', '/')
            Added = $added.ToArray()
            AlreadyPresent = $alreadyPresent.ToArray()
            SkippedReason = $null
        }) | Out-Null
}

Write-Host ''
Write-Host 'Summary:'
$results | Format-Table -AutoSize
