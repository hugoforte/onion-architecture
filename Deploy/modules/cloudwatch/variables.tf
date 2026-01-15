variable "environment" {
  description = "Environment name"
  type        = string
}

variable "log_retention_days" {
  description = "Log retention in days"
  type        = number
  default     = 7
}

variable "alarm_threshold_cpu" {
  description = "CPU alarm threshold percentage"
  type        = number
  default     = 80
}

variable "alarm_threshold_memory" {
  description = "Memory alarm threshold percentage"
  type        = number
  default     = 80
}

variable "alarm_threshold_error_rate" {
  description = "Error rate threshold percentage"
  type        = number
  default     = 10
}

variable "alarm_sns_topic_arn" {
  description = "SNS topic ARN for alarms"
  type        = string
  default     = ""
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}
