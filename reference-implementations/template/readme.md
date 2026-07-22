# Template

> [DOSA](../../readme.md) :: [Reference Implementations](../readme.md) :: Template

Language-agnostic scaffold for a single domain repository. Folder names and locations are the standard;
build-artifact and package names inside them stay idiomatic to whatever stack a real domain uses.

| Area | What it holds |
|---|---|
| [modules/](modules/readme.md) | The domain's code, layered so dependencies only point inward |
| [docs/](docs/readme.md) | This domain's own documentation and decisions |
| [databases/](databases/readme.md) | Schema and migrations, one directory per data source |
| [deploy/](deploy/readme.md) | How the domain is packaged and deployed |
| [tests/](tests/readme.md) | Cross-module and end-to-end verification |
| [tooling/](tooling/readme.md) | Local-dev and CI-adjacent helpers, not the domain's runtime |

A real domain starts by copying this structure, then fills each area in with its own logic — the
folders stay, the emptiness doesn't.

---

References: [docs/adrs/0002-adopt-clean-architecture.md](../../docs/adrs/0002-adopt-clean-architecture.md), [docs/adrs/0005-standard-domain-module-layout.md](../../docs/adrs/0005-standard-domain-module-layout.md)
