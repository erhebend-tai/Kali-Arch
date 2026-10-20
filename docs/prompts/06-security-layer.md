Design the security layer for the platform — implementation structure, not
just the identity interfaces we have.

Cover:

- **Authentication**: JWT bearer wiring per provider (Entra ID, Cognito,
  IAM, Keycloak, Workers auth-worker-issued tokens) — one scheme shape,
  provider-specific validation. Where the config lives.
- **Authorization**: policy-based authZ — `Application/Security/` (or
  API/Security/?) with requirements, handlers, and resource-based checks
  (e.g., can this user manage VMs in this resource group). How it composes
  with the MediatR AuthorizationBehavior.
- **API surface hardening**: rate limiting (per-user/per-tenant), CORS
  policy structure, security-headers middleware, request size limits.
- **Secrets lifecycle**: rotation strategy through ISecretStoreService
  (Key Vault / Secrets Manager / Vault / KMS), rotation workers, and how
  the baremetal/scripts/rotate-secrets.sh relates.
- **API keys for machine clients** vs OAuth2 client credentials — where
  key issuance/validation lives.
- **Threat model + security docs** under docs/security/, and security
  test conventions (authZ test matrix).
- Audit logging: who did what, feeding the existing logging layer.

Standard format: mental model, tree, code, cheat sheet, connections.
