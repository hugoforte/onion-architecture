# C# Payments

## Dependencies

### Required Software
- **Docker Desktop** - Required for running the application containers

### Windows Installation Script
Run the following PowerShell script as Administrator to install dependencies:

**To run PowerShell as Administrator:**
1. Press `Windows + X` and select "Windows PowerShell (Admin)" or "Terminal (Admin)"
2. Or search for "PowerShell" in Start menu, right-click and select "Run as administrator"

```powershell
Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Force
$ChocoPackages = @(
    "wsl2",
    "docker-desktop"
)
 
$ChocoInstall = Join-Path ([System.Environment]::GetFolderPath("CommonApplicationData")) "Chocolatey\bin\choco.exe"
 
if(!(Test-Path $ChocoInstall)) {
     try {
 
         Invoke-Expression ((New-Object net.webclient).DownloadString("https://chocolatey.org/install.ps1")) -ErrorAction Stop
     }
     catch {
         Throw "Failed to install Chocolatey"
     }       
}
 
foreach($Package in $ChocoPackages) {
     try {
         Invoke-Expression "cmd.exe /c $ChocoInstall Install $Package -y" -ErrorAction Stop
     }
     catch {
         Throw "Failed to install $Package"
     }
}
```

## System Design
```mermaid
flowchart TB
    LM[LinenMaster Legacy]
    LMW[LinenMaster Web]
    LMC[LinenMaster Customer Portal]
    MySQL@{ shape: cyl, label: "Client-Specific DB
    _MySQL_" }
    Producer[Arbitrary Producer]

    LM --- MySQL
    LMW --- MySQL
    LMC --- MySQL

    NServiceBus([NServiceBus
    _SQS_])
    LM ---> Payments
    LMC ---> Payments
    NServiceBus -.-> |Listen for Results|LMW
    Producer ---> Payments
    NServiceBus -.-> |Listen for Results|Producer

    Payments[Payments
    _C#_]
    Postgres@{ shape: cyl, label: "Payments DB
    _Postgres_" }
    React["Payments FE
    _React_"]
    Payments --- Postgres
    Payments <--> React
    Payments -.-> |Post results once payment complete|NServiceBus

    Payabli[/Payabli/]
    Payments <--> |For Automated, Tokenized Payments|Payabli
    React <--> Payabli

    style MySQL fill:#e3f2fd,stroke:#0d47a1,stroke-width:1.5px,color: #1a1a1a;
    style Postgres fill:#ede7f6,stroke:#4527a0,stroke-width:1.5px,color: #1a1a1a;
    %% style Snowflake fill:#e0f7fa,stroke:#006064,stroke-width:1.5px,color: #1a1a1a;
    style NServiceBus fill:#f3e5f5,stroke:#6a1b9a,stroke-width:1.5px,color: #1a1a1a;
    style Payabli fill:#fff3e0,stroke:#ef6c00,stroke-width:1.5px,color: #1a1a1a;
    style LM fill:#f1f8e9,stroke:#33691e,stroke-width:1.5px,color: #1a1a1a;
    style LMW fill:#f1f8e9,stroke:#33691e,stroke-width:1.5px,color: #1a1a1a;
    style LMC fill:#f1f8e9,stroke:#33691e,stroke-width:1.5px,color: #1a1a1a;
    style Payments fill:#ede7f6,stroke:#311b92,stroke-width:1.5px,color: #1a1a1a;
    style React fill:#e0f7fa,stroke:#00838f,stroke-width:1.5px,color: #1a1a1a;
```

## DB Design
erDiagram
    Biller ||--o{ Invoice : creates
    Biller ||--o{ Customer : belongs_to
    Customer ||--o{ Invoice : receives
    Invoice ||--|{ InvoiceLineItem : contains
    Invoice ||--o{ Payment : has
    PaymentMethod ||--o{ Payment : used_for
    Customer ||--o{ PaymentMethod : owns
    User }o--|| Customer : belongs_to
    PaymentMethod }o--|| User : optionally_owned_by
    Customer }o--|| PaymentGateway : uses

    Biller {
        long Id
        Guid PublicId
        string Name
        string ApiKey
        DateTime CreatedAt
        DateTime UpdatedAt
    }

    Customer {
        long Id
        Guid PublicId
        long BillerId
        long PaymentGatewayId
        string Name
        bool AutopayEnabled
        DateTime CreatedAt
        DateTime UpdatedAt
    }

    Invoice {
        long Id
        Guid PublicId
        long BillerId
        long CustomerId
        DateTime DueDate
        JsonDocument Fees
        bool PassThruFees
        decimal SalesTax
        decimal TotalAmount
        string Currency
        string Status
        DateTime CreatedAt
        DateTime UpdatedAt
    }

    InvoiceLineItem {
        long Id
        long InvoiceId
        string Description
        int Quantity
        decimal UnitPrice
        DateTime CreatedAt
        DateTime UpdatedAt
    }

    InvoiceLog {
        string Object
        int ObjectId
        JsonDocument Changes
        DateTime CreatedAt
    }

    PaymentMethod {
        long Id
        Guid PublicId
        long CustomerId
        long OwnerUserId
        string Token
        string Type
        string Last4
        string Brand
        DateTime ExpiryDate
        bool IsShared
        DateTime CreatedAt
        DateTime UpdatedAt
    }

    Payment {
        long Id
        Guid PublicId
        long InvoiceId
        long PaymentMethodId
        string Status
        int AttemptCount
        decimal Amount
        string TransactionReference
        DateTime CreatedAt
        DateTime UpdatedAt
    }

    User {
        long Id
        Guid PublicId
        long CustomerId
        string Email
        string Name
        DateTime CreatedAt
        DateTime UpdatedAt
    }

    PaymentGateway {
        long Id
        string Name
        string Type
        DateTime CreatedAt
        DateTime UpdatedAt
    }

