# Quickstart: Day 10 Part 1

## Prepare inputs
- Place files in `/aoc2025/input/` (sibling folder to repo):
  - `input10_example.txt`
  - `input10.txt`

## Create script
```powershell
Copy-Item aocSPEC/templates/day-template.fsx -Destination day10.fsx
```

## Run

### Part 1
```powershell
# From /aoc2025/ directory
# Example
dotnet fsi day10.fsx -- --day 10 --part 1 --example

# Real
dotnet fsi day10.fsx -- --day 10 --part 1
```

### Part 2
```powershell
# From /aoc2025/ directory
# Example
dotnet fsi day10.fsx -- --day 10 --part 2 --example

# Real (direct - may timeout on large inputs)
dotnet fsi day10.fsx -- --day 10 --part 2

# Real (with timeout wrapper - recommended)
.\run-aoc.ps1 -Day 10 -Part 2 -TimeoutSeconds 300
```

**Note**: Part 2 uses BFS which may timeout on large inputs (153 machines). Use `run-aoc.ps1` wrapper for automatic timeout handling.

## Expected behavior
- Stdout: single answer line
  - Part 1 example: `7`
  - Part 2 example: `33`
- Stderr: `Timing: total=NNms`
- Part 2 timeout: wrapper exits with error after 300 seconds

## Implement steps inside `day10.fsx`

### Part 1
- Parse `[diagram] (buttons...) {ignored}` per line
- Build target vector (binary) and button masks (binary)
- Solve `A * x = b` over GF(2); minimize presses (Hamming weight of `x`)
- Sum minimal presses across machines and print

### Part 2
- Parse `(buttons...) {joltage}` per line (ignore diagram)
- Build target vector (integers) from joltage requirements
- Build button masks (integer increments)
- Solve `A * x = b` over integers; minimize sum(x) with x ≥ 0
- Sum minimal presses across machines and print
