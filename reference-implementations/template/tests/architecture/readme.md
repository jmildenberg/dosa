# Template :: Tests :: Architecture

> [Template](../../readme.md) :: [Tests](../readme.md) :: Architecture

Fitness functions that make the [Clean Architecture](../../../../docs/adrs/0002-adopt-clean-architecture.md)
dependency rule enforceable rather than conventional: [Domain](../../modules/domain/readme.md) must not
reference [Infrastructure](../../modules/infrastructure/readme.md), [hosts/web](../../modules/hosts/web/readme.md)
and [hosts/mcp](../../modules/hosts/mcp/readme.md) must not reference anything but
[Contracts](../../modules/contracts/readme.md)/[Clients](../../modules/clients/readme.md), and so on for
every dependency rule in [ADR-0005](../../../../docs/adrs/0005-standard-domain-module-layout.md). A
layer violation should fail here, in CI, not surface later as a design smell.
