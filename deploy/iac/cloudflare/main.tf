# Cloudflare entry — Workers + Tunnel + R2 + KV.

terraform {
  required_version = ">= 1.9"
  required_providers {
    cloudflare = { source = "cloudflare/cloudflare", version = "~> 5.0" }
  }
}

variable "account_id" {}
variable "zone_id"    {}
variable "env"        { default = "dev" }

locals {
  prefix = "cleanarch-${var.env}"
}

resource "cloudflare_workers_script" "gateway" {
  account_id  = var.account_id
  script_name = "${local.prefix}-gateway"
  content     = file("${path.module}/../../../workers/cf-workers/gateway-worker/dist/index.js")
  main_module = "index.js"
}

resource "cloudflare_workers_kv_namespace" "config" {
  account_id = var.account_id
  title      = "${local.prefix}-config"
}

resource "cloudflare_r2_bucket" "storage" {
  account_id = var.account_id
  name       = "${local.prefix}-storage"
  location   = "ENAM"
}

resource "cloudflare_zero_trust_tunnel_cloudflared" "platform" {
  account_id = var.account_id
  name       = "${local.prefix}-tunnel"
}

output "tunnel_id" {
  value = cloudflare_zero_trust_tunnel_cloudflared.platform.id
}
