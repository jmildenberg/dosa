# C# :: Tests :: Architecture

> [C#](../../readme.md) :: [Tests](../readme.md) :: Architecture

Fitness functions that make the dependency rule enforceable rather than conventional:
[Domain](../../modules/domain/readme.md) must not reference [Infrastructure](../../modules/infrastructure/readme.md),
[hosts/web](../../modules/hosts/web/readme.md) and [hosts/mcp](../../modules/hosts/mcp/readme.md) must
not reference anything but [Contracts](../../modules/contracts/readme.md)/[Clients](../../modules/clients/readme.md),
and so on for every dependency rule the module layout defines. A layer violation should fail here, in
CI, not surface later as a design smell.

---

References: [docs/adrs/0002-adopt-clean-architecture.md](../../../../docs/adrs/0002-adopt-clean-architecture.md), [docs/adrs/0005-standard-domain-module-layout.md](../../../../docs/adrs/0005-standard-domain-module-layout.md)
