open System
open System.IO
open System.Text.Json
open System.Text.Json.Serialization
open System.Threading.Tasks

type InstructionData = {
    [<JsonPropertyName("day")>]
    Day: int
    [<JsonPropertyName("phase1")>]
    Phase1: string
    [<JsonPropertyName("phase2")>]
    Phase2: string option
}

type SpeedEntry = {
    [<JsonPropertyName("llm")>]
    LLM: string
    [<JsonPropertyName("phase")>]
    Phase: int
    [<JsonPropertyName("speed_ms")>]
    SpeedMs: int64
    [<JsonPropertyName("timestamp")>]
    Timestamp: string
}

type DaySpeedStats = {
    [<JsonPropertyName("day")>]
    Day: int
    [<JsonPropertyName("entries")>]
    Entries: SpeedEntry list
}

let baseDir = 
    // Use the project source directory for data persistence, not the binary directory
    // Navigate up from bin/Debug/net10.0 to the project root
    let currentDir = AppContext.BaseDirectory
    let parentDir = Directory.GetParent(currentDir).Parent.Parent.Parent.FullName
    if Directory.Exists(Path.Combine(parentDir, "instructions")) then
        parentDir  // We're in correct directory
    else
        AppContext.BaseDirectory  // Fallback to binary directory if source not found

let instructionsDir = Path.Combine(baseDir, "instructions")
let speedsDir = Path.Combine(baseDir, "speeds")

let jsonOptions = JsonSerializerOptions(PropertyNamingPolicy = JsonNamingPolicy.CamelCase)

let ensureDirectories () =
    Directory.CreateDirectory(instructionsDir) |> ignore
    Directory.CreateDirectory(speedsDir) |> ignore

let loadInstruction (day: int) : Result<InstructionData, string> =
    try
        let filePath = Path.Combine(instructionsDir, $"day{day:D2}.json")
        if not (File.Exists filePath) then
            Error $"Instruction file not found for day {day}"
        else
            let json = File.ReadAllText filePath
            let data = JsonSerializer.Deserialize<InstructionData>(json, jsonOptions)
            Ok data
    with ex ->
        Error $"Error loading instruction: {ex.Message}"

let getAvailableDays () : int list =
    try
        Directory.GetFiles(instructionsDir, "day*.json")
        |> Array.map (fun f -> 
            let name = Path.GetFileNameWithoutExtension f
            match name.Substring(3) with
            | dayStr when Int32.TryParse(dayStr) |> fst -> Int32.Parse(dayStr)
            | _ -> 0)
        |> Array.filter (fun d -> d > 0)
        |> Array.sortDescending
        |> Array.toList
    with _ -> []

let loadDaySpeed (day: int) : DaySpeedStats =
    try
        let filePath = Path.Combine(speedsDir, $"day{day:D2}.json")
        if File.Exists filePath then
            let json = File.ReadAllText filePath
            JsonSerializer.Deserialize<DaySpeedStats>(json, jsonOptions)
        else
            { Day = day; Entries = [] }
    with _ ->
        { Day = day; Entries = [] }

let saveDaySpeed (stats: DaySpeedStats) : Result<unit, string> =
    try
        let filePath = Path.Combine(speedsDir, $"day{stats.Day:D2}.json")
        let json = JsonSerializer.Serialize(stats, jsonOptions)
        File.WriteAllText(filePath, json)
        Ok()
    with ex ->
        Error $"Error saving speed data: {ex.Message}"

let recordSpeed (day: int) (phase: int) (llm: string) (speedMs: int64) : Result<DaySpeedStats, string> =
    try
        let entry: SpeedEntry = {
            LLM = llm
            Phase = phase
            SpeedMs = speedMs
            Timestamp = DateTime.UtcNow.ToString("o")
        }
        let stats = loadDaySpeed day
        let updatedEntries = entry :: stats.Entries
        let updated = { stats with Entries = updatedEntries }
        match saveDaySpeed updated with
        | Ok() -> Ok updated
        | Error e -> Error e
    with ex ->
        Error $"Error recording speed: {ex.Message}"

let listAvailableInstructions () : unit =
    let days = getAvailableDays()
    if days.IsEmpty then
        printfn "No instruction files found in %s" instructionsDir
    else
        printfn "Available Advent of Code days:"
        days |> List.iter (fun day -> printfn "  Day %d" day)

let showDay (day: int) : unit =
    match loadInstruction day with
    | Ok instr ->
        printfn "=== Day %d ===" day
        printfn "\n[Phase 1]\n%s" instr.Phase1
        (match instr.Phase2 with
         | Some p2 -> printfn "\n[Phase 2]\n%s" p2
         | None -> ())
    | Error e -> printfn "Error: %s" e

let recordSpeedCli (day: int) (phase: int) (llm: string) (speed: int64) : unit =
    match recordSpeed day phase llm speed with
    | Ok stats ->
        let llmEntries = stats.Entries |> List.filter (fun e -> e.LLM = llm)
        let avg = if llmEntries.IsEmpty then 0L else llmEntries |> List.map (fun e -> e.SpeedMs) |> List.averageBy float |> int64
        printfn "Recorded %s Day %d Phase %d: %dms (avg: %dms)" llm day phase speed avg
    | Error e -> printfn "Error: %s" e

let runMcpServer () : unit = 
    let stdin = Console.In
    let stdout = Console.Out
    
    let rec processInput () =
        try
            let line = stdin.ReadLine()
            if line = null then ()
            else
                try
                    let json = JsonDocument.Parse(line)
                    let root = json.RootElement
                    
                    let method_ = root.GetProperty("method").GetString()
                    let mutable paramsRef = Unchecked.defaultof<JsonElement>
                    let hasParams = root.TryGetProperty("params", &paramsRef)
                    let mutable idRef = Unchecked.defaultof<JsonElement>
                    let hasId = root.TryGetProperty("id", &idRef)
                    
                    if not hasId then
                        processInput()
                    else
                        let id = idRef.GetInt32()
                        
                        let response = 
                            match method_ with
                            | "resources/list" ->
                                let days = getAvailableDays()
                                let resourceList =
                                    days
                                    |> List.map (fun day -> 
                                        sprintf """{"uri":"aoc://day%02d","name":"Day %d Instructions","description":"Full puzzle instructions for Advent of Code 2025 Day %d","mimeType":"text/plain"}""" day day day)
                                    |> String.concat ","
                                sprintf """{"jsonrpc":"2.0","id":%d,"result":{"resources":[%s]}}""" id resourceList
                            
                            | "resources/read" ->
                                if hasParams then
                                    let uri = paramsRef.GetProperty("uri").GetString()
                                    let dayMatch = uri.Replace("aoc://day", "")
                                    if Int32.TryParse(dayMatch) |> fst then
                                        let day = Int32.Parse(dayMatch)
                                        match loadInstruction day with
                                        | Ok instr ->
                                            let content =
                                                match instr.Phase2 with
                                                | Some p2 -> $"# Day {day}\n\n## Phase 1\n{instr.Phase1}\n\n## Phase 2\n{p2}"
                                                | None -> $"# Day {day}\n\n{instr.Phase1}"
                                            let escaped = JsonSerializer.Serialize(content)
                                            sprintf """{"jsonrpc":"2.0","id":%d,"result":{"contents":[{"uri":"aoc://day%02d","mimeType":"text/plain","text":%s}]}}""" id day escaped
                                        | Error e ->
                                            sprintf """{"jsonrpc":"2.0","id":%d,"error":{"code":-1,"message":"Error loading instruction: %s"}}""" id e
                                    else
                                        sprintf """{"jsonrpc":"2.0","id":%d,"error":{"code":-1,"message":"Invalid day format"}}""" id
                                else
                                    sprintf """{"jsonrpc":"2.0","id":%d,"error":{"code":-32602,"message":"Missing params"}}""" id
                            
                            | "tools/list" ->
                                sprintf """{"jsonrpc":"2.0","id":%d,"result":{"tools":[{"name":"fetch_instruction","description":"Fetch the full puzzle instruction for a specific day","inputSchema":{"type":"object","properties":{"day":{"type":"integer","description":"Day number (1-25)"}},"required":["day"]}},{"name":"record_speed","description":"Record the solving speed for an LLM on a specific day and phase","inputSchema":{"type":"object","properties":{"day":{"type":"integer","description":"Day number"},"phase":{"type":"integer","description":"Phase (1 or 2)"},"llm_name":{"type":"string","description":"LLM name"},"speed_ms":{"type":"integer","description":"Solving time in milliseconds"}},"required":["day","phase","llm_name","speed_ms"]}}]}}""" id
                            
                            | "tools/call" ->
                                if hasParams then
                                    let toolName = paramsRef.GetProperty("name").GetString()
                                    let mutable argsRef = Unchecked.defaultof<JsonElement>
                                    if paramsRef.TryGetProperty("arguments", &argsRef) then
                                        match toolName with
                                        | "fetch_instruction" ->
                                            let day = argsRef.GetProperty("day").GetInt32()
                                            (match loadInstruction day with
                                             | Ok instr ->
                                                 let content = match instr.Phase2 with Some p2 -> $"# Day {day}\n\n## Phase 1\n{instr.Phase1}\n\n## Phase 2\n{p2}" | None -> $"# Day {day}\n\n{instr.Phase1}"
                                                 sprintf """{"jsonrpc":"2.0","id":%d,"result":{"content":[{"type":"text","text":"%s"}]}}""" id (content.Replace("\"", "\\\"").Replace("\n", "\\n"))
                                             | Error e ->
                                                 sprintf """{"jsonrpc":"2.0","id":%d,"error":{"code":-1,"message":"%s"}}""" id e)
                                        | "record_speed" ->
                                            let day = argsRef.GetProperty("day").GetInt32()
                                            let phase = argsRef.GetProperty("phase").GetInt32()
                                            let llm = argsRef.GetProperty("llm_name").GetString()
                                            let speedMs = argsRef.GetProperty("speed_ms").GetInt64()
                                            (match recordSpeed day phase llm speedMs with
                                             | Ok stats ->
                                                 let llmEntries = stats.Entries |> List.filter (fun e -> e.LLM = llm)
                                                 let avg = if llmEntries.IsEmpty then 0L else llmEntries |> List.map (fun e -> e.SpeedMs) |> List.averageBy float |> int64
                                                 let result = $"Recorded {llm} Day {day} Phase {phase}: {speedMs}ms (avg: {avg}ms)"
                                                 sprintf """{"jsonrpc":"2.0","id":%d,"result":{"content":[{"type":"text","text":"%s"}]}}""" id result
                                             | Error e ->
                                                 sprintf """{"jsonrpc":"2.0","id":%d,"error":{"code":-1,"message":"%s"}}""" id e)
                                        | _ ->
                                            sprintf """{"jsonrpc":"2.0","id":%d,"error":{"code":-32601,"message":"Tool not found"}}""" id
                                    else
                                        sprintf """{"jsonrpc":"2.0","id":%d,"error":{"code":-32602,"message":"Missing arguments"}}""" id
                                else
                                    sprintf """{"jsonrpc":"2.0","id":%d,"error":{"code":-32602,"message":"Missing params"}}""" id
                            
                            | _ ->
                                sprintf """{"jsonrpc":"2.0","id":%d,"error":{"code":-32601,"message":"Method not found"}}""" id
                        
                        stdout.WriteLine(response)
                        stdout.Flush()
                        processInput()
                with ex ->
                    let errorMsg = sprintf """{"jsonrpc":"2.0","error":{"code":-32700,"message":"Parse error: %s"}}""" (ex.Message.Replace("\"", "\\\""))
                    stdout.WriteLine(errorMsg)
                    stdout.Flush()
                    processInput()
        with _ -> ()
    
    processInput()

[<EntryPoint>]
let main args =
    ensureDirectories()
    
    match args with
    | [| "mcp" |] -> 
        runMcpServer()
        0
    | [| |] | [| "list" |] -> listAvailableInstructions(); 0
    | [| "show"; dayStr |] when Int32.TryParse(dayStr) |> fst -> showDay (Int32.Parse(dayStr)); 0
    | [| "speed"; dayStr; phaseStr; llm; speedStr |] when Int32.TryParse(dayStr) |> fst && Int32.TryParse(phaseStr) |> fst && Int64.TryParse(speedStr) |> fst ->
        recordSpeedCli (Int32.Parse(dayStr)) (Int32.Parse(phaseStr)) llm (Int64.Parse(speedStr)); 0
    | _ ->
        printfn "AoC MCP Server - Advent of Code Instruction & Speed Tracker"
        printfn ""
        printfn "Usage:"
        printfn "  aocMCP [list]                                    - List available instruction days"
        printfn "  aocMCP show <day>                                - Show instructions for a day"
        printfn "  aocMCP speed <day> <phase> <llm> <speed_ms>    - Record solving speed"
        printfn "  aocMCP mcp                                       - Start MCP server on stdio"
        printfn ""
        printfn "Instructions are stored in: %s" instructionsDir
        printfn "Speed records are stored in: %s" speedsDir
        0
