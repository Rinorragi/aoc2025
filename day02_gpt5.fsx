open System.IO

let isRepeatedExactlyTwice (s: string) =
    let len = s.Length
    len % 2 = 0 && len > 0 && s.Substring(0, len / 2) = s.Substring(len / 2)

let isRepeatedAtLeastTwice (s: string) =
    let len = s.Length
    [1 .. len / 2]
    |> List.exists (fun patternLen ->
        len % patternLen = 0 &&
        let pattern = s.Substring(0, patternLen)
        let repetitions = len / patternLen
        repetitions >= 2 && s = String.replicate repetitions pattern)

let hasLeadingZero (s: string) = s.Length > 1 && s.[0] = '0'

let isInvalidPart1 (n: int64) =
    let s = string n
    not (hasLeadingZero s) && isRepeatedExactlyTwice s

let isInvalidPart2 (n: int64) =
    let s = string n
    not (hasLeadingZero s) && isRepeatedAtLeastTwice s

let inputFile = fsi.CommandLineArgs.[1]
let numbers =
    File.ReadAllText(inputFile).Split(',')
    |> Array.collect (fun range ->
        let parts = range.Trim().Split('-')
        [| int64 parts.[0] .. int64 parts.[1] |])

numbers |> Array.filter isInvalidPart1 |> Array.sum |> printfn "%d"
numbers |> Array.filter isInvalidPart2 |> Array.sum |> printfn "%d"
