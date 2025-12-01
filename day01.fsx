open System.IO

// Parse a single instruction like "L68" or "R48"
let parseInstruction (s: string) =
    let direction = s.[0]
    let amount = int (s.Substring(1))
    (direction, amount)

// Count how many times the dial passes through 0 during a single turn
// "Passing through 0" means the dial points at 0 at some point during the turn
let countZeroCrossings (startPos: int) (direction: char) (amount: int) =
    if amount = 0 then 0
    else
        match direction with
        | 'L' -> 
            let firstZeroAt = if startPos = 0 then 100 else startPos
            if firstZeroAt <= amount then
                1 + (amount - firstZeroAt) / 100
            else 0
        | 'R' -> 
            let firstZeroAt = if startPos = 0 then 100 else (100 - startPos)
            if firstZeroAt <= amount then
                1 + (amount - firstZeroAt) / 100
            else 0
        | _ -> 0

// Get the new position after a turn
let turn (currentPos: int) (direction: char) (amount: int) =
    match direction with
    | 'L' -> (currentPos - amount % 100 + 100) % 100
    | 'R' -> (currentPos + amount) % 100
    | _ -> currentPos

// Process all instructions, counting zero crossings
let solve (startPos: int) (instructions: (char * int) list) =
    instructions
    |> List.fold (fun (pos, stopZero, zeroCount) (dir, amt) -> 
        let crossings = countZeroCrossings pos dir amt
        let newPos = turn pos dir amt
        let zeroStop = if newPos = 0 then stopZero + 1 else stopZero
        (newPos, zeroStop, zeroCount + crossings)
    ) (startPos, 0, 0)

// Main
let input = 
    File.ReadAllLines("input/input01.txt")
    |> Array.filter (fun s -> s.Length > 0)
    |> Array.map parseInstruction
    |> Array.toList

let startPosition = 50
let (finalPos, stopZero, zeroCrossings) = solve startPosition input

printfn "Starting position: %d" startPosition
printfn "Final dial position: %d" finalPos
printfn "Times dial pointed at 0: %d" zeroCrossings
printfn "Times stopped at 0 during turns: %d" stopZero