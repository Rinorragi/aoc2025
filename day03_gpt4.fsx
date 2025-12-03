open System.IO

let maxJoltage (bank: string) =
    bank
    |> Seq.mapi (fun i c -> i, c)
    |> Seq.toList
    |> fun chars -> List.allPairs chars chars
    |> List.filter (fun ((i, _), (j, _)) -> i < j)
    |> List.map (fun ((_, c1), (_, c2)) -> int (string c1 + string c2))
    |> List.max

let solve (lines: string[]) =
    lines
    |> Array.map maxJoltage
    |> Array.sum

let maxJoltage12 (bank: string) =
    let chars = bank |> Seq.toList
    let rec greedy pos remaining acc =
        if remaining = 0 then acc |> List.rev |> List.map string |> String.concat "" |> int64
        else
            let minStart = pos
            let maxStart = List.length chars - remaining
            chars.[minStart..maxStart]
            |> List.mapi (fun i c -> minStart + i, c)
            |> List.maxBy snd
            |> fun (idx, ch) -> greedy (idx + 1) (remaining - 1) (ch :: acc)
    greedy 0 12 []

let solve2 (lines: string[]) =
    lines
    |> Array.map maxJoltage12
    |> Array.sum

let lines1 = File.ReadAllLines("input/input03_example.txt")
let lines2 = File.ReadAllLines("input/input03.txt")

printfn "Part 1:"
printfn "Example: %d" (solve lines1)
printfn "Real: %d" (solve lines2)

printfn "\nPart 2:"
printfn "Example: %d" (solve2 lines1)
printfn "Real: %d" (solve2 lines2)
