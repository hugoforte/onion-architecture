#!/bin/bash
# LocalStack initialization script
# Runs inside LocalStack to set up AWS resources for testing

echo "Initializing LocalStack environment..."

# Set AWS credentials
export AWS_ACCESS_KEY_ID=test
export AWS_SECRET_ACCESS_KEY=test
export AWS_DEFAULT_REGION=us-east-1

# Create S3 bucket
aws s3 mb s3://todo-app-test-frontend --region us-east-1 --endpoint-url=http://localhost:4566 || true

# Create SQS queues
aws sqs create-queue --queue-name todo-dev-queue --region us-east-1 --endpoint-url=http://localhost:4566 || true
aws sqs create-queue --queue-name todo-dev-dlq --region us-east-1 --endpoint-url=http://localhost:4566 || true

# Create IAM roles
aws iam create-role --role-name todo-dev-ecs-task-execution-role --assume-role-policy-document '{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": "sts:AssumeRole",
      "Effect": "Allow",
      "Principal": {
        "Service": "ecs-tasks.amazonaws.com"
      }
    }
  ]
}' --region us-east-1 --endpoint-url=http://localhost:4566 || true

aws iam create-role --role-name todo-dev-ecs-task-role --assume-role-policy-document '{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": "sts:AssumeRole",
      "Effect": "Allow",
      "Principal": {
        "Service": "ecs-tasks.amazonaws.com"
      }
    }
  ]
}' --region us-east-1 --endpoint-url=http://localhost:4566 || true

aws iam create-role --role-name todo-dev-rds-monitoring-role --assume-role-policy-document '{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": "sts:AssumeRole",
      "Effect": "Allow",
      "Principal": {
        "Service": "monitoring.rds.amazonaws.com"
      }
    }
  ]
}' --region us-east-1 --endpoint-url=http://localhost:4566 || true

# Create KMS key
aws kms create-key --region us-east-1 --endpoint-url=http://localhost:4566 || true

echo "LocalStack initialization complete!"
