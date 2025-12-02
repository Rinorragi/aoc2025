open System.IO

let isInvalid (id: int64) =
    let s = string id
    let len = s.Length
    len % 2 = 0 && s.[0] <> '0' && s.[0..(len/2-1)] = s.[len/2..]

let parseRange (range: string) =
    let parts = range.Split('-')
    int64 parts.[0], int64 parts.[1]

let sumInvalidInRange (a, b) =
    seq { a .. b }
    |> Seq.filter isInvalid
    |> Seq.sum

File.ReadAllText("input/input02_example.txt").Split(',')
|> Array.map (fun s -> s.Trim())
|> Array.map parseRange
|> Array.map sumInvalidInRange
|> Array.sum
|> printfn "%d"
