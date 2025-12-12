# AoC 2025 Documentation Index

**Ahoy! Ye primary navigational chart fer this here repository!**

---

## 🏴‍☆ Quick Start

**New to this project?** Start here:
1. **[Project Overview](./project-overview.md)** - Understand the monorepo structure and goals
2. **[Technology Stack](./technology-stack.md)** - Learn the technologies in use
3. **[Architecture: Main Solutions](./architecture-main-solutions.md)** - See how multi-agent competition works
4. **[API Contracts: aocMCP](./api-contracts-aocMCP.md)** - Understand the MCP server integration

**Want to run a puzzle?**
```powershell
.\run-aoc.ps1 -Day "11" -Part 2
```

**Want to run all agents in parallel?**
```powershell
.\generated_scripts\run_day11_parallel.ps1
```

---

## 📚 Documentation Inventory

### **Core Documentation (This Folder: `aocBMADdocs/`)**

| Document | Purpose | Audience | Status |
|----------|---------|----------|--------|
| **[index.md](./index.md)** | Master navigation (you are here) | All | ✅ Complete |
| **[project-overview.md](./project-overview.md)** | High-level repository structure | All | ✅ Complete |
| **[technology-stack.md](./technology-stack.md)** | Technology inventory (3 parts) | Developers, AI Agents | ✅ Complete |
| **[api-contracts-aocMCP.md](./api-contracts-aocMCP.md)** | MCP server API reference | Integration Developers | ✅ Complete |
| **[architecture-main-solutions.md](./architecture-main-solutions.md)** | Multi-agent pipeline architecture | Developers, AI Agents | ✅ Complete |
| **[project-scan-report.json](./project-scan-report.json)** | Workflow state tracking | BMAD Agents Only | ✅ Active |

### **Existing Documentation (Root & Subfolders)**

| Document | Purpose | Location | Status |
|----------|---------|----------|--------|
| **[README.md](../README.md)** | Daily journal & framework comparisons | Root | ✅ Up-to-date (Day 12) |
| **[AGENTS.md](../AGENTS.md)** | OpenSpec agent instructions | Root | ✅ Complete |
| **[aocMCP/MCP_SERVER.md](../aocMCP/MCP_SERVER.md)** | MCP server setup guide | aocMCP/ | ✅ Complete |
| **[aocMCP/README.md](../aocMCP/README.md)** | MCP component overview | aocMCP/ | ✅ Complete |
| **[aocSPEC/README.md](../aocSPEC/README.md)** | Spec-Kit experimentation notes | aocSPEC/ | ✅ Complete |
| **[openspec/AGENTS.md](../openspec/AGENTS.md)** | OpenSpec workflow instructions | openspec/ | ✅ Complete |
| **[openspec/project.md](../openspec/project.md)** | OpenSpec project metadata | openspec/ | ✅ Complete |
| **[docs/day09_plan.md](../docs/day09_plan.md)** | Day 9 planning document | docs/ | ✅ Archived |

---

## 🗺️ Documentation by Audience

### **For AI Agents (PRD Creation, Feature Development)**
Start here to understand the codebase before generating proposals:
1. **[project-overview.md](./project-overview.md)** - Understand the 3-part monorepo
2. **[technology-stack.md](./technology-stack.md)** - Know the tech stack constraints
3. **[architecture-main-solutions.md](./architecture-main-solutions.md)** - Grasp the multi-agent pattern
4. **[api-contracts-aocMCP.md](./api-contracts-aocMCP.md)** - Learn integration points

### **For Human Developers**
Get productive quickly:
1. **[README.md](../README.md)** - Daily journal shows recent context
2. **[project-overview.md](./project-overview.md)** - Repository structure
3. **[architecture-main-solutions.md](./architecture-main-solutions.md)** - How to run solutions
4. **[aocMCP/MCP_SERVER.md](../aocMCP/MCP_SERVER.md)** - Setup MCP server

### **For Integration Work**
Building on top o' this project:
1. **[api-contracts-aocMCP.md](./api-contracts-aocMCP.md)** - MCP RPC API reference
2. **[technology-stack.md](./technology-stack.md)** - Dependency constraints
3. **[architecture-main-solutions.md](./architecture-main-solutions.md)** - Execution patterns

---

## 🧩 Documentation by Project Part

### **Part 1: aocMCP (Backend Service)**
- **[api-contracts-aocMCP.md](./api-contracts-aocMCP.md)** - RPC methods, tools, data models
- **[../aocMCP/MCP_SERVER.md](../aocMCP/MCP_SERVER.md)** - Setup & configuration
- **[../aocMCP/README.md](../aocMCP/README.md)** - Component overview
- **[technology-stack.md](./technology-stack.md)** (Part 1 section) - Tech details

### **Part 2: Main AoC Solutions (Data Pipeline)**
- **[architecture-main-solutions.md](./architecture-main-solutions.md)** - Multi-agent orchestration
- **[project-overview.md](./project-overview.md)** (Part 2 section) - High-level description
- **[technology-stack.md](./technology-stack.md)** (Part 2 section) - Agent ecosystem
- **[../README.md](../README.md)** - Daily journal with results

### **Part 3: aocSPEC (Spec-Kit Experiment)**
- **[../aocSPEC/README.md](../aocSPEC/README.md)** - Spec-Kit setup
- **[technology-stack.md](./technology-stack.md)** (Part 3 section) - Framework integration
- **[project-overview.md](./project-overview.md)** (Part 3 section) - Purpose & structure

---

## 🔧 Documentation by Task

### **Running Puzzles**
- **[architecture-main-solutions.md](./architecture-main-solutions.md)** → "Getting Started" section
- **[../README.md](../README.md)** → "Running Solutions" (if exists)

### **Understanding MCP Integration**
- **[api-contracts-aocMCP.md](./api-contracts-aocMCP.md)** → Complete API reference
- **[../aocMCP/MCP_SERVER.md](../aocMCP/MCP_SERVER.md)** → VS Code setup

### **Adding New Agents**
- **[architecture-main-solutions.md](./architecture-main-solutions.md)** → "Configuration & Extensibility"
- **[technology-stack.md](./technology-stack.md)** → "Agent Ecosystem" table

### **Framework Comparison**
- **[../README.md](../README.md)** → Daily journal (Days 10-12)
- **[project-overview.md](./project-overview.md)** → "Framework Journey" table
- **[technology-stack.md](./technology-stack.md)** → "Cross-Cutting Technologies"

### **Troubleshooting**
- **[technology-stack.md](./technology-stack.md)** → "Known Issues & Technical Debt"
- **[architecture-main-solutions.md](./architecture-main-solutions.md)** → "Error Handling"

---

## 📊 Documentation Completeness

| Area | Coverage | Status |
|------|----------|--------|
| **Project Overview** | ✅ Complete | High-level architecture documented |
| **Technology Stack** | ✅ Complete | All 3 parts inventoried |
| **API Contracts** | ✅ Complete | 6 RPC methods + 2 tools |
| **Architecture (Main)** | ✅ Complete | Multi-agent pipeline detailed |
| **Architecture (aocMCP)** | ⚠️ Partial | Covered in API contracts |
| **Architecture (aocSPEC)** | ⏳ Pending | Not yet documented |
| **Development Guide** | ⏳ Pending | Quick start in overview only |
| **Deployment Guide** | ⏳ Pending | Not applicable (dev project) |
| **Component Inventory** | ✅ Complete | In project-overview.md |
| **Source Tree Analysis** | ⏳ Pending | High-level in overview |

---

## 🏴‍☆ Framework Documentation

### **BMAD Framework**
- **Location:** `.bmad/` (~300 files)
- **Agent Instructions:** `.bmad/bmm/agents/analyst.md` (not included in output)
- **Workflows:** `.bmad/bmm/workflows/document-project/`
- **Current Workflow:** `full-scan-instructions.md` (deep scan mode)

### **OpenSpec Framework**
- **[../openspec/AGENTS.md](../openspec/AGENTS.md)** - Agent workflow instructions
- **[../openspec/project.md](../openspec/project.md)** - Project metadata
- **Specs:** `../openspec/specs/` (active)
- **Changes:** `../openspec/changes/` (archived proposals)

### **Spec-Kit Framework**
- **[../aocSPEC/README.md](../aocSPEC/README.md)** - Framework overview
- **Config:** `../aocSPEC/.specify/`
- **Templates:** `../aocSPEC/templates/`
- **Specs:** `../aocSPEC/specs/`

---

## 🚀 Quick Reference Commands

### **Execution**
```powershell
# Run single agent solution
.\run-aoc.ps1 -Day "11" -Part 2 -TimeoutSeconds 300

# Run with example input
.\run-aoc.ps1 -Day "07" -Part 1 -Example

# Run all agents in parallel
.\generated_scripts\run_day11_parallel.ps1
```

### **Memory Management**
```powershell
# Store execution result
.\memory\memory-manager.ps1 -Action store -Title "day11_gpt4" -Content "Result: 358458157650450"

# Search memories
.\memory\memory-manager.ps1 -Action search -Query "day11"

# List all memories
.\memory\memory-manager.ps1 -Action list
```

### **MCP Server**
```powershell
# Start MCP server
cd aocMCP
dotnet run

# Test instruction fetch (via VS Code MCP client)
# See api-contracts-aocMCP.md for RPC examples
```

### **Development**
```powershell
# Check .NET version
dotnet --version  # Expected: 10.0.101

# Format F# code
dotnet fantomas day11_gpt4.fsx

# Run F# script directly
dotnet fsi day11_gpt4.fsx -- --day 11 --part 2
```

---

## 🗂️ File Organization

```
aoc2025/
├── aocBMADdocs/                    # ← YOU ARE HERE (generated docs)
│   ├── index.md                    # Master index (this file)
│   ├── project-overview.md         # High-level architecture
│   ├── technology-stack.md         # Tech inventory (3 parts)
│   ├── api-contracts-aocMCP.md     # MCP API reference
│   ├── architecture-main-solutions.md  # Multi-agent pipeline
│   └── project-scan-report.json    # Workflow state
│
├── aocMCP/                         # Part 1: MCP Server
│   ├── Program.fs                  # Entry point
│   ├── MCP_SERVER.md               # Setup guide
│   └── README.md                   # Component overview
│
├── aocSPEC/                        # Part 3: Spec-Kit
│   └── README.md                   # Spec-Kit notes
│
├── openspec/                       # OpenSpec Framework
│   ├── AGENTS.md                   # Workflow instructions
│   └── project.md                  # Project metadata
│
├── memory/                         # Execution memory (JSON)
│   └── memory-manager.ps1          # CRUD operations
│
├── input/                          # Puzzle inputs
│
├── generated_scripts/              # Parallel runners
│
├── day##_[agent].fsx               # 45+ puzzle solutions
├── run-aoc.ps1                     # Generic wrapper
│
├── README.md                       # Daily journal
└── AGENTS.md                       # OpenSpec instructions
```

---

## 📝 Documentation Conventions

### **Pirate English**
All BMAD-generated docs use "Pirate English" fer a touch o' whimsy:
- "Ahoy!" instead of "Hello"
- "Yer" instead of "Your"
- "Fer" instead of "For"
- "Be" instead of "Is/Are"

(This be configurable in `.bmad/config/config.yaml`)

### **File Naming**
- `project-overview.md` - High-level architecture
- `technology-stack.md` - Tech inventory
- `api-contracts-[component].md` - API references
- `architecture-[component].md` - Detailed architecture
- `index.md` - Master navigation

### **Status Indicators**
- ✅ Complete - Documentation finished
- ⚠️ Partial - Some coverage, needs expansion
- ⏳ Pending - Not yet created
- 🔄 In Progress - Currently being written

---

## 🔗 External Resources

- **Advent of Code 2025:** https://adventofcode.com/2025
- **F# Documentation:** https://docs.microsoft.com/en-us/dotnet/fsharp/
- **Model Context Protocol:** https://modelcontextprotocol.io/
- **BMAD Framework:** (Internal, `.bmad/` folder)
- **OpenSpec:** (Internal, `openspec/` folder)
- **Spec-Kit:** (Internal, `aocSPEC/.specify/` folder)

---

## 🎯 Next Steps

### **For New Contributors**
1. Read **[project-overview.md](./project-overview.md)**
2. Review **[technology-stack.md](./technology-stack.md)**
3. Check **[../README.md](../README.md)** fer recent context
4. Run a test puzzle: `.\run-aoc.ps1 -Day "01" -Part 1 -Example`

### **For AI Agents**
1. Load **[project-overview.md](./project-overview.md)**
2. Load **[technology-stack.md](./technology-stack.md)**
3. Load **[architecture-main-solutions.md](./architecture-main-solutions.md)**
4. Review user request and create targeted PRD

### **For Framework Experimentation**
1. Review **[../README.md](../README.md)** Days 10-12
2. Check **[technology-stack.md](./technology-stack.md)** "Known Issues"
3. Consider adding to `docs/` folder

---

## 📊 Project Statistics (as of Day 12)

- **Days Completed:** 11
- **Total Solutions:** 45+ F# scripts
- **Agents:** 5 (gpt4, gpt5, claudeopus45, gemini, grok)
- **Frameworks Tested:** 3 (Spec-Kit, OpenSpec, BMAD)
- **MCP Tools:** 2 (fetch_instruction, record_speed)
- **Memory Entries:** 7+
- **Documentation Files:** 15+ (including this index)
- **Lines of F# Code:** ~2,000+ (estimated)
- **PowerShell Scripts:** 10+

---

## 🏴‍☆ About This Documentation

**Generated by:** Mary (BMAD Analyst Agent) in Pirate English mode  
**Workflow:** document-project (deep scan, 12 steps)  
**Framework:** BMAD v6.0.0-alpha.16  
**Date:** 2025-12-12  
**Output Folder:** `aocBMADdocs/`  
**Scan Level:** Deep (critical files read, not exhaustive)

**Purpose:** Provide comprehensive AI-retrievable documentation fer brownfield project analysis, PRD creation, and feature development using BMAD's enterprise AI development method.

---

*Arrr! May this index guide ye safely through the codebase seas!* 🏴‍☆
