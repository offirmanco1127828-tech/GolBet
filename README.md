# GolBet

Proyecto académico de Diseño de Software · .NET 8 · Arquitectura N-Capas.

## Módulos incluidos

- Module 1: N-layer solution and MVC navigation.
- Module 2: Domain entities, EF Core and Code First.
- Module 3: Generic repositories and database seeder.
- Module 4: Services, DTOs, AutoMapper and match board.
- Module 5: Match detail page and board filters.

## Architecture

GolBet.Web -> GolBet.Services -> GolBet.Repositories -> GolBet.Entities

## Run

Configure the SQL Server connection string in `GolBet.Web/appsettings.json`, then run the project with .NET 8.

The course materials use SQL Server. Do not replace it with PostgreSQL/Supabase unless the instructor authorizes that change.

Do not commit passwords, API keys or other secrets.
