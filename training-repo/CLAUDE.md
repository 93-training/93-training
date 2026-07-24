# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

**OrderHub** — an internal order-management web app (create/query orders, manage products & customers). It is the practice codebase for an **AI-agent coding training course**; the exercises live in `../documents/` (git root is one level up from this solution folder). Keep this in mind:

- **The code intentionally contains planted bugs and rough spots** used by the exercises (e.g. order-list pagination, Gold-tier discount math, stock restore on cancel). Treat current behavior as *possibly-a-defect*, not as spec. **Do not proactively "fix", refactor, or clean up code unless the user explicitly asks** — you may be erasing an exercise.
- It's a small single-database internal system: no multi-tenancy, no high-concurrency/microservice concerns. Match that complexity level.
- Source code, comments, error messages, and UI strings are in **Traditional Chinese (zh-TW)** — write new user-facing strings and messages the same way.

## Tech stack & key versions

- **.NET 8** / ASP.NET Core MVC with Razor Views + Bootstrap 5 (frontend assets are vendored under `wwwroot/lib`, **no CDN**)
- **EF Core 8** (`8.0.11`) + SQL Server
- Tests: **xUnit** (`2.5.3`) using EF Core **InMemory** — tests do **not** need SQL Server

## Commands

Run all commands from this folder (`training-repo/`, where `OrderHub.sln` lives).

- Build: `dotnet build`
- Run all tests: `dotnet test`
- Run one test class: `dotnet test --filter "FullyQualifiedName~OrderServicePricingTests"`
- Run one test by name: `dotnet test --filter "DisplayName~<method name>"`
- Run the web app: `dotnet run --project src/OrderHub.Web` → http://localhost:5150 (https: 7147). First run auto-applies EF migrations and seeds data (see `Program.cs`).
- Reset the database (drops + re-seeds on next run): `dotnet ef database drop -f -p src/OrderHub.Infrastructure -s src/OrderHub.Web`

Requires a local SQL Server to *run* the app (not to test). The `Default` connection string is in `appsettings.json` (LocalDB) / `appsettings.Development.json` (`Server=localhost`, Windows auth).

## Architecture — layering & conventions

Three projects, dependencies point inward: `OrderHub.Web` → `OrderHub.Core` ← `OrderHub.Infrastructure` (Core has no project references).

- **OrderHub.Core** — domain models (`Domain/`), service interfaces + business logic (`Services/`), and cross-cutting result types (`Common/`). All business rules live here.
- **OrderHub.Infrastructure** — `OrderHubDbContext`, repositories, EF migrations, `DbSeeder`. **Only repositories touch `DbContext`.**
- **OrderHub.Web** — thin controllers, ViewModels, Razor Views. Controllers only wire services to views.

Follow these when adding or changing features:

- **Keep controllers thin**: no business logic, no EF/`DbContext` access. Put logic in a Core service behind an interface, injected via constructor (all registered scoped in `Program.cs`).
- **Data access goes through a repository** — never use `DbContext` directly from a service or controller.
- **Services return `ServiceResult<T>`** (see `Common/ServiceResult.cs`) to express expected failures — do **not** throw for validation/business errors. Paged queries return `PagedResult<T>`.
- **Views bind to a ViewModel, never a domain model.** Mapping is hand-written in the controller (see `OrdersController.MapToDetails`).
- **Validate user input with DataAnnotations + `ModelState`**; on invalid input re-render the form with errors — bad input must never produce a 500.
- **Money is always `decimal`.** Discount logic is centralized in `OrderService` (`GetDiscountRate` / `CalculateTotal`) — don't recompute discounts elsewhere. Order line prices are captured as a snapshot (`OrderItem.UnitPriceSnapshot`).
- Surface operation outcomes via `TempData["Success"]` / `TempData["Error"]` (shared alert block in `Views/Shared/_Layout.cshtml`).
- Customer tiers: `Standard` / `Silver` / `Gold` (`Domain/CustomerTier.cs`); order states in `Domain/OrderStatus.cs`.
- **Reference implementations to copy the style from**: controller → `Controllers/ProductsController.cs`; service → `Services/ProductService.cs`.

### Tests

- xUnit with EF Core InMemory. Use `TestSetup` helpers (`CreateContext`, `CreateOrderService`, `AddCustomer`, `AddProduct`) — each context uses a fresh unique in-memory DB.
- Test files are named `<Service><Area>Tests.cs` (e.g. `OrderServicePricingTests`).

## Code style

Enforced by `.editorconfig`: file-scoped namespaces, Allman braces, `var` when the type is apparent, `System` usings sorted first, 4-space indent for `.cs`/`.cshtml` (2-space for json/js/css). Nullable reference types and implicit usings are enabled in all projects.

## Don'ts

- Don't fix bugs or refactor unrelated code unprompted (see "What this is" — bugs may be exercises).
- Don't add NuGet packages without asking first.
- Don't hand-edit files under `src/OrderHub.Infrastructure/Migrations/**` — they are generated history.
- Don't change `appsettings*.json` connection strings without confirming.
