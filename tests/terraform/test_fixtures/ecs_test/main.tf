terraform {
  required_version = ">= 1.5"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
}

provider "aws" {
  region = "us-east-1"

  endpoints {
    ec2 = "http://localhost:4566"
    iam = "http://localhost:4566"
    ecs = "http://localhost:4566"
  }

  skip_credentials_validation = true
  skip_metadata_api_check     = true
  skip_requesting_account_id  = true
}

module "vpc" {
  source = "../../../modules/vpc"

  environment           = "test"
  vpc_cidr              = "10.0.0.0/16"
  availability_zones    = ["us-east-1a", "us-east-1b"]
  public_subnet_cidrs   = ["10.0.1.0/24", "10.0.2.0/24"]
  private_subnet_cidrs  = ["10.0.10.0/24", "10.0.11.0/24"]
  enable_nat_gateway    = false
  enable_flow_logs      = false
}

module "iam" {
  source = "../../../modules/iam"

  environment  = "test"
  service_name = "todo"
}

module "alb" {
  source = "../../../modules/alb"

  environment       = "test"
  alb_name          = "todo-test-alb"
  vpc_id            = module.vpc.vpc_id
  subnet_ids        = module.vpc.public_subnet_ids
  target_group_name = "todo-test-tg"
  container_port    = 5273
}

module "ecs" {
  source = "../../../modules/ecs"

  environment                  = "test"
  cluster_name                 = "todo-test-cluster"
  task_family                  = "todo-api"
  docker_image                 = "nginx:latest"
  container_port               = 5273
  desired_count                = 1
  cpu                          = 256
  memory                        = 512
  vpc_id                       = module.vpc.vpc_id
  subnet_ids                   = module.vpc.private_subnet_ids
  alb_target_group_arn         = module.alb.target_group_arn
  ecs_task_execution_role_arn  = module.iam.ecs_task_execution_role_arn
  ecs_task_role_arn            = module.iam.ecs_task_role_arn
  enable_autoscaling           = false
}
