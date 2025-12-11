# Change Proposal: solve-day11

**Status:** Proposed  
**Created:** 2025-12-11  
**Type:** Feature Addition

## Summary
Implement solution for Advent of Code 2025 Day 11 puzzle using multi-agent orchestration system. 

**Phase 1**: Find all paths from a "you" device to an "out" device through a directed graph of electrical conduits.

**Phase 2**: Find all paths from "svr" (server rack) to "out" that visit both "dac" (digital-to-analog converter) and "fft" (fast Fourier transform) devices.

## Motivation
Continue the AoC 2025 challenge progression by solving Day 11. This puzzle introduces graph traversal and path-finding algorithms, providing an opportunity to compare different AI agents' approaches to implementing graph algorithms in functional F#.

## Proposed Changes

### ADDED Requirements

#### Requirement: Parse device connection graph
Parse puzzle input into a graph data structure representing devices and their output connections.

##### Scenario: Parse example input
**Given** the example input:
```
aaa: you hhh
you: bbb ccc
bbb: ddd eee
```
**When** parsing the input
**Then** create a map where:
- "you" maps to ["bbb", "ccc"]
- "bbb" maps to ["ddd", "eee"]
- "aaa" maps to ["you", "hhh"]

#### Requirement: Find all paths from "you" to "out" (Phase 1)
Implement graph traversal algorithm to enumerate all distinct paths from starting device "you" to destination device "out".

##### Scenario: Count paths in example
**Given** the example graph with 5 possible paths
**When** finding all paths from "you" to "out"
**Then** return count of 5 distinct paths

##### Scenario: Handle cycles and dead ends
**Given** a graph that may contain cycles or devices with no outputs
**When** traversing the graph
**Then** avoid infinite loops and only count valid paths reaching "out"

#### Requirement: Find paths visiting required intermediate nodes (Phase 2)
Implement path enumeration that filters paths to include only those visiting all required intermediate nodes (dac and fft).

##### Scenario: Count paths with required visits in example
**Given** example graph with 8 total paths from "svr" to "out", where only 2 visit both "dac" and "fft"
**When** finding all paths that visit both required nodes
**Then** return count of 2 paths

##### Scenario: Track visited nodes in path
**Given** a path being constructed during traversal
**When** building the path
**Then** maintain list of visited nodes to check for required intermediate stops

##### Scenario: Validate required nodes visited
**Given** a complete path from start to destination
**When** checking if path is valid for Phase 2
**Then** confirm path contains both "dac" and "fft" nodes

#### Requirement: Support multiple agent implementations
Generate solutions using all available AI agents (GPT-4, GPT-5, Claude Opus 4.5, Gemini, Grok) for comparison.

##### Scenario: Generate agent-specific solutions
**Given** the Day 11 puzzle specification
**When** orchestrator invokes implementor agents
**Then** create files: day11_gpt4.fsx, day11_gpt5.fsx, day11_claudeopus45.fsx, day11_gemini.fsx, day11_grok.fsx

#### Requirement: Verify solutions against example and actual input
Test all agent solutions against example input first, then validate with actual puzzle input.

##### Scenario: Validate with example input
**Given** input11_example.txt containing example data
**When** running each agent's solution
**Then** all solutions should output 5 paths

##### Scenario: Validate with puzzle input
**Given** input11.txt containing actual puzzle data
**When** running verified solutions
**Then** determine correct answer for AoC submission

#### Requirement: Record performance metrics
Store execution results and solving times in memory system for analysis.

##### Scenario: Store agent results
**Given** completed execution of an agent's solution
**When** solution finishes successfully
**Then** store result in memory/memory-manager.ps1 with title "day11_[agent]"

## Implementation Plan
See `tasks.md` for detailed implementation checklist.

## Success Criteria

### Phase 1
- [x] All 5 agent solutions parse Day 11 input correctly
- [x] Solutions correctly count all paths for example input (expected: 5)
- [x] Solutions produce correct answer for actual puzzle input (431 paths)
- [x] Execution results stored in memory system
- [x] Performance metrics recorded via MCP server

### Phase 2
- [ ] All 5 agent solutions modified to track visited nodes in paths
- [ ] Solutions correctly filter paths visiting both "dac" and "fft"
- [ ] Solutions correctly count filtered paths for example input (expected: 2)
- [ ] Solutions produce correct answer for Phase 2 actual puzzle input
- [ ] Phase 2 execution results stored in memory system

## Risks and Mitigations
- **Risk**: Graph cycles causing infinite loops
  - **Mitigation**: Implement path tracking to detect revisited nodes
- **Risk**: Different agents may interpret problem differently
  - **Mitigation**: Provide clear examples and validation against known output
- **Risk**: Phase 2 exponential blowup with dense graphs (622 nodes, high branching factor)
  - **Initial approach failed**: Naive enumeration takes hours/days
  - **Mitigation**: Use inclusion-exclusion principle to count paths efficiently

## Phase 2 Performance Analysis

### Problem Evolution
1. **Naive enumeration**: Check each path for dac+fft → Exponential, hours/days
2. **Inclusion-exclusion**: 4 separate counts with pruning → Still exponential per count
3. **Root cause**: Graph has 10^12+ paths from svr→out - enumeration is fundamentally infeasible

### Actual Graph Characteristics
- 622 nodes, 1,756 edges, average branching 2.82
- **Critical insight**: Even counting ALL paths svr→out doesn't complete in reasonable time
- This means: >10^12 paths exist, possibly 10^15+
- **Any algorithm that enumerates paths will fail**

### Correct Solution: Dynamic Programming with State Memoization

Stop enumerating paths. Instead, count states:

**Key Insight**: Many different paths pass through the same (node, seenDac, seenFft) state. Calculate each state exactly once.

**Algorithm:**
```fsharp
// Memoization dictionary: (node × seenDac × seenFft) → count
let memo = Dictionary<string * bool * bool, int64>()

let rec dp current seenDac seenFft =
    if current = "out" then
        if seenDac && seenFft then 1L else 0L
    else
        let newSeenDac = seenDac || (current = "dac")
        let newSeenFft = seenFft || (current = "fft")
        let key = (current, newSeenDac, newSeenFft)
        
        match memo.TryGetValue(key) with
        | true, cached -> cached
        | false, _ ->
            let result =
                neighbors(current)
                |> List.sumBy (fun next -> dp next newSeenDac newSeenFft)
            memo.[key] <- result
            result
```

**Complexity**: O(V × 4) where V is nodes, 4 is state combinations (dac/fft seen or not)
- **Actual operations**: 622 × 4 = 2,488 unique states maximum
- **Expected time**: Milliseconds to seconds (vs infinite for enumeration)

**Why This Works**:
- Without memoization: Calculate same (node, state) exponentially many times
- With memoization: Calculate each (node, state) exactly once
- Reduces from O(paths) to O(states) = O(V)

## Alternatives Considered
- **Single-agent approach**: Rejected to maintain consistency with Days 2-10 multi-agent pattern
- **Parallel execution**: Not feasible due to VS Code API limitations (subagents run sequentially)

## Related Changes
None - this is an isolated daily puzzle solution.
