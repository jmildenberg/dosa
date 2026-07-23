# C# :: Modules :: Contracts

> [C#](../../readme.md) :: [Modules](../readme.md) :: Contracts

Published request, response, and event schema types this domain exposes to the outside world, kept in
lockstep with the interface specs authored in `docs/contracts/`. Split by protocol:

- [api/](api/readme.md) — request/response types for synchronous calls
- [messaging/](messaging/readme.md) — event types for asynchronous messaging

**Depends on:** nothing — no Domain internal leaks into a contract type. **Depended on by:** Clients
(the `api/` side), [Infrastructure](../infrastructure/readme.md)'s messaging adapters (the `messaging/`
side), and by any other domain that consumes this one. Contracts are the *only* thing another domain is
allowed to depend on — a breaking change here is a breaking change for every consumer, not an internal
refactor.

---

References: [docs/adrs/0003-adopt-domain-oriented-service-architecture.md](../../../../docs/adrs/0003-adopt-domain-oriented-service-architecture.md), [docs/adrs/0013-openapi-and-asyncapi-for-contract-authoring.md](../../../../docs/adrs/0013-openapi-and-asyncapi-for-contract-authoring.md)
