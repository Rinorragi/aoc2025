open System
open System.Diagnostics

let parseInput (lines: string[]) =
    let opLine = lines.[lines.Length - 1]
    let dataLines = lines |> Array.take (lines.Length - 1)
    let isColSpace idx =
        dataLines |> Array.forall (fun line -> idx >= line.Length || line.[idx] = ' ')
    let rec findRanges idx acc =
        if idx >= opLine.Length then List.rev acc
        elif opLine.[idx] = ' ' && isColSpace idx then findRanges (idx+1) acc
        elif opLine.[idx] <> ' ' then
            let start = idx
            let rec findEnd j =
                if j < opLine.Length && (opLine.[j] <> ' ' || not (isColSpace j)) then findEnd (j+1) else j
            let endPos = findEnd idx
            findRanges endPos ((start, endPos-1)::acc)
        else findRanges (idx+1) acc
    let colRanges = findRanges 0 []
    colRanges, dataLines, opLine

// For Phase 2, read each character column individually, bottom-to-top, to form numbers
let solvePhase2 (lines: string[]) =
    let colRanges, dataLines, opLine = parseInput lines
    let reversedColRanges = List.rev colRanges
    let reversedLines = Array.rev dataLines
    
    reversedColRanges
    |> List.map (fun (start, endPos) ->
        let op = opLine.[start]
        printfn "  Problem with op '%c', cols %d-%d:" op start endPos
        // Each character column forms one number
        let colNums =
            [start..endPos]
            |> List.choose (fun colIdx ->
                let digits =
                    reversedLines
                    |> Array.choose (fun line ->
                        if colIdx < line.Length then
                            let c = line.[colIdx]
                            if c = ' ' then None else Some (string c)
                        else None)
                if Array.length digits > 0 then
                    let numStr = String.concat "" digits |> fun s -> String(s.ToCharArray() |> Array.rev)  // Reverse the string!
                    let num = bigint.Parse numStr
                    printfn "    Col %d: [%A] = %s = %A" colIdx (String.concat "," digits) numStr num
                    Some num
                else
                    printfn "    Col %d: (empty)" colIdx
                    None)
        
        let result = 
            match op with
            | '*' -> List.fold ( * ) 1I colNums
            | '+' -> List.fold ( + ) 0I colNums
            | _ -> 0I
        printfn "    Result: %A" result
        result)
    |> List.sum

let exampleLines = [| "123 328  51 64 "; " 45 64  387 23 "; "  6 98  215 314"; "*   +   *   +  " |]

let result = solvePhase2 exampleLines
printfn "Phase 2 result: %A (expected 3263827)" result
