# Advent of Code 2025

Repository to my puzzle solutions for [https://adventofcode.com/2025](https://adventofcode.com/2025).

Might be that this year I also test some AI stuff. 

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