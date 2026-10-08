# UrlShortener

A lightweight ASP.NET Core URL shortener service built with .NET 10. It accepts a long URL, generates a short code, stores the mapping in PostgreSQL, and caches lookups in Redis for fast redirect resolution.

## Features

- Create short URLs from long URLs
- Resolve redirect targets using the generated short code
- Persist mappings in PostgreSQL
- Cache frequently accessed mappings in Redis
- Docker-ready container setup
- REST API built with ASP.NET Core controllers

## Tech Stack

- ASP.NET Core Web API on .NET 10
- Entity Framework Core
- PostgreSQL via Npgsql
- Redis via StackExchange.Redis
- Docker

## Project Structure

```text
UrlShortener/
├── Common/
│   └── Result.cs               # Generic result wrapper for service responses
├── Controllers/
│   └── UrlController.cs        # API endpoints for creation and lookup
├── Data/
│   └── ApplicationDbContext.cs  # EF Core database context
├── DTOs/
│   └── CreateShortUrlDto.cs    # DTO for creating a short URL
├── Models/
│   ├── Url.cs                  # Url entity
│   └── Configuration/
│       └── UrlConfiguration.cs  # EF Core model configuration
├── Services/
│   ├── Abstraction/
│   │   ├── ICacheService.cs
│   │   └── IShortUrlService.cs
│   ├── CacheService.cs         # Redis cache implementation
│   └── ShortUrlService.cs      # URL generation and resolution logic
├── appsettings.json            # Default app configuration
├── appsettings.Development.json
├── Dockerfile                  # Multi-stage Docker build for the app
├── Program.cs                 # ASP.NET Core startup configuration
├── UrlShortener.csproj        # Project file and package references
├── UrlShortener.http          # HTTP request examples
└── Properties/
    └── launchSettings.json
```

## API Overview

The service exposes a REST API under `api/Url`.

### Create short URL

- Method: `POST`
- Route: `/api/url`
- Request Body:

```json
{
  "longUrl": "https://example.com/very/long/path"
}
```

- Response:
  - `200 OK` with the generated short URL string when successful
  - `400 Bad Request` with validation or service errors

Example:

```bash
curl -X POST "http://localhost:5037/api/url" \
  -H "Content-Type: application/json" \
  -d '{"longUrl":"https://example.com/very/long/path"}'
```

### Resolve short URL

- Method: `GET`
- Route: `/api/url/{url}`
- Example: `/api/url/AbCd1234`
- Behavior: if the link exists, the API redirects to the original long URL via `RedirectPermanent`

## Data Model

The `Url` model contains:

- `ShortUrl`: unique key used to identify the short URL entry
- `LongUrl`: the original URL to redirect to
- `CreatedAt`: timestamp for creation

The entity configuration sets `ShortUrl` as the primary key.

## Configuration

The application expects the following settings in `appsettings.json` or environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=UrlShortenerDb;Username=postgres;Password=your_password"
  },
  "Domain": "http://localhost:5037",
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

### Required services

- PostgreSQL database running locally or in a container
- Redis instance running locally or in a container

## Runtime Behavior

The application uses a simple architecture:

1. `UrlController` accepts HTTP requests.
2. `ShortUrlService` creates a random 8-character short code and stores it.
3. The URL mapping is saved to PostgreSQL using `ApplicationDbContext`.
4. Redis is used as a cache aside layer to improve read performance.
5. Redirect resolution checks Redis first and falls back to PostgreSQL if there is a cache miss.

## Docker

The project includes a multi-stage Dockerfile:

- Build stage uses the .NET SDK image
- Publish stage creates the output folder
- Runtime stage uses the ASP.NET runtime image
- Container exposes port `8080`

Build the image:

```bash
docker build -t urlshortener .
```

Run the container:

```bash
docker run -p 8080:8080 --name urlshortener urlshortener
```

## Running Locally

### Prerequisites

- .NET 10 SDK
- PostgreSQL instance
- Redis instance

### Restore and run

```bash
dotnet restore

dotnet run --project UrlShortener/UrlShortener.csproj
```

The application is configured to run with ASP.NET Core development settings and can be launched via the default project profile in `Properties/launchSettings.json`.

## Notes

- The generated short code is random and attempts to avoid duplicate primary key collisions.
- The application is a backend-focused API and does not include a frontend UI.
- In its current form, the service expects the base domain to be configured correctly so that generated short URLs are valid for the environment where the app is hosted.

## Future Improvements

Potential enhancements for this project include:

- URL validation and normalization
- Rate limiting and abuse prevention
- Click analytics / statistics tracking
- Expiration support for short URLs
- Better short-code collision handling and uniqueness strategy
- Swagger/OpenAPI documentation generation
- Health checks and monitoring
