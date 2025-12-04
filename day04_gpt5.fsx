open System.IO

let parseGrid (lines: string[]) =
    lines
    |> Array.mapi (fun y line -> 
        line.ToCharArray() 
        |> Array.mapi (fun x c -> ((y, x), c)))
    |> Array.concat
    |> Map.ofArray

let getAdjacent (y, x) =
    [(-1, -1); (-1, 0); (-1, 1); (0, -1); (0, 1); (1, -1); (1, 0); (1, 1)]
    |> List.map (fun (dy, dx) -> (y + dy, x + dx))

let countAccessibleRolls grid =
    grid
    |> Map.filter (fun _ c -> c = '@')
    |> Map.toSeq
    |> Seq.filter (fun (pos, _) ->
        getAdjacent pos
        |> List.filter (fun adjPos -> 
            Map.tryFind adjPos grid 
            |> Option.map ((=) '@') 
            |> Option.defaultValue false)
        |> List.length
        |> (fun count -> count < 4))
    |> Seq.length

let getAccessiblePositions grid =
    grid
    |> Map.filter (fun _ c -> c = '@')
    |> Map.toSeq
    |> Seq.filter (fun (pos, _) ->
        getAdjacent pos
        |> List.filter (fun adjPos -> 
            Map.tryFind adjPos grid 
            |> Option.map ((=) '@') 
            |> Option.defaultValue false)
        |> List.length
        |> (fun count -> count < 4))
    |> Seq.map fst
    |> Set.ofSeq

let rec removeAccessibleRolls grid totalRemoved =
    let accessible = getAccessiblePositions grid
    if Set.isEmpty accessible then
        totalRemoved
    else
        let newGrid = 
            accessible
            |> Set.fold (fun g pos -> Map.remove pos g) grid
        removeAccessibleRolls newGrid (totalRemoved + Set.count accessible)

let solvePhase1 filePath =
    filePath
    |> File.ReadAllLines
    |> parseGrid
    |> countAccessibleRolls

let solvePhase2 filePath =
    filePath
    |> File.ReadAllLines
    |> parseGrid
    |> (fun grid -> removeAccessibleRolls grid 0)

printfn "Example Phase 1: %d" (solvePhase1 "input/input04_example.txt")
printfn "Example Phase 2: %d" (solvePhase2 "input/input04_example.txt")
printfn "Real Phase 1: %d" (solvePhase1 "input/input04.txt")
printfn "Real Phase 2: %d" (solvePhase2 "input/input04.txt")
