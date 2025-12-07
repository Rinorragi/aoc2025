open System
open System.Diagnostics

let parseInput (lines: string[]) =
    let opLine = lines.[lines.Length - 1]
    let dataLines = lines |> Array.take (lines.Length - 1)
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

let exampleLines = [| "123 328  51 64 "; " 45 64  387 23 "; "  6 98  215 314"; "*   +   *   +  " |]

let colRanges, dataLines, opLine = parseInput exampleLines

printfn "Column ranges: %A" colRanges
printfn ""
printfn "Data lines:"
for i, line in dataLines |> Array.indexed do
    printfn "  [%d]: '%s'" i line

printfn ""
printfn "Phase 1 - Forward order:"
for (start, endPos) in colRanges do
    let op = opLine.[start]
    let nums =
        dataLines
        |> Array.choose (fun line ->
            if start < line.Length then
                let s = line.[start..min endPos (line.Length-1)].Trim()
                if s = "" then None else Some (bigint.Parse s)
            else None)
    let result = 
        match op with
        | '*' -> Array.fold ( * ) 1I nums
        | '+' -> Array.fold ( + ) 0I nums
        | _ -> 0I
    printfn "  Cols %d-%d op '%c': %A = %A" start endPos op nums result

printfn ""
printfn "Phase 2 - Reverse order:"
let reversedRanges = List.rev colRanges
let reversedLines = Array.rev dataLines
printfn "Reversed ranges: %A" reversedRanges
printfn "Reversed lines:"
for i, line in reversedLines |> Array.indexed do
    printfn "  [%d]: '%s'" i line

for (start, endPos) in reversedRanges do
    let op = opLine.[start]
    let nums =
        reversedLines
        |> Array.choose (fun line ->
            if start < line.Length then
                let s = line.[start..min endPos (line.Length-1)].Trim()
                if s = "" then None else Some (bigint.Parse s)
            else None)
    let result = 
        match op with
        | '*' -> Array.fold ( * ) 1I nums
        | '+' -> Array.fold ( + ) 0I nums
        | _ -> 0I
    printfn "  Cols %d-%d op '%c': %A = %A" start endPos op nums result
