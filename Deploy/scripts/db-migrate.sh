#!/bin/bash
set -e

# Database Migration Script
# This script runs EF Core migrations against the RDS database

ENVIRONMENT=${1:-dev}
MIGRATION_NAME=${2:-""}

if [[ ! $ENVIRONMENT =~ ^(dev|staging|prod)$ ]]; then
  echo "❌ Invalid environment: $ENVIRONMENT"
  echo "Usage: $0 {dev|staging|prod} [migration-name]"
  exit 1
fi

BACKEND_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../Backend" && pwd)"

echo "📦 Running database migrations for $ENVIRONMENT environment..."

cd "$BACKEND_DIR"

if [ -z "$MIGRATION_NAME" ]; then
  echo "🔄 Updating database (latest migration)..."
  dotnet ef database update --context TodoContext
else
  echo "🔄 Updating database to migration: $MIGRATION_NAME..."
  dotnet ef database update "$MIGRATION_NAME" --context TodoContext
fi

echo "✅ Database migration complete!"
