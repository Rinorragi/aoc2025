# VS Code Workspace Setup

This workspace is configured with MCP (Model Context Protocol) server integration for Advent of Code puzzle management.

## Opening the Workspace

Open the workspace file instead of just the folder:
```powershell
code aoc2025.code-workspace
```

Or in VS Code:
- File → Open Workspace from File → Select `aoc2025.code-workspace`

## MCP Server Configuration

The workspace includes configuration for the **aocMCP** server located in the `aocMCP/` subfolder.

### Server Details

- **Name**: aoc
- **Command**: `dotnet run mcp`
- **Working Directory**: `aocMCP/`
- **Protocol**: JSON-RPC 2.0 over stdio

### Available Resources

Once connected, the MCP server provides:

#### Resources
- `aoc://day01` through `aoc://day25` - Full puzzle instructions for each day

#### Tools
- `fetch_instruction` - Retrieve puzzle instructions for a specific day
- `record_speed` - Record LLM solving times with day, phase, LLM name, and milliseconds

### Using the Server

The server automatically starts when you interact with it in Claude via the VS Code Copilot extension.

To manually test:
```powershell
cd aocMCP
dotnet run mcp
# Then send JSON-RPC messages to stdin
echo '{"jsonrpc":"2.0","method":"resources/list","id":1}' | dotnet run mcp
```

## Workspace Structure

```
aoc2025/
├── aoc2025.code-workspace          # Workspace configuration
├── .vscode/
│   ├── settings.json               # Workspace settings with MCP config
│   └── extensions.json             # Recommended extensions
├── aocMCP/                         # MCP Server (separate folder)
│   ├── Program.fs                  # Server implementation
│   ├── aocMCP.fsproj              # Project file
│   ├── instructions/               # Puzzle instruction files
│   └── speeds/                     # Speed tracking data
├── day01.fsx through day04.fsx    # F# solution scripts
├── input/                          # Puzzle input files
└── memory/                         # Custom memory system
```

## Development

- **Language**: F# and PowerShell
- **Framework**: .NET 10.0
- **Key Tools**: Ionide for F# support, MCP for agent integration

## Notes

- The `memory/` directory is excluded from file watching to reduce overhead
- Binary directories (`bin/`, `obj/`) are excluded from search and watching
- MCP server runs in the `aocMCP` workspace folder context
