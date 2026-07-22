# C# :: Modules :: Hosts

> [C#](../../readme.md) :: [Modules](../readme.md) :: Hosts

Deployable entry points for this domain. A domain contains only the hosts it needs — not all five are
mandatory — divided by whether the role operates on domain state:

| Host | Role | References |
|---|---|---|
| [api](api/readme.md) | In-process operator — domain-logic API | Domain, Application, Infrastructure |
| [worker](worker/readme.md) | In-process operator — asynchronous processing | Domain, Application, Infrastructure |
| [web](web/readme.md) | External consumer — request-shaping / backend-for-frontend | Contracts, Clients |
| [mcp](mcp/readme.md) | External consumer — Model Context Protocol server | Contracts, Clients |
| [shared](shared/readme.md) | Cross-host wiring shared by two or more hosts above | Varies |

The operator/consumer split is the point: `web` and `mcp` reach this domain only through Contracts and
Clients — the same published-interface access any other domain has — so they cannot reach domain state
even in principle.

---

References: [docs/adrs/0005-standard-domain-module-layout.md](../../../../docs/adrs/0005-standard-domain-module-layout.md)
