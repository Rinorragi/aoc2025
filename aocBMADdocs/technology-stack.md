# Technology Stack Analysis

Generated: 2025-12-12  
Project: Advent of Code 2025 Solutions  
Repository Type: Monorepo (3 parts)

---

## Part 1: aocMCP (MCP Server)

### Technology Table

| Category | Technology | Version | Justification |
|----------|-----------|---------|---------------|
| Runtime | .NET | 10.0.101 | Modern .NET platform for F# development |
| Language | F# | 10.0 | Functional-first language, excellent for puzzle solving and server development |
| Framework | ModelContextProtocol | 0.5.0-preview.1 | Custom MCP server implementation for VS Code integration |
| Build Tool | MSBuild | (via .NET SDK) | Standard .NET project build system |
| Data Format | JSON | System.Text.Json | Serialization for puzzle instructions and speed tracking |
| Protocol | JSON-RPC | (MCP standard) | Communication protocol for MCP server |

### Architecture Pattern
**Backend Service (MCP Server)**
- STDIO-based JSON-RPC server
- Request/response pattern fer puzzle instruction delivery
- File-based storage fer instructions (markdown/JSON) and speed data
- RESTful-style resource access pattern (tools/resources)

---

## Part 2: Main AoC Solutions

### Technology Table

| Category | Technology | Version | Justification |
|----------|-----------|---------|---------------|
| Runtime | .NET | 10.0.101 | F# scripting execution environment |
| Language | F# Scripts (.fsx) | 10.0 | Interactive F# scripting fer rapid puzzle solving |
| Scripting Engine | F# Interactive (FSI) | 10.0 | Command-line REPL fer running .fsx files |
| Orchestration | PowerShell | 7.x | Automation, parallel execution, workflow management |
| Data Storage | JSON | PowerShell/F# | Memory system persistence |
| Code Formatter | Fantomas | 7.0.5 | F# code formatting tool |

### Agent Ecosystem

| Agent | Purpose | Implementation |
|-------|---------|----------------|
| GPT-4 | Puzzle solver | day{XX}_gpt4.fsx |
| GPT-5 | Puzzle solver | day{XX}_gpt5.fsx |
| Claude Opus 4.5 | Puzzle solver | day{XX}_claudeopus45.fsx |
| Gemini 3 | Puzzle solver | day{XX}_gemini.fsx |
| Grok | Puzzle solver | day{XX}_grok.fsx |

### Architecture Pattern
**Multi-Agent Data Processing Pipeline**
- Script-based functional programming approach
- Each puzzle day has 5 competing agent implementations
- PowerShell orchestration coordinates parallel/sequential execution
- Memory system (JSON-based) tracks results and agent performance
- Input data stored in `input/` directory (dayXX.txt, dayXX_example.txt)

### Key Components
1. **Puzzle Solutions** - 45+ F# script files (dayXX_*.fsx)
2. **Orchestration** - PowerShell scripts (run-aoc.ps1, generated_scripts/)
3. **Memory System** - PowerShell-based JSON storage (memory/memory-manager.ps1)
4. **Input Management** - Text files with puzzle inputs

---

## Part 3: aocSPEC (Spec-Kit Experimentation)

### Technology Table

| Category | Technology | Version | Justification |
|----------|-----------|---------|---------------|
| Runtime | .NET | 10.0 | F# script execution |
| Language | F# Scripts | 10.0 | Spec-driven puzzle solving |
| Framework | Spec-Kit | latest | GitHub's spec-driven AI development framework |
| Templates | F# | custom | Day templates fer consistent puzzle structure |

### Architecture Pattern
**Spec-Driven Development**
- Template-based code generation
- Specification-first approach to puzzle solving
- Isolated experimentation environment
- Shares MCP server with main repository

---

## Cross-Cutting Technologies

### AI/ML Frameworks

| Framework | Version | Purpose |
|-----------|---------|---------|
| BMAD | 6.0.0-alpha.16 | Enterprise AI development method (~300 files) |
| OpenSpec | latest | Brownfield specification framework |
| Spec-Kit | latest | Greenfield specification framework |

### Development Tools

| Tool | Purpose |
|------|---------|
| VS Code | Primary IDE with AI agent integration |
| MCP Protocol | AI<->Tool communication |
| GitHub Copilot | AI coding assistant |
| PowerShell | Automation and scripting |

### Configuration Management

| Config File | Purpose |
|-------------|---------|
| .vscode/settings.json | MCP server configuration |
| .config/dotnet-tools.json | .NET local tools (Fantomas) |
| mcp.json | MCP server settings (STDIO mode) |
| .bmad/core/config.yaml | BMAD framework config |
| openspec/project.md | OpenSpec project metadata |

---

## Key Patterns & Conventions

### Functional Programming
- Immutability by default
- Pipeline operators (`|>`)
- Pattern matching fer control flow
- Higher-order functions (map, fold, filter)

### Multi-Agent Architecture
- 5 competing AI agents per puzzle
- Sequential implementation (subAgent blocking limitation)
- Parallel result gathering via PowerShell jobs
- Speed tracking and comparison

### Memory Management
- JSON-based persistence (GUID-based IDs)
- PowerShell script interface (store/search/get/list)
- Scoped to avoid context pollution
- Intentionally poisonable fer agent manipulation testing

### File Naming Conventions
- Puzzle solutions: `day{XX}_{agent}.fsx`
- Input files: `input{XX}.txt`, `input{XX}_example.txt`
- Generated scripts: `run_day{XX}_parallel.ps1`
- Instructions: `Day{XX}_phase1.md`, `Day{XX}_phase2.md`

---

## Development Workflow Evolution

### Days 1-3: Basic Setup
- Direct AI generation
- Manual execution
- Learning AI behavior patterns

### Days 4-6: Orchestration Introduction
- Memory system implementation
- Multi-agent coordination
- MCP server development

### Days 7-9: Context Management Challenges
- Instruction complexity led to "context collapse"
- Simplified agent instructions
- Fixed MCP server STDIO mode

### Days 10-12: Framework Experimentation
- **Day 10**: Spec-Kit integration
- **Day 11**: OpenSpec integration
- **Day 12**: BMAD framework (current)

---

## Technical Debt & Known Issues

1. **SubAgent Blocking** - Can't truly run agents in parallel (VS Code limitation)
2. **Instruction Leakage** - Agents sometimes ignore or bypass instructions
3. **Context Collapse** - Complex instruction sets lead to degraded performance
4. **Memory Poisoning** - By design, but demonstrates security concerns
5. **Framework Overhead** - BMAD added ~300 files fer enterprise patterns

---

## Dependencies Summary

### Runtime Dependencies
- .NET 10 SDK (required)
- PowerShell 7+ (automation)
- Node.js (fer OpenSpec tooling)
- Python uv (fer Spec-Kit tooling)

### Package Dependencies
- ModelContextProtocol 0.5.0-preview.1
- System.Text.Json (standard library)

### Tool Dependencies
- Fantomas 7.0.5 (F# formatter)
- OpenSpec CLI (npm global)
- Spec-Kit CLI (uv tool)
- BMAD Method (npx)

---

*This be yer complete technology manifest, Captain! Every piece of treasure catalogued proper-like!* 🏴‍☠️
