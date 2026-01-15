#!/bin/bash
set -e

# Terraform Backend Initialization Script
# This script initializes the Terraform backend in S3 with DynamoDB state locking

REGION="us-east-1"
STATE_BUCKET="todo-app-terraform-state-$(date +%s)"
LOCK_TABLE="terraform-locks"

echo "🚀 Initializing Terraform Backend..."
echo "Region: $REGION"
echo "State Bucket: $STATE_BUCKET"
echo "Lock Table: $LOCK_TABLE"

# Create S3 bucket for Terraform state
echo "📦 Creating S3 bucket for state..."
aws s3api create-bucket \
  --bucket "$STATE_BUCKET" \
  --region "$REGION" \
  --create-bucket-configuration LocationConstraint="$REGION" || true

# Enable versioning on the bucket
aws s3api put-bucket-versioning \
  --bucket "$STATE_BUCKET" \
  --versioning-configuration Status=Enabled

# Enable encryption on the bucket
aws s3api put-bucket-encryption \
  --bucket "$STATE_BUCKET" \
  --server-side-encryption-configuration '{
    "Rules": [
      {
        "ApplyServerSideEncryptionByDefault": {
          "SSEAlgorithm": "AES256"
        }
      }
    ]
  }'

# Block public access
aws s3api put-public-access-block \
  --bucket "$STATE_BUCKET" \
  --public-access-block-configuration \
    "BlockPublicAcls=true,IgnorePublicAcls=true,BlockPublicPolicy=true,RestrictPublicBuckets=true"

# Create DynamoDB table for state locking
echo "🔐 Creating DynamoDB table for state locking..."
aws dynamodb create-table \
  --table-name "$LOCK_TABLE" \
  --attribute-definitions AttributeName=LockID,AttributeType=S \
  --key-schema AttributeName=LockID,KeyType=HASH \
  --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5 \
  --region "$REGION" || true

echo "✅ Backend initialization complete!"
echo "📝 Use the following in your terraform init command:"
echo "terraform init -backend-config=\"bucket=$STATE_BUCKET\" -backend-config=\"region=$REGION\" -backend-config=\"dynamodb_table=$LOCK_TABLE\""
