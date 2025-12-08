# AoC MCP Server

A simple F# command-line MCP server for Advent of Code puzzle instructions and LLM solving speed tracking.

## Features

- **Instruction Management**: Store and retrieve full Advent of Code puzzle instructions (both phases) as Markdown files or JSON
- **Speed Tracking**: Record and aggregate LLM solving times per day and phase
- **Simple CLI**: Easy-to-use command-line interface for listing, viewing, and recording data

## Directory Structure

```
aocMCP/
├── Program.fs              # Main F# implementation
├── aocMCP.fsproj           # Project file
├── instructions/           # Puzzle instructions
│   ├── Day01_phase1.md     # Phase 1 problem (Markdown format)
│   ├── Day01_phase2.md     # Phase 2 problem (Markdown format)
│   └── dayXX.json          # Legacy JSON format (optional)
└── speeds/                 # Speed tracking JSON files (dayXX.json)
```

## Usage

### List Available Days
```powershell
dotnet run list
# or simply:
dotnet run
```

### Show Day Instructions
```powershell
dotnet run show <day>
# Example:
dotnet run show 4
```

### Record Solving Speed
```powershell
dotnet run speed <day> <phase> <llm_name> <speed_ms>
# Example:
dotnet run speed 4 1 gpt4 1200
```

## Instruction File Format

Instructions can be stored in two formats:

### Markdown Format (Recommended)
Create two separate markdown files per day:
- `Day01_phase1.md` - Phase 1 problem statement
- `Day01_phase2.md` - Phase 2 problem statement

This format is easier to share and edit:

```markdown
# Day 1 - Phase 1

## Problem
Parse a list of numbers and find the sum...

## Example
...
```

### JSON Format (Legacy)
For backward compatibility, instructions can also be stored as JSON:

```json
{
  "day": 1,
  "phase1": "Full problem statement for phase 1...",
  "phase2": "Full problem statement for phase 2 (optional)..."
}
```

## File Structure

```
aocMCP/
├── Program.fs              # Main F# implementation
├── aocMCP.fsproj           # Project file
├── instructions/           # Puzzle instructions (DayXX_phase1.md, DayXX_phase2.md, or dayXX.json)
└── speeds/                 # Speed tracking JSON files (dayXX.json)
```

## Speed Tracking Format

Speed records are aggregated per day in JSON files:

```json
{
  "day": 4,
  "entries": [
    {
      "llm": "gpt4",
      "phase": 1,
      "speed_ms": 1200,
      "timestamp": "2025-12-07T17:51:59Z"
    },
    ...
  ]
}
```

## Building & Running

```powershell
# Build
cd aocMCP
dotnet build

# Run with arguments
dotnet run -- list
dotnet run -- show 4
dotnet run -- speed 4 1 gpt4 1200

# Or publish as standalone
dotnet publish -c Release
```

## Future MCP Integration

This server is designed to work with the [Model Context Protocol](https://modelcontextprotocol.io/) for integration with Claude and other AI tools. Future versions will add:

- Stdio transport for MCP communication
- Resource definitions for instructions
- Tool definitions for speed recording
- Full MCP server lifecycle management
