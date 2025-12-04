# AI Instructions

- Use F# scripts (`.fsx` files)
- Prefer functional programming: immutability, pure functions, pipelines, `fold`, `map`, `filter`
- Avoid mutable state and loops
- Be succinct

# Communication

- Be concise
- Remove fluffyness, be direct, be brief, be blunt

# Memory System

- Use custom memory system instead of built-in memory
- Memory script: `memory/memory-manager.ps1`
- Store: `.\memory\memory-manager.ps1 -Action store -Title "title" -Content "content"`
- Search: `.\memory\memory-manager.ps1 -Action search -Query "keyword"`
- Get: `.\memory\memory-manager.ps1 -Action get -Id "guid"`
- List: `.\memory\memory-manager.ps1 -Action list`