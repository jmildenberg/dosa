# C# :: Tests

> [C#](../readme.md) :: Tests

Repo-wide tests: per-module unit tests live inside their own module, following whatever placement the
build tooling dictates (`src/test` for JVM, a separate test project for .NET). What lives here is what
has no single module to belong to.

| Area | What it covers |
|---|---|
| [architecture/](architecture/readme.md) | Enforces the module layout's dependency rules |
| [system/](system/readme.md) | Exercises the domain end-to-end, across modules |

---

References: [docs/adrs/0005-standard-domain-module-layout.md](../../../docs/adrs/0005-standard-domain-module-layout.md), [docs/adrs/0007-testing-trophy-and-test-classification.md](../../../docs/adrs/0007-testing-trophy-and-test-classification.md)
