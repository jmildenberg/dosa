# Template :: Modules :: Hosts :: API

> [Template](../../../readme.md) :: [Modules](../../readme.md) :: [Hosts](../readme.md) :: API

In-process operator host — the domain-logic API. References
[Domain](../../domain/readme.md), [Application](../../application/readme.md), and
[Infrastructure](../../infrastructure/readme.md) directly, and serves the
[Contracts/api](../../contracts/api/readme.md) surface over HTTP.

Framework and transport wiring (routing, DI container, middleware) lives here — this is the
"Frameworks and Drivers" layer of [Clean Architecture](../../../../../docs/adrs/0002-adopt-clean-architecture.md),
the outermost ring, allowed to know about everything inward but exposing only what
[Contracts/api](../../contracts/api/readme.md) declares.
