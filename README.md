# ExaminationSystem-gp2-team5
# 📚 Examination System — Online LMS Platform

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white)

A full-featured, enterprise-grade online examination platform built with **ASP.NET Core Web API**, following **Clean Architecture**, **CQRS**, and **MediatR** patterns — developed as a team capstone project for the Elevate Advanced .NET Bootcamp.

</div>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [API Endpoints](#-api-endpoints)
- [Design Patterns](#-design-patterns)
- [Team](#-team)

---

## 🔍 Overview

The **Examination System** is a backend REST API that powers a learning management system (LMS) supporting multiple user roles — **Admin** and **Student**. It enables administrators to manage exams and track performance trends, while students can view personalized exam history, scores, and progress.

The system was built by a team of 5 developers following Agile/Scrum methodology with Jira for sprint planning and GitHub for version control.

---

## ✨ Features

### 👤 Admin
- Full exam management (create, update, delete, schedule exams)
- Comprehensive dashboard analytics:
  - Statistics on total exams, active students, and pass/fail rates
  - Performance trend reports using complex LINQ and EF Core queries
- Student management and result oversight
- Role-based access control

### 🎓 Student
- Register and authenticate securely via JWT
- Take scheduled, timed exams
- View personalized dashboard:
  - Exam history and scores
  - Progress tracking over time
- Paginated result browsing with efficient data loading

### ⚙️ System
- Reusable **Generic Pagination Service** with a `PaginationResult<T>` wrapper for handling large datasets
- **CurrentUser Service** for centralized JWT claims extraction (user identity + roles) across the application
- Clean separation between application layers via Clean Architecture + CQRS

---

## 🏗️ Architecture

The project follows **Clean Architecture** with strict layer separation:

```
┌─────────────────────────────────────┐
│           Presentation Layer        │  ← ASP.NET Core Web API (Controllers)
├─────────────────────────────────────┤
│           Application Layer         │  ← CQRS Commands/Queries, MediatR Handlers
├─────────────────────────────────────┤
│             Domain Layer            │  ← Entities, Interfaces, Domain Logic
├─────────────────────────────────────┤
│         Infrastructure Layer        │  ← EF Core, SQL Server, JWT, Repositories
└─────────────────────────────────────┘
```

### CQRS Flow

```
HTTP Request
    │
    ▼
Controller
    │ sends Command / Query via MediatR
    ▼
Handler (Application Layer)
    │ uses Repository / DbContext
    ▼
SQL Server (via EF Core)
    │
    ▼
Mapped Response (via Mapster)
    │
    ▼
HTTP Response
```

---

## 🛠️ Tech Stack

| Category | Technology |
|---|---|
| Framework | ASP.NET Core Web API (.NET 8) |
| Language | C# |
| ORM | Entity Framework Core (Code First) |
| Database | SQL Server |
| Caching | Redis |
| Authentication | JWT Bearer Tokens |
| Mediator | MediatR (CQRS pattern) |
| Mapping | Mapster |
| Architecture | Clean Architecture |
| API Docs | Swagger / OpenAPI |
| Version Control | Git & GitHub |
| Project Management | Jira (Agile/Scrum) |

---

## 📁 Project Structure

```
ExaminationSystem/
│
├── ExaminationSystem.API/              # Presentation layer — Controllers, Middleware, Program.cs
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ExamController.cs
│   │   └── DashboardController.cs
│   └── Program.cs
│
├── ExaminationSystem.Application/      # Application layer — CQRS, Handlers, DTOs, Services
│   ├── Features/
│   │   ├── Exams/
│   │   │   ├── Commands/
│   │   │   └── Queries/
│   │   ├── Students/
│   │   └── Dashboard/
│   ├── Common/
│   │   ├── Pagination/
│   │   │   ├── PaginationService.cs
│   │   │   └── PaginationResult.cs
│   │   └── Services/
│   │       └── CurrentUserService.cs
│   └── Interfaces/
│
├── ExaminationSystem.Domain/           # Domain layer — Entities, Enums, Domain Interfaces
│   ├── Entities/
│   │   ├── Exam.cs
│   │   ├── Student.cs
│   │   ├── Question.cs
│   │   └── ExamResult.cs
│   └── Enums/
│
└── ExaminationSystem.Infrastructure/   # Infrastructure layer — EF Core, Repositories, JWT
    ├── Data/
    │   ├── AppDbContext.cs
    │   └── Migrations/
    ├── Repositories/
    └── Services/
        └── JwtService.cs
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (or SQL Server Express / LocalDB)
- [Redis](https://redis.io/download/) (or use Docker: `docker run -d -p 6379:6379 redis`)
- [Git](https://git-scm.com/)

### Installation

1. **Clone the repository**

```bash
git clone https://github.com/elevate-exam-system-groups/ExaminationSystem-gp2-team5.git
cd ExaminationSystem-gp2-team5
git checkout main-testing
```

2. **Configure the connection strings**

Open `ExaminationSystem.API/appsettings.json` and update:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ExaminationSystemDB;Trusted_Connection=True;TrustServerCertificate=True",
    "Redis": "localhost:6379"
  },
  "JWT": {
    "Key": "YOUR_SECRET_KEY_MIN_32_CHARS",
    "Issuer": "ExaminationSystem",
    "Audience": "ExaminationSystemUsers",
    "DurationInDays": 7
  }
}
```

3. **Apply database migrations**

```bash
cd ExaminationSystem.Infrastructure
dotnet ef database update --startup-project ../ExaminationSystem.API
```

4. **Run the application**

```bash
cd ExaminationSystem.API
dotnet run
```

5. **Open Swagger UI**

Navigate to: `https://localhost:{port}/swagger`

---

## 📡 API Endpoints

### 🔐 Authentication

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `POST` | `/api/auth/register` | Register a new student | Public |
| `POST` | `/api/auth/login` | Login and receive JWT token | Public |

### 📊 Admin Dashboard

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `GET` | `/api/dashboard/stats` | Overall exam & student statistics | Admin |
| `GET` | `/api/dashboard/performance` | Performance trend analytics | Admin |
| `GET` | `/api/dashboard/results` | All student results (paginated) | Admin |

### 📝 Exams

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `GET` | `/api/exams` | List all exams (paginated) | Admin |
| `POST` | `/api/exams` | Create a new exam | Admin |
| `PUT` | `/api/exams/{id}` | Update exam details | Admin |
| `DELETE` | `/api/exams/{id}` | Delete an exam | Admin |
| `GET` | `/api/exams/{id}/start` | Start a timed exam session | Student |
| `POST` | `/api/exams/{id}/submit` | Submit exam answers | Student |

### 👤 Student Dashboard

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `GET` | `/api/student/dashboard` | Personalized exam history & scores | Student |
| `GET` | `/api/student/progress` | Progress tracking over time | Student |
| `GET` | `/api/student/results` | Paginated personal results | Student |

> **Note:** All protected endpoints require the `Authorization: Bearer {token}` header.

---

## 🧩 Design Patterns

### Generic Pagination Service

Handles large datasets efficiently across the entire application:

```csharp
// Usage in any query handler
var paginatedResult = await _paginationService.CreateAsync(
    source: query,
    pageIndex: request.PageIndex,
    pageSize: request.PageSize
);
// Returns: PaginationResult<T> { Items, TotalCount, PageIndex, PageSize }
```

### CurrentUser Service

Centralizes user identity extraction from JWT claims, eliminating repeated boilerplate:

```csharp
public interface ICurrentUserService
{
    string UserId { get; }
    string Email { get; }
    IEnumerable<string> Roles { get; }
    bool IsInRole(string role);
}
```

### CQRS with MediatR

Every feature is a self-contained Command or Query:

```csharp
// Query example
public record GetStudentDashboardQuery(string StudentId) : IRequest<StudentDashboardDto>;

// Handler
public class GetStudentDashboardHandler : IRequestHandler<GetStudentDashboardQuery, StudentDashboardDto>
{
    public async Task<StudentDashboardDto> Handle(GetStudentDashboardQuery request, CancellationToken ct)
    {
        // Fetch and return personalized data
    }
}
```

---

## 👥 Team

Built by a team of 5 developers as part of the **Elevate Advanced .NET Bootcamp** Capstone Project.

| Contributor | Role / Focus Area |
|---|---|
| [Karim Salah](https://github.com/karimsalahabdelghany) | Admin Dashboard Analytics, Student Dashboard Queries, Pagination Service, CurrentUser Service |
| Team Member 2 | *(add name and role)* |
| Team Member 3 | *(add name and role)* |
| Team Member 4 | *(add name and role)* |
| Team Member 5 | *(add name and role)* |

---

## 📄 License

This project was developed for educational purposes as part of the Elevate Advanced .NET Bootcamp capstone. All rights reserved by the team members.

---

<div align="center">
  <sub>Built with ❤️ using ASP.NET Core · Clean Architecture · CQRS · MediatR</sub>
</div>
