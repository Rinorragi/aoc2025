open System.IO

let parseInstruction (s: string) = (s.[0], int s.[1..])

let countZeroCrossings startPos dir amount =
    if amount = 0 then 0
    else
        let firstZeroAt = 
            match dir, startPos with
            | _, 0 -> 100
            | 'L', _ -> startPos
            | 'R', _ -> 100 - startPos
            | _ -> amount + 1
        if firstZeroAt <= amount then 1 + (amount - firstZeroAt) / 100 else 0

let turn pos dir amount =
    match dir with
    | 'L' -> (pos - amount % 100 + 100) % 100
    | 'R' -> (pos + amount) % 100
    | _ -> pos

let solve startPos instructions =
    ((startPos, 0, 0), instructions)
    ||> List.fold (fun (pos, stops, crossings) (dir, amt) ->
        let newPos = turn pos dir amt
        (newPos, 
         stops + (if newPos = 0 then 1 else 0), 
         crossings + countZeroCrossings pos dir amt))

let input = 
    File.ReadAllLines("input/input01.txt")
    |> Array.filter ((<>) "")
    |> Array.map parseInstruction
    |> Array.toList

let (_, stopZero, zeroCrossings) = solve 50 input

printfn "Times stopped at 0: %d" stopZero
printfn "Times dial pointed at 0: %d" zeroCrossings