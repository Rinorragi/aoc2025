// Day 8: Advent of Code 2025 - Junction Box Circuits
// Implements Union-Find (Disjoint Set Union) to track circuits

open System
open System.IO

type Point3D = { X: int; Y: int; Z: int }

type UnionFind = {
    Parent: int[]
    Rank: int[]
    Size: int[]
}

// Parse input into 3D points
let parseInput (lines: string[]) =
    lines
    |> Array.filter (fun l -> l.Trim() <> "")
    |> Array.map (fun line ->
        let parts = line.Split(',')
        { X = int parts.[0]; Y = int parts.[1]; Z = int parts.[2] })

// Calculate squared distance (avoid sqrt for performance)
let distanceSquared p1 p2 =
    let dx = p1.X - p2.X |> int64
    let dy = p1.Y - p2.Y |> int64
    let dz = p1.Z - p2.Z |> int64
    dx * dx + dy * dy + dz * dz

// Initialize Union-Find structure
let createUnionFind n =
    {
        Parent = Array.init n id
        Rank = Array.create n 0
        Size = Array.create n 1
    }

// Find root with path compression
let rec find (uf: UnionFind) x =
    if uf.Parent.[x] <> x then
        uf.Parent.[x] <- find uf uf.Parent.[x]
    uf.Parent.[x]

// Union by rank, returns true if merged (were in different sets)
let union (uf: UnionFind) x y =
    let rootX = find uf x
    let rootY = find uf y
    
    if rootX = rootY then
        false
    else
        if uf.Rank.[rootX] < uf.Rank.[rootY] then
            uf.Parent.[rootX] <- rootY
            uf.Size.[rootY] <- uf.Size.[rootY] + uf.Size.[rootX]
        elif uf.Rank.[rootX] > uf.Rank.[rootY] then
            uf.Parent.[rootY] <- rootX
            uf.Size.[rootX] <- uf.Size.[rootX] + uf.Size.[rootY]
        else
            uf.Parent.[rootY] <- rootX
            uf.Size.[rootX] <- uf.Size.[rootX] + uf.Size.[rootY]
            uf.Rank.[rootX] <- uf.Rank.[rootX] + 1
        true

// Get all circuit sizes
let getCircuitSizes (uf: UnionFind) =
    uf.Parent
    |> Array.mapi (fun i _ -> if find uf i = i then uf.Size.[i] else 0)
    |> Array.filter (fun s -> s > 0)

// Generate all pairs with their distances
let generatePairs (points: Point3D[]) =
    let n = points.Length
    [| for i in 0 .. n - 2 do
        for j in i + 1 .. n - 1 do
            let dist = distanceSquared points.[i] points.[j]
            (dist, i, j, points.[i], points.[j]) |]
    |> Array.sortBy (fun (dist, _, _, _, _) -> dist)

// Phase 1: Connect 1000 closest pairs
let solvePhase1 (points: Point3D[]) =
    let uf = createUnionFind points.Length
    let pairs = generatePairs points
    
    let mutable connections = 0
    let mutable idx = 0
    
    while connections < 1000 && idx < pairs.Length do
        let (_, i, j, _, _) = pairs.[idx]
        if union uf i j then
            connections <- connections + 1
        idx <- idx + 1
    
    let circuits = getCircuitSizes uf
    let top3 = 
        circuits 
        |> Array.sortDescending 
        |> Array.truncate 3
    top3 |> Array.fold (*) 1

// Phase 2: Connect until one circuit
let solvePhase2 (points: Point3D[]) =
    let uf = createUnionFind points.Length
    let pairs = generatePairs points
    
    let mutable numCircuits = points.Length
    let mutable idx = 0
    let mutable lastConnection = (0, 0)
    
    while numCircuits > 1 && idx < pairs.Length do
        let (_, i, j, p1, p2) = pairs.[idx]
        if union uf i j then
            numCircuits <- numCircuits - 1
            lastConnection <- (p1.X, p2.X)
        idx <- idx + 1
    
    let (x1, x2) = lastConnection
    x1 * x2

// Main execution
let lines = File.ReadAllLines("input/day08.txt")
let points = parseInput lines

printfn "Phase 1: %d" (solvePhase1 points)
printfn "Phase 2: %d" (solvePhase2 points)
