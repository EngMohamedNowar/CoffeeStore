# CoffeeStore

A coffee-store catalogue and basket API built with .NET 10 using Clean Architecture.

## Solution layout

Everything lives under this folder and is tracked by a single solution file,
`CoffeeStore.slnx`:

```
CoffeeStore.slnx            the only solution file
ECommerce/                  API host - controllers, DI wiring, middleware
ECommerce.Domain/           Entities, repository contracts, specification contracts
ECommerce.Application/      Services, DTOs, mapping profiles, FluentValidation validators
ECommerce.Infrastructure/   EF Core DbContexts, repositories, Redis, Identity, migrations
tests/ECommerce.Tests/      xUnit tests
```

Build and test from the repository root with:

```bash
dotnet build CoffeeStore/CoffeeStore.slnx
dotnet test  CoffeeStore/CoffeeStore.slnx
```

Dependency flow is strictly inward: `Api -> Infrastructure -> Application -> Domain`.

### Two databases, two contexts

| Context                 | Database                 | Holds                                     |
| ----------------------- | ------------------------ | ----------------------------------------- |
| `StoreDbContext`        | `CoffeeStoreDb`          | Catalogue: products, categories, orders   |
| `StoreIdentityDbContext` | `CoffeeStoreDb.Identity` | ASP.NET Identity: users, roles, addresses  |

Migrations for each context live in separate folders and are discovered per-context,
so neither applies the other's migrations.

### Patterns in use

- **Result** instead of exceptions (`Result`, `Result<T>`, `Error`, `ErrorType`).
  Errors are mapped to HTTP status codes in `ApiControllerBase.ToProblem`.
- **Specification** for composable queries (`ISpecification<T>`, `BaseSpecification<T>`,
  `SpecificationEvaluator`).
- **Repository + Unit of Work**. `SpecificationEvaluator.CreateCountQuery` is used for
  counts so that paging on a specification can never truncate the reported total.
- **Keyed DI** for seeders (`AddKeyedScoped<IDataSeeder, ...>("Catalog" | "Identity")`).
- **Soft delete** via `IsDeleted` with EF global query filters.
- **JWT bearer** authentication, configured through `Jwt:SigningKey`.

## Running locally

### 1. Prerequisites

- .NET SDK 10.0.x
- Docker (for SQL Server and Redis)

### 2. Configure secrets

Secrets are **not** committed. `appsettings.json` ships with empty placeholders.

```bash
cp .env.example .env
# edit .env and set MSSQL_SA_PASSWORD
```

Start the dependencies:

```bash
docker compose up -d
```

Then set the connection strings and signing key through user-secrets:

```bash
cd CoffeeStore/ECommerce
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=127.0.0.1,1433;Database=CoffeeStoreDb;User Id=sa;Password=<your password>;TrustServerCertificate=True"
dotnet user-secrets set "ConnectionStrings:IdentityConnection" "Server=127.0.0.1,1433;Database=CoffeeStoreDb.Identity;User Id=sa;Password=<your password>;TrustServerCertificate=True"
dotnet user-secrets set "Jwt:SigningKey" "<at least 32 characters>"
```

To seed an admin account, also set:

```bash
dotnet user-secrets set "Seed:Admin:UserName" "..."
dotnet user-secrets set "Seed:Admin:Email" "..."
dotnet user-secrets set "Seed:Admin:Password" "..."
```

Seeding is skipped with a warning if `Seed:Admin` is incomplete.

In deployed environments use environment variables instead
(`ConnectionStrings__DefaultConnection`, `Jwt__SigningKey`, `Seed__Admin__Email`, ...).

### 3. Run

```bash
dotnet run --project CoffeeStore/ECommerce
```

Migrations and seed data are applied on startup. Swagger is available at `/swagger`
in the Development environment.

## Tests

```bash
dotnet test CoffeeStore/CoffeeStore.slnx
```

The suite covers the specification evaluator, the `Result` type, and the product
service against an in-memory database. Tests around counting and the not-found path
are regression guards: each one was verified to fail when its fix is reverted.

## Configuration reference

| Key                          | Purpose                                        |
| ---------------------------- | ---------------------------------------------- |
| `ConnectionStrings:DefaultConnection`   | Catalogue database              |
| `ConnectionStrings:IdentityConnection`  | Identity database               |
| `ConnectionStrings:RedisConnection`     | Redis, used for cache and baskets |
| `Jwt:Issuer` / `Jwt:Audience`           | Token validation parameters      |
| `Jwt:SigningKey`                        | HMAC-SHA256 key, minimum 32 characters |
| `Jwt:ExpiryMinutes`                     | Token lifetime                   |
| `Seed:Admin:*`                          | Optional admin account to seed   |
| `BaseUrl`                               | Prefix used to build absolute image URLs |
