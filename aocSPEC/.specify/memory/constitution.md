# aoc2025 Constitution
<!--
Sync Impact Report
- Version change: 1.0.0 → 1.1.1
- Modified principles: VII. Performance Awareness - PATCH clarification to include space complexity
- Added sections: None (v1.1.0 added Principle VII)
- Removed sections: Template placeholders removed
- Templates requiring updates:
	✅ `.specify/templates/plan-template.md` reviewed — Constitution Check now includes Performance Awareness
	✅ `.specify/templates/spec-template.md` reviewed — no edits needed
	✅ `.specify/templates/tasks-template.md` reviewed — path conventions remain generic
	✅ `.specify/templates/agent-file-template.md` reviewed — will reflect F# once plans exist
- Follow-up TODOs: TODO(RATIFICATION_DATE): Original adoption date unknown
- Amendment rationale: Clarified that performance includes both time and space complexity; exponential memory growth is as critical as runtime
-->

## Core Principles

### I. Script-First (F# fsi)
All solutions MUST be implemented as F# `fsi` scripts (`.fsx`) without compiled
projects. Each script stands alone with a clear entry point and minimal
dependencies.

Rationale: Ensures fast iteration and zero-build workflow suitable for AoC tasks.

### II. Text I/O Discipline
Scripts MUST read input from files or stdin and write results to stdout. Errors
MUST go to stderr. Output MUST be deterministic for a given input.

Rationale: Text I/O enables easy testing, piping, and reproducibility.

### III. Reproducible Runs
Every script MUST include a tiny runner example or instructions to run via `fsi`
with exact input path(s). Randomness (if any) MUST be seeded.

Rationale: Guarantees that others can execute solutions identically.

### IV. Minimal Dependencies
Prefer standard F# libraries. External packages SHOULD be avoided; if used,
pin versions and document installation steps. No global state beyond the script
file.

Rationale: Keeps environment simple and portable.

### V. Testability (Lightweight)
Where feasible, include lightweight checks (assertions or sample-case
verification) directly in the script guarded behind a `--test` flag or simple
function calls.

Rationale: Encourages correctness without requiring full test frameworks.

### VI. Functional Programming (Immutable)
All solutions MUST use immutable data structures and pure functions. Mutable
variables (`mutable`, `ref`) are PROHIBITED. Prefer recursion, pattern matching,
and higher-order functions (map, fold, filter) over imperative loops with state
mutation.

Rationale: Immutability ensures predictable, testable code with no side effects.

### VII. Performance Awareness (Algorithmic Efficiency)
Advent of Code puzzles typically contain performance pitfalls requiring algorithmic
optimization. Solutions MUST evaluate computational complexity (both time and space)
and avoid brute-force approaches when input scales require efficient algorithms.
Consider exponential growth in memory usage alongside runtime. Profile and optimize
bottlenecks before submitting.

Rationale: Ensures solutions complete in reasonable time without exhausting memory,
demonstrating algorithmic thinking essential for competitive programming.

## Additional Constraints

Environment assumptions MUST be documented: Windows with PowerShell (`pwsh`) and
F# Interactive available. Scripts MUST avoid OS-specific paths where possible
and prefer relative paths for inputs.

## Development Workflow

- Each day/part is a separate `.fsx` script in a clear folder structure.
- Include a header comment with usage: `pwsh` command and expected input file.
- Keep functions pure where practical; isolate parsing and I/O.

## Governance

- This constitution governs implementation standards for AoC F# `fsi` scripts.
- Amendments require documenting changes in this file and updating any impacted
	templates or guidance.
- Versioning policy: MAJOR for principle removals/redefinitions; MINOR for new
	principles or material expansions; PATCH for clarifications.
- Compliance: PRs/reviews MUST verify adherence to Script-First, Text I/O,
	Reproducible Runs, Minimal Dependencies, Testability, Functional Programming,
	and Performance Awareness.

**Version**: 1.1.1 | **Ratified**: TODO(RATIFICATION_DATE): Original adoption date unknown | **Last Amended**: 2025-12-10
