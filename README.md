# TaskKoshelek MessagingApp

A distributed messaging application built with .NET, featuring real-time updates and comprehensive telemetry monitoring.

---

## 🚀 **Architecture**

This application follows a service-oriented architecture with two main services:

### 🖥️ **Frontend Service (UI)**
- .NET MVC application
- Real-time message updates using SignalR
- RESTful communication with backend API
- **Port:** `9090`

### 🔧 **Backend Service (API)**
- RESTful API with Clean Architecture
- OpenTelemetry integration
- SQL Server database
- Comprehensive request/response logging
- **Port:** `9098`

---

## ✨ **Features**

✅ Real-time messaging with order preservation  
✅ Message history with a 10-minute window view  
✅ Comprehensive system telemetry  
✅ Performance monitoring and tracing  
✅ Docker containerization  
✅ Swagger API documentation  

---

## 🛠 **Tech Stack**

- **Frontend:** .NET MVC, SignalR, JavaScript
- **Backend:** .NET Web API
- **Database:** SQL Server
- **Monitoring:** OpenTelemetry
- **Containerization:** Docker
- **Documentation:** Swagger

---

## ⚙️ **Prerequisites**

- Docker & Docker Compose
- .NET SDK 8.0
- SQL Server (provided via Docker)

---

## 🚀 **Getting Started**

1. **Clone the repository:**
   ```bash
   git clone https://github.com/huseyinTalo/TaskKoshelek.MessagingApp.git
   cd TaskKoshelek.MessagingApp
   ```

2. **Start the application using Docker Compose:**
   ```bash
   docker-compose up --build
   ```

3. **Access the applications:**
   - **UI:** [http://localhost:9090](http://localhost:9090)
   - **API:** [http://localhost:9098](http://localhost:9098)
   - **Swagger Documentation:** [http://localhost:9098/swagger](http://localhost:9098/swagger)

---

## 📁 **Project Structure**

```
TaskKoshelek.MessagingApp/
├── TaskKoshelek.MessagingApp.UI/       # Frontend Service
├── TaskKoshelek.MessagingApp.API/      # Backend API
├── TaskKoshelek.MessagingApp.Core/     # Domain Models
├── TaskKoshelek.MessagingApp.DAL/      # Data Access Layer
└── docker-compose.yml                   # Container orchestration
```

---

## 🔧 **Configuration**

Environment variables in `docker-compose.yml`:
- `DB_CONNECTION_STRING`: Main database connection
- `ASPNETCORE_ENVIRONMENT`: Development/Production environment
- Additional connection strings for telemetry

---

## 🔗 **API Endpoints**

| Method | Endpoint                          | Description                      |
|--------|-----------------------------------|----------------------------------|
| `POST` | `/api/messages/create`           | Create a new message            |
| `GET`  | `/api/messages/all`              | Get all messages                 |
| `GET`  | `/api/messages/all10minutes`     | Get recent messages (last 10 min) |
| `GET`  | `/api/messages/latest-order-number` | Get the latest message order    |

---

## 📊 **Monitoring and Telemetry**

The application includes comprehensive monitoring:
- ✅ Request/response logging
- ✅ Performance metrics
- ✅ Operation tracking
- ✅ Distributed tracing
- ✅ SQL query monitoring

---

## 🗄 **Database Schema**

### **Databases:**
1. **Application Database (`TaskKoshelek.MessagingApp.DB`)**
   - `Messages` table
2. **Telemetry Database (`MessagingAppTelemetry`)**
   - `RequestLogs`
   - `OperationLogs`
   - `PerformanceMetrics`

---

## 🛠 **List of Technologies**

- .NET 8.0  
- ASP.NET Core  
- ASP.NET Core Web API  
- ASP.NET Core MVC  
- System.Data.SqlClient  
- AutoMapper  
- OpenTelemetry  
- SignalR  
- JavaScript  
- Microsoft SQL Server 2022  
- Swagger/OpenAPI  
- Docker  
- Docker Compose  
- HTTP/REST  
- WebSockets  

---

### 📌 **This project was created based on the following requirements:**

The service consists of three components:
- **Web server**
- **SQL database (preferably PostgreSQL)**
- **Three clients:**
  - Client 1: Sends messages
  - Client 2: Receives and displays messages in real time
  - Client 3: Retrieves and displays messages from the last 10 minutes

Each message contains:
- **Text** (up to 128 characters)
- **Timestamp** (set by the server)
- **Sequence number** (sent by the client)

### **System Workflow:**
1. **Client 1** sends messages to the service.
2. **Backend** processes messages, stores them in the database, and forwards them to **Client 2** via WebSocket.
3. **Client 2** receives and displays messages in real-time.
4. **Client 3** retrieves and displays the last 10 minutes of message history.

### **Additional Requirements:**
- **Swagger documentation** (for REST API)
- **Languages:** C# or Go
- **Architecture:** MVC or similar
- **DAL layer without ORM**
- **Comprehensive logging**
- **Dockerized deployment**
- **GitHub repository with Docker Compose**

---

### 🎨 **Notes:**
- UI design **is not important** (focus is on backend development).
- Clients are **simple test utilities** that a backend developer should be able to write.

🚀 **Happy Coding!** 🎉

