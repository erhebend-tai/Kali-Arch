# CleanArch — Unified Clean Architecture Template

A single scaffold containing **every** structure variant from the design discussion:
monolith, multi-cloud infrastructure (Azure / AWS / GCP / Alibaba / Oracle /
Cloudflare / BareMetal), microservices + Dapr, serverless Functions, OpenAPI,
Cloudflare Workers, Go microservices, cloudflared tunneling, bare-metal
provisioning, Roslyn tooling, presentation apps, MCP servers, GitHub Actions,
and DocFX — all structure-only, all building cleanly (`dotnet build CleanArch.slnx`).

## Top-Level Layout

```
CleanArch/
│
├── src/                        ── MONOLITH (modular monolith baseline)
│   ├── CleanArch.Domain/           Entities, ValueObjects, Enums, Events,
│   │                               Exceptions, Rules, repository interfaces —
│   │                               plus pattern folders: Factories/,
│   │                               Specifications/, Commands/ (GoF undo/redo),
│   │                               Builders/, Templates/. Zero deps.
│   ├── CleanArch.Application/      UseCases, Interfaces (cloud-NEUTRAL names),
│   │                               DTOs, Behaviors — plus MediatR CQRS:
│   │                               Commands/, Queries/, Notifications/,
│   │                               Pipelines/ (compensating steps), Chains/,
│   │                               Common/ (marker interfaces),
│   │                               Interfaces/Strategies/. Domain only.
│   ├── CleanArch.Infrastructure.Common/   Provider-free plumbing: Resilience,
│   │                               Serialization, Caching, Observability —
│   │                               plus Decorators/ (cache/retry/log wraps),
│   │                               Outbox/ (reliable messaging),
│   │                               Persistence/ (UoW base, Spec evaluator).
│   ├── CleanArch.Infrastructure.Azure/    Azure SDK implementations
│   ├── CleanArch.Infrastructure.AWS/      AWS SDK implementations
│   ├── CleanArch.Infrastructure.GCP/      GCP SDK implementations
│   ├── CleanArch.Infrastructure.Alibaba/  Alibaba SDK impls (OSS, RocketMQ,  (same
│   ├── CleanArch.Infrastructure.Oracle/   TableStore, OKE, RAM, KMS)          folder
│   ├── CleanArch.Infrastructure.Cloudflare/   CF impls (R2, Queues, KV, D1,   set per
│   │                               Workers AI, Tunnel/ → ITunnelService)    provider)
│   ├── CleanArch.Infrastructure.BareMetal/    Self-hosted impls (RabbitMQ,
│   │                               Keycloak, Vault, Nginx, PostgreSQL)
│   ├── CleanArch.API/              Controllers, Middleware, OpenApi/.
│   │                               Entry point — picks provider via config.
│   └── CleanArch.Functions/        Serverless entry point.
│                                   Triggers/{Http,Timer,Queue,EventHub,Blob,Dapr}
│
├── services/                   ── MICROSERVICES (each = mini Clean Architecture)
│   ├── VirtualMachineService/
│   ├── IdentityService/
│   ├── StorageService/
│   ├── AIService/                  each contains:
│   │   ├── src/{Svc}.Domain/         Entities, ValueObjects, Events, Interfaces
│   │   │                               + Factories/ Specifications/ Commands/
│   │   │                               Builders/ Templates/
│   │   ├── src/{Svc}.Application/    UseCases, Interfaces, EventHandlers
│   │   │                               + Commands/ Queries/ Notifications/
│   │   │                               Behaviors/ Pipelines/ Chains/ Common/
│   │   ├── src/{Svc}.Infrastructure/ <category>/, Persistence, Messaging
│   │   │                               + Decorators/ Strategies/ Factories/
│   │   │                               Outbox/
│   │   ├── src/{Svc}.API/            Controllers, Subscribers (Dapr endpoints)
│   │   ├── src/{Svc}.Functions/      Triggers/{Http,Timer,Queue,EventHub,Blob,Dapr}
│   │   ├── tests/                    Domain / Application / Infrastructure .Tests
│   │   └── Dockerfile
│   └── GatewayService/             YARP reverse proxy + OpenAPI aggregation
│       └── src/GatewayService.API/   OpenApi/, yarp.json
│
├── shared/                     ── SHARED KERNEL (referenced by all services)
│   ├── CleanArch.Shared.Domain/        ValueObjects, Interfaces (IDomainEvent)
│   ├── CleanArch.Shared.Contracts/     Integration Events + Commands
│   │                                   (the ONLY thing that crosses service
│   │                                    boundaries — primitive types only)
│   └── CleanArch.Shared.Infrastructure/ Middleware, Observability, Dapr helpers
│
├── workers/                    ── BACKGROUND + EDGE RUNTIMES
│   ├── CleanArch.Workers/          .NET Worker Services (BackgroundService):
│   │                               Compute/ Messaging/ Sync/ Maintenance/ —
│   │                               the always-on counterpart to Functions
│   └── cf-workers/                 Cloudflare Workers (TypeScript, edge):
│       ├── gateway-worker/         src/{middleware,handlers}, wrangler.toml
│       ├── auth-worker/            JWT issue/verify at the edge
│       ├── storage-worker/         R2 bindings (upload/download handlers)
│       └── ai-worker/              Workers AI binding (inference/embedding)
│
├── cf-microservices/           ── CLOUDFLARE GO MICROSERVICES
│   ├── vm-service/                 cmd/ + internal/{domain,application,
│   ├── identity-service/           infrastructure,api} — Clean Architecture
│   └── storage-service/            in Go, behind Workers/tunnel
│
├── tunnel/                     ── CLOUDFLARED
│   ├── cloudflared/                config.yaml (ingress rules), credentials/,
│   │                               Dockerfile
│   └── config/                     baremetal.yaml, local-dev.yaml,
│                                   k8s-ingress.yaml
│
├── baremetal/                  ── BARE METAL PROVISIONING
│   ├── ansible/                    inventory/, playbooks/, roles/{dotnet,
│   │                               docker,rabbitmq,keycloak,vault,nginx,
│   │                               cloudflared}
│   ├── terraform/                  main.tf + providers/{hetzner,equinix,ovh}
│   ├── scripts/                    bootstrap.sh, deploy.sh, health-check.sh
│   └── os/                         cloud-init for ubuntu-22.04 / debian-12
│
├── tools/                      ── TOOLING
│   ├── analyzers/                  CleanArch.Analyzers (netstandard2.0 Roslyn
│   │                               rules: Architecture/ Domain/ Naming/
│   │                               Security/ — YP#### diagnostic IDs)
│   │                               + CleanArch.Analyzers.Tests
│   ├── codefixes/                  CleanArch.CodeFixes (lightbulb fixes
│   │                               paired with analyzer diagnostics)
│   ├── generators/                 CleanArch.Generators (source generators:
│   │                               Repository/ UseCase/ DI/ OpenApi/ Contracts)
│   ├── linters/                    .editorconfig, .globalconfig, stylecop.json,
│   │                               .roslynatorconfig, Directory.Build.props,
│   │                               CleanArch.Linting (architecture-as-tests)
│   └── codeserver/                 Dockerfile, docker-compose, devcontainer,
│                                   settings/, tasks/ — browser VS Code env
│
├── presentation/               ── PRESENTATION LAYER
│   ├── web/CleanArch.Web           Blazor — Pages/ Components/ Services/ State/
│   ├── web/CleanArch.Web.Tests
│   ├── mobile/CleanArch.Mobile     MAUI stub — Platforms/ Pages/ ViewModels/
│   ├── desktop/CleanArch.Desktop   Avalonia stub — Views/ ViewModels/
│   └── flutter/                    Dart/Flutter client (mobile+web+desktop)
│       ├── lib/core/               errors/ (Failure types), usecases/ (UseCase
│       │                           base), network/, constants/
│       ├── lib/features/<f>/       compute/ identity/ storage/ ai/ — each:
│       │                           domain/{entities,repositories,usecases}
│       │                           data/{models,datasources,repositories}
│       │                           presentation/{bloc,pages,widgets}
│       ├── lib/injection_container.dart   get_it — mirrors Program.cs DI
│       ├── test/features/          mirrors lib/features/
│       └── pubspec.yaml            flutter_bloc, get_it, dio, dartz,
│                                   equatable, build_runner, mocktail
│
├── mcp/                        ── MCP SERVERS (expose platform to AI tools)
│   ├── platform-mcp-server/        Tools/{Compute,Storage,AI,Identity},
│   │                               Resources/, Prompts/
│   ├── vm-mcp-server/              VM-scoped tools
│   └── docs-mcp-server/            Documentation search tools/resources
│
├── cli/                        ── COMMAND-LINE TOOLS
│   ├── CleanArch.Cli/              Operator CLI — Commands/{Compute,Storage,
│   │                               Identity,Config,Provider}, Output/
│   │                               formatters, Auth/. Calls use cases
│   │                               directly through the same DI graph.
│   └── CleanArch.DevCli/           Dev CLI — Commands/{Scaffold,Analyze,
│                                   Docs,Tunnel}: scaffold new services/
│                                   use cases, run analyzers, build docs.
│
├── packages/                   ── PACKAGE MANAGER OUTPUT
│   ├── nuget/                      .nupkg output (analyzers, codefixes,
│   │                               shared contracts/domain)
│   ├── npm/@cleanarch/             worker-types + generated api-client
│   └── Directory.Packages.props    Central Package Management (single
│                                   version per package, solution-wide)
│
├── logging/                    ── LOGGING / SINKS
│   ├── CleanArch.Logging/          Serilog bootstrap — Configuration/,
│   │                               Enrichers/ (ServiceName, CloudProvider,
│   │                               CorrelationId, Tenant), Middleware/
│   └── sinks/                      CleanArch.Sinks.{Cloudflare,Loki,Oracle}
│                                   per-provider log destinations
│
├── build/                      ── BUILD AGENT + ORCHESTRATION
│   ├── agents/                     Dockerfile.agent (self-hosted GH runner
│   │                               with full toolchain), setup-agent.sh
│   ├── scripts/                    build.cake (CAKE: Clean→Restore→Analyze
│   │                               →Test→Pack→Docs), pack-nuget.sh,
│   │                               pack-npm.sh, generate-version.sh
│   └── targets/                    custom MSBuild targets: Analyzers,
│                                   CodeGen, DocFx, Pack
│
├── watchers/                   ── FILE WATCHERS
│   ├── CleanArch.FileWatcher/      Watchers/ (config hot-reload, blob sync,
│   │                               schema validation, cert renewal),
│   │                               Handlers/, Extensions/
│   └── config/watchers.json        per-watcher paths + options
│
├── .github/                    ── GITHUB ACTIONS
│   ├── workflows/                  ci, cd-staging, cd-production, pr-checks,
│   │                               security-scan, docs, workers-deploy,
│   │                               go-services, release
│   └── actions/                    setup-dotnet, setup-dapr, run-analyzers,
│                                   deploy-service (reusable composites)
│
├── tests/                      ── MONOLITH tests (mirror of src/, one per
│                                 provider infrastructure project)
│
├── deploy/
│   ├── docker-compose.yml          Per-service pattern: app + daprd sidecar
│   ├── dapr/components/            Active component set (pubsub/statestore/
│   │   │                           secretstore — Azure defaults)
│   │   └── <provider>/             azure/ aws/ gcp/ alibaba/ oracle/
│   │                               cloudflare/ baremetal/ — ready-made
│   │                               per-provider spec.type sets to swap in
│   └── k8s/<provider>/             per-provider overlays
│
├── docs/
│   ├── docfx.json                  DocFX: metadata over src/ + services/,
│   │                               builds articles + auto API reference
│   ├── toc.yml, index.md
│   ├── articles/                   getting-started/ architecture/ services/
│   │                               providers/ tools/ deployment/
│   ├── api/                        auto-generated API reference output
│   ├── _template/                  custom DocFX theme (styles/, partials/)
│   ├── openapi/                    exported per-service specs
│   ├── postman/
│   └── prompts/                    paste-ready prompt set for the next
│                                   design rounds (00-index = gap map)
│
├── CleanArch.slnx                  All 77 projects, one solution
└── scaffold.sh                     Regenerates the whole structure
```

## The Dependency Rule (as wired in the .csproj files)

```
        ┌── API / Functions ──┐        entry points (reference everything,
        │    (entry points)   │        for DI wiring only)
        └─────────┬───────────┘
                  │
        ┌── Infrastructure.{Azure|AWS|GCP} ── implements interfaces
        │           + Infrastructure.Common
        └─────────┬───────────┘
                  │
        ┌── Application ── defines interfaces + use cases
        └─────────┬───────────┘
                  │
        └────► Domain ── no outward dependencies, ever
```

- Domain → (Shared.Domain only)
- Application → Domain, Shared.Domain, Shared.Contracts
- Infrastructure.* → Application (+ Common, Shared.Contracts, Shared.Infrastructure)
- API / Functions → Application + all Infrastructure providers + Shared.Infrastructure
- Tests → the single project they test

## Where Things Go — Cheat Sheet

| Concept | Project | Folder |
|---|---|---|
| Entity, ValueObject, Domain Event, Rule | `*.Domain` | `Entities/ ValueObjects/ Events/ Rules/` |
| Repository interface | `*.Domain` | `Interfaces/Repositories/` |
| Use case (one per user action) | `*.Application` | `UseCases/` |
| MediatR command / query + handler (CQRS) | `*.Application` | `Commands/<domain>/`, `Queries/<domain>/` |
| MediatR notification (domain event → handlers) | `*.Application` | `Notifications/` |
| Pipeline behavior (log/validate/tx/cache/auth/perf) | `*.Application` | `Behaviors/` |
| Ordered compensating workflow step | `*.Application` | `Pipelines/<domain>/Steps/` |
| Chain-of-Responsibility handler | `*.Application` | `Chains/` |
| Marker interfaces (ICommand/IQuery/handler) | `*.Application` | `Common/` |
| Unit of Work interface | `*.Application` | `Interfaces/` — impl in `Infrastructure.*/Persistence/` |
| Strategy interface / provider impl | `*.Application` / `*.Infrastructure.<p>` | `Interfaces/Strategies/` / `Strategies/` |
| GoF command (undo/redo operations) | `*.Domain` | `Commands/` |
| Factory (entity create/reconstitute) | `*.Domain` | `Factories/` — provider abstract factories in `Infrastructure.<p>/Factories/` |
| Specification (composable rule → EF query) | `*.Domain` | `Specifications/` — evaluator in `Infrastructure.Common/Persistence/` |
| Fluent entity builder | `*.Domain` | `Builders/` |
| Template method (algorithm skeleton) | `*.Domain` | `Templates/` |
| Decorator (cache/retry/log wrapper) | `Infrastructure.Common` | `Decorators/` |
| Outbox (at-least-once event delivery) | `Infrastructure.Common` / `{Svc}.Infrastructure` | `Outbox/` |
| Cloud-neutral service interface | `*.Application` | `Interfaces/` |
| Azure/AWS/GCP SDK call | `*.Infrastructure.{provider}` | category folder (`AI/ Storage/ …`) |
| Integration event schema | `Shared.Contracts` | `Events/ Commands/` |
| Shared value object | `Shared.Domain` | `ValueObjects/` |
| Correlation/exception middleware | `Shared.Infrastructure` | `Middleware/` |
| REST controller | `*.API` | `Controllers/` |
| Dapr subscriber endpoint | `*.API` | `Subscribers/` |
| OpenAPI filters/examples | `*.API` | `OpenApi/` |
| Any serverless trigger | `*.Functions` | `Triggers/<type>/` |
| Provider-agnostic helpers | `Infrastructure.Common` | `Resilience/ Serialization/ …` |
| Message-bus / secret-store / state config | — | `deploy/dapr/components/<provider>/*.yaml` |
| Cloudflare Worker (edge code) | — | `workers/cf-workers/<name>/src/` |
| Background worker service | `workers/CleanArch.Workers` | `Compute/ Messaging/ Sync/ Maintenance/` |
| Operator CLI | `cli/CleanArch.Cli` | `Commands/<domain>/`, `Output/` formatters |
| Dev/scaffolding CLI | `cli/CleanArch.DevCli` | `Commands/{Scaffold,Analyze,Docs,Tunnel}` |
| NuGet package output | — | `packages/nuget/` |
| npm packages (types, api-client) | — | `packages/npm/@cleanarch/` |
| Central package versions | — | `packages/Directory.Packages.props` |
| Serilog config + enrichers | `logging/CleanArch.Logging` | `Configuration/ Enrichers/ Middleware/` |
| Provider-specific log sink | `logging/sinks/CleanArch.Sinks.<p>` | — |
| Self-hosted CI runner image | — | `build/agents/Dockerfile.agent` |
| Build orchestration (CAKE) | — | `build/scripts/build.cake` |
| Custom MSBuild targets | — | `build/targets/*.targets` |
| File watcher (config/blob/schema/cert) | `watchers/CleanArch.FileWatcher` | `Watchers/` |
| Go microservice (Clean Arch in Go) | — | `cf-microservices/<svc>/internal/{domain,application,infrastructure,api}` |
| cloudflared tunnel config | — | `tunnel/cloudflared/config.yaml`, `tunnel/config/` |
| Bare metal provisioning | — | `baremetal/ansible/` playbooks+roles, `baremetal/terraform/` |
| Compile-time architecture/naming/security rule | `tools/analyzers/CleanArch.Analyzers` | `Architecture/ Naming/ Security/` |
| IDE lightbulb fix for a diagnostic | `tools/codefixes/CleanArch.CodeFixes` | matching category folder |
| Boilerplate code generation | `tools/generators/CleanArch.Generators` | `Repository/ UseCase/ …` |
| Style & formatting rules | — | `tools/linters/.editorconfig` et al. |
| Architecture-as-unit-tests | `tools/linters/CleanArch.Linting` | `Tests/` |
| Browser VS Code environment | — | `tools/codeserver/` |
| Blazor web UI | `presentation/web/CleanArch.Web` | `Pages/ Components/ Services/ State/` |
| Flutter client (Dart) | — | `presentation/flutter/lib/features/<feature>/{domain,data,presentation}` |
| MCP tool / resource / prompt | `mcp/<server>/src/<proj>` | `Tools/ Resources/ Prompts/` |
| CI/CD pipeline | — | `.github/workflows/*.yml` |
| Reusable workflow step | — | `.github/actions/<name>/action.yml` |
| DocFX config & articles | — | `docs/docfx.json`, `docs/articles/` |

## Design Pattern Map — where each pattern lives

| Pattern | Primary layer | Secondary layer |
|---|---|---|
| MediatR (Commands/Queries/Notifications) | `Application` | `API` (thin `ISender` dispatch) |
| Unit of Work | `Application` (interface) | `Infrastructure.*/Persistence` |
| Step / Pipeline (compensating) | `Application/Pipelines` | `Domain` |
| GoF Command (undo/redo) | `Domain/Commands` | `Application` |
| Factory / Abstract Factory | `Domain/Factories` | `Infrastructure.<p>/Factories` |
| Repository | `Domain/Interfaces/Repositories` | `Infrastructure.*/Persistence` |
| Specification | `Domain/Specifications` | `Infrastructure.Common/Persistence` |
| Strategy | `Application/Interfaces/Strategies` | `Infrastructure.<p>/Strategies` |
| Decorator (cache/retry/log) | `Infrastructure.Common/Decorators` | per-service `Infrastructure` |
| Observer (domain events → handlers) | `Domain/Events` | `Application/Notifications` |
| Chain of Responsibility | `Application/Chains` | `Infrastructure` |
| Template Method | `Domain/Templates` | `Application` |
| Builder | `Domain/Builders` | `Application` |
| Outbox | `Infrastructure.Common/Outbox` | `Application` |
| Saga / Process Manager | `Application/Pipelines` | `Infrastructure` |

Request journey: `Controller → ISender → Behaviors (log→auth→validate→tx→
cache→perf) → CommandHandler → Strategy → Pipeline steps → Factory →
UoW.Commit → domain events → Notifications → Outbox → integration events`.

## Multi-Cloud Naming Rule

Interface names are **cloud-neutral**; implementation names carry the provider:

```
IObjectStorageService  →  AzureBlobStorageService
                       →  AwsS3Service
                       →  GcpCloudStorageService
                       →  AlibabaOSSService
                       →  OracleObjectStorageService
                       →  CloudflareR2Service        (S3-compatible endpoint)
                       →  LocalFileStorageService   (bare metal)
```

`appsettings.json → "CloudProvider": "Azure" | "AWS" | "GCP" | "Alibaba" |
"Oracle" | "Cloudflare" | "BareMetal"` selects which
`Add{Provider}Infrastructure()` runs in `Program.cs`.

Cloudflare is dual-runtime: C# `Infrastructure.Cloudflare` covers R2 / Queues /
KV / D1 / Workers-AI / `Tunnel/` (cloudflared API), while `workers/` (TypeScript)
and `cf-microservices/` (Go) are the edge/microservice runtimes that front it.

## CI/CD — Push to Published

```
push → PR checks (format + analyzers via run-analyzers action)
     → merge to develop → CI (analyze+generators, unit+arch tests,
       go tests, worker tests, security scan, docker images)
     → CD staging (version → pack NuGet/npm → DocFX build →
       k8s deploy → CF Workers → tunnel config → worker services → smoke)
     → merge to main → CD production (release → publish NuGet/npm →
       DocFX → GitHub Pages → blue/green deploy → post-deploy checks)
```

The tools layer gates the pipeline: `dotnet build` fires the Roslyn
analyzers and source generators; `Directory.Build.props` +
`TreatWarningsAsErrors=true` turns violations into CI failures. DocFX runs
`metadata` (XML comments → `api/*.yml`) then `build` (→ `docs/_site`) in
`docs.yml` / `cd-staging` / `cd-production`. `build/scripts/build.cake`
orchestrates the same steps locally or on the self-hosted agent.

## Domain Event vs Integration Event

- **Domain event** (`*.Domain/Events/`): raised by an entity, handled in-process.
- **Integration event** (`Shared.Contracts/Events/`): published via Dapr pub/sub,
  handled by *other* services. Primitives only — never domain objects.

## Usage

```bash
dotnet build CleanArch.slnx     # verify structure (0 errors)
bash scaffold.sh                # regenerate structure from scratch

# Flutter client (presentation/flutter):
flutter pub get                 # restore deps
dart analyze                    # must be clean
dart format .                   # format
flutter test                    # unit/widget tests
dart run build_runner build --delete-conflicting-outputs   # codegen
```

`.gitkeep` files mark every intended folder so the empty tree survives git.
Delete them as you add real files. Test projects are plain classlib stubs —
add your framework of choice (xUnit/NUnit + `Microsoft.NET.Test.Sdk`) when ready.


Structure Attribution: Office 365 Frontier Team