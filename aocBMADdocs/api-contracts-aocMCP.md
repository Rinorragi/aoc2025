# API Contracts - aocMCP Server

Generated: 2025-12-12  
Part: aocMCP (Backend)  
Protocol: Model Context Protocol (MCP) via JSON-RPC 2.0

---

## Overview

The aocMCP server be a custom implementation of the Model Context Protocol, providing Advent of Code puzzle instructions and speed tracking through a STDIO-based JSON-RPC interface.

---

## Communication Protocol

**Transport**: STDIO (Standard Input/Output)  
**Protocol**: JSON-RPC 2.0  
**Line Format**: Newline-delimited JSON messages  
**MCP Version**: 2024-11-05

---

## Server Capabilities

```json
{
  "protocolVersion": "2024-11-05",
  "capabilities": {
    "resources": {
      "subscribe": false,
      "listChanged": false
    },
    "tools": {},
    "prompts": {}
  },
  "serverInfo": {
    "name": "aocMCP",
    "version": "1.0.0"
  }
}
```

---

## RPC Methods

### 1. initialize

**Purpose**: Initialize the MCP server connection  
**Request**:
```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "initialize",
  "params": {}
}
```

**Response**:
```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "result": {
    "protocolVersion": "2024-11-05",
    "capabilities": { ... },
    "serverInfo": { "name": "aocMCP", "version": "1.0.0" }
  }
}
```

---

### 2. resources/list

**Purpose**: List all available puzzle instruction resources  
**Request**:
```json
{
  "jsonrpc": "2.0",
  "id": 2,
  "method": "resources/list",
  "params": {}
}
```

**Response**:
```json
{
  "jsonrpc": "2.0",
  "id": 2,
  "result": {
    "resources": [
      {
        "uri": "aoc://day01",
        "name": "Day 1 Instructions",
        "description": "Full puzzle instructions for Advent of Code 2025 Day 1",
        "mimeType": "text/plain"
      },
      ...
    ]
  }
}
```

**Resource URI Pattern**: `aoc://day{XX}` where XX be the zero-padded day number (01-25)

---

### 3. resources/read

**Purpose**: Read the full instruction content fer a specific day  
**Request**:
```json
{
  "jsonrpc": "2.0",
  "id": 3,
  "method": "resources/read",
  "params": {
    "uri": "aoc://day01"
  }
}
```

**Response**:
```json
{
  "jsonrpc": "2.0",
  "id": 3,
  "result": {
    "contents": [
      {
        "uri": "aoc://day01",
        "mimeType": "text/plain",
        "text": "# Day 1\n\n## Phase 1\n...\n\n## Phase 2\n..."
      }
    ]
  }
}
```

**Content Format**:
- Phase 1 only: `# Day {X}\n\n{phase1_content}`
- Both phases: `# Day {X}\n\n## Phase 1\n{phase1}\n\n## Phase 2\n{phase2}`
- Line endings: Normalized to Unix (\\n)

**Error Response**:
```json
{
  "jsonrpc": "2.0",
  "id": 3,
  "error": {
    "code": -1,
    "message": "Error loading instruction: ..."
  }
}
```

---

### 4. prompts/list

**Purpose**: List available prompts (currently empty)  
**Request**:
```json
{
  "jsonrpc": "2.0",
  "id": 4,
  "method": "prompts/list",
  "params": {}
}
```

**Response**:
```json
{
  "jsonrpc": "2.0",
  "id": 4,
  "result": {
    "prompts": []
  }
}
```

---

### 5. tools/list

**Purpose**: List available tools fer interaction  
**Request**:
```json
{
  "jsonrpc": "2.0",
  "id": 5,
  "method": "tools/list",
  "params": {}
}
```

**Response**:
```json
{
  "jsonrpc": "2.0",
  "id": 5,
  "result": {
    "tools": [
      {
        "name": "fetch_instruction",
        "description": "Fetch the full puzzle instruction for a specific day",
        "inputSchema": {
          "type": "object",
          "properties": {
            "day": {
              "type": "integer",
              "description": "Day number (1-25)"
            }
          },
          "required": ["day"]
        }
      },
      {
        "name": "record_speed",
        "description": "Record the solving speed for an LLM on a specific day and phase",
        "inputSchema": {
          "type": "object",
          "properties": {
            "day": { "type": "integer", "description": "Day number" },
            "phase": { "type": "integer", "description": "Phase (1 or 2)" },
            "llm_name": { "type": "string", "description": "LLM name" },
            "speed_ms": { "type": "integer", "description": "Solving time in milliseconds" }
          },
          "required": ["day", "phase", "llm_name", "speed_ms"]
        }
      }
    ]
  }
}
```

---

### 6. tools/call

**Purpose**: Execute a tool by name with arguments

#### Tool: fetch_instruction

**Request**:
```json
{
  "jsonrpc": "2.0",
  "id": 6,
  "method": "tools/call",
  "params": {
    "name": "fetch_instruction",
    "arguments": {
      "day": 1
    }
  }
}
```

**Response**:
```json
{
  "jsonrpc": "2.0",
  "id": 6,
  "result": {
    "content": [
      {
        "type": "text",
        "text": "# Day 1\\n\\n## Phase 1\\n...\\n\\n## Phase 2\\n..."
      }
    ]
  }
}
```

#### Tool: record_speed

**Request**:
```json
{
  "jsonrpc": "2.0",
  "id": 7,
  "method": "tools/call",
  "params": {
    "name": "record_speed",
    "arguments": {
      "day": 1,
      "phase": 1,
      "llm_name": "gpt4",
      "speed_ms": 1250
    }
  }
}
```

**Response**:
```json
{
  "jsonrpc": "2.0",
  "id": 7,
  "result": {
    "content": [
      {
        "type": "text",
        "text": "Recorded gpt4 Day 1 Phase 1: 1250ms (avg: 1180ms)"
      }
    ]
  }
}
```

---

## Error Codes

| Code | Message | Description |
|------|---------|-------------|
| -32700 | Parse error | Invalid JSON received |
| -32601 | Method not found | Unknown RPC method |
| -32602 | Invalid params | Missing or invalid parameters |
| -1 | Custom error | Application-specific error (see message) |

---

## Data Models

### InstructionData (Internal)
```fsharp
type InstructionData = {
    Day: int
    Phase1: string
    Phase2: string option
}
```

### SpeedEntry
```fsharp
type SpeedEntry = {
    LLM: string
    Phase: int
    SpeedMs: int64
    Timestamp: string  // ISO 8601 format
}
```

### DaySpeedStats
```fsharp
type DaySpeedStats = {
    Day: int
    Entries: SpeedEntry list
}
```

---

## File Storage

### Instructions
**Location**: `instructions/`  
**Formats**:
1. **Markdown** (preferred): `Day{XX}_phase1.md`, `Day{XX}_phase2.md`
2. **JSON** (legacy): `day{XX}.json`

**JSON Format**:
```json
{
  "day": 1,
  "phase1": "...",
  "phase2": "..."
}
```

### Speed Data
**Location**: `speeds/`  
**Format**: `day{XX}.json`

**Structure**:
```json
{
  "day": 1,
  "entries": [
    {
      "llm": "gpt4",
      "phase": 1,
      "speedMs": 1250,
      "timestamp": "2025-12-01T10:30:00.000Z"
    },
    ...
  ]
}
```

---

## Integration Points

### VS Code Configuration
```json
{
  "servers": {
    "aocMCP": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "${workspaceFolder}/aocMCP/aocMCP.fsproj"
      ]
    }
  }
}
```

### Usage from AI Agents
1. List available days: `resources/list`
2. Fetch puzzle instructions: `resources/read` or `tools/call` with `fetch_instruction`
3. Record solution speed: `tools/call` with `record_speed`

---

## Known Limitations

1. **No Subscriptions** - Resource changes don't trigger notifications
2. **No Prompts** - Prompt capability not implemented
3. **Line Ending Normalization** - Windows-style CRLF converted to Unix LF fer JSON compatibility
4. **Sequential Processing** - One request at a time via STDIO
5. **No Authentication** - Local-only, trust-based access

---

*All endpoints tested and verified through the Daily Challenge workflow, Captain!* ⚓
