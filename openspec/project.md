# Project Context

## Purpose
Advent of Code 2025 puzzle solutions repository with AI agent orchestration experiments. Testing multi-agent systems where different AI models (GPT-4, GPT-5, Claude Opus 4.5, Gemini, Grok) compete to solve daily programming puzzles.

## Tech Stack
- **F# (.NET 10 SDK)** - Primary language for puzzle solutions
- **PowerShell** - Scripting, automation, and memory management
- **MCP (Model Context Protocol) Server** - Custom F# server for managing puzzle instructions and performance tracking
- **F# Script Files (.fsx)** - Individual puzzle solutions (day01.fsx, day02_[agent].fsx, etc.)

## Project Conventions

### Code Style
- **Language**: F# functional programming exclusively
  - Immutability by default
  - Pipeline operators (`|>`, `||>`)
  - Higher-order functions (`fold`, `map`, `filter`)
  - Pattern matching over conditionals
- **Naming**: 
  - Files: `dayXX.fsx` for main solutions, `dayXX_[agent].fsx` for agent-specific implementations
  - Input files: `input/inputXX.txt`, `input/inputXX_example.txt`
- **Communication Style**: Concise, direct, blunt - no fluff

### Architecture Patterns
- **Agent Orchestration**: Multi-agent system with specialized roles
  - Orchestrator agent: Coordinates subagents, fetches instructions via MCP
  - Implementor agents: Generate solutions (one per AI model)
  - Result Gatherer: Stores execution results in memory system
  - Verificator: Validates solutions
- **Memory System**: JSON-based memory storage for execution results only
  - Location: `memory/` directory
  - Manager: `memory/memory-manager.ps1`
  - Store results only, NOT instructions or decision logic
- **MCP Server**: Custom F# server (`aocMCP/`) for:
  - Fetching daily puzzle instructions
  - Recording solving speeds per agent
  - Managing instruction files (Markdown and JSON formats)

### Testing Strategy
- Solutions tested against example inputs first (`inputXX_example.txt`)
- Verification against actual puzzle inputs (`inputXX.txt`)
- Multiple agent solutions compared for correctness
- Performance tracking via MCP speed recording

### Git Workflow
- Main branch: `main`
- Repository: `aoc2025` (owner: Rinorragi)
- Daily commits as puzzles are solved
- Agent-specific solution files retained for comparison

## Domain Context
- **Advent of Code**: Daily programming puzzles released December 1-25
- **Two-phase puzzles**: Each day has Part 1 and Part 2
- **Agent isolation**: Each agent should work independently without accessing other agents' code
- **No cross-agent learning**: Agents should not read aocMCP files directly
- **Blocking subagents**: GitHub Copilot subagents run sequentially, not in parallel (known limitation)

## Important Constraints
- **Environment**: Windows PowerShell only (no bash/Linux commands)
- **No parallel subagents**: Due to VS Code API limitation, agents cannot truly run in parallel
- **Agent privileges**: Orchestrator leaks all privileges to subagents - rely on prompt engineering
- **Memory scope**: Only execution results stored, not architectural decisions or instructions
- **Input management**: Puzzle inputs must be manually added to `input/` folder

## External Dependencies
- **.NET 10 SDK**: Required for F# script execution
- **Advent of Code API**: Source of daily puzzle instructions
- **GitHub Copilot**: Multi-agent orchestration platform
- **VS Code**: Development environment with agent support
