open System.IO

let adjacentOffsets = 
    [ (-1, -1); (-1, 0); (-1, 1)
      ( 0, -1);          ( 0, 1)
      ( 1, -1); ( 1, 0); ( 1, 1) ]

let countAdjacentPapers grid row col =
    adjacentOffsets
    |> List.filter (fun (dr, dc) ->
        let r, c = row + dr, col + dc
        r >= 0 && r < Array.length grid && 
        c >= 0 && c < Array.length grid.[0] && 
        grid.[r].[c] = '@')
    |> List.length

let solvePhase1 input =
    input
    |> String.filter ((<>) '\r')
    |> fun s -> s.Split('\n')
    |> Array.filter ((<>) "")
    |> Array.map (fun line -> line.ToCharArray())
    |> fun grid ->
        grid
        |> Array.mapi (fun row line ->
            line
            |> Array.mapi (fun col ch ->
                if ch = '@' && countAdjacentPapers grid row col < 4 then 1 else 0)
            |> Array.sum)
        |> Array.sum

let solvePhase2 input =
    let initialGrid =
        input
        |> String.filter ((<>) '\r')
        |> fun s -> s.Split('\n')
        |> Array.filter ((<>) "")
        |> Array.map (fun line -> line.ToCharArray())
    
    let rec removeAccessible grid totalRemoved =
        let accessible =
            grid
            |> Array.mapi (fun row line ->
                line
                |> Array.mapi (fun col ch ->
                    if ch = '@' && countAdjacentPapers grid row col < 4 then Some (row, col) else None)
                |> Array.choose id)
            |> Array.concat
        
        if Array.isEmpty accessible then
            totalRemoved
        else
            let newGrid = grid |> Array.map Array.copy
            accessible |> Array.iter (fun (r, c) -> newGrid.[r].[c] <- '.')
            removeAccessible newGrid (totalRemoved + Array.length accessible)
    
    removeAccessible initialGrid 0

let exampleInput = File.ReadAllText("input/input04_example.txt")
let realInput = File.ReadAllText("input/input04.txt")

printfn "Example Phase 1: %d" (solvePhase1 exampleInput)
printfn "Example Phase 2: %d" (solvePhase2 exampleInput)
printfn "Real Phase 1: %d" (solvePhase1 realInput)
printfn "Real Phase 2: %d" (solvePhase2 realInput)
