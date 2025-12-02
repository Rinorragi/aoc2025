open System.IO

let isRepeatedDigits (s: string) =
    s.Length % 2 = 0 && s.Length > 0 && s.[0] <> '0' &&
    s.[..s.Length/2-1] = s.[s.Length/2..]

File.ReadAllText("input/input02_example.txt").Split(',')
|> Seq.collect (fun range -> 
    let parts = range.Split('-')
    seq { int64 parts.[0] .. int64 parts.[1] })
|> Seq.filter (string >> isRepeatedDigits)
|> Seq.sum
|> printfn "%d"
