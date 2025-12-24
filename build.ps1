#!/usr/bin/env pwsh
#Requires -Version 5.1

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ($null -eq (Get-Command "dotnet" -ErrorAction Ignore)) {
	throw "Could not find 'dotnet'; please install the .NET SDK from https://dot.net/"
}

dotnet tool restore
if ($LastExitCode -ne 0) { exit $LastExitCode }

dotnet restore
if ($LastExitCode -ne 0) { exit $LastExitCode }

dotnet build --no-restore --configuration Release
if ($LastExitCode -ne 0) { exit $LastExitCode }

dotnet test --no-build --configuration Release --coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml --coverage-settings .runsettings
if ($LastExitCode -ne 0) { exit $LastExitCode }

dotnet reportgenerator -reports:**/TestResults/**/coverage.cobertura.xml -targetdir:CoverageReport -reporttypes:"TextSummary" -verbosity:off
if ($LastExitCode -ne 0) { exit $LastExitCode }

Write-Host ""
Write-Host "Code coverage report:" -ForegroundColor DarkGreen
Write-Host ""
Get-Content CoverageReport/Summary.txt
