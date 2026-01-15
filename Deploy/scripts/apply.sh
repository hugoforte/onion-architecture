#!/bin/bash
set -e

# Terraform Apply Script
# This script applies terraform configuration for a specific environment

ENVIRONMENT=${1:-dev}
TERRAFORM_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

if [[ ! $ENVIRONMENT =~ ^(dev|staging|prod)$ ]]; then
  echo "❌ Invalid environment: $ENVIRONMENT"
  echo "Usage: $0 {dev|staging|prod}"
  exit 1
fi

echo "🚀 Applying infrastructure for $ENVIRONMENT environment..."
cd "$TERRAFORM_DIR/environments/$ENVIRONMENT"

if [ -f tfplan ]; then
  terraform apply tfplan
else
  terraform apply
fi

echo "✅ Infrastructure deployment complete!"
echo "📝 Run: terraform show"
