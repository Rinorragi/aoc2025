open System.IO

let isRepeatingSubstring (s: string) =
    let len = s.Length
    len % 2 = 0 && s.[..len/2-1] = s.[len/2..]

let hasLeadingZero (s: string) = s.Length > 1 && s.[0] = '0'

System.IO.File.ReadAllText("input/input02.txt").Split(',')
|> Array.collect (fun range ->
    let parts = range.Split('-')
    let start, finish = int64 parts.[0], int64 parts.[1]
    [|start..finish|])
|> Array.filter (fun n ->
    let s = string n
    not (hasLeadingZero s) && isRepeatingSubstring s)
|> Array.sumBy int64
|> printfn "%d"
