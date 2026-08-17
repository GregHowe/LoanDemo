# Niuro Loan Application

Demo video: <a href="https://drive.google.com/file/d/1u3JkO0Vt2QpmRkOaTGBcsMFycxmynnLc/view?usp=sharing" target="_blank" rel="noopener noreferrer">Watch the walkthrough</a>

This repository contains a full-stack loan application flow built with .NET and Next.js. It includes a backend rule engine, transactional persistence, asynchronous background event processing, and a mock external customer service used to simulate downstream integration.

## Project structure

- `Backend/LoanApp` — ASP.NET Core API for handling loan submissions
- `Backend/MockExternalService` — mock downstream service for customer persistence
- `Backend/LoanApp.Tests` — xUnit tests covering rules, service logic, and controller behavior
- `FrontEnd` — Next.js client app containing the web application files

## Prerequisites

- .NET 8 SDK
- SQL Server instance available locally
- Database named `LoanAppDb` (or create it before running migrations)

The backend is configured for SQL Server using the connection string in `Backend/LoanApp/appsettings.json`.

## Run the backend

```powershell
cd Backend/LoanApp
dotnet run
```

Swagger is available at:

- `https://localhost:5001/swagger`
- or the port configured in `launchSettings.json`

## Run the mock external service

```powershell
cd Backend/MockExternalService
dotnet run --launch-profile http
```

The mock service exposes the customer endpoints used by the background publisher.

## Test scenarios

### Approved application

```json
{
  "firstName": "Juan",
  "lastName": "Perez",
  "address": "Av. Siempre Viva 123",
  "state": "CA",
  "companyName": "TechCorp",
  "requestedAmount": 5000,
  "ssn": "123456789"
}
```

Expected result:

- approved = true
- customer and application saved to the database
- external mock receives a POST for a new customer

### Denied by state

```json
{
  "firstName": "John",
  "lastName": "Smith",
  "address": "123 Main St",
  "state": "NY",
  "companyName": "AcmeCorp",
  "requestedAmount": 10000,
  "ssn": "987654321"
}
```

Expected result:

- approved = false
- reason = "Applicants from NY are not allowed."
- no database write
- no external mock call

### Denied by blacklisted SSN

```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "address": "456 Oak Ave",
  "state": "TX",
  "companyName": "SecretCorp",
  "requestedAmount": 7500,
  "ssn": "666123456"
}
```

Expected result:

- approved = false
- reason = "SSN is blacklisted."
- no database write
- no external mock call

### Returning customer

Use the same SSN twice with different details. The second request should update the existing customer and application instead of creating a duplicate.

## Run the tests

```powershell
cd Backend
dotnet test LoanApp.Tests/LoanApp.Tests.csproj --nologo
```

Current verification status:

- 8 tests passed
- 0 failed

## Notes

- The rule engine is implemented as a list of `IRule` instances in the service.
- Data writes are wrapped in a database transaction when the provider supports it.
- The external call is processed asynchronously through an in-memory channel and a background service.
- Returning customers are checked via the mock service before deciding between POST and PUT.
