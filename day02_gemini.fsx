open System.IO

let hasRepeatedSubstring (s: string) =
    seq { 1 .. s.Length / 2 }
    |> Seq.exists (fun len ->
        let chunk = s.Substring(0, len)
        s.Replace(chunk, "").Length = 0 && s.Length / len > 1)

let isValidId (s: string) =
    not (s.StartsWith("0")) && hasRepeatedSubstring s

File.ReadAllText("input/input02.txt").Split(',')
|> Seq.collect (fun range ->
    let parts = range.Split('-')
    let start, end' = int64 parts.[0], int64 parts.[1]
    seq { start .. end' })
|> Seq.map string
|> Seq.filter isValidId
|> Seq.sumBy int64
|> printfn "%d"
