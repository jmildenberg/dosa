# Template :: Modules :: Domain

> [Template](../../readme.md) :: [Modules](../readme.md) :: Domain

Enterprise Business Rules in [Clean Architecture](../../../../docs/adrs/0002-adopt-clean-architecture.md)
terms — the innermost layer. Entities and domain rules that would still be true if every framework,
database, and transport in this repository were swapped out.

**Depends on:** nothing. No framework, persistence, or infrastructure reference belongs here — that is
the whole point of the layer. **Depended on by:** [Application](../application/readme.md) and
[Infrastructure](../infrastructure/readme.md).

Because it has no dependencies, this is the module that should be the cheapest to unit-test — see the
[Testing Trophy](../../../../docs/adrs/0007-testing-trophy-and-test-classification.md).
