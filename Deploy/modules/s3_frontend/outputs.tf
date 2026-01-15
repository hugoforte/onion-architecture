output "s3_bucket_id" {
  description = "S3 bucket ID"
  value       = aws_s3_bucket.frontend.id
}

output "s3_bucket_arn" {
  description = "S3 bucket ARN"
  value       = aws_s3_bucket.frontend.arn
}

output "s3_bucket_domain" {
  description = "S3 bucket regional domain"
  value       = aws_s3_bucket.frontend.bucket_regional_domain_name
}

output "cloudfront_distribution_id" {
  description = "CloudFront distribution ID"
  value       = try(aws_cloudfront_distribution.frontend[0].id, "")
}

output "cloudfront_domain_name" {
  description = "CloudFront distribution domain name"
  value       = try(aws_cloudfront_distribution.frontend[0].domain_name, "")
}

output "origin_access_identity_arn" {
  description = "CloudFront OAI ARN"
  value       = aws_cloudfront_origin_access_identity.frontend.iam_arn
}
