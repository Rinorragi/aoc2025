# AI Instructions - Simplified & Isolated

## Core Rules (All Agents)
- **Language**: F# functional programming (immutability, pipelines, `fold`/`map`/`filter`)
- **Environment**: Windows PowerShell only (no bash/Linux)
- **Communication**: Be concise, direct, blunt

## Agent-Specific Instructions
Each agent has specific instructions in `.github/agents/[agent-name].agent.md`:
Each agent has specific instructions in `.github/agents/[agent-name].agent.md`:
- **Access**: Orchestrator fetches via MCP tool, passes to implementors
- **DO NOT** read aocMCP files directly

## Memory System (Results Only)
Result Gatherer stores execution results via:
```powershell
.\memory\memory-manager.ps1 -Action store -Title "dayXX_[agent]" -Content "execution output"
```

DO NOT store instructions, decision logic, or architectural rules in memory.
