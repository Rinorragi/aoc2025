open System.IO
open System.Collections.Generic

// Parse device connections
let parseDevice (line: string) =
    let parts = line.Split([| ':' |], 2)
    if parts.Length = 2 then
        let name = parts.[0].Trim()
        let connections = 
            parts.[1].Trim().Split(' ')
            |> Array.filter (fun s -> s.Length > 0)
            |> List.ofArray
        Some (name, connections)
    else None

// Build adjacency map from input
let buildMap (lines: string[]) =
    lines
    |> Array.choose parseDevice
    |> Map.ofArray

// Recursively count paths using DFS
let rec countPathsDFS map visited current destination =
    if current = destination then 1
    elif Set.contains current visited then 0
    else
        Map.tryFind current map
        |> Option.map (fun nexts ->
            nexts
            |> List.fold (fun acc next ->
                acc + countPathsDFS map (Set.add current visited) next destination
            ) 0
        )
        |> Option.defaultValue 0

let countPhase2WithMemo (map: Map<string, string list>) =
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
                    Map.tryFind current map
                    |> Option.map (fun nexts ->
                        nexts
                        |> List.sumBy (fun next -> dp next newSeenDac newSeenFft))
                    |> Option.defaultValue 0L
                
                memo.[key] <- result
                result
    
    dp "svr" false false

let input = File.ReadAllLines("input/input11.txt")
let deviceMap = buildMap input

let phase1 = countPathsDFS deviceMap Set.empty "you" "out"
printfn "Phase 1 - Paths from you to out: %d" phase1

let phase2 = countPhase2WithMemo deviceMap
printfn "Phase 2 - Paths from svr to out via dac and fft: %d" phase2
