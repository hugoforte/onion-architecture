# Todo App Template - Terraform Deployment PRD

**Document Status:** Draft  
**Date:** January 15, 2026  
**Author:** GitHub Copilot  
**Version:** 1.0

---

## Executive Summary

This Product Requirements Document (PRD) outlines the infrastructure-as-code (IaC) strategy for deploying the full-stack Todo App template to AWS using Terraform. The solution will provide a scalable, secure, and maintainable deployment pipeline across development, staging, and production environments.

---

## 1. Project Overview

### 1.1 Application Architecture
- **Backend**: ASP.NET Core 9.0 Web API (Onion Architecture)
- **Frontend**: React 18 + TypeScript + Vite
- **Database**: PostgreSQL (managed RDS)
- **Messaging**: NServiceBus with Amazon SQS
- **Container Runtime**: AWS ECS Fargate
- **CI/CD**: GitHub Actions

### 1.2 Deployment Goal
Deploy a production-ready, multi-environment (dev, staging, prod) infrastructure that:
- Automatically builds and deploys on code commits
- Scales based on demand
- Includes monitoring, logging, and alerting
- Maintains security best practices
- Reduces manual operational overhead

---

## 2. Infrastructure Requirements

### 2.1 AWS Services Required

#### Compute
- **ECS Fargate**: Container orchestration for backend API
- **CloudFront**: CDN for frontend static assets
- **Lambda** (optional): Scheduled tasks or event-driven functions

#### Networking
- **VPC**: Virtual Private Cloud with public/private subnets
- **ALB**: Application Load Balancer for API
- **NAT Gateway**: For private subnet outbound traffic
- **Route 53**: DNS management
- **Security Groups**: Network access control

#### Data & Storage
- **RDS PostgreSQL**: Managed database service
- **S3**: Static website hosting for frontend
- **Secrets Manager**: Sensitive data management (API keys, DB passwords)

#### Messaging
- **SQS**: Message queue for NServiceBus
- **SNS** (optional): Pub/sub for notifications

#### Monitoring & Logging
- **CloudWatch**: Logs, metrics, alarms
- **X-Ray**: Distributed tracing
- **EventBridge**: Event routing

#### CI/CD Integration
- **ECR**: Docker image registry
- **CodePipeline/CodeBuild** (optional): Or use GitHub Actions directly

---

## 3. Terraform Architecture & Modules

### 3.1 Proposed Module Structure

```
Deploy/
├── modules/
│   ├── vpc/                    # VPC, subnets, NAT, security groups
│   ├── ecs/                    # ECS cluster, task definitions, services
│   ├── rds/                    # PostgreSQL database
│   ├── alb/                    # Application Load Balancer
│   ├── s3_frontend/            # S3 bucket + CloudFront for frontend
│   ├── sqs/                    # SQS queues for messaging
│   ├── iam/                    # IAM roles and policies
│   ├── cloudwatch/             # Logs, metrics, alarms
│   ├── secrets/                # Secrets Manager
│   └── rds_migration/          # Database migration utilities
├── environments/
│   ├── dev/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   ├── terraform.tfvars
│   │   └── outputs.tf
│   ├── staging/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   ├── terraform.tfvars
│   │   └── outputs.tf
│   └── prod/
│       ├── main.tf
│       ├── variables.tf
│       ├── terraform.tfvars
│       └── outputs.tf
├── shared/
│   ├── backend.tf             # Terraform backend (S3 + DynamoDB)
│   └── provider.tf            # AWS provider configuration
└── scripts/
    ├── init.sh                # Initialize Terraform
    ├── apply.sh               # Apply Terraform
    ├── destroy.sh             # Destroy infrastructure
    └── db-migrate.sh          # Run database migrations
```

### 3.2 Module Breakdown

#### 3.2.1 VPC Module
**Purpose**: Network foundation for all resources
- VPC with CIDR block
- Public subnets (for ALB, NAT)
- Private subnets (for ECS, RDS)
- NAT Gateway for outbound traffic
- Internet Gateway
- Route tables and associations
- Network ACLs

**Variables**:
- `environment`: dev/staging/prod
- `vpc_cidr`: VPC CIDR block
- `availability_zones`: List of AZs
- `public_subnet_cidrs`: Public subnet ranges
- `private_subnet_cidrs`: Private subnet ranges

#### 3.2.2 ECS Module
**Purpose**: Container orchestration for backend API
- ECS cluster
- Task definition (backend API container)
- ECS service with auto-scaling
- CloudWatch log groups
- Container image from ECR

**Variables**:
- `cluster_name`: ECS cluster name
- `task_family`: Task definition family
- `docker_image`: ECR image URI
- `container_port`: Container listening port (5273)
- `desired_count`: Number of tasks
- `cpu`: Task CPU units (256-4096)
- `memory`: Task memory (512-30720)

#### 3.2.3 RDS Module
**Purpose**: Managed PostgreSQL database
- DB instance (multi-AZ for prod)
- DB subnet group
- Parameter group
- Security group for database access
- Backup configuration
- Enhanced monitoring

**Variables**:
- `engine_version`: PostgreSQL version (15.x)
- `instance_class`: db.t3.micro/small/medium
- `allocated_storage`: Initial storage size (20-1000 GB)
- `multi_az`: Enable multi-AZ (true for prod)
- `backup_retention_days`: Backup retention (7-35 days)
- `db_name`: Initial database name
- `master_username`: Database admin user

#### 3.2.4 ALB Module
**Purpose**: Load balancing for backend API
- Application Load Balancer
- Target group for ECS tasks
- Health check configuration
- HTTPS listener (with ACM certificate)
- HTTP to HTTPS redirect

**Variables**:
- `alb_name`: Load balancer name
- `target_group_name`: Target group name
- `health_check_path`: Health check endpoint (/)
- `certificate_arn`: SSL/TLS certificate ARN

#### 3.2.5 S3 & CloudFront Module
**Purpose**: Frontend static hosting
- S3 bucket for frontend assets
- CloudFront distribution
- Origin access identity (OAI)
- Cache invalidation policy
- HTTPS enforcement

**Variables**:
- `bucket_name`: S3 bucket name
- `environment`: dev/staging/prod
- `cloudfront_domain`: Custom domain (optional)
- `certificate_arn`: SSL certificate for CloudFront

#### 3.2.6 SQS Module
**Purpose**: Message queuing for NServiceBus
- Standard SQS queues
- Dead-letter queue (DLQ)
- Queue policies
- Message retention

**Variables**:
- `queue_name_prefix`: Prefix for queue names
- `message_retention_seconds`: Message retention (60-1209600 seconds)
- `visibility_timeout_seconds`: Visibility timeout
- `max_message_size`: Max message size (1024-262144 bytes)

#### 3.2.7 IAM Module
**Purpose**: Access control and permissions
- ECS task execution role
- ECS task role
- RDS database user (optional)
- S3 bucket policies
- SQS queue policies
- Secrets Manager access policies

**Variables**:
- `service_name`: Service name for naming
- `environment`: Environment name
- `allowed_actions`: List of allowed actions per resource

#### 3.2.8 CloudWatch Module
**Purpose**: Monitoring, logging, and alerting
- CloudWatch log groups (ECS, RDS, ALB)
- Custom metrics
- Alarms (CPU, memory, error rates)
- Dashboard
- Log retention policies

**Variables**:
- `log_retention_days`: Log retention (7-3653 days)
- `alarm_threshold_cpu`: CPU alarm threshold (%)
- `alarm_threshold_memory`: Memory alarm threshold (%)
- `alarm_actions`: SNS topic for notifications

#### 3.2.9 Secrets Manager Module
**Purpose**: Sensitive data management
- Database credentials
- API keys
- JWT secrets
- Connection strings

**Variables**:
- `environment`: Environment name
- `secrets_map`: Map of secret names and values

---

## 4. Environment Configuration

### 4.1 Development Environment
- **ECS Task Count**: 1
- **RDS Instance**: db.t3.micro (single-AZ)
- **Storage**: 20 GB
- **Backup**: 7 days
- **CloudFront**: Disabled (direct S3 access)
- **Auto-scaling**: Disabled
- **Monitoring**: Basic (5-minute intervals)

### 4.2 Staging Environment
- **ECS Task Count**: 2
- **RDS Instance**: db.t3.small (single-AZ)
- **Storage**: 50 GB
- **Backup**: 14 days
- **CloudFront**: Enabled with 1-day cache
- **Auto-scaling**: Enabled (2-4 tasks)
- **Monitoring**: Enhanced (1-minute intervals)

### 4.3 Production Environment
- **ECS Task Count**: 3 (minimum)
- **RDS Instance**: db.t3.medium (multi-AZ)
- **Storage**: 100 GB
- **Backup**: 30 days
- **CloudFront**: Enabled with intelligent caching
- **Auto-scaling**: Enabled (3-10 tasks, target CPU 70%)
- **Monitoring**: Full (1-minute intervals, detailed logs)
- **HA/DR**: Multi-AZ, automated failover

---

## 5. CI/CD Integration

### 5.1 GitHub Actions Workflow

#### Build & Test Pipeline
1. Trigger on: Push to `develop` or `main` branch
2. Build Docker image for backend
3. Run backend tests
4. Build frontend (npm build)
5. Run frontend tests
6. Push Docker image to ECR
7. Create artifact for frontend build

#### Deploy Pipeline
1. Trigger on: Successful build + manual approval
2. Terraform plan (with approval)
3. Update RDS schema (EF Core migrations)
4. Update ECS task definition
5. Deploy to environment
6. Run smoke tests
7. Run integration tests (optional)

#### Rollback Capability
- Keep previous ECS task definition
- Quick rollback to previous version via CLI/console
- Database rollback (if applicable)

### 5.2 Deployment Strategy

**Development**: Auto-deploy on commit to `develop`  
**Staging**: Manual approval required, triggers on `develop` commits  
**Production**: Manual approval required, triggers on `main` commits

---

## 6. Security Requirements

### 6.1 Network Security
- VPC isolation with public/private subnets
- Security groups with least-privilege rules
- No direct internet access to RDS
- ALB only accessible via HTTPS

### 6.2 Data Security
- Secrets Manager for sensitive configuration
- RDS encryption at-rest (AWS KMS)
- Database credentials rotated regularly
- Backend API requires authentication (TBD)

### 6.3 Infrastructure Security
- IAM roles with minimal permissions
- CloudTrail for audit logging
- VPC Flow Logs for network monitoring
- Regular security group audits

### 6.4 Application Security
- HTTPS enforced (ALB + CloudFront)
- CORS properly configured
- SQL injection prevention (via ORM)
- Rate limiting (ALB or app-level)

---

## 7. Monitoring & Observability

### 7.1 Metrics to Track
- **Application Level**:
  - Request latency (p50, p95, p99)
  - Error rate (4xx, 5xx)
  - Todo CRUD operation latency
  - Message queue depth (SQS)

- **Infrastructure Level**:
  - ECS CPU utilization
  - ECS memory utilization
  - RDS CPU and connections
  - ALB request count and latency
  - CloudFront cache hit ratio

### 7.2 Logging Strategy
- ECS logs → CloudWatch Logs
- RDS logs → CloudWatch Logs
- ALB access logs → S3
- Frontend errors → CloudWatch (via backend logging endpoint)
- Centralized log analysis (CloudWatch Insights)

### 7.3 Alerting
- High CPU usage (ECS/RDS)
- High memory usage (ECS)
- Database connection errors
- ALB target health checks failing
- Error rate threshold exceeded
- RDS backup failures

### 7.4 Dashboards
- Executive dashboard (uptime, error rate, performance)
- Operator dashboard (detailed metrics, logs)
- Development dashboard (real-time app metrics)

---

## 8. Database Considerations

### 8.1 Schema Management
- EF Core migrations automated via GitHub Actions
- Pre-production environment for testing migrations
- Rollback capability via migration history
- Zero-downtime deployment strategy (TBD)

### 8.2 Backup & Disaster Recovery
- Automated daily backups
- Cross-region backup (optional for prod)
- Point-in-time recovery (PITR) enabled
- RTO: 1 hour, RPO: 4 hours (target)

### 8.3 Performance
- RDS performance insights enabled (prod)
- CloudWatch enhanced monitoring
- Database query logging (for troubleshooting)
- Connection pooling via app (Entity Framework)

---

## 9. Cost Estimation

### 9.1 Monthly Cost Breakdown (Estimates)

**Development Environment**: ~$50-80/month
- ECS Fargate (1 task, t3.micro): $15
- RDS (db.t3.micro): $20
- Data transfer: $5
- S3 + CloudFront: $5-10

**Staging Environment**: ~$150-200/month
- ECS Fargate (2 tasks avg): $30
- RDS (db.t3.small): $40
- Data transfer: $10
- S3 + CloudFront: $15-20
- SQS: $5-10

**Production Environment**: ~$400-600/month
- ECS Fargate (3+ tasks avg): $60
- RDS (db.t3.medium multi-AZ): $150
- Data transfer: $30
- S3 + CloudFront: $30
- SQS, monitoring, other: $100-150

**Total**: ~$600-880/month for all environments

### 9.2 Cost Optimization Opportunities
- Use Reserved Instances (RDS) for prod
- CloudFront cache optimization
- ECS auto-scaling to reduce idle capacity
- Consider AWS Savings Plans

---

## 10. Testing Strategy with LocalStack

### 10.1 LocalStack Overview
**LocalStack** is a fully functional local AWS cloud stack for testing infrastructure without AWS costs.

**Testing Levels**:
1. **Unit Tests**: Terraform syntax validation (`terraform validate`)
2. **Integration Tests**: Deploy to LocalStack and validate outputs (Terratest)
3. **End-to-End Tests**: Full stack deployment + application testing

### 10.2 Testing Framework Architecture

```
tests/
├── terraform/
│   ├── modules/             # Reference to actual modules
│   ├── test_fixtures/       # Test configurations
│   │   ├── vpc_test/
│   │   ├── ecs_test/
│   │   ├── rds_test/
│   │   └── full_stack_test/
│   └── localstack_setup.tf
├── integration/             # Terratest Go tests
│   ├── vpc_test.go
│   ├── ecs_test.go
│   ├── rds_test.go
│   ├── full_stack_test.go
│   └── helper.go
├── docker-compose.localstack.yml
├── init-aws.sh
├── Makefile
└── README.md
```

### 10.3 Test Fixtures

Each module has a test fixture that deploys to LocalStack:

**Example: VPC Test Fixture** (`tests/terraform/test_fixtures/vpc_test/main.tf`)
```hcl
provider "aws" {
  region = "us-east-1"
  endpoints {
    ec2 = "http://localhost:4566"
    iam = "http://localhost:4566"
    # ... other services
  }
  skip_credentials_validation = true
}

module "vpc" {
  source = "../../../modules/vpc"
  environment = "test"
  vpc_cidr = "10.0.0.0/16"
  # ... variables
}
```

### 10.4 Integration Tests with Terratest

Tests are written in Go using Terratest framework:

**Example: VPC Test** (`tests/integration/vpc_test.go`)
```go
func TestVPCModule(t *testing.T) {
  ctx := NewTestContext(t, "../terraform/test_fixtures/vpc_test")
  defer ctx.Destroy()
  
  t.Run("Initialize and Apply", func(t *testing.T) {
    ctx.InitAndApply()
  })
  
  t.Run("Validate VPC Creation", func(t *testing.T) {
    vpcId := ctx.GetOutput("vpc_id")
    // Verify in LocalStack
    vpcs, _ := ec2Client.DescribeVpcs(&ec2.DescribeVpcsInput{
      VpcIds: []*string{aws.String(vpcId)},
    })
    assert.Equal(t, 1, len(vpcs.Vpcs))
  })
}
```

### 10.5 Running Tests Locally

**Prerequisites**:
- Docker & Docker Compose
- Terraform 1.5+
- Go 1.21+
- AWS CLI

**Quick Start**:
```bash
cd tests

# Start LocalStack
make setup

# Run VPC tests
make test-vpc

# Run all tests
make test-all

# View logs
make logs

# Cleanup
make teardown
```

### 10.6 Makefile Targets

```makefile
make setup              # Start LocalStack
make teardown           # Stop LocalStack
make test-vpc           # Test VPC module
make test-ecs           # Test ECS module
make test-rds           # Test RDS module
make test-all           # Test all modules
make lint               # Lint Terraform code
make fmt                # Format code
make validate           # Validate syntax
make clean              # Clean all resources
```

### 10.7 Test Coverage

**Module Tests**:
- ✅ VPC module (subnets, routing, NAT)
- ✅ ECS module (cluster, services, scaling)
- ✅ RDS module (database creation, backups)
- ✅ ALB module (load balancing, health checks)
- ✅ S3/CloudFront module (static hosting)
- ✅ IAM module (roles and policies)

**Integration Tests**:
- ✅ Module composition (VPC + ECS)
- ✅ Cross-module references
- ✅ Output validation
- ✅ Security group rules

**End-to-End Tests**:
- ✅ Full stack deployment
- ✅ Application deployment
- ✅ Database connectivity
- ✅ Health checks passing

### 10.8 CI/CD Integration

GitHub Actions workflow automatically runs tests on PR:

```yaml
name: Terraform Tests
on:
  pull_request:
    paths:
      - 'Deploy/modules/**'
      - 'tests/**'

jobs:
  validate:
    runs-on: ubuntu-latest
    steps:
      - name: Terraform Validate
        run: terraform fmt -check && terraform validate
      
  integration-tests:
    runs-on: ubuntu-latest
    services:
      localstack:
        image: localstack/localstack:latest
    steps:
      - name: Run Tests
        run: cd tests && make test-all
```

### 10.9 Test Best Practices

1. **Isolation**: Each test uses unique resource names
2. **Cleanup**: Always destroy resources after tests
3. **Timeouts**: Set appropriate timeouts (10-20 minutes)
4. **Assertions**: Validate resource creation AND configuration
5. **Logging**: Enable debug logs for troubleshooting
6. **Realistic Data**: Use production-like test data
7. **Documentation**: Document test purpose and assertions

---

## 11. Deployment Procedure

### 11.1 Initial Setup
```bash
# 1. Initialize Terraform state backend
./scripts/init.sh

# 2. Plan dev environment
terraform -chdir=environments/dev plan

# 3. Apply dev environment
terraform -chdir=environments/dev apply
```

### 11.2 Ongoing Deployments
```bash
# 1. Push changes to develop branch
git push origin develop

# 2. GitHub Actions automatically:
#    - Runs Terraform tests in LocalStack
#    - Builds Docker image
#    - Runs application tests
#    - Pushes to ECR

# 3. Manual approval triggers deployment
#    - terraform plan
#    - terraform apply
#    - ECS task update
#    - Database migrations
#    - Smoke tests
```

### 11.3 Rollback Procedure
```bash
# 1. Identify previous task definition
aws ecs describe-task-definition --task-definition api-prod:N

# 2. Update service to use previous task definition
aws ecs update-service --cluster api-prod --service api-prod-service --task-definition api-prod:N-1

# 3. Monitor rollback
aws ecs describe-services --cluster api-prod --services api-prod-service
```

---

## 12. Success Criteria

### 12.1 Infrastructure
- [ ] All Terraform modules created and tested
- [ ] Dev, staging, and prod environments deployed
- [ ] Auto-scaling functional
- [ ] Multi-AZ failover working in prod

### 12.2 CI/CD
- [ ] GitHub Actions workflow deployed
- [ ] Automated builds and tests passing
- [ ] LocalStack integration tests passing
- [ ] Automated deployments to dev on each commit
- [ ] Manual approval gates for staging/prod

### 12.3 Monitoring
- [ ] CloudWatch dashboards created
- [ ] Alarms configured and tested
- [ ] Log aggregation working
- [ ] Health checks configured on all endpoints

### 12.4 Security
- [ ] SSL/TLS certificates installed
- [ ] Security groups configured correctly
- [ ] Secrets Manager storing all credentials
- [ ] IAM roles following least-privilege principle

### 12.5 Documentation
- [ ] Terraform code documented
- [ ] Deployment runbooks created
- [ ] Troubleshooting guides written
- [ ] Disaster recovery procedures documented

---

## 13. Timeline & Phases

### Phase 1: Foundation (Week 1-2)
- [ ] Terraform backend setup (S3 + DynamoDB)
- [ ] LocalStack Docker Compose configuration
- [ ] VPC module implementation and tests
- [ ] IAM module implementation and tests
- [ ] Documentation

### Phase 2: Core Services (Week 3-4)
- [ ] ECS module implementation and tests
- [ ] RDS module implementation and tests
- [ ] ALB module implementation and tests
- [ ] SQS module implementation and tests

### Phase 3: Frontend & Monitoring (Week 5)
- [ ] S3 + CloudFront module implementation
- [ ] CloudWatch module implementation
- [ ] Secrets Manager module implementation
- [ ] Environment configurations

### Phase 4: CI/CD & Testing (Week 6)
- [ ] GitHub Actions workflow setup
- [ ] LocalStack integration test pipeline
- [ ] Automated testing pipeline
- [ ] Deployment automation
- [ ] UAT and validation

### Phase 5: Documentation & Handoff (Week 7)
- [ ] Complete documentation
- [ ] Runbook creation
- [ ] Team training
- [ ] Cutover to production

---

## 14. Risk & Mitigation

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|-----------|
| Database migration failure | Medium | High | Test migrations in staging, keep rollback plan |
| ECS service failures | Low | High | Multi-AZ setup, health checks, auto-restart |
| SSL certificate expiration | Low | High | Auto-renewal, monitoring alerts |
| Terraform state corruption | Very Low | Critical | S3 versioning, regular backups |
| LocalStack divergence from AWS | Medium | Medium | Regular testing, AWS parity validation |
| Test infrastructure bloat | Medium | Medium | Cleanup policies, cost monitoring |
| Cost overruns | Medium | Medium | Monitor usage, budget alerts, reserved instances |
| Deployment failures | Medium | Medium | Comprehensive testing, canary deployments, rollback |

---

## 15. Open Questions & Decisions Needed

1. **Domain Names**: What domains should we use? (api.example.com, app.example.com)
2. **SSL Certificates**: Use ACM or external provider?
3. **Monitoring**: CloudWatch only or integrate with Datadog/New Relic?
4. **Database**: Single RDS instance or Aurora?
5. **Caching**: Redis/ElastiCache needed?
6. **Authentication**: How should API authentication work? (JWT, OAuth, API keys)
7. **Database seeding**: How to populate initial data?
8. **Blue/Green Deployments**: Required or not?
9. **SLA/Uptime Target**: 99.9%, 99.95%, 99.99%?
10. **Budget Cap**: What's the maximum acceptable monthly cost?
11. **Test Retention**: How long to keep LocalStack test data?
12. **Test Notifications**: Alert on test failures?

---

## 16. Approval & Sign-Off

| Role | Name | Date | Signature |
|------|------|------|-----------|
| Project Lead | TBD | | |
| Infrastructure Lead | TBD | | |
| Security Officer | TBD | | |
| Finance | TBD | | |

---

## Appendix: References

- [Terraform AWS Provider Documentation](https://registry.terraform.io/providers/hashicorp/aws/latest/docs)
- [AWS ECS Best Practices](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/welcome.html)
- [AWS RDS Best Practices](https://docs.aws.amazon.com/AmazonRDS/latest/UserGuide/CHAP_BestPractices.html)
- [Entity Framework Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [GitHub Actions for AWS Deployment](https://github.com/aws-actions)
- [LocalStack Documentation](https://docs.localstack.cloud/)
- [Terratest Documentation](https://terratest.gruntwork.io/)

---

**Next Steps:**
1. Review and approve this PRD
2. Answer open questions (Section 15)
3. Set up LocalStack testing infrastructure (Phase 1)
4. Schedule kick-off meeting for Phase 1
5. Begin Terraform module development
