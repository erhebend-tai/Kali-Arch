Design the complete testing strategy — our test projects are currently
empty classlib stubs.

Cover each level, its tooling, folder location, and how CI runs it:

- **Unit tests** (exists): xunit + NSubstitute + FluentAssertions;
  Domain.Tests (entities/specs/factories), Application.Tests (handlers,
  behaviors, pipelines with fakes).
- **Architecture tests**: tools/linters/CleanArch.Linting already has
  ArchUnitNET-style rules — extend conventions.
- **Integration tests**: Testcontainers per infrastructure provider —
  Postgres/Mongo/RabbitMQ/Vault containers for BareMetal, Azurite/LocalStack
  emulators for Azure/AWS; shared test fixtures and WebApplicationFactory.
- **Contract tests**: Pact or equivalent between services — provider/
  consumer contract structure, where contracts live (tests/contracts?),
  and verification in CI.
- **E2E tests**: Playwright for the Blazor UI + REST E2E through the gateway.
- **Load tests**: k6 or NBomber — where scripts live, CI smoke-load vs
  scheduled full-load.
- **Mutation testing**: Stryker.NET config gating coverage quality.
- **Benchmarks**: BenchmarkDotNet for hot paths (serialization, pipelines).
- **Test data builders**: a Builders pattern for test fixtures (mirrors
  Domain Builders) and shared TestHelpers project placement.

Include a folder tree for tests/, per-service tests/, CI job wiring
(ci.yml additions), and a cheat sheet mapping test type → project → CI stage.

Standard format: mental model, tree, code, cheat sheet, connections.
