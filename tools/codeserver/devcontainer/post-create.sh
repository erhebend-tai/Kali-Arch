#!/usr/bin/env bash
set -euo pipefail
dotnet restore CleanArch.slnx
for dir in cf-microservices/*/; do (cd "$dir" && go mod download || true); done
for dir in workers/cf-workers/*/; do (cd "$dir" && npm install || true); done
echo "Dev environment ready."
