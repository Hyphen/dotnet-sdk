#!/usr/bin/env pwsh
#Requires -Version 5.1

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ($null -eq (Get-Command "dotnet" -ErrorAction Ignore)) {
	throw "Could not find 'dotnet'; please install the .NET SDK from https://dot.net/"
}

dotnet restore
dotnet build --no-restore --configuration Release
dotnet test --no-build --configuration Release
