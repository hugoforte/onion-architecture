locals {
  name_prefix = "todo-${var.environment}"
  tags = merge(
    var.tags,
    {
      Module = "Secrets"
    }
  )
}

# Secrets Manager Secret
resource "aws_secretsmanager_secret" "main" {
  for_each = var.secrets_map

  name                    = "${local.name_prefix}/${each.key}"
  description             = "Secret for ${each.key}"
  recovery_window_in_days = 7

  tags = merge(
    local.tags,
    { Name = "${local.name_prefix}-${each.key}" }
  )
}

# Secrets Manager Secret Version
resource "aws_secretsmanager_secret_version" "main" {
  for_each = var.secrets_map

  secret_id       = aws_secretsmanager_secret.main[each.key].id
  secret_string   = each.value
  version_stages = ["AWSCURRENT"]
}
