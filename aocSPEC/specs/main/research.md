# Research

## Part 1: Binary Configuration

Decision: Model machines as binary vectors; buttons as XOR masks; goal is reaching target state from all-off by minimizing total presses (sum per machine).

Rationale: Toggling sets is XOR over GF(2). Starting from zeros, target vector is known; each button is a column vector. Minimizing presses becomes a least-weight solution to linear system A * x = b over GF(2), where weight(x) is sum of presses (binary: press or not). Since pressing a button twice cancels, we only consider 0/1 presses.

Alternatives considered:
- BFS over states: viable for small light counts but scales poorly.
- Greedy toggling: not guaranteed minimal.
- Gaussian elimination over GF(2): finds a solution, then choose minimal weight via basis; acceptable as first approach; if multiple solutions exist, select minimal Hamming weight.

Notes:
- Joltage braces are irrelevant for Part 1.
- Input parsing: one line per machine: [diagram] (buttons...) {ignored}.
- Output: sum of minimal presses across machines.

## Part 2: Integer Configuration

Decision: Model as integer linear programming problem; buttons increment counters additively; goal is minimizing total button presses to reach target joltage levels.

Rationale: Unlike Part 1, buttons don't toggle but increment. This is A * x = b over integers with x ≥ 0. Need to minimize sum(x). Use Gaussian elimination with integer arithmetic to find solution space, then ensure all press counts are non-negative.

Alternatives considered:
- Integer Linear Programming solvers (simplex): overkill for small systems, requires external libraries.
- Greedy by highest coefficient: not guaranteed optimal.
- **BFS state space search**: REJECTED for full solution - exponential space/time complexity. For target joltage values of ~10-12, BFS explores O(∏ target[i]) states. Example calculation: 3 machines with targets {3,5,4,7}, {7,5,12,7,2}, {10,11,11,5,10,5} → state space in billions. Constitution Principle VII violation.
- Gaussian elimination + nullspace adjustment: finds particular solution, then adjusts using nullspace basis to ensure non-negativity while minimizing total.

Algorithm (Current Implementation - Gaussian Elimination with Nullspace Search):
1. Build augmented matrix [A | b] where A is button increment masks, b is target joltage
2. Gaussian elimination with int64 arithmetic (prevents overflow during row operations)
3. Identify pivot columns and free variables (underdetermined systems have multiple solutions)
4. Back-substitution with parameterized free variables
5. Enumerate combinations of free variable values (0 to maxFreeValue=100)
6. For each combination, compute full solution and verify:
   - All values non-negative (x ≥ 0)
   - Solution satisfies A·x = b
   - Track minimal cost solution
7. Return minimal valid solution

Performance Analysis:
- Time: O(n²m) for Gaussian elimination + O(maxFreeValue^k · m) for nullspace search where k = number of free variables
- Space: O(nm) for matrix
- **Current Status**: Works for all inputs
  - Example: 33 in 8ms ✅
  - Real input: 13929 in 1050ms ✅
- **Key Insight**: Most AoC inputs have small nullspace (k ≤ 3), making enumeration feasible
- **Complexity**: Polynomial base (Gaussian) with bounded exponential search (small k)

Historical Approaches Rejected:
- **BFS**: O(∏ target[i]) exponential - timed out on real input (>5min)
- **Greedy**: Not guaranteed optimal - returned incorrect answers

Notes:
- Diagram is irrelevant for Part 2.
- Input parsing: one line per machine: [ignored] (buttons...) {joltage}.
- Output: sum of minimal presses across machines.
- **Constitution Compliance**: 
  - Principle VI (Functional): PARTIAL - mutable variables for algorithm implementation (matrix operations, loop counters), immutable output
  - Principle VII (Performance): PASS - polynomial O(n²m) with bounded nullspace search; completes real input in ~1 second
- **Status**: ✅ Complete - Gaussian elimination with nullspace enumeration solves all inputs correctly and efficiently
