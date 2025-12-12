// ============================================
// DAY 12: PRESENT PACKING PUZZLE
// Algorithm: Greedy Tiling with Feasibility Checks
// ============================================

open System
open System.IO
open System.Diagnostics

// ──────── DATA TYPES ────────
type Coord = int * int

type Shape = {
    Index: int
    Coords: Coord list
}

type Transformation = {
    ShapeIndex: int
    Coords: Coord list
}

type Region = {
    Width: int
    Height: int
    Grid: bool[,]
    Requirements: (int * int) list  // (shapeIndex, count)
}

// ──────── PARSING ────────
let parseShapes (lines: string list) : Shape list =
    let rec parse acc currentIndex currentCoords currentRow lineIdx =
        if lineIdx >= List.length lines then
            if currentCoords <> [] then
                { Index = currentIndex; Coords = List.rev currentCoords } :: acc |> List.rev
            else
                List.rev acc
        else
            let line = lines.[lineIdx]
            if line.Contains(":") && not (line.Contains("x")) then
                // New shape starting (e.g., "0:")
                let newAcc = 
                    if currentCoords <> [] then
                        { Index = currentIndex; Coords = List.rev currentCoords } :: acc
                    else
                        acc
                let idx = int (line.Split(':').[0])
                parse newAcc idx [] 0 (lineIdx + 1)
            elif line.Trim() = "" || line.Contains("x") then
                // Empty line or region line - skip
                parse acc currentIndex currentCoords currentRow (lineIdx + 1)
            else
                // Parse shape row
                let newCoords = 
                    line 
                    |> Seq.mapi (fun col c -> if c = '#' then Some (col, currentRow) else None)
                    |> Seq.choose id
                    |> Seq.toList
                    |> List.append currentCoords
                parse acc currentIndex newCoords (currentRow + 1) (lineIdx + 1)
    
    parse [] -1 [] 0 0

let parseRegions (lines: string list) : Region list =
    lines
    |> List.filter (fun line -> line.Contains("x") && line.Contains(":"))
    |> List.map (fun line ->
        let parts = line.Split(':')
        let dims = parts.[0].Split('x')
        let width = int dims.[0]
        let height = int dims.[1]
        let counts = 
            parts.[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
            |> Array.mapi (fun i count -> (i, int count))
            |> Array.filter (fun (_, c) -> c > 0)
            |> Array.toList
        {
            Width = width
            Height = height
            Grid = Array2D.create width height false
            Requirements = counts
        })

// ──────── TRANSFORMATIONS ────────
let rotate90 (coords: Coord list) : Coord list =
    coords |> List.map (fun (x, y) -> (-y, x))

let flipHorizontal (coords: Coord list) : Coord list =
    coords |> List.map (fun (x, y) -> (-x, y))

let normalize (coords: Coord list) : Coord list =
    if coords = [] then []
    else
        let minX = coords |> List.map fst |> List.min
        let minY = coords |> List.map snd |> List.min
        coords |> List.map (fun (x, y) -> (x - minX, y - minY))

let generateTransformations (shape: Shape) : Transformation list =
    let baseCoords = normalize shape.Coords
    
    let allTransforms = [
        baseCoords
        rotate90 baseCoords
        rotate90 (rotate90 baseCoords)
        rotate90 (rotate90 (rotate90 baseCoords))
        flipHorizontal baseCoords
        rotate90 (flipHorizontal baseCoords)
        rotate90 (rotate90 (flipHorizontal baseCoords))
        rotate90 (rotate90 (rotate90 (flipHorizontal baseCoords)))
    ]
    
    allTransforms
    |> List.map normalize
    |> List.distinct
    |> List.map (fun coords -> { ShapeIndex = shape.Index; Coords = coords })

// ──────── PLACEMENT LOGIC ────────
let canPlace (grid: bool[,]) (trans: Transformation) (x: int) (y: int) : bool =
    let width = Array2D.length1 grid
    let height = Array2D.length2 grid
    trans.Coords
    |> List.forall (fun (dx, dy) ->
        let nx, ny = x + dx, y + dy
        nx >= 0 && nx < width && ny >= 0 && ny < height && not grid.[nx, ny])

let placePresent (grid: bool[,]) (trans: Transformation) (x: int) (y: int) : unit =
    trans.Coords |> List.iter (fun (dx, dy) -> grid.[x + dx, y + dy] <- true)

let removePresent (grid: bool[,]) (trans: Transformation) (x: int) (y: int) : unit =
    trans.Coords |> List.iter (fun (dx, dy) -> grid.[x + dx, y + dy] <- false)

// ──────── LIMITED BACKTRACKING SOLVER ────────
let solve (region: Region) (transformations: Map<int, Transformation list>) : bool =
    let totalCellsNeeded = 
        region.Requirements 
        |> List.sumBy (fun (shapeIdx, count) -> 
            let shapeCells = transformations.[shapeIdx].[0].Coords.Length
            shapeCells * count)
    
    if totalCellsNeeded > region.Width * region.Height then
        false
    else
        let presents = 
            region.Requirements 
            |> List.collect (fun (shapeIdx, count) -> List.replicate count shapeIdx)
        
        let mutable totalAttempts = 0
        let maxAttempts = 100000  // Limit total attempts per region
        
        let rec tryPlace (remaining: int list) : bool =
            totalAttempts <- totalAttempts + 1
            if totalAttempts > maxAttempts then 
                false  // Too many attempts - assume unsolvable
            else
                match remaining with
                | [] -> true
                | shapeIdx :: rest ->
                    let trans = transformations.[shapeIdx]
                    let mutable placed = false
                    
                    for t in trans do
                        if not placed then
                            for x in 0 .. region.Width - 1 do
                                if not placed then
                                    for y in 0 .. region.Height - 1 do
                                        if not placed && canPlace region.Grid t x y then
                                            placePresent region.Grid t x y
                                            if tryPlace rest then
                                                placed <- true
                                            else
                                                removePresent region.Grid t x y
                    placed
        
        tryPlace presents

// ──────── MAIN EXECUTION ────────
let args = fsi.CommandLineArgs |> Array.skip 1
let useExample = args |> Array.contains "--example"

let inputFile = 
    if useExample then "input/input12_example.txt"
    else "input/input12.txt"

let lines = File.ReadAllLines(inputFile) |> Array.toList

printfn "🎄 Advent of Code 2025 - Day 12: Present Packing 🎁"
printfn "═══════════════════════════════════════════════════"

let shapes = parseShapes lines
let regions = parseRegions lines

printfn "📦 Parsed %d unique present shapes" shapes.Length
printfn "🌲 Testing %d regions for feasibility..." regions.Length

// Precompute all transformations
let transformationCache = 
    shapes 
    |> List.map (fun s -> (s.Index, generateTransformations s))
    |> Map.ofList

let sw = Stopwatch.StartNew()
let mutable solvableCount = 0
let mutable regionIdx = 0

for region in regions do
    regionIdx <- regionIdx + 1
    
    if regionIdx <= 10 || useExample then
        printfn "\n─────────────────────────────────────────────────"
        printfn "Region %d: %dx%d grid (%d presents required)" 
            regionIdx region.Width region.Height 
            (region.Requirements |> List.sumBy snd)
        
        let reqStr = 
            region.Requirements 
            |> List.map (fun (idx, cnt) -> sprintf "%dx Shape#%d" cnt idx)
            |> String.concat ", "
        printfn "Required: %s" reqStr
    
    let regionSw = Stopwatch.StartNew()
    let canFit = solve region transformationCache
    regionSw.Stop()
    
    if canFit then
        solvableCount <- solvableCount + 1
        if regionIdx <= 10 || useExample then
            printfn "Solving... ✓ SOLVABLE (%dms)" regionSw.ElapsedMilliseconds
    else
        if regionIdx <= 10 || useExample then
            printfn "Solving... ✗ UNSOLVABLE (%dms)" regionSw.ElapsedMilliseconds

sw.Stop()

printfn "\n═══════════════════════════════════════════════════"
printfn "📊 RESULTS SUMMARY"
printfn "═══════════════════════════════════════════════════"
printfn "Total Regions:    %d" regions.Length
printfn "Solvable:         %d  ✓" solvableCount
printfn "Unsolvable:       %d  ✗" (regions.Length - solvableCount)
printfn "Total Time:       %dms" sw.ElapsedMilliseconds
printfn ""
printfn "🎯 ANSWER: %d regions can fit all presents" solvableCount
printfn "═══════════════════════════════════════════════════"
