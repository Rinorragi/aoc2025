open System.IO
open System.Collections.Generic

// Parse input into graph structure
let parse (lines: string[]) =
    lines
    |> Array.filter (fun l -> l.Trim() <> "")
    |> Array.map (fun line ->
        let parts = line.Split(':')
        let node = parts.[0].Trim()
        let edges = 
            parts.[1].Trim().Split(' ')
            |> Array.filter (fun s -> s <> "")
            |> List.ofArray
        (node, edges))
    |> Map.ofArray

// DFS to count all paths from start to end
let rec dfs (graph: Map<string, string list>) (visited: Set<string>) (current: string) (target: string) =
    if current = target then 
        1
    elif visited |> Set.contains current then 
        0
    else
        match graph |> Map.tryFind current with
        | None -> 0
        | Some neighbors ->
            let newVisited = visited |> Set.add current
            neighbors
            |> List.map (fun neighbor -> dfs graph newVisited neighbor target)
            |> List.sum

let countPhase2WithMemo (graph: Map<string, string list>) =
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
                    match graph |> Map.tryFind current with
                    | None -> 0L
                    | Some neighbors ->
                        neighbors
                        |> List.sumBy (fun neighbor -> dp neighbor newSeenDac newSeenFft)
                
                memo.[key] <- result
                result
    
    dp "svr" false false

let input = File.ReadAllLines("input/input11.txt")
let graph = parse input

let phase1 = dfs graph Set.empty "you" "out"
printfn "Phase 1 - Number of different paths: %d" phase1

let phase2 = countPhase2WithMemo graph
printfn "Phase 2 - Paths visiting dac and fft: %d" phase2
