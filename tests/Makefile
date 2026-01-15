# Terraform Test Setup Makefile

.PHONY: setup teardown test-vpc test-ecs test-rds test-all lint fmt validate clean help

# Variables
TF_VERSION = 1.5.0

# Help target
help:
	@echo "Test Targets:"
	@echo "  make setup          - Verify Terraform installation"
	@echo "  make test-all       - Run all validation tests"
	@echo "  make test-vpc       - Test VPC module validation"
	@echo "  make test-ecs       - Test ECS module validation"
	@echo "  make test-rds       - Test RDS module validation"
	@echo "  make lint           - Lint all Terraform code"
	@echo "  make fmt            - Format all Terraform code"
	@echo "  make clean          - Clean test artifacts"
	@echo "  make validate       - Validate all modules and environments"

# Verify Terraform setup
setup:
	@echo "🚀 Verifying Terraform setup..."
	@which terraform > /dev/null || (echo "❌ Terraform not found" && exit 1)
	@terraform --version
	@echo "✅ Terraform is ready!"

# Test VPC module
test-vpc: setup
	@echo "🧪 Testing VPC module..."
	cd terraform/test_fixtures/vpc_test && \
		terraform init -upgrade > /dev/null 2>&1 && \
		terraform validate
	@echo "✅ VPC module validation passed!"

# Test ECS module
test-ecs: setup
	@echo "🧪 Testing ECS module..."
	cd terraform/test_fixtures/ecs_test && \
		terraform init -upgrade > /dev/null 2>&1 && \
		terraform validate
	@echo "✅ ECS module validation passed!"

# Test RDS module
test-rds: setup
	@echo "🧪 Testing RDS module..."
	cd terraform/test_fixtures/rds_test && \
		terraform init -upgrade > /dev/null 2>&1 && \
		terraform validate
	@echo "✅ RDS module validation passed!"

# Run all tests
test-all: test-vpc test-ecs test-rds validate
	@echo ""
	@echo "✅ All tests passed!"

# Lint Terraform code
lint:
	@echo "🔍 Linting Terraform code..."
	terraform fmt -recursive -check ../modules ../environments 2>/dev/null || \
		(echo "⚠️  Some files need formatting" && exit 1)
	@echo "✅ All files are properly formatted!"

# Format Terraform code
fmt:
	@echo "📝 Formatting Terraform code..."
	terraform fmt -recursive ../modules ../environments
	@echo "✅ Formatting complete!"

# Validate all Terraform
validate:
	@echo "✔️  Validating all Terraform configurations..."
	@for dir in ../modules/* ../environments/*/; do \
		if [ -d "$$dir" ]; then \
			echo "Validating $$dir"; \
			cd "$$dir" && terraform validate > /dev/null 2>&1 || exit 1; \
			cd - > /dev/null; \
		fi \
	done
	@echo "✅ All Terraform validations passed!"

# Clean up test artifacts
clean:
	@echo "🧹 Cleaning up test artifacts..."
	@find terraform -name ".terraform" -type d -exec rm -rf {} + 2>/dev/null || true
	@find terraform -name ".terraform.lock.hcl" -delete 2>/dev/null || true
	@find terraform -name "tfplan" -delete 2>/dev/null || true
	@echo "✅ Cleanup complete!"

# Full cleanup (including state)
distclean: clean
	@echo "🗑️  Full cleanup complete"
