# .NET CRUD API - Product Management System

A professional RESTful API for managing products with full CRUD (Create, Read, Update, Delete) operations. Built with .NET 8, Entity Framework Core, and SQLite.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-512BD4?logo=entity-framework)
![SQLite](https://img.shields.io/badge/SQLite-003B57?logo=sqlite)

## 📋 Table of Contents

- [Features](#features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Data Model](#data-model)
- [Database](#database)
- [Testing with Swagger](#testing-with-swagger)
- [Architecture](#architecture)
- [Future Enhancements](#future-enhancements)

## ✨ Features

- **Full CRUD Operations**: Create, Read, Update, and Delete products
- **RESTful API Design**: Follows REST principles with standard HTTP methods
- **Data Validation**: Input validation with error messages
- **Swagger Documentation**: Interactive API documentation with XML comments
- **SQLite Database**: Lightweight, file-based database (no server required)
- **Entity Framework Core**: Modern ORM for database operations
- **Error Handling**: Comprehensive error handling with logging
- **Async/Await**: Non-blocking operations for better performance

## 🛠 Technology Stack

- **.NET 8.0** - Latest .NET framework
- **ASP.NET Core** - Web framework for building APIs
- **Entity Framework Core 8.0** - ORM for database operations
- **SQLite** - Embedded database
- **Swagger/OpenAPI** - API documentation and testing
- **C#** - Programming language

## 📁 Project Structure

```
CrudApp/
├── Controllers/
│   └── ProductsController.cs    # API endpoints for product operations
├── Models/
│   └── Product.cs                # Product data model with validation
├── Data/
│   └── ApplicationDbContext.cs   # Entity Framework database context
├── Properties/
│   └── launchSettings.json       # Application launch configuration
├── Program.cs                    # Application entry point and configuration
├── appsettings.json              # Application settings and connection strings
└── CrudApp.csproj                # Project file with dependencies
```

## 🚀 Getting Started

### Prerequisites

- **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- Verify installation: `dotnet --version` (should show 8.x.x)

### Installation & Running

1. **Clone or download the project**
   ```bash
   cd CrudApp
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Access the application**
   - Swagger UI: `http://localhost:5000/swagger`
   - API Base URL: `http://localhost:5000/api/products`

The SQLite database (`products.db`) will be automatically created on first run.

## 📡 API Endpoints

| Method | Endpoint | Description | Response Codes |
|--------|----------|-------------|----------------|
| `GET` | `/api/products` | Get all products | 200 OK |
| `GET` | `/api/products/{id}` | Get product by ID | 200 OK, 404 Not Found |
| `POST` | `/api/products` | Create new product | 201 Created, 400 Bad Request |
| `PUT` | `/api/products/{id}` | Update existing product | 204 No Content, 400 Bad Request, 404 Not Found |
| `DELETE` | `/api/products/{id}` | Delete product | 204 No Content, 404 Not Found |

### Example Requests

**Create a Product:**
```http
POST /api/products
Content-Type: application/json

{
  "name": "Laptop",
  "description": "High-performance gaming laptop",
  "price": 1299.99,
  "category": "Electronics"
}
```

**Update a Product:**
```http
PUT /api/products/1
Content-Type: application/json

{
  "id": 1,
  "name": "Updated Laptop",
  "description": "Updated description",
  "price": 1199.99,
  "category": "Electronics"
}
```

## 📊 Data Model

### Product

```json
{
  "id": 1,
  "name": "Product Name",
  "description": "Product Description",
  "price": 29.99,
  "category": "Category Name",
  "createdAt": "2024-01-01T00:00:00Z"
}
```

### Validation Rules

- **Name**: Required, maximum 200 characters
- **Description**: Optional, maximum 1000 characters
- **Price**: Required, must be greater than 0
- **Category**: Optional, maximum 100 characters
- **CreatedAt**: Automatically set to current UTC time

## 💾 Database

- **Type**: SQLite (file-based)
- **File**: `products.db` (created automatically)
- **Location**: Project root directory
- **ORM**: Entity Framework Core
- **Migrations**: Automatic schema creation on first run

The database file persists data between application restarts. You can view it using SQLite browser tools or delete it to start fresh.

## 🧪 Testing with Swagger

Swagger UI provides an interactive interface to test all API endpoints:

1. Navigate to `http://localhost:5000/swagger`
2. Expand any endpoint
3. Click "Try it out"
4. Fill in the required parameters
5. Click "Execute"
6. View the response

Swagger includes:
- Complete API documentation
- XML comments and descriptions
- Request/response examples
- Try-it-out functionality

## 🏗 Architecture

```
┌─────────────┐
│   Client    │ (Swagger UI, Postman, Frontend)
└──────┬──────┘
       │ HTTP Requests (GET, POST, PUT, DELETE)
       ▼
┌─────────────┐
│  Controller │ (ProductsController)
│  - Routing  │
│  - Validation│
│  - Error Handling│
└──────┬──────┘
       │
       ▼
┌─────────────┐
│  DbContext  │ (ApplicationDbContext)
│  - EF Core  │
│  - LINQ     │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│  SQLite DB  │ (products.db)
│  - Tables   │
│  - Data     │
└─────────────┘
```

### Key Components

1. **Controllers**: Handle HTTP requests and responses
2. **Models**: Define data structure and validation
3. **DbContext**: Manages database connections and operations
4. **Entity Framework**: Translates C# code to SQL queries

## 🔮 Future Enhancements

Potential improvements for production use:

- [ ] Authentication & Authorization (JWT tokens)
- [ ] Pagination for product listings
- [ ] Search and filtering capabilities
- [ ] Image upload for products
- [ ] Caching for better performance
- [ ] Unit and integration tests
- [ ] Docker containerization
- [ ] CI/CD pipeline setup
- [ ] API versioning
- [ ] Rate limiting
- [ ] Logging to file/database
- [ ] Health check endpoints

## 📝 License

This project is open source and available for educational purposes.

## 👤 Author

Built as a learning project to demonstrate .NET API development with Entity Framework Core.

---

**Note**: This is a development/demo project. For production use, consider adding authentication, proper error handling, logging, and security measures.
