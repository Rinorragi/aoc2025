open System.IO

let isRepeatedString (s: string) =
    let len = s.Length
    len % 2 = 0 && len > 0 && s.Substring(0, len / 2) = s.Substring(len / 2)

let hasLeadingZero (s: string) = s.Length > 1 && s.[0] = '0'

let isInvalid (n: int64) =
    let s = string n
    not (hasLeadingZero s) && isRepeatedString s

File.ReadAllText("input/input02_example.txt").Split(',')
|> Array.collect (fun range ->
    let parts = range.Trim().Split('-')
    [| int64 parts.[0] .. int64 parts.[1] |])
|> Array.filter isInvalid
|> Array.sum
|> printfn "%d"
