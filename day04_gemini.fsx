open System.IO

let adjacentDeltas = 
    [(-1, -1); (-1, 0); (-1, 1); (0, -1); (0, 1); (1, -1); (1, 0); (1, 1)]

let countNeighbors (grid: char[][]) (row, col) =
    adjacentDeltas
    |> List.filter (fun (dr, dc) ->
        let r, c = row + dr, col + dc
        r >= 0 && r < grid.Length && c >= 0 && c < grid.[0].Length && grid.[r].[c] = '@')
    |> List.length

let parseGrid (input: string) =
    input.Split([|'\n'; '\r'|], System.StringSplitOptions.RemoveEmptyEntries)
    |> Array.map (fun (line: string) -> line.ToCharArray())

let phase1 (grid: char[][]) =
    seq {
        for row in 0 .. grid.Length - 1 do
            for col in 0 .. grid.[row].Length - 1 do
                if grid.[row].[col] = '@' && countNeighbors grid (row, col) < 4 then
                    yield 1
    }
    |> Seq.sum

let findAccessible (grid: char[][]) =
    seq {
        for row in 0 .. grid.Length - 1 do
            for col in 0 .. grid.[row].Length - 1 do
                if grid.[row].[col] = '@' && countNeighbors grid (row, col) < 4 then
                    yield (row, col)
    }
    |> Set.ofSeq

let removeRolls (grid: char[][]) (positions: Set<int * int>) =
    grid |> Array.mapi (fun r row ->
        row |> Array.mapi (fun c cell ->
            if Set.contains (r, c) positions then '.' else cell))

let rec iterativeRemoval (grid: char[][]) (totalRemoved: int) =
    let accessible = findAccessible grid
    if Set.isEmpty accessible then
        totalRemoved
    else
        let newGrid = removeRolls grid accessible
        iterativeRemoval newGrid (totalRemoved + Set.count accessible)

let phase2 (grid: char[][]) =
    iterativeRemoval grid 0

let exampleInput = File.ReadAllText("input/input04_example.txt")
let realInput = File.ReadAllText("input/input04.txt")

let exampleGrid = parseGrid exampleInput
let realGrid = parseGrid realInput

printfn "Example Phase 1: %d" (phase1 exampleGrid)
printfn "Example Phase 2: %d" (phase2 exampleGrid)
printfn "Real Phase 1: %d" (phase1 realGrid)
printfn "Real Phase 2: %d" (phase2 realGrid)
