open System.IO

// Part 1: Pick exactly 2 digits
let maxJoltage bank =
    let digits = bank |> Seq.toList
    List.allPairs [0..digits.Length-1] [0..digits.Length-1]
    |> List.filter (fun (i, j) -> i < j)
    |> List.map (fun (i, j) -> int (string digits.[i] + string digits.[j]))
    |> List.max

let solve1 lines =
    lines
    |> Array.map maxJoltage
    |> Array.sum

// Part 2: Pick exactly 12 digits (lexicographically largest subsequence)
let maxJoltage12 bank =
    let digits = bank |> Seq.toList
    let n = List.length digits
    
    let rec selectDigits pos selected remaining =
        if remaining = 0 then selected
        else
            let minNeeded = remaining - 1
            let maxChar = 
                [pos .. n - minNeeded - 1]
                |> List.map (fun i -> digits.[i], i)
                |> List.maxBy fst
            let (digit, idx) = maxChar
            selectDigits (idx + 1) (selected @ [digit]) (remaining - 1)
    
    selectDigits 0 [] 12
    |> List.map string
    |> String.concat ""
    |> int64

let solve2 lines =
    lines
    |> Array.map maxJoltage12
    |> Array.sum

let exampleLines = File.ReadAllLines("input/input03_example.txt")
let realLines = File.ReadAllLines("input/input03.txt")

printfn "Part 1:"
printfn "Example: %d" (solve1 exampleLines)
printfn "Real: %d" (solve1 realLines)
printfn ""
printfn "Part 2:"
printfn "Example: %d" (solve2 exampleLines)
printfn "Real: %d" (solve2 realLines)
