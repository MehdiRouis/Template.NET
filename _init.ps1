param(
    [Parameter(Mandatory = $true)]
    [string]$ProjectName
)

Write-Host "Initializing project: $ProjectName"

$root = Get-Location

# Rename directories
Get-ChildItem -Recurse -Directory |
Where-Object { $_.Name -match "Template" } |
Sort-Object FullName -Descending |
ForEach-Object {
    Rename-Item $_.FullName ($_.Name -replace "Template", $ProjectName)
}

# Rename files
Get-ChildItem -Recurse -File |
Where-Object { $_.Name -match "Template" } |
ForEach-Object {
    Rename-Item $_.FullName ($_.Name -replace "Template", $ProjectName)
}

# Replace content
Get-ChildItem -Recurse -Include *.cs,*.csproj,*.sln,*.json |
ForEach-Object {
    (Get-Content $_.FullName) `
        -replace "Template", $ProjectName |
        Set-Content $_.FullName
}

# Generate README.md
$readme = @"
# $ProjectName

Enterprise-ready .NET API generated from the IAM Core Template.

## Stack

- ASP.NET Core
- JWT with centralized session authority
- EF Core
- PostgreSQL / MySQL
- Rate limiting

## Authentication Model

Login → Short-lived JWT  
      → Centralized Session Authority (DB)  
      → Controllers

## Getting Started

1. Configure database in appsettings.json
2. Configure JWT signing key
3. Run migrations
4. Start API

Security-first architecture with real session revocation.
"@

Set-Content -Path "$root/README.md" -Value $readme

Write-Host "Project successfully initialized."
