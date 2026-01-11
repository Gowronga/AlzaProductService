# AlzaProductService – Summary

AlzaProductService is a backend service implemented in **C# using .NET 10**, providing product data for an e-shop.
The solution was built with a focus on **clean architecture**, **maintainability**, and **production-ready practices**.

The service exposes both **REST** and **GraphQL** APIs and includes full documentation and unit tests.

---

## Requirements Covered

* REST API to list all products
* REST API to retrieve a product by ID
* REST API to partially update product description (PATCH)
* API versioning with pagination support
* GraphQL alternative endpoint
* Swagger / OpenAPI documentation
* Unit tests covering core functionality
* Layered architecture following SOLID principles
* .NET and C# using a modern framework version

All requirements are fully implemented.

---

## API Overview

REST API:

* **v1**: Basic product endpoints without pagination
* **v2**: Paginated product listing

  * Allowed page sizes: 10, 30, 50
  * Default page size: 10

GraphQL:

* Query products
* Query product by ID
* Update product description
* Accessible via `/graphql`

Swagger:

* Available at `/swagger` in Development environment
* Separate documentation for v1 and v2 endpoints

---

## Architecture

The solution follows a clean layered architecture:

* **Domain** – core entities and business logic
* **Application** – DTOs and interfaces
* **Infrastructure** – EF Core, repositories, database access
* **API** – REST controllers, GraphQL schema, configuration
* **Tests** – unit tests for repositories and controllers

The design avoids deprecated libraries and keeps validation logic inside the application code.

---

## Data & Configuration

* Entity Framework Core is used for data access
* Supports SQL Server and InMemory database
* Easy switching between mock data and real database via configuration

---

## Testing

* Unit tests implemented with xUnit, Moq, and FluentAssertions
* Repository tests use EF Core InMemory provider
* Controller tests use mocked repositories
* Tests are isolated, fast, and deterministic

---

## Result

The final solution delivers a **production-quality backend service** that meets all stated requirements, follows modern .NET best practices, and is easy to extend and maintain.

---
