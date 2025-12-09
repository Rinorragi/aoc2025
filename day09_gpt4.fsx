// Day 9: Red Pyroclastic Flow - GPT4 Agent
// Phase 1: Find largest rectangle with red tiles at two opposite corners
// Phase 2: Same but all tiles in rectangle must be red or green (following polygon rules)

open System
open System.IO
open System.Diagnostics

// Parse input to list of red tile coordinates
let parseInput (lines: string[]) =
    lines
    |> Array.filter (fun line -> not (String.IsNullOrWhiteSpace(line)))
    |> Array.map (fun line ->
        let parts = line.Split(',')
        (int parts.[0], int parts.[1]))
    |> Array.toList

// Phase 1: Find largest rectangle with red tiles at opposite corners (any tiles inside allowed)
let phase1 (redTiles: (int*int) list) =
    redTiles
    |> List.indexed
    |> List.collect (fun (i, (x1, y1)) ->
        redTiles
        |> List.skip (i + 1)
        |> List.choose (fun (x2, y2) ->
            // Must form a proper rectangle (different x AND y)
            if x1 <> x2 && y1 <> y2 then
                let area = int64(abs(x2-x1) + 1) * int64(abs(y2-y1) + 1)
                Some area
            else
                None))
    |> function
        | [] -> 0L
        | areas -> List.max areas

// Build boundary green tiles: straight lines between consecutive red tiles in the list
let buildBoundaryGreen (polygon: (int*int) list) =
    polygon
    |> List.indexed
    |> List.collect (fun (i, (x1, y1)) ->
        let (x2, y2) = polygon.[(i + 1) % polygon.Length]
        if x1 = x2 then
            // Vertical line
            [min y1 y2 .. max y1 y2] |> List.map (fun y -> (x1, y))
        else
            // Horizontal line
            [min x1 x2 .. max x1 x2] |> List.map (fun x -> (x, y1)))
    |> Set.ofList

// Point-in-polygon test using ray casting algorithm
let isInsidePolygon (polygon: (int*int) array) (x, y) =
    let mutable inside = false
    let mutable j = polygon.Length - 1

    for i in 0 .. polygon.Length - 1 do
        let (xi, yi) = polygon.[i]
        let (xj, yj) = polygon.[j]

        if ((yi > y) <> (yj > y)) && (x < (xj - xi) * (y - yi) / (yj - yi) + xi) then
            inside <- not inside
        j <- i

    inside

// Convex hull using Graham scan
let convexHull (points: (int*int) list) =
    let sorted = points |> List.sortBy (fun (x, y) -> (x, y))
    
    let cross (ox, oy) (ax, ay) (bx, by) =
        (ax - ox) * (by - oy) - (ay - oy) * (bx - ox)
    
    let rec buildHalf points hull =
        match points with
        | [] -> hull
        | p :: rest ->
            let rec removeNonLeft hull =
                match hull with
                | a :: b :: tail when cross b a p <= 0 -> removeNonLeft (b :: tail)
                | _ -> hull
            buildHalf rest (p :: removeNonLeft hull)
    
    let lower = buildHalf sorted []
    let upper = buildHalf (List.rev sorted) []
    List.distinct (lower @ upper)

// Fast sampled perimeter validation with memoization
let isRectangleValid (x1, y1) (x2, y2) (redSet: Set<int*int>) (boundaryGreen: Set<int*int>) (polygon: (int*int) array) =  
    let xMin, xMax = min x1 x2, max x1 x2
    let yMin, yMax = min y1 y2, max y1 y2
    let width = xMax - xMin + 1
    let height = yMax - yMin + 1

    // Memoization for expensive polygon checks
    let memoized = System.Collections.Generic.Dictionary<int*int, bool>()

    let isRedOrGreen (x, y) =
        if redSet.Contains(x, y) || boundaryGreen.Contains(x, y) then
            true
        else
            match memoized.TryGetValue((x, y)) with
            | true, result -> result
            | false, _ ->
                let result = isInsidePolygon polygon (x, y)
                memoized.[(x, y)] <- result
                result

    // FULL perimeter validation with early termination (List.forall stops on first false)
    // Check top and bottom edges completely
    let topBottomValid =
        [xMin .. xMax]
        |> List.forall (fun x -> isRedOrGreen(x, yMin) && isRedOrGreen(x, yMax))

    if not topBottomValid then false
    else
        // Check left and right edges completely (skip corners already checked)
        [yMin + 1 .. yMax - 1]
        |> List.forall (fun y -> isRedOrGreen(xMin, y) && isRedOrGreen(xMax, y))// Phase 2: All pairs with early exit optimization
let phase2 (redTiles: (int*int) list) (debug: bool) =
    let polygon = redTiles |> List.toArray
    let boundaryGreen = buildBoundaryGreen redTiles
    let redSet = redTiles |> Set.ofList
    
    // Pre-compute bounding box for quick rejection
    let (minBoundX, maxBoundX, minBoundY, maxBoundY) =
        let xs = redTiles |> List.map fst
        let ys = redTiles |> List.map snd
        (List.min xs, List.max xs, List.min ys, List.max ys)
    
    // Fast pre-validation before expensive perimeter check
    let quickReject (x1, y1) (x2, y2) =
        // Check if all four corners are red/green first (cheapest check)
        let corners = [(x1, y1); (x2, y2); (x1, y2); (x2, y1)]
        not (corners |> List.forall (fun (x, y) ->
            redSet.Contains(x, y) ||
            boundaryGreen.Contains(x, y) ||
            isInsidePolygon polygon (x, y)))
    
    if debug then
        printfn "Total red tiles: %d" redTiles.Length
        printfn "Boundary green tiles: %d" boundaryGreen.Count
        printfn "Bounding box: X[%d..%d] Y[%d..%d]" minBoundX maxBoundX minBoundY maxBoundY    // Generate ALL candidates sorted by area (don't use convex hull - may miss valid rectangles)
    let candidates =
        redTiles
        |> List.indexed
        |> List.collect (fun (i, (x1, y1)) ->
            redTiles
            |> List.skip (i + 1)
            |> List.choose (fun (x2, y2) ->
                if x1 <> x2 && y1 <> y2 then
                    let area = int64(abs(x2-x1) + 1) * int64(abs(y2-y1) + 1)
                    Some (area, x1, y1, x2, y2)
                else None))
        |> List.sortByDescending (fun (area, _, _, _, _) -> area)
    
    if debug then
        printfn "Total candidates: %d" candidates.Length
        printfn "Top 10 candidates by area:"
        candidates 
        |> List.take (min 10 candidates.Length)
        |> List.iter (fun (area, x1, y1, x2, y2) ->
            printfn "  (%d,%d) to (%d,%d) = %d" x1 y1 x2 y2 area)
    
    let mutable tested = 0
    let mutable rejected = 0

    if debug then
        printfn "Testing ALL %d candidates..." candidates.Length

    let result =
        candidates
        |> List.indexed
        |> List.tryFind (fun (idx, (_, x1, y1, x2, y2)) ->
            if quickReject (x1, y1) (x2, y2) then
                rejected <- rejected + 1
                false
            else
                tested <- tested + 1
                let valid = isRectangleValid (x1, y1) (x2, y2) redSet boundaryGreen polygon
                if valid && debug then
                    printfn "  FOUND VALID: (%d,%d) to (%d,%d) after testing %d, rejecting %d" x1 y1 x2 y2 tested rejected
                valid)
        |> Option.map snd

    printfn "Quick rejected: %d, Full tested: %d, Found: %b" rejected tested result.IsSome
    
    result
    |> Option.map (fun (area, _, _, _, _) -> area)
    |> Option.defaultValue 0L

// Main execution
let solve (inputPath: string) (debug: bool) =
    let lines = File.ReadAllLines(inputPath)
    let redTiles = parseInput lines

    let sw1 = Stopwatch.StartNew()
    let result1 = phase1 redTiles
    sw1.Stop()

    let sw2 = Stopwatch.StartNew()
    let result2 = phase2 redTiles debug
    sw2.Stop()

    printfn "Phase 1: %d" result1
    printfn "Phase 2: %d" result2
    printfn "Phase 1 Time: %dms" sw1.ElapsedMilliseconds
    printfn "Phase 2 Time: %dms" sw2.ElapsedMilliseconds

// Command-line argument support
let args = Environment.GetCommandLineArgs() |> Array.skip 1

match args with
| [| "example" |] ->
    printfn "=== Example Data ==="
    solve "input/input09_example.txt" true
| [| "real" |] ->
    printfn "=== Real Data ==="
    solve "input/input09.txt" false
| _ ->
    printfn "=== Example Data ==="
    solve "input/input09_example.txt" false
    printfn ""
    printfn "=== Real Data ==="
    solve "input/input09.txt" false