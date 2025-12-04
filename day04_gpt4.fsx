open System.IO

let countAdjacentRolls (grid: char array array) (row: int) (col: int) =
    [ (-1, -1); (-1, 0); (-1, 1)
      ( 0, -1);          ( 0, 1)
      ( 1, -1); ( 1, 0); ( 1, 1) ]
    |> List.filter (fun (dr, dc) ->
        let r, c = row + dr, col + dc
        r >= 0 && r < grid.Length && c >= 0 && c < grid.[0].Length && grid.[r].[c] = '@')
    |> List.length

let solvePhase1 (input: string) =
    let grid = 
        input.Split('\n')
        |> Array.map (fun line -> line.Trim().ToCharArray())
        |> Array.filter (fun row -> row.Length > 0)
    
    grid
    |> Array.mapi (fun row line ->
        line
        |> Array.mapi (fun col ch ->
            if ch = '@' && countAdjacentRolls grid row col < 4 then 1 else 0)
        |> Array.sum)
    |> Array.sum

let solvePhase2 (input: string) =
    let initialGrid = 
        input.Split('\n')
        |> Array.map (fun line -> line.Trim().ToCharArray())
        |> Array.filter (fun row -> row.Length > 0)
    
    let rec removeAccessible grid totalRemoved =
        let accessible =
            grid
            |> Array.mapi (fun row line ->
                line
                |> Array.mapi (fun col ch ->
                    if ch = '@' && countAdjacentRolls grid row col < 4 then Some (row, col) else None)
                |> Array.choose id)
            |> Array.concat
        
        if Array.isEmpty accessible then
            totalRemoved
        else
            let newGrid = grid |> Array.map Array.copy
            accessible |> Array.iter (fun (r, c) -> newGrid.[r].[c] <- '.')
            removeAccessible newGrid (totalRemoved + accessible.Length)
    
    removeAccessible initialGrid 0

let exampleInput = File.ReadAllText("input/input04_example.txt")
let realInput = File.ReadAllText("input/input04.txt")

printfn "Example Phase 1: %d" (solvePhase1 exampleInput)
printfn "Example Phase 2: %d" (solvePhase2 exampleInput)
printfn "Real Phase 1: %d" (solvePhase1 realInput)
printfn "Real Phase 2: %d" (solvePhase2 realInput)
