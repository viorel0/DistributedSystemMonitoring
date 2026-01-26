# 📊 Distributed System Monitoring

## Full-stack solution designed to collect and visualize real-time sensor data from multiple distributed nodes. This project demonstrates a robust integration between an Asynchronous Web API and a WinForms Desktop Interface.

# 🏗️ Project Architecture

### The solution is divided into two main components:

* Backend (WebApplication1): An ASP.NET Core Web API built with .NET 9. It handles data ingestion, storage logic using Dapper ORM, and exposes RESTful endpoints.
* Frontend (Interfata): A Windows Forms application that consumes the API to display real-time metrics and sensor types for each registered node.
