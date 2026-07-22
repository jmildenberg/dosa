# C# :: Modules

> [C#](../readme.md) :: Modules

All code for the domain lives here, organized so dependencies only point inward: a module never depends
on something built on top of it, only on what it's built on.

| Module | Responsibility | Depends on |
|---|---|---|
| [Domain](domain/readme.md) | Core business entities and domain rules | Nothing |
| [Application](application/readme.md) | Use cases and orchestration | Domain |
| [Common](common/readme.md) | Shared kernel — cross-cutting types and utilities, no business logic | Nothing |
| [Contracts](contracts/readme.md) | Published request, response, and event schema types this domain exposes | Nothing |
| [Clients](clients/readme.md) | Consumer-facing SDKs for calling this domain | Contracts |
| [Infrastructure](infrastructure/readme.md) | Adapters for persistence, messaging, and external services | Application, Domain |
| [Hosts](hosts/readme.md) | Deployable entry points | Varies by host role — see [Hosts](hosts/readme.md) |
| Frontend *(optional, not scaffolded here)* | Presentation role, reaches the domain only through `hosts/web` | Nothing in this domain directly |

A consuming domain never reaches into another domain's `modules/`. It depends only on that domain's
published Contracts, typically through its Clients SDK.

---

References: [docs/adrs/0002-adopt-clean-architecture.md](../../../docs/adrs/0002-adopt-clean-architecture.md), [docs/adrs/0003-adopt-domain-oriented-service-architecture.md](../../../docs/adrs/0003-adopt-domain-oriented-service-architecture.md), [docs/adrs/0005-standard-domain-module-layout.md](../../../docs/adrs/0005-standard-domain-module-layout.md)
