# Backup & Restore Runbook

## Backup schedule

| Store        | Method                      | Retention |
|--------------|-----------------------------|-----------|
| SQL database | PITR / automated snapshot   | 35 days   |
| Object store | Versioning + lifecycle rule | 90 days   |
| Secrets      | Versioned rotation          | Last 3    |
| Outbox table | Included in DB backup       | —         |

## Quarterly restore drill

1. Provision an isolated restore environment.
2. Restore the latest production snapshot to a scratch DB.
3. Point a scratch deployment at it (`CloudProvider` stays
   the same — no app changes needed).
4. Run `build/scripts/smoke-test.sh` against the scratch API.
5. Record RTO measured in the drill log; file gaps as issues.

## Partial restore (single tenant)

With database-per-tenant or schema-per-tenant isolation
(`MultiTenancy/`), restore only the affected tenant's schema
or database — other tenants are untouched.

## What is NOT backed up

- L1/L2 cache contents (warm on demand).
- In-flight queue messages older than the DLQ window.
- Ephemeral worker state — workers are idempotent.
