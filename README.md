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