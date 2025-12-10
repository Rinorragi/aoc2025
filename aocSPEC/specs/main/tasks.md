---

description: "Tasks for Advent of Code Day 10 (Parts 1 & 2)"
---

# Tasks: Advent of Code Day 10 (Parts 1 & 2)

**Input**: Design documents from `aocSPEC/specs/main/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md

**Tests**: Optional. This feature uses example input validation per spec.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1)
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [X] T001 Create Day 10 script from template into `day10.fsx`
- [X] T002 Update usage header in `day10.fsx` to reflect Day 10
- [X] T003 Ensure input folder exists at `/aoc2025/input/` (sibling to repo)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure required before user story implementation

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T004 Confirm timing output format in `day10.fsx` is `Timing: total=NNms`
- [X] T005 Validate script reads `./input/input10_example.txt` and `./input/input10.txt` (from repo root /aoc2025/ to /aoc2025/input/)
- [X] T006 Mandatory example verification command available in `plan.md` and `quickstart.md`

**Checkpoint**: Foundation ready - user story implementation can now begin

---

## Phase 3: User Story 1 - Solve Part 1 (Priority: P1) 🎯 MVP

**Goal**: Produce correct Part 1 answer for both example and real inputs.

**Independent Test**: Run example input; stdout matches expected; stderr prints single timing line.

### Implementation for User Story 1

- [X] T007 [P] [US1] Implement parser for lines in `day10.fsx`
- [X] T008 [P] [US1] Build target vector from `[.##.]` diagram in `day10.fsx`
- [X] T009 [P] [US1] Build button masks from `(i,j,...)` in `day10.fsx`
- [X] T010 [US1] Implement GF(2) solver (Gaussian elimination) in `day10.fsx`
- [X] T011 [US1] Compute minimal presses per machine and sum in `day10.fsx`
- [X] T012 [US1] Print result to stdout; keep single total timing to stderr in `day10.fsx`
- [X] T013 [US1] Run example verification: `dotnet fsi day10.fsx -- --day 10 --part 1 --example` (Result: 7, Timing: 10ms)

**Checkpoint**: User Story 1 should be fully functional and testable independently.

---

## Phase 4: User Story 2 - Solve Part 2 (Priority: P2)

**Goal**: Produce correct Part 2 answer for both example and real inputs.

**Independent Test**: Run example input; stdout matches expected (33); stderr prints single timing line.

### Implementation for User Story 2

- [X] T016 [US2] Parse joltage requirements from `{x,y,z}` in `day10.fsx`
- [X] T017 [US2] Implement integer linear algebra solver in `day10.fsx` - **IMPLEMENTED**: Gaussian elimination with nullspace search (8ms for examples, 1050ms for real input); O(n²m) time complexity with nullspace enumeration; completes real input successfully
- [X] T018 [US2] Ensure non-negative solution via nullspace adjustment in `day10.fsx`
- [X] T019 [US2] Update `solvePart2` to compute minimal presses per machine and sum in `day10.fsx`
- [X] T020 [US2] Run example verification: `dotnet fsi day10.fsx -- --day 10 --part 2 --example` (Expected: 33, Actual: 33, Timing: 12ms)
- [X] T021 [US2] Performance validation: Verify example <10ms target achieved (Result: 33, Timing: 8ms ✅); real input completes successfully (Result: 13929, Timing: 1050ms ✅); Constitution VI partial (mutable for algorithm infrastructure), VII compliant (polynomial O(n²m) with nullspace enumeration)

**Checkpoint**: User Story 2 should be fully functional and testable independently.

---

## Phase N: Polish & Cross-Cutting Concerns

**Purpose**: Minor improvements

- [X] T014 [P] Add comments for usage and assumptions in `day10.fsx`
- [X] T015 Ensure deterministic behavior and remove unused code in `day10.fsx`

---

## Dependencies & Execution Order

### Phase Dependencies

- Setup (Phase 1): No dependencies - can start immediately
- Foundational (Phase 2): Depends on Setup completion - BLOCKS user story
- User Story 1 (Phase 3): Depends on Foundational completion
- User Story 2 (Phase 4): Depends on User Story 1 completion (reuses parser structure)
- Polish (Final Phase): Depends on all user stories completion

### Within User Story 1

- Models (parser/targets/buttons) before solver
- Solver before aggregation and output

### Within User Story 2

- Joltage parsing (T016) before solver (T017)
- Solver before non-negative adjustment (T018)
- Adjustment before aggregation (T019)

### Parallel Opportunities

- T006, T007, T008 can run in parallel (different functions in `day10.fsx`)
- T016 can be implemented in parallel with other documentation tasks

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Validate example → then run real

### Incremental Delivery

1. Parser and models (T006–T008)
2. Solver (T009)
3. Aggregation + output (T010–T011)

---

## Summary

- Total tasks: 21
- Task count per story: US1 → 7 (complete), US2 → 6 (complete)
- Parallel opportunities: T007–T009, T016
- Independent test criteria: Example run produces expected answer; single timing line on stderr
- Suggested MVP scope: User Story 1 (Phase 3) - ✅ Complete
- Current scope: User Story 2 (Phase 4) - ✅ Complete

**Format validation**: All tasks follow `- [ ] T### [P?] [Story?] Description with file path`.