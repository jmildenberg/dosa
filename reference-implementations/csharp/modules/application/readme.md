# C# :: Modules :: Application

> [C#](../../readme.md) :: [Modules](../readme.md) :: Application

Use cases and orchestration. Coordinates Domain entities to satisfy a specific request, but contains no
business rule of its own and no infrastructure detail.

**Depends on:** [Domain](../domain/readme.md) and [Common](../common/readme.md). **Depended on by:** the
in-process Hosts (`hosts/api`, `hosts/worker`) and Infrastructure, which implements the ports this layer
defines.

---

References: [docs/adrs/0002-adopt-clean-architecture.md](../../../../docs/adrs/0002-adopt-clean-architecture.md)
