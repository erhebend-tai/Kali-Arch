Design feature flags and multi-tenancy for the platform.

**Feature flags:**

- A `IFeatureFlagService` abstraction in Application/Interfaces with
  provider implementations: Azure App Configuration, a config-file/
  environment fallback for bare metal, and Cloudflare KV for the edge.
- Where flags get evaluated: MediatR behavior (FeatureFlagBehavior
  short-circuiting gated commands), controller attributes, worker gating,
  and the Flutter/Blazor clients reading flag state.
- Flag lifecycle: naming conventions, expiry, kill-switch flags vs
  rollout flags, and flags in CI (deploy with flag off → enable).

**Multi-tenancy:**

- Tenant model: TenantId in domain/auditable entities, tenant resolution
  middleware (subdomain? claim? header?), ITenantContext in Application,
  TenantEnricher already stubbed in logging.
- Data isolation strategies per provider: shared-schema row-filter vs
  schema-per-tenant vs database-per-tenant — recommend per provider and
  justify.
- Tenant-aware caching, outbox, and blob prefixes.

Cover folder trees, DI wiring, and a combined cheat sheet.

Standard format: mental model, tree, code, cheat sheet, connections.
