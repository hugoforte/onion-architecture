locals {
  name_prefix = "todo-${var.environment}"
  tags = merge(
    var.tags,
    {
      Module = "CloudWatch"
    }
  )
}

# CloudWatch Log Group for Application
resource "aws_cloudwatch_log_group" "application" {
  name              = "/aws/application/${local.name_prefix}"
  retention_in_days = var.log_retention_days

  tags = merge(
    local.tags,
    { Name = "${local.name_prefix}-app-lg" }
  )
}

# CloudWatch Dashboard
resource "aws_cloudwatch_dashboard" "main" {
  dashboard_name = "${local.name_prefix}-dashboard"

  dashboard_body = jsonencode({
    widgets = [
      {
        type = "metric"
        properties = {
          metrics = [
            ["AWS/ECS", "CPUUtilization", { stat = "Average" }],
            [".", "MemoryUtilization", { stat = "Average" }]
          ]
          period = 300
          stat   = "Average"
          region = data.aws_region.current.name
          title  = "ECS Performance"
        }
      },
      {
        type = "metric"
        properties = {
          metrics = [
            ["AWS/RDS", "CPUUtilization", { stat = "Average" }],
            [".", "DatabaseConnections", { stat = "Sum" }]
          ]
          period = 300
          stat   = "Average"
          region = data.aws_region.current.name
          title  = "RDS Performance"
        }
      },
      {
        type = "log"
        properties = {
          query   = "fields @timestamp, @message | stats count() as error_count by @message"
          region  = data.aws_region.current.name
          title   = "Application Errors"
        }
      }
    ]
  })
}

# Get current AWS region
data "aws_region" "current" {}

# SNS Topic for Alarms (if not provided)
resource "aws_sns_topic" "alarms" {
  count = var.alarm_sns_topic_arn == "" ? 1 : 0
  name  = "${local.name_prefix}-alarms"

  tags = local.tags
}

# CloudWatch Alarm - ECS CPU
resource "aws_cloudwatch_metric_alarm" "ecs_cpu" {
  alarm_name          = "${local.name_prefix}-ecs-cpu-high"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = "2"
  metric_name         = "CPUUtilization"
  namespace           = "AWS/ECS"
  period              = "300"
  statistic           = "Average"
  threshold           = var.alarm_threshold_cpu
  alarm_description   = "Alert when ECS CPU exceeds threshold"
  alarm_actions       = [try(aws_sns_topic.alarms[0].arn, var.alarm_sns_topic_arn)]

  tags = local.tags
}

# CloudWatch Alarm - ECS Memory
resource "aws_cloudwatch_metric_alarm" "ecs_memory" {
  alarm_name          = "${local.name_prefix}-ecs-memory-high"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = "2"
  metric_name         = "MemoryUtilization"
  namespace           = "AWS/ECS"
  period              = "300"
  statistic           = "Average"
  threshold           = var.alarm_threshold_memory
  alarm_description   = "Alert when ECS memory exceeds threshold"
  alarm_actions       = [try(aws_sns_topic.alarms[0].arn, var.alarm_sns_topic_arn)]

  tags = local.tags
}

# CloudWatch Alarm - RDS CPU
resource "aws_cloudwatch_metric_alarm" "rds_cpu" {
  alarm_name          = "${local.name_prefix}-rds-cpu-high"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = "2"
  metric_name         = "CPUUtilization"
  namespace           = "AWS/RDS"
  period              = "300"
  statistic           = "Average"
  threshold           = var.alarm_threshold_cpu
  alarm_description   = "Alert when RDS CPU exceeds threshold"
  alarm_actions       = [try(aws_sns_topic.alarms[0].arn, var.alarm_sns_topic_arn)]

  tags = local.tags
}
