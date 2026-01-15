# RDS Subnet Group
resource "aws_db_subnet_group" "postgres" {
  name       = "${var.project_name}-db-subnet-group"
  subnet_ids = [aws_subnet.private_1.id, aws_subnet.private_2.id]

  tags = {
    Name = "${var.project_name}-db-subnet-group"
  }
}

# RDS PostgreSQL Instance
resource "aws_db_instance" "postgres" {
  identifier              = "${var.project_name}-db"
  db_name                 = var.database_name
  engine                  = "postgres"
  engine_version          = "16"
  instance_class          = var.environment == "prod" ? "db.t3.small" : "db.t3.micro"
  allocated_storage       = var.environment == "prod" ? 100 : 20
  storage_type            = "gp3"
  username                = var.database_username
  password                = var.database_password
  db_subnet_group_name    = aws_db_subnet_group.postgres.name
  vpc_security_group_ids  = [aws_security_group.rds.id]
  skip_final_snapshot     = var.environment != "prod"
  multi_az                = var.environment == "prod" ? true : false
  publicly_accessible     = false
  backup_retention_period = var.environment == "prod" ? 30 : 7
  deletion_protection     = var.environment == "prod" ? true : false

  tags = {
    Name = "${var.project_name}-postgres"
  }
}
