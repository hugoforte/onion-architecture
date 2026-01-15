#!/bin/bash
set -e

# Terraform Destroy Script
# This script destroys terraform infrastructure for a specific environment

ENVIRONMENT=${1:-dev}
TERRAFORM_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

if [[ ! $ENVIRONMENT =~ ^(dev|staging|prod)$ ]]; then
  echo "❌ Invalid environment: $ENVIRONMENT"
  echo "Usage: $0 {dev|staging|prod}"
  exit 1
fi

echo "⚠️  WARNING: You are about to destroy infrastructure in $ENVIRONMENT!"
read -p "Are you sure? (type 'yes' to confirm): " confirm

if [ "$confirm" != "yes" ]; then
  echo "Cancelled."
  exit 0
fi

echo "💥 Destroying infrastructure for $ENVIRONMENT environment..."
cd "$TERRAFORM_DIR/environments/$ENVIRONMENT"

terraform destroy

echo "✅ Infrastructure destroyed!"
