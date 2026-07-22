# Template :: Modules :: Contracts

> [Template](../../readme.md) :: [Modules](../readme.md) :: Contracts

Published request, response, and event schema types this domain exposes to the outside world —
generated from or paired with the OpenAPI/AsyncAPI specs in [docs/contracts/](../../docs/contracts/readme.md)
(see [ADR-0013](../../../../docs/adrs/0013-openapi-and-asyncapi-for-contract-authoring.md)). Split by
protocol:

- [api/](api/readme.md) — request/response types for synchronous calls
- [messaging/](messaging/readme.md) — event types for asynchronous messaging

**Depends on:** nothing — no [Domain](../domain/readme.md) internals leak into a contract type.
**Depended on by:** [Clients](../clients/readme.md), and by any other domain that consumes this one.
Contracts are the *only* thing another domain is allowed to depend on
(see [ADR-0003](../../../../docs/adrs/0003-adopt-domain-oriented-service-architecture.md)) — a breaking
change here is a breaking change for every consumer, not an internal refactor.
