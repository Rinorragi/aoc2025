# Design Document: solve-day11

## Overview
Day 11 puzzle requires finding all paths through a directed graph from a starting node ("you") to a destination node ("out"). This is a classic graph traversal problem best solved with depth-first search (DFS) or breadth-first enumeration.

## Problem Analysis

### Input Format
```
device_name: output1 output2 output3 ...
```
Each line defines a device and its output connections. Data flows only forward through outputs.

### Graph Characteristics
- **Directed graph**: Edges only flow from device to its outputs
- **Multiple paths possible**: Same nodes can appear in different paths
- **Cycles possible**: Graph may contain cycles requiring visited-node tracking
- **Dead ends**: Some devices may have no outputs or not lead to "out"

### Example
```
you: bbb ccc
bbb: ddd eee
ccc: ddd eee fff
ddd: ggg
eee: out
fff: out
ggg: out
```

Valid paths from "you" to "out":
1. you → bbb → ddd → ggg → out
2. you → bbb → eee → out
3. you → ccc → ddd → ggg → out
4. you → ccc → eee → out
5. you → ccc → fff → out

**Total: 5 paths**

## Technical Approach

### Data Structure
Use F# Map to represent adjacency list:
```fsharp
type Graph = Map<string, string list>
```

### Algorithm: Depth-First Search with Path Tracking

**Pseudocode:**
```
function findAllPaths(graph, current, target, visited):
    if current == target:
        return 1 (found one path)
    
    if current in visited:
        return 0 (cycle detected)
    
    if current not in graph or graph[current] is empty:
        return 0 (dead end)
    
    mark current as visited
    pathCount = 0
    
    for each neighbor in graph[current]:
        pathCount += findAllPaths(graph, neighbor, target, visited)
    
    unmark current as visited (backtrack)
    return pathCount
```

### F# Implementation Pattern
```fsharp
let rec countPaths (graph: Map<string, string list>) (visited: Set<string>) (current: string) (target: string) : int =
    if current = target then 1
    elif Set.contains current visited then 0
    else
        match Map.tryFind current graph with
        | None -> 0
        | Some neighbors ->
            let newVisited = Set.add current visited
            neighbors
            |> List.map (fun next -> countPaths graph newVisited next target)
            |> List.sum
```

### Parsing Strategy
```fsharp
let parseLine (line: string) : string * string list =
    let parts = line.Split(':')
    let device = parts.[0].Trim()
    let outputs = 
        parts.[1].Trim().Split(' ')
        |> Array.filter (fun s -> s <> "")
        |> Array.toList
    (device, outputs)

let parseGraph (lines: string[]) : Map<string, string list> =
    lines
    |> Array.filter (fun s -> s <> "")
    |> Array.map parseLine
    |> Map.ofArray
```

## Functional Programming Considerations

### Immutability
- Use immutable `Set<string>` for visited tracking
- Pass new visited set at each recursion level (no mutation)
- Graph represented as immutable `Map`

### Pipeline Operators
- Use `|>` for data transformation flows
- Use `||>` for folding operations if needed
- Chain operations: parse → build graph → traverse → count

### Pattern Matching
- Use `match` for handling `Map.tryFind` results
- Pattern match on base cases (target reached, dead end, cycle)

## Edge Cases

1. **Self-loops**: Device points to itself → visited set prevents infinite recursion
2. **No path exists**: "you" not connected to "out" → return 0
3. **Empty graph**: No devices defined → return 0
4. **Target same as start**: If "you" = "out" → return 1 immediately
5. **Multiple edges to same node**: Count as separate paths

## Testing Strategy

### Example Input Validation
First validate against provided example expecting 5 paths.

### Actual Input
Run against actual puzzle input once example passes.

### Agent Comparison
Compare results across all 5 agents to identify consensus answer or outliers.

## Performance Considerations

### Phase 1
- **Time Complexity**: O(paths) where paths = 431 for the example
- Works fine because the subgraph from "you" to "out" has manageable path count

### Phase 2 - Critical Performance Issue

**The Fundamental Problem**: The graph contains 10^12+ paths from svr→out

**All Path Enumeration Approaches FAIL**:
1. ✗ Naive: Check each path → Infinite time
2. ✗ Decomposition: Still enumerates exponential paths per segment
3. ✗ Inclusion-Exclusion: Each of the 4 counts still enumerates exponential paths
4. ✗ Any DFS that counts paths → Exponential

**Why Our Estimates Were Wrong**:
- Estimated: "10^6 paths, maybe 10 minutes"
- Reality: ">10^12 paths, never completes"
- With avg branching 2.82 over depth 50: 2.82^50 ≈ 10^22 theoretical max
- Visited-set pruning helps but doesn't prevent exponential blowup

### Correct Solution: Dynamic Programming with Memoization

**Key Insight**: Stop enumerating paths. Count states instead.

Many paths pass through the same (node, seenDac, seenFft) state. Why recalculate?

**Implementation**:

```fsharp
open System.Collections.Generic

let countPathsWithMemo (graph: Map<string, string list>) =
    // Cache: (node, seenDac, seenFft) → count of paths to 'out'
    let memo = Dictionary<string * bool * bool, int64>()
    
    let rec dp current seenDac seenFft =
        // Base case: reached destination
        if current = "out" then
            if seenDac && seenFft then 1L else 0L
        else
            // Update state
            let newSeenDac = seenDac || (current = "dac")
            let newSeenFft = seenFft || (current = "fft")
            let key = (current, newSeenDac, newSeenFft)
            
            // Check cache
            match memo.TryGetValue(key) with
            | true, cached -> cached
            | false, _ ->
                // Calculate (only once per state!)
                let result =
                    match Map.tryFind current graph with
                    | None -> 0L
                    | Some neighbors ->
                        neighbors
                        |> List.sumBy (fun next -> dp next newSeenDac newSeenFft)
                
                memo.[key] <- result
                result
    
    dp "svr" false false

```

**Critical Differences**:

| Approach | What it counts | Complexity | Time |
|----------|---------------|------------|------|
| DFS enumeration | Every path | O(paths) | ∞ (>10^12 paths) |
| DP memoization | Every unique state | O(V × states) | Milliseconds |

**Complexity Analysis**:
- States per node: 4 (dac×fft: 00, 01, 10, 11)
- Nodes: 622
- **Total unique states**: 622 × 4 = 2,488
- Each state computed once: O(V × 4) = O(V)
- **Expected time**: <1 second

**Why No Visited Set Needed**:
- Memoization inherently prevents infinite recursion
- Once we compute dp[node][state], we return cached value
- If there's a cycle, it gets the cached result (initially 0 if being computed)

**The Breakthrough**: From exponential path enumeration to linear state counting!



## Alternative Approaches

### BFS with Path Enumeration
Store full paths instead of counting. Less memory-efficient but easier to debug.

### Dynamic Programming
If same subproblems recur, memoization could help. However, visited set makes caching complex.

### Iterative DFS
Use explicit stack instead of recursion. More verbose in F# but avoids stack overflow.

**Recommendation**: Stick with recursive DFS for clarity and functional style.
