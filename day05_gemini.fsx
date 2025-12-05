open System
open System.IO

/// Represents a range of IDs with a start and end value
type Range = 
    {
        Start: int64
        End: int64
    }
    
    /// Check if an ID is within this range
    member this.Contains (id: int64) : bool =
        id >= this.Start && id <= this.End
    
    /// Try to merge with another range if they overlap or are adjacent
    member this.TryMerge (other: Range) : Range option =
        if other.Start <= this.End + 1L then
            Some { Start = this.Start; End = max this.End other.End }
        else
            None
    
    /// Calculate the count of IDs in this range
    member this.Size : int64 =
        this.End - this.Start + 1L
    
    /// Parse a range from a string like "start-end"
    static member TryParse (line: string) : Range option =
        let parts = line.Split('-')
        if parts.Length >= 2 then
            match Int64.TryParse(parts.[0]), Int64.TryParse(parts.[1]) with
            | (true, start), (true, end_) when start <= end_ -> 
                Some { Start = start; End = end_ }
            | _ -> None
        else
            None

/// Represents a collection of ranges
type RangeCollection =
    {
        Ranges: Range list
    }
    
    /// Check if an ID is in any of the ranges
    member this.Contains (id: int64) : bool =
        this.Ranges |> List.exists (fun range -> range.Contains id)
    
    /// Merge overlapping or adjacent ranges
    member this.Merge () : RangeCollection =
        if this.Ranges.IsEmpty then
            { Ranges = [] }
        else
            let merged =
                this.Ranges
                |> List.sortBy (fun r -> r.Start)
                |> List.fold (fun acc range ->
                    match acc with
                    | [] -> [range]
                    | prevRange :: rest ->
                        match prevRange.TryMerge range with
                        | Some merged -> merged :: rest
                        | None -> range :: acc
                ) []
                |> List.rev
            { Ranges = merged }
    
    /// Calculate the total count of IDs across all ranges
    member this.TotalSize : int64 =
        this.Ranges |> List.sumBy (fun r -> r.Size)
    
    /// Create a collection from a list of ranges
    static member Create (ranges: Range list) : RangeCollection =
        { Ranges = ranges }

/// Represents the solver for the problem
type Solver =
    {
        Ranges: RangeCollection
        Ids: int64 list
    }
    
    /// Solve phase 1: count IDs that are in any range
    member this.SolvePhase1 () : int =
        this.Ids
        |> List.filter (fun id -> this.Ranges.Contains id)
        |> List.length
    
    /// Solve phase 2: count union size of merged ranges
    member this.SolvePhase2 () : int64 =
        let mergedRanges = this.Ranges.Merge ()
        mergedRanges.TotalSize
    
    /// Solve both phases
    member this.Solve () : int * int64 =
        (this.SolvePhase1 (), this.SolvePhase2 ())
    
    /// Create a solver from ranges and IDs
    static member Create (ranges: Range list) (ids: int64 list) : Solver =
        {
            Ranges = RangeCollection.Create ranges
            Ids = ids
        }

/// Represents a file processor
type FileProcessor =
    {
        FilePath: string
    }
    
    /// Parse ranges and IDs from file
    member this.ParseFile () : (Range list * int64 list) option =
        if not (File.Exists(this.FilePath)) then
            printfn "File not found: %s" this.FilePath
            None
        else
            let lines = File.ReadAllLines(this.FilePath)
            
            let blankLineIndex =
                lines
                |> Array.tryFindIndex (fun line -> String.IsNullOrWhiteSpace(line))
                |> Option.defaultValue -1

            if blankLineIndex < 0 then
                printfn "Invalid input format: no blank line separator found"
                None
            else
                let ranges =
                    lines.[0..blankLineIndex-1]
                    |> Array.choose Range.TryParse
                    |> Array.toList

                let ids =
                    lines.[blankLineIndex+1..]
                    |> Array.filter (fun line -> not (String.IsNullOrWhiteSpace(line)))
                    |> Array.map Int64.Parse
                    |> Array.toList

                Some (ranges, ids)
    
    /// Process the file and print results
    member this.Process () : unit =
        match this.ParseFile () with
        | None -> ()
        | Some (ranges, ids) ->
            let solver = Solver.Create ranges ids
            let (phase1, phase2) = solver.Solve ()
            printfn "%s: %d %d" this.FilePath phase1 phase2
    
    /// Create a file processor
    static member Create (filePath: string) : FileProcessor =
        { FilePath = filePath }

// Execute processing
FileProcessor.Create "input/day05_example.txt" |> fun p -> p.Process ()
FileProcessor.Create "input/day05.txt" |> fun p -> p.Process ()
