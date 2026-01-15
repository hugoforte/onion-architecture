# Production Environment Terraform Variables
# IMPORTANT: Update these values for production deployment
# Use strong passwords and store in AWS Secrets Manager

aws_region                      = "us-east-1"
project_name                    = "todo-app"

# RDS - Production Configuration
rds_instance_class_prod         = "db.t3.medium"
rds_allocated_storage_prod      = 100
rds_backup_retention_days_prod  = 30

# ECS - Production Configuration (High Availability)
ecs_desired_count_prod          = 3
ecs_cpu_prod                    = 512
ecs_memory_prod                 = 1024
ecs_min_capacity_prod           = 3
ecs_max_capacity_prod           = 10

# S3/CloudFront - Production Configuration
cloudfront_enabled_prod         = true

# CloudWatch - Production Configuration
log_retention_days              = 30
alarm_threshold_cpu             = 70
alarm_threshold_memory          = 70
alarm_threshold_error_rate      = 1

# NOTE: The following MUST be set before applying:
# - certificate_arn_prod: ACM certificate for HTTPS/CloudFront
# - alarm_sns_topic_arn_prod: SNS topic for alarm notifications
# - docker_image_prod: ECR image URI for backend API
# - rds_master_password: Strong password (stored in Secrets Manager)
# - jwt_secret: JWT signing secret (stored in Secrets Manager)
# - domain_name_prod: Custom domain for CloudFront (if applicable)
