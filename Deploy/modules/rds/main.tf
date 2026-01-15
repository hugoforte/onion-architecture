locals {
  name_prefix = "todo-${var.environment}"
  tags = merge(
    var.tags,
    {
      Module = "RDS"
    }
  )
}

# DB Subnet Group
resource "aws_db_subnet_group" "main" {
  name       = "${local.name_prefix}-db-subnet-group"
  subnet_ids = var.subnet_ids

  tags = merge(
    local.tags,
    { Name = "${local.name_prefix}-db-subnet-group" }
  )
}

# Security Group for RDS
resource "aws_security_group" "rds" {
  name        = "${local.name_prefix}-rds-sg"
  description = "Security group for RDS database"
  vpc_id      = var.vpc_id

  ingress {
    from_port       = 5432
    to_port         = 5432
    protocol        = "tcp"
    security_groups = [var.ecs_security_group_id]
    description     = "PostgreSQL from ECS"
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = merge(
    local.tags,
    { Name = "${local.name_prefix}-rds-sg" }
  )
}

# RDS Instance
resource "aws_db_instance" "main" {
  identifier            = "${local.name_prefix}-db"
  engine               = "postgres"
  engine_version       = var.engine_version
  instance_class       = var.instance_class
  allocated_storage    = var.allocated_storage
  storage_type         = "gp3"
  
  db_name  = var.db_name
  username = var.master_username
  password = var.master_password

  db_subnet_group_name            = aws_db_subnet_group.main.name
  vpc_security_group_ids          = [aws_security_group.rds.id]
  publicly_accessible             = false
  multi_az                        = var.multi_az
  storage_encrypted               = true
  backup_retention_period         = var.backup_retention_days
  backup_window                   = "03:00-04:00"
  maintenance_window              = "sun:04:00-sun:05:00"
  skip_final_snapshot             = false
  final_snapshot_identifier       = "${local.name_prefix}-final-snapshot-${formatdate("YYYY-MM-DD-hhmm", timestamp())}"
  copy_tags_to_snapshot           = true
  delete_automated_backups        = false
  
  enabled_cloudwatch_logs_exports = ["postgresql"]

  performance_insights_enabled    = var.multi_az ? true : false
  monitoring_interval             = var.enable_monitoring ? 60 : 0
  monitoring_role_arn             = var.enable_monitoring ? var.monitoring_role_arn : null
  enable_iam_database_authentication = true

  tags = merge(
    local.tags,
    { Name = "${local.name_prefix}-db" }
  )

  depends_on = [
    aws_db_subnet_group.main,
    aws_security_group.rds
  ]
}

# DB Parameter Group for enhanced logging
resource "aws_db_parameter_group" "main" {
  name   = "${local.name_prefix}-db-params"
  family = "postgres15"

  parameter {
    name  = "log_statement"
    value = "all"
  }

  parameter {
    name  = "log_duration"
    value = "1"
  }

  tags = merge(
    local.tags,
    { Name = "${local.name_prefix}-db-params" }
  )
}

# CloudWatch Log Group for RDS
resource "aws_cloudwatch_log_group" "rds" {
  name              = "/aws/rds/instance/${aws_db_instance.main.id}/postgresql"
  retention_in_days = var.log_retention_days

  tags = merge(
    local.tags,
    { Name = "${local.name_prefix}-rds-lg" }
  )
}
