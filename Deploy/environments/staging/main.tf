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
      Environment = "staging"
      Project     = "todo-app"
      ManagedBy   = "Terraform"
    }
  }
}

module "vpc" {
  source = "../../modules/vpc"

  environment           = "staging"
  vpc_cidr              = var.vpc_cidr
  availability_zones    = var.availability_zones
  public_subnet_cidrs   = var.public_subnet_cidrs
  private_subnet_cidrs  = var.private_subnet_cidrs
  enable_nat_gateway    = var.enable_nat_gateway
  enable_flow_logs      = var.enable_flow_logs
}

module "iam" {
  source = "../../modules/iam"

  environment  = "staging"
  service_name = "todo"
}

module "alb" {
  source = "../../modules/alb"

  environment         = "staging"
  alb_name            = "${var.project_name}-staging-alb"
  vpc_id              = module.vpc.vpc_id
  subnet_ids          = module.vpc.public_subnet_ids
  target_group_name   = "${var.project_name}-staging-tg"
  container_port      = var.container_port
  health_check_path   = var.health_check_path
  certificate_arn     = var.certificate_arn_staging
}

module "rds" {
  source = "../../modules/rds"

  environment              = "staging"
  instance_class           = var.rds_instance_class_staging
  allocated_storage        = var.rds_allocated_storage_staging
  engine_version           = var.rds_engine_version
  db_name                  = var.rds_db_name
  master_username          = var.rds_master_username
  master_password          = var.rds_master_password
  multi_az                 = false
  backup_retention_days    = var.rds_backup_retention_days_staging
  vpc_id                   = module.vpc.vpc_id
  subnet_ids               = module.vpc.private_subnet_ids
  ecs_security_group_id    = module.ecs.security_group_id
  enable_monitoring        = true
  monitoring_role_arn      = module.iam.rds_monitoring_role_arn
  log_retention_days       = var.log_retention_days
}

module "ecs" {
  source = "../../modules/ecs"

  environment                  = "staging"
  cluster_name                 = "${var.project_name}-staging-cluster"
  task_family                  = "${var.project_name}-api"
  docker_image                 = var.docker_image_staging
  container_port               = var.container_port
  desired_count                = var.ecs_desired_count_staging
  cpu                          = var.ecs_cpu_staging
  memory                        = var.ecs_memory_staging
  vpc_id                       = module.vpc.vpc_id
  subnet_ids                   = module.vpc.private_subnet_ids
  alb_target_group_arn         = module.alb.target_group_arn
  ecs_task_execution_role_arn  = module.iam.ecs_task_execution_role_arn
  ecs_task_role_arn            = module.iam.ecs_task_role_arn
  log_retention_days           = var.log_retention_days
  environment_variables        = var.ecs_environment_variables
  secrets                      = var.ecs_secrets
  enable_autoscaling           = true
  min_capacity                 = var.ecs_min_capacity_staging
  max_capacity                 = var.ecs_max_capacity_staging
  target_cpu                   = var.target_cpu
}

module "s3_frontend" {
  source = "../../modules/s3_frontend"

  environment           = "staging"
  bucket_name           = var.s3_bucket_name_staging
  cloudfront_enabled    = var.cloudfront_enabled_staging
  certificate_arn       = var.certificate_arn_staging
  domain_name           = var.domain_name_staging
  cache_ttl_default     = var.cache_ttl_default
}

module "sqs" {
  source = "../../modules/sqs"

  environment              = "staging"
  queue_name_prefix        = var.project_name
  message_retention_seconds = var.sqs_message_retention_seconds
}

module "cloudwatch" {
  source = "../../modules/cloudwatch"

  environment                = "staging"
  log_retention_days         = var.log_retention_days
  alarm_threshold_cpu        = var.alarm_threshold_cpu
  alarm_threshold_memory     = var.alarm_threshold_memory
  alarm_threshold_error_rate = var.alarm_threshold_error_rate
  alarm_sns_topic_arn        = var.alarm_sns_topic_arn_staging
}

module "secrets" {
  source = "../../modules/secrets"

  environment  = "staging"
  secrets_map  = merge(
    {
      "database-password" = var.rds_master_password
      "jwt-secret"        = var.jwt_secret
    },
    var.additional_secrets_staging
  )
}
