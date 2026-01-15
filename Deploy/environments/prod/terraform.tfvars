environment = "prod"
aws_region  = "us-east-1"

# Update with your container image from ECR
container_image = "REPLACE_WITH_ECR_IMAGE_URI"

# Use AWS Secrets Manager or Parameter Store for production credentials
database_username = "postgres"
database_password = "prod_password_change_me"

ecs_task_cpu      = 1024
ecs_task_memory   = 2048
ecs_desired_count = 3
