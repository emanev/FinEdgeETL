# FinEdgeETL

## Overview
FinEdgeETL is a data pipeline built with .NET and SQL Server that extracts, transforms, and loads (ETL) transactional data from multiple sources into a centralized database. It supports bulk inserts, parallel processing, and error handling to ensure efficient data processing.

## Features
- Extracts data from CSV files and databases
- Transforms data with business rules (e.g., filtering and deduplication)
- Loads data into SQL Server with bulk insert and upsert (MERGE)
- Parallel processing for scalability
- Logging and error handling using Serilog
- Configurable via `appsettings.json`

## Prerequisites
Before setting up the project, ensure you have the following installed:

- .NET 8.0 SDK ([Download](https://dotnet.microsoft.com/en-us/download/dotnet/8.0))
- SQL Server ([Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads))
- Git ([Download](https://git-scm.com/downloads))
- Visual Studio 2022 or later ([Download](https://visualstudio.microsoft.com/))

## Setup Instructions

### 1. Clone the Repository
```sh
git clone https://github.com/your-username/FinEdgeETL.git
cd FinEdgeETL
```

### 2. Configure Database
1. Open `SQL Server Management Studio (SSMS)`.
2. Execute the SQL script provided (`database_setup.sql`) to create the required databases and tables:
   ```sh
   sqlcmd -S your_server -i database_setup.sql
   ```
3. Update `appsettings.json` with your SQL Server connection details.

### 3. Build the Solution
```sh
dotnet build
```

### 4. Run the ETL Process
```sh
dotnet run --project FinEdgeETL
```

### 5. Run Unit Tests
```sh
dotnet test
```

## Configuration
The project uses `appsettings.json` for configuration. Example:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your_server;Database=FinEdgeAnalytics;User Id=your_user;Password=your_password;",
    "SourceDBConnection": "Server=your_server;Database=SourceDB;User Id=your_user;Password=your_password;"
  },
  "ETL": {
    "CsvFilePath": "Data/transactions.csv"    
  },
  "Logging": {
    "FilePath": "logs/log.txt"
  }
}
```

## Design Choices

### 🔹 Parallel Processing
- Uses `Task.WhenAll()` for parallel data extraction.
- Utilizes `PLINQ` for optimized data transformation.
- Implements `Parallel.ForEach()` for parallel database insertion.

### 🔹 Bulk Insert and Upsert Strategy
- Uses `SqlBulkCopy` for efficient data loading.
- Implements a `MERGE` statement in a stored procedure to handle upserts.

### 🔹 Scalability Considerations
- Supports adding new data sources dynamically via `appsettings.json`.
- Uses table-valued parameters for bulk operations.
- Can be extended with microservices architecture if needed.

### 🔹 Logging and Error Handling
- Logs all ETL steps using `Serilog`.
- Catches and logs errors at every stage to avoid data loss.




