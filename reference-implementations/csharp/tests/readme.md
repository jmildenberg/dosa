# C# :: Tests

> [C#](../readme.md) :: Tests

Repo-wide tests, plus — for .NET, where the build tooling dictates a physically separate test project
rather than a co-located folder like JVM's `src/test` — the per-module unit test projects themselves,
mirrored here against [modules/](../modules/readme.md).

| Area | What it covers |
|---|---|
| [architecture/](architecture/readme.md) | Enforces the module layout's dependency rules |
| [system/](system/readme.md) | Exercises the domain end-to-end, across modules |
| [support/](support/readme.md) | Shared test fixtures/builders with no single module to belong to |
| [domain/](domain/readme.md) | Tests for [modules/domain](../modules/domain/readme.md) |
| [application/](application/readme.md) | Tests for [modules/application](../modules/application/readme.md) |
| [common/](common/readme.md) | Tests for [modules/common](../modules/common/readme.md) |
| [contracts/](contracts/readme.md) | Tests for [modules/contracts](../modules/contracts/readme.md) |
| [clients/](clients/readme.md) | Tests for [modules/clients](../modules/clients/readme.md) |
| [infrastructure/](infrastructure/readme.md) | Tests for [modules/infrastructure](../modules/infrastructure/readme.md) |
| [hosts/](hosts/readme.md) | Tests for [modules/hosts](../modules/hosts/readme.md) |

---

References: [docs/adrs/0005-standard-domain-module-layout.md](../../../docs/adrs/0005-standard-domain-module-layout.md), [docs/adrs/0007-testing-trophy-and-test-classification.md](../../../docs/adrs/0007-testing-trophy-and-test-classification.md)
