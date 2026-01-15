# Staging Environment Terraform Variables
# Update these values as needed for your staging environment

aws_region                      = "us-east-1"
project_name                    = "todo-app"

# RDS
rds_instance_class_staging      = "db.t3.small"
rds_allocated_storage_staging   = 50
rds_backup_retention_days_staging = 14

# ECS
ecs_desired_count_staging       = 2
ecs_cpu_staging                 = 256
ecs_memory_staging              = 512
ecs_min_capacity_staging        = 2
ecs_max_capacity_staging        = 4

# S3
cloudfront_enabled_staging      = true

# CloudWatch
log_retention_days              = 14
