# 0020. Entity Identity: Relational Surrogate and Public Identifier

Date: 2026-07-09

## Status

Accepted

## Context

Every persisted entity needs an identity, but that identity is referenced from three places with conflicting needs:

- **Inside the database** — as the primary key and the target of foreign keys and joins. This wants a key that is compact, sequential, and cheap to index and join.
- **Across the domain's boundaries** — in published API and event contracts ([ADR-0013](0013-openapi-and-asyncapi-for-contract-authoring.md)), and to other systems and business units. This wants an identifier that is stable, globally unique, non-guessable, and independent of any one database instance — and that leaks neither row counts nor insertion order.
- **Between aggregates within the domain** — where Domain-Oriented Service Architecture ([ADR-0003](0003-adopt-domain-oriented-service-architecture.md)) treats each aggregate as its own consistency boundary and expects references to cross that boundary by identity.

A single identifier cannot serve all three well. A natural/business key can change and carries meaning that shouldn't be load-bearing. A single random UUID used as the primary key sacrifices index and foreign-key locality. A raw sequential database key exposed to the outside leaks volume and ordering, is not globally unique, and is not stable across environments — a restore, reseed, replica, or reshard changes it, breaking any external reference bound to it.

There is a further, cross-cutting choice once two identifiers are in play: whether the relational surrogate is **visible** in the model or **hidden** as a pure persistence detail. Some object-relational mappers can hide the key entirely (a "shadow" key the model never names). That keeps the model minimal, but it depends on an ORM-specific capability, is not available in every language the standard targets, and obscures how joins actually work.

This decision applies the schema-as-authored-contract stance of [ADR-0014](0014-schema-first-persistence.md) to the specific question of entity identity, so every domain and every language reference treats it the same way.

## Decision

Every persisted entity carries **two identifiers**, with distinct and non-overlapping roles.

- **Public identifier** — a UUID, preferably time-ordered (e.g. UUIDv7), **assigned by the domain at creation**. It is the entity's identity everywhere above persistence: on API and event contracts, in cross-aggregate references, and to every external consumer. It is stable for the life of the entity, globally unique, environment-independent, and free of business meaning or ordering.
- **Relational surrogate** — a compact sequential key (e.g. a database-generated `bigint` identity), **assigned by the database on insert**. It is the primary key and the sole target of foreign keys and joins within the domain's database. It **never leaves the database**: never on a contract, never on an event, never as a cross-aggregate or cross-system reference.

Supporting rules:

- **References across aggregates and systems use the public identifier**, never the surrogate — consistent with aggregates as consistency boundaries and with the domain owning its data.
- **The surrogate is modelled explicitly, not hidden.** It is a first-class part of the persistence model and, where the implementation language allows, of the entity itself — rather than relying on an ORM's hidden-key feature. This keeps the mapping uniform across the standard's language references and keeps the surrogate's join-only role legible.
- **Identity and equality are always taken from the public identifier.** Because the surrogate is database-assigned, it is absent on a not-yet-persisted entity; nothing may treat it as identity.
- **A published contract must never expose the surrogate.** This is enforceable (a fitness check in the reference implementations fails if a contract type carries the surrogate's type).

The **Database** area ([ADR-0005](0005-standard-domain-module-layout.md)) authors both columns as part of the schema it owns; application code conforms to them ([ADR-0014](0014-schema-first-persistence.md)).

## Consequences

- Storage keeps a compact, sequential primary key, so indexes and foreign-key joins stay locality-friendly regardless of the public identifier's format.
- The public identity is safe to expose — it reveals neither volume nor ordering — and is stable and globally unique, so external references never depend on a database-local value.
- Because the domain assigns the public identifier up front rather than the database, an entity has a valid identity before it is persisted: identifiers can be minted client-side, enabling offline creation and idempotent, retry-safe writes.
- References held by other aggregates, other systems, and published events survive database operations that change surrogates (restore, reseed, replica promotion, resharding), because those references use the public identifier.
- Two identifiers per entity is more to hold in mind. The rule — *the public identifier is the identity; the surrogate is joins-only and never leaves the database* — must be taught and guarded, or the surrogate can leak into a contract or be mistaken for identity.
- Modelling the surrogate explicitly rather than hiding it costs a little purity — a persistence-shaped field appears on the model and is unset until insert — but buys a single, explainable mapping that works in every language the standard targets, including those whose ORMs cannot hide a key. A hidden-key approach would make the pattern non-portable across the reference set.
- Equality and identity semantics must be defined on the public identifier, not the surrogate.
- A time-ordered UUID keeps the public identifier's own unique index locality-friendly, narrowing the historical performance gap that once motivated surrogate-only designs.

## Reversibility

One-way door (costly), for the same reason a contract change is: once the public identifier is published on API and event contracts, external consumers and other domains bind to it, so changing *what the identity is* is a breaking contract change coordinated across consumers. Adopting the pattern, by contrast, is cheap — both columns are authored in the schema from the start — and the internal surrogate can be reshaped freely because nothing outside the database references it. The commitment that is hard to reverse is the public identifier's stability, which is exactly the property it exists to provide.
