---
mode: agent
description: Create .slnx files for section folders and add Start/End projects
---

Create .slnx files for every section under a target folder and add both Start and End projects.

Inputs:
- `targetFolder`: absolute or workspace-relative path to the module folder.

Rules:
1. Treat each immediate child directory of `targetFolder` whose name starts with `N.N - ` as a section folder.
2. In each section folder, create `<Section Folder Name>.slnx` if missing.
3. Find one Start project and one End project:
- Start project: first `.csproj` under a subfolder name ending with ` - Start`.
- End project: first `.csproj` under a subfolder name ending with ` - End`.
4. Add both projects to the section `.slnx`.
5. Be idempotent: do not fail if solution exists or project is already added.
6. Print a per-section result summary:
- created or reused solution file
- start/end project detected
- projects added or already present
- skipped sections and reason

Preferred implementation:
- Use PowerShell and `dotnet` CLI.
- Use `Test-Path`, `Get-ChildItem`, and `dotnet sln` commands.
- Do not modify unrelated files.

PowerShell template to run (adapt and execute):

```powershell
param(
  [Parameter(Mandatory=$true)]
  [string]$TargetFolder
)

$target = Resolve-Path $TargetFolder
$sections = Get-ChildItem -Path $target -Directory |
  Where-Object { $_.Name -match '^\d+\.\d+\s-\s' }

foreach ($section in $sections) {
  $sectionPath = $section.FullName
  $sectionName = $section.Name
  $slnxPath = Join-Path $sectionPath ("$sectionName.slnx")

  if (-not (Test-Path $slnxPath)) {
    dotnet new sln --format slnx --name "$sectionName" --output "$sectionPath" | Out-Null
    $slnState = "created"
  }
  else {
    $slnState = "reused"
  }

  $startProject = Get-ChildItem -Path $sectionPath -Recurse -Filter *.csproj |
    Where-Object { $_.Directory.Name -match ' - Start$' } |
    Select-Object -First 1

  $endProject = Get-ChildItem -Path $sectionPath -Recurse -Filter *.csproj |
    Where-Object { $_.Directory.Name -match ' - End$' } |
    Select-Object -First 1

  if (-not $startProject -or -not $endProject) {
    Write-Host "[SKIP] $sectionName | slnx: $slnState | missing Start or End project"
    continue
  }

  $slnText = Get-Content $slnxPath -Raw

  foreach ($proj in @($startProject, $endProject)) {
    if ($slnText -notmatch [regex]::Escape($proj.Name)) {
      dotnet sln "$slnxPath" add "$($proj.FullName)" | Out-Null
    }
  }

  Write-Host "[OK]   $sectionName | slnx: $slnState | start: $($startProject.Name) | end: $($endProject.Name)"
}
```

Example invocation target:
- `08 - Working with Text, Streaming and Image Content Types`
