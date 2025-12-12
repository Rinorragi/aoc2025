# AoC 2025 Project Overview (Pirate Translation)

**Ahoy! Welcome to the Advent of Code 2025 Repository - Where AI Agents Battle Fer Puzzle Supremacy!**

## 🏴‍☆ Executive Summary

This here be a **brownfield monorepo** experimentin' with enterprise AI development methods while solvin' Advent of Code 2025 puzzles. The project be testin' three different frameworks (BMAD, OpenSpec, Spec-Kit) and orchestratin' **5 competing AI agents** (GPT-4, GPT-5, Claude Opus 4.5, Gemini, Grok) across 11 completed days o' puzzles.

**Project Type:** Monorepo (3 architectural parts)  
**Primary Language:** F# 10.0 (.NET 10.0.101)  
**Orchestration:** PowerShell 7.x  
**Status:** Active development (Day 12 in progress)  
**Documentation Date:** 2025-12-12

---

## 🗺️ The Three Parts o' This Vessel

### **Part 1: aocMCP/** - The Backend Service
A custom **Model Context Protocol (MCP) server** that exposes puzzle instructions and speed tracking to AI agents via JSON-RPC over STDIO.

- **Technology:** F# + ModelContextProtocol 0.5.0-preview.1
- **Integration:** VS Code MCP client configuration
- **Key Features:**
  - Puzzle instruction retrieval (markdown-first, JSON fallback)
  - LLM speed tracking and statistics
  - 6 RPC methods + 2 tools
  - File-based storage (instructions/, speeds/)

**Entry Point:** `aocMCP/Program.fs`

### **Part 2: Main AoC Solutions/** - The Multi-Agent Arena
45+ F# script files (`.fsx`) where 5 AI agents compete to solve puzzles fastest and most elegantly.

- **Technology:** F# Interactive (FSI) + PowerShell orchestration
- **Key Components:**
  - `day##_[agent].fsx` - Individual agent solutions
  - `run-aoc.ps1` - Generic timeout wrapper
  - `generated_scripts/run_day##_parallel.ps1` - Parallel agent execution
  - `memory/memory-manager.ps1` - JSON-based execution memory
  - `input/` - Puzzle inputs and examples

**Entry Point:** `run-aoc.ps1` with parameters

### **Part 3: aocSPEC/** - The Spec-Kit Experiment
A separate Spec-Kit installation fer experimentin' with spec-driven development on Day 10.

- **Technology:** Spec-Kit framework + F# templates
- **Key Components:**
  - `.specify/` - Spec-Kit configuration
  - `templates/` - Day templates
  - `specs/` - Puzzle specifications
  - `day10.fsx` - Spec-driven solution

**Entry Point:** `aocSPEC/day10.fsx`

---

## 🧭 Framework Journey (Days 1-12)

| Day | Framework | Approach | Status |
|-----|-----------|----------|--------|
| 1-9 | **Manual** | Direct F# scripts, no framework | ✅ Complete |
| 10 | **Spec-Kit** | Template-driven spec system | ✅ Complete |
| 11 | **OpenSpec** | Change proposal + spec workflow | ✅ Complete (archived) |
| 12 | **BMAD** | Enterprise AI method + documentation | 🔄 In Progress |

---

## 🏗️ Architecture Patterns

### **Multi-Agent Competition Pattern**
```
User Request → Orchestrator Agent → [GPT-4, GPT-5, Claude, Gemini, Grok]
                                           ↓
                                    Parallel Execution
                                           ↓
                                  Compare Results/Speed
                                           ↓
                                   Select Best Solution
```

### **MCP Integration Pattern**
```
VS Code → MCP Client → STDIO → aocMCP Server
                                    ↓
                           [Puzzle Instructions]
                           [Speed Tracking]
                                    ↓
                              File Storage
```

### **Memory System Pattern**
```
Execution Result → memory-manager.ps1 → GUID.json
                                           ↓
                                   Timestamped Storage
                                           ↓
                               Search/Retrieve by Query
```

---

## 🔧 Key Technologies

**Core Stack:**
- F# 10.0 (functional scripting)
- .NET 10.0.101 (runtime)
- PowerShell 7.x (orchestration)
- FSI (F# Interactive)

**Frameworks:**
- BMAD v6.0.0-alpha.16 (~300 files)
- OpenSpec (change proposal system)
- Spec-Kit (template-driven specs)

**Protocols:**
- Model Context Protocol (MCP) 2024-11-05
- JSON-RPC 2.0 over STDIO

**Tools:**
- Fantomas 7.0.5 (F# formatter)
- VS Code MCP extension
- Git version control

---

## 📂 Repository Structure (High-Level)

```
aoc2025/
├── aocMCP/              # MCP server (backend)
│   ├── Program.fs       # Server entry point
│   ├── instructions/    # Puzzle data (markdown/JSON)
│   └── speeds/          # LLM performance tracking
│
├── day##_[agent].fsx    # 45+ puzzle solutions (5 agents × 9 days)
├── run-aoc.ps1          # Generic execution wrapper
├── generated_scripts/   # Parallel agent runners
│
├── memory/              # JSON-based execution memory
│   ├── memory-manager.ps1
│   └── *.json           # GUID-named memory entries
│
├── input/               # Puzzle inputs and examples
│
├── aocSPEC/             # Spec-Kit experimentation
│   ├── .specify/        # Framework config
│   ├── templates/       # Day templates
│   └── specs/           # Puzzle specs
│
├── openspec/            # OpenSpec framework
│   ├── AGENTS.md        # Agent instructions
│   ├── specs/           # Active specs
│   └── changes/         # Archived proposals
│
├── .bmad/               # BMAD framework (~300 files)
│   └── bmm/             # Business Model Manager
│       ├── agents/      # Analyst/Orchestrator/etc.
│       └── workflows/   # Document-project workflow
│
└── docs/                # Additional documentation
```

---

## 🚀 Getting Started (Quick Reference)

### **Run a Puzzle Solution**
```powershell
.\run-aoc.ps1 -Day "11" -Part 2 -TimeoutSeconds 300
```

### **Run All Agents in Parallel**
```powershell
.\generated_scripts\run_day11_parallel.ps1
```

### **Start MCP Server**
```powershell
cd aocMCP
dotnet run
```

### **Store Execution Memory**
```powershell
.\memory\memory-manager.ps1 -Action store -Title "day11_gpt4" -Content "Result: 358458157650450"
```

### **Search Memory**
```powershell
.\memory\memory-manager.ps1 -Action search -Query "day11"
```

---

## 🎯 Current Status (Day 12)

**Active Work:** BMAD framework documentation generation  
**Agent:** Mary (Analyst, Pirate English mode)  
**Workflow:** document-project (deep scan, 12 steps)  
**Progress:** Step 4 of 12 (conditional analysis in progress)

**Completed Documentation:**
- ✅ `technology-stack.md` - Full tech inventory
- ✅ `api-contracts-aocMCP.md` - MCP server API reference
- 🔄 `project-overview.md` (this file)

---

## 🧩 Integration Points

1. **VS Code ↔ aocMCP:** MCP client config enables puzzle retrieval via RPC
2. **Main Solutions ↔ Memory:** PowerShell scripts persist results to JSON
3. **Frameworks ↔ Solutions:** BMAD/OpenSpec/Spec-Kit guide development flow
4. **Agents ↔ Orchestrator:** Parallel execution with result aggregation

---

## 📊 Success Metrics

**Puzzle Completion:** 11 days solved (Days 1-11)  
**Agent Count:** 5 competing implementations  
**Total Scripts:** 45+ F# files  
**Frameworks Tested:** 3 (Manual, Spec-Kit, OpenSpec, BMAD in progress)  
**MCP Tools:** 2 (fetch_instruction, record_speed)  
**Memory Entries:** 7+ execution results stored

---

## 🏴‍☆ Known Issues & Technical Debt

1. **SubAgent Blocking:** Workflow agents can't launch other agents (BMAD limitation)
2. **Instruction Leakage:** Phase instructions sometimes visible in prompts
3. **Context Collapse:** Large workflows trigger token budget limits
4. **Memory Poisoning:** Cross-agent result contamination in shared memory
5. **Framework Overhead:** 300+ files for BMAD seem excessive for small projects

*(See `technology-stack.md` for detailed technical debt analysis)*

---

## 🗂️ Related Documentation

- **[technology-stack.md](./technology-stack.md)** - Complete technology inventory
- **[api-contracts-aocMCP.md](./api-contracts-aocMCP.md)** - MCP server API reference
- **[../README.md](../README.md)** - Daily journal with framework comparisons
- **[../aocMCP/MCP_SERVER.md](../aocMCP/MCP_SERVER.md)** - MCP server setup guide
- **[../openspec/AGENTS.md](../openspec/AGENTS.md)** - OpenSpec agent instructions

---

**Generated by:** Mary (BMAD Analyst Agent) in Pirate English mode  
**Workflow:** document-project (deep scan)  
**Timestamp:** 2025-12-12  
**Output Folder:** `aocBMADdocs/`

*Arrr! May yer code be bug-free and yer puzzles be solved swiftly!* 🏴‍☆
