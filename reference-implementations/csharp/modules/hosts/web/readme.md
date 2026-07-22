# C# :: Modules :: Hosts :: Web

> [C#](../../../readme.md) :: [Modules](../../readme.md) :: [Hosts](../readme.md) :: Web

External-consumer host — request-shaping / backend-for-frontend. References only
[Contracts](../../contracts/readme.md) and [Clients](../../clients/readme.md), the same
published-interface access any other domain has, so it cannot reach domain state even in principle.

Holds presentation-facing concerns: request shaping, response aggregation across calls, and anything an
optional `frontend` module (not scaffolded here) needs that isn't a 1:1 pass-through of the published
API. It has no logic of its own beyond that shaping.

---

References: [docs/adrs/0005-standard-domain-module-layout.md](../../../../../docs/adrs/0005-standard-domain-module-layout.md)
