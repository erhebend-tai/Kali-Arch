# CAKE bootstrapper (Windows): .\build.ps1 -Target Default
param([string]$Target = "Default")
dotnet tool list -g | Select-String "cake.tool" | Out-Null
if ($LASTEXITCODE -ne 0) { dotnet tool install -g Cake.Tool }
dotnet cake "$PSScriptRoot/build.cake" --target=$Target $args
