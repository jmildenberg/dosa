# Template :: Modules

> [Template](../readme.md) :: Modules

All code for the domain lives here, organized so the [Clean Architecture](../../../docs/adrs/0002-adopt-clean-architecture.md)
dependency rule — dependencies point inward — is structural, not just convention.

| Module | Responsibility | Depends on |
|---|---|---|
| [Domain](domain/readme.md) | Core business entities and domain rules | Nothing |
| [Application](application/readme.md) | Use cases and orchestration | Domain |
| [Common](common/readme.md) | Shared kernel — cross-cutting types and utilities, no business logic | Nothing |
| [Contracts](contracts/readme.md) | Published request, response, and event schema types this domain exposes | Nothing |
| [Clients](clients/readme.md) | Consumer-facing SDKs for calling this domain | Contracts |
| [Infrastructure](infrastructure/readme.md) | Adapters for persistence, messaging, and external services | Application, Domain |
| [Hosts](hosts/readme.md) | Deployable entry points | Varies by host role — see [Hosts](hosts/readme.md) |

A consuming domain never reaches into another domain's `modules/`. It depends only on that domain's
published [Contracts](contracts/readme.md), typically through its [Clients](clients/readme.md) SDK —
see [ADR-0003 (Domain-Oriented Service Architecture)](../../../docs/adrs/0003-adopt-domain-oriented-service-architecture.md).
The full rationale for this layout lives in
[ADR-0005 (Standard Domain Module Layout)](../../../docs/adrs/0005-standard-domain-module-layout.md).
