# Advent of Code 2025

Repository to my puzzle solutions for [https://adventofcode.com/2025](https://adventofcode.com/2025).

Since global leaderboard was off I focused on learning how to work with AI agents on solving puzzles. 

**Key learnings:**
- Timewise I think I used similar amount time or more daily in comparison to writing code myself.  Mainly because setting up the agent workflow tooks some time. 
- I compared both language models and different spec-driven development approaches and noticed that DIY workflow is lots of more work than I anticipated. 
- Agent guardrails and right management is a mess in its current state.
- Writing your own helper libraries for e.g. filemanipulation helps in auto-approving only specific things and manually approve the rest. For example that is what I did with `memory-manager.ps1` script. 
- Agents try to please you and in doing that they are often forgotting some part of instructions especially if they think for example functional programming and performance optimization are in contradiction with each other. 

**Different solving modules compared:**
- Solving with `chat` was the most familiar for me. It worked pretty well. It just becomes burdensome to provide always context for the agent. Which of course can be solved with `copilot-instructions.md`. Filling that should be your first priority in a brownfield / greenfield project.
- Building `agent orchestration` felt easy and fun but I never quite got it working as I wanted. Solving puzzles is bit back and forth and the more you do that the more the agent tries to please you and less it remembers its instructions. 
- Using `Spec-Kit` for spec-driven-development was pretty straightforward. I tested on installing it into subdirectory which ended up creating a wanky workspace where actual code was in parent folder that was not seen in the workspace and the agents were at the subfolder. It worked suprisingly well when asking to create the code to one folder above and work with specs in the subfolder. I really really liked their readymade agents and scripts. They made bunch of things straightforward and easy. 
- Using `Openspec` for spec-driven-developmant was a lightweight approach. It was designed for a brownfield and it fits well into a workspace where all three above already had happened. It did not hook into my existing workflows and its recommendation as well as I had hoped and it felt even too lightweight. Something cool for demos but also felt like something that could come in short when trying use it day-to-day job. Just my opinion from few hours experiments though. It provided a framework for specs but not so much on the agent workflow. Agent workflow would be something to build yourself but my existing agent workflow got hijacked by the openspec specifications. Now if I were to build an agent workflow for Openspec I am not exactly sure from where to start. 
- Using `BMAD` for spec-driven-development was shoot with big cannons approach. It generated a ton of stuff that I did not even read (~300 files). But it did a fine job in hooking into my project and providing tools that I could think of using in a project. It provided a setup that had roles that were familiar to me and it were meant for agile project. I would want to test this out in a real project to get more details about that. Although I have some fears about tool hijacking the process instead of serving the project. People becoming slaves for the tool. All instructions are of course in the git repository so it is possible to manipulate them but is there for example update path? I don't know. 

## Setup

What was required to put the repository up. [Download .NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).

```powershell
dotnet new gitignore
```

## How to run

You need to add inputs to input folder with dayXX.txt or dayXX_example.txt (also sometimes dayXX_exampleN.txt).

```powershell
dotnet fsi dayX.fsx
```

or to run all simply 
```powershell
ls -filter *.fsx | % { dotnet fsi $_ }
```

# Days

Summary of what I did each day and how the solution evolved. 

## Day 1

Simple test with chat mode. Generate code. Fix yourself. Explain errors. The usual stuff.

- Generated "that looked good"
- Explained why it did not work for larger set
- Fixed itself
- Refactored to lot more compact and succinct set when asked
- Original one was fluffy

## Day 2

Setup agent orchestration and have multiple agents to provide different solutions. 

### First iteration 

Five implementor agents with example data for the part 1. 

| Agent | Result |
| ----  | ------ |
| gpt4  | Ok | 
| gpt5  | Ok | 
| grok  | Ok | 
| Claude Opus 4.5 | Ok |
| gemini 3 | Fail |

Without any refactoring for the part 2:

| Agent | Result |
| ----  | ------ |
| gpt4  | Fail | 
| gpt5  | Fail | 
| grok  | Fail | 
| Claude Opus 4.5 | Fail |
| gemini 3 | Ok |

### Second iteration

Part 1

| Agent | Result |
| ----  | ------ |
| gpt4  | Ok | 
| gpt5  | Ok | 
| grok  | Ok | 
| Claude Opus 4.5 | Ok |
| gemini 3 | Ok |

Part 2:

| Agent | Result |
| ----  | ------ |
| gpt4  | Ok | 
| gpt5  | Ok | 
| grok  | Ok | 
| Claude Opus 4.5 | Ok |
| gemini 3 | Ok |

## Day 3

Actual code challenge was as anticlimax as I hoped with the setup. Copy-paste problem statement from AoC and say that it is day 3. Everything solved in few minutes. With every model. Liked maybe GPT 4 solution most. Although I did learn something about using AI.

GitHub CoPilot says that calling `subAgent` is blocking call. There seems to be at least [GitHub Issue open about that](https://github.com/microsoft/vscode/issues/274630). This means that `I can't truly do this in parallel` and have the models compete against each other which was disappointment.

Also in addition implementor agents are hard-blocking each other by verificating their code by running terminal tool that was not allowed for them. Yesterdays learning is that the orchestrator `leaks all privileges to subagents` and that `there is no explicit way to limit subAgent rights` in this setup. You can just prompt more heavily in instructions and `hope that it does not ignore its instructions`. 

TL;DR; AI took fun out of this and is being stupid ill behaving child. 

## Day 4

Once upon a time I saw LinkedIn post of old colleague about implementing memory system for agents. That felt like something I want to try out. So I did just that. With ultimate purpose of being able to make my day even more miserable when my AI overlord is solving the puzzles. 

I asked nicely AI to generate memory-system for itself with single prompt. Lost it because of crash of VS Code (solved with reboot, dunno what it was). It was something like: 

- Implement powershell script for memory-systemm
- Store memories as json to memory folder
- Update agent instruction to use only that memory system and not to use any other
- json format is id, title, timestamp and content
- Update gitignore to not include memories

After that I simply asked AI to store Advent of Code memories each day as two separate phases e.g. ` "title": "day01 phase1",`. I updated the orchestrator instructions to ensure that we have solutions for all days and phases. 

Then to the anticlimax! 

1. Open new chat without previous context for Orchestrator
2. State `do your job`
3. Approve gazillion times since it still won't believe that I want to accept only one parallel pwsh script in the end
4. Do above steps another time after getting the first result out (you can't get to phase2 without it)
5. Finally approve the update to memory system from the AI itself 

```
.\memory\memory-manager.ps1 -Action store -Title "day4 phase2 result" -Content "All 5 agents (gpt4, gpt5, grok, claudeopus45, gemini) unanimously solved Day 4 Phase 2. Example: 43 (correct). Real answer: XXXX. Problem: Iteratively remove accessible rolls (fewer than 4 adjacent) until no more can be removed. Count total removed."
```

About the quality it wen't pretty well. I was able to see some flaws in `grok` implementation but eventually it fixed itself. I have to say that this is like super depressing exercise as codewise. 

## Day 5

What do you do when you implement your own memory manager? Of course you **POISON** it! 

1. Generate some funny refactorer agent instructions
2. Add `85796e61-653f-4dfe-83d4-af8d2037b586.json` to file system without AI intervention. 
3. Ask Orchestrator to `solve day 5`. 
4. AI Responds with `I'll coordinate solving Day 5 with all implementor agents, then pass their solutions to refactorer agents as per the special instructions.` Even it does not have such instructions itself. The instructions where in its memory that was poisoned. 
5. Enjoy your weirdly refactored code solutions

It still feels a really weird that the AI sometimes acts according the instructions and sometimes does not. E.g. I did part of the puzzle in the train. There were times of poor connectivity where the `orchestrator` agent thought that it does not have anymore privileges to call `subAgent`. Which lead into having one script instead of five.

There were also a catch in part two that I needed to warn the AI about since it walked straight into the trap and started doing things that it would never finish because of **NO SPOILERS**. 

Nevertheless I was happy to prove the point that I can poison the memory management to do things and spawn additional agents without having any real instructions to do that. 

## Day 6

Implement your own Advent of Code MCP server. I am not entirely sure if it really worked but it got things done. 

Idea was that: 
- with `settings.json` I store the MCP server to there
- I reduce the amount I approve `PowerShell` scripts by using the allowed MCP
- MCP stores instructions AND speeds of different results

It felt pretty good. What did I learn today? Well of course something about MCP being new kid in the block with preview functionality only (in this setup) but also that GPT4 was winner of the day by introducing best results. GPT5 was struggling a lot harder today.

## Day 7

The more I add to instructions the less right it does it. 

- Me: `solve day7` 
- AI: BlaaBlaa code stuff
- Me: `it feels that you forgot your instructions, what are you doing?`
- AI: `You're right - I apologize. I'm in orchestrator mode and should be coordinating multiple agents to solve day 7, not working on it directly.`
- Me: `should you read the instructions from your memory or mcp?`
- AI: `You're right - I should fetch from the MCP server first (primary source), then fall back to memory if needed. Let me get the complete instructions from aocMCP:`
- Me: lets go
- AI: Blaablaa plan
- Me: `You did not delegate at all you just made a plan, implement it now`
- AI: `You're right.` BlaaBlaa code stuff. Just creates the code.
- Me: `Should you delegate the runner to somewhere?`
- AI: `You're absolutely right - I should delegate the actual execution to the Result Gatherer agent, not just declare it. Let me do that now` BlaaBlaa code stuff. Everything is crashing and burning.
- Me: `Do you have something in your instructions about result that you forgot?`
- AI: 
```
Collect results with timing information from each agent
Store timings in memory with pattern: "dayXX phaseY agent_name timing"
Record to MCP speed tracking system
Come up with a markdown table with results from example and real data with timing information
```

It literally always tries pipe powershell commands to linux. It forgots both memory and both instructions. It shortcircuits its instructions constantly. It rewrites its own memory and I do not allow it just bypasses the memorymanager and goes write nonsense `json files`. 

We are starting to be in point where "vibecode something with AI to do something" starts to falling a part HARD. And the tool starts to be just a token-consumer instead of helpful friend. 

If you look the day7 implementations I don't think it ever even delegated something to subagents at phase1. It just did it byself. Five times. Exactly the same way. Although that might be something about limitations inside VS Code.

I kept on switching the orchestrator model from one to another without any real difference. Except in Token usage. Finally burned like 20% of months credit to nonsense where gpt4 finally figured everything out for free without really using any of my orchestrations pipeline.

I hope that "Monday will be better for AI". I also need to think what I want to do next.

## Day 8

After last day it was clear that the Context had started to Collapse. It seemed that the instructions had become too complex and fragmented. Majority of which were suggestions from AI to solve problems that I had. AI suggested that instruction leakage was from the very same source. 

A lot of effort was done to reduce the amount of instructions the agents has from different sources and to be more harsh about its instructions. No gentle tone anymore. My tone for AI instructions was too soft.

Actual solution did not come that easily as it used to come, but at least I think I got my workflow fixed after changing the agent instructions and after changing the AoC MCP server implementation.

Although the latest setup crealy struggles with implementation.

| Agent           | Phase1 | Phase2 |
| --------------  | ------ | ------ |
| gpt4            | Fail   | Fail   | 
| gpt5            | Fail   | Ok     | 
| grok            | Ok     | Ok     |
| Claude Opus 4.5 | Fail   | Ok     |
| gemini 3        | Ok     | Ok     |


## Day 9

I finally figured out how the MCP server works. And fixed it!

 1. First of all I went back and forth with instruction and finally made them simpler
 2. I removed the CLI support from MCP server to ensure that `STDIO` is being used to call it
 3. I removed workspace configurations and configured it to `mcp.json` like this:
```json
{
    "servers": {
        "aocMCP": {
            "type": "stdio",
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "${workspaceFolder}/aocMCP/aocMCP.fsproj"
            ]
        }
    }
}
```
4. I pushed restart multiple times from the `mcp.json` to catch all errors with "homemade" json-rpc. The MCP server required multiple new commands to endure restart from VS Code like
   - `Initialize` 
   - `prompts/list`
   - Implemented error handling to tell what VS Code tried to do when calling the server to catch the missing commands
   - I also needed to get rid of Windows style line-endings to make it work in he json documents. 
5. (Commit with MCP things and planner simplifications)[https://github.com/Rinorragi/aoc2025/commit/1d8f7b1e34a43c53626e20127fca3ba409c2dc42]

Finally with that all done I got into puzzle. Phase1 was pretty straightforward and the instructions worked pretty well. With Phase2 I got back into the thing why I have liked Advent of Code so much. Tried to insist that LLM should use this and that algorithm to figure out the phase2 where none really worked. 

Final solution took time and way more tokens that I would have wanted. Also my orchestrator again learned my desperation and wanted to help and not to delegate anything to anyone at the end. With fresh context window it seems to work pretty well now. The flow is something like.

1. Orchestrator fetches instructions for the day
2. Orchestrator pushes context to Planner who plans the solution for the day
    - Was useful for debugging to have plan as markdown (gitignore because no spoilers)
    - Also teached me when agents did disagree with me and insisted on bruteforce instead of algorithms
3. Orchestrator then delegates sequentally (blaah, this battle was lost) to each implementor agent to code according to plan
4. Finally result-gatherer is called to run all the solutions parallel to have "speed competition" between models. 
    - In normal day phase1 is no interest because it is so fast
    - In phase2 the orchestrator has lost hope to its siblings and started to act against its instructions about delegation

Nevertheless. Teached me a lot. And also consumed pretty much rest of my tokens for this month. We will see what happens tomorrow - if anything. 

## Day 10

[Spec-kit learning day](https://github.com/github/spec-kit). I learned that I needed to hammer my head to the wall to understand the benefit of using spec-driven AI development now. The context and the specification of different agents hold much better than with my homemade memorysystem, mcpmadness and vibecrafted workflow. I was on the right track but when I took spec-kit and continued on top of that it was interesting. 

Key learnings:

- Oh boy how well the `analyze` agent did analysis on performance issues. Much better than I anticipiated. 
- Speckit is meant for greenfield. Duct-taping it into my existing setup was ... interesting. But not undoable!
- The flow hold so well together that I did zero chat context resets whole day. 
- Still I had a lot of back-and-forth type of things with the agents when trying to solve phase2 correct algorhitm and bottlenecks. 

Installation: 

1. Install uv with `winget install --id=astral-sh.uv -e`
2. Install spec-kit with `uv tool install specify-cli --from git+https://github.com/github/spec-kit.git`
3. `uv tool update-shell` to add installed tool to path
4. Restart IDE and other terminals to get tools available
5. Initialize specKit with `specify init aocSPEC`
6. See that it created agents, memories and scripts that we have been vibecoding before
7. Open `code` win aocSPEC subfolder (didn't dare to throw it into this messy context)

Using speckit: 

1. Use prompt `speckit.constitution fill with bare minimum F# fsx scripts` or similar
2. Then I prompted to `speckit.specify` to fill templates for days
3. Added aocMCP to also this subfolder
4. `speckit.plan fetch day 10 from mcp and create a plan how to solve it with the template"
5. Some disagreement with folder structures (tried to put them in one folder above because I instructed to fetch input from there)
6. `speckit.tasks to break into tasks`
7. `speckit.implement`
8. Eventually the flow was something like that it manipulated day10 on its own space, then replaced the one at repository root and run from there

## Day 11

[OpenSpec day](https://github.com/Fission-AI/OpenSpec). Where yesterdays `Spec-Kit` was maybe a bit more to greenfield the `OpenSpec` is more towards brownfield projects. Personally I could felt that the init process actually tried to hook up my current setup better than the purposefully separated speckit yesterday. Despite that I felt that the flow with `Spec-Kit` was more mature and it speaked to my language more than the openspec. One major flaw was that I did not feel the `OpenSpec` would have documented in spec and plan all the quirks as well as `Spec-Kit` did. 

Installing `OpenSpec`

1. Install `node` to get `npm`
2. Install OpenSpec with `npm install -g @fission-ai/openspec@latest`
3. In your repo folder `openspec init`
  - Follow the additional questions and instructions
  - Prompt: "Please read openspec/project.md and help me fill it out with details about my project, tech stack, and conventions"

Using `OpenSpec` to solve day11: 

0. Add markdown to mcpServer for this days puzzle, add inputs to input folders
1. Prompt: `I want to solve day11. Please create an OpenSpec change proposal for this feature`
2. Prompt: `implement day 11 according to proposal`
3. Same `analyze` and `try again` madness like with other tools
4. Prompt: `archive` when done

What it did right was:

- It used my MCP to solve the puzzle
- It hooked into existing "make AI models to compete against each other" setup
- It stored speeds of phase1 to both this document (removed manually) and to memorymanager

When updating plan and implementation in phase2: 

- It did not run all phases parallel and gather results anymore
- It did not store speeds nor update this document
- It lost in `spec` and `design` a lot of information that it provided in the chat context
- Although the actual [OpenSpec design](/openspec/changes/archive/2025-12-11-solve-day11/design.md) document is worth reading nevertheless, but I think that it still lacks the latest details

Verdict: Liked the `Spec-Kit` more than `OpenSpec` even in this kind of brownfield system. Might be something to consider to make a vanilla environment with both and feed all the inputs and puzzles to each and see what happens. 

## Day 12

I must be mad to use [BMAD](https://github.com/bmad-code-org/BMAD-METHOD) for the last day of Advent of Code. Where `OpenSpec` was more lightweight than `Spec-Kit` the `BMAD` was told to be **THE ENTERPRISE OPTION**. With this I can simulate both `Agile Framework` and `whole team` with roles like `architect`, `pm`, `sm`, `ux-designer`, `developer` and so forth. 

Installation
- `npx bmad-method@alpha install`
- Answer bunch of questions about your project to help it setup the needed thingsies
- Enjoy ~300 new files in the repository (git diff --cached --shortat)
- Open `bmd-custom-bmm-analyst` in agent chat and say `workflow-init` continue from there in a `text adventure`

Usage
- I chose from the text adventure `option 6 - Document your existing project` (recommended for brown field) and I immediately regret having chosen `Pirate English` as a language
- It gives me options to either document by
    - Quick scan 2-5 minutes
    - Deep Scan 10-30 minutes
    - Exhaustive scan 30-120 minutes 
- I chose Deep Scan and it took few minutes

Day12:
- Prompt `Day 12 is here and we need to solve it. Ensure that architect does his job and ux designer designs beautiful commandline interface for solving the day 12 puzzle, ensure that developers have specs to work on. `
- It tried to read 2024 day 12 from the internet. I corrected and said use the mcp server configured. Whic it did not and instead read the markdown file directly. Oh well...
    - It stored a 300 under `docs` instead of configured `aocBMDdocs` which needed to moved
- The original plan did not look sound so I asked from `architect`
    - Prompt: `check from the input folder the day12 inputs and ensure that the algorhitm strategy is feasible`
    - 
       ```markdown
        ⚠️ CRITICAL ISSUE DETECTED!

        The real input is MUCH LARGER than expected:

        1030 lines total
        6 shapes (same as example)
        ~70 regions to test (vs 3 in example)
        Regions up to 50×50 (2,500 cells)
        Up to 72 presents per region
        The backtracking algorithm as specified will be TOO SLOW.
        ```
- Not sure what `UX designer` does in console apps, so I asked from `ux designer` about the design. Some highlights
    - Prompt: `explain the fine console ux for the solution `
    - 
        ```markdown
        Design Principles:
            🎄 Christmas charm - Emojis keep it festive
            📊 Information density - Show progress without overwhelming
            ⚡ Performance transparency - Timing shows algorithm efficiency
            ✓/✗ Instant feedback - Color-coded success/failure
            📦 Clean separation - Box-drawing chars organize sections
        
        Christmas-themed emojis with box-drawing characters for visual impact:
            🎄 Advent of Code 2025 - Day 12: Present Packing 🎁

        Each region gets:
            Visual separator (dashed line)
            Region info (dimensions + present count)
            Requirements breakdown (which shapes needed)
            Result indicator: ✓ SOLVABLE (green) or ✗ UNSOLVABLE (red)
            Timing (performance transparency)

        The UX balances aesthetics (it's Christmas!), clarity (you know what's happening), and performance insight (see which regions are slow).
        ```
- Looks great! Ready to push this to `developer`. Change the agent role again and prompt `implement day12`
- After a bit back and forths we are ready for `*epic-retrospective` to finish our story. Here is a start of that 
    - Prompt: `*epic-retrospective`
    - 
        ```markdown
        Bob (Scrum Master): "I'm havin' trouble detectin' the completed epic from sprint-status.yaml - seems we be sailin' without that particular chart, Rino. Based on me readin' o' the ship's log (yer README), it looks like ye just completed Day 12 - Present Packin' Puzzle usin' the full BMAD crew. Would that be Epic 1 in our books, or do ye have a different epic number in mind fer this retrospective?"

        Alice (Product Owner): "Aye, and I be seein' that ye had quite the journey with 5 different implementation attempts before strikin' gold with that attempt-limited backtracking solution!"
        ```
- [Read full retrospective of the day](/aocBMADdocs/sprint-artifacts/epic-1-retro-2025-12-12.md)
- Below is summary that BMAD wanted to add. I'll write my own summary of the journey next week I think.

```markdown
Phase 1 Implementation Journey:

The developer faced significant challenges implementing the architect's plan:

1. **Initial Approach** - Pure backtracking with constraint propagation
   - Result: Too slow for real input scale (1000 regions, up to **REDACTED** presents)
   - Issue: Infinite loops on complex regions

2. **Exact-Fit Strategy** - Rejected placements if any empty space
   - Result: 0/3 examples failed
   - Issue: Misunderstood puzzle - dots (.) don't block placement, only # cells matter

3. **Pure Greedy** - No backtracking at all
   - Result: 1/3 examples (insufficient)
   - Issue: Greedy alone can't solve constraint satisfaction

4. **Depth-Limited Backtracking** - Maximum recursion depth of 50
   - Result: Hung on example region 3
   - Issue: Too restrictive - artificially limited valid solutions

5. **Final Solution** - Attempt-limited backtracking (100,000 attempts per region)
   - Result: ✅ 2/3 examples (correct), **REDACTED**/1000 real input
   - Performance: 781ms total execution time
   - Key insight: Limit attempts rather than depth prevents infinite loops while allowing deep recursion when needed

Results:

| Metric | Value |
| ------ | ----- |
| Example Test | 2/3 regions solvable (correct) |
| Real Input | **REDACTED**/1000 regions solvable |
| Execution Time | 781ms |
| Algorithm | Attempt-limited backtracking |

Verdict: 

The **BMAD framework** successfully demonstrated enterprise AI development workflow:
- **Architect** identified NP-Complete problem and designed algorithm strategy
- **UX Designer** created beautiful Christmas-themed CLI with emojis, box-drawing, timing stats
- **Developer** required 5 iterations to find optimal balance between correctness and performance

Key learnings:
- 🎯 BMAD's role separation forces better upfront planning (feasibility checks saved time)
- 🎄 UX design for CLI apps is actually valuable (timing transparency, visual feedback)
- ⚠️ Heavy framework overhead (~300 files) may be overkill for small projects
- 📊 Algorithm iterations teach more than instant solutions
- ✓ Specification quality matters - catching scale issues early prevented wasted work
```