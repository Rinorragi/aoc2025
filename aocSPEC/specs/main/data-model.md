# Data Model

## Part 1: Binary Configuration (Indicator Lights)

- Machine
  - lights: int (count)
  - target: bool list (from [.##.] etc.)
  - buttons: list<bool list> (each button mask toggles indices)

- Parsed Line → Machine
  - diagram: string inside [ ] → target (binary state)
  - buttons: sequences inside ( ) as comma-separated indices → toggle mask
  - joltage: ignored for Part 1

Relationships
- Each button mask length equals lights; true at toggled indices
- Solution: binary vector (0/1 presses per button)

## Part 2: Integer Configuration (Joltage Counters)

- Machine
  - counters: int (count of joltage requirements)
  - targetJoltage: int list (from {3,5,4,7} etc.)
  - buttons: list<int list> (each button increments counter indices)

- Parsed Line → Machine
  - diagram: ignored for Part 2
  - buttons: sequences inside ( ) as comma-separated indices → increment mask
  - joltage: string inside { } → target values

Relationships
- Each button mask length equals counters; 1 at affected indices, 0 elsewhere
- Solution: non-negative integer vector (N≥0 presses per button)
- Constraint: sum of (button_i presses × button_i mask) = target joltage
