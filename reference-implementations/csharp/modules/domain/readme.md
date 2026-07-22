# C# :: Modules :: Domain

> [C#](../../readme.md) :: [Modules](../readme.md) :: Domain

The innermost layer — entities and domain rules that would still be true if every framework, database,
and transport in this repository were swapped out.

**Depends on:** nothing. No framework, persistence, or infrastructure reference belongs here — that is
the whole point of the layer. **Depended on by:** Application and Infrastructure.

Because it has no dependencies, this is the module that should be the cheapest to unit-test.

---

References: [docs/adrs/0002-adopt-clean-architecture.md](../../../../docs/adrs/0002-adopt-clean-architecture.md), [docs/adrs/0007-testing-trophy-and-test-classification.md](../../../../docs/adrs/0007-testing-trophy-and-test-classification.md)
