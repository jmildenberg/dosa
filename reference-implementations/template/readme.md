# Template

> [DOSA](../../readme.md) :: [Reference Implementations](../readme.md) :: Template

Language-agnostic scaffold for a single domain repository, conforming to
[ADR-0005 (Standard Domain Module Layout)](../../docs/adrs/0005-standard-domain-module-layout.md).
Folder names and locations are the standard; build-artifact and package names inside them stay
idiomatic to whatever stack a real domain uses.

Each area below maps to guidance in [docs/](../../docs/):

| Area | Carries forward | Question it answers |
|---|---|---|
| [modules/](modules/readme.md) | [ADR-0002](../../docs/adrs/0002-adopt-clean-architecture.md), [ADR-0005](../../docs/adrs/0005-standard-domain-module-layout.md) | Where does the code live, and which layers may depend on which |
| [docs/](docs/readme.md) | [Architecture](../../docs/architecture.md), [Decision Records](../../docs/adrs/readme.md) | Why this domain is shaped the way it is |
| [databases/](databases/readme.md) | [Engineering — Persistence](../../docs/engineering.md) | Where schema and migrations live, per data source |
| [deploy/](deploy/readme.md) | [Engineering — Delivery](../../docs/engineering.md), [ADR-0009](../../docs/adrs/0009-containerization-with-oci-images.md), [ADR-0010](../../docs/adrs/0010-adopt-kubernetes-as-deployment-target.md) | How the domain is packaged and deployed |
| [tests/](tests/readme.md) | [ADR-0007](../../docs/adrs/0007-testing-trophy-and-test-classification.md) | How the layout and cross-module behavior are verified |
| [tooling/](tooling/readme.md) | [Engineering — Delivery](../../docs/engineering.md) | Local-dev and CI-adjacent helpers, not the domain's runtime |

A real domain starts by copying this structure, then fills each area in with its own logic —
the folders stay, the emptiness doesn't.
