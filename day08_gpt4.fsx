open System
open System.IO
open System.Globalization

// Parse a line into a 3D point
let parseLine (line: string) =
    let parts = line.Split(',')
    match parts with
    | [|x; y; z|] ->
        let xf = float x
        let yf = float y
        let zf = float z
        (xf, yf, zf)
    | _ -> failwith $"Invalid line: {line}"

// Euclidean distance between two points
let distance (x1, y1, z1) (x2, y2, z2) =
    let dx = x1 - x2
    let dy = y1 - y2
    let dz = z1 - z2
    sqrt (dx * dx + dy * dy + dz * dz)

// Read all points from input file
let inputFile = "input/day08.txt"
let points =
    File.ReadAllLines(inputFile)
    |> Array.filter (fun l -> l.Trim() <> "")
    |> Array.map parseLine

let n = points.Length

// Generate all unique pairs (i < j)
let pairs =
    [ for i in 0 .. n-2 do
        for j in i+1 .. n-1 do
            yield (i, j) ]

// Compute distances for all pairs
let connections =
    pairs
    |> List.map (fun (i, j) -> ((i, j), distance points.[i] points.[j]))
    |> List.sortBy snd
    |> List.truncate 1000

// Union-Find (Disjoint Set) implementation
let unionFind (size: int) (edges: (int * int) list) =
    let parent = Array.init size id
    let rec find x =
        if parent.[x] = x then x
        else
            let root = find parent.[x]
            parent.[x] <- root
            root
    let union x y =
        let rx = find x
        let ry = find y
        if rx <> ry then parent.[ry] <- rx
    edges |> List.iter (fun (a, b) -> union a b)
    // Count sizes of each component
    let groups =
        [0 .. size-1]
        |> List.map find
        |> List.groupBy id
        |> List.map (fun (_, members) -> List.length members)
    groups

// Build circuits from connections
let edgeList = connections |> List.map (fun ((i, j), _) -> (i, j))
let sizes = unionFind n edgeList
let top3 =
    sizes
    |> List.sortDescending
    |> List.truncate 3
let result = top3 |> List.fold (*) 1
printfn "%d" result
