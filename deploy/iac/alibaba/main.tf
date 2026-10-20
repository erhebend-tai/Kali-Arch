# Alibaba Cloud entry — SAE (Serverless App Engine) + ACR + KMS.

terraform {
  required_version = ">= 1.9"
  required_providers {
    alicloud = { source = "aliyun/alicloud", version = "~> 1.240" }
  }
}

provider "alicloud" {
  region = var.region
}

variable "region" { default = "cn-hangzhou" }
variable "env"    { default = "dev" }

locals {
  prefix = "cleanarch-${var.env}"
}

resource "alicloud_sae_application" "api" {
  app_name          = "${local.prefix}-api"
  namespace_id      = alicloud_sae_namespace.platform.id
  package_type      = "Image"
  image_url         = "${local.prefix}-registry.cn-hangzhou.aliyuncs.com/platform/api:latest"
  replicas          = var.env == "production" ? 2 : 1
  cpu               = 500
  memory            = 1024
  envs = jsonencode([
    { name = "CloudProvider", value = "Alibaba" }
  ])
}

resource "alicloud_sae_namespace" "platform" {
  namespace_id          = "${var.region}:${local.prefix}"
  namespace_name        = local.prefix
  namespace_description = "CleanArch platform — ${var.env}"
}

output "app_name" { value = alicloud_sae_application.api.app_name }
