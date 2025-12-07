
open System
open System.Diagnostics

let parseInput (lines: string[]) =
    let opLine = lines.[lines.Length - 1]
    let dataLines = lines |> Array.take (lines.Length - 1)
    // Find column ranges: contiguous non-space regions, separated by columns of only spaces
    let isColSpace idx =
        dataLines |> Array.forall (fun line -> idx >= line.Length || line.[idx] = ' ')
    let rec findRanges idx acc =
        if idx >= opLine.Length then List.rev acc
        elif opLine.[idx] = ' ' && isColSpace idx then findRanges (idx+1) acc
        elif opLine.[idx] <> ' ' then
            let start = idx
            let rec findEnd j =
                if j < opLine.Length && (opLine.[j] <> ' ' || not (isColSpace j)) then findEnd (j+1) else j
            let endPos = findEnd idx
            findRanges endPos ((start, endPos-1)::acc)
        else findRanges (idx+1) acc
    let colRanges = findRanges 0 []
    colRanges, dataLines, opLine

let solve (lines: string[]) =
    let colRanges, dataLines, opLine = parseInput lines
    let colValues =
        colRanges
        |> List.map (fun (start, endPos) ->
            let op = opLine.[start]
            let nums =
                dataLines
                |> Array.choose (fun line ->
                    if start < line.Length then
                        let s = line.[start..min endPos (line.Length-1)].Trim()
                        if s = "" then None else Some (bigint.Parse s)
                    else None)
            match op with
            | '*' -> Array.fold ( * ) 1I nums
            | '+' -> Array.fold ( + ) 0I nums
            | _ -> 0I)
    List.sum colValues

let solvePhase2 (lines: string[]) =
    let colRanges, dataLines, opLine = parseInput lines
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

let timeit f x =
    let sw = Stopwatch.StartNew()
    let result = f x
    sw.Stop()
    result, sw.ElapsedMilliseconds


let validateExample () =
    let exampleLines = System.IO.File.ReadAllLines("input/input06_example.txt")
    let expected1 = 4277556I
    let expected2 = 3263827I
    let res1, ms1 = timeit solve exampleLines
    let res2, ms2 = timeit solvePhase2 exampleLines
    if res1 = expected1 && res2 = expected2 then
        printfn "Example OK Phase 1: %A (Time: %d ms)" res1 ms1
        printfn "Example OK Phase 2: %A (Time: %d ms)" res2 ms2
    else
        failwithf "Example failed: Phase 1 got %A expected %A, Phase 2 got %A expected %A" res1 expected1 res2 expected2


let runPhases () =
    let inputLines = System.IO.File.ReadAllLines("input/input06.txt")
    let part1, ms1 = timeit solve inputLines
    let part2, ms2 = timeit solvePhase2 inputLines
    printfn "Phase 1: %A (Time: %d ms)" part1 ms1
    printfn "Phase 2: %A (Time: %d ms)" part2 ms2


validateExample()
runPhases()
