environment = "dev"
aws_region  = "us-east-1"

# Update with your container image from ECR
# Example: 123456789012.dkr.ecr.us-east-1.amazonaws.com/starter-todo-app/starter-api:latest
container_image = "REPLACE_WITH_ECR_IMAGE_URI"

# Database credentials - use AWS Secrets Manager in production!
database_username = "postgres"
database_password = "dev_password_change_me"

ecs_task_cpu      = 256
ecs_task_memory   = 512
ecs_desired_count = 1
