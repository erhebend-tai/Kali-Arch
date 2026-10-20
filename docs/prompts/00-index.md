# Prompt Set — Next Design Rounds

Paste-ready prompts for extending the CleanArch platform design. Each is
self-contained — paste one at a time into the assistant. Every prompt asks for
the established output format: mental model → folder structure tree →
representative code → "Where does it go?" cheat sheet → how it connects.

Recommended order (roughly priority / dependency order):

| # | Prompt | Adds |
|---|--------|------|
| 01 | Error handling & Result pattern | `Result<T>`/`Error`, exception→ProblemDetails mapping |
| 02 | Validation & mapping | FluentValidation `Validators/`, mapping `Profiles/` |
| 03 | Inbox & idempotency | InboxMessage dedupe, idempotent consumers/commands |
| 04 | Options & configuration | strongly-typed `Options/`, startup validation |
| 05 | Observability | OTel, dashboards, alerts, SLOs, health checks |
| 06 | Security layer | authZ policies, rate limiting, rotation, headers |
| 07 | Testing strategy | integration/contract/E2E/load/mutation/benchmarks |
| 08 | Cloud IaC | `deploy/iac/` Bicep/CDK/Terraform + plan/apply in CI |
| 09 | Event versioning & AsyncAPI | schema registry, V1/V2 contracts, `docs/asyncapi/` |
| 10 | Feature flags & multi-tenancy | flag providers, tenant resolution, isolation |
| 11 | Real-time & alternate protocols | SignalR, gRPC, GraphQL through the same pipeline |
| 12 | GitOps & DR | ArgoCD/Flux, sealed secrets, runbooks, backups |
| 13 | Developer experience | dotnet-tools.json, Makefile, git hooks |

Context to include if starting a fresh conversation:

> The platform is a multi-cloud Clean Architecture solution: a modular
> monolith (Domain / Application / Infrastructure.{Azure,AWS,GCP,Alibaba,
> Oracle,Cloudflare,BareMetal} / API / Functions) plus microservices under
> services/ (each a mini Clean Architecture), shared kernel under shared/,
> Cloudflare workers + Go microservices, cloudflared tunnels, bare-metal
> provisioning, Roslyn analyzers/codefixes/generators/linters, presentation
> apps (Blazor/MAUI/Avalonia/Flutter), MCP servers, CLIs, logging sinks,
> file watchers, .NET worker services, GitHub Actions CI/CD, and DocFX docs.
> Application uses MediatR-style Commands/Queries/Notifications/Behaviors,
> Pipelines (compensating steps), Chains, and Interfaces/Strategies. Domain
> uses Entities, ValueObjects, Events, Factories, Specifications, GoF
> Commands, Builders, and Templates.
