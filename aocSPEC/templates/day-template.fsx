(***
Advent of Code Day Template (F# fsi)
Usage (PowerShell):

  # Example input
  fsi templates/day-template.fsx --day 01 --part 1 --example

  # Real input
  fsi templates/day-template.fsx --day 01 --part 2

Inputs are expected at (repo root):
  ./input/inputXX_example.txt
  ./input/inputXX.txt
where XX is the zero-padded day number.

Note: Instruction fetching via MCP is handled by speckit workflows, not code.

Flags:
  --day NN    : required, zero-padded day (e.g., 01)
  --part N    : required, puzzle part (1 or 2)
  --example   : optional, use example input file
***)

open System
open System.IO
open System.Diagnostics

let parseArgs (argv: string[]) =
    let rec loop i dayOpt partOpt useExample =
        if i >= argv.Length then (dayOpt, partOpt, useExample)
        else
            match argv.[i] with
            | "--day" when i + 1 < argv.Length -> loop (i+2) (Some argv.[i+1]) partOpt useExample
            | "--part" when i + 1 < argv.Length -> loop (i+2) dayOpt (Some (int argv.[i+1])) useExample
            | "--example" -> loop (i+1) dayOpt partOpt true
            | unknown ->
                eprintfn "Unknown argument: %s" unknown
                loop (i+1) dayOpt partOpt useExample
    match loop 0 None None false with
    | Some d, Some p, ex -> d, p, ex
    | _ ->
        eprintfn "Missing required --day NN and/or --part N"
        eprintfn "Example: fsi day01.fsx --day 01 --part 1 --example"
        Environment.Exit 2; "00", 0, false

let readInput (day: string) (useExample: bool) =
    let baseDir = __SOURCE_DIRECTORY__
    let fileName = if useExample then sprintf "input%s_example.txt" day else sprintf "input%s.txt" day
    // Repo root: __SOURCE_DIRECTORY__/input/inputXX*.txt
    let inputPath = Path.Combine(baseDir, "input", fileName)
    if not (File.Exists inputPath) then
        eprintfn "Input file not found: %s" inputPath
        Environment.Exit 3
    File.ReadAllLines inputPath |> Array.toList

// Parsing helpers
let parseLines (lines: string list) =
    // Adjust per puzzle
    lines

// Instruction management is external; this script only reads inputs.

// Part 1 solution
let solvePart1 (data: string list) =
    // TODO: implement part 1 logic
    // Return string to print
    sprintf "Not implemented (Part 1). Items=%d" data.Length

// Part 2 solution
let solvePart2 (data: string list) =
    // TODO: implement part 2 logic
    sprintf "Not implemented (Part 2). Items=%d" data.Length

let main argv =
    let swTotal = Stopwatch.StartNew()
    let day, part, useExample = parseArgs argv
    let lines = readInput day useExample
    let data = parseLines lines
    let result =
        match part with
        | 1 -> solvePart1 data
        | 2 -> solvePart2 data
        | n ->
            eprintfn "Unsupported part: %d" n
            Environment.Exit 4; ""
    printfn "%s" result
    swTotal.Stop()
    eprintfn "Timing: total=%dms" swTotal.ElapsedMilliseconds
    0

// FSI invocation
main fsi.CommandLineArgs.[1..] |> ignore
