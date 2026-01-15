# Development Environment Terraform Variables
# Update these values as needed for your development environment

aws_region                  = "us-east-1"
project_name                = "todo-app"

# RDS
rds_instance_class_dev      = "db.t3.micro"
rds_allocated_storage_dev   = 20
rds_backup_retention_days_dev = 7

# ECS
ecs_desired_count_dev       = 1
ecs_cpu_dev                 = 256
ecs_memory_dev              = 512

# S3
cloudfront_enabled_dev      = false

# ECR Image - Update with your actual image
# docker_image_dev          = "123456789.dkr.ecr.us-east-1.amazonaws.com/todo-app:latest"
