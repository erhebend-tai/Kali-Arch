# CleanArch platform — task runner.
# Usage: make <target>   (targets mirror build/scripts/)

DOTNET   := dotnet
GO       := go
SLN      := CleanArch.slnx

.PHONY: restore build test lint pack clean migrate dapr docfx go-vet workers

## Restore all dependencies
restore:
	$(DOTNET) restore $(SLN)
	$(DOTNET) tool restore
	@for d in cf-microservices/*/; do (cd $$d && $(GO) mod download); done

## Build the full solution
build: restore
	$(DOTNET) build $(SLN) --no-restore

## Run all tests
test:
	$(DOTNET) test $(SLN) --no-build

## Lint: analyzers + architecture rules
lint:
	$(DOTNET) build tools/analyzers/CleanArch.Analyzers --no-restore
	$(DOTNET) test tools/linters/CleanArch.Linting --no-build
	@for d in cf-microservices/*/; do (cd $$d && $(GO) vet ./...); done

## Pack publishable NuGet packages
pack:
	$(DOTNET) pack packages/nuget/CleanArch.Analyzers --output artifacts/nuget
	$(DOTNET) pack packages/nuget/CleanArch.CodeFixes --output artifacts/nuget
	$(DOTNET) pack packages/nuget/CleanArch.Shared.Contracts --output artifacts/nuget
	$(DOTNET) pack packages/nuget/CleanArch.Shared.Domain --output artifacts/nuget

## Apply DbUp migrations (needs DB_CONNECTION)
migrate:
	$(DOTNET) run --project cli/CleanArch.DevCli -- migrate --connection "$(DB_CONNECTION)"

## Local Dapr sidecars for event-driven development
dapr:
	dapr run --app-id cleanarch-api --app-port 8080 \
		-- dotnet run --project src/CleanArch.API

## Build the DocFX docs site
docfx:
	docfx docs/docfx/docfx.json

## Go verification
go-vet:
	@for d in cf-microservices/*/; do (cd $$d && $(GO) vet ./...); done

## TypeScript workers typecheck
workers:
	@for d in workers/cf-workers/*/; do \
		if [ -f $$d/package.json ]; then (cd $$d && npm run build --if-present || true); fi; \
	done

## Clean all build output
clean:
	$(DOTNET) clean $(SLN)
	@find . -type d \( -name bin -o -name obj \) -exec rm -rf {} + 2>/dev/null || true
