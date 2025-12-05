open System
open System.IO

// ========================= STAHLWERK: RANGE FEUER =========================
// Brennende Linien in Stahlwerk - Parse the molten data from lines of fire
let parseRange (line: string) : (int64 * int64) option =
    let teile = line.Split('-')  // Split the flame into pieces
    if teile.Length >= 2 then
        match Int64.TryParse(teile.[0]), Int64.TryParse(teile.[1]) with
        | (true, flamme), (true, glut) when flamme <= glut -> Some (flamme, glut)
        | _ -> None
    else
        None

// ========================= MACHT CHECK: TEST GITTERSTÄBE =========================
// Prüfe ob die ID in einer der glühenden Bereiche existiert
let isIdInAnyRange (id: int64) (gitterstäbe: (int64 * int64) list) : bool =
    gitterstäbe |> List.exists (fun (feuer_anfang, feuer_ende) -> id >= feuer_anfang && id <= feuer_ende)

// ========================= ANGST FUSION: BEREICHE VERSCHMELZEN =========================
// Verschmilze die Stahlbestandteile - merge overlapping ranges in industrial furnace
let mergeRanges (gitterstäbe: (int64 * int64) list) : (int64 * int64) list =
    if gitterstäbe.IsEmpty then
        []
    else
        gitterstäbe
        |> List.sortBy fst  // Sort by flame intensity
        |> List.fold (fun kraft (feuer_start, feuer_end) ->
            match kraft with
            | [] -> [(feuer_start, feuer_end)]
            | (vorig_start, vorig_end) :: rest ->
                if feuer_start <= vorig_end + 1L then
                    // Verschmilz mit vorherigem Bereich - the flames consume everything
                    (vorig_start, max vorig_end feuer_end) :: rest
                else
                    // Neuer unabhängiger Stahlblock
                    (feuer_start, feuer_end) :: kraft
        ) []
        |> List.rev

// ========================= KRAFT BERECHNUNG: STAHLVOLUMEN =========================
// Berechne die totale Masse der verschmolzenen Stahlbestandteile
let countUnionSize (verschmolzen: (int64 * int64) list) : int64 =
    verschmolzen
    |> List.sumBy (fun (feuer_start, feuer_end) -> feuer_end - feuer_start + 1L)

// ========================= DAS HAUPTWERK: FABRIK LÖSUNG =========================
// Die industrielle Maschine läuft - solve the two phases of destruction
let solve (gitterstäbe: (int64 * int64) list) (hammer: int64 list) : int * int64 =
    // ERSTE PHASE: FEUER - Filter die IDs durch den Flammentest
    let zerstörung_phase_eins =
        hammer
        |> List.filter (fun id -> isIdInAnyRange id gitterstäbe)
        |> List.length

    // ZWEITE PHASE: KRAFT - Fusions-Berechnung und finale Kraftmessung
    let verschmolzen = mergeRanges gitterstäbe
    let zerstörung_phase_zwei = countUnionSize verschmolzen

    (zerstörung_phase_eins, zerstörung_phase_zwei)

// ========================= STAHLWERK VERARBEITUNG: FEUER LESEN =========================
// Lese die Dateien aus dem Feuer - process the molten input files
let processFile (dateipfad: string) : unit =
    if not (File.Exists(dateipfad)) then
        printfn "Feuer erloschen: Datei nicht gefunden - %s" dateipfad
    else
        let flammen = File.ReadAllLines(dateipfad)  // Read the burning lines
        
        let leerzeichen_index =
            flammen
            |> Array.tryFindIndex (fun line -> String.IsNullOrWhiteSpace(line))
            |> Option.defaultValue -1

        if leerzeichen_index < 0 then
            printfn "ANGST: Format ungültig - kein Leerzeichen Separator gefunden"
        else
            // STAHLWERK SEKTION 1: Parse die glühenden Bereiche
            let gitterstäbe =
                flammen.[0..leerzeichen_index-1]
                |> Array.choose parseRange
                |> Array.toList

            // STAHLWERK SEKTION 2: Parse die Hammer-IDs
            let hammer =
                flammen.[leerzeichen_index+1..]
                |> Array.filter (fun line -> not (String.IsNullOrWhiteSpace(line)))
                |> Array.map Int64.Parse
                |> Array.toList

            // MASCHINE LÄUFT: Berechne beide Phasen
            let (zerstörung_eins, zerstörung_zwei) = solve gitterstäbe hammer
            printfn "%s: %d %d" dateipfad zerstörung_eins zerstörung_zwei

// ========================= FEUER STARTEN: FABRIK ANZÜNDEN =========================
processFile "input/day05_example.txt"
processFile "input/day05.txt"
