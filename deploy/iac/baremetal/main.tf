# Bare metal entry — k3s on supplied hosts via SSH + Ansible handoff.
# The heavy lifting lives in deploy/baremetal/ansible; Terraform
# provisions records/secrets inventory.

terraform {
  required_version = ">= 1.9"
  required_providers {
    local = { source = "hashicorp/local", version = "~> 2.5" }
  }
}

variable "hosts" {
  type    = list(string)
  default = ["10.0.0.10", "10.0.0.11"]
}
variable "ssh_user" { default = "deploy" }
variable "env"      { default = "dev" }

resource "local_file" "ansible_inventory" {
  filename = "${path.module}/../ansible/inventory.ini"
  content  = <<-EOT
    [platform]
    %{ for ip in var.hosts ~}
    ${ip} ansible_user=${var.ssh_user}
    %{ endfor ~}

    [platform:vars]
    env=${var.env}
    ansible_python_interpreter=/usr/bin/python3
  EOT
}

output "inventory_path" { value = local_file.ansible_inventory.filename }
