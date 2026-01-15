locals {
  name_prefix = "${var.queue_name_prefix}-${var.environment}"
  tags = merge(
    var.tags,
    {
      Module = "SQS"
    }
  )
}

# Main SQS Queue
resource "aws_sqs_queue" "main" {
  name                      = "${local.name_prefix}-queue"
  message_retention_seconds = var.message_retention_seconds
  visibility_timeout_seconds = var.visibility_timeout_seconds
  max_message_size          = var.max_message_size

  redrive_policy = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.dlq.arn
    maxReceiveCount     = 3
  })

  tags = merge(
    local.tags,
    { Name = "${local.name_prefix}-queue" }
  )
}

# Dead-Letter Queue
resource "aws_sqs_queue" "dlq" {
  name                      = "${local.name_prefix}-dlq"
  message_retention_seconds = var.message_retention_seconds

  tags = merge(
    local.tags,
    { Name = "${local.name_prefix}-dlq" }
  )
}

# SQS Queue Policy (allow messages from any service)
resource "aws_sqs_queue_policy" "main" {
  queue_url = aws_sqs_queue.main.id

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Principal = {
          AWS = "*"
        }
        Action   = "sqs:SendMessage"
        Resource = aws_sqs_queue.main.arn
      }
    ]
  })
}
