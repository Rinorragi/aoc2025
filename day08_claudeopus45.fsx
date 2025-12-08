// Day 8: Junction Box Circuits - Advent of Code 2025
// Union-Find approach for circuit tracking

open System
open System.IO

type Point3D = { X: int; Y: int; Z: int }

// Parse a line into a Point3D
let parsePoint (line: string) =
    match line.Split(',') |> Array.map int with
    | [| x; y; z |] -> { X = x; Y = y; Z = z }
    | _ -> failwithf "Invalid line: %s" line

// Euclidean distance between two 3D points
let distance p1 p2 =
    let dx = float (p1.X - p2.X)
    let dy = float (p1.Y - p2.Y)
    let dz = float (p1.Z - p2.Z)
    sqrt (dx * dx + dy * dy + dz * dz)

// Union-Find with path compression
let find (parent: int[]) x =
    let rec findRoot x =
        if parent.[x] = x then
            x
        else
            let root = findRoot parent.[x]
            parent.[x] <- root // Path compression
            root

    findRoot x

let union (parent: int[]) (size: int[]) x y =
    let xr = find parent x
    let yr = find parent y

    if xr <> yr then
        if size.[xr] < size.[yr] then
            parent.[xr] <- yr
            size.[yr] <- size.[yr] + size.[xr]
        else
            parent.[yr] <- xr
            size.[xr] <- size.[xr] + size.[yr]

        true // Successfully connected
    else
        false // Already in same circuit

let countCircuits (parent: int[]) n =
    [ 0 .. n - 1 ] |> List.filter (fun i -> find parent i = i) |> List.length

let getCircuitSizes (parent: int[]) (size: int[]) n =
    [ 0 .. n - 1 ]
    |> List.filter (fun i -> find parent i = i)
    |> List.map (fun root -> size.[root])
    |> List.sortDescending

// Generate all pairs with their distances, sorted by distance
let getAllConnectionsSorted (points: Point3D[]) =
    [| for i in 0 .. points.Length - 2 do
           for j in i + 1 .. points.Length - 1 do
               let dist = distance points.[i] points.[j]
               (i, j, dist, points.[i], points.[j]) |]
    |> Array.sortBy (fun (_, _, d, _, _) -> d)

let solvePhase1 (points: Point3D[]) =
    let parent = Array.init points.Length id
    let size = Array.create points.Length 1
    let connections = getAllConnectionsSorted points

    let mutable connectionsMade = 0
    let mutable idx = 0

    while connectionsMade < 1000 && idx < connections.Length do
        let (i, j, _, _, _) = connections.[idx]

        if union parent size i j then
            connectionsMade <- connectionsMade + 1

        idx <- idx + 1

    let sizes = getCircuitSizes parent size points.Length
    let top3 = sizes |> List.truncate 3 |> List.toArray

    if top3.Length = 3 then
        top3.[0] * top3.[1] * top3.[2]
    else
        // If fewer than 3 circuits, multiply what we have
        top3 |> Array.fold (*) 1

let solvePhase2 (points: Point3D[]) =
    let parent = Array.init points.Length id
    let size = Array.create points.Length 1
    let connections = getAllConnectionsSorted points

    let mutable lastConnection = (0, 0, 0.0, points.[0], points.[0])
    let mutable idx = 0

    while countCircuits parent points.Length > 1 && idx < connections.Length do
        let (i, j, _, p1, p2) = connections.[idx]

        if union parent size i j then
            lastConnection <- connections.[idx]

        idx <- idx + 1

    let (_, _, _, p1, p2) = lastConnection
    p1.X * p2.X

let run () =
    let points =
        File.ReadAllLines("input/day08.txt")
        |> Array.filter (fun l -> l.Trim() <> "")
        |> Array.map parsePoint

    let phase1 = solvePhase1 points
    let phase2 = solvePhase2 points

    printfn "Phase 1: %d" phase1
    printfn "Phase 2: %d" phase2

run ()
