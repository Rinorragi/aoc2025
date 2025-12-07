open System
open System.Diagnostics

// Beams only move DOWN. Track active beam columns per row.
let findStart (lines: string[]) =
    lines
    |> Array.tryFindIndex (fun line -> line.Contains('S'))
    |> Option.map (fun row -> (row, lines.[row].IndexOf('S')))

let solve (lines: string[]) =
    match findStart lines with
    | None -> 0
    | Some (startRow, startCol) ->
        let height = lines.Length
        let width = lines.[0].Length
        
        // Process row by row, tracking active beam columns
        let rec processRows row activeColumns splitCount =
            if row >= height || Set.isEmpty activeColumns then
                splitCount
            else
                // Check each active column for splitters in this row
                let (newColumns, newSplits) =
                    activeColumns
                    |> Set.fold (fun (cols, splits) col ->
                        if col >= 0 && col < width then
                            match lines.[row].[col] with
                            | '^' ->
                                // Splitter: beam stops, two new beams start left and right
                                let leftCol = col - 1
                                let rightCol = col + 1
                                let newCols = 
                                    cols 
                                    |> (if leftCol >= 0 then Set.add leftCol else id)
                                    |> (if rightCol < width then Set.add rightCol else id)
                                (newCols, splits + 1)
                            | '.' | 'S' ->
                                // Empty space: beam continues
                                (Set.add col cols, splits)
                            | _ -> (cols, splits)
                        else (cols, splits)
                    ) (Set.empty, 0)
                processRows (row + 1) newColumns (splitCount + newSplits)
        
        // Start from row after S, with single beam at S column
        processRows (startRow + 1) (Set.singleton startCol) 0

let solvePhase2 (lines: string[]) =
    match findStart lines with
    | None -> 0L
    | Some (startRow, startCol) ->
        let height = lines.Length
        let width = lines.[0].Length
        let dp = Array.init height (fun _ -> Array.zeroCreate<int64> width)
        dp.[startRow].[startCol] <- 1L
        for row in startRow+1 .. height-1 do
            for col in 0 .. width-1 do
                let above = dp.[row-1].[col]
                if above > 0L then
                    match lines.[row].[col] with
                    | '^' ->
                        if col > 0 then dp.[row].[col-1] <- dp.[row].[col-1] + above
                        if col < width-1 then dp.[row].[col+1] <- dp.[row].[col+1] + above
                    | '.' | 'S' ->
                        dp.[row].[col] <- dp.[row].[col] + above
                    | _ -> ()
        Array.sum dp.[height-1]

let timeit f x =
    let sw = Stopwatch.StartNew()
    let result = f x
    sw.Stop()
    result, sw.ElapsedMilliseconds

let runPhases () =
    // Test with example first
    let exampleLines = System.IO.File.ReadAllLines("input/input07_example.txt")
    let exampleResult = solve exampleLines
    let exampleResult2 = solvePhase2 exampleLines
    printfn "Example P1: %d (expected: 21), P2: %d (expected: 40)" exampleResult exampleResult2
    
    let inputLines = System.IO.File.ReadAllLines("input/input07.txt")
    let part1, ms1 = timeit solve inputLines
    let part2, ms2 = timeit solvePhase2 inputLines
    printfn "Phase 1: %d (Time: %d ms)" part1 ms1
    printfn "Phase 2: %d (Time: %d ms)" part2 ms2

runPhases()
