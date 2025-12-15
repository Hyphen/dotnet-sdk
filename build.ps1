#!/usr/bin/env pwsh
#Requires -Version 5.1

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ($null -eq (Get-Command "dotnet" -ErrorAction Ignore)) {
	throw "Could not find 'dotnet'; please install the .NET SDK from https://dot.net/"
}

dotnet tool restore
dotnet restore
dotnet build --no-restore --configuration Release
dotnet test --no-build --configuration Release --coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml
dotnet reportgenerator -reports:**/TestResults/**/coverage.cobertura.xml -targetdir:CoverageReport -reporttypes:"TextSummary" -verbosity:off
Write-Host ""
Write-Host "Code coverage report:" -ForegroundColor DarkGreen
Write-Host ""
Get-Content CoverageReport/Summary.txt
