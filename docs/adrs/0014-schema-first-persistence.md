# 0014. Schema-First Persistence

Date: 2026-06-22

## Status

Accepted

## Context

[ADR-0013](0013-openapi-and-asyncapi-for-contract-authoring.md) established contract-first authoring for a domain's external interfaces. A domain's persistence schema is an internal contract subject to the same forces: it defines the structure of the data the domain owns and is the source of truth for it, and changes to it must be deliberate and reviewable.

Code-first and auto-generated schema approaches invert this discipline — the schema becomes a byproduct of the code rather than an authored artifact. That makes schema changes implicit, hard to review independently, and dependent on the ORM or framework that generated them, weakening the domain's authority over its own data as established by Domain-Oriented Service Architecture.

How schema changes are *rolled out* relative to code is a further decision. Applying a schema change in place, coupled to the code release that needs it, removes any independent checkpoint: if that code must be rolled back, the schema has already changed underneath it. A rollout discipline is needed that keeps schema and code independently reversible.

## Decision

We will treat a domain's persistence schema as an authored contract. The schema is defined first as an explicit schema definition (DDL for relational stores) and application code conforms to it, rather than the schema being generated from code.

- Schema changes are managed through a schema migration tool as deliberate, version-controlled scripts, reviewed like any other contract change
- The **Database** area ([ADR-0005](0005-standard-domain-module-layout.md)) owns the schema and its migration history; the owning domain is the sole authority over it
- Code-first and auto-generated schema approaches are not used

Schema changes follow an **expand-contract** discipline and deploy **ahead of** the code that depends on them:

- **Expand → migrate → contract** — add the new shape alongside the old, move code onto it, then remove the old shape in a separate later change. Each step is additive and backward-compatible at deploy time, so old and new shapes both work against whatever code is live when the migration runs.
- **Schema ahead of code** — a release's migration is applied while the previous code version is still running; only once it succeeds does the new code roll out. Because the migration is additive, the previous code keeps working, so rollback is always a code-artifact rollback, never a schema rollback.
- **Breaking-change gate** — a required check blocks a migration containing a destructive change (a dropped column, a narrowed type, a non-nullable column added without a default) unless it is a deliberate, approved contract step, mirroring the contract breaking-change detection in [ADR-0013](0013-openapi-and-asyncapi-for-contract-authoring.md).

## Consequences

- The schema is an authored, reviewable artifact, so changes to a domain's data structure are deliberate rather than an implicit side effect of code
- Migrations form a version-controlled history of how the schema evolved, auditable like any other contract change
- Persistence is decoupled from any single ORM or framework's generation behavior, keeping the schema portable and authoritative
- The discipline mirrors contract-first authoring end to end — external interfaces and internal data are governed the same way
- Teams accustomed to ORM-driven, code-first workflows must change practice and maintain migrations by hand
- The decision is framed for relational stores; domains using non-relational persistence apply the same authored-schema intent through whatever schema mechanism their store provides
- Expand-contract keeps schema and code independently reversible: because every migration is additive and backward-compatible at deploy time, rollback is always a code-only operation and never requires reversing a migration against live data, and schema-ahead-of-code gives an explicit checkpoint validated against the running previous version before new code ships
- The discipline costs more steps per change — a breaking change becomes at least two releases (expand, then contract) — and depends on the migration gate being enforced, trading immediate convenience for safe rollback

## Reversibility

One-way door (costly). Once data lives in production under authored, migration-tracked schemas, the migration history becomes part of each domain's persistence record. Reverting to a code-first or ORM-generated model would mean reconciling the existing schema with generated definitions and unwinding the migration history, which is risky against live data even though it is contained within each domain rather than system-wide.
