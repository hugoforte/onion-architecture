variable "environment" {
  description = "Environment name"
  type        = string
}

variable "queue_name_prefix" {
  description = "Prefix for queue names"
  type        = string
  default     = "todo"
}

variable "message_retention_seconds" {
  description = "Message retention period in seconds"
  type        = number
  default     = 345600 # 4 days
}

variable "visibility_timeout_seconds" {
  description = "Visibility timeout in seconds"
  type        = number
  default     = 300
}

variable "max_message_size" {
  description = "Maximum message size in bytes"
  type        = number
  default     = 262144 # 256 KB
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}
