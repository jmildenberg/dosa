# Template :: Modules :: Infrastructure

> [Template](../../readme.md) :: [Modules](../readme.md) :: Infrastructure

Interface Adapters in [Clean Architecture](../../../../docs/adrs/0002-adopt-clean-architecture.md)
terms — persistence, messaging, and external-service adapters that implement the ports
[Application](../application/readme.md) defines.

**Depends on:** [Application](../application/readme.md) and [Domain](../domain/readme.md).
**Depended on by:** the in-process [Hosts](../hosts/readme.md) (`hosts/api`, `hosts/worker`), which wire
this module in at startup.

May be extended into focused subfolders as needed (e.g. `persistence/`, `messaging/`) the same way
[Contracts](../contracts/readme.md) splits by protocol — one subfolder per concern, no subfolder that
leaks a Domain internal past this layer.
