variable "environment" {
  description = "Environment name"
  type        = string
}

variable "bucket_name" {
  description = "S3 bucket name for frontend"
  type        = string
}

variable "cloudfront_enabled" {
  description = "Enable CloudFront distribution"
  type        = bool
  default     = true
}

variable "certificate_arn" {
  description = "SSL certificate ARN for CloudFront"
  type        = string
  default     = ""
}

variable "domain_name" {
  description = "Custom domain name for CloudFront"
  type        = string
  default     = ""
}

variable "cache_ttl_default" {
  description = "Default cache TTL in seconds"
  type        = number
  default     = 3600
}

variable "cache_ttl_max" {
  description = "Maximum cache TTL in seconds"
  type        = number
  default     = 86400
}

variable "cache_ttl_min" {
  description = "Minimum cache TTL in seconds"
  type        = number
  default     = 0
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}
