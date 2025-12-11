# Capability: aoc-puzzle-solver

## ADDED Requirements

### Requirement: Day 11 graph path enumeration
The system SHALL implement solution for Day 11 puzzle that counts all paths through a directed graph from "you" to "out" (Phase 1) and from "svr" to "out" visiting both "dac" and "fft" (Phase 2).

#### Scenario: Parse device connection graph
**Given** puzzle input with device connections in format `device: output1 output2`
**When** parsing the input file
**Then** create Map representing directed graph adjacency list

#### Scenario: Count all paths in example graph (Phase 1)
**Given** example input with known 5 paths from "you" to "out"
**When** executing path-finding algorithm
**Then** return count of 5 distinct paths

#### Scenario: Handle graph cycles safely
**Given** graph that may contain cycles
**When** traversing graph with DFS
**Then** use visited set to prevent infinite loops and only count valid paths

#### Scenario: Detect dead ends
**Given** graph with devices that have no outputs or don't lead to target
**When** traversing these branches
**Then** return 0 paths for dead end branches

#### Scenario: Count paths with required intermediate nodes (Phase 2)
**Given** Phase 2 example with 8 total paths from "svr" to "out"
**When** filtering paths that visit both "dac" and "fft"
**Then** return count of 2 paths meeting criteria

#### Scenario: Use inclusion-exclusion for efficient counting
**Given** a dense graph where naive enumeration is infeasible
**When** counting paths visiting both required nodes
**Then** calculate: (all paths) - (avoiding dac) - (avoiding fft) + (avoiding both)

#### Scenario: Prune branches avoiding specific nodes
**Given** DFS traversal to count paths
**When** node to avoid is encountered
**Then** return 0 for that branch (aggressive pruning)

### Requirement: Multi-agent solution generation
The system SHALL generate Day 11 solutions using all five AI agent implementors for comparison.

#### Scenario: Create agent-specific solution files
**Given** Day 11 puzzle specification
**When** orchestrator delegates to implementor agents
**Then** produce files day11_gpt4.fsx, day11_gpt5.fsx, day11_claudeopus45.fsx, day11_gemini.fsx, day11_grok.fsx

#### Scenario: Validate all agent solutions
**Given** multiple agent implementations
**When** testing against example input (expected: 5)
**Then** all agents should produce correct result before processing actual input

### Requirement: Solution validation workflow
The system SHALL test solutions against example data before running on actual puzzle input.

#### Scenario: Example input validation
**Given** input11_example.txt containing example graph
**When** running `dotnet fsi day11_[agent].fsx` with example input
**Then** each solution outputs 5 paths

#### Scenario: Actual input execution
**Given** validated solutions that pass example test
**When** running against input11.txt
**Then** determine answer for AoC submission

### Requirement: Result persistence
The system SHALL store execution results and performance metrics in memory system.

#### Scenario: Store agent execution results
**Given** completed solution execution
**When** agent finishes processing
**Then** invoke memory-manager.ps1 to store result with title "day11_[agent]"

#### Scenario: Record solving speed
**Given** solution completes successfully
**When** performance metrics available
**Then** record speed via MCP server for analysis
