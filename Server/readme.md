# Recipe Application - Backend Server

A robust and scalable backend for the Recipe Application, built with **.NET 8**. This server manages user authentication, recipe storage, and media handling, providing a high-performance RESTful API for the frontend client.

---

## 🚀 Tech Stack

- **Framework**: [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens) with BCrypt hashing
- **File Storage**: Cloudinary Integration
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Logging**: Serilog
- **Documentation**: Swagger/OpenAPI

---

## ✨ Features

- **User Authentication**: Secure register/login flow with JWT, including OTP support for email verification.
- **Recipe Management**: Full CRUD operations for recipes, including image uploads.
- **Advanced Validation**: Robust request validation using FluentValidation.
- **Structured Logging**: Comprehensive application logging with Serilog.
- **Secure Handling**: Passwords managed with BCrypt; environment variables handled via `DotNetEnv`.

---

## 🛠️ Project Structure

```text
Server/
├── Controllers/    # API Controllers (Auth, Recipe)
├── DTOs/           # Data Transfer Objects
├── Handlers/       # Business Logic & Templates
├── Models/         # Data Models & DB Context
├── Repositories/   # Data Access Layer
├── Services/       # Application Services (Auth, Image, etc.)
├── Validators/     # Request Validation Logic
└── Program.cs      # Application Entry Point & Configuration
```

---

## 📋 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Environment Configuration

1. Create a `.env` file in the root directory.
2. Add the following required environment variables:
   ```env
   CLOUDINARY_URL=your_cloudinary_url
   JWT_KEY=your_secret_jwt_key
   DATABASE_CONNECTION=your_sql_server_connection_string
   ```
3. Update `appsettings.json` with your database connection details under `ConnectionStrings`.

### Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/krrishmittal/Recipe-Application.git
   cd Recipe-Application/Server
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Apply Database Migrations**:
   ```bash
   dotnet ef database update
   ```

4. **Run the application**:
   ```bash
   dotnet run
   ```
   The API will be available at `http://localhost:7212` (or the port configured in `launchSettings.json`).

---

## 📖 API Documentation

Once the server is running, you can explore the API using Swagger:

`http://localhost:<port>/swagger/index.html`

### Key Endpoints

- **Auth**: `/api/Auth/...` (Register, Login, OTP Verification)
- **Recipes**: `/api/Recipe/...` (List, Get, Create, Update, Delete)

---

## 🤝 Contributing

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.
