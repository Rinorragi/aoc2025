open System.IO

let isRepeatedExactlyTwice (s: string) =
    let len = s.Length
    len % 2 = 0 && len > 0 && s.[0] <> '0' &&
    s.[..len/2-1] = s.[len/2..]

let isRepeatedAtLeastTwice (s: string) =
    let len = s.Length
    if len < 2 || s.[0] = '0' then false
    else
        [2..len]
        |> List.exists (fun reps ->
            len % reps = 0 &&
            let patLen = len / reps
            [1..reps-1] |> List.forall (fun i -> s.[0..patLen-1] = s.[i*patLen..(i+1)*patLen-1]))

let inputFile = fsi.CommandLineArgs |> Array.tryItem 1 |> Option.defaultValue "input/input02.txt"

let numbers =
    File.ReadAllText(inputFile).Split(',')
    |> Seq.collect (fun range -> 
        let parts = range.Split('-')
        seq { int64 parts.[0] .. int64 parts.[1] })
    |> Seq.map string
    |> Seq.toList

let part1 = numbers |> List.filter isRepeatedExactlyTwice |> List.sumBy int64
let part2 = numbers |> List.filter isRepeatedAtLeastTwice |> List.sumBy int64

printfn "%d" part1
printfn "%d" part2
