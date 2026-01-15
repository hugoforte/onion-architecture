variable "environment" {
  description = "Environment name"
  type        = string
}

variable "service_name" {
  description = "Service name for IAM resource naming"
  type        = string
  default     = "todo"
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}
