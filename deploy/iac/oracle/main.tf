# OCI entry — Container Instances + Vault + Logging.

terraform {
  required_version = ">= 1.9"
  required_providers {
    oci = { source = "oracle/oci", version = "~> 6.0" }
  }
}

provider "oci" {
  tenancy_ocid = var.tenancy_ocid
  region       = var.region
}

variable "tenancy_ocid"   {}
variable "compartment_id" {}
variable "region"         { default = "us-ashburn-1" }
variable "env"            { default = "dev" }

locals {
  prefix = "cleanarch-${var.env}"
}

resource "oci_container_instances_container_instance" "api" {
  compartment_id      = var.compartment_id
  availability_domain = data.oci_identity_availability_domains.ads.availability_domains[0].name
  display_name        = "${local.prefix}-api"
  shape               = "CI.Standard.E4.Flex"
  shape_config {
    ocpus         = 1
    memory_in_gbs = 4
  }
  containers {
    display_name = "api"
    image_url    = "${var.region}.ocir.io/tenancy/${local.prefix}/api:latest"
    environment_variables = {
      CloudProvider = "Oracle"
    }
  }
}

data "oci_identity_availability_domains" "ads" {
  compartment_id = var.tenancy_ocid
}

output "container_instance_id" {
  value = oci_container_instances_container_instance.api.id
}
