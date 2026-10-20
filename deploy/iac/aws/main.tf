# AWS entry — composes the platform on ECS Fargate.
# deploy: terraform init && terraform apply -var="env=dev"

terraform {
  required_version = ">= 1.9"
  required_providers {
    aws = { source = "hashicorp/aws", version = "~> 5.0" }
  }
  backend "s3" {
    # Configure per environment: bucket/key/region.
  }
}

provider "aws" {
  region = var.region
}

variable "env"    { default = "dev" }
variable "region" { default = "us-east-1" }

locals {
  prefix = "cleanarch-${var.env}"
}

resource "aws_ecs_cluster" "platform" {
  name = "${local.prefix}-cluster"
}

resource "aws_cloudwatch_log_group" "api" {
  name              = "/ecs/${local.prefix}-api"
  retention_in_days = var.env == "production" ? 90 : 30
}

resource "aws_ecs_task_definition" "api" {
  family                   = "${local.prefix}-api"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = 512
  memory                   = 1024

  container_definitions = jsonencode([
    {
      name      = "api"
      image     = "${aws_ecr_repository.api.repository_url}:latest"
      essential = true
      portMappings = [{ containerPort = 8080 }]
      environment = [
        { name = "CloudProvider", value = "AWS" }
      ]
      logConfiguration = {
        logDriver = "awslogs"
        options = {
          "awslogs-group"         = aws_cloudwatch_log_group.api.name
          "awslogs-region"        = var.region
          "awslogs-stream-prefix" = "api"
        }
      }
    }
  ])
}

resource "aws_ecr_repository" "api" {
  name                 = "${local.prefix}/api"
  image_tag_mutability = "MUTABLE"
}

resource "aws_secretsmanager_secret" "db" {
  name = "${local.prefix}/db-connection"
}

output "ecr_repository_url" { value = aws_ecr_repository.api.repository_url }
