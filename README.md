# VSA CQRS Template

An open-source starter template for building ASP.NET Core Web APIs with
**Vertical Slice Architecture (VSA)** and **CQRS**.

The template provides a small but practical foundation for a new API. It
includes feature-based endpoint organization, command and query handlers,
FluentValidation, Entity Framework Core, ASP.NET Core Identity, JWT
authentication, database migrations, role seeding, and OpenAPI documentation.
The authentication feature is an example of how application features are
structured; it is intended to be replaced or extended for your domain.

## What this is for

Use this repository when you want to start a .NET API without first creating
the same application plumbing for every project. It is designed to keep code
for one use case together instead of organizing the whole application into
large global folders such as `Controllers`, `Services`, and `Repositories`.

Each feature owns the code needed for its use cases:

- The endpoint defines the HTTP contract.
- The command or query defines the request.
- The handler contains the use-case behavior.
- The validator contains input rules.
- Feature-specific responses remain close to the feature that uses them.

This approach makes features easier to find, change, test, and remove. The
shared `Common` and `Infrastructure` folders contain only cross-cutting
building blocks and technical concerns.

## Included features

- ASP.NET Core minimal APIs
- Vertical Slice Architecture
- CQRS abstractions for commands and queries
- Convention-based endpoint discovery
- FluentValidation
- Entity Framework Core 10
- PostgreSQL configuration through `Npgsql.EntityFrameworkCore.PostgreSQL`
- ASP.NET Core Identity with `ApplicationUser`
- JWT access tokens and refresh tokens
- Automatic application of EF Core migrations at startup
- Initial role seeding
- OpenAPI and Scalar API reference in development
- A .NET project template definition for `dotnet new`

The application currently uses PostgreSQL in `Program.cs`. MySQL support is
also referenced by the project, but changing database providers requires
updating the DbContext configuration and migrations for the provider you
choose.

## Requirements

- .NET SDK 10.0 or later
- PostgreSQL
- The `dotnet` CLI

Check the installed SDK with:

```bash
dotnet --version
```

## Use the repository as a project

1. Clone the repository:

   ```bash
   git clone https://github.com/Fidel-Jr/vsa-cqrs-template.git
   cd vsa-cqrs-template/VsaCrqsTemplate
   ```

2. Configure a PostgreSQL connection string and a JWT signing key. Do not
   commit real credentials or signing keys. For local development, User
   Secrets are preferable:

   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=vsa_template;Username=postgres;Password=your-password"
   dotnet user-secrets set "Jwt:Key" "replace-with-a-long-development-only-signing-key"
   ```

   The corresponding configuration keys are:

   - `ConnectionStrings:DefaultConnection`
   - `Jwt:Key`
   - `Jwt:Issuer`
   - `Jwt:Audience`
   - `Jwt:ExpiresInMinutes`

3. Restore, build, and run:

   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```

   On startup, the application applies pending migrations and seeds the
   configured roles. The database must be reachable before the application
   starts.

4. In development, open the Scalar API reference at the URL printed by the
   application. The OpenAPI document is also exposed by the development
   configuration.

## Use it as a `dotnet new` template

From the repository directory that contains `.template.config/template.json`,
install the template locally:

```bash
dotnet new install .
```

Create a new API from the installed template:

```bash
dotnet new vsa-cqrs-template -n Catalog.Api
cd Catalog.Api
dotnet restore
dotnet build
```

The template replaces the source name `VsaCqrsTemplate` with the project name
you provide and generates HTTP and HTTPS development ports. To see the
installed template:

```bash
dotnet new list vsa-cqrs-template
```

To uninstall the local template:

```bash
dotnet new uninstall .
```

## Project structure

```text
.
├── .template.config/
│   └── template.json       # dotnet new template metadata and exclusions
├── Common/
│   ├── Abstractions/       # Shared contracts and Result<T>
│   ├── Cqrs/               # Command/query and handler interfaces
│   └── Extensions/         # CQRS registration and endpoint discovery
├── Features/
│   └── Auth/
│       ├── Login/          # Login command, handler, validator, endpoint
│       ├── Me/             # Current-user query, handler, response, endpoint
│       └── Register/       # Registration command, handler, validator, endpoint
├── Infrastructure/
│   ├── Persistence/        # EF Core DbContext and persistence models
│   │   └── Auth/            # Identity user, current user, refresh token
│   └── Security/           # JWT service implementation
├── Migrations/              # EF Core database migrations
├── Seeders/                 # Startup data seeding, including roles
├── Program.cs               # Dependency injection and HTTP pipeline setup
├── appsettings*.json        # Application configuration defaults
└── VsaCqrsTemplate.csproj   # Target framework and NuGet dependencies
```

Generated build directories such as `bin/` and `obj/` are intentionally not
part of the template output.

## Adding a feature

Create a folder under `Features` for the business capability, then create one
subfolder per use case. For example:

```text
Features/
└── Products/
    └── Create/
        ├── CreateProductCommand.cs
        ├── CreateProductEndpoint.cs
        ├── CreateProductHandler.cs
        └── CreateProductValidator.cs
```

A typical command feature should:

1. Define a request that implements `ICommand<TResponse>`.
2. Implement `ICommandHandler<TCommand, TResponse>`.
3. Add a validator when the request has input rules.
4. Implement `IEndpoint` and map the endpoint in `MapEndpoint`.
5. Return the shared `Result<T>` type when success and failure need to be
   represented explicitly.

Queries follow the same pattern with `IQuery<TResponse>` and
`IQueryHandler<TQuery, TResponse>`. Endpoint discovery and handler
registration are wired from the assembly in `Program.cs`, so a new feature
does not require a central controller or route registry.

## Authentication example

The starter includes these example routes:

| Method | Route | Purpose |
|---|---|---|
| `POST` | `/api/auth/register` | Register a user |
| `POST` | `/api/auth/login` | Authenticate and issue access/refresh tokens |
| `GET` | `/api/auth/me` | Return the authenticated user |

Use the authentication feature as a reference implementation, then adjust the
claims, roles, token lifetime, password policy, and response contracts for
your application.

## Database migrations

The application calls `Database.MigrateAsync()` during startup. After changing
an EF Core model, create a migration from the project directory:

```bash
dotnet ef migrations add DescribeYourChange
dotnet ef database update
```

Review generated migrations before applying them in a shared or production
environment. For production deployments, many teams prefer applying migrations
as an explicit deployment step rather than from application startup.

## Production checklist

Before deploying an API created from this template:

- Replace all placeholder configuration values.
- Store the JWT key and database credentials in a managed secret store.
- Use a strong, sufficiently long JWT signing key.
- Review password, token, refresh-token, and role policies.
- Review and harden CORS, HTTPS, logging, and allowed hosts for your hosting
  environment.
- Decide whether startup migrations and development OpenAPI exposure are
  appropriate for production.
- Add automated tests for each feature and its authorization rules.
- Review the generated EF Core migrations and database permissions.

## Contributing

Issues and pull requests are welcome. When proposing changes, keep the
template-oriented experience in mind: avoid project-specific assumptions,
document new setup requirements, and keep new features consistent with the
vertical-slice structure.

## License

No license file is currently included. Add a license before distributing
derivative projects or accepting contributions under an explicit open-source
license.
