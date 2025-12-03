open System.IO

// Part 1: Find max joltage from any 2 batteries
let maxJoltage bank =
    let indexed = bank |> Seq.toList |> List.indexed
    List.allPairs indexed indexed
    |> List.filter (fun ((i, _), (j, _)) -> i < j)
    |> List.map (fun ((_, d1), (_, d2)) -> int (string d1 + string d2))
    |> List.max

let solvePart1 lines =
    lines
    |> Array.map maxJoltage
    |> Array.sum

// Part 2: Find max joltage from exactly 12 batteries (greedy subsequence)
let maxJoltage12 bank =
    let digits = bank |> Seq.toList
    let n = List.length digits
    let k = 12
    
    let rec greedy pos selected remaining =
        if remaining = 0 then List.rev selected
        else
            let minNeeded = remaining - 1
            let maxPos = n - minNeeded - 1
            let candidates = digits.[pos..maxPos]
            let maxDigit = List.max candidates
            let nextPos = digits.[pos..] |> List.findIndex ((=) maxDigit) |> (+) pos
            greedy (nextPos + 1) (maxDigit :: selected) (remaining - 1)
    
    greedy 0 [] k
    |> List.map string
    |> String.concat ""
    |> int64

let solvePart2 lines =
    lines
    |> Array.map maxJoltage12
    |> Array.sum

let exampleLines = File.ReadAllLines("input/input03_example.txt")
let realLines = File.ReadAllLines("input/input03.txt")

printfn "Example:"
printfn "Part 1: %d" (solvePart1 exampleLines)
printfn "Part 2: %d" (solvePart2 exampleLines)

printfn "\nReal:"
printfn "Part 1: %d" (solvePart1 realLines)
printfn "Part 2: %d" (solvePart2 realLines)
