// Day 8: Junction Box Circuits - Gemini Implementation
// Using Union-Find (Disjoint Set Union) for efficient circuit tracking

open System
open System.IO

type Point3D = { X: int; Y: int; Z: int }

let parsePoint (line: string) : Point3D =
    let parts = line.Split(',')

    { X = int parts.[0]
      Y = int parts.[1]
      Z = int parts.[2] }

let squaredDistance (p1: Point3D) (p2: Point3D) : int64 =
    let dx = int64 (p1.X - p2.X)
    let dy = int64 (p1.Y - p2.Y)
    let dz = int64 (p1.Z - p2.Z)
    dx * dx + dy * dy + dz * dz

let getAllDistances (points: Point3D array) : (int64 * int * int) array =
    let n = points.Length

    [| for i in 0 .. n - 2 do
           for j in i + 1 .. n - 1 do
               let dist = squaredDistance points.[i] points.[j]
               yield (dist, i, j) |]
    |> Array.sortBy (fun (d, _, _) -> d)

// Union-Find with path compression and union by rank
let find (parent: int array) (x: int) : int =
    let rec loop x =
        if parent.[x] = x then
            x
        else
            let root = loop parent.[x]
            parent.[x] <- root // Path compression
            root

    loop x

let union (parent: int array) (size: int array) (x: int) (y: int) : bool =
    let rootX = find parent x
    let rootY = find parent y

    if rootX = rootY then
        false // Already in same set
    else
        // Union by size
        if size.[rootX] < size.[rootY] then
            parent.[rootX] <- rootY
            size.[rootY] <- size.[rootY] + size.[rootX]
        else
            parent.[rootY] <- rootX
            size.[rootX] <- size.[rootX] + size.[rootY]

        true

let countCircuits (parent: int array) (n: int) : int =
    [ 0 .. n - 1 ] |> List.map (find parent) |> List.distinct |> List.length

let getCircuitSizes (parent: int array) (size: int array) (n: int) : int list =
    let roots = [ 0 .. n - 1 ] |> List.map (find parent) |> List.distinct

    roots |> List.map (fun root -> size.[root]) |> List.sortDescending

let solvePhase1 (points: Point3D array) : int =
    let n = points.Length
    let distances = getAllDistances points
    let parent = Array.init n id
    let size = Array.create n 1

    // Process the 1000 closest pairs (by distance), not 1000 successful connections
    for idx in 0..999 do
        if idx < distances.Length then
            let (_, i, j) = distances.[idx]
            union parent size i j |> ignore

    let sizes = getCircuitSizes parent size n

    // Multiply the three largest circuit sizes
    let topThree = if sizes.Length >= 3 then sizes |> List.take 3 else sizes

    topThree |> List.fold (*) 1

let solvePhase2 (points: Point3D array) : int =
    let n = points.Length
    let distances = getAllDistances points
    let parent = Array.init n id
    let size = Array.create n 1

    let mutable idx = 0
    let mutable lastConnection = None

    while countCircuits parent n > 1 && idx < distances.Length do
        let (_, i, j) = distances.[idx]

        if union parent size i j then
            lastConnection <- Some(points.[i].X * points.[j].X)

        idx <- idx + 1

    match lastConnection with
    | Some result -> result
    | None -> 0

let solve () =
    let input = File.ReadAllLines("input/day08.txt")

    let points =
        input |> Array.filter (fun line -> line.Trim() <> "") |> Array.map parsePoint

    let phase1 = solvePhase1 points
    let phase2 = solvePhase2 points

    printfn "Phase 1: %d" phase1
    printfn "Phase 2: %d" phase2

solve ()
