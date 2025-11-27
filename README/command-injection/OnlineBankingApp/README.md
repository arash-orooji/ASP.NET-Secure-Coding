# OnlineBankingApp (Blazor Server, .NET 10 preview)

This sample upgrades the original Razor Pages project to a Blazor Server application targeting the upcoming .NET 10 runtime. Legacy Razor Pages remain available under `/legacy/...` routes for reference, while the primary experience is delivered through interactive Blazor components.

## Project layout
- `Components/` – shared imports, layouts, and the new Blazor pages for dashboard, customers, fund transfers, and backups.
- `Pages/_Host.cshtml` – host page that bootstraps the Blazor Server circuit.
- `Pages/legacy` – existing Razor Pages retained under `/legacy` routes for historical comparison.
- `Data/`, `Models/`, `Services/` – Entity Framework Core context, domain models, and reusable services.

## Prerequisites
- .NET 10 preview SDK (or the latest available preview build). Earlier SDKs will fail to restore packages targeting `net10.0`.
- SQLite tooling when using the development configuration (default connection string).

## Running the app
```pwsh
# From the OnlineBankingApp directory
pwsh -NoLogo -NoProfile -Command "dotnet run"
```
Navigate to `https://localhost:5001` (or the port shown in the console). The main navigation exposes the new Blazor experience, while legacy Razor Pages can be reached beneath `/legacy`.

## Tests
The sample project does not include automated tests. You can trigger a build check via:
```pwsh
pwsh -NoLogo -NoProfile -Command "dotnet build"
```

## Notes
- Because `net10.0` is pre-release, several transitive authentication packages currently emit vulnerability advisories (`NU1901/NU1902`). Monitor the package ecosystem and update once patched versions become available.
- Entity Framework Core is configured with both context injection and a context factory to support Blazor DI patterns. Development builds use SQLite; production defaults to SQL Server.
