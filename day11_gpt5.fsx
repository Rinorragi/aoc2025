open System.IO
open System.Collections.Generic

let parseLine (line: string) =
    match line.Split(':') with
    | [| device; outputs |] ->
        let device = device.Trim()
        let outputs = outputs.Trim().Split(' ') |> Array.filter ((<>) "") |> Array.toList
        (device, outputs)
    | _ -> ("", [])

let buildGraph (lines: string[]) =
    lines
    |> Array.filter (fun (s: string) -> s.Trim() <> "")
    |> Array.map parseLine
    |> Array.filter (fun (d, _) -> d <> "")
    |> Map.ofArray

let rec findAllPaths graph visited current target =
    if current = target then 1
    elif Set.contains current visited then 0
    else
        graph
        |> Map.tryFind current
        |> Option.map (fun neighbors ->
            let newVisited = Set.add current visited
            neighbors
            |> List.sumBy (fun next -> findAllPaths graph newVisited next target))
        |> Option.defaultValue 0

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
                    graph
                    |> Map.tryFind current
                    |> Option.map (fun neighbors ->
                        neighbors
                        |> List.sumBy (fun next -> dp next newSeenDac newSeenFft))
                    |> Option.defaultValue 0L
                
                memo.[key] <- result
                result
    
    dp "svr" false false

let lines = File.ReadAllLines("input/input11.txt")
let graph = buildGraph lines

let phase1 = findAllPaths graph Set.empty "you" "out"
printfn "Phase 1 - Total paths from 'you' to 'out': %d" phase1

let phase2 = countPhase2WithMemo graph
printfn "Phase 2 - Paths from 'svr' to 'out' visiting 'dac' and 'fft': %d" phase2
