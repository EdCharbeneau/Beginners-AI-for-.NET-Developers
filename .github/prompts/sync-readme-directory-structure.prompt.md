---
mode: agent
description: Sync README directory tree to actual section .slnx and Start/End folders
---

Update a module README directory tree so it matches the actual files on disk.

Inputs:
- `moduleFolder`: absolute or workspace-relative path to the module folder (for example: `08 - Working with Text, Streaming and Image Content Types`).
- `readmePath` (optional): path to README file. Default: `<moduleFolder>/README.md`.

Goal:
- Replace the fenced tree block in the README with an accurate tree generated from the module's real section folders.

Include only these items in the tree:
1. Section folders that match `^\d+\.\d+\s-\s`.
2. The section `.slnx` file in each section folder.
3. Subfolders ending in ` - Start` and ` - End`.

Do not include anything else:
- no project files (`.csproj`),
- no extra folders,
- no unrelated root files.

Required output format:
- Use a fenced code block with tree characters (`├──`, `└──`, `│`).
- Root line is the module folder name.
- For each section line:
  - section folder line,
  - `.slnx` line,
  - ` - Start` line,
  - ` - End` line.
- If one of these entries does not exist, omit it.
- Preserve existing README prose outside the tree code block.

Execution rules:
1. Scan `moduleFolder` immediate child directories for section folders.
2. For each section, detect:
- `<SectionName>.slnx` in that section root,
- one folder ending with ` - Start`,
- one folder ending with ` - End`.
3. Build a deterministic tree sorted by section number/name ascending.
4. Replace only the README tree code block; do not rewrite unrelated content.
5. Keep line endings and markdown style consistent with the existing README.

Validation checklist before finishing:
- Tree reflects current filesystem.
- Only `.slnx`, `Start`, and `End` entries are shown under each section.
- No tabs are used for indentation in the tree.
- README contains a single up-to-date tree block.

Example invocation:
- `moduleFolder: 08 - Working with Text, Streaming and Image Content Types`
- `readmePath: 08 - Working with Text, Streaming and Image Content Types/README.md`
