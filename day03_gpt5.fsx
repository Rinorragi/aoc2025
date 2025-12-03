open System.IO

let allPairs seq =
    seq
    |> Seq.indexed
    |> Seq.collect (fun (i, x) -> 
        seq |> Seq.skip (i + 1) |> Seq.map (fun y -> (x, y)))

let maxJoltage (line: string) =
    line
    |> Seq.map (fun c -> int c - int '0')
    |> allPairs
    |> Seq.map (fun (i, j) -> i * 10 + j)
    |> Seq.max

let maxJoltage12 (line: string) =
    let digits = line |> Seq.map (fun c -> int c - int '0') |> Seq.toArray
    let n = digits.Length
    
    let rec selectGreedy pos needed acc =
        if needed = 0 then acc
        else
            let minRemaining = needed - 1
            let maxSearchPos = n - minRemaining - 1
            let maxDigit = 
                digits 
                |> Array.skip pos 
                |> Array.take (maxSearchPos - pos + 1)
                |> Array.max
            let nextPos = 
                digits 
                |> Array.indexed 
                |> Array.skip pos
                |> Array.take (maxSearchPos - pos + 1)
                |> Array.find (fun (_, d) -> d = maxDigit)
                |> fst
            selectGreedy (nextPos + 1) (needed - 1) (acc @ [maxDigit])
    
    selectGreedy 0 12 []
    |> List.fold (fun acc d -> acc * 10L + int64 d) 0L

let solve1 (inputFile: string) =
    File.ReadAllLines inputFile
    |> Seq.map maxJoltage
    |> Seq.sum

let solve2 (inputFile: string) =
    File.ReadAllLines inputFile
    |> Seq.map maxJoltage12
    |> Seq.sum

printfn "Part 1:"
printfn "Example: %d" (solve1 "input/input03_example.txt")
printfn "Real: %d" (solve1 "input/input03.txt")

printfn "\nPart 2:"
printfn "Example: %d" (solve2 "input/input03_example.txt")
printfn "Real: %d" (solve2 "input/input03.txt")
