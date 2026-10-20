Add cloud infrastructure-as-code — baremetal/ already has Terraform +
Ansible, but the cloud providers have no IaC layer.

Cover:

- A `deploy/iac/` (or top-level `iac/`) structure: per-provider modules
  (azure/, aws/, gcp/, alibaba/, oracle/, cloudflare/), shared modules
  (network, k8s cluster, dns, secrets), and per-environment overlays
  (dev/staging/prod tfvars or equivalent).
- Tooling choice per provider and why: Bicep for Azure, CDK/Terraform for
  AWS/GCP, Terraform for Alibaba/Oracle/Cloudflare — one Terraform root
  with provider workspaces vs separate stacks.
- What each module provisions: the services each Infrastructure.{p}
  project consumes (event hubs, buckets, queues, registries, vaults,
  IAM roles), plus the cloudflared tunnel resource itself (CF tunnels are
  Terraform-managed — cf_tunnel resource).
- CI integration: plan on PR (comment diff), apply on merge to main,
  state backend per provider, drift detection schedule.
- How IaC outputs flow into Dapr component metadata and appsettings
  (generated config vs manual).
- Relationship to deploy/k8s/<provider>/ overlays and baremetal/terraform.

Standard format: mental model, tree, code (representative tf/bicep),
cheat sheet, connections.
