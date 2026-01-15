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
  default     = true
}

# ALB Variables
variable "certificate_arn_prod" {
  description = "SSL certificate ARN for prod"
  type        = string
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
variable "rds_instance_class_prod" {
  description = "RDS instance class for prod"
  type        = string
  default     = "db.t3.medium"
}

variable "rds_allocated_storage_prod" {
  description = "RDS allocated storage for prod (GB)"
  type        = number
  default     = 100
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

variable "rds_backup_retention_days_prod" {
  description = "RDS backup retention days for prod"
  type        = number
  default     = 30
}

# ECS Variables
variable "docker_image_prod" {
  description = "Docker image URI for prod"
  type        = string
}

variable "ecs_desired_count_prod" {
  description = "ECS desired count for prod"
  type        = number
  default     = 3
}

variable "ecs_cpu_prod" {
  description = "ECS CPU for prod"
  type        = number
  default     = 512
}

variable "ecs_memory_prod" {
  description = "ECS memory for prod"
  type        = number
  default     = 1024
}

variable "ecs_min_capacity_prod" {
  description = "ECS minimum capacity for prod"
  type        = number
  default     = 3
}

variable "ecs_max_capacity_prod" {
  description = "ECS maximum capacity for prod"
  type        = number
  default     = 10
}

variable "target_cpu" {
  description = "Target CPU utilization for auto-scaling"
  type        = number
  default     = 70
}

variable "ecs_environment_variables" {
  description = "ECS environment variables"
  type        = map(string)
  default = {
    ENVIRONMENT = "production"
    LOG_LEVEL   = "Warning"
  }
}

variable "ecs_secrets" {
  description = "ECS secrets from Secrets Manager"
  type        = map(string)
  default     = {}
}

# S3/CloudFront Variables
variable "s3_bucket_name_prod" {
  description = "S3 bucket name for prod"
  type        = string
  default     = "todo-app-prod-frontend"
}

variable "cloudfront_enabled_prod" {
  description = "Enable CloudFront for prod"
  type        = bool
  default     = true
}

variable "domain_name_prod" {
  description = "Domain name for prod"
  type        = string
  default     = ""
}

variable "cache_ttl_default" {
  description = "Default cache TTL"
  type        = number
  default     = 86400
}

# SQS Variables
variable "sqs_message_retention_seconds" {
  description = "SQS message retention seconds"
  type        = number
  default     = 1209600 # 14 days
}

# CloudWatch Variables
variable "log_retention_days" {
  description = "Log retention days"
  type        = number
  default     = 30
}

variable "alarm_threshold_cpu" {
  description = "CPU alarm threshold"
  type        = number
  default     = 70
}

variable "alarm_threshold_memory" {
  description = "Memory alarm threshold"
  type        = number
  default     = 70
}

variable "alarm_threshold_error_rate" {
  description = "Error rate alarm threshold"
  type        = number
  default     = 1
}

variable "alarm_sns_topic_arn_prod" {
  description = "SNS topic ARN for alarms in prod"
  type        = string
}

# Secrets Variables
variable "jwt_secret" {
  description = "JWT secret"
  type        = string
  sensitive   = true
}

variable "additional_secrets_prod" {
  description = "Additional secrets for prod"
  type        = map(string)
  sensitive   = true
  default     = {}
}
