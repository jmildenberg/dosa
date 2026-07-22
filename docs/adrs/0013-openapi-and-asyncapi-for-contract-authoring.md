# 0013. OpenAPI and AsyncAPI for Contract Authoring

Date: 2026-06-22

## Status

Accepted

## Context

The Contract Compatibility principle requires that published interfaces be authored as the source of truth and that code be verified to conform to them — not the reverse. Realizing this needs concrete specification formats for both interface kinds the architecture recognizes: synchronous request/response APIs and asynchronous events. Without standard formats, contracts are described inconsistently or generated as a byproduct of code, which inverts the contract-first discipline and makes compatibility impossible to verify automatically.

Domains are built in different technologies, so the formats must be language-agnostic and supported by tooling for validation and breaking-change detection.

A published API contract also defines its error responses. Without a standard error representation, each domain invents its own error body, and a consumer calling several domains must learn a different error convention for each — the opposite of the uniformity contract-first authoring is meant to provide.

## Decision

We will author published contracts using OpenAPI for synchronous APIs and AsyncAPI for asynchronous event and message contracts. The specification is authored first and lives in the domain's **Contracts** module ([ADR-0005](0005-standard-domain-module-layout.md)) as the authoritative definition of what the domain exposes; code is written to conform to it.

- Specifications are reviewed and versioned alongside the code that implements them
- Conformance is verified by executable tests asserting the running implementation matches its specification
- Contract changes are compared against the previously published version with schema-diffing tooling; an incompatible change fails the pipeline

Synchronous error responses use a **single, organization-wide error shape** rather than a per-domain convention. The shape carries at least a machine-readable type, a title, the response status, and a human-readable detail; a domain may add its own extension fields but never omits or renames the base fields. **RFC 9457 (`application/problem+json`)** is the realization; the decision is one consistent error contract across domains, defined once and reused. The shed-load responses in the backpressure contract (429 and 503, with `Retry-After`) use this same shape.

## Consequences

- Both synchronous and asynchronous interfaces are described in standard, language-agnostic formats, so contracts are uniform across domains regardless of implementation technology
- Authoring the specification first keeps the contract as the source of truth and prevents it from becoming a byproduct of code
- Standard formats enable conformance testing and automated breaking-change detection, so drift and incompatible changes are caught before release
- Housing specifications in the Contracts module gives consumers a single authoritative interface to reference without pulling in domain internals
- Contract-first authoring is a discipline shift for teams accustomed to generating specifications from code, and requires the supporting test and diffing tooling to be in place
- A single standard error shape lets a consumer calling multiple domains learn one error convention and write error handling once; the cost is adopting RFC 9457's vocabulary (type/title/status/detail/instance) even where a domain might have preferred its own, with extension fields as the escape valve for domain-specific detail
- The mechanism for expressing API versions remains open and will be recorded in a future ADR

## Reversibility

One-way door (costly). Contracts authored in these formats become the published, consumed source of truth, and contract-first authoring shapes the development workflow and tooling (conformance tests, breaking-change diffing). Switching specification formats would mean re-authoring every published contract and rebuilding the verification toolchain, though already-published interfaces could be migrated incrementally rather than all at once.
