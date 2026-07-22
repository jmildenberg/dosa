# Template :: Modules :: Hosts :: MCP

> [Template](../../../readme.md) :: [Modules](../../readme.md) :: [Hosts](../readme.md) :: MCP

External-consumer host — a Model Context Protocol server. References only
[Contracts](../../contracts/readme.md) and [Clients](../../clients/readme.md), the same
published-interface access any other domain has, so it cannot reach domain state even in principle.

Not a special case: it is just another external-consumer host, like [web](../web/readme.md). It holds no
model or agent runtime of its own — it exposes this domain's published surface as MCP tools/resources,
and the agent itself lives in whatever client connects.
