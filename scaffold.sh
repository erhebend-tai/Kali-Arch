#!/usr/bin/env bash
# Scaffold generator: unified Clean Architecture template (monolith + multi-cloud + microservices)
set -e
cd "$(dirname "$0")"

# ── helpers ──────────────────────────────────────────────────────────────────
write_csproj() { # <dir> <name> <sdk> <outtype> <refs...>
  local dir="$1" name="$2" sdk="$3" outtype="$4"; shift 4
  mkdir -p "$dir"
  {
    echo "<Project Sdk=\"$sdk\">"
    echo ""
    echo "  <PropertyGroup>"
    echo "    <TargetFramework>net10.0</TargetFramework>"
    echo "    <ImplicitUsings>enable</ImplicitUsings>"
    echo "    <Nullable>enable</Nullable>"
    [ -n "$outtype" ] && echo "    <OutputType>$outtype</OutputType>"
    echo "  </PropertyGroup>"
    if [ $# -gt 0 ]; then
      echo ""
      echo "  <ItemGroup>"
      for r in "$@"; do
        echo "    <ProjectReference Include=\"$r\" />"
      done
      echo "  </ItemGroup>"
    fi
    echo ""
    echo "</Project>"
  } > "$dir/$name.csproj"
}

lib()  { write_csproj "$1" "$2" "Microsoft.NET.Sdk"     ""        "${@:3}"; }
web()  { write_csproj "$1" "$2" "Microsoft.NET.Sdk.Web" "Library" "${@:3}"; } # Library so it compiles with no Program.cs
test() { write_csproj "$1" "$2" "Microsoft.NET.Sdk"     ""        "${@:3}"; }

# Roslyn components (analyzers/codefixes/generators) must target netstandard2.0
# to load inside the compiler — separate helper, same shape.
analyzer() { # <dir> <name> <refs...>
  local dir="$1" name="$2"; shift 2
  mkdir -p "$dir"
  {
    echo "<Project Sdk=\"Microsoft.NET.Sdk\">"
    echo ""
    echo "  <PropertyGroup>"
    echo "    <TargetFramework>netstandard2.0</TargetFramework>"
    echo "    <LangVersion>latest</LangVersion>"
    echo "    <Nullable>enable</Nullable>"
    echo "    <IsRoslynComponent>true</IsRoslynComponent>"
    echo "  </PropertyGroup>"
    if [ $# -gt 0 ]; then
      echo ""
      echo "  <ItemGroup>"
      for r in "$@"; do
        echo "    <ProjectReference Include=\"$r\" />"
      done
      echo "  </ItemGroup>"
    fi
    echo ""
    echo "</Project>"
  } > "$dir/$name.csproj"
}

# ══ SHARED ═══════════════════════════════════════════════════════════════════
S="shared"

lib "$S/CleanArch.Shared.Domain" "CleanArch.Shared.Domain"
mkdir -p "$S/CleanArch.Shared.Domain"/{ValueObjects,Interfaces}

lib "$S/CleanArch.Shared.Contracts" "CleanArch.Shared.Contracts"
mkdir -p "$S/CleanArch.Shared.Contracts"/{Events/{Compute,Identity,Storage},Commands}

lib "$S/CleanArch.Shared.Infrastructure" "CleanArch.Shared.Infrastructure" \
  "..\\CleanArch.Shared.Domain\\CleanArch.Shared.Domain.csproj" \
  "..\\CleanArch.Shared.Contracts\\CleanArch.Shared.Contracts.csproj"
mkdir -p "$S/CleanArch.Shared.Infrastructure"/{Middleware,Observability,Dapr}

# ══ MONOLITH (src/) ══════════════════════════════════════════════════════════
SD="..\\..\\shared\\CleanArch.Shared.Domain\\CleanArch.Shared.Domain.csproj"
SC="..\\..\\shared\\CleanArch.Shared.Contracts\\CleanArch.Shared.Contracts.csproj"
SI="..\\..\\shared\\CleanArch.Shared.Infrastructure\\CleanArch.Shared.Infrastructure.csproj"

# ── Domain ──
lib "src/CleanArch.Domain" "CleanArch.Domain" "$SD"
mkdir -p src/CleanArch.Domain/Entities/{Common,Compute,Storage,Database,Messaging,Identity,AI,Networking}
mkdir -p src/CleanArch.Domain/ValueObjects/{Common,Compute,Storage,Database,Identity,AI}
mkdir -p src/CleanArch.Domain/Enums/{Compute,Storage,Database,Identity,AI}
mkdir -p src/CleanArch.Domain/Events/{Compute,Storage,Database,Identity}
mkdir -p src/CleanArch.Domain/{Exceptions,Rules/{Compute,Storage,Identity}}
mkdir -p src/CleanArch.Domain/Interfaces/Repositories/{Compute,Storage,Database,Identity}
# Design patterns in Domain (see pattern map):
#   Factories/       entity creation + reconstitution (pure, no SDK)
#   Specifications/  composable business rules → SpecificationEvaluator in Infra
#   Commands/        GoF Command pattern (undo/redo) — NOT MediatR commands
#   Builders/        fluent entity construction
#   Templates/       Template Method — algorithm skeletons
mkdir -p src/CleanArch.Domain/Factories/Compute
mkdir -p src/CleanArch.Domain/Specifications/Compute
mkdir -p src/CleanArch.Domain/Commands/Compute
mkdir -p src/CleanArch.Domain/{Builders,Templates}

# ── Application ──
lib "src/CleanArch.Application" "CleanArch.Application" \
  "..\\CleanArch.Domain\\CleanArch.Domain.csproj" "$SD" "$SC"
mkdir -p src/CleanArch.Application/UseCases/{AI,Compute,Containers,Database,Identity,Storage,Messaging}
mkdir -p src/CleanArch.Application/Interfaces/{AI,Analytics,Compute,Database,DevOps,Identity,Secrets,Storage,Messaging,Observability,Networking}
mkdir -p src/CleanArch.Application/Interfaces/Strategies
mkdir -p src/CleanArch.Application/DTOs/{AI,Compute,Database,Identity,Storage,Messaging}
mkdir -p src/CleanArch.Application/Behaviors
# Design patterns in Application (see pattern map):
#   Commands/Queries/Notifications  MediatR CQRS + domain-event dispatch
#   Behaviors/                      pipeline decorators (log/validate/tx/cache/auth/perf)
#   Pipelines/<d>/Steps/            Step pattern — ordered, compensating workflows
#   Chains/                         Chain of Responsibility handlers
#   Common/                         ICommand/IQuery/IHandler marker interfaces
mkdir -p src/CleanArch.Application/Common
mkdir -p src/CleanArch.Application/Commands/{Compute,Identity,Storage}
mkdir -p src/CleanArch.Application/Queries/{Compute,Identity}
mkdir -p src/CleanArch.Application/Notifications
mkdir -p src/CleanArch.Application/Pipelines/{Compute/Steps,Identity/Steps}
mkdir -p src/CleanArch.Application/Chains/Compute
mkdir -p src/CleanArch.Application/Factories/Compute

# ── Infrastructure.Common ──
lib "src/CleanArch.Infrastructure.Common" "CleanArch.Infrastructure.Common" \
  "..\\CleanArch.Application\\CleanArch.Application.csproj"
mkdir -p src/CleanArch.Infrastructure.Common/{Resilience,Serialization,Caching,Observability}
# Provider-agnostic plumbing shared by all providers:
#   Decorators/   caching/retry/logging wrappers around provider services
#   Outbox/       OutboxMessage + OutboxProcessor (at-least-once delivery)
#   Persistence/  UnitOfWork base + SpecificationEvaluator (Spec → IQueryable)
mkdir -p src/CleanArch.Infrastructure.Common/Decorators/{Storage,Messaging}
mkdir -p src/CleanArch.Infrastructure.Common/{Outbox,Persistence}

# ── Infrastructure providers ──
for p in Azure AWS GCP Alibaba Oracle Cloudflare BareMetal; do
  lib "src/CleanArch.Infrastructure.$p" "CleanArch.Infrastructure.$p" \
    "..\\CleanArch.Application\\CleanArch.Application.csproj" \
    "..\\CleanArch.Infrastructure.Common\\CleanArch.Infrastructure.Common.csproj" \
    "$SC" "$SI"
  mkdir -p "src/CleanArch.Infrastructure.$p"/{AI,Analytics,Compute,Database,DevOps,Identity,Secrets,Storage,Messaging,Networking,Observability}
  mkdir -p "src/CleanArch.Infrastructure.$p"/Persistence/{Repositories,Migrations}
  # Provider-specific pattern implementations:
  #   Strategies/   swappable algorithms (provider selection, pricing)
  #   Factories/    Abstract Factory — provider SDK object creation
  mkdir -p "src/CleanArch.Infrastructure.$p"/Strategies/{ProviderSelection,Pricing}
  mkdir -p "src/CleanArch.Infrastructure.$p"/Factories
  # Cloudflare also manages Cloudflared tunnels (ITunnelService lives in Application/Interfaces/Networking)
  [ "$p" = "Cloudflare" ] && mkdir -p "src/CleanArch.Infrastructure.$p"/{Tunnel,Caching}
done

# ── API ──
web "src/CleanArch.API" "CleanArch.API" \
  "..\\CleanArch.Application\\CleanArch.Application.csproj" \
  "..\\CleanArch.Infrastructure.Common\\CleanArch.Infrastructure.Common.csproj" \
  "..\\CleanArch.Infrastructure.Azure\\CleanArch.Infrastructure.Azure.csproj" \
  "..\\CleanArch.Infrastructure.AWS\\CleanArch.Infrastructure.AWS.csproj" \
  "..\\CleanArch.Infrastructure.GCP\\CleanArch.Infrastructure.GCP.csproj" \
  "..\\CleanArch.Infrastructure.Alibaba\\CleanArch.Infrastructure.Alibaba.csproj" \
  "..\\CleanArch.Infrastructure.Oracle\\CleanArch.Infrastructure.Oracle.csproj" \
  "..\\CleanArch.Infrastructure.Cloudflare\\CleanArch.Infrastructure.Cloudflare.csproj" \
  "..\\CleanArch.Infrastructure.BareMetal\\CleanArch.Infrastructure.BareMetal.csproj" \
  "$SI"
mkdir -p src/CleanArch.API/{Controllers,Middleware,OpenApi/Examples}
cat > src/CleanArch.API/appsettings.json <<'JSON'
{
  "CloudProvider": "Azure",
  "Azure": {},
  "AWS": {},
  "GCP": {},
  "Alibaba": {},
  "Oracle": {},
  "Cloudflare": {},
  "BareMetal": {}
}
JSON

# ── Functions ──
lib "src/CleanArch.Functions" "CleanArch.Functions" \
  "..\\CleanArch.Application\\CleanArch.Application.csproj" \
  "..\\CleanArch.Infrastructure.Azure\\CleanArch.Infrastructure.Azure.csproj" \
  "..\\CleanArch.Infrastructure.AWS\\CleanArch.Infrastructure.AWS.csproj" \
  "..\\CleanArch.Infrastructure.GCP\\CleanArch.Infrastructure.GCP.csproj" \
  "..\\CleanArch.Infrastructure.Alibaba\\CleanArch.Infrastructure.Alibaba.csproj" \
  "..\\CleanArch.Infrastructure.Oracle\\CleanArch.Infrastructure.Oracle.csproj" \
  "..\\CleanArch.Infrastructure.Cloudflare\\CleanArch.Infrastructure.Cloudflare.csproj" \
  "..\\CleanArch.Infrastructure.BareMetal\\CleanArch.Infrastructure.BareMetal.csproj" \
  "$SI"
mkdir -p src/CleanArch.Functions/Triggers/{Http,Timer,Queue,EventHub,Blob,Dapr}
mkdir -p src/CleanArch.Functions/Middleware
echo '{}' > src/CleanArch.Functions/host.json
cat > src/CleanArch.Functions/local.settings.json <<'JSON'
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  }
}
JSON

# ══ MICROSERVICES (services/) ════════════════════════════════════════════════
SVC_DOMAIN="..\\..\\..\\..\\shared\\CleanArch.Shared.Domain\\CleanArch.Shared.Domain.csproj"
SVC_CONTRACTS="..\\..\\..\\..\\shared\\CleanArch.Shared.Contracts\\CleanArch.Shared.Contracts.csproj"
SVC_INFRA="..\\..\\..\\..\\shared\\CleanArch.Shared.Infrastructure\\CleanArch.Shared.Infrastructure.csproj"

declare -A SVC=( [VirtualMachineService]=Compute [IdentityService]=Identity [StorageService]=Storage [AIService]=AI )

for svc in "${!SVC[@]}"; do
  cat="${SVC[$svc]}"
  B="services/$svc/src/$svc"

  lib "$B.Domain" "$svc.Domain" "$SVC_DOMAIN"
  mkdir -p "$B.Domain"/{Entities,ValueObjects,Events,Interfaces}
  # Same Domain pattern set as the monolith (mini Clean Architecture)
  mkdir -p "$B.Domain"/{Factories,Specifications,Commands,Builders,Templates}

  lib "$B.Application" "$svc.Application" \
    "..\\$svc.Domain\\$svc.Domain.csproj" "$SVC_DOMAIN" "$SVC_CONTRACTS"
  mkdir -p "$B.Application"/{UseCases,Interfaces,EventHandlers}
  # MediatR CQRS + behaviors + pipelines + chains (same pattern set)
  mkdir -p "$B.Application"/{Commands,Queries,Notifications,Behaviors,Common,Pipelines,Chains,Factories}
  mkdir -p "$B.Application/Interfaces/Strategies"

  lib "$B.Infrastructure" "$svc.Infrastructure" \
    "..\\$svc.Application\\$svc.Application.csproj" \
    "..\\$svc.Domain\\$svc.Domain.csproj" "$SVC_CONTRACTS" "$SVC_INFRA"
  mkdir -p "$B.Infrastructure"/{$cat,Persistence,Messaging}
  # Pattern plumbing: decorators, strategies, provider factories, outbox
  mkdir -p "$B.Infrastructure"/{Decorators,Strategies,Factories,Outbox}

  web "$B.API" "$svc.API" \
    "..\\$svc.Application\\$svc.Application.csproj" \
    "..\\$svc.Infrastructure\\$svc.Infrastructure.csproj" "$SVC_INFRA"
  mkdir -p "$B.API"/{Controllers,Subscribers}
  echo '{}' > "$B.API/appsettings.json"

  lib "$B.Functions" "$svc.Functions" \
    "..\\$svc.Application\\$svc.Application.csproj" \
    "..\\$svc.Infrastructure\\$svc.Infrastructure.csproj" "$SVC_INFRA"
  mkdir -p "$B.Functions"/Triggers/{Http,Timer,Queue,EventHub,Blob,Dapr}
  mkdir -p "$B.Functions"/Middleware
  echo '{}' > "$B.Functions/host.json"

  # tests — depth is one level deeper than src
  T="services/$svc/tests"
  test "$T/$svc.Domain.Tests"         "$svc.Domain.Tests"         "..\\..\\src\\$svc.Domain\\$svc.Domain.csproj"
  test "$T/$svc.Application.Tests"    "$svc.Application.Tests"    "..\\..\\src\\$svc.Application\\$svc.Application.csproj"
  test "$T/$svc.Infrastructure.Tests" "$svc.Infrastructure.Tests" "..\\..\\src\\$svc.Infrastructure\\$svc.Infrastructure.csproj"

  # Dockerfile placeholder
  cat > "services/$svc/Dockerfile" <<DOCKER
# Build from repo root:
#   docker build -f services/$svc/Dockerfile -t ${svc,,} .
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "services/$svc/src/$svc.API/$svc.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "$svc.API.dll"]
DOCKER
done

# ── Gateway ──
web "services/GatewayService/src/GatewayService.API" "GatewayService.API" "$SVC_INFRA"
mkdir -p services/GatewayService/src/GatewayService.API/OpenApi
cat > services/GatewayService/src/GatewayService.API/yarp.json <<'JSON'
{
  "ReverseProxy": {
    "Routes": {},
    "Clusters": {}
  }
}
JSON
cat > services/GatewayService/Dockerfile <<'DOCKER'
# Build from repo root:
#   docker build -f services/GatewayService/Dockerfile -t gatewayservice .
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "services/GatewayService/src/GatewayService.API/GatewayService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GatewayService.API.dll"]
DOCKER

# ══ MONOLITH TESTS ═══════════════════════════════════════════════════════════
test "tests/CleanArch.Domain.Tests"              "CleanArch.Domain.Tests"              "..\\..\\src\\CleanArch.Domain\\CleanArch.Domain.csproj"
test "tests/CleanArch.Application.Tests"         "CleanArch.Application.Tests"         "..\\..\\src\\CleanArch.Application\\CleanArch.Application.csproj"
test "tests/CleanArch.Infrastructure.Azure.Tests" "CleanArch.Infrastructure.Azure.Tests" "..\\..\\src\\CleanArch.Infrastructure.Azure\\CleanArch.Infrastructure.Azure.csproj"
test "tests/CleanArch.Infrastructure.AWS.Tests"   "CleanArch.Infrastructure.AWS.Tests"   "..\\..\\src\\CleanArch.Infrastructure.AWS\\CleanArch.Infrastructure.AWS.csproj"
test "tests/CleanArch.Infrastructure.GCP.Tests"   "CleanArch.Infrastructure.GCP.Tests"   "..\\..\\src\\CleanArch.Infrastructure.GCP\\CleanArch.Infrastructure.GCP.csproj"
test "tests/CleanArch.Infrastructure.Alibaba.Tests"    "CleanArch.Infrastructure.Alibaba.Tests"    "..\\..\\src\\CleanArch.Infrastructure.Alibaba\\CleanArch.Infrastructure.Alibaba.csproj"
test "tests/CleanArch.Infrastructure.Oracle.Tests"     "CleanArch.Infrastructure.Oracle.Tests"     "..\\..\\src\\CleanArch.Infrastructure.Oracle\\CleanArch.Infrastructure.Oracle.csproj"
test "tests/CleanArch.Infrastructure.Cloudflare.Tests" "CleanArch.Infrastructure.Cloudflare.Tests" "..\\..\\src\\CleanArch.Infrastructure.Cloudflare\\CleanArch.Infrastructure.Cloudflare.csproj"
test "tests/CleanArch.Infrastructure.BareMetal.Tests"  "CleanArch.Infrastructure.BareMetal.Tests"  "..\\..\\src\\CleanArch.Infrastructure.BareMetal\\CleanArch.Infrastructure.BareMetal.csproj"

# ══ DEPLOY ═══════════════════════════════════════════════════════════════════
mkdir -p deploy/{dapr/components,k8s}

# Per-provider Dapr component sets — copy a provider's files up to
# components/ (or point --components-path at the provider dir) to switch.
declare -A DAPR_PUBSUB=(
  [azure]=pubsub.azure.eventhubs [aws]=pubsub.aws.snssqs [gcp]=pubsub.gcp.pubsub
  [alibaba]=pubsub.rocketmq      [oracle]=pubsub.kafka   [cloudflare]=pubsub.rabbitmq
  [baremetal]=pubsub.rabbitmq)
declare -A DAPR_STATE=(
  [azure]=state.azure.cosmosdb   [aws]=state.aws.dynamodb  [gcp]=state.gcp.firestore
  [alibaba]=state.alicloud.tablestore [oracle]=state.oracle [cloudflare]=state.redis
  [baremetal]=state.postgresql)
declare -A DAPR_SECRET=(
  [azure]=secretstores.azure.keyvault [aws]=secretstores.aws.secretsmanager
  [gcp]=secretstores.gcp.secretmanager [alibaba]=secretstores.local.env
  [oracle]=secretstores.hashicorp.vault [cloudflare]=secretstores.local.env
  [baremetal]=secretstores.hashicorp.vault)

for p in "${!DAPR_PUBSUB[@]}"; do
  mkdir -p "deploy/dapr/components/$p"
  cat > "deploy/dapr/components/$p/pubsub.yaml" <<YAML
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: pubsub
spec:
  type: ${DAPR_PUBSUB[$p]}
  version: v1
  metadata: []
YAML
  cat > "deploy/dapr/components/$p/statestore.yaml" <<YAML
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: statestore
spec:
  type: ${DAPR_STATE[$p]}
  version: v1
  metadata: []
YAML
  cat > "deploy/dapr/components/$p/secretstore.yaml" <<YAML
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: secretstore
spec:
  type: ${DAPR_SECRET[$p]}
  version: v1
  metadata: []
YAML
done

# Per-provider k8s overlays
mkdir -p deploy/k8s/{azure,aws,gcp,alibaba,oracle,baremetal}

# Active defaults (Azure) — the files compose actually mounts
cat > deploy/dapr/components/pubsub.yaml <<'YAML'
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: pubsub
spec:
  type: pubsub.azure.eventhubs   # swap: pubsub.kafka | pubsub.aws.snssqs | pubsub.gcp.pubsub
  version: v1
  metadata: []
YAML

cat > deploy/dapr/components/statestore.yaml <<'YAML'
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: statestore
spec:
  type: state.azure.cosmosdb     # swap: state.redis | state.aws.dynamodb | state.gcp.firestore
  version: v1
  metadata: []
YAML

cat > deploy/dapr/components/secretstore.yaml <<'YAML'
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: secretstore
spec:
  type: secretstores.azure.keyvault  # swap: secretstores.aws.secretsmanager | secretstores.gcp.secretmanager
  version: v1
  metadata: []
YAML

cat > deploy/dapr/config.yaml <<'YAML'
apiVersion: dapr.io/v1alpha1
kind: Configuration
metadata:
  name: dapr-config
spec:
  tracing:
    samplingRate: "1"
YAML

cat > deploy/docker-compose.yml <<'YAML'
services:
  # Pattern per service: the app container + a daprd sidecar sharing its network.
  # vm-service:
  #   build:
  #     context: ..
  #     dockerfile: services/VirtualMachineService/Dockerfile
  # vm-service-dapr:
  #   image: daprio/daprd:latest
  #   command: ["./daprd", "--app-id=virtual-machine-service", "--app-port=8080",
  #             "--dapr-http-port=3500", "--components-path=/components"]
  #   volumes: ["./dapr/components:/components"]
  #   network_mode: "service:vm-service"

  redis:
    image: redis:7-alpine
    ports: ["6379:6379"]

  zipkin:
    image: openzipkin/zipkin
    ports: ["9411:9411"]
YAML

# ══ WORKERS (Cloudflare Workers — TypeScript, edge runtime) ═════════════════
# CF Workers live under workers/cf-workers/ — workers/ also hosts the
# .NET background Worker Services (CleanArch.Workers, added below).
for w in gateway-worker auth-worker storage-worker ai-worker; do
  mkdir -p "workers/cf-workers/$w/src"/{middleware,handlers}
  cat > "workers/cf-workers/$w/package.json" <<JSON
{
  "name": "$w",
  "private": true,
  "scripts": {
    "dev": "wrangler dev",
    "deploy": "wrangler deploy",
    "test": "vitest run"
  },
  "devDependencies": {
    "wrangler": "^4.0.0",
    "typescript": "^5.0.0",
    "vitest": "^3.0.0"
  }
}
JSON
  cat > "workers/cf-workers/$w/tsconfig.json" <<'JSON'
{
  "compilerOptions": {
    "target": "ES2022",
    "module": "ESNext",
    "moduleResolution": "bundler",
    "strict": true,
    "types": ["@cloudflare/workers-types"]
  }
}
JSON
done

# Per-worker wrangler.toml with its bindings
cat > workers/cf-workers/gateway-worker/wrangler.toml <<'TOML'
name = "gateway-worker"
main = "src/index.ts"
compatibility_date = "2024-01-01"

# Edge gateway: routes traffic to downstream services reachable via
# Cloudflared tunnel hostnames (see tunnel/cloudflared/config.yaml).
[vars]
VM_SERVICE_URL       = "https://vm.internal.yourplatform.com"
IDENTITY_SERVICE_URL = "https://identity.internal.yourplatform.com"
STORAGE_SERVICE_URL  = "https://storage.internal.yourplatform.com"
AI_SERVICE_URL       = "https://ai.internal.yourplatform.com"

# [[kv_namespaces]]
# binding = "RATE_LIMIT_KV"
# id = "your-kv-namespace-id"
TOML

cat > workers/cf-workers/auth-worker/wrangler.toml <<'TOML'
name = "auth-worker"
main = "src/index.ts"
compatibility_date = "2024-01-01"

# JWT issue/verify at the edge using Web Crypto.
# AUTH_SECRET goes in via: wrangler secret put AUTH_SECRET
TOML

cat > workers/cf-workers/storage-worker/wrangler.toml <<'TOML'
name = "storage-worker"
main = "src/index.ts"
compatibility_date = "2024-01-01"

# [[r2_buckets]]
# binding = "STORAGE_BUCKET"
# bucket_name = "yourplatform-objects"
TOML

cat > workers/cf-workers/ai-worker/wrangler.toml <<'TOML'
name = "ai-worker"
main = "src/index.ts"
compatibility_date = "2024-01-01"

# [ai]
# binding = "AI"    # Workers AI binding — e.g. @cf/meta/llama-3.1-8b-instruct
TOML

# ══ CF-MICROSERVICES (Go services behind Workers/tunnel) ════════════════════
for g in vm-service identity-service storage-service; do
  mkdir -p "cf-microservices/$g"/{cmd,internal/{domain,application,infrastructure,api}}
  cat > "cf-microservices/$g/go.mod" <<GO
module yourplatform/$g

go 1.23
GO
  cat > "cf-microservices/$g/Dockerfile" <<DOCKER
# Build from repo root:
#   docker build -f cf-microservices/$g/Dockerfile -t cf-$g ./cf-microservices/$g
FROM golang:1.23-alpine AS build
WORKDIR /src
COPY . .
RUN go build -o /out/service ./cmd

FROM alpine:latest
COPY --from=build /out/service /service
ENTRYPOINT ["/service"]
DOCKER
done

# ══ TUNNEL (cloudflared) ═════════════════════════════════════════════════════
mkdir -p tunnel/cloudflared/credentials tunnel/config

cat > tunnel/cloudflared/config.yaml <<'YAML'
# cloudflared tunnel --config config.yaml run
tunnel: your-tunnel-id
credentials-file: /etc/cloudflared/credentials/tunnel-credentials.json

ingress:
  - hostname: api.yourplatform.com
    service: http://gateway-service:5000
  - hostname: vm.internal.yourplatform.com
    service: http://vm-service:5001
  - hostname: identity.internal.yourplatform.com
    service: http://identity-service:5002
  - hostname: ai.internal.yourplatform.com
    service: http://ai-service:5003
  - hostname: storage.internal.yourplatform.com
    service: http://storage-service:5004
  - hostname: go-vm.internal.yourplatform.com
    service: http://go-vm-service:8080
  - hostname: baremetal-api.yourplatform.com
    service: http://localhost:5000
    originRequest:
      connectTimeout: 30s
      noTLSVerify: false
  - hostname: grafana.internal.yourplatform.com
    service: http://grafana:3000
  - service: http_status:404              # catch-all must be last
YAML

cat > tunnel/config/baremetal.yaml <<'YAML'
# Run on the bare metal host — exposes local services with no public IP:
#   cloudflared tunnel --config baremetal.yaml run
tunnel: your-baremetal-tunnel-id
credentials-file: /etc/cloudflared/credentials/baremetal-credentials.json

ingress:
  - hostname: baremetal.yourplatform.com
    service: http://localhost:5000        # .NET API
  - hostname: auth.yourplatform.com
    service: http://localhost:8080        # Keycloak
  - hostname: rabbitmq.internal.yourplatform.com
    service: http://localhost:15672       # RabbitMQ management UI
  - hostname: vault.internal.yourplatform.com
    service: http://localhost:8200        # HashiCorp Vault
  - service: http_status:404
YAML

cat > tunnel/config/local-dev.yaml <<'YAML'
# Developer tunnel — share local services through a public hostname:
#   cloudflared tunnel --config local-dev.yaml run
tunnel: your-dev-tunnel-id
credentials-file: ~/.cloudflared/dev-credentials.json

ingress:
  - hostname: dev-api.yourplatform.com
    service: http://localhost:5000
  - hostname: dev-vm.yourplatform.com
    service: http://localhost:5001
  - service: http_status:404
YAML

cat > tunnel/config/k8s-ingress.yaml <<'YAML'
# In-cluster cloudflared (replaces ingress controller / public LB):
#   kubectl run cloudflared --image=cloudflare/cloudflared -- tunnel run
tunnel: your-k8s-tunnel-id
credentials-file: /etc/cloudflared/credentials/k8s-credentials.json

ingress:
  - hostname: k8s-api.yourplatform.com
    service: http://gateway-service.default.svc.cluster.local
  - service: http_status:404
YAML

cat > tunnel/cloudflared/Dockerfile <<'DOCKER'
FROM cloudflare/cloudflared:latest
COPY config.yaml  /etc/cloudflared/config.yaml
COPY credentials/ /etc/cloudflared/credentials/
CMD ["tunnel", "--config", "/etc/cloudflared/config.yaml", "run"]
DOCKER

# ══ BARE METAL ═══════════════════════════════════════════════════════════════
mkdir -p baremetal/ansible/{inventory/group_vars,playbooks}
mkdir -p baremetal/ansible/roles/{dotnet,docker,rabbitmq,keycloak,vault,nginx,cloudflared}/tasks
mkdir -p baremetal/terraform/providers
mkdir -p baremetal/scripts
mkdir -p baremetal/os/{ubuntu-22.04,debian-12}

cat > baremetal/ansible/inventory/hosts.yaml <<'YAML'
all:
  children:
    baremetal:
      hosts:
        bm-01:
          ansible_host: 192.0.2.10
YAML

cat > baremetal/ansible/inventory/group_vars/all.yaml <<'YAML'
platform_dir: /opt/yourplatform
dotnet_channel: "9.0"
go_version: "1.23"
YAML

cat > baremetal/ansible/inventory/group_vars/baremetal.yaml <<'YAML'
rabbitmq_port: 5672
keycloak_port: 8080
vault_port: 8200
cloudflared_config: /etc/cloudflared/config.yaml
YAML

cat > baremetal/ansible/playbooks/provision.yaml <<'YAML'
---
- name: Provision bare metal server
  hosts: baremetal
  become: true
  roles:
    - docker
    - dotnet
    - rabbitmq
    - keycloak
    - vault
    - nginx
    - cloudflared
YAML

cat > baremetal/ansible/playbooks/deploy-services.yaml <<'YAML'
---
- name: Deploy platform services
  hosts: baremetal
  become: true
  tasks:
    - name: Copy docker-compose for bare metal
      template:
        src: docker-compose.baremetal.yml.j2
        dest: "{{ platform_dir }}/docker-compose.yml"
    - name: Start all services
      community.docker.docker_compose_v2:
        project_src: "{{ platform_dir }}"
        state: present
YAML

cat > baremetal/ansible/playbooks/deploy-cloudflared.yaml <<'YAML'
---
- name: Install and run cloudflared tunnel daemon
  hosts: baremetal
  become: true
  tasks:
    - name: Install cloudflared
      ansible.builtin.get_url:
        url: https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64
        dest: /usr/local/bin/cloudflared
        mode: "0755"
    - name: Copy tunnel config
      ansible.builtin.copy:
        src: "{{ cloudflared_config }}"
        dest: /etc/cloudflared/config.yaml
YAML

cat > baremetal/ansible/playbooks/deploy-monitoring.yaml <<'YAML'
---
- name: Deploy monitoring stack (Prometheus + Grafana)
  hosts: baremetal
  become: true
  tasks:
    - name: Start monitoring via compose
      community.docker.docker_compose_v2:
        project_src: "{{ platform_dir }}/monitoring"
        state: present
YAML

for role in dotnet docker rabbitmq keycloak vault nginx cloudflared; do
  cat > "baremetal/ansible/roles/$role/tasks/main.yaml" <<YAML
---
# $role role — install/configure $role on the bare metal host.
YAML
done

cat > baremetal/terraform/main.tf <<'TF'
# Bare metal provisioning — pick a provider file under providers/.
# Terraform orders/claims the hardware; Ansible (../ansible) configures the OS.
TF

cat > baremetal/terraform/variables.tf <<'TF'
variable "server_name" { type = string }
variable "region"      { type = string }
TF

cat > baremetal/terraform/outputs.tf <<'TF'
output "server_ipv4" { value = "" }
TF

for tf in hetzner equinix ovh; do
  cat > "baremetal/terraform/providers/$tf.tf" <<TF
# $tf bare metal provider — add provider block + server resource here.
TF
done

for s in bootstrap deploy health-check rotate-secrets; do
  cat > "baremetal/scripts/$s.sh" <<SH
#!/usr/bin/env bash
# $s — bare metal helper script.
set -euo pipefail
SH
  chmod +x "baremetal/scripts/$s.sh"
done

for os in ubuntu-22.04 debian-12; do
  cat > "baremetal/os/$os/cloud-init.yaml" <<YAML
#cloud-config
# $os first-boot provisioning — packages, users, ssh, cloudflared install.
packages:
  - curl
  - git
  - docker.io
YAML
done

# ══ TOOLS ════════════════════════════════════════════════════════════════════
# ── Roslyn analyzers (compile-time architecture/naming/security rules) ──
analyzer "tools/analyzers/CleanArch.Analyzers" "CleanArch.Analyzers"
mkdir -p tools/analyzers/CleanArch.Analyzers/{Architecture,Domain,Application,Naming,Security}

test "tools/analyzers/CleanArch.Analyzers.Tests" "CleanArch.Analyzers.Tests" \
  "..\\CleanArch.Analyzers\\CleanArch.Analyzers.csproj"
mkdir -p tools/analyzers/CleanArch.Analyzers.Tests/{Architecture,Domain,Helpers}

# ── Code fixes (IDE lightbulb actions paired with the analyzers) ──
analyzer "tools/codefixes/CleanArch.CodeFixes" "CleanArch.CodeFixes" \
  "..\\..\\analyzers\\CleanArch.Analyzers\\CleanArch.Analyzers.csproj"
mkdir -p tools/codefixes/CleanArch.CodeFixes/{Architecture,Domain,Naming,Security}

# ── Source generators (compile-time boilerplate elimination) ──
analyzer "tools/generators/CleanArch.Generators" "CleanArch.Generators"
mkdir -p tools/generators/CleanArch.Generators/{Repository,UseCase,DependencyInjection,OpenApi,Contracts,Helpers}

# ── Linters (.editorconfig + ArchUnitNET-style architecture tests) ──
lib "tools/linters/CleanArch.Linting" "CleanArch.Linting" \
  "..\\..\\..\\src\\CleanArch.Application\\CleanArch.Application.csproj"
mkdir -p tools/linters/CleanArch.Linting/{Rules,Tests}

cat > tools/linters/.editorconfig <<'INI'
root = true

[*.cs]
indent_style            = space
indent_size             = 4
end_of_line             = lf
insert_final_newline    = true
trim_trailing_whitespace = true

csharp_new_line_before_open_brace  = all
csharp_prefer_braces               = true:warning
csharp_style_var_elsewhere         = false:warning

# Analyzer severities (YP = platform rules, see tools/analyzers)
dotnet_diagnostic.YP0001.severity  = error    # Domain must not reference SDKs
dotnet_diagnostic.YP0002.severity  = error    # Infrastructure leak
dotnet_diagnostic.YP1000.severity  = warning  # Cloud-brand name in interface
dotnet_diagnostic.YP2000.severity  = error    # Hardcoded secret
INI

cat > tools/linters/.globalconfig <<'INI'
is_global = true
global_level = 100
# Solution-wide analyzer configuration.
INI

cat > tools/linters/stylecop.json <<'JSON'
{
  "settings": {
    "documentationRules": { "companyName": "YourPlatform" },
    "orderingRules":      { "usingDirectivesPlacement": "outsideNamespace" }
  }
}
JSON

cat > tools/linters/.roslynatorconfig <<'INI'
# Roslynator rule configuration — enable/disable refactorings here.
INI

cat > tools/linters/Directory.Build.props <<'XML'
<Project>
  <PropertyGroup>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>latest</LangVersion>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <!-- NOTE: move/copy to the repo root to apply solution-wide -->
    <TreatWarningsAsErrors Condition="'$(CI)' == 'true'">true</TreatWarningsAsErrors>
  </PropertyGroup>

  <!-- Analyzer packages applied to every project when placed at repo root:
    <PackageReference Include="CleanArch.Analyzers"   PrivateAssets="all" />
    <PackageReference Include="StyleCop.Analyzers"    PrivateAssets="all" />
    <PackageReference Include="Roslynator.Analyzers"  PrivateAssets="all" />
    <PackageReference Include="SonarAnalyzer.CSharp"  PrivateAssets="all" />
  -->
</Project>
XML

# ── Code Server (browser-based VS Code dev environment) ──
mkdir -p tools/codeserver/{settings,devcontainer,tasks}

cat > tools/codeserver/Dockerfile <<'DOCKER'
FROM codercom/code-server:latest
USER root

# .NET
RUN curl -fsSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin \
      --channel 9.0 --install-dir /usr/local/dotnet
ENV PATH="$PATH:/usr/local/dotnet" DOTNET_ROOT="/usr/local/dotnet"

# Go (cf-microservices) + Node (workers)
RUN wget -q https://go.dev/dl/go1.23.linux-amd64.tar.gz \
    && tar -C /usr/local -xzf go1.23.linux-amd64.tar.gz \
    && rm go1.23.linux-amd64.tar.gz
ENV PATH="$PATH:/usr/local/go/bin"
RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y nodejs

# cloudflared + wrangler
RUN curl -L https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64 \
      -o /usr/local/bin/cloudflared && chmod +x /usr/local/bin/cloudflared
RUN npm install -g wrangler

USER coder
RUN code-server --install-extension ms-dotnettools.csharp \
    && code-server --install-extension golang.go \
    && code-server --install-extension humao.rest-client \
    && code-server --install-extension eamodio.gitlens

EXPOSE 8080
DOCKER

cat > tools/codeserver/docker-compose.yml <<'YAML'
services:
  codeserver:
    build: .
    ports: ["8080:8080"]
    volumes:
      - ../../:/workspace
    environment:
      PASSWORD: dev
YAML

cat > tools/codeserver/settings/settings.json <<'JSON'
{
  "editor.formatOnSave": true,
  "dotnet.defaultSolution": "CleanArch.slnx",
  "go.toolsManagement.autoUpdate": true
}
JSON

cat > tools/codeserver/settings/extensions.json <<'JSON'
{
  "recommendations": [
    "ms-dotnettools.csharp",
    "ms-dotnettools.csdevkit",
    "golang.go",
    "humao.rest-client",
    "eamodio.gitlens"
  ]
}
JSON

cat > tools/codeserver/settings/keybindings.json <<'JSON'
[]
JSON

cat > tools/codeserver/devcontainer/devcontainer.json <<'JSON'
{
  "name": "CleanArch Dev Environment",
  "dockerComposeFile": "../docker-compose.yml",
  "service": "codeserver",
  "workspaceFolder": "/workspace",
  "features": {
    "ghcr.io/devcontainers/features/dotnet:2": { "version": "9.0" },
    "ghcr.io/devcontainers/features/go:1":     { "version": "1.23" },
    "ghcr.io/devcontainers/features/node:1":   { "version": "20" }
  },
  "postCreateCommand": "bash devcontainer/post-create.sh",
  "forwardPorts": [5000, 5001, 8080]
}
JSON

cat > tools/codeserver/devcontainer/post-create.sh <<'SH'
#!/usr/bin/env bash
set -euo pipefail
dotnet restore CleanArch.slnx
for dir in cf-microservices/*/; do (cd "$dir" && go mod download || true); done
for dir in workers/cf-workers/*/; do (cd "$dir" && npm install || true); done
echo "Dev environment ready."
SH

for t in build test run-all-services; do
  cat > "tools/codeserver/tasks/$t.sh" <<SH
#!/usr/bin/env bash
# $t task for the dev container.
set -euo pipefail
SH
  chmod +x "tools/codeserver/tasks/$t.sh"
done

# ══ PRESENTATION ═════════════════════════════════════════════════════════════
P_SC="..\\..\\..\\shared\\CleanArch.Shared.Contracts\\CleanArch.Shared.Contracts.csproj"

# ── Web (Blazor) ──
web "presentation/web/CleanArch.Web" "CleanArch.Web" "$P_SC"
mkdir -p presentation/web/CleanArch.Web/Pages/{Dashboard,Compute,Storage,AI,Identity}
mkdir -p presentation/web/CleanArch.Web/Components/{Layout,Shared,Compute}
mkdir -p presentation/web/CleanArch.Web/{Services,State}
mkdir -p presentation/web/CleanArch.Web/wwwroot/{css,js}

test "presentation/web/CleanArch.Web.Tests" "CleanArch.Web.Tests" \
  "..\\CleanArch.Web\\CleanArch.Web.csproj"
mkdir -p presentation/web/CleanArch.Web.Tests/{Pages,Components}

# ── Mobile (.NET MAUI — stub project; add MAUI workload when implementing) ──
lib "presentation/mobile/CleanArch.Mobile" "CleanArch.Mobile" "$P_SC"
mkdir -p presentation/mobile/CleanArch.Mobile/Platforms/{Android,iOS,Windows}
mkdir -p presentation/mobile/CleanArch.Mobile/{Pages,ViewModels,Services}

# ── Desktop (Avalonia — stub project) ──
lib "presentation/desktop/CleanArch.Desktop" "CleanArch.Desktop" "$P_SC"
mkdir -p presentation/desktop/CleanArch.Desktop/{Views,ViewModels}

# ── Flutter (Dart) — one client for mobile + web + desktop ──
FLUT="presentation/flutter"
mkdir -p "$FLUT/lib"/core/{constants,errors,network,usecases}
for f in compute identity storage ai; do
  mkdir -p "$FLUT/lib/features/$f"/{domain/{entities,repositories,usecases},data/{models,datasources,repositories},presentation/{bloc,pages,widgets}}
  mkdir -p "$FLUT/test/features/$f"/{domain,data,presentation}
done
mkdir -p "$FLUT/integration_test"

cat > "$FLUT/pubspec.yaml" <<'YAML'
name: cleanarch_flutter
description: CleanArch Flutter presentation client.
publish_to: "none"
version: 0.1.0

environment:
  sdk: ^3.5.0

dependencies:
  flutter:
    sdk: flutter
  flutter_bloc: ^8.1.6     # state management (Bloc/Cubit per feature)
  get_it: ^7.7.0           # service locator — mirrors Program.cs DI wiring
  dio: ^5.7.0              # HTTP client for the gateway API
  dartz: ^0.10.1           # Either<Failure, T> for use-case results
  equatable: ^2.0.5        # value equality for entities/models/states

dev_dependencies:
  flutter_test:
    sdk: flutter
  flutter_lints: ^5.0.0
  mocktail: ^1.0.4
  build_runner: ^2.4.13    # codegen — dart run build_runner build

flutter:
  uses-material-design: true
YAML

cat > "$FLUT/analysis_options.yaml" <<'YAML'
include: package:flutter_lints/flutter.yaml
YAML

cat > "$FLUT/lib/main.dart" <<'DART'
import 'package:flutter/material.dart';

import 'injection_container.dart' as di;

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await di.initDependencies();
  runApp(const CleanArchApp());
}

class CleanArchApp extends StatelessWidget {
  const CleanArchApp({super.key});

  @override
  Widget build(BuildContext context) {
    return const MaterialApp(
      home: Scaffold(body: Center(child: Text('CleanArch'))),
    );
  }
}
DART

cat > "$FLUT/lib/injection_container.dart" <<'DART'
import 'package:get_it/get_it.dart';

/// Service locator — mirrors the DI wiring in each service's Program.cs.
/// Register per feature: datasource → repository → use case → bloc.
final sl = GetIt.instance;

Future<void> initDependencies() async {
  // Example:
  // sl.registerLazySingleton<VmRemoteDatasource>(() => VmRemoteDatasourceImpl(sl()));
  // sl.registerLazySingleton<VirtualMachineRepository>(() => VmRepositoryImpl(sl()));
  // sl.registerLazySingleton(() => GetVmList(sl()));
  // sl.registerFactory(() => VmListBloc(sl()));
}
DART

cat > "$FLUT/lib/core/errors/failures.dart" <<'DART'
import 'package:equatable/equatable.dart';

/// Base class for failures returned from the data layer.
/// Use cases return `Either<Failure, T>` — never throw across layers.
abstract class Failure extends Equatable {
  const Failure([this.message = '']);

  final String message;

  @override
  List<Object?> get props => [message];
}

class ServerFailure extends Failure {
  const ServerFailure([super.message]);
}

class CacheFailure extends Failure {
  const CacheFailure([super.message]);
}
DART

cat > "$FLUT/lib/core/usecases/usecase.dart" <<'DART'
import 'package:dartz/dartz.dart';
import 'package:equatable/equatable.dart';

import '../errors/failures.dart';

/// One class per user action — mirrors a Use Case in the Application layer.
abstract class UseCase<T, Params> {
  Future<Either<Failure, T>> call(Params params);
}

class NoParams extends Equatable {
  const NoParams();

  @override
  List<Object?> get props => [];
}
DART

cat > "$FLUT/test/widget_test.dart" <<'DART'
import 'package:flutter_test/flutter_test.dart';

void main() {
  testWidgets('placeholder', (tester) async {
    expect(true, isTrue);
  });
}
DART

# ══ MCP SERVERS ══════════════════════════════════════════════════════════════
# platform-mcp-server — tools/resources/prompts over the whole platform
lib "mcp/platform-mcp-server/src/PlatformMcpServer" "PlatformMcpServer" \
  "..\\..\\..\\..\\src\\CleanArch.Application\\CleanArch.Application.csproj" \
  "..\\..\\..\\..\\shared\\CleanArch.Shared.Contracts\\CleanArch.Shared.Contracts.csproj"
mkdir -p mcp/platform-mcp-server/src/PlatformMcpServer/Tools/{Compute,Storage,AI,Identity}
mkdir -p mcp/platform-mcp-server/src/PlatformMcpServer/{Resources,Prompts}
cat > mcp/platform-mcp-server/Dockerfile <<'DOCKER'
# Build from repo root:
#   docker build -f mcp/platform-mcp-server/Dockerfile -t platform-mcp-server .
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "mcp/platform-mcp-server/src/PlatformMcpServer/PlatformMcpServer.csproj" -c Release -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PlatformMcpServer.dll"]
DOCKER

# vm-mcp-server — VM-specific tools, scoped to VirtualMachineService.Application
lib "mcp/vm-mcp-server/src/VmMcpServer" "VmMcpServer" \
  "..\\..\\..\\..\\services\\VirtualMachineService\\src\\VirtualMachineService.Application\\VirtualMachineService.Application.csproj"
mkdir -p mcp/vm-mcp-server/src/VmMcpServer/Tools
cat > mcp/vm-mcp-server/Dockerfile <<'DOCKER'
# Build from repo root:
#   docker build -f mcp/vm-mcp-server/Dockerfile -t vm-mcp-server .
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "mcp/vm-mcp-server/src/VmMcpServer/VmMcpServer.csproj" -c Release -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VmMcpServer.dll"]
DOCKER

# docs-mcp-server — exposes documentation search to AI assistants
lib "mcp/docs-mcp-server/src/DocsMcpServer" "DocsMcpServer"
mkdir -p mcp/docs-mcp-server/src/DocsMcpServer/{Tools,Resources}
cat > mcp/docs-mcp-server/Dockerfile <<'DOCKER'
# Build from repo root:
#   docker build -f mcp/docs-mcp-server/Dockerfile -t docs-mcp-server .
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "mcp/docs-mcp-server/src/DocsMcpServer/DocsMcpServer.csproj" -c Release -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DocsMcpServer.dll"]
DOCKER

# ══ CLI ══════════════════════════════════════════════════════════════════════
# Operator CLI (ca <cmd>) — calls use cases directly, same DI graph as the API
lib "cli/CleanArch.Cli" "CleanArch.Cli" \
  "..\\..\\src\\CleanArch.Application\\CleanArch.Application.csproj" \
  "..\\..\\shared\\CleanArch.Shared.Contracts\\CleanArch.Shared.Contracts.csproj"
mkdir -p cli/CleanArch.Cli/Commands/{Compute,Storage,Identity,Config,Provider}
mkdir -p cli/CleanArch.Cli/{Output,Auth}

# Developer CLI (ca-dev <cmd>) — scaffolds services/use cases, runs analyzers,
# builds docs locally, manages the dev tunnel
lib "cli/CleanArch.DevCli" "CleanArch.DevCli"
mkdir -p cli/CleanArch.DevCli/Commands/{Scaffold,Analyze,Docs,Tunnel}

# ══ PACKAGES ═════════════════════════════════════════════════════════════════
# nuget/ — .nupkg output for analyzers, codefixes, shared contracts/domain
# npm/   — published TypeScript packages (worker types, generated api-client)
mkdir -p packages/nuget
mkdir -p "packages/npm/@cleanarch/worker-types"/{src,dist}
mkdir -p "packages/npm/@cleanarch/api-client/src"

cat > packages/Directory.Packages.props <<'XML'
<Project>
  <!-- Central Package Management — one version per package, solution-wide.
       NOTE: move/copy to the repo root (or <Import> it) to take effect. -->
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <!-- Logging -->
    <PackageVersion Include="Serilog.AspNetCore"            Version="8.0.3" />
    <PackageVersion Include="Serilog.Sinks.Console"         Version="6.0.0" />
    <PackageVersion Include="Serilog.Sinks.Seq"             Version="8.0.0" />
    <PackageVersion Include="Serilog.Sinks.Elasticsearch"   Version="10.0.0" />
    <PackageVersion Include="Serilog.Enrichers.Environment" Version="3.0.0" />
    <PackageVersion Include="Serilog.Enrichers.Thread"      Version="4.0.0" />
    <PackageVersion Include="Serilog.Enrichers.Span"        Version="3.1.0" />

    <!-- Testing / architecture tests -->
    <PackageVersion Include="xunit"                         Version="2.9.2" />
    <PackageVersion Include="FluentAssertions"              Version="6.12.1" />
    <PackageVersion Include="NSubstitute"                   Version="5.1.0" />

    <!-- Analyzers -->
    <PackageVersion Include="StyleCop.Analyzers"            Version="1.2.0-beta.556" />
    <PackageVersion Include="Roslynator.Analyzers"          Version="4.12.0" />
  </ItemGroup>
</Project>
XML

cat > "packages/npm/@cleanarch/worker-types/package.json" <<'JSON'
{
  "name": "@cleanarch/worker-types",
  "version": "0.1.0",
  "description": "Shared TypeScript types for CleanArch Cloudflare Workers",
  "main": "dist/index.js",
  "types": "dist/index.d.ts",
  "files": ["dist"]
}
JSON

cat > "packages/npm/@cleanarch/api-client/package.json" <<'JSON'
{
  "name": "@cleanarch/api-client",
  "version": "0.1.0",
  "description": "Generated API client — produced from the gateway OpenAPI spec",
  "main": "src/index.ts",
  "files": ["src"]
}
JSON

# ══ LOGGING ══════════════════════════════════════════════════════════════════
# Serilog bootstrap + enrichers — every service calls AddPlatformLogging()
lib "logging/CleanArch.Logging" "CleanArch.Logging"
mkdir -p logging/CleanArch.Logging/{Configuration,Enrichers,Middleware,Extensions}

# Per-sink projects — provider-specific log destinations
for s in Cloudflare Loki Oracle; do
  lib "logging/sinks/CleanArch.Sinks.$s" "CleanArch.Sinks.$s" \
    "..\\..\\CleanArch.Logging\\CleanArch.Logging.csproj"
done

# ══ BUILD AGENTS ═════════════════════════════════════════════════════════════
# Self-hosted GitHub Actions runners with the full toolchain preinstalled
mkdir -p build/{agents,scripts,targets}

cat > build/agents/Dockerfile.agent <<'DOCKER'
# Self-hosted runner: .NET SDK + Go + Node/Wrangler + docfx + Dapr + cloudflared
FROM ubuntu:22.04
ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get install -y \
    curl wget git unzip jq docker.io docker-compose \
    && rm -rf /var/lib/apt/lists/*

RUN curl -fsSL https://dot.net/v1/dotnet-install.sh | \
    bash /dev/stdin --channel 9.0 --install-dir /usr/local/dotnet
ENV PATH="$PATH:/usr/local/dotnet" DOTNET_ROOT="/usr/local/dotnet"

RUN wget -q https://go.dev/dl/go1.23.linux-amd64.tar.gz \
    && tar -C /usr/local -xzf go1.23.linux-amd64.tar.gz
ENV PATH="$PATH:/usr/local/go/bin"

RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y nodejs && npm install -g wrangler
RUN dotnet tool install -g docfx
ENV PATH="$PATH:/root/.dotnet/tools"

RUN wget -q https://raw.githubusercontent.com/dapr/cli/master/install/install.sh -O - | bash
RUN curl -L https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64 \
    -o /usr/local/bin/cloudflared && chmod +x /usr/local/bin/cloudflared

ARG RUNNER_VERSION=2.321.0
RUN mkdir /actions-runner && cd /actions-runner \
    && curl -O -L https://github.com/actions/runner/releases/download/v${RUNNER_VERSION}/actions-runner-linux-x64-${RUNNER_VERSION}.tar.gz \
    && tar xzf actions-runner-linux-x64-${RUNNER_VERSION}.tar.gz

WORKDIR /actions-runner
COPY setup-agent.sh .
RUN chmod +x setup-agent.sh
ENTRYPOINT ["./setup-agent.sh"]
DOCKER

cat > build/agents/docker-compose.agents.yml <<'YAML'
services:
  gh-runner:
    build:
      context: .
      dockerfile: Dockerfile.agent
    environment:
      GH_URL:   https://github.com/yourorg/CleanArch
      GH_TOKEN: ${GH_RUNNER_TOKEN}
      RUNNER_NAME: cleanarch-agent-1
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
    # deploy: { replicas: 3 }   # scale out for parallel jobs
YAML

cat > build/agents/setup-agent.sh <<'SH'
#!/usr/bin/env bash
# Registers this container as a self-hosted GitHub Actions runner.
set -euo pipefail
: "${GH_URL:?set GH_URL}" "${GH_TOKEN:?set GH_RUNNER_TOKEN}"
./config.sh --unattended --url "$GH_URL" --token "$GH_TOKEN" \
  --name "${RUNNER_NAME:-cleanarch-agent}" --work _work
./run.sh
SH
chmod +x build/agents/setup-agent.sh

# ── Build orchestration scripts (CAKE) ──
cat > build/scripts/build.sh <<'SH'
#!/usr/bin/env bash
# CAKE bootstrapper: ./build.sh --target=Default
set -euo pipefail
dotnet tool list -g | grep -q "cake.tool" || dotnet tool install -g Cake.Tool
dotnet cake "$(dirname "$0")/build.cake" "$@"
SH
chmod +x build/scripts/build.sh

cat > build/scripts/build.ps1 <<'PS1'
# CAKE bootstrapper (Windows): .\build.ps1 -Target Default
param([string]$Target = "Default")
dotnet tool list -g | Select-String "cake.tool" | Out-Null
if ($LASTEXITCODE -ne 0) { dotnet tool install -g Cake.Tool }
dotnet cake "$PSScriptRoot/build.cake" --target=$Target $args
PS1

cat > build/scripts/build.cake <<'CAKE'
var target  = Argument("target",        "Default");
var config  = Argument("configuration", "Release");
var version = Argument("version",       "1.0.0");

Task("Clean").Does(() =>
{
    CleanDirectories("**/bin");
    CleanDirectories("**/obj");
    CleanDirectory("artifacts");
});

Task("Restore").IsDependentOn("Clean")
    .Does(() => DotNetRestore("CleanArch.slnx"));

// Analyzers + source generators run inside the build —
// TreatWarningsAsErrors turns every YP#### violation into a CI failure.
Task("Analyze").IsDependentOn("Restore").Does(() =>
{
    DotNetBuild("CleanArch.slnx", new DotNetBuildSettings
    {
        Configuration   = config,
        NoRestore       = true,
        MSBuildSettings = new DotNetMSBuildSettings()
            .WithProperty("TreatWarningsAsErrors", "true")
            .WithProperty("EnforceCodeStyleInBuild", "true")
    });
});

Task("Test").IsDependentOn("Analyze").Does(() =>
{
    DotNetTest("CleanArch.slnx", new DotNetTestSettings
    {
        Configuration    = config,
        NoRestore        = true,
        NoBuild          = true,
        Collectors       = new[] { "XPlat Code Coverage" },
        ResultsDirectory = "artifacts/test-results"
    });
});

Task("Pack-NuGet").IsDependentOn("Test").Does(() =>
{
    var packProjects = new[]
    {
        "tools/analyzers/CleanArch.Analyzers/CleanArch.Analyzers.csproj",
        "tools/codefixes/CleanArch.CodeFixes/CleanArch.CodeFixes.csproj",
        "shared/CleanArch.Shared.Contracts/CleanArch.Shared.Contracts.csproj",
        "shared/CleanArch.Shared.Domain/CleanArch.Shared.Domain.csproj"
    };
    foreach (var csproj in packProjects)
        DotNetPack(csproj, new DotNetPackSettings
        {
            Configuration   = config,
            OutputDirectory = "artifacts/nuget",
            MSBuildSettings = new DotNetMSBuildSettings()
                .WithProperty("Version", version)
        });
});

Task("Build-Docs").Does(() =>
{
    // dotnet tool install -g docfx; docfx docs/docfx.json
    StartProcess("docfx", "docs/docfx.json");
});

Task("Default")
    .IsDependentOn("Test")
    .IsDependentOn("Pack-NuGet")
    .IsDependentOn("Build-Docs");

RunTarget(target);
CAKE

cat > build/scripts/pack-nuget.sh <<'SH'
#!/usr/bin/env bash
# Pack + push NuGet packages to the feed (nuget.org or GitHub Packages).
set -euo pipefail
VERSION="${1:?usage: pack-nuget.sh <version>}"
for p in tools/analyzers/CleanArch.Analyzers \
         tools/codefixes/CleanArch.CodeFixes \
         shared/CleanArch.Shared.Contracts \
         shared/CleanArch.Shared.Domain; do
  dotnet pack "$p" -c Release -o artifacts/nuget /p:Version="$VERSION"
done
# dotnet nuget push artifacts/nuget/*.nupkg -k "$NUGET_API_KEY" -s "$NUGET_SOURCE"
SH

cat > build/scripts/pack-npm.sh <<'SH'
#!/usr/bin/env bash
# Pack + publish npm packages (@cleanarch/worker-types, @cleanarch/api-client).
set -euo pipefail
for p in packages/npm/@cleanarch/*/; do
  (cd "$p" && npm pack --pack-destination ../../../artifacts/npm && npm publish --access public)
done
SH

cat > build/scripts/generate-version.sh <<'SH'
#!/usr/bin/env bash
# Semantic version from git tags: v1.4.2 -> 1.4.2, no tag -> 1.0.0-ci.<sha>
set -euo pipefail
if git describe --tags --exact-match 2>/dev/null; then
  git describe --tags --exact-match | sed 's/^v//'
else
  echo "1.0.0-ci.$(git rev-parse --short HEAD)"
fi
SH
chmod +x build/scripts/{pack-nuget.sh,pack-npm.sh,generate-version.sh}

# ── Custom MSBuild targets — imported by Directory.Build.props when enabled ──
for t in Analyzers CodeGen DocFx Pack; do
  cat > "build/targets/$t.targets" <<XML
<Project>
  <!-- $t.targets — custom MSBuild target hooks. Import from
       Directory.Build.props or individual .csproj files. -->
</Project>
XML
done

# ══ FILE WATCHERS ════════════════════════════════════════════════════════════
# FileSystemWatcher / IChangeToken services — config hot-reload, blob sync,
# schema validation, cert renewal. Registered as hosted services.
lib "watchers/CleanArch.FileWatcher" "CleanArch.FileWatcher" \
  "..\\..\\src\\CleanArch.Application\\CleanArch.Application.csproj"
mkdir -p watchers/CleanArch.FileWatcher/{Watchers,Handlers,Extensions}
mkdir -p watchers/config

cat > watchers/config/watchers.json <<'JSON'
{
  "FileWatcher": {
    "Config":   { "Path": "appsettings.json", "HotReload": true },
    "BlobSync": { "LocalPath": "./sync", "Container": "uploads" },
    "Schema":   { "Path": "docs/openapi", "Validate": true },
    "Certificate": { "Path": "/etc/ssl/certs", "RenewBeforeDays": 14 }
  }
}
JSON

# ══ .NET WORKER SERVICES ═════════════════════════════════════════════════════
# Background services (BackgroundService / IHostedService) — the always-on
# counterpart to the serverless Functions triggers.
lib "workers/CleanArch.Workers" "CleanArch.Workers" \
  "..\\..\\src\\CleanArch.Application\\CleanArch.Application.csproj" \
  "..\\..\\watchers\\CleanArch.FileWatcher\\CleanArch.FileWatcher.csproj" \
  "..\\..\\logging\\CleanArch.Logging\\CleanArch.Logging.csproj"
mkdir -p workers/CleanArch.Workers/{Compute,Messaging,Sync,Maintenance}

# ══ GITHUB ACTIONS ═══════════════════════════════════════════════════════════
mkdir -p .github/{workflows,actions/{setup-dotnet,setup-dapr,run-analyzers,deploy-service}}

cat > .github/workflows/ci.yml <<'YAML'
name: CI
on:
  push:        { branches: [main, develop] }
  pull_request:{ branches: [main, develop] }

jobs:
  analyze:          # Roslyn analyzers: dotnet build /p:TreatWarningsAsErrors=true
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: ./.github/actions/setup-dotnet
      - run: dotnet build CleanArch.slnx -c Release /p:TreatWarningsAsErrors=true

  test-dotnet:      # unit + architecture tests, coverage upload
    needs: analyze
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: ./.github/actions/setup-dotnet
      - run: dotnet test CleanArch.slnx -c Release --filter "Category=Unit" --collect:"XPlat Code Coverage"
      - run: dotnet test CleanArch.slnx -c Release --filter "Category=Architecture"   # CleanArch.Linting arch tests

  test-go:          # go test ./... for each cf-microservices/*
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-go@v5
        with: { go-version: "1.23" }
      - run: for d in cf-microservices/*/; do (cd "$d" && go test ./...); done

  test-workers:     # npm ci && npm test for each workers/cf-workers/*
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with: { node-version: "20" }
      - run: for d in workers/cf-workers/*/; do (cd "$d" && npm ci && npm test); done

  docker:           # matrix: build + push each service image to ghcr.io
    needs: [test-dotnet, test-go]
    runs-on: ubuntu-latest
    strategy:
      matrix:
        service: [VirtualMachineService, IdentityService, AIService, StorageService, GatewayService]
    steps:
      - uses: actions/checkout@v4
      - run: docker build -f services/${{ matrix.service }}/Dockerfile -t ${{ matrix.service }}:${{ github.sha }} .
YAML

cat > .github/workflows/cd-staging.yml <<'YAML'
name: Deploy to Staging
on:
  push: { branches: [develop] }
jobs:
  version:           # build/scripts/generate-version.sh -> 1.0.0-ci.<sha>
    runs-on: ubuntu-latest
    outputs: { version: ${{ steps.v.outputs.version }} }
    steps:
      - uses: actions/checkout@v4
      - id: v
        run: echo "version=$(build/scripts/generate-version.sh)" >> "$GITHUB_OUTPUT"

  build-and-pack:    # CAKE Pack-NuGet + npm pack -> artifacts/{nuget,npm}
    needs: version
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: ./.github/actions/setup-dotnet
      - run: build/scripts/pack-nuget.sh ${{ needs.version.outputs.version }}
      - run: build/scripts/pack-npm.sh
      - uses: actions/upload-artifact@v4
        with: { name: packages, path: artifacts/ }

  build-docs:        # DocFX: metadata -> api/*.yml, build -> docs/_site (staging)
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: ./.github/actions/setup-dotnet
      - run: |
          dotnet tool install -g docfx
          docfx metadata docs/docfx.json
          docfx build docs/docfx.json
      - uses: actions/upload-artifact@v4
        with: { name: docs-site, path: docs/_site }

  deploy-services:   # .NET services -> k8s staging via deploy-service action
    needs: version
    runs-on: ubuntu-latest
    environment: staging
    steps:
      - uses: actions/checkout@v4
      - uses: ./.github/actions/deploy-service
        with: { environment: staging, image-tag: ${{ github.sha }} }

  deploy-workers:    # wrangler deploy --env staging per workers/cf-workers/*
    runs-on: ubuntu-latest
    environment: staging
    steps:
      - uses: actions/checkout@v4
      - run: for d in workers/cf-workers/*/; do (cd "$d" && npm ci && npx wrangler deploy --env staging); done
        env: { CLOUDFLARE_API_TOKEN: ${{ secrets.CF_API_TOKEN }} }

  deploy-worker-services:  # .NET background workers -> k8s staging
    needs: version
    runs-on: ubuntu-latest
    environment: staging
    steps:
      - run: echo "kubectl apply -f deploy/k8s/staging/workers/"

  deploy-tunnel:     # PUT updated ingress rules to the Cloudflare tunnel API
    runs-on: ubuntu-latest
    environment: staging
    steps:
      - run: echo "Update tunnel config via CF API — see tunnel/cloudflared/config.yaml"

  smoke-tests:       # health checks + E2E against staging, then notify
    needs: [deploy-services, deploy-workers]
    runs-on: ubuntu-latest
    steps:
      - run: curl -fsS https://staging-api.yourplatform.com/health
YAML

cat > .github/workflows/cd-production.yml <<'YAML'
name: Deploy to Production
on:
  push: { branches: [main] }          # requires PR checks green + approvals
  workflow_dispatch:
jobs:
  create-release:    # git tag v<x.y.z> + gh release with changelog + packages
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: softprops/action-gh-release@v2
        with: { generate_release_notes: true }

  publish-nuget:     # dotnet nuget push artifacts/nuget/*.nupkg
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: ./.github/actions/setup-dotnet
      - run: dotnet nuget push 'artifacts/nuget/*.nupkg' -k ${{ secrets.NUGET_API_KEY }} -s https://api.nuget.org/v3/index.json

  publish-npm:       # npm publish for packages/npm/@cleanarch/*
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with: { node-version: "20", registry-url: "https://registry.npmjs.org" }
      - run: for d in packages/npm/@cleanarch/*/; do (cd "$d" && npm publish --access public); done
        env: { NODE_AUTH_TOKEN: ${{ secrets.NPM_TOKEN }} }

  publish-docs:      # DocFX -> docs/_site -> GitHub Pages (docs site)
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: ./.github/actions/setup-dotnet
      - run: |
          dotnet tool install -g docfx
          docfx docs/docfx.json
      - uses: peaceiris/actions-gh-pages@v4
        with:
          github_token: ${{ secrets.GITHUB_TOKEN }}
          publish_dir: ./docs/_site

  deploy-services:   # blue/green + canary rollout via deploy-service action
    runs-on: ubuntu-latest
    environment: production
    steps:
      - uses: actions/checkout@v4
      - uses: ./.github/actions/deploy-service
        with: { environment: production, image-tag: ${{ github.sha }} }

  deploy-workers-production:   # wrangler deploy --env production
    runs-on: ubuntu-latest
    environment: production
    steps:
      - uses: actions/checkout@v4
      - run: for d in workers/cf-workers/*/; do (cd "$d" && npm ci && npx wrangler deploy --env production); done
        env: { CLOUDFLARE_API_TOKEN: ${{ secrets.CF_API_TOKEN }} }

  post-deploy:       # prod smoke tests + status page + notify
    needs: [deploy-services, deploy-workers-production]
    runs-on: ubuntu-latest
    steps:
      - run: curl -fsS https://api.yourplatform.com/health
YAML

cat > .github/workflows/pr-checks.yml <<'YAML'
name: PR Checks
on:
  pull_request: { types: [opened, synchronize, reopened] }
jobs:
  lint:              # dotnet format vs tools/linters/.editorconfig
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: ./.github/actions/setup-dotnet
      - run: dotnet format CleanArch.slnx --verify-no-changes

  analyze:           # TOOLS LAYER — analyzers + generators run inside the build;
                     # TreatWarningsAsErrors fails CI on any YP#### violation
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: ./.github/actions/setup-dotnet
      - uses: ./.github/actions/run-analyzers

  size-check:        # label PR XS/S/M/L/XL
    runs-on: ubuntu-latest
    steps:
      - uses: CodelyTV/pr-size-labeler@v1
        with: { GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }} }

  label:             # auto-label by touched paths
    runs-on: ubuntu-latest
    steps:
      - uses: actions/labeler@v5
YAML

cat > .github/workflows/security-scan.yml <<'YAML'
name: Security Scan
on:
  push: { branches: [main, develop] }
  schedule: [{ cron: "0 6 * * 1" }]
jobs:
  trivy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: aquasecurity/trivy-action@master
        with: { scan-type: fs, severity: "CRITICAL,HIGH" }
  dotnet-audit:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: ./.github/actions/setup-dotnet
      - run: dotnet list package --vulnerable --include-transitive
YAML

cat > .github/workflows/docs.yml <<'YAML'
name: Build and Publish Docs
on:
  push:
    branches: [main]
    paths: ["docs/**", "src/**/*.cs"]
jobs:
  build-docs:        # docfx docs/docfx.json -> GitHub Pages
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: ./.github/actions/setup-dotnet
      - run: |
          dotnet tool install -g docfx
          docfx docs/docfx.json
      - uses: peaceiris/actions-gh-pages@v4
        with:
          github_token: ${{ secrets.GITHUB_TOKEN }}
          publish_dir: ./docs/_site
YAML

cat > .github/workflows/workers-deploy.yml <<'YAML'
name: Deploy Cloudflare Workers
on:
  push:
    branches: [main]
    paths: ["workers/cf-workers/**"]
jobs:
  deploy:
    runs-on: ubuntu-latest
    strategy:
      matrix: { worker: [gateway-worker, auth-worker, storage-worker, ai-worker] }
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with: { node-version: "20" }
      - run: cd workers/cf-workers/${{ matrix.worker }} && npm ci && npx wrangler deploy
        env: { CLOUDFLARE_API_TOKEN: ${{ secrets.CF_API_TOKEN }} }
YAML

cat > .github/workflows/go-services.yml <<'YAML'
name: Go Services
on:
  push:
    branches: [main, develop]
    paths: ["cf-microservices/**"]
jobs:
  test:
    runs-on: ubuntu-latest
    strategy:
      matrix: { service: [vm-service, identity-service, storage-service] }
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-go@v5
        with: { go-version: "1.23" }
      - run: cd cf-microservices/${{ matrix.service }} && go test ./... -race
YAML

cat > .github/workflows/release.yml <<'YAML'
name: Release
on:
  push: { tags: ["v*"] }
jobs:
  release:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: softprops/action-gh-release@v2
        with: { generate_release_notes: true }
YAML

cat > .github/actions/setup-dotnet/action.yml <<'YAML'
name: Setup .NET
description: Installs .NET SDK and caches NuGet packages
runs:
  using: composite
  steps:
    - uses: actions/setup-dotnet@v4
      with: { dotnet-version: "9.0.x" }
    - uses: actions/cache@v4
      with:
        path: ~/.nuget/packages
        key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
        restore-keys: ${{ runner.os }}-nuget-
YAML

cat > .github/actions/setup-dapr/action.yml <<'YAML'
name: Setup Dapr
description: Installs the Dapr CLI and initializes daprd
runs:
  using: composite
  steps:
    - run: wget -q https://raw.githubusercontent.com/dapr/cli/master/install/install.sh -O - | bash
      shell: bash
    - run: dapr init
      shell: bash
YAML

cat > .github/actions/run-analyzers/action.yml <<'YAML'
name: Run Analyzers
description: Builds with TreatWarningsAsErrors so Roslyn rules fail CI on violation
runs:
  using: composite
  steps:
    - run: dotnet build CleanArch.slnx -c Release /p:TreatWarningsAsErrors=true /p:EnforceCodeStyleInBuild=true
      shell: bash
YAML

cat > .github/actions/deploy-service/action.yml <<'YAML'
name: Deploy Service
description: Deploys a service image to the target environment
inputs:
  environment: { required: true }
  image-tag:   { required: true }
runs:
  using: composite
  steps:
    - run: echo "deploy ${{ inputs.image-tag }} -> ${{ inputs.environment }} (kubectl/helm here)"
      shell: bash
YAML

# ══ DOCS (DocFX + exported specs) ════════════════════════════════════════════
mkdir -p docs/{openapi,postman,api,_template/{styles,partials}}
mkdir -p docs/articles/{getting-started,architecture,services,providers,tools,deployment}

cat > docs/docfx.json <<'JSON'
{
  "metadata": [
    {
      "src": [
        {
          "files": ["**/*.csproj"],
          "exclude": ["**/obj/**", "**/bin/**", "**/*.Tests.csproj"],
          "src": "../src"
        },
        {
          "files": ["**/*.csproj"],
          "exclude": ["**/obj/**", "**/bin/**", "**/*.Tests.csproj"],
          "src": "../services"
        }
      ],
      "dest": "api"
    }
  ],
  "build": {
    "content": [
      { "files": ["articles/**/*.md", "api/**/*.yml"] },
      { "files": ["toc.yml", "index.md"] }
    ],
    "resource": [{ "files": ["images/**"] }],
    "dest": "_site",
    "globalMetadata": {
      "_appName": "CleanArch",
      "_appTitle": "CleanArch Documentation",
      "_enableSearch": true
    },
    "template": ["default", "modern", "_template"]
  }
}
JSON

cat > docs/toc.yml <<'YAML'
- name: Getting Started
  href: articles/getting-started/toc.yml
- name: Architecture
  href: articles/architecture/toc.yml
- name: Services
  href: articles/services/toc.yml
- name: Cloud Providers
  href: articles/providers/toc.yml
- name: Tools
  href: articles/tools/toc.yml
- name: Deployment
  href: articles/deployment/toc.yml
- name: API Reference
  href: api/toc.yml
YAML

cat > docs/index.md <<'MD'
# CleanArch

Clean Architecture template: monolith + multi-cloud + microservices +
serverless functions + Cloudflare workers + bare metal.

Start with **Getting Started** in the table of contents.
MD

cat > docs/api/toc.yml <<'YAML'
# DocFX fills this in from the metadata step (src/**, services/**).
- name: Namespaces
  href: .
YAML

declare -A DOCS_TOC=(
  [getting-started]="overview.md quickstart.md installation.md"
  [architecture]="clean-architecture.md multi-cloud.md microservices.md domain-layer.md adding-a-provider.md"
  [services]="vm-service.md identity-service.md ai-service.md storage-service.md"
  [providers]="azure.md aws.md gcp.md alibaba.md oracle.md cloudflare.md baremetal.md"
  [tools]="analyzers.md generators.md codeserver.md"
  [deployment]="docker.md kubernetes.md cloudflare-workers.md baremetal.md"
)

for section in "${!DOCS_TOC[@]}"; do
  {
    for page in ${DOCS_TOC[$section]}; do
      title="${page%.md}"
      echo "- name: ${title//-/ }"
      echo "  href: $page"
    done
  } > "docs/articles/$section/toc.yml"
  for page in ${DOCS_TOC[$section]}; do
    title="${page%.md}"
    printf '# %s\n' "${title//-/ }" > "docs/articles/$section/$page"
  done
done

cat > docs/_template/styles/main.css <<'CSS'
/* DocFX theme overrides */
CSS

cat > docs/_template/partials/head.tmpl.partial <<'TMPL'
{{! DocFX partial: custom <head> additions }}
TMPL

# ══ .gitkeep in every empty dir ══════════════════════════════════════════════
find . -type d -empty -exec touch {}/.gitkeep \;

echo "Scaffold complete."
