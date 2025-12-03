open System.IO

let maxJoltagePart1 bank =
    let indexed = bank |> Seq.indexed |> Seq.toList
    List.allPairs indexed indexed
    |> List.filter (fun ((i, _), (j, _)) -> i < j)
    |> List.map (fun ((_, c1), (_, c2)) -> int (string c1 + string c2))
    |> List.max

let maxJoltagePart2 bank =
    let digits = bank |> Seq.toArray
    let rec selectMax pos remaining acc =
        if remaining = 0 then acc
        else
            let minStartPos = pos
            let maxStartPos = digits.Length - remaining
            let bestDigit = digits.[minStartPos..maxStartPos] |> Array.max
            let bestPos = digits.[minStartPos..maxStartPos] |> Array.findIndex ((=) bestDigit) |> (+) minStartPos
            selectMax (bestPos + 1) (remaining - 1) (acc + string bestDigit)
    selectMax 0 12 "" |> int64

let solvePart1 lines =
    lines
    |> Array.map maxJoltagePart1
    |> Array.sum

let solvePart2 lines =
    lines
    |> Array.map maxJoltagePart2
    |> Array.sum

let example = File.ReadAllLines "input/input03_example.txt"
let real = File.ReadAllLines "input/input03.txt"

printfn "Part 1:"
printfn "Example: %d" (solvePart1 example)
printfn "Real: %d" (solvePart1 real)

printfn "\nPart 2:"
printfn "Example: %d" (solvePart2 example)
printfn "Real: %d" (solvePart2 real)
