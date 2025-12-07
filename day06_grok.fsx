
open System
open System.Diagnostics

let parseInput (lines: string[]) =
    let opLine = lines |> Array.last
    let dataLines = lines |> Array.take (lines.Length - 1)
    let colIndices =
        opLine
        |> Seq.indexed
        |> Seq.filter (fun (_, c) -> c <> ' ')
        |> Seq.map fst
        |> Seq.toList
    let colRanges =
        let rec loop idx acc =
            match colIndices |> List.tryItem idx with
            | None -> List.rev acc
            | Some start ->
                let nextSpace =
                    colIndices
                    |> List.tryItem (idx + 1)
                    |> Option.defaultValue (opLine.Length)
                loop (idx + 1) ((start, nextSpace - 1) :: acc)
        loop 0 []
    dataLines, opLine, colRanges

let solvePhase (dataLines: string[], opLine: string, colRanges: (int*int) list) =
    let evalCol (start, endPos) =
        let op = opLine.[start]
        let nums =
            dataLines
            |> Array.choose (fun line ->
                if start < line.Length then
                    let substr = line.Substring(start, min (endPos - start + 1) (line.Length - start)).Trim()
                    if substr = "" then None else Some (int64 substr)
                else None)
            |> Array.toList
        match op with
        | '*' -> List.fold ( * ) 1L nums
        | '+' -> List.fold ( + ) 0L nums
        | _ -> failwithf "Unknown op: %c" op
    colRanges |> List.map evalCol |> List.sum

let solvePhase2 (dataLines: string[], opLine: string, colRanges: (int*int) list) =
    let reversedRanges = List.rev colRanges
    let reversedLines = Array.rev dataLines
    reversedRanges
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
                    Some (int64 numStr)
                else
                    None)
        
        match op with
        | '*' -> List.fold ( * ) 1L colNums
        | '+' -> List.fold ( + ) 0L colNums
        | _ -> 0L)
    |> List.sum

let runWithInput inputPath =
    let lines = System.IO.File.ReadAllLines inputPath
    let dataLines, opLine, colRanges = parseInput lines
    let sw1 = Stopwatch.StartNew()
    let phase1 = solvePhase (dataLines, opLine, colRanges)
    sw1.Stop()
    phase1, sw1.ElapsedMilliseconds

let validateExample () =
    let lines = System.IO.File.ReadAllLines "input/input06_example.txt"
    let dataLines, opLine, colRanges = parseInput lines
    let sw1 = Stopwatch.StartNew()
    let phase1 = solvePhase (dataLines, opLine, colRanges)
    sw1.Stop()
    let sw2 = Stopwatch.StartNew()
    let phase2 = solvePhase2 (dataLines, opLine, colRanges)
    sw2.Stop()
    let expected1 = 4277556L
    let expected2 = 3263827L
    if phase1 <> expected1 || phase2 <> expected2 then
        failwithf "Example failed: Phase 1 got %d expected %d, Phase 2 got %d expected %d" phase1 expected1 phase2 expected2
    printfn "Example OK: Phase 1: %d (time: %d ms), Phase 2: %d (time: %d ms)" phase1 sw1.ElapsedMilliseconds phase2 sw2.ElapsedMilliseconds

validateExample()

let phase1, ms1 = runWithInput "input/input06.txt"
printfn "Phase 1: %d" phase1
printfn "Phase 1 time: %d ms" ms1

let lines = System.IO.File.ReadAllLines "input/input06.txt"
let dataLines, opLine, colRanges = parseInput lines
let sw2 = Stopwatch.StartNew()
let phase2 = solvePhase2 (dataLines, opLine, colRanges)
sw2.Stop()
printfn "Phase 2: %d" phase2
printfn "Phase 2 time: %d ms" sw2.ElapsedMilliseconds
