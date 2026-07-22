# 0017. API Versioning via URL Path

Date: 2026-06-24

## Status

Accepted

## Context

[ADR-0013](0013-openapi-and-asyncapi-for-contract-authoring.md) and the [engineering
document](../engineering.md#9-api-and-contract-design) require contracts to be explicitly versioned
and breaking changes to be deliberate, but deliberately left *how a version is expressed to callers*
undecided — "under evaluation … established as an Architecture Decision Record when decided." The
[Contract Compatibility](../architecture.md#5-contract-compatibility) principle requires that
versioning be explicit, breaking changes be deliberate, and consumers be given a migration path.

A single, consistent mechanism is needed across domains. The candidates were a version in the **URL
path**, in a **header**, or in a **media type**. The deciding forces: the mechanism must be explicit
and obvious to consumers, easy to route and cache at the edge, straightforward to test and document,
and able to serve two major versions **concurrently** during a deprecation window.

## Decision

We will express **API major versions in the URL path** — for example `/v1/...`, `/v2/...`.

- Only the **major** version appears in the path. Minor and patch changes are backward-compatible
  ([ADR-0013](0013-openapi-and-asyncapi-for-contract-authoring.md) versioning rules) and are
  transparent to callers — no URL change.
- A breaking change is a new major path; the previous major is retained during the deprecation
  window and then removed.
- This applies to **synchronous HTTP APIs**. Asynchronous events (AsyncAPI) are not URL-addressed —
  they version through the message/schema, where a breaking change is a new major message version.

## Consequences

- Versioning is explicit and visible in every request and log line; consumers pin a major by URL.
- Two majors can run **side by side** during migration, satisfying the deprecation lifecycle, and
  the path routes/caches cleanly at gateways and CDNs.
- Minor/patch evolution requires no consumer change, keeping additive change cheap.
- The path carries only the major, so finer-grained negotiation (header/media-type) is not available
  — an accepted trade for simplicity and routability.
- A major bump is a visible, deliberate event (new path + deprecation of the old), reinforcing the
  Contract Compatibility expectations rather than allowing silent breakage.

## Reversibility

Two-way door (low cost). The versioning mechanism is a routing and convention choice; moving to header- or media-type-based versioning would change gateway routing and consumer expectations but touches no domain logic. Because two majors already run side by side during deprecation, an alternative scheme can be introduced alongside the path scheme and migrated to gradually.
