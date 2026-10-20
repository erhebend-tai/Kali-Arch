#!/usr/bin/env bash
# bootstrap.sh — one-command platform setup for new contributors.
set -euo pipefail

cd "$(dirname "$0")"

echo "=== CleanArch bootstrap ==="

echo "→ Checking prerequisites..."
command -v dotnet >/dev/null || { echo "dotnet not found"; exit 1; }
command -v go     >/dev/null || echo "⚠ go not found — Go services skipped"
command -v node   >/dev/null || echo "⚠ node not found — workers skipped"
command -v make   >/dev/null || echo "⚠ make not found — use build/scripts directly"

echo "→ Restoring .NET tools + packages..."
dotnet tool restore
dotnet restore CleanArch.slnx

echo "→ Enabling git hooks..."
git config core.hooksPath .githooks 2>/dev/null || true

echo "→ Go modules..."
for d in cf-microservices/*/; do
    if [ -f "$d/go.mod" ]; then
        (cd "$d" && go mod download && go vet ./...)
    fi
done

echo "→ TypeScript workers..."
for d in workers/cf-workers/*/; do
    if [ -f "$d/package.json" ]; then
        (cd "$d" && npm ci --silent 2>/dev/null || npm install --silent)
    fi
done

echo "→ Building solution..."
dotnet build CleanArch.slnx --no-restore -v q

echo "→ Running unit tests..."
dotnet test tests/CleanArch.Domain.Tests --no-build -v q
dotnet test tests/CleanArch.Application.Tests --no-build -v q

echo ""
echo "=== Bootstrap complete ==="
echo "Next:"
echo "  make build    — full rebuild"
echo "  make test     — all tests"
echo "  make dapr     — local run with Dapr"
echo "  dotnet run --project src/CleanArch.API"
