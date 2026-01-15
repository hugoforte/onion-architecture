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
  region = var.aws_region

  default_tags {
    tags = {
      Environment = "prod"
      Project     = "todo-app"
      ManagedBy   = "Terraform"
    }
  }
}

module "vpc" {
  source = "../../modules/vpc"

  environment           = "prod"
  vpc_cidr              = var.vpc_cidr
  availability_zones    = var.availability_zones
  public_subnet_cidrs   = var.public_subnet_cidrs
  private_subnet_cidrs  = var.private_subnet_cidrs
  enable_nat_gateway    = var.enable_nat_gateway
  enable_flow_logs      = var.enable_flow_logs
}

module "iam" {
  source = "../../modules/iam"

  environment  = "prod"
  service_name = "todo"
}

module "alb" {
  source = "../../modules/alb"

  environment         = "prod"
  alb_name            = "${var.project_name}-prod-alb"
  vpc_id              = module.vpc.vpc_id
  subnet_ids          = module.vpc.public_subnet_ids
  target_group_name   = "${var.project_name}-prod-tg"
  container_port      = var.container_port
  health_check_path   = var.health_check_path
  certificate_arn     = var.certificate_arn_prod
}

module "rds" {
  source = "../../modules/rds"

  environment              = "prod"
  instance_class           = var.rds_instance_class_prod
  allocated_storage        = var.rds_allocated_storage_prod
  engine_version           = var.rds_engine_version
  db_name                  = var.rds_db_name
  master_username          = var.rds_master_username
  master_password          = var.rds_master_password
  multi_az                 = true
  backup_retention_days    = var.rds_backup_retention_days_prod
  vpc_id                   = module.vpc.vpc_id
  subnet_ids               = module.vpc.private_subnet_ids
  ecs_security_group_id    = module.ecs.security_group_id
  enable_monitoring        = true
  monitoring_role_arn      = module.iam.rds_monitoring_role_arn
  log_retention_days       = var.log_retention_days
}

module "ecs" {
  source = "../../modules/ecs"

  environment                  = "prod"
  cluster_name                 = "${var.project_name}-prod-cluster"
  task_family                  = "${var.project_name}-api"
  docker_image                 = var.docker_image_prod
  container_port               = var.container_port
  desired_count                = var.ecs_desired_count_prod
  cpu                          = var.ecs_cpu_prod
  memory                        = var.ecs_memory_prod
  vpc_id                       = module.vpc.vpc_id
  subnet_ids                   = module.vpc.private_subnet_ids
  alb_target_group_arn         = module.alb.target_group_arn
  ecs_task_execution_role_arn  = module.iam.ecs_task_execution_role_arn
  ecs_task_role_arn            = module.iam.ecs_task_role_arn
  log_retention_days           = var.log_retention_days
  environment_variables        = var.ecs_environment_variables
  secrets                      = var.ecs_secrets
  enable_autoscaling           = true
  min_capacity                 = var.ecs_min_capacity_prod
  max_capacity                 = var.ecs_max_capacity_prod
  target_cpu                   = var.target_cpu
}

module "s3_frontend" {
  source = "../../modules/s3_frontend"

  environment           = "prod"
  bucket_name           = var.s3_bucket_name_prod
  cloudfront_enabled    = var.cloudfront_enabled_prod
  certificate_arn       = var.certificate_arn_prod
  domain_name           = var.domain_name_prod
  cache_ttl_default     = var.cache_ttl_default
}

module "sqs" {
  source = "../../modules/sqs"

  environment              = "prod"
  queue_name_prefix        = var.project_name
  message_retention_seconds = var.sqs_message_retention_seconds
}

module "cloudwatch" {
  source = "../../modules/cloudwatch"

  environment                = "prod"
  log_retention_days         = var.log_retention_days
  alarm_threshold_cpu        = var.alarm_threshold_cpu
  alarm_threshold_memory     = var.alarm_threshold_memory
  alarm_threshold_error_rate = var.alarm_threshold_error_rate
  alarm_sns_topic_arn        = var.alarm_sns_topic_arn_prod
}

module "secrets" {
  source = "../../modules/secrets"

  environment  = "prod"
  secrets_map  = merge(
    {
      "database-password" = var.rds_master_password
      "jwt-secret"        = var.jwt_secret
    },
    var.additional_secrets_prod
  )
}
