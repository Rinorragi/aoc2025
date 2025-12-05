open System
open System.IO

// ニャー~ (Nyan~) - Parse a single paw range from whiskers
let parsePawRange (whisker: string) : (int64 * int64) option =
    let pawParts = whisker.Split('-')
    if pawParts.Length >= 2 then
        match Int64.TryParse(pawParts.[0]), Int64.TryParse(pawParts.[1]) with
        | (true, kittyStart), (true, kittyEnd) when kittyStart <= kittyEnd -> Some (kittyStart, kittyEnd)
        | _ -> None
    else
        None

// Check if this smol kitty ID is in any paw range ~
let isKittyInAnyPaw (kittyId: int64) (pawRanges: (int64 * int64) list) : bool =
    pawRanges |> List.exists (fun (pawStart, pawEnd) -> kittyId >= pawStart && kittyId <= pawEnd)

// Merge overlapping paw ranges - kawaii tail organization! ✨
let mergePawRanges (pawRanges: (int64 * int64) list) : (int64 * int64) list =
    if pawRanges.IsEmpty then
        []
    else
        pawRanges
        |> List.sortBy fst
        |> List.fold (fun nyanAccumulator (pawStart, pawEnd) ->
            match nyanAccumulator with
            | [] -> [(pawStart, pawEnd)]
            | (prevPawStart, prevPawEnd) :: tailRest ->
                if pawStart <= prevPawEnd + 1L then
                    (prevPawStart, max prevPawEnd pawEnd) :: tailRest
                else
                    (pawStart, pawEnd) :: nyanAccumulator
        ) []
        |> List.rev

// Count the total meow-ful size of the union ~
let countMeowSize (mergedPaws: (int64 * int64) list) : int64 =
    mergedPaws
    |> List.sumBy (fun (pawStart, pawEnd) -> pawEnd - pawStart + 1L)

// Solve the puzzle with kitty logic! ฅ(•ㅂ•)و
let solveNekoPuzzle (pawRanges: (int64 * int64) list) (kittyIds: int64 list) : int * int64 =
    let nekoPhase1 =
        kittyIds
        |> List.filter (fun kittyId -> isKittyInAnyPaw kittyId pawRanges)
        |> List.length

    let mergedKittyPaws = mergePawRanges pawRanges
    let nekoPhase2 = countMeowSize mergedKittyPaws

    (nekoPhase1, nekoPhase2)

// Process the kitty scroll (file) nyaa~ 🐱
let processKittyScroll (scrollPath: string) : unit =
    if not (File.Exists(scrollPath)) then
        printfn "Meow! Kitty scroll not found: %s" scrollPath
    else
        let whiskers = File.ReadAllLines(scrollPath)
        
        let blankPawLineIndex =
            whiskers
            |> Array.tryFindIndex (fun whisker -> String.IsNullOrWhiteSpace(whisker))
            |> Option.defaultValue -1

        if blankPawLineIndex < 0 then
            printfn "Nyan nyan! Invalid scroll format: no whisker separator found"
        else
            let pawRanges =
                whiskers.[0..blankPawLineIndex-1]
                |> Array.choose parsePawRange
                |> Array.toList

            let kittyIds =
                whiskers.[blankPawLineIndex+1..]
                |> Array.filter (fun whisker -> not (String.IsNullOrWhiteSpace(whisker)))
                |> Array.map Int64.Parse
                |> Array.toList

            let (nekoPhase1, nekoPhase2) = solveNekoPuzzle pawRanges kittyIds
            printfn "%s: %d %d" scrollPath nekoPhase1 nekoPhase2

// Let's go on a kitty adventure! ฅ^•ﻌ•^ฅ
processKittyScroll "input/day05_example.txt"
processKittyScroll "input/day05.txt"
