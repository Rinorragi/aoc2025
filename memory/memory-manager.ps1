param(
    [Parameter(Mandatory=$true)]
    [ValidateSet('store', 'search', 'get', 'list')]
    [string]$Action,
    
    [string]$Title,
    [string]$Content,
    [string]$Id,
    [string]$Query
)

$memoryPath = Split-Path -Parent $PSCommandPath

function New-Memory {
    param([string]$Title, [string]$Content)
    
    $id = [guid]::NewGuid().ToString()
    $memory = @{
        id = $id
        title = $Title
        content = $Content
        timestamp = (Get-Date -Format "o")
    }
    
    $filePath = Join-Path $memoryPath "$id.json"
    $memory | ConvertTo-Json -Depth 10 | Set-Content $filePath
    
    Write-Host "Memory stored with ID: $id"
    return $id
}

function Search-Memories {
    param([string]$Query)
    
    $memories = Get-ChildItem -Path $memoryPath -Filter "*.json" | ForEach-Object {
        Get-Content $_.FullName | ConvertFrom-Json
    }
    
    $results = $memories | Where-Object {
        $_.title -like "*$Query*" -or $_.content -like "*$Query*"
    } | Select-Object id, title, timestamp | Sort-Object timestamp -Descending
    
    if ($results) {
        $results | Format-Table -AutoSize
    } else {
        Write-Host "No memories found matching: $Query"
    }
}

function Get-Memory {
    param([string]$Id)
    
    $filePath = Join-Path $memoryPath "$Id.json"
    if (Test-Path $filePath) {
        $memory = Get-Content $filePath | ConvertFrom-Json
        Write-Host "`nID: $($memory.id)"
        Write-Host "Title: $($memory.title)"
        Write-Host "Timestamp: $($memory.timestamp)"
        Write-Host "`nContent:"
        Write-Host $memory.content
    } else {
        Write-Host "Memory not found: $Id"
    }
}

function Get-AllMemories {
    $memories = Get-ChildItem -Path $memoryPath -Filter "*.json" | ForEach-Object {
        Get-Content $_.FullName | ConvertFrom-Json
    } | Select-Object id, title, timestamp | Sort-Object timestamp -Descending
    
    if ($memories) {
        $memories | Format-Table -AutoSize
    } else {
        Write-Host "No memories stored yet."
    }
}

switch ($Action) {
    'store' {
        if (-not $Title -or -not $Content) {
            Write-Error "Title and Content are required for storing a memory."
            exit 1
        }
        New-Memory -Title $Title -Content $Content
    }
    'search' {
        if (-not $Query) {
            Write-Error "Query is required for searching memories."
            exit 1
        }
        Search-Memories -Query $Query
    }
    'get' {
        if (-not $Id) {
            Write-Error "Id is required for retrieving a memory."
            exit 1
        }
        Get-Memory -Id $Id
    }
    'list' {
        Get-AllMemories
    }
}
