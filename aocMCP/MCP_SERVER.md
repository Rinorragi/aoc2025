# aocMCP - Advent of Code MCP Server

A local Model Context Protocol (MCP) server for managing Advent of Code 2025 puzzle instructions and tracking LLM solving speeds.

## Running the Server

Start the MCP server on stdio:

```bash
dotnet run mcp
```

The server will listen for JSON-RPC 2.0 messages on stdin and write responses to stdout.

## Supported Methods

### Resources

#### `resources/list`
List all available puzzle instructions.

**Request:**
```json
{"jsonrpc":"2.0","method":"resources/list","id":1}
```

**Response:**
```json
{"jsonrpc":"2.0","id":1,"result":{"resources":[
  {"uri":"aoc://day01","name":"Day 1 Instructions","description":"...","mimeType":"text/plain"},
  ...
]}}
```

#### `resources/read`
Fetch the full puzzle instruction for a specific day.

**Request:**
```json
{"jsonrpc":"2.0","method":"resources/read","params":{"uri":"aoc://day01"},"id":2}
```

**Response:**
```json
{"jsonrpc":"2.0","id":2,"result":{"contents":[
  {"uri":"aoc://day01","mimeType":"text/plain","text":"# Day 1\n\n## Phase 1\n..."}
]}}
```

### Tools

#### `tools/list`
List all available tools.

**Request:**
```json
{"jsonrpc":"2.0","method":"tools/list","id":3}
```

**Response:**
```json
{"jsonrpc":"2.0","id":3,"result":{"tools":[
  {"name":"fetch_instruction","description":"...","inputSchema":{...}},
  {"name":"record_speed","description":"...","inputSchema":{...}}
]}}
```

#### `tools/call`

##### fetch_instruction
Fetch puzzle instructions for a specific day.

**Request:**
```json
{"jsonrpc":"2.0","method":"tools/call","params":{"name":"fetch_instruction","arguments":{"day":1}},"id":4}
```

**Response:**
```json
{"jsonrpc":"2.0","id":4,"result":{"content":[{"type":"text","text":"# Day 1\n..."}]}}
```

##### record_speed
Record the solving time for an LLM on a specific day and phase.

**Request:**
```json
{"jsonrpc":"2.0","method":"tools/call","params":{"name":"record_speed","arguments":{"day":1,"phase":1,"llm_name":"gpt-4","speed_ms":1250}},"id":5}
```

**Response:**
```json
{"jsonrpc":"2.0","id":5,"result":{"content":[{"type":"text","text":"Recorded gpt-4 Day 1 Phase 1: 1250ms (avg: 1250ms)"}]}}
```

## CLI Commands

```bash
# List available instruction days
dotnet run list

# Show instructions for a specific day
dotnet run show 1

# Record solving speed for an LLM
dotnet run speed 1 1 gpt-4 1250

# Start MCP server on stdio
dotnet run mcp
```

## Data Storage

- **Instructions**: `instructions/day01.json` - `day25.json`
  - Format: `{day: int, phase1: string, phase2: string | null}`
  - Full problem text for each phase

- **Speeds**: `speeds/day01.json` - `day25.json` (created at runtime)
  - Format: `{day: int, entries: [{llm, phase, speed_ms, timestamp}, ...]}`
  - Aggregates all LLM solving times per day

## Integration with Claude Desktop

To use with Claude Desktop, add this to `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "aoc": {
      "command": "dotnet",
      "args": ["run", "mcp"],
      "cwd": "/path/to/aoc2025/aocMCP"
    }
  }
}
```

Then Claude can:
- Fetch Advent of Code puzzle instructions
- Record and track solving times for different LLMs
- Reference historical performance data while solving puzzles
