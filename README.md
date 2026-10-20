# Soap Shop API

Product service of a small online soap shop, built on .NET 9 with **clean architecture** and designed to
run behind an API gateway.

## Architecture

```
Presentation  ──►  Infrastructure  ──►  Application  ──►  Domain
      │                  │
      └──────── Core ────┘   (JWT scheme, global exception handling, gateway-only middleware, responses)
```

| Project | Contents |
|---|---|
| `Domain` | `Product` entity (name, price, quantity) |
| `Application` | `ProductDto`, entity ↔ DTO conversions, `IProduct` contract |
| `Infrastructure` | EF Core `ProductDbContext`, `ProductRepository`, DI registration |
| `Core` | Generic repository interface, JWT authentication scheme, Serilog setup, `GlobalException` and `ListenToOnlyApiGateway` middleware, `Response` type |
| `Presentation` | `ProductController`, Swagger |

## Endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/api/product/get-products` | List products |
| GET | `/api/product/get-product/{id}` | Get one product |
| POST | `/api/product/add-product` | Create a product |
| PUT | `/api/product/update-product` | Update a product |
| DELETE | `/api/product/delete-product` | Delete a product |

## Notable details

- **Gateway-only access (prepared):** `ListenToOnlyApiGateway` middleware rejects requests without the
  `Api-Gateway` header (`503`). It is registered but commented out in `Core`, so the service can be run
  stand-alone; enable it once the gateway is in place.
- **Consistent errors:** `GlobalException` maps exceptions and status codes (401, 403, 429, timeouts)
  to a single problem-details response and logs them.
- **Structured logging** with Serilog (debug, console and daily rolling file).

## Tech

.NET 9 · ASP.NET Core Web API · Entity Framework Core · SQL Server (LocalDB) · JWT Bearer · Serilog · Swagger

## Run

```bash
dotnet user-secrets init --project Presentation
dotnet user-secrets set "Authentication:Key" "<your-32+-character-key>" --project Presentation
dotnet ef database update --project Infrastructure --startup-project Presentation
dotnet run --project Presentation
```
