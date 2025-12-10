# Advent of Code 2025 (F# fsi)

Folder structure and daily script template.

## Structure

```
../input/
  day01_example.txt
  day01.txt
  ...
./templates/
  day-template.fsx
```

## Usage (PowerShell)

```powershell
# Copy template for a day
Copy-Item templates/day-template.fsx -Destination day01.fsx

# Run example
fsi day01.fsx --day 01 --part 1 --example

# Run real input
fsi day01.fsx --day 01 --part 2

## Instructions
- Puzzle instruction fetching and specification are managed by speckit tooling.
- The F# `.fsx` scripts focus solely on reading inputs and producing outputs.
```

## Conventions
- Scripts are standalone `.fsx` and use text I/O.
- Inputs: `input/dayXX_example.txt` and `input/dayXX.txt`.
- Deterministic output for a given input.
- Prefer pure functions; keep parsing and I/O isolated.
