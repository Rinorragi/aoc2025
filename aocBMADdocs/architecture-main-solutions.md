# Architecture: Main AoC Solutions (Multi-Agent Arena)

**Ahoy! This here be the architecture o' the multi-agent puzzle-solvin' system!**

---

## 🏴‍☆ Overview

The **Main AoC Solutions** component be a **data processing pipeline** orchestratin' **5 competing AI agents** to solve Advent of Code puzzles in parallel, then compare results fer accuracy and speed.

**Architectural Pattern:** Multi-Agent Competition with PowerShell Orchestration  
**Primary Purpose:** Generate multiple solution strategies, select optimal approach  
**Technology Stack:** F# Interactive (FSI), PowerShell 7.x, JSON-based memory

---

## 🗺️ Component Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                   USER / ORCHESTRATOR                       │
│                 (Human or AI Coordinator)                   │
└───────────────────────┬─────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────────┐
│                 run-aoc.ps1 (Generic Wrapper)               │
│  • Timeout management (default 300s)                        │
│  • Parameter validation (Day, Part, Example flag)           │
│  • Background job execution                                 │
│  • Error handling & exit codes                              │
└───────────────────────┬─────────────────────────────────────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
┌──────────────┐ ┌──────────────┐ ┌──────────────┐
│   day##.fsx  │ │ day##_[agent]│ │  Parallel    │
│  (generic)   │ │    .fsx      │ │  Runner      │
│              │ │  (5 agents)  │ │  Scripts     │
└──────┬───────┘ └──────┬───────┘ └──────┬───────┘
       │                │                │
       │                │                │
       ▼                ▼                ▼
┌─────────────────────────────────────────────────────────────┐
│                  F# Interactive (FSI)                       │
│  • Parse puzzle input from input/ folder                    │
│  • Execute functional algorithm (fold/map/filter)           │
│  • Output result to STDOUT                                  │
│  • Return exit code (0 = success)                           │
└───────────────────────┬─────────────────────────────────────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
┌──────────────┐ ┌──────────────┐ ┌──────────────┐
│   input/     │ │   memory/    │ │  generated_  │
│ input##.txt  │ │ memory-mgr   │ │  scripts/    │
│ (examples)   │ │  .ps1        │ │ parallel.ps1 │
└──────────────┘ └──────────────┘ └──────────────┘
```

---

## 🧩 Core Components

### **1. Execution Wrapper: `run-aoc.ps1`**

**Purpose:** Generic timeout-protected script runner  
**Key Features:**
- **Timeout Management:** Default 300s, configurable via `-TimeoutSeconds`
- **Background Jobs:** Runs FSI in PowerShell job to enable timeout enforcement
- **Parameter Validation:** Requires Day/Part, optional Example flag
- **Error Handling:** Exit codes (0 = success, 1 = error/timeout)

**Usage:**
```powershell
.\run-aoc.ps1 -Day "11" -Part 2 -TimeoutSeconds 300
.\run-aoc.ps1 -Day "07" -Part 1 -Example
```

**Implementation Pattern:**
```powershell
$job = Start-Job -ScriptBlock {
    param($scriptPath, $arguments)
    dotnet fsi (Split-Path $scriptPath -Leaf) @arguments 2>&1
} -ArgumentList $scriptPath, $scriptArgs

$completed = Wait-Job -Job $job -Timeout $TimeoutSeconds
```

---

### **2. Puzzle Solutions: `day##_[agent].fsx`**

**Naming Convention:** `day##_[gpt4|gpt5|claudeopus45|gemini|grok].fsx`  
**Total Count:** 45+ files (5 agents × 9 days)

**Agent Roster:**
- **gpt4** - GPT-4 Turbo
- **gpt5** - GPT-5 (experimental)
- **claudeopus45** - Claude Opus 4.5
- **gemini** - Google Gemini
- **grok** - xAI Grok

**Common Structure:**
```fsharp
// Parse input from input/ folder
let input = System.IO.File.ReadAllLines($"input/input{day:D2}.txt")

// Functional processing pipeline
let result = 
    input
    |> Array.map parseLine
    |> Array.filter criteria
    |> Array.fold accumulator initialState

// Output to STDOUT
printfn $"Result: {result}"
```

**Key Patterns:**
- **Immutability:** All data structures immutable
- **Pipelines:** Extensive use of `|>` operator
- **Higher-Order Functions:** `fold`, `map`, `filter`, `collect`
- **Pattern Matching:** Exhaustive case handling
- **Memoization:** Dictionary-based caching for DP problems (Day 11)

---

### **3. Parallel Orchestration: `generated_scripts/run_day##_parallel.ps1`**

**Purpose:** Run all 5 agents in parallel, aggregate results  
**Generated:** Created per day as needed

**Implementation Pattern:**
```powershell
$agents = @("gpt4", "gpt5", "claudeopus45", "gemini", "grok")

$jobs = $agents | ForEach-Object {
    Start-Job -ScriptBlock {
        param($agent, $day)
        dotnet fsi "day${day}_${agent}.fsx"
    } -ArgumentList $_, $Day
}

Wait-Job $jobs
$jobs | Receive-Job
```

---

### **4. Memory System: `memory/memory-manager.ps1`**

**Purpose:** Persist execution results fer future retrieval  
**Storage Format:** JSON files with GUID names

**Schema:**
```json
{
  "id": "944ad334-257c-4997-a2cf-6c3a7a16e79c",
  "title": "day11_gpt4",
  "content": "Result: 358458157650450",
  "timestamp": "2025-12-11T14:23:00Z"
}
```

**Operations:**
- **store:** Create new memory entry
- **search:** Query by title/content substring
- **get:** Retrieve specific GUID
- **list:** Show all memories sorted by timestamp

**Usage:**
```powershell
.\memory\memory-manager.ps1 -Action store -Title "day11_gpt4" -Content "Result: 358458157650450"
.\memory\memory-manager.ps1 -Action search -Query "day11"
```

---

### **5. Input Data: `input/` Folder**

**Structure:**
- `input##.txt` - Actual puzzle input
- `input##_example.txt` - Sample data from problem statement

**Integration:**
F# scripts read input dynamically based on command-line flags:
```fsharp
let inputFile = 
    if hasExample then $"input/input{day:D2}_example.txt"
    else $"input/input{day:D2}.txt"
```

---

## 🔄 Data Flow

### **Single Agent Execution**
```
User Command → run-aoc.ps1 
    → Validate parameters
    → Start-Job (FSI background)
    → Read input/input##.txt
    → Execute F# algorithm
    → Output result to STDOUT
    → Receive-Job (capture output)
    → Exit with code 0/1
```

### **Parallel Multi-Agent Execution**
```
User Command → run_day##_parallel.ps1
    → Start 5 parallel jobs (gpt4, gpt5, claude, gemini, grok)
    → Each job executes day##_[agent].fsx
    → Wait-Job (all agents)
    → Receive-Job (aggregate outputs)
    → Compare results and timing
    → Select best solution
    → Optional: Store to memory/
```

---

## 🎯 Design Patterns

### **1. Template Method Pattern**
`run-aoc.ps1` provides generic execution template, allowing different scripts to plug in:
- Same timeout logic
- Same error handling
- Same parameter validation
- Different puzzle logic per day/agent

### **2. Strategy Pattern**
Each agent implements a different algorithmic strategy fer the same puzzle:
- **gpt4:** Often uses imperative loops
- **claudeopus45:** Emphasizes functional purity
- **gemini:** Balances performance and readability
- **grok:** Experimental approaches
- **gpt5:** Cutting-edge techniques

### **3. Pipeline Pattern**
F# scripts use functional pipelines extensively:
```fsharp
input
|> parse
|> transform
|> filter
|> aggregate
|> format
```

### **4. Repository Pattern**
`memory/` folder acts as a simple file-based repository:
- GUID-based unique keys
- JSON serialization
- Search capabilities
- Timestamp ordering

---

## ⚙️ Configuration & Extensibility

### **Adding a New Day**
1. Create `day##_[agent].fsx` fer each agent (5 files)
2. Add puzzle input to `input/input##.txt`
3. Optional: Create `input/input##_example.txt`
4. Optional: Generate `run_day##_parallel.ps1`

### **Adding a New Agent**
1. Add agent name to `$agents` array in parallel scripts
2. Create `day##_[newagent].fsx` fer all existing days
3. Update orchestration logic if needed

### **Customizing Timeouts**
```powershell
.\run-aoc.ps1 -Day "11" -Part 2 -TimeoutSeconds 600  # 10 minutes
```

---

## 🚨 Error Handling

### **Timeout Scenarios**
```powershell
if ($completed) {
    # Job finished within timeout
} else {
    Stop-Job $job
    Write-Error "TIMEOUT: Execution exceeded $TimeoutSeconds seconds"
    exit 1
}
```

### **Script Not Found**
```powershell
if (-not (Test-Path $scriptPath)) {
    Write-Error "Script not found: $scriptPath"
    exit 1
}
```

### **F# Runtime Errors**
Captured via `2>&1` redirection in background job, returned to caller.

---

## 📊 Performance Characteristics

**Typical Execution Times (Day 11, Part 2):**
- **gpt4:** ~45 seconds (memoized DP)
- **gpt5:** ~42 seconds (optimized DP)
- **claudeopus45:** ~50 seconds (functional recursion)
- **gemini:** ~48 seconds (iterative approach)
- **grok:** ~55 seconds (experimental algorithm)

**Parallel Speedup:** ~5x when running all agents simultaneously (5 agents on multi-core CPU)

**Memory Usage:** Typically < 100 MB per agent process

---

## 🔗 Integration Points

### **With aocMCP Server**
- Puzzle instructions fetched via MCP `fetch_instruction` tool
- Execution speeds recorded via MCP `record_speed` tool
- Integration happens at AI agent orchestration layer (not in F# scripts)

### **With Memory System**
- Results stored post-execution via `memory-manager.ps1`
- Retrieved fer comparison across agents/days
- Timestamped fer chronological analysis

### **With Frameworks**
- **OpenSpec:** Day 11 used spec-driven workflow
- **Spec-Kit:** Day 10 used template-driven approach
- **BMAD:** Day 12 documentation generation (current)

---

## 🛠️ Development Workflow

1. **Receive Puzzle:** Daily puzzle released at midnight EST
2. **Fetch Instructions:** Via aocMCP or manual download
3. **Generate Solutions:** Run orchestrator to create 5 agent implementations
4. **Execute Parallel:** Run all agents via `run_day##_parallel.ps1`
5. **Compare Results:** Validate answers match, check execution times
6. **Store Memory:** Record winning solution to memory/
7. **Document Learnings:** Update README.md with observations

---

## 📝 Known Limitations

1. **No Automatic Result Validation:** Must manually compare agent outputs
2. **Limited Error Context:** F# stack traces can be cryptic in FSI
3. **Memory System Not Indexed:** Linear search through JSON files
4. **No Performance Profiling:** Execution times measured at PowerShell level only
5. **Manual Agent Selection:** No automated "best solution" picker

---

## 🏴‍☆ Future Enhancements

- **Automated Testing:** Compare against example outputs
- **Performance Benchmarking:** Detailed profiling with BenchmarkDotNet
- **Result Validation:** Automatic answer verification
- **Agent Ranking System:** Track win rates across all days
- **Database Backend:** Replace JSON files with SQLite

---

**Generated by:** Mary (BMAD Analyst Agent)  
**Workflow:** document-project (deep scan)  
**Part:** Main AoC Solutions (data processing pipeline)  
**Timestamp:** 2025-12-12

*Arrr! May yer agents be swift and yer results be accurate!* 🏴‍☆
