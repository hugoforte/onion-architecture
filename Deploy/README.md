# Terraform Infrastructure as Code for Todo App

AWS infrastructure deployment using Terraform with ECS Fargate, RDS PostgreSQL, and Application Load Balancer.

## Architecture

```
┌─────────────────────────────────────────────┐
│            AWS VPC (10.0.0.0/16)            │
│                                              │
│  ┌────────────────────────────────────────┐ │
│  │    Public Subnets (Load Balancer)      │ │
│  │  - 10.0.1.0/24 (AZ1)                   │ │
│  │  - 10.0.2.0/24 (AZ2)                   │ │
│  └────────────────────────────────────────┘ │
│              ↓        ↓                      │
│  ┌────────────────────────────────────────┐ │
│  │   Application Load Balancer (ALB)      │ │
│  │   - Port 80 → ECS Service              │ │
│  └────────────────────────────────────────┘ │
│              ↓        ↓                      │
│  ┌────────────────────────────────────────┐ │
│  │  Private Subnets (ECS, RDS)            │ │
│  │  - 10.0.10.0/24 (AZ1)                  │ │
│  │  - 10.0.11.0/24 (AZ2)                  │ │
│  │                                         │ │
│  │  ECS Fargate Cluster                    │ │
│  │  - Starter API Tasks                    │ │
│  │  - Auto-scaling (CPU/Memory)            │ │
│  │                                         │ │
│  │  RDS PostgreSQL                         │ │
│  │  - Multi-AZ (Production)                │ │
│  │  - Single AZ (Dev/UAT)                  │ │
│  └────────────────────────────────────────┘ │
│                                              │
│            NAT Gateway (AZ1)                 │
│            - Outbound internet access       │
└─────────────────────────────────────────────┘
```

## Prerequisites

- Terraform >= 1.0
- AWS CLI configured with credentials
- ECR repository with pushed container image

## Files

- `main.tf` - Provider configuration
- `variables.tf` - Input variables
- `outputs.tf` - Output values
- `network.tf` - VPC, subnets, routing
- `security_groups.tf` - Security groups for ALB, ECS, RDS
- `load_balancer.tf` - Application Load Balancer configuration
- `database.tf` - RDS PostgreSQL instance
- `ecs.tf` - ECS cluster, service, task definitions, auto-scaling
- `environments/dev/terraform.tfvars` - Development configuration
- `environments/uat/terraform.tfvars` - UAT configuration
- `environments/prod/terraform.tfvars` - Production configuration

## Deployment

### Development

```bash
cd Deploy
terraform init
terraform plan -var-file=environments/dev/terraform.tfvars
terraform apply -var-file=environments/dev/terraform.tfvars
```

### UAT

```bash
terraform plan -var-file=environments/uat/terraform.tfvars
terraform apply -var-file=environments/uat/terraform.tfvars
```

### Production

```bash
terraform plan -var-file=environments/prod/terraform.tfvars
terraform apply -var-file=environments/prod/terraform.tfvars
```

## Configuration

### Before Deploying

1. Build and push container image to ECR:

```bash
# Build Docker image
docker build -t starter-api:latest Backend/3_Run/Web

# Tag image for ECR
docker tag starter-api:latest <ACCOUNT_ID>.dkr.ecr.us-east-1.amazonaws.com/starter-todo-app/starter-api:latest

# Push to ECR
docker push <ACCOUNT_ID>.dkr.ecr.us-east-1.amazonaws.com/starter-todo-app/starter-api:latest
```

2. Update `container_image` in environment terraform.tfvars files with your ECR image URI

3. Update database password in terraform.tfvars (or use AWS Secrets Manager for production)

## Outputs

After deployment, Terraform will output:

- `alb_dns_name` - Load balancer DNS name for accessing the API
- `rds_endpoint` - RDS database endpoint
- `ecr_repository_url` - ECR repository URL
- `ecs_cluster_name` - ECS cluster name
- `ecs_service_name` - ECS service name

## Database

- **Engine**: PostgreSQL 16
- **Dev**: db.t3.micro, 20GB, 7-day backups
- **UAT**: db.t3.micro, 20GB, 7-day backups
- **Prod**: db.t3.small, 100GB, Multi-AZ, 30-day backups, deletion protection

## Auto-Scaling

ECS service auto-scales based on:

- **CPU**: Target 70% average utilization
- **Memory**: Target 80% average utilization

### Task Counts

- **Dev**: Min 1, Max 2
- **UAT**: Min 1, Max 2
- **Prod**: Min 2, Max 5

## Security

- RDS instances are in private subnets
- ECS tasks only accept traffic from ALB
- ALB accepts HTTP traffic on port 80
- NAT Gateway for private subnet outbound traffic

## Notes

- Database credentials should be managed via AWS Secrets Manager or Systems Manager Parameter Store in production
- Consider enabling HTTPS/TLS termination at ALB for production
- Container images should be scanned for vulnerabilities in ECR
- CloudWatch Logs retention: 7 days (dev/uat), 30 days (prod)
