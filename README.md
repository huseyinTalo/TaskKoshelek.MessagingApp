# TaskKoshelek MessagingApp

A distributed messaging application built with .NET, featuring real-time updates and comprehensive telemetry monitoring.

## Architecture

This application follows a service-oriented architecture with two main services:

### Frontend Service (UI)
- .NET MVC application
- Real-time message updates using SignalR
- RESTful communication with backend API
- Port: 9090

### Backend Service (API)
- RESTful API with Clean Architecture
- OpenTelemetry integration
- SQL Server database
- Comprehensive request/response logging
- Port: 9098

## Features

- Real-time messaging with order preservation
- Message history with 10-minute window view
- Comprehensive system telemetry
- Performance monitoring and tracing
- Docker containerization
- Swagger API documentation

## Tech Stack

- **Frontend**: .NET MVC, SignalR, JavaScript
- **Backend**: .NET Web API
- **Database**: SQL Server
- **Monitoring**: OpenTelemetry
- **Containerization**: Docker
- **Documentation**: Swagger

## Prerequisites

- Docker and Docker Compose
- .NET SDK 8.0
- SQL Server (provided via Docker)

## Getting Started

1. Clone the repository:
```bash
git clone https://github.com/yourusername/TaskKoshelek.MessagingApp.git
cd TaskKoshelek.MessagingApp
```

2. Start the application using Docker Compose:
```bash
docker-compose up --build
```

3. Access the applications:
- UI: http://localhost:9090
- API: http://localhost:9098
- Swagger Documentation: http://localhost:9098/swagger

## Project Structure

```
TaskKoshelek.MessagingApp/
├── TaskKoshelek.MessagingApp.UI/       # Frontend Service
├── TaskKoshelek.MessagingApp.API/      # Backend API
├── TaskKoshelek.MessagingApp.Core/     # Domain Models
├── TaskKoshelek.MessagingApp.DAL/      # Data Access Layer
└── docker-compose.yml                   # Container orchestration
```

## Configuration

Environment variables in docker-compose.yml:
- `DB_CONNECTION_STRING`: Main database connection
- `ASPNETCORE_ENVIRONMENT`: Development/Production environment
- Additional connection strings for telemetry

## API Endpoints

- `POST /api/messages/create`: Create new message
- `GET /api/messages/all`: Get all messages
- `GET /api/messages/all10minutes`: Get recent messages
- `GET /api/messages/latest-order-number`: Get latest message order

## Monitoring and Telemetry

The application includes comprehensive monitoring:
- Request/response logging
- Performance metrics
- Operation tracking
- Distributed tracing
- SQL query monitoring

## Database Schema

Two main databases:
1. Application Database (TaskKoshelek.MessagingApp.DB)
   - Messages table
2. Telemetry Database (MessagingAppTelemetry)
   - RequestLogs
   - OperationLogs
   - PerformanceMetrics
  
## List of technologies

  .NET 8.0,
  ASP.NET Core,
  ASP.NET Core Web API,
  ASP.NET Core MVC,
  System.Data.SqlClient,
  AutoMapper,
  OpenTelemetry,
  SignalR,
  JavaScript,
  Microsoft SQL Server 2022,
  Swagger/OpenAPI,
  Docker,
  Docker Compose,
  HTTP/REST,
  WebSockets,

This task was created according to this document:

The task is to create a simple web-based messaging service.

The service consists of three components:

Web server
SQL database (preferably PostgreSQL)
Three clients:
The first client sends messages.
The second client receives and displays messages in real time.
The third client retrieves and displays messages from the last minute.
All three client applications can be implemented as separate web applications or as a single application with different URLs (whichever is more convenient).

Each message consists of:

A text (up to 128 characters)
A timestamp (set by the server)
A sequence number (sent by the client)
System Workflow:
The first client sends messages to the service (one API call per message).
The service processes each message, stores it in the database, and forwards it to the second client via WebSocket.
The second client receives messages via WebSocket and displays them in real time (showing the timestamp and sequence number).
The third client retrieves and displays the history of messages from the last 10 minutes.
Server API (REST or GraphQL):
Send a message
Retrieve messages within a date range
Additional Requirements:
Swagger documentation (if using REST API)
Programming languages: C# or Go
Architecture: MVC or similar
DAL layer without ORM
Logging should be implemented to allow debugging and understanding the current state of the application
Dockerized deployment
GitHub repository with docker-compose.yml to start all system components
Notes:
UI design is not important, as the focus is on backend development.
Clients are simple test utilities that a backend developer should be able to write.
