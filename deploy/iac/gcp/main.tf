# GCP entry — Cloud Run + Artifact Registry + Secret Manager.

terraform {
  required_version = ">= 1.9"
  required_providers {
    google = { source = "hashicorp/google", version = "~> 6.0" }
  }
}

provider "google" {
  project = var.project_id
  region  = var.region
}

variable "project_id" {}
variable "region"     { default = "us-east1" }
variable "env"        { default = "dev" }

locals {
  prefix = "cleanarch-${var.env}"
}

resource "google_artifact_registry_repository" "api" {
  repository_id = "${local.prefix}-api"
  format        = "DOCKER"
  location      = var.region
}

resource "google_cloud_run_v2_service" "api" {
  name     = "${local.prefix}-api"
  location = var.region

  template {
    containers {
      image = "${var.region}-docker.pkg.dev/${var.project_id}/${google_artifact_registry_repository.api.repository_id}/api:latest"
      env {
        name  = "CloudProvider"
        value = "GCP"
      }
      resources {
        limits = { cpu = "1", memory = "1Gi" }
      }
    }
    scaling {
      min_instance_count = var.env == "production" ? 1 : 0
      max_instance_count = 10
    }
  }
}

resource "google_secret_manager_secret" "db_connection" {
  secret_id = "${local.prefix}-db-connection"
  replication { auto {} }
}

output "api_url" { value = google_cloud_run_v2_service.api.uri }
