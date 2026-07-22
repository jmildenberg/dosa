# Template :: Modules :: Application

> [Template](../../readme.md) :: [Modules](../readme.md) :: Application

Application Business Rules in [Clean Architecture](../../../../docs/adrs/0002-adopt-clean-architecture.md)
terms — use cases and orchestration. Coordinates [Domain](../domain/readme.md) entities to satisfy a
specific request, but contains no business rule of its own and no infrastructure detail.

**Depends on:** [Domain](../domain/readme.md) only. **Depended on by:** the in-process
[Hosts](../hosts/readme.md) (`hosts/api`, `hosts/worker`) and [Infrastructure](../infrastructure/readme.md),
which implements the ports this layer defines.
