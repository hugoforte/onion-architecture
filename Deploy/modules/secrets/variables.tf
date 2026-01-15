variable "environment" {
  description = "Environment name"
  type        = string
}

variable "secrets_map" {
  description = "Map of secret names and values"
  type        = map(string)
  sensitive   = true
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}
