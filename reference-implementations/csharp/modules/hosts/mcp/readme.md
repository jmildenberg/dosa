# C# :: Modules :: Hosts :: MCP

> [C#](../../../readme.md) :: [Modules](../../readme.md) :: [Hosts](../readme.md) :: MCP

External-consumer host — a Model Context Protocol server. Like [web](../web/readme.md), it's bound to
[Contracts](../../contracts/readme.md) and [Clients](../../clients/readme.md) only (see [Hosts](../readme.md)
for why external-consumer hosts are structurally boxed out of domain state).

Not a special case beyond that boundary: it holds no model or agent runtime of its own — it exposes this
domain's published surface as MCP tools/resources, and the agent itself lives in whatever client
connects.

---

References: [docs/adrs/0005-standard-domain-module-layout.md](../../../../../docs/adrs/0005-standard-domain-module-layout.md)
