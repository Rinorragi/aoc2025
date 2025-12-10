# Implementation Plan: Advent of Code Day 10 (Parts 1 & 2)

**Branch**: `main` | **Date**: 2025-12-10 | **Spec**: aocSPEC/specs/main/spec.md
**Input**: Puzzle instructions fetched via speckit MCP for Day 10, Phases 1 & 2

## Summary

Solve Day 10 Parts 1 & 2 using F# fsi script `day10.fsx` (at repo root), reading inputs from `./input/input10_example.txt` and `./input/input10.txt` (input folder at /aoc2025/input/ inside repo). Keep text I/O and single total timing per run. Always verify the script against example input before running the real input.

**Part 1**: Binary configuration problem (GF(2)) - minimize button presses to configure indicator lights.
**Part 2**: Integer linear programming - minimize button presses to configure joltage counters.

## Technical Context

**Language/Version**: F# (fsi) - dotnet SDK
**Primary Dependencies**: None (standard library only)
**Storage**: N/A
**Testing**: Lightweight assertions via `--example` runs
**Target Platform**: Windows + PowerShell (`pwsh`)
**Project Type**: single-script per day
**Performance Goals**: Single-run total time reported; aim < 200ms for examples (✅ Part 1: 6ms, Part 2: 8ms); real input (✅ Part 1: 8ms, Part 2: 1050ms)
**Constraints**: Deterministic output; no external MCP from script; immutable functional style
**Scale/Scope**: Single-day, parts 1 & 2

### Part 1 Algorithm
- Model as GF(2) linear system: A·x = b (mod 2)
- Gaussian elimination to find solution space
- Search nullspace for minimal Hamming weight

### Part 2 Algorithm
**Current Implementation** (Gaussian Elimination with Nullspace Search):
- Model as integer linear system: A·x = b (x ≥ 0, integers)
- Gaussian elimination with int64 arithmetic (prevents overflow)
- Identify free variables (underdetermined system)
- Enumerate nullspace to find minimal non-negative solution
- Time complexity: O(n²m) Gaussian + O(maxFreeValue^k) nullspace where k = free variables
- Successfully solves real input: 13929 in 1050ms
- Constitution compliant: Principle VII satisfied (polynomial base with bounded enumeration)

## Constitution Check

- Script-First: PASS (fsx template)
- Text I/O: PASS (stdin/files → stdout; errors → stderr)
- Reproducible Runs: PASS (args + fixed paths)
- Minimal Dependencies: PASS
- Testability (Lightweight): PASS (example run)
- Functional Programming (Immutable): PARTIAL (Part 1: PASS; Part 2: mutable variables for Gaussian elimination algorithm, immutable data structures for results)
- Performance Awareness: PASS (Part 1: O(n²m) Gaussian elimination over GF(2); Part 2: O(n²m) Gaussian elimination with bounded nullspace search; both complete in reasonable time)

## Project Structure

### Documentation (this feature)

```text
aocSPEC/specs/main/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── tasks.md             # Phase 1 output
└── spec.md              # Feature spec with Parts 1 & 2
```

### Source Code (repository root)

```text
.
├── aocSPEC/
│   └── templates/day-template.fsx
└── day10.fsx

/aoc2025/input/:
├── input10_example.txt
└── input10.txt
```

**Structure Decision**: Single project with per-day `.fsx` script and `./input/` folder at repo root.

## Complexity Tracking

N/A

## Verification

- Mandatory: Run example verification prior to real run.
    - Part 1 Command: `dotnet fsi day10.fsx -- --day 10 --part 1 --example` (from /aoc2025/ directory)
    - Part 1 Expected: stdout = `7`; stderr includes `Timing: total=NNms`.
    - Part 2 Command: `dotnet fsi day10.fsx -- --day 10 --part 2 --example` (from /aoc2025/ directory)
    - Part 2 Expected: stdout = `33`; stderr includes `Timing: total=NNms`.
