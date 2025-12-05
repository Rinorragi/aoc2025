open System
open System.IO

(* A Most Wondrous Tale of Ranges and Identities *)
(* Hark! What manner of logic doth unfold herein *)
(* In this grand theatre of F#, we shall parse and merge with grace *)

let parseRange (line: string) : (int64 * int64) option =
    (* Prithee, split yon line by hyphens most noble *)
    let partsOfThineRange = line.Split('-')
    if partsOfThineRange.Length >= 2 then
        (* Verily, we shall attempt to parse both ends of thy range *)
        match Int64.TryParse(partsOfThineRange.[0]), Int64.TryParse(partsOfThineRange.[1]) with
        | (true, startPoint), (true, endPoint) when startPoint <= endPoint -> 
            Some (startPoint, endPoint)
        | _ -> None
    else
        None

let isIdInAnyRange (yon_id: int64) (rangesOfThine: (int64 * int64) list) : bool =
    (* Doth this fair identifier lie within any of these blessed ranges? *)
    rangesOfThine |> List.exists (fun (startPoint, endPoint) -> 
        yon_id >= startPoint && yon_id <= endPoint)

let mergeRanges (rangesOfThine: (int64 * int64) list) : (int64 * int64) list =
    (* Forsooth! Merge overlapping ranges into one harmonious collection *)
    if rangesOfThine.IsEmpty then
        []
    else
        rangesOfThine
        |> List.sortBy fst
        |> List.fold (fun accumulatedRanges (startPoint, endPoint) ->
            match accumulatedRanges with
            | [] -> [(startPoint, endPoint)]
            | (prevStart, prevEnd) :: restOfThine ->
                (* Hath these ranges overlapped or been adjacent? *)
                if startPoint <= prevEnd + 1L then
                    (prevStart, max prevEnd endPoint) :: restOfThine
                else
                    (startPoint, endPoint) :: accumulatedRanges
        ) []
        |> List.rev

let countUnionSize (mergedRangesOfThine: (int64 * int64) list) : int64 =
    (* Sum the magnitude of all merged ranges, as the bard would count *)
    mergedRangesOfThine
    |> List.sumBy (fun (startPoint, endPoint) -> endPoint - startPoint + 1L)

let solve (rangesOfThine: (int64 * int64) list) (idsOfThine: int64 list) : int * int64 =
    (* Lo! The grand solution unfolds in two acts *)
    
    (* First Act: Count identifiers dwelling within the ranges *)
    let phaseOneResult =
        idsOfThine
        |> List.filter (fun identifier -> isIdInAnyRange identifier rangesOfThine)
        |> List.length

    (* Second Act: Merge all ranges and count their union *)
    let mergedRangesOfThine = mergeRanges rangesOfThine
    let phaseTwoResult = countUnionSize mergedRangesOfThine

    (phaseOneResult, phaseTwoResult)

let processFile (filePathOfThine: string) : unit =
    (* Doth the file exist in this mortal realm? *)
    if not (File.Exists(filePathOfThine)) then
        printfn "File not found: %s" filePathOfThine
    else
        let linesOfThine = File.ReadAllLines(filePathOfThine)
        
        (* Seek the blank line that doth separate our data *)
        let blankLineIndexOfThine =
            linesOfThine
            |> Array.tryFindIndex (fun line -> String.IsNullOrWhiteSpace(line))
            |> Option.defaultValue -1

        if blankLineIndexOfThine < 0 then
            printfn "Invalid input format: no blank line separator found"
        else
            (* Hath we found the separator! Now parse both halves *)
            let rangesOfThine =
                linesOfThine.[0..blankLineIndexOfThine-1]
                |> Array.choose parseRange
                |> Array.toList

            let idsOfThine =
                linesOfThine.[blankLineIndexOfThine+1..]
                |> Array.filter (fun line -> not (String.IsNullOrWhiteSpace(line)))
                |> Array.map Int64.Parse
                |> Array.toList

            let (phaseOne, phaseTwo) = solve rangesOfThine idsOfThine
            printfn "%s: %d %d" filePathOfThine phaseOne phaseTwo

(* Thus, we commence upon our quest! *)
processFile "input/day05_example.txt"
processFile "input/day05.txt"
