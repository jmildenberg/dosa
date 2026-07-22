# 0018. Adopt a Service Catalog

Date: 2026-06-26

## Status

Accepted

## Context

[ADR-0004](0004-one-repository-per-domain.md) gives each domain its own repository, which makes domain boundaries physical but scatters contracts across the estate by design — each domain's published interfaces live in its own Contracts module ([ADR-0005](0005-standard-domain-module-layout.md)), with no aggregating index of what exists. As the number of domains grows, an engineer or architect has no single place to discover which domains, services, and contracts are already available before building something new.

The Contract Compatibility discipline assumes consumers can find and reuse published contracts, but the guidance never says *how* they are found. Without a central view, reuse depends on word of mouth and tribal knowledge, and the polyrepo model's deliberate scattering of contracts becomes a discoverability gap rather than only a boundary mechanism.

## Decision

We will adopt a developer portal / service catalog that catalogs the system's domains, services, APIs and contracts, ownership, version, and status, so that engineers and architects can discover what already exists before they build.

- The catalog is **populated by each domain's pipeline** on release: a publish-to-catalog step alongside the publish-image step submits the domain's authored contracts and catalog metadata. The single source of truth stays in the domain repository — the catalog copy is **generated** from authoritative artifacts, so it cannot drift from what the domain actually publishes.
- Ownership and status are sourced from the data the governance model already maintains (`docs/ownership.md` and `CODEOWNERS`).
- The catalog is **human-facing**: it lets a person discover and judge the fit of a published interface. A contract appearing in the catalog with the expected shape indicates it exists and is published — it does not certify that it suits a given problem; that judgment remains with the engineer.
- The guidance is tool-agnostic. [Backstage](https://backstage.io/) is named as a representative example of such a portal; it is recommended as a reference, not mandated.

## Consequences

- Domains and their published contracts become discoverable from one place, so reuse is a deliberate choice based on what exists rather than on tribal knowledge
- Generating the catalog from each pipeline keeps it in step with reality — the authoritative artifact stays in the domain repo, and the catalog cannot present a stale or hand-maintained view
- Discoverability completes the Contract Compatibility story: contracts are not only compatible and versioned but findable
- Ownership and status surface alongside each entry, so a consumer knows who owns an interface and whether it is current before depending on it
- The catalog is a new cross-cutting capability that must be operated, and every domain's pipeline gains a publish-to-catalog step it must implement and keep working
- A published interface signals existence, not suitability; treating catalog presence as certification of fit would be a misuse of a human-facing discovery tool

## Reversibility

Two-way door (moderate). The catalog is a generated, read-side view assembled from authoritative artifacts that continue to live in the domain repositories; the source of truth is never in the catalog itself. A portal can be swapped for another tool, or rebuilt from scratch, by re-running the domains' publish steps against a new target — the cost is re-pointing the pipeline publish step and standing up the replacement, not migrating any system of record.
