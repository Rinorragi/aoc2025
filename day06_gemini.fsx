open System
open System.Diagnostics

/// Parse input: extract column ranges (separated by all-space columns) and operation symbols
let parseInput (lines: string[]) : (int * int) list * string[] * string =
    let opLine = lines.[lines.Length - 1]
    let dataLines = lines |> Array.take (lines.Length - 1)
    
    /// Check if a column index contains only spaces in data lines
    let isAllSpaceColumn idx =
        dataLines |> Array.forall (fun line -> idx >= line.Length || line.[idx] = ' ')
    
    /// Find all column ranges (contiguous non-space regions)
    let rec findRanges idx acc =
        if idx >= opLine.Length then
            List.rev acc
        elif opLine.[idx] = ' ' && isAllSpaceColumn idx then
            findRanges (idx + 1) acc
        elif opLine.[idx] <> ' ' then
            let start = idx
            let endPos =
                Seq.initInfinite ((+) idx)
                |> Seq.takeWhile (fun j -> j < opLine.Length && (opLine.[j] <> ' ' || not (isAllSpaceColumn j)))
                |> Seq.last
            findRanges (endPos + 1) ((start, endPos) :: acc)
        else
            findRanges (idx + 1) acc
    
    let colRanges = findRanges 0 []
    (colRanges, dataLines, opLine)

/// Extract numbers from a column range
let extractNumbers (dataLines: string[]) (start: int) (endPos: int) : bigint[] =
    dataLines
    |> Array.choose (fun line ->
        if start < line.Length then
            let s = line.[start .. min endPos (line.Length - 1)].Trim()
            if s = "" then None else Some (bigint.Parse s)
        else None)

/// Compute the result for a single column
let computeColumn (dataLines: string[]) (opLine: string) (start: int) (endPos: int) : bigint =
    let op = opLine.[start]
    let nums = extractNumbers dataLines start endPos
    match op with
    | '*' -> Array.fold ( * ) 1I nums
    | '+' -> Array.fold ( + ) 0I nums
    | _ -> 0I

/// Solve phase 1: process all columns and sum results
let solve (lines: string[]) : bigint =
    let (colRanges, dataLines, opLine) = parseInput lines
    colRanges
    |> List.map (fun (start, endPos) -> computeColumn dataLines opLine start endPos)
    |> List.sum

/// Solve phase 2: reverse column order and read each character column bottom-to-top
let solvePhase2 (lines: string[]) : bigint =
    let (colRanges, dataLines, opLine) = parseInput lines
    let reversedColRanges = List.rev colRanges
    let reversedLines = Array.rev dataLines
    
    reversedColRanges
    |> List.map (fun (start, endPos) ->
        let op = opLine.[start]
        let colNums =
            [start..endPos]
            |> List.choose (fun colIdx ->
                let digits =
                    reversedLines
                    |> Array.choose (fun line ->
                        if colIdx < line.Length then
                            let c = line.[colIdx]
                            if c = ' ' then None else Some (string c)
                        else None)
                if Array.length digits > 0 then
                    let numStr = String.concat "" digits |> fun s -> String(s.ToCharArray() |> Array.rev)
                    Some (bigint.Parse numStr)
                else
                    None)
        
        match op with
        | '*' -> List.fold ( * ) 1I colNums
        | '+' -> List.fold ( + ) 0I colNums
        | _ -> 0I)
    |> List.sum

/// Time a function execution and return result with elapsed milliseconds
let timeit f x =
    let sw = Stopwatch.StartNew()
    let result = f x
    sw.Stop()
    (result, sw.ElapsedMilliseconds)

/// Validate example input produces correct output
let validateExample () =
    let exampleLines = System.IO.File.ReadAllLines("input/input06_example.txt")
    let expected1 = 4277556I
    let expected2 = 3263827I
    let (result1, ms1) = timeit solve exampleLines
    let (result2, ms2) = timeit solvePhase2 exampleLines
    if result1 = expected1 && result2 = expected2 then
        printfn "✓ Example OK Phase 1: %A (Time: %d ms)" result1 ms1
        printfn "✓ Example OK Phase 2: %A (Time: %d ms)" result2 ms2
    else
        failwithf "✗ Example failed: Phase 1 got %A expected %A, Phase 2 got %A expected %A" result1 expected1 result2 expected2

/// Run both phases on actual input
let runPhases () =
    let inputLines = System.IO.File.ReadAllLines("input/input06.txt")
    let (part1, ms1) = timeit solve inputLines
    let (part2, ms2) = timeit solvePhase2 inputLines
    printfn "Phase 1: %A (Time: %d ms)" part1 ms1
    printfn "Phase 2: %A (Time: %d ms)" part2 ms2
    printfn ""
    printfn "Summary:"
    printfn "  Phase 1: %d ms" ms1
    printfn "  Phase 2: %d ms" ms2

// Main execution
validateExample()
printfn ""
runPhases()
