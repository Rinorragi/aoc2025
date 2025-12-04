open System.IO

let adjacent = [(-1,-1);(-1,0);(-1,1);(0,-1);(0,1);(1,-1);(1,0);(1,1)]

let countAdjacent pos grid =
    adjacent 
    |> List.map (fun (dy,dx) -> (fst pos + dy, snd pos + dx))
    |> List.filter (fun p -> grid |> Map.tryFind p |> Option.filter ((=) '@') |> Option.isSome)
    |> List.length

let phase1 grid =
    grid 
    |> Map.filter (fun _ c -> c = '@') 
    |> Map.toSeq 
    |> Seq.filter (fun (pos,_) -> countAdjacent pos grid < 4)
    |> Seq.length

let rec removeAccessible grid =
    let accessible = 
        grid 
        |> Map.filter (fun _ c -> c = '@')
        |> Map.filter (fun pos _ -> countAdjacent pos grid < 4)
    if accessible.IsEmpty then 
        0
    else
        let newGrid = grid |> Map.filter (fun pos _ -> not (accessible.ContainsKey pos))
        accessible.Count + removeAccessible newGrid

let solve path =
    let grid = 
        File.ReadAllLines path 
        |> Array.mapi (fun y line -> line |> Seq.mapi (fun x c -> ((y,x),c))) 
        |> Seq.concat 
        |> Map.ofSeq
    (phase1 grid, removeAccessible grid)

let exampleResult = solve "input/input04_example.txt"
printfn "Example - Phase 1: %d, Phase 2: %d" (fst exampleResult) (snd exampleResult)

let realResult = solve "input/input04.txt"
printfn "Real - Phase 1: %d, Phase 2: %d" (fst realResult) (snd realResult)
