# Advent of Code 2024

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