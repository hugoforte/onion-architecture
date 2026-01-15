output "alb_dns_name" {
  description = "ALB DNS name"
  value       = "https://${module.alb.alb_dns_name}"
}

output "cloudfront_domain" {
  description = "CloudFront domain"
  value       = try(module.s3_frontend.cloudfront_domain_name, "CloudFront not enabled")
}

output "rds_endpoint" {
  description = "RDS endpoint"
  value       = module.rds.db_instance_endpoint
}

output "ecs_cluster_name" {
  description = "ECS cluster name"
  value       = module.ecs.cluster_name
}

output "sqs_queue_url" {
  description = "SQS queue URL"
  value       = module.sqs.queue_url
}

output "s3_bucket" {
  description = "S3 bucket name"
  value       = module.s3_frontend.s3_bucket_id
}
