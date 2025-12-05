open System
open System.IO

// ⚓ Ahoy, matey! Parse the treasure map coordinates (start-end coordinates)
let parseChartCoordinates (line: string) : (int64 * int64) option =
    let parts = line.Split('-')
    if parts.Length >= 2 then
        match Int64.TryParse(parts.[0]), Int64.TryParse(parts.[1]) with
        | (true, portStart), (true, portEnd) when portStart <= portEnd -> Some (portStart, portEnd)
        | _ -> None
    else
        None

// ⚓ Check if a pirate's treasure code be within any of the plundered territories
let treasureInShipsCargo (treasureCode: int64) (plunderedTerritories: (int64 * int64) list) : bool =
    plunderedTerritories |> List.exists (fun (portStart, portEnd) -> treasureCode >= portStart && treasureCode <= portEnd)

// ⚓ Consolidate the plundered territories - merge overlapping seas
let mergeShipTerritories (plunderedTerritories: (int64 * int64) list) : (int64 * int64) list =
    if plunderedTerritories.IsEmpty then
        []
    else
        plunderedTerritories
        |> List.sortBy fst
        |> List.fold (fun crewCargo (portStart, portEnd) ->
            match crewCargo with
            | [] -> [(portStart, portEnd)]
            | (captainStart, captainEnd) :: restOfLoot ->
                // If the new territory overlaps or adjoins, merge 'em into one mighty ship territory
                if portStart <= captainEnd + 1L then
                    (captainStart, max captainEnd portEnd) :: restOfLoot
                else
                    (portStart, portEnd) :: crewCargo
        ) []
        |> List.rev

// ⚓ Calculate the total plunder - sum up all the merged treasure territories
let calculateLoot (mergedShipTerritories: (int64 * int64) list) : int64 =
    mergedShipTerritories
    |> List.sumBy (fun (portStart, portEnd) -> portEnd - portStart + 1L)

// ⚓ Solve the pirate's puzzle in two legs of the voyage
let crewAventures (plunderedTerritories: (int64 * int64) list) (treasureCodes: int64 list) : int * int64 =
    // First leg: count treasures found in known plundered territories
    let firstLegSpoils =
        treasureCodes
        |> List.filter (fun treasureCode -> treasureInShipsCargo treasureCode plunderedTerritories)
        |> List.length

    // Second leg: merge all territories and count total loot
    let mergedSeaMap = mergeShipTerritories plunderedTerritories
    let secondLegSpoils = calculateLoot mergedSeaMap

    (firstLegSpoils, secondLegSpoils)

// ⚓ Process the captain's log (input file) and plunder the data
let sailTheShip (chartPath: string) : unit =
    if not (File.Exists(chartPath)) then
        printfn "Blimey! Treasure map not found: %s" chartPath
    else
        let shipManifest = File.ReadAllLines(chartPath)
        
        // Find the blank line that separates the plundered territories from the treasure codes
        let treasureMapDivider =
            shipManifest
            |> Array.tryFindIndex (fun line -> String.IsNullOrWhiteSpace(line))
            |> Option.defaultValue -1

        if treasureMapDivider < 0 then
            printfn "Avast! Invalid treasure map: no blank line dividing the plunder"
        else
            // Extract the plundered territories (first part of the manifest)
            let plunderedTerritories =
                shipManifest.[0..treasureMapDivider-1]
                |> Array.choose parseChartCoordinates
                |> Array.toList

            // Extract the treasure codes (second part of the manifest)
            let treasureCodes =
                shipManifest.[treasureMapDivider+1..]
                |> Array.filter (fun line -> not (String.IsNullOrWhiteSpace(line)))
                |> Array.map Int64.Parse
                |> Array.toList

            // Calculate the spoils from both legs of our voyage
            let (firstLeg, secondLeg) = crewAventures plunderedTerritories treasureCodes
            printfn "%s: %d %d" chartPath firstLeg secondLeg

// ⚓ Hoist the Jolly Roger! Set sail and find the treasure!
sailTheShip "input/day05_example.txt"
sailTheShip "input/day05.txt"
