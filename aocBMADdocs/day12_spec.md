# Day 12 Architecture & UX Specification
**BMAD Enterprise Development - Advent of Code 2025**

---

## 🏗️ ARCHITECT'S ANALYSIS

### Problem Classification
**Type:** ⚠️ **REVISED** - Polyomino Tiling / Exact Cover Problem  
**Complexity:** NP-Complete BUT with massive scale (1000 regions, up to 386 presents)  
**Input:** 6 unique present shapes (polyominoes) + 1000 region requirements  
**Output:** Count of regions that can fit all required presents

**CRITICAL CONSTRAINTS:**
- 1,000 regions to test (not 3!)
- Up to 386 presents per region
- Grids up to 50×50 (2,500 cells)
- Each shape ~3-6 cells
- **Pure backtracking = INFEASIBLE**

### Core Algorithm Strategy

#### **REVISED Approach: Dancing Links (DLX) / Greedy Tiling with Early Termination**

1. **Parse Input**
   - Extract shape definitions (convert `#` to coordinates)
   - Parse region requirements (width × height + shape counts)
   
2. **Generate Shape Transformations**
   - For each shape, generate all 8 transformations:
     - 4 rotations (0°, 90°, 180°, 270°)
     - 2 flips (horizontal, vertical) × 4 rotations
   - Normalize to (0,0) origin
   - Remove duplicates (some shapes are symmetric)

3. **Fast Feasibility Check**
   ```
   function canFitPresents(region, presents):
       // Quick impossible check
       totalCellsNeeded = sum(present.shape.cells for present in presents)
       if totalCellsNeeded > region.width * region.height:
           return false  // Mathematically impossible
       
       // Try greedy placement with backtracking limit
       return greedyPlaceWithTimeout(region, presents, maxAttempts=10000)
   ```

4. **Greedy Placement Strategy**
   - **Pre-compute transformations:** Cache all 8 transformations per shape
   - **Greedy ordering:** Place largest shapes first (fewer options = faster)
   - **First-fit placement:** Try each transformation at first available position
   - **Early termination:** If no progress after N attempts, return false
   - **Timeout protection:** Limit total placement attempts per region
   
5. **Optimization Heuristics**
   - **Cell count pruning:** Reject if total cells needed > grid cells
   - **Density check:** If presents need >90% of grid, likely unsolvable
   - **Fast transformations:** Precompute all transformations at startup
   - **Bitmap grid:** Use bool[,] for O(1) collision detection

### Data Structures

```fsharp
type Coord = int * int

type Shape = {
    Index: int
    Coords: Coord list  // Relative coordinates where '#' appears
}

type Transformation = {
    OriginalShape: int
    Coords: Coord list  // Transformed coordinates
}

type Region = {
    Width: int
    Height: int
    Grid: bool[,]  // true = occupied
    Requirements: Map<int, int>  // shape index -> count
}

type Present = {
    Shape: int
    Id: char  // For visualization (A, B, C, ...)
}
```

### Key Functions

1. **`parseShapes: string[] -> Shape list`**
2. **`generateTransformations: Shape -> Transformation list`**
3. **`parseRegions: string[] -> Region list`**
4. **`canPlace: Region -> Transformation -> int -> int -> bool`**
5. **`placePresent: Region -> Transformation -> int -> int -> char -> unit`**
6. **`removePresent: Region -> Transformation -> int -> int -> unit`**
7. **`solve: Region -> Present list -> bool`**

### Complexity Analysis
- **Time:** O(1000 regions × attempts per region)
  - Worst case per region: O(n × 8 × w × h) with early termination
  - With 386 presents: Need aggressive pruning and timeouts
- **Space:** O(w × h) per grid + O(n × 8) for transformation cache
- **Target Performance:** < 30 seconds total for 1000 regions
- **Strategy:** Many regions will fail quickly (impossible cell counts), focus optimization on feasible candidates

---

## 🎨 UX DESIGNER'S SPECIFICATION

### CLI Interface Design

#### **Visual Requirements**
1. **Progress Indication** - Show which region is being tested
2. **Success/Failure Feedback** - Clear indication of solvable regions
3. **Solution Visualization** - ASCII art showing present placement
4. **Timing Statistics** - Performance metrics per region
5. **Summary Table** - Final results overview

#### **Output Format**

```
🎄 Advent of Code 2025 - Day 12: Present Packing 🎁
═══════════════════════════════════════════════════

📦 Parsed 6 unique present shapes
🌲 Testing 3 regions for feasibility...

─────────────────────────────────────────────────
Region 1: 4×4 grid (2 presents required)
─────────────────────────────────────────────────
Required: 2× Shape#4

Solving... ✓ SOLVABLE (23ms)

Solution:
  AAA.
  ABAB
  ABAB
  .BBB

─────────────────────────────────────────────────
Region 2: 12×5 grid (6 presents required)
─────────────────────────────────────────────────
Required: 1× Shape#0, 1× Shape#2, 2× Shape#4, 2× Shape#5

Solving... ✓ SOLVABLE (145ms)

Solution:
  ....AAAFFE.E
  .BBBAAFFFEEE
  DDDBAAFFCECE
  DBBB....CCC.
  DDD.....C.C.

─────────────────────────────────────────────────
Region 3: 12×5 grid (7 presents required)
─────────────────────────────────────────────────
Required: 1× Shape#0, 1× Shape#2, 3× Shape#4, 2× Shape#5

Solving... ✗ UNSOLVABLE (892ms)

═══════════════════════════════════════════════════
📊 RESULTS SUMMARY
═══════════════════════════════════════════════════
Total Regions:    3
Solvable:         2  ✓
Unsolvable:       1  ✗
Total Time:       1060ms

🎯 ANSWER: 2 regions can fit all presents
═══════════════════════════════════════════════════
```

#### **Design Principles**
1. **Clarity:** Use box-drawing characters for visual separation
2. **Feedback:** Immediate visual feedback (✓/✗) for each region
3. **Aesthetics:** Christmas-themed emojis (🎄🎁📦🌲) for charm
4. **Information Density:** Balance detail with readability
5. **Performance Transparency:** Show timing to indicate algorithm efficiency

#### **Color Scheme (if terminal supports)**
- ✓ Success: Green
- ✗ Failure: Red
- Headers: Cyan/Blue
- Timing: Yellow/Gray
- Present IDs: Rainbow colors (A=Red, B=Blue, C=Green, etc.)

---

## 📋 IMPLEMENTATION SPECIFICATION

### F# Script Structure

```fsharp
// ============================================
// DAY 12: PRESENT PACKING PUZZLE
// Algorithm: Backtracking with CSP
// ============================================

open System
open System.IO

// ──────── DATA TYPES ────────
type Coord = int * int
type Shape = { Index: int; Coords: Coord list }
type Transformation = { Shape: int; Coords: Coord list }
type Region = { /* ... */ }

// ──────── PARSING ────────
let parseShapes (lines: string[]) : Shape list = 
    // Parse shape definitions from input

let parseRegions (lines: string[]) : Region list = 
    // Parse region requirements

// ──────── TRANSFORMATIONS ────────
let rotate90 (coords: Coord list) : Coord list = 
    // Rotate coordinates 90° clockwise

let flipHorizontal (coords: Coord list) : Coord list = 
    // Flip coordinates horizontally

let normalize (coords: Coord list) : Coord list = 
    // Shift to (0,0) origin

let generateTransformations (shape: Shape) : Transformation list = 
    // Generate all unique transformations

// ──────── PLACEMENT LOGIC ────────
let canPlace (grid: bool[,]) (trans: Transformation) (x: int) (y: int) : bool = 
    // Check if transformation fits at (x,y)

let placePresent (grid: bool[,]) (trans: Transformation) (x: int) (y: int) : unit = 
    // Mark grid cells as occupied

let removePresent (grid: bool[,]) (trans: Transformation) (x: int) (y: int) : unit = 
    // Clear grid cells

// ──────── BACKTRACKING SOLVER ────────
let rec solve (region: Region) (presents: Present list) : bool = 
    // Recursive backtracking solver

// ──────── VISUALIZATION ────────
let visualizeGrid (grid: char[,]) : string = 
    // Convert grid to ASCII art

// ──────── MAIN EXECUTION ────────
let input = File.ReadAllLines("input/input12.txt")
let shapes = parseShapes input
let regions = parseRegions input

printfn "🎄 Advent of Code 2025 - Day 12: Present Packing 🎁"
printfn "═══════════════════════════════════════════════════"

let mutable solvableCount = 0

regions |> List.iteri (fun i region ->
    printfn "\n─────────────────────────────────────────────────"
    printfn "Region %d: %dx%d grid" (i+1) region.Width region.Height
    
    let sw = System.Diagnostics.Stopwatch.StartNew()
    let canFit = solve region (createPresentList region.Requirements)
    sw.Stop()
    
    if canFit then
        solvableCount <- solvableCount + 1
        printfn "✓ SOLVABLE (%dms)" sw.ElapsedMilliseconds
    else
        printfn "✗ UNSOLVABLE (%dms)" sw.ElapsedMilliseconds
)

printfn "\n═══════════════════════════════════════════════════"
printfn "🎯 ANSWER: %d regions can fit all presents" solvableCount
```

### Implementation Requirements

1. **Input Handling**
   - Support both `input12.txt` and `input12_example.txt`
   - Robust parsing with error handling

2. **Algorithm Efficiency**
   - Implement heuristics to reduce search space
   - Target: < 5 seconds per region on standard hardware

3. **Output Format**
   - Match UX specification exactly
   - Optional: Disable visualization for speed testing

4. **Testing**
   - Verify example input produces answer `2`
   - Validate transformation logic with unit tests

---

## 🚀 DEVELOPER CHECKLIST

- [ ] Parse shapes correctly (handle `#` and `.` characters)
- [ ] Generate all 8 transformations per shape
- [ ] Implement backtracking solver with pruning
- [ ] Add visualization for solution grids
- [ ] Implement timing and progress output
- [ ] Test with example input (expected: 2)
- [ ] Run with real input
- [ ] Optimize if runtime > 10 seconds

---

**Generated by:** BMAD Architect + UX Designer (simulated)  
**Target Audience:** 5 Developer Agents (gpt4, gpt5, claudeopus45, gemini, grok)  
**Deadline:** Day 12 submission  
**Priority:** HIGH

*Now let the implementors compete fer the fastest solution!* 🏴‍☆
