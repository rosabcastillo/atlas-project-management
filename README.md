# Atlas - Project Management

A modern project management application for tracking resources, projects, and allocations built with .NET 8 and Blazor Server.

## Features

- **Projects** - Manage projects with status, priority, delivery method, and accountable lead
- **Resources** - Track team members with roles, skills, and vendor associations
- **Allocations** - Assign resources to projects with flexible date ranges and percentage allocation
- **Calendar View** - Monthly calendar view for visualizing resource and project allocations
- **Authentication** - Cookie-based admin authentication with secure password hashing

## Tech Stack

- **.NET 8.0**
- **Blazor Server** - Interactive server-side UI
- **Entity Framework Core** - ORM with SQLite database
- **Clean Architecture** - Domain, Application, Infrastructure, and Web layers

## Project Structure

```
src/
├── ProjectManagement.Domain/        # Entities, enums, domain logic
├── ProjectManagement.Application/   # Application services, interfaces
├── ProjectManagement.Infrastructure/# Data access, EF Core, seeding
└── ProjectManagement.Web/           # Blazor components, pages, UI
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Run the Application

```bash
cd src/ProjectManagement.Web
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`.

### Default Admin Credentials

- **Username:** admin
- **Password:** admin123

## Database

The application uses SQLite with automatic database creation and seeding. The database file (`projectmanagement.db`) is created in the Web project directory on first run.

## License

MIT
