open System.IO

let hasLeadingZero (s: string) = s.Length > 1 && s.[0] = '0'

// Part 1: Check if string is a sequence repeated EXACTLY twice
let isRepeatedExactlyTwice (s: string) =
    let len = s.Length
    len % 2 = 0 && s.[..len/2-1] = s.[len/2..]

// Part 2: Check if string is a sequence repeated AT LEAST twice
let isRepeatedAtLeastTwice (s: string) =
    let len = s.Length
    [1 .. len/2]
    |> List.exists (fun patternLen ->
        len % patternLen = 0 &&
        let pattern = s.[..patternLen-1]
        s |> Seq.chunkBySize patternLen 
           |> Seq.map System.String 
           |> Seq.forall ((=) pattern))

let inputFile = fsi.CommandLineArgs.[1]

let numbers =
    File.ReadAllText(inputFile).Split(',')
    |> Array.collect (fun range ->
        let parts = range.Split('-')
        let start, finish = int64 parts.[0], int64 parts.[1]
        [|start..finish|])

let part1 =
    numbers
    |> Array.filter (fun n ->
        let s = string n
        not (hasLeadingZero s) && isRepeatedExactlyTwice s)
    |> Array.sumBy int64

let part2 =
    numbers
    |> Array.filter (fun n ->
        let s = string n
        not (hasLeadingZero s) && isRepeatedAtLeastTwice s)
    |> Array.sumBy int64

printfn "%d" part1
printfn "%d" part2
