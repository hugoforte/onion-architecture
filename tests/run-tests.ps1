#!/usr/bin/env pwsh

# Terraform Validation Tests

$ErrorActionPreference = "Stop"

function Write-Header {
    param([string]$Message)
    Write-Host "`n" -NoNewline
    Write-Host "🚀 $Message" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-Error {
    param([string]$Message)
    Write-Host "❌ $Message" -ForegroundColor Red
}

function Test-TerraformInstalled {
    Write-Header "Checking Terraform installation..."
    try {
        $version = terraform --version
        Write-Host $version
        Write-Success "Terraform is installed and ready!"
        return $true
    }
    catch {
        Write-Error "Terraform not found. Please install Terraform 1.5+`n$_"
        return $false
    }
}

function Test-Module {
    param([string]$ModulePath, [string]$ModuleName)
    Write-Header "Testing $ModuleName module..."
    
    try {
        Push-Location $ModulePath
        
        Write-Host "  → Running terraform init..."
        terraform init -upgrade -no-color -input=false | Out-Null
        
        Write-Host "  → Running terraform validate..."
        $output = terraform validate
        Write-Host $output
        
        Pop-Location
        Write-Success "$ModuleName module validation passed!"
        return $true
    }
    catch {
        Write-Error "$ModuleName module validation failed!`n$_"
        Pop-Location
        return $false
    }
}

function Test-AllModules {
    Write-Header "Running all module tests..."
    
    $results = @{
        vpc = $false
        ecs = $false
        rds = $false
    }
    
    # Test individual modules
    $results.vpc = Test-Module "terraform/test_fixtures/vpc_test" "VPC"
    $results.ecs = Test-Module "terraform/test_fixtures/ecs_test" "ECS"
    $results.rds = Test-Module "terraform/test_fixtures/rds_test" "RDS"
    
    return $results
}

# Main execution
Write-Host "`n╔════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  Terraform Infrastructure Tests       ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Cyan

if (-not (Test-TerraformInstalled)) {
    exit 1
}

$results = Test-AllModules

Write-Host "`n╔════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  Test Results                         ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Cyan

$allPassed = $true
foreach ($module in @("vpc", "ecs", "rds")) {
    if ($results[$module]) {
        $status = "✅ PASSED"
    } else {
        $status = "❌ FAILED"
        $allPassed = $false
    }
    Write-Host "  $module : $status"
}

if ($allPassed) {
    Write-Host "`n" -NoNewline
    Write-Success "All tests passed!"
    exit 0
}
else {
    Write-Host "`n" -NoNewline
    Write-Error "Some tests failed!"
    exit 1
}
