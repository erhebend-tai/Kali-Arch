#!/usr/bin/env bash
# CAKE bootstrapper: ./build.sh --target=Default
set -euo pipefail
dotnet tool list -g | grep -q "cake.tool" || dotnet tool install -g Cake.Tool
dotnet cake "$(dirname "$0")/build.cake" "$@"
