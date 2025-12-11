open System.IO
open System.Collections.Generic

// Parse a line into device name and its outputs
let parseLine (line: string) =
    let parts = line.Split(':')
    let device = parts.[0].Trim()
    let outputs = 
        parts.[1].Trim().Split(' ')
        |> Array.filter (fun s -> s <> "")
        |> Array.toList
    (device, outputs)

// Parse all lines into a graph (adjacency list)
let parseGraph (lines: string[]) =
    lines
    |> Array.filter (fun s -> s.Trim() <> "")
    |> Array.map parseLine
    |> Map.ofArray

// Count all paths from current node to target using DFS with visited tracking
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

// Phase 2: DP with memoization - count states not paths!
let countPathsWithMemo (graph: Map<string, string list>) =
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
                    match Map.tryFind current graph with
                    | None -> 0L
                    | Some neighbors ->
                        neighbors
                        |> List.sumBy (fun next -> dp next newSeenDac newSeenFft)
                
                memo.[key] <- result
                result
    
    dp "svr" false false

// Main execution
let input = File.ReadAllLines("input/input11.txt")
let graph = parseGraph input

// Phase 1: All paths from 'you' to 'out'
let phase1Count = countPaths graph Set.empty "you" "out"
printfn "Phase 1 - Number of paths from 'you' to 'out': %d" phase1Count

// Phase 2: DP with memoization
let phase2Count = countPathsWithMemo graph
printfn "Phase 2 - Paths visiting 'dac' and 'fft': %d" phase2Count
