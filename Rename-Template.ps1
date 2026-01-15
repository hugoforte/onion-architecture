#Requires -Version 5.1

<#
.SYNOPSIS
    Renames the Starter Todo App template to a custom project name and namespace.

.DESCRIPTION
    This script automates the renaming of the template from "Starter" to your custom project name.
    It updates:
    - Project files and folder names
    - Namespace references throughout the codebase
    - Solution file
    - Docker image names
    - Configuration files

.PARAMETER NewProjectName
    The new project name (e.g., "MyAwesomeApp", "InvoicingSystem")
    Used for folder names, Docker images, and file names.

.PARAMETER NewNamespace
    The new root namespace (e.g., "MyCompany.MyApp", "Acme.Invoicing")
    Used for C# namespace references throughout the backend.

.PARAMETER TemplateRootPath
    The root path of the template (defaults to current directory).

.EXAMPLE
    .\Rename-Template.ps1 -NewProjectName "InvoicingSystem" -NewNamespace "Acme.Invoicing"

.EXAMPLE
    .\Rename-Template.ps1 -NewProjectName "MyApp" -NewNamespace "Company.MyApp" -TemplateRootPath "C:\Projects\template"

.NOTES
    - Backup your template before running this script
    - The script performs string replacements in many files
    - Some manual adjustments may be needed for custom configurations
    - Run from the template root directory or specify TemplateRootPath
#>

param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$NewProjectName,

    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$NewNamespace,

    [Parameter(Mandatory = $false)]
    [string]$TemplateRootPath = (Get-Location).Path
)

$ErrorActionPreference = "Stop"

# Color functions
function Write-ColorOutput($color, $message) {
    Write-Host $message -ForegroundColor $color
}

function Write-Success($message) {
    Write-ColorOutput "Green" "✓ $message"
}

function Write-Warning($message) {
    Write-ColorOutput "Yellow" "⚠ $message"
}

function Write-Error($message) {
    Write-ColorOutput "Red" "✗ $message"
}

function Write-Info($message) {
    Write-ColorOutput "Cyan" "ℹ $message"
}

# Constants
$OldProjectName = "Starter"
$OldNamespace = "Starter"

Write-Info "Template Renaming Utility"
Write-Info "=============================="
Write-Info "Old Project Name: $OldProjectName"
Write-Info "New Project Name: $NewProjectName"
Write-Info "Old Namespace: $OldNamespace"
Write-Info "New Namespace: $NewNamespace"
Write-Info "Template Root: $TemplateRootPath"
Write-Info ""

# Verify template root exists
if (-not (Test-Path $TemplateRootPath)) {
    Write-Error "Template root path does not exist: $TemplateRootPath"
    exit 1
}

# Verify we're in a template directory
$expectedFolders = @("Backend", "Frontend", "Deploy")
$missingFolders = $expectedFolders | Where-Object { -not (Test-Path (Join-Path $TemplateRootPath $_)) }

if ($missingFolders) {
    Write-Error "Missing expected folders: $($missingFolders -join ', ')"
    exit 1
}

Write-Info "Template structure verified"
Write-Info ""

# Step 1: Rename project folders
Write-Info "Step 1: Renaming project folders..."

$folderMappings = @(
    "Backend\0_Core\Common",
    "Backend\0_Core\Contracts",
    "Backend\0_Core\Domain",
    "Backend\0_Core\Messaging",
    "Backend\1_Infrastructure\Infrastructure",
    "Backend\2_Application\Services",
    "Backend\2_Application\Services.Abstractions",
    "Backend\3_Run\Web",
    "Backend\3_Run\ServiceBus",
    "Backend\Tests\UnitTests",
    "Backend\Tests\AcceptanceTests"
)

foreach ($folder in $folderMappings) {
    $folderPath = Join-Path $TemplateRootPath $folder
    if (Test-Path $folderPath) {
        $csprojFile = Get-ChildItem $folderPath -Name "Starter*.csproj" -ErrorAction SilentlyContinue
        if ($csprojFile) {
            $newCsprojName = $csprojFile -replace "^Starter", $NewProjectName
            $oldCsprojPath = Join-Path $folderPath $csprojFile
            $newCsprojPath = Join-Path $folderPath $newCsprojName
            
            Rename-Item -Path $oldCsprojPath -NewName $newCsprojName
            Write-Success "Renamed: $csprojFile → $newCsprojName"
        }
    }
}

# Step 2: Update solution file
Write-Info ""
Write-Info "Step 2: Updating solution file..."

$slnFile = Get-ChildItem $TemplateRootPath -Name "*.sln" | Select-Object -First 1
if ($slnFile) {
    $slnPath = Join-Path $TemplateRootPath $slnFile
    $content = Get-Content $slnPath -Raw
    
    # Replace project names in solution file
    $content = $content -replace "Starter\.Domain", "$NewProjectName.Domain"
    $content = $content -replace "Starter\.Contracts", "$NewProjectName.Contracts"
    $content = $content -replace "Starter\.Messaging", "$NewProjectName.Messaging"
    $content = $content -replace "Starter\.Common", "$NewProjectName.Common"
    $content = $content -replace "Starter\.Services\.Abstractions", "$NewProjectName.Services.Abstractions"
    $content = $content -replace "Starter\.Services", "$NewProjectName.Services"
    $content = $content -replace "Starter\.Infrastructure", "$NewProjectName.Infrastructure"
    $content = $content -replace "Starter\.Web", "$NewProjectName.Web"
    $content = $content -replace "Starter\.ServiceBus", "$NewProjectName.ServiceBus"
    $content = $content -replace "Starter\.UnitTests", "$NewProjectName.UnitTests"
    $content = $content -replace "Starter\.AcceptanceTests", "$NewProjectName.AcceptanceTests"
    
    Set-Content -Path $slnPath -Value $content
    Write-Success "Updated: $slnFile"
}

# Step 3: Update C# files with new namespace
Write-Info ""
Write-Info "Step 3: Updating C# namespaces in Backend..."

$csFiles = Get-ChildItem $TemplateRootPath -Filter "*.cs" -Recurse -Path "$TemplateRootPath\Backend"

foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    # Replace namespace declarations
    $content = $content -replace "^namespace $OldNamespace", "namespace $NewNamespace" -Flags Multiline
    $content = $content -replace "^using $OldNamespace", "using $NewNamespace" -Flags Multiline
    
    # Replace assembly references
    $content = $content -replace "\[$OldProjectName\.", "[$NewProjectName."
    $content = $content -replace 'InternalsVisibleTo\("$OldProjectName\.', "InternalsVisibleTo(""$NewProjectName."
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content
        Write-Success "Updated: $($file.FullName.Replace($TemplateRootPath, ''))"
    }
}

# Step 4: Update project files
Write-Info ""
Write-Info "Step 4: Updating .csproj project references..."

$csprojFiles = Get-ChildItem $TemplateRootPath -Filter "*.csproj" -Recurse

foreach ($file in $csprojFiles) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    # Replace project references
    $content = $content -replace "Starter\.Domain", "$NewProjectName.Domain"
    $content = $content -replace "Starter\.Contracts", "$NewProjectName.Contracts"
    $content = $content -replace "Starter\.Messaging", "$NewProjectName.Messaging"
    $content = $content -replace "Starter\.Common", "$NewProjectName.Common"
    $content = $content -replace "Starter\.Services\.Abstractions", "$NewProjectName.Services.Abstractions"
    $content = $content -replace "Starter\.Services", "$NewProjectName.Services"
    $content = $content -replace "Starter\.Infrastructure", "$NewProjectName.Infrastructure"
    $content = $content -replace "Starter\.Web", "$NewProjectName.Web"
    $content = $content -replace "Starter\.ServiceBus", "$NewProjectName.ServiceBus"
    
    # Update AssemblyName
    $content = $content -replace "<AssemblyName>Starter\.", "<AssemblyName>$NewProjectName."
    $content = $content -replace "<AssemblyName>Starter</AssemblyName>", "<AssemblyName>$NewProjectName</AssemblyName>"
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content
        Write-Success "Updated: $($file.FullName.Replace($TemplateRootPath, ''))"
    }
}

# Step 5: Update configuration files
Write-Info ""
Write-Info "Step 5: Updating configuration files..."

# appsettings.json
$appsettingsFiles = Get-ChildItem $TemplateRootPath -Filter "appsettings*.json" -Recurse

foreach ($file in $appsettingsFiles) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    $content = $content -replace '"Starter', "'$NewProjectName'"
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content
        Write-Success "Updated: $($file.FullName.Replace($TemplateRootPath, ''))"
    }
}

# Step 6: Update Docker configuration
Write-Info ""
Write-Info "Step 6: Updating Docker configuration..."

$dockerComposeFile = Join-Path $TemplateRootPath "Backend\3_Run\Docker\docker-compose.yml"
if (Test-Path $dockerComposeFile) {
    $content = Get-Content $dockerComposeFile -Raw
    $originalContent = $content
    
    $content = $content -replace "starter-", "$($NewProjectName.ToLower())-"
    $content = $content -replace "Starter\.", "$NewProjectName."
    
    if ($content -ne $originalContent) {
        Set-Content -Path $dockerComposeFile -Value $content
        Write-Success "Updated: docker-compose.yml"
    }
}

$dockerfileFile = Join-Path $TemplateRootPath "Backend\3_Run\Web\Dockerfile"
if (Test-Path $dockerfileFile) {
    $content = Get-Content $dockerfileFile -Raw
    $originalContent = $content
    
    $content = $content -replace "Starter\.Web\.dll", "$NewProjectName.Web.dll"
    
    if ($content -ne $originalContent) {
        Set-Content -Path $dockerfileFile -Value $content
        Write-Success "Updated: Dockerfile"
    }
}

# Step 7: Update Terraform configuration
Write-Info ""
Write-Info "Step 7: Updating Terraform configuration..."

$tfFiles = Get-ChildItem $TemplateRootPath -Filter "*.tf" -Recurse

foreach ($file in $tfFiles) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    $projectNameLower = $NewProjectName.ToLower()
    $content = $content -replace 'starter-todo-app', "$projectNameLower-app"
    $content = $content -replace 'starter-api', "$projectNameLower-api"
    $content = $content -replace '"starter', "'$projectNameLower'"
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content
        Write-Success "Updated: $($file.FullName.Replace($TemplateRootPath, ''))"
    }
}

# Step 8: Update React/TypeScript components
Write-Info ""
Write-Info "Step 8: Updating React/TypeScript configuration..."

$tsxFiles = Get-ChildItem $TemplateRootPath -Filter "*.tsx" -Recurse -Path "$TemplateRootPath\Frontend"
$tsFiles = Get-ChildItem $TemplateRootPath -Filter "*.ts" -Recurse -Path "$TemplateRootPath\Frontend"
$jsonFiles = Get-ChildItem $TemplateRootPath -Filter "package.json" -Recurse -Path "$TemplateRootPath\Frontend"

foreach ($file in ($tsxFiles + $tsFiles + $jsonFiles)) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    $content = $content -replace "@starter/", "@$($NewProjectName.ToLower())/"
    $content = $content -replace '"starter', """$($NewProjectName.ToLower())"""
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content
        Write-Success "Updated: $($file.FullName.Replace($TemplateRootPath, ''))"
    }
}

# Step 9: Update README files
Write-Info ""
Write-Info "Step 9: Updating README files..."

$readmeFiles = Get-ChildItem $TemplateRootPath -Filter "README.md" -Recurse

foreach ($file in $readmeFiles) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    $content = $content -replace "Starter", $NewProjectName
    $content = $content -replace "starter-todo-app", "$($NewProjectName.ToLower())-app"
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content
        Write-Success "Updated: $($file.FullName.Replace($TemplateRootPath, ''))"
    }
}

# Summary
Write-Info ""
Write-Success "=============================="
Write-Success "Template renamed successfully!"
Write-Success "=============================="
Write-Info ""
Write-Info "Next steps:"
Write-Info "1. Verify the changes look correct"
Write-Info "2. Run: dotnet clean && dotnet build"
Write-Info "3. Run: dotnet test"
Write-Info "4. Test locally: cd Backend/3_Run/Web && dotnet run"
Write-Info ""
Write-Info "If you encounter any issues:"
Write-Info "- Check the solution file: *.sln"
Write-Info "- Verify namespace declarations in *.cs files"
Write-Info "- Check project references in *.csproj files"
Write-Info ""
