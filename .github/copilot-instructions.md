# AI Instructions

- Use F# scripts (`.fsx` files)
- Prefer functional programming: immutability, pure functions, pipelines, `fold`, `map`, `filter`
- Avoid mutable state and loops
- Be succinct

# Environment

- **OS**: Windows with PowerShell (pwsh.exe)
- **Shell**: Always use PowerShell commands, NEVER use Linux/bash commands
- Use PowerShell cmdlets: `Get-ChildItem` (not `ls`), `Test-Path` (not `test`), `Copy-Item` (not `cp`), `Remove-Item` (not `rm`), `Move-Item` (not `mv`)
- Use forward slash `/` OR escaped backslash `\\` in paths, or use PowerShell variables like `$PSScriptRoot`, `$PWD`
- For file operations, prefer PowerShell cmdlets over shell syntax
- Use semicolons `;` to chain commands in PowerShell, not `&&`
- Use backticks for line continuation in PowerShell when needed
- Don't use following linux commands: `cat`, `echo`, `grep`, `sed`, `awk`, `head`, `tail`, `find`, `xargs`, `chmod`, `chown`, `curl`, `wget`, `tar`, `zip`, `unzip`, `ls`, `cd`, `pwd`, `mv`, `cp`, `rm`, `len`, `test`, `date`, `time`, `which`, `man`, `top`, `ps`, `kill` instead use PowerShell equivalents


# Communication

- Be concise
- Remove fluffyness, be direct, be brief, be blunt

# Memory System

- Use custom memory system instead of built-in memory
- Memory script: `memory/memory-manager.ps1`
- Store: `.\memory\memory-manager.ps1 -Action store -Title "title" -Content "content"`
- Search: `.\memory\memory-manager.ps1 -Action search -Query "keyword"`
- Get: `.\memory\memory-manager.ps1 -Action get -Id "guid"`
- List: `.\memory\memory-manager.ps1 -Action list`

# Instruction Fetching

## Strategy: Three-tier fallback chain

When scripts need AoC puzzle instructions, use this priority:

1. **Primary: MCP Server** - Fetch from `aocMCP/instructions/dayXX.json`
   - Fastest, most reliable source
   - Return immediately if found

2. **Fallback: Memory System** - Search `memory/` via memory-manager.ps1
   - If MCP doesn't have it, search memory
   - Enables offline access

3. **Auto-sync**: When instructions found in memory but not MCP
   - Store fetched instructions to MCP server
   - Ensures future runs use faster MCP source
   - Non-blocking operation
