open System.IO

let isRepeatedExactlyTwice (s: string) =
    let len = s.Length
    len % 2 = 0 
    && s.[0] <> '0' 
    && s.[0..len/2-1] = s.[len/2..]

let isRepeatedAtLeastTwice (s: string) =
    let len = s.Length
    if s.[0] = '0' then false
    else
        seq { 1 .. len/2 }
        |> Seq.exists (fun patternLen ->
            if len % patternLen <> 0 || len / patternLen < 2 then false
            else
                let pattern = s.[0..patternLen-1]
                seq { 0 .. len/patternLen - 1 }
                |> Seq.forall (fun i -> s.[i*patternLen..(i+1)*patternLen-1] = pattern))

let isInvalidPart1 (id: int64) = isRepeatedExactlyTwice (string id)
let isInvalidPart2 (id: int64) = isRepeatedAtLeastTwice (string id)

let parseRange (range: string) =
    let parts = range.Split('-')
    int64 parts.[0], int64 parts.[1]

let sumInvalidInRange isInvalid (a, b) =
    seq { a .. b }
    |> Seq.filter isInvalid
    |> Seq.sum

let filePath = fsi.CommandLineArgs.[1]
let ranges = 
    File.ReadAllText(filePath).Split(',')
    |> Array.map (fun s -> s.Trim())
    |> Array.map parseRange

let part1 = ranges |> Array.map (sumInvalidInRange isInvalidPart1) |> Array.sum
let part2 = ranges |> Array.map (sumInvalidInRange isInvalidPart2) |> Array.sum

printfn "%d" part1
printfn "%d" part2
