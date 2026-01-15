variable "aws_region" {
  description = "AWS region"
  type        = string
  default     = "us-east-1"
}

variable "project_name" {
  description = "Project name"
  type        = string
  default     = "todo-app"
}

# VPC Variables
variable "vpc_cidr" {
  description = "VPC CIDR block"
  type        = string
  default     = "10.0.0.0/16"
}

variable "availability_zones" {
  description = "Availability zones"
  type        = list(string)
  default     = ["us-east-1a", "us-east-1b"]
}

variable "public_subnet_cidrs" {
  description = "Public subnet CIDR blocks"
  type        = list(string)
  default     = ["10.0.1.0/24", "10.0.2.0/24"]
}

variable "private_subnet_cidrs" {
  description = "Private subnet CIDR blocks"
  type        = list(string)
  default     = ["10.0.10.0/24", "10.0.11.0/24"]
}

variable "enable_nat_gateway" {
  description = "Enable NAT Gateway"
  type        = bool
  default     = true
}

variable "enable_flow_logs" {
  description = "Enable VPC Flow Logs"
  type        = bool
  default     = false
}

# ALB Variables
variable "certificate_arn_dev" {
  description = "SSL certificate ARN for dev"
  type        = string
  default     = ""
}

variable "health_check_path" {
  description = "Health check path"
  type        = string
  default     = "/"
}

variable "container_port" {
  description = "Container port"
  type        = number
  default     = 5273
}

# RDS Variables
variable "rds_instance_class_dev" {
  description = "RDS instance class for dev"
  type        = string
  default     = "db.t3.micro"
}

variable "rds_allocated_storage_dev" {
  description = "RDS allocated storage for dev (GB)"
  type        = number
  default     = 20
}

variable "rds_engine_version" {
  description = "PostgreSQL engine version"
  type        = string
  default     = "15.3"
}

variable "rds_db_name" {
  description = "Database name"
  type        = string
  default     = "todoapp"
}

variable "rds_master_username" {
  description = "RDS master username"
  type        = string
  default     = "admin"
}

variable "rds_master_password" {
  description = "RDS master password"
  type        = string
  sensitive   = true
}

variable "rds_backup_retention_days_dev" {
  description = "RDS backup retention days for dev"
  type        = number
  default     = 7
}

# ECS Variables
variable "docker_image_dev" {
  description = "Docker image URI for dev"
  type        = string
  default     = "123456789.dkr.ecr.us-east-1.amazonaws.com/todo-app:latest"
}

variable "ecs_desired_count_dev" {
  description = "ECS desired count for dev"
  type        = number
  default     = 1
}

variable "ecs_cpu_dev" {
  description = "ECS CPU for dev"
  type        = number
  default     = 256
}

variable "ecs_memory_dev" {
  description = "ECS memory for dev"
  type        = number
  default     = 512
}

variable "ecs_environment_variables" {
  description = "ECS environment variables"
  type        = map(string)
  default = {
    ENVIRONMENT = "development"
    LOG_LEVEL   = "Debug"
  }
}

variable "ecs_secrets" {
  description = "ECS secrets from Secrets Manager"
  type        = map(string)
  default     = {}
}

# S3/CloudFront Variables
variable "s3_bucket_name_dev" {
  description = "S3 bucket name for dev"
  type        = string
  default     = "todo-app-dev-frontend"
}

variable "cloudfront_enabled_dev" {
  description = "Enable CloudFront for dev"
  type        = bool
  default     = false
}

variable "domain_name_dev" {
  description = "Domain name for dev"
  type        = string
  default     = ""
}

variable "cache_ttl_default" {
  description = "Default cache TTL"
  type        = number
  default     = 3600
}

# SQS Variables
variable "sqs_message_retention_seconds" {
  description = "SQS message retention seconds"
  type        = number
  default     = 345600
}

# CloudWatch Variables
variable "log_retention_days" {
  description = "Log retention days"
  type        = number
  default     = 7
}

variable "alarm_threshold_cpu" {
  description = "CPU alarm threshold"
  type        = number
  default     = 80
}

variable "alarm_threshold_memory" {
  description = "Memory alarm threshold"
  type        = number
  default     = 80
}

variable "alarm_threshold_error_rate" {
  description = "Error rate alarm threshold"
  type        = number
  default     = 10
}

variable "alarm_sns_topic_arn_dev" {
  description = "SNS topic ARN for alarms in dev"
  type        = string
  default     = ""
}

# Secrets Variables
variable "jwt_secret" {
  description = "JWT secret"
  type        = string
  sensitive   = true
  default     = ""
}

variable "additional_secrets_dev" {
  description = "Additional secrets for dev"
  type        = map(string)
  sensitive   = true
  default     = {}
}
