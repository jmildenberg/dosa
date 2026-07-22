# Template :: Modules :: Clients

> [Template](../../readme.md) :: [Modules](../readme.md) :: Clients

Consumer-facing SDK for calling this domain — the concrete implementation other domains and external
callers use instead of hand-rolling HTTP or messaging calls against [Contracts](../contracts/readme.md)
directly.

**Depends on:** [Contracts](../contracts/readme.md) only — never Domain, Application, or Infrastructure.
**Depended on by:** [hosts/web](../hosts/web/readme.md) and [hosts/mcp](../hosts/mcp/readme.md) (the
external-consumer hosts, which reach this domain only through Contracts and Clients), and by any other
domain that calls this one.

A consumer needs one client, regardless of whether the calls it wraps are synchronous or messaging —
so unlike [Contracts](../contracts/readme.md), there is no per-protocol split here. The split that
matters is by target language — a real domain adds one subfolder per language it publishes an SDK for,
e.g. `java/`, `csharp/`, `go/` — since a consumer in another domain's stack needs a client in its own
language, not this domain's.

---

References: [docs/adrs/0005-standard-domain-module-layout.md](../../../../docs/adrs/0005-standard-domain-module-layout.md)
