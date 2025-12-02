open System.IO

let hasRepeatedExactlyTwice (s: string) =
    seq { 1 .. s.Length / 2 }
    |> Seq.exists (fun len ->
        let chunk = s.Substring(0, len)
        s.Replace(chunk, "").Length = 0 && s.Length / len = 2)

let hasRepeatedAtLeastTwice (s: string) =
    seq { 1 .. s.Length / 2 }
    |> Seq.exists (fun len ->
        let chunk = s.Substring(0, len)
        s.Replace(chunk, "").Length = 0 && s.Length / len > 1)

let isInvalidPart1 (s: string) =
    not (s.StartsWith("0")) && hasRepeatedExactlyTwice s

let isInvalidPart2 (s: string) =
    not (s.StartsWith("0")) && hasRepeatedAtLeastTwice s

let inputFile = fsi.CommandLineArgs.[1]

let ids =
    File.ReadAllText(inputFile).Split(',')
    |> Seq.collect (fun range ->
        let parts = range.Split('-')
        let start, end' = int64 parts.[0], int64 parts.[1]
        seq { start .. end' })
    |> Seq.map string
    |> Seq.toList

let part1 = ids |> List.filter isInvalidPart1 |> List.sumBy int64
let part2 = ids |> List.filter isInvalidPart2 |> List.sumBy int64

printfn "%d" part1
printfn "%d" part2
