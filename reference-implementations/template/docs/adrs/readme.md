# Template :: Documentation :: Decision Records

> [Template](../../readme.md) :: [Documentation](../readme.md) :: Decision Records

Architecture Decision Records for choices specific to this domain — not the org-wide standard, which
lives in the root [docs/adrs/](../../../../docs/adrs/). Follow the same format as an org ADR
(see the [org ADR readme](../../../../docs/adrs/readme.md) for the template and lifecycle), scoped to a
decision this domain alone needs to make: a specific database engine, a messaging broker, an internal
module split beyond what [ADR-0005](../../../../docs/adrs/0005-standard-domain-module-layout.md) mandates.

A domain-local ADR may reference the org ADR it implements or extends. It does not override an org ADR —
a deviation from the shared standard follows the exception process in
[Governance](../../../../docs/governance.md), not a local ADR alone.
