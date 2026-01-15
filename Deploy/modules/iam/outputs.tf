output "ecs_task_execution_role_arn" {
  description = "ARN of ECS task execution role"
  value       = aws_iam_role.ecs_task_execution_role.arn
}

output "ecs_task_role_arn" {
  description = "ARN of ECS task role"
  value       = aws_iam_role.ecs_task_role.arn
}

output "rds_monitoring_role_arn" {
  description = "ARN of RDS monitoring role"
  value       = aws_iam_role.rds_monitoring_role.arn
}

output "lambda_execution_role_arn" {
  description = "ARN of Lambda execution role"
  value       = aws_iam_role.lambda_execution_role.arn
}

output "s3_access_role_arn" {
  description = "ARN of S3 access role"
  value       = aws_iam_role.s3_access_role.arn
}
