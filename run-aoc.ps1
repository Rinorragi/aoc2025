# Generic wrapper script for running AoC solutions with timeout
param(
    [Parameter(Mandatory=$true)]
    [string]$Day,
    
    [Parameter(Mandatory=$true)]
    [int]$Part,
    
    [switch]$Example,
    
    [int]$TimeoutSeconds = 300  # 5 minute default timeout
)

$scriptPath = Join-Path $PSScriptRoot "day$Day.fsx"

if (-not (Test-Path $scriptPath)) {
    Write-Error "Script not found: $scriptPath"
    exit 1
}

$scriptArgs = @("--", "--day", $Day, "--part", $Part)
if ($Example) {
    $scriptArgs += "--example"
}

Write-Host "Running: dotnet fsi day$Day.fsx $($scriptArgs -join ' ')" -ForegroundColor Cyan
Write-Host "Timeout: $TimeoutSeconds seconds" -ForegroundColor Gray

$job = Start-Job -ScriptBlock {
    param($scriptPath, $arguments)
    Set-Location (Split-Path $scriptPath)
    dotnet fsi (Split-Path $scriptPath -Leaf) @arguments 2>&1
} -ArgumentList $scriptPath, $scriptArgs

$completed = Wait-Job -Job $job -Timeout $TimeoutSeconds

if ($completed) {
    $output = Receive-Job $job
    Remove-Job $job
    Write-Output $output
    exit 0
} else {
    Stop-Job $job
    Remove-Job $job
    Write-Error "TIMEOUT: Execution exceeded $TimeoutSeconds seconds"
    exit 1
}
