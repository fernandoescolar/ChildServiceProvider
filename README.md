# ChildServiceProvider

A small, high‑performance extension for `Microsoft.Extensions.DependencyInjection` that lets you create **child service providers** on top of an existing `ServiceProvider`, overriding or extending registrations while still sharing the same dependency graph.

- **Zero API magic**: built on top of `IServiceCollection`, `ServiceProvider`, and scopes you already know.
- **Child‑first resolution**: child registrations override parent ones (including open generics and keyed services).
- **Scope‑aware**: scoped services and lifetimes behave exactly as with the default DI container.
- **Focused**: no configuration system, no modules, just child containers.

> [!NOTE]
> This library targets `net8.0`, `net9.0` and `net10.0` and is designed to be a thin layer over the built‑in DI container.

## Getting started

### Installation

Add the package from NuGet (package name pending):

```bash
dotnet add package ChildServiceProvider
```

### Basic usage

Start with a regular parent `ServiceCollection` and build a standard `ServiceProvider`. Then create a `ChildServiceCollection` that inherits those registrations and build a `ChildServiceProvider` from it.

```csharp
using Microsoft.Extensions.DependencyInjection;

var parentServices = new ServiceCollection();
parentServices.AddSingleton<IMyService, ParentService>();

var parentProvider = parentServices.BuildServiceProvider();

// Create child collection based on the parent registrations
var childServices = parentServices.CreateChildServiceCollection();

// Override or add services in the child
childServices.AddSingleton<IMyService, ChildService>();
childServices.AddSingleton<IOtherService, OtherService>();

// Build the child provider
var childProvider = childServices.BuildChildServiceProvider(parentProvider);

// Resolves ChildService from the child provider
var service = childProvider.GetRequiredService<IMyService>();

// Still resolves ParentService from the parent provider
var parentService = parentProvider.GetRequiredService<IMyService>();
```

Key points:
- Parent registrations are visible from the child unless overridden.
- Child registrations never leak back into the parent provider.
- Transient, scoped and singleton lifetimes behave the same as in the default container.

## Concepts

### IChildServiceCollection

`IChildServiceCollection` is a view over two collections:

- `ParentServices`: the original parent `IServiceCollection`.
- `ChildServices`: the additional/overriding registrations for the child.

You usually create it from an existing `IServiceCollection`:

```csharp
IServiceCollection parentServices = new ServiceCollection();
// ... configure parent ...

IChildServiceCollection childServices = parentServices.CreateChildServiceCollection();
```

### ChildServiceProvider

`ChildServiceProvider` is an `IServiceProvider`, `IKeyedServiceProvider`, and `IServiceScopeFactory` that delegates to:

- the **parent provider** for services not overridden in the child; and
- the **inner provider** built from the child collection for overridden registrations.

Resolution rules:

- **Closed types**: if a service type is registered in the child, it wins over the parent.
- **Open generics**: if the child registers an open generic, it overrides parent closed and open generics for that service type.
- **Keyed services**: keyed registrations are treated independently per key; child keyed registrations override parent keyed ones with the same key.

### Scopes

`ChildServiceProvider` fully supports scoping:

- `CreateScope()` creates a new scope on both the parent and inner providers.
- Scoped services resolved from the child behave as in the default container.
- Cross‑resolution works: a service from the parent can depend on a service registered only in the child and vice versa.

Example:

```csharp
using var scope = childProvider.CreateScope();
var scoped = scope.ServiceProvider.GetRequiredService<IScopedService>();
```

## Keyed services

On .NET 8+ with the new keyed services APIs, `ChildServiceProvider` implements `IKeyedServiceProvider` and understands keys during resolution.

```csharp
var services = new ServiceCollection();
services.AddKeyedSingleton<IMyService>("parent", (sp, _) => new ParentService());

var parentProvider = services.BuildServiceProvider();
var child = services.CreateChildServiceCollection();
child.AddKeyedSingleton<IMyService>("parent", (sp, _) => new ChildService());

var childProvider = child.BuildChildServiceProvider(parentProvider);

var keyed = ((IKeyedServiceProvider)childProvider)
    .GetRequiredKeyedService(typeof(IMyService), "parent");
```

In this example, the child keyed registration for key `"parent"` overrides the parent one.

## Benchmarks

This repository includes a small BenchmarkDotNet project under `benchmarks/` comparing:

- default `ServiceProvider` resolution,
- `ChildServiceProvider` with all registrations in the parent,
- `ChildServiceProvider` with overrides in the child,
- mixed scenarios where dependencies live in parent/child.

To run the benchmarks:

```bash
cd benchmarks
dotnet run -c Release
```

> [!TIP]
> Benchmark results depend on your CPU and .NET version. Run them locally to compare behaviors for your own workloads.

## Development

### Requirements

- .NET SDK 8.0 or later (project targets `net8.0`, `net9.0`, `net10.0`).

### Build and test

From the repository root:

```bash
dotnet build

dotnet test
```

### Project structure

- `src/` – main library (`ChildServiceProvider`, `ChildServiceCollection`, resolution helpers).
- `tests/` – xUnit test suite covering resolution, scoping, disposal and generics.
- `benchmarks/` – BenchmarkDotNet project to measure performance.

## Usage notes

- Use a `ChildServiceProvider` when you need **per‑tenant or per‑feature overrides** without duplicating the entire registration graph.
- Avoid deep hierarchies of child providers; a small number of layers keeps reasoning about lifetimes simpler.
- When debugging resolution, it can help to check whether a service is present in the child or only in the parent collection.
