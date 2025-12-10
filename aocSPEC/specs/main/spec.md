# Feature Specification: Advent of Code Day 10 (Parts 1 & 2)

**Feature Branch**: `main`
**Created**: 2025-12-10
**Updated**: 2025-12-10 (Added Part 2)
**Status**: Part 1 Complete, Part 2 Complete
**Input**: User description: "Fetch Day 10 Phase 2 via speckit MCP and plan solution"

## User Scenarios & Testing (mandatory)

### User Story 1 - Solve Part 1 (Priority: P1)

Run `day10.fsx` to produce the correct answer for Day 10 Part 1 for both example and real inputs.

**Why this priority**: Unlocks part 2 and validates the pipeline.

**Independent Test**: Compare script output with expected example output from puzzle. Validate deterministic output and single timing stderr line.

**Acceptance Scenarios**:

1. Given example input in `../input/day10_example.txt`, When running `fsi day10.fsx --day 10 --part 1 --example`, Then stdout matches expected example answer.
2. Given real input in `../input/day10.txt`, When running `fsi day10.fsx --day 10 --part 1`, Then stdout prints a single answer and stderr shows `Timing: total=NNms`.

### User Story 2 - Solve Part 2 (Priority: P2)

Run `day10.fsx` to produce the correct answer for Day 10 Part 2 for both example and real inputs.

**Why this priority**: Completes the day's challenge.

**Independent Test**: Compare script output with expected example output (33 for the given example). Validate deterministic output and single timing stderr line.

**Acceptance Scenarios**:

1. Given example input in `./input/input10_example.txt`, When running `dotnet fsi day10.fsx -- --day 10 --part 2 --example`, Then stdout outputs `33`.
2. Given real input in `./input/input10.txt`, When running `dotnet fsi day10.fsx -- --day 10 --part 2`, Then stdout prints a single answer and stderr shows `Timing: total=NNms`.

### Edge Cases

- Missing `./input/input10*.txt` files → script exits with error on stderr.
- Extra whitespace lines in input → parser handles gracefully.
- No feasible solution → script reports error (unlikely with valid AoC inputs).

## Requirements (mandatory)

### Functional Requirements

- FR-001: Script MUST read input from `./input/input10_example.txt` or `./input/input10.txt` based on flag.
- FR-002: Script MUST produce deterministic stdout with the Part 1 or Part 2 answer based on --part flag.
- FR-003: Script MUST emit a single total timing line to stderr.
- FR-004: Script MUST isolate parsing and computation functions.
- FR-005: Script MUST avoid MCP calls inside code (speckit handles instructions).
- FR-006: Script MUST parse joltage requirements from {curly braces} for Part 2.
- FR-007: Script MUST solve integer linear programming problem for Part 2 (minimize button presses).

### Key Entities

- Machine (Part 1): indicator lights (binary state), button toggle masks
- Machine (Part 2): joltage counters (integer values), button increment masks
- Buttons: common to both parts, defined by comma-separated indices in (parentheses)

## Success Criteria (mandatory)

### Measurable Outcomes

- SC-001: Part 1 example run produces answer `7`.
- SC-002: Part 1 real run produces an answer without runtime errors.
- SC-003: Part 2 example run produces answer `33`.
- SC-004: Part 2 real run produces an answer without runtime errors.
- SC-005: Timing line format exactly `Timing: total=NNms`.
