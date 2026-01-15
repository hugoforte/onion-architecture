environment = "uat"
aws_region  = "us-east-1"

# Update with your container image from ECR
container_image = "REPLACE_WITH_ECR_IMAGE_URI"

database_username = "postgres"
database_password = "uat_password_change_me"

ecs_task_cpu      = 512
ecs_task_memory   = 1024
ecs_desired_count = 2
