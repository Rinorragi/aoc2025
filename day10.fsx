(***
Advent of Code Day 10 (F# fsi)
Usage (PowerShell):

  # Example input
  fsi day10.fsx --day 10 --part 1 --example

  # Real input
  fsi day10.fsx --day 10 --part 1

Inputs are expected at (sibling to repo):
  ../input/input10_example.txt
  ../input/input10.txt

Note: Instruction fetching via MCP is handled by speckit workflows, not code.

Flags:
  --day NN    : required, zero-padded day (e.g., 10)
  --part N    : required, puzzle part (1 or 2)
  --example   : optional, use example input file

Solution approach:
- Parse each line: [diagram] (buttons...) {ignored}
- Model as GF(2) system: buttons are XOR masks, target is goal state
- Use Gaussian elimination to find solution space
- Minimize Hamming weight (total presses) across nullspace
***)

open System
open System.IO
open System.Diagnostics

let parseArgs (argv: string[]) =
    let rec loop i dayOpt partOpt useExample =
        if i >= argv.Length then (dayOpt, partOpt, useExample)
        else
            match argv.[i] with
            | "--day" when i + 1 < argv.Length -> loop (i+2) (Some argv.[i+1]) partOpt useExample
            | "--part" when i + 1 < argv.Length -> loop (i+2) dayOpt (Some (int argv.[i+1])) useExample
            | "--example" -> loop (i+1) dayOpt partOpt true
            | unknown ->
                eprintfn "Unknown argument: %s" unknown
                loop (i+1) dayOpt partOpt useExample
    match loop 0 None None false with
    | Some d, Some p, ex -> d, p, ex
    | _ ->
        eprintfn "Missing required --day NN and/or --part N"
        eprintfn "Example: fsi day10.fsx --day 10 --part 1 --example"
        Environment.Exit 2; "00", 0, false

let readInput (day: string) (useExample: bool) =
    let baseDir = __SOURCE_DIRECTORY__
    let fileName = if useExample then sprintf "input%s_example.txt" day else sprintf "input%s.txt" day
    let inputPath = Path.Combine(baseDir, "input", fileName)
    if not (File.Exists inputPath) then
        eprintfn "Input file not found: %s" inputPath
        Environment.Exit 3
    File.ReadAllLines inputPath |> Array.toList

// Part 1: Parse machine line for binary configuration: [.##.] (0,1,2) (3,4) {ignored}
type Machine = { Target: bool list; Buttons: bool list list }

// Part 2: Parse machine line for integer configuration: [ignored] (0,1,2) (3,4) {3,5,4,7}
type MachineInt = { TargetJoltage: int list; Buttons: int list list }

let parseMachine (line: string) =
    // Extract target diagram between [ ]
    let startBracket = line.IndexOf('[')
    let endBracket = line.IndexOf(']')
    let diagram = line.Substring(startBracket + 1, endBracket - startBracket - 1)
    let target = diagram |> Seq.map (fun c -> c = '#') |> Seq.toList
    let numLights = target.Length
    
    // Extract button definitions between ( )
    let rec extractButtons (pos: int) acc =
        let openParen = line.IndexOf('(', pos)
        if openParen = -1 then List.rev acc
        else
            let closeParen = line.IndexOf(')', openParen)
            let buttonStr = line.Substring(openParen + 1, closeParen - openParen - 1)
            let indices = 
                buttonStr.Split([|','|], StringSplitOptions.RemoveEmptyEntries)
                |> Array.map (fun s -> int (s.Trim()))
                |> Set.ofArray
            let mask = List.init numLights (fun i -> indices.Contains i)
            extractButtons (closeParen + 1) (mask :: acc)
    
    let buttons = extractButtons 0 []
    { Target = target; Buttons = buttons }

let parseMachineInt (line: string) =
    // Extract joltage requirements between { }
    let startBrace = line.IndexOf('{')
    let endBrace = line.IndexOf('}')
    let joltageStr = line.Substring(startBrace + 1, endBrace - startBrace - 1)
    let targetJoltage = 
        joltageStr.Split([|','|], StringSplitOptions.RemoveEmptyEntries)
        |> Array.map (fun s -> int (s.Trim()))
        |> Array.toList
    let numCounters = targetJoltage.Length
    
    // Extract button definitions between ( ) - same as Part 1 but convert to int masks
    let rec extractButtons (pos: int) acc =
        let openParen = line.IndexOf('(', pos)
        if openParen = -1 then List.rev acc
        else
            let closeParen = line.IndexOf(')', openParen)
            let buttonStr = line.Substring(openParen + 1, closeParen - openParen - 1)
            let indices = 
                buttonStr.Split([|','|], StringSplitOptions.RemoveEmptyEntries)
                |> Array.map (fun s -> int (s.Trim()))
                |> Set.ofArray
            // For Part 2: mask has 1 at affected indices, 0 elsewhere
            let mask = List.init numCounters (fun i -> if indices.Contains i then 1 else 0)
            extractButtons (closeParen + 1) (mask :: acc)
    
    let buttons = extractButtons 0 []
    { TargetJoltage = targetJoltage; Buttons = buttons }

// GF(2) Gaussian elimination with minimal weight solution (Part 1)
let minimizePresses (machine: Machine) =
    let n = machine.Target.Length
    let m = machine.Buttons.Length
    
    // Build augmented matrix [A | b] where A is button masks, b is target
    let matrix = 
        List.init n (fun row ->
            let buttonCols = machine.Buttons |> List.map (fun btn -> if btn.[row] then 1 else 0)
            let targetCol = if machine.Target.[row] then 1 else 0
            buttonCols @ [targetCol] |> List.toArray
        )
        |> List.toArray
    
    // Gaussian elimination over GF(2) - immutable fold-based approach
    let eliminateColumn (state: int * int array) col =
        let pivot, pivotCols = state
        // Find pivot row
        let pivotRow = 
            seq { pivot .. n - 1 }
            |> Seq.tryFind (fun row -> matrix.[row].[col] = 1)
        
        match pivotRow with
        | Some row ->
            // Swap rows if needed
            if row <> pivot then
                let temp = matrix.[pivot]
                matrix.[pivot] <- matrix.[row]
                matrix.[row] <- temp
            
            pivotCols.[col] <- pivot
            
            // Eliminate other rows
            for r = 0 to n - 1 do
                if r <> pivot && matrix.[r].[col] = 1 then
                    for c = 0 to m do
                        matrix.[r].[c] <- matrix.[r].[c] ^^^ matrix.[pivot].[c]
            
            (pivot + 1, pivotCols)
        | None -> (pivot, pivotCols)
    
    let pivot, pivotCols = 
        [0 .. m - 1] 
        |> List.fold eliminateColumn (0, Array.create m -1)
    
    // Check for inconsistency
    let isInconsistent =
        seq { pivot .. n - 1 }
        |> Seq.exists (fun row -> matrix.[row].[m] = 1)
    
    if isInconsistent then
        failwith "No solution exists"
    
    // Find basic solution
    let baseSol =
        Array.init m (fun col ->
            if pivotCols.[col] >= 0 then
                matrix.[pivotCols.[col]].[m]
            else
                0
        )
    
    // Find free variables (nullspace basis)
    let freeVars = 
        [0 .. m - 1] 
        |> List.filter (fun col -> pivotCols.[col] = -1)
    
    // If no free variables, return basic solution weight
    if freeVars.IsEmpty then
        baseSol |> Array.sum
    else
        // Search nullspace for minimal weight solution
        let nullspaceBasis =
            freeVars |> List.map (fun freeCol ->
                Array.init m (fun col ->
                    if col = freeCol then 1
                    elif pivotCols.[col] >= 0 then matrix.[pivotCols.[col]].[freeCol]
                    else 0
                )
            )
        
        // Try all combinations of nullspace vectors (up to 2^|freeVars|)
        let maxCombos = 1 <<< (min freeVars.Length 18)  // Cap at 2^18 to prevent overflow
        let baseWeight = baseSol |> Array.sum
        
        let computeWeight combo =
            let additions =
                nullspaceBasis 
                |> List.indexed
                |> List.filter (fun (i, _) -> (combo &&& (1 <<< i)) <> 0)
                |> List.map snd
            
            let candidate =
                Array.init m (fun j ->
                    additions
                    |> List.fold (fun acc basis -> acc ^^^ basis.[j]) baseSol.[j]
                )
            
            candidate |> Array.sum
        
        seq { 0 .. maxCombos - 1 }
        |> Seq.map computeWeight
        |> Seq.min

let solvePart1 (lines: string list) =
    let machines = lines |> List.map parseMachine
    let totalPresses = machines |> List.sumBy minimizePresses
    string totalPresses

// Integer linear programming solver for Part 2 - Gaussian elimination
let minimizePressesInt (machine: MachineInt) =
    let n = machine.TargetJoltage.Length
    let m = machine.Buttons.Length
    
    if m = 0 then 0
    else
        // Use int64 to prevent overflow during elimination
        let target = machine.TargetJoltage |> List.toArray |> Array.map int64
        let buttons = machine.Buttons |> List.map (List.toArray >> Array.map int64) |> List.toArray
        
        // Build augmented matrix [A | b] where A is button increments, b is target
        let matrix = 
            Array.init n (fun row ->
                let buttonCols = buttons |> Array.map (fun btn -> btn.[row])
                Array.append buttonCols [|target.[row]|]
            )
        
        // Gaussian elimination with integer arithmetic
        let mutable pivot = 0
        let pivotCols = Array.create m -1
        
        for col = 0 to m - 1 do
            // Find pivot row with non-zero element
            let pivotRow =
                seq { pivot .. n - 1 }
                |> Seq.tryFind (fun row -> matrix.[row].[col] <> 0L)
            
            match pivotRow with
            | Some row when pivot < n ->
                // Swap rows if needed
                if row <> pivot then
                    let temp = matrix.[pivot]
                    matrix.[pivot] <- matrix.[row]
                    matrix.[row] <- temp
                
                pivotCols.[col] <- pivot
                
                // Eliminate other rows using integer arithmetic
                for r = 0 to n - 1 do
                    if r <> pivot && matrix.[r].[col] <> 0L then
                        let factor = matrix.[r].[col]
                        let pivotVal = matrix.[pivot].[col]
                        for c = 0 to m do
                            matrix.[r].[c] <- matrix.[r].[c] * pivotVal - matrix.[pivot].[c] * factor
                
                pivot <- pivot + 1
            | _ -> ()
        
        // Find free variables (columns without pivots)
        let freeVars = 
            [0 .. m - 1] 
            |> List.filter (fun col -> pivotCols.[col] = -1)
        
        // Back-substitution with parameterized free variables
        let computeSolution (freeValues: int64 array) =
            let solution = Array.create m 0L
            
            // Set free variables
            freeVars |> List.iteri (fun i col -> solution.[col] <- freeValues.[i])
            
            // Back-substitute for pivot variables
            for col = m - 1 downto 0 do
                if pivotCols.[col] >= 0 then
                    let row = pivotCols.[col]
                    let mutable sum = matrix.[row].[m]
                    
                    for c = col + 1 to m - 1 do
                        sum <- sum - matrix.[row].[c] * solution.[c]
                    
                    if matrix.[row].[col] <> 0L then
                        // Check if division is exact
                        if sum % matrix.[row].[col] = 0L then
                            solution.[col] <- sum / matrix.[row].[col]
                        else
                            // Not an exact solution - mark as invalid
                            solution.[col] <- -1000000L
            
            solution
        
        // Search nullspace for minimal non-negative solution
        if freeVars.IsEmpty then
            // No free variables - unique solution
            let solution = computeSolution [||]
            let isValid = Array.forall (fun x -> x >= 0L) solution
            if isValid then solution |> Array.sumBy int else 0
        else
            // Try different values for free variables to find minimal solution
            let maxFreeValue = 300L  // Increased for large targets
            let mutable bestCost = System.Int32.MaxValue
            let mutable foundValid = false
            
            // Generate combinations of free variable values
            let rec searchFreeVars (index: int) (current: int64 list) =
                // Early termination if current partial cost exceeds best
                let partialCost = List.sum current
                if partialCost > int64 bestCost then ()
                elif index = freeVars.Length then
                    let solution = computeSolution (List.rev current |> List.toArray)
                    let isValid = Array.forall (fun x -> x >= 0L) solution
                    
                    if isValid then
                        // Verify solution
                        let reconstructed = Array.init n (fun j ->
                            Array.sumBy (fun i -> buttons.[i].[j] * solution.[i]) [|0 .. m-1|]
                        )
                        if Array.forall2 (=) reconstructed target then
                            let cost = solution |> Array.sumBy int
                            if cost < bestCost then
                                bestCost <- cost
                                foundValid <- true
                else
                    for value in 0L .. maxFreeValue do
                        searchFreeVars (index + 1) (value :: current)
            
            searchFreeVars 0 []
            if foundValid then bestCost else 0

let solvePart2 (lines: string list) =
    let machines = lines |> List.map parseMachineInt
    let totalPresses = machines |> List.sumBy minimizePressesInt
    string totalPresses

let main argv =
    let swTotal = Stopwatch.StartNew()
    let day, part, useExample = parseArgs argv
    let lines = readInput day useExample
    let result =
        match part with
        | 1 -> solvePart1 lines
        | 2 -> solvePart2 lines
        | n ->
            eprintfn "Unsupported part: %d" n
            Environment.Exit 4; ""
    printfn "%s" result
    swTotal.Stop()
    eprintfn "Timing: total=%dms" swTotal.ElapsedMilliseconds
    0

// FSI invocation
main fsi.CommandLineArgs.[1..] |> ignore
