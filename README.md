# 📊 Distributed System Monitoring

## Full-stack solution designed to collect and visualize real-time sensor data from multiple distributed nodes. This project demonstrates a robust integration between an Asynchronous Web API and a WinForms Desktop Interface.

<img width="1147" height="661" alt="winform" src="https://github.com/user-attachments/assets/371fcbb5-eca6-42a0-92a6-e83fffc8dc88" />

# 🏗️ Project Architecture

### The solution is divided into two main components:

* Backend (WebApplication1): An ASP.NET Core Web API built with .NET 9. It handles data ingestion, storage logic using Dapper ORM, and exposes RESTful endpoints.
* Frontend (Interfata): A Windows Forms application that consumes the API to display real-time metrics and sensor types for each registered node.

# 🛠️ Tech Stack

* Framework: .NET 10 (C#)
* Data Access: Dapper ORM
* Database: Microsoft SQL Server

# 🗄️ Database Schema

Schema is designed for Microsoft SQL Server (T-SQL)

```sql
CREATE TABLE Nodes (
    NodeID INT IDENTITY(1,1) PRIMARY KEY,
    NodeName NVARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE Measurements (
    MeasureID BIGINT IDENTITY(1,1) PRIMARY KEY,
    NodeID INT NOT NULL,
    SensorType NVARCHAR(50) NOT NULL,
    SensorValue FLOAT NOT NULL,
    RecordedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Node_Measurements FOREIGN KEY (NodeID) 
    REFERENCES Nodes(NodeID) ON DELETE CASCADE
);
```
# ⚙️ Setup & Installation

1. Database Setup
* Open SQL Server Management Studio (SSMS).
* Create a new database named Measurements.
* Execute the SQL script provided above to create the tables.

2. Configuration
Update the appsettings.json file in the WebApplication1 project with your local connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=Measurements;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3. Execution
* Open the .slnx (or .sln) file in Visual Studio.
* Set both WebApplication1 and Interfata as Startup Projects.
* Press F5 to run the full system.
