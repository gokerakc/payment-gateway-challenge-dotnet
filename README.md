# Payment Gateway API

A .NET-based payment processing gateway that validates payment requests, processes them through a bank, and enables payment retrieval.

## Features

- Process payments with credit card details
- Retrieve payment details by ID
- Validate payment requests
- Bank integration simulation
- Structured logging with Serilog
- Global exception handling
- OpenAPI documentation (Swagger)

## Prerequisites

- .NET 8.0 SDK
- Docker Desktop or Podman Desktop
- Visual Studio Code or Visual Studio 2022 or Rider

## Getting Started

### Running Locally

1. Clone the repository:
```bash
git clone https://github.com/gokerakc/payment-gateway-challenge-dotnet.git
cd payment-gateway-challenge-dotnet
```

2. Build the solution:
```bash
dotnet build
```

3. Run the tests:
```bash
dotnet test
```

4. Start the bank simulator and API:
```bash
docker-compose up -d
dotnet run --project src/PaymentGateway.Api
```

The API will be available at:
- HTTPS: https://localhost:5001
- Swagger UI: https://localhost:5001/swagger

### Running with Docker

Build and run everything using Docker Compose:

```bash
docker-compose up --build -d
```

## Testing

The solution includes multiple test projects:

- Unit Tests: `PaymentGateway.UnitTests`
- Integration Tests: `PaymentGateway.IntegrationTests`

Run all tests:
```bash
dotnet test
```

Run specific test project:
```bash
dotnet test test/PaymentGateway.UnitTests
```

## API Endpoints

### Make Payment

```http
POST /api/payments
Content-Type: application/json
x-merchant-id: your-merchant-id
x-idempotency-token: 123e4567-e89b-12d3-a456-426614174000

{
  "cardNumber": "4111111111111111",
  "expiryMonth": 12,
  "expiryYear": 2025,
  "currency": "GBP",
  "amount": 1000,
  "cvv": "123"
}
```

### Retrieve Payment

```http
GET /api/payments/{paymentId}
x-merchant-id: your-merchant-id
```

## Project Structure

```
src/
  PaymentGateway.Api/        # Main API project
    Features/                # API endpoints and contracts
    Domain/                  # Domain models and services
    Infrastructure/         # External dependencies implementation
    Ports/                 # Interfaces for infrastructure
test/
  PaymentGateway.UnitTests/       # Unit tests
  PaymentGateway.IntegrationTests/ # Integration tests
imposters/ - contains the bank simulator configuration. Don't change this

.editorconfig - don't change this. It ensures a consistent set of rules for submissions when reformatting code
docker-compose.yml - configures the bank simulator
PaymentGateway.sln
```

## Technical Decisions

- Minimal API endpoints for lightweight implementation
- Clean Architecture principles for separation of concerns
- In-memory storage for simplicity (can be replaced with proper database)
- Integration tests using WebApplicationFactory
- Structured logging with Serilog
- Global exception handling with ProblemDetails
- Resilient bank API communication
  - Uses `StandardResilienceHandler` for automatic retries
  - Handles transient failures gracefully
  - Built-in circuit breaker protection
  - Default retry policies for HTTP 5xx responses
- Idempotency support using x-idempotency-token header
  - Prevents duplicate payment processing
  - Allows safe retry of failed requests
  - Token must be a valid GUID
- Merchant identification via x-merchant-id header
  - Enables multi-tenant support
  - Helps with request tracing and logging
  - Used for diagnostic context in structured logging
