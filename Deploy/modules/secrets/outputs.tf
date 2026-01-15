output "secret_arns" {
  description = "Map of secret ARNs"
  value = {
    for key, secret in aws_secretsmanager_secret.main :
    key => secret.arn
  }
}

output "secret_names" {
  description = "Map of secret names"
  value = {
    for key, secret in aws_secretsmanager_secret.main :
    key => secret.name
  }
}
