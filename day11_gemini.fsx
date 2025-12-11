open System.IO
open System.Collections.Generic

type Graph = Map<string, string list>

let parseLine (line: string) : (string * string list) option =
    let parts = line.Split(':')
    if parts.Length = 2 then
        let device = parts.[0].Trim()
        let outputs = 
            parts.[1].Trim().Split(' ')
            |> Array.filter (fun s -> s <> "")
            |> Array.toList
        Some (device, outputs)
    else
        None

let parseInput (lines: string[]) : Graph =
    lines
    |> Array.filter (fun line -> line.Trim() <> "")
    |> Array.choose parseLine
    |> Map.ofArray

let rec countAllPaths (graph: Graph) (visited: Set<string>) (from: string) (target: string) : int =
    if from = target then
        1
    elif visited.Contains(from) then
        0
    else
        match graph.TryFind(from) with
        | Some destinations ->
            let updatedVisited = visited.Add(from)
            destinations
            |> List.map (fun dest -> countAllPaths graph updatedVisited dest target)
            |> List.sum
        | None ->
            0

let countPhase2WithMemo (graph: Graph) =
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
                    match graph.TryFind(current) with
                    | Some destinations ->
                        destinations
                        |> List.sumBy (fun dest -> dp dest newSeenDac newSeenFft)
                    | None -> 0L
                
                memo.[key] <- result
                result
    
    dp "svr" false false

let lines = File.ReadAllLines("input/input11.txt")
let graph = parseInput lines

let phase1 = countAllPaths graph Set.empty "you" "out"
printfn "Phase 1 - Total number of paths: %d" phase1

let phase2 = countPhase2WithMemo graph
printfn "Phase 2 - Paths visiting dac and fft: %d" phase2
