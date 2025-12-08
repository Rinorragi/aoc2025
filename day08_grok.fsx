// Day 8: Junction Box Circuits - Union-Find Solution
open System
open System.IO

// Parse input into 3D coordinates
let parsePoint (line: string) =
    let parts = line.Split(',')
    (int parts.[0], int parts.[1], int parts.[2])

// Calculate squared Euclidean distance (avoid sqrt for efficiency)
let distanceSquared (x1, y1, z1) (x2, y2, z2) =
    let dx = int64 x1 - int64 x2
    let dy = int64 y1 - int64 y2
    let dz = int64 z1 - int64 z2
    dx * dx + dy * dy + dz * dz

// Union-Find with path compression
let rec find (parent: int[]) x =
    if parent.[x] = x then
        x
    else
        parent.[x] <- find parent parent.[x]
        parent.[x]

let union (parent: int[]) (size: int[]) x y =
    let rootX = find parent x
    let rootY = find parent y

    if rootX = rootY then
        false
    else
        if size.[rootX] < size.[rootY] then
            parent.[rootX] <- rootY
            size.[rootY] <- size.[rootY] + size.[rootX]
        else
            parent.[rootY] <- rootX
            size.[rootX] <- size.[rootX] + size.[rootY]

        true

let countComponents (parent: int[]) =
    [ 0 .. parent.Length - 1 ]
    |> List.filter (fun i -> find parent i = i)
    |> List.length

let getComponentSizes (parent: int[]) (size: int[]) =
    [ 0 .. parent.Length - 1 ]
    |> List.filter (fun i -> find parent i = i)
    |> List.map (fun i -> size.[i])
    |> List.sortDescending

// Read and parse input
let points =
    File.ReadAllLines("input/day08.txt")
    |> Array.filter (fun l -> l.Trim() <> "")
    |> Array.map parsePoint

// Generate all pairs with distances
let allPairs =
    [| for i in 0 .. points.Length - 2 do
           for j in i + 1 .. points.Length - 1 do
               let dist = distanceSquared points.[i] points.[j]
               (dist, i, j, points.[i], points.[j]) |]
    |> Array.sortBy (fun (d, _, _, _, _) -> d)

// Phase 1: Connect the 1000 closest pairs (even if some are already connected)
let parent1 = Array.init points.Length id
let size1 = Array.create points.Length 1
let mutable actualConnections = 0

// Take the first 1000 pairs and attempt to connect them
for pairIdx in 0..999 do
    let (_, i, j, _, _) = allPairs.[pairIdx]

    if union parent1 size1 i j then
        actualConnections <- actualConnections + 1

let componentSizes = getComponentSizes parent1 size1
let topThree = componentSizes |> List.take 3
let phase1 = topThree |> List.map int64 |> List.reduce (*)

printfn "Phase 1: %d" phase1

// Phase 2: Connect until all in one circuit
let parent2 = Array.init points.Length id
let size2 = Array.create points.Length 1
let mutable lastPair = None
let mutable idx2 = 0

while countComponents parent2 > 1 && idx2 < allPairs.Length do
    let (_, i, j, p1, p2) = allPairs.[idx2]

    if union parent2 size2 i j then
        lastPair <- Some(p1, p2)

    idx2 <- idx2 + 1

match lastPair with
| Some((x1, _, _), (x2, _, _)) ->
    let phase2 = int64 x1 * int64 x2
    printfn "Phase 2: %d" phase2
| None -> printfn "Phase 2: Error - no last connection found"
