variable "aws_region" {
  description = "AWS region"
  type        = string
  default     = "us-east-1"
}

variable "environment" {
  description = "Environment name (dev, uat, prod)"
  type        = string
  validation {
    condition     = contains(["dev", "uat", "prod"], var.environment)
    error_message = "Environment must be dev, uat, or prod."
  }
}

variable "project_name" {
  description = "Project name"
  type        = string
  default     = "starter-todo-app"
}

variable "app_name" {
  description = "Application name"
  type        = string
  default     = "starter-api"
}

variable "container_port" {
  description = "Container port"
  type        = number
  default     = 5003
}

variable "container_image" {
  description = "Docker image URI for ECS task"
  type        = string
}

variable "database_name" {
  description = "RDS database name"
  type        = string
  default     = "starter"
}

variable "database_username" {
  description = "RDS master username"
  type        = string
  default     = "postgres"
  sensitive   = true
}

variable "database_password" {
  description = "RDS master password"
  type        = string
  sensitive   = true
}

variable "ecs_task_cpu" {
  description = "ECS task CPU (256, 512, 1024, 2048, 4096)"
  type        = number
  default     = 256
}

variable "ecs_task_memory" {
  description = "ECS task memory (MB)"
  type        = number
  default     = 512
}

variable "ecs_desired_count" {
  description = "Desired number of ECS tasks"
  type        = number
  default     = 1
}
