locals {
  name_prefix = "todo-${var.environment}"
  tags = merge(
    var.tags,
    {
      Module = "S3Frontend"
    }
  )
  has_certificate = var.certificate_arn != ""
  has_domain      = var.domain_name != ""
}

# S3 Bucket for Frontend
resource "aws_s3_bucket" "frontend" {
  bucket = var.bucket_name

  tags = merge(
    local.tags,
    { Name = "${local.name_prefix}-frontend-bucket" }
  )
}

# Block Public Access
resource "aws_s3_bucket_public_access_block" "frontend" {
  bucket = aws_s3_bucket.frontend.id

  block_public_acls       = true
  block_public_policy     = true
  ignore_public_acls      = true
  restrict_public_buckets = true
}

# Versioning
resource "aws_s3_bucket_versioning" "frontend" {
  bucket = aws_s3_bucket.frontend.id

  versioning_configuration {
    status = "Enabled"
  }
}

# Server-side Encryption
resource "aws_s3_bucket_server_side_encryption_configuration" "frontend" {
  bucket = aws_s3_bucket.frontend.id

  rule {
    apply_server_side_encryption_by_default {
      sse_algorithm = "AES256"
    }
  }
}

# Origin Access Identity for CloudFront
resource "aws_cloudfront_origin_access_identity" "frontend" {
  comment = "OAI for ${local.name_prefix} frontend"
}

# S3 Bucket Policy
resource "aws_s3_bucket_policy" "frontend" {
  bucket = aws_s3_bucket.frontend.id

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Sid    = "AllowCloudFrontAccess"
        Effect = "Allow"
        Principal = {
          AWS = aws_cloudfront_origin_access_identity.frontend.iam_arn
        }
        Action   = "s3:GetObject"
        Resource = "${aws_s3_bucket.frontend.arn}/*"
      },
      {
        Sid    = "AllowListBucket"
        Effect = "Allow"
        Principal = {
          AWS = aws_cloudfront_origin_access_identity.frontend.iam_arn
        }
        Action   = "s3:ListBucket"
        Resource = aws_s3_bucket.frontend.arn
      }
    ]
  })
}

# CloudFront Distribution
resource "aws_cloudfront_distribution" "frontend" {
  count   = var.cloudfront_enabled ? 1 : 0
  enabled = true

  origin {
    domain_name = aws_s3_bucket.frontend.bucket_regional_domain_name
    origin_id   = "S3Frontend"

    s3_origin_config {
      origin_access_identity = aws_cloudfront_origin_access_identity.frontend.cloudfront_access_identity_path
    }
  }

  default_cache_behavior {
    allowed_methods  = ["GET", "HEAD", "OPTIONS"]
    cached_methods   = ["GET", "HEAD"]
    target_origin_id = "S3Frontend"

    cache_policy_id = aws_cloudfront_cache_policy.frontend[0].id

    viewer_protocol_policy = "redirect-to-https"
  }

  dynamic "aliases" {
    for_each = local.has_domain ? [var.domain_name] : []
    content {
      items = [aliases.value]
    }
  }

  viewer_certificate {
    cloudfront_default_certificate = !local.has_certificate
    acm_certificate_arn            = local.has_certificate ? var.certificate_arn : null
    ssl_support_method             = local.has_certificate ? "sni-only" : null
    minimum_protocol_version       = local.has_certificate ? "TLSv1.2_2021" : null
  }

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }

  default_root_object = "index.html"

  custom_error_response {
    error_code            = 404
    response_code         = 200
    response_page_path    = "/index.html"
    error_caching_min_ttl = 0
  }

  tags = merge(
    local.tags,
    { Name = "${local.name_prefix}-cloudfront" }
  )
}

# CloudFront Cache Policy
resource "aws_cloudfront_cache_policy" "frontend" {
  count   = var.cloudfront_enabled ? 1 : 0
  name    = "${local.name_prefix}-cache-policy"
  comment = "Cache policy for frontend"

  default_ttl = var.cache_ttl_default
  max_ttl     = var.cache_ttl_max
  min_ttl     = var.cache_ttl_min

  parameters_in_cache_key_and_forwarded_to_origin {
    enable_accept_encoding_gzip   = true
    enable_accept_encoding_brotli = true

    query_strings_config {
      query_string_behavior = "none"
    }

    headers_config {
      header_behavior = "none"
    }

    cookies_config {
      cookie_behavior = "none"
    }
  }
}
