#!/bin/bash
set -e

# Terraform Plan Script
# This script runs terraform plan for a specific environment

ENVIRONMENT=${1:-dev}
TERRAFORM_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

if [[ ! $ENVIRONMENT =~ ^(dev|staging|prod)$ ]]; then
  echo "❌ Invalid environment: $ENVIRONMENT"
  echo "Usage: $0 {dev|staging|prod}"
  exit 1
fi

echo "📋 Planning infrastructure for $ENVIRONMENT environment..."
cd "$TERRAFORM_DIR/environments/$ENVIRONMENT"

terraform plan -out=tfplan

echo "✅ Plan complete! Review the above changes."
echo "📝 Run: terraform apply tfplan"
