Design the GitOps and disaster-recovery layers.

**GitOps:**

- deploy/gitops/ structure for ArgoCD or Flux (recommend one, justify):
  app-of-apps layout, per-cluster bootstrap, sync waves for ordering
  (infra → dapr → services), and how it complements rather than replaces
  the existing cd-staging/cd-production kubectl steps.
- Secrets in GitOps: sealed-secrets or SOPS with the Vault/KeyVault
  backing — rotation flow connecting to the secrets rotation design.
- Environment promotion: dev → staging → prod as GitOps PRs vs
  pipeline-driven deploys — choose a model.

**Disaster recovery:**

- Backup strategy per provider and per datastore (VM snapshots, Postgres
  PITR, Mongo backups, blob/geo-redundancy, Dapr state).
- docs/runbooks/: restore runbook, regional failover runbook, RTO/RPO
  targets per service tier.
- Chaos/failover testing in CI (scheduled workflow).

Cover folder trees, representative YAML/runbook stubs, CI wiring, and a
cheat sheet.

Standard format: mental model, tree, code, cheat sheet, connections.
