# Infrastructure as Code - Terraform Deployment

Complete infrastructure-as-code setup for deploying the Todo App to AWS using Terraform with ECS Fargate, RDS PostgreSQL, CloudFront CDN, and automated CI/CD pipelines.

## Quick Start

### 1. Initialize Terraform Backend

```bash
cd scripts
chmod +x init.sh
./init.sh
```

### 2. Deploy Development

```bash
cd environments/dev
terraform init
terraform plan -var="rds_master_password=PASS" -var="jwt_secret=SECRET"
terraform apply
```

### 3. Deploy Staging

```bash
cd environments/staging
terraform init
terraform plan -var="rds_master_password=PASS" -var="jwt_secret=SECRET"
terraform apply
```

### 4. Deploy Production

```bash
cd environments/prod
terraform init
terraform plan \
  -var="certificate_arn_prod=ARN" \
  -var="alarm_sns_topic_arn_prod=ARN" \
  -var="docker_image_prod=IMAGE" \
  -var="rds_master_password=PASS" \
  -var="jwt_secret=SECRET"
terraform apply
```

## Architecture

```
┌─────────────────────────────────────────────┐
│            AWS VPC (10.0.0.0/16)            │
├─────────────────────────────────────────────┤
│  Public Subnets (ALB, NAT Gateways)         │
│  ├─ 10.0.1.0/24 (us-east-1a)               │
│  └─ 10.0.2.0/24 (us-east-1b)               │
│              ↓                              │
│  ┌────────────────────────────────────────┐ │
│  │ Application Load Balancer (HTTPS)      │ │
│  │ ↓                                       │ │
│  │ CloudFront CDN (Frontend)               │ │
│  └────────────────────────────────────────┘ │
├─────────────────────────────────────────────┤
│  Private Subnets (ECS, RDS, SQS)            │
│  ├─ 10.0.10.0/24 (us-east-1a)              │
│  └─ 10.0.11.0/24 (us-east-1b)              │
│                                              │
│  ECS Fargate Cluster                         │
│  ├─ API Service (Auto-scaling)              │
│  ├─ CloudWatch Logs                         │
│  └─ Container Insights Monitoring           │
│                                              │
│  RDS PostgreSQL                              │
│  ├─ Multi-AZ (Production)                   │
│  ├─ Enhanced Monitoring                     │
│  └─ Automated Backups                       │
│                                              │
│  SQS Queues + DLQ                            │
│  └─ NServiceBus Integration                 │
│                                              │
│  S3 + CloudFront                             │
│  └─ Frontend Static Hosting                 │
└─────────────────────────────────────────────┘
```

## Modules

- **vpc** - Virtual Private Cloud, subnets, NAT Gateways, VPC Flow Logs
- **iam** - IAM roles and policies for ECS, RDS, Lambda, S3
- **alb** - Application Load Balancer, target groups, health checks
- **ecs** - ECS Fargate cluster, services, task definitions, auto-scaling
- **rds** - PostgreSQL RDS instance with backups, monitoring, encryption
- **s3_frontend** - S3 bucket, CloudFront CDN, origin access identity
- **sqs** - SQS queues with dead-letter queues for messaging
- **cloudwatch** - Monitoring, dashboards, alarms, log groups
- **secrets** - Secrets Manager for sensitive configuration

## Environments

| Environment | ECS Tasks | RDS Instance | CloudFront | Auto-scaling | Backups |
|---|---|---|---|---|---|
| **dev** | 1 | t3.micro | ❌ | ❌ | 7 days |
| **staging** | 2 | t3.small | ✅ | 2-4 tasks | 14 days |
| **prod** | 3 | t3.medium | ✅ | 3-10 tasks | 30 days |

## Prerequisites

- Terraform >= 1.5
- AWS CLI v2
- AWS Account with appropriate permissions
- Docker (for LocalStack testing)
- Git
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
