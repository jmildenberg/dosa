# DOSA :: Documentation :: Architecture Decision Records

> [DOSA](../../readme.md) :: [Documentation](../readme.md) :: Achitecture Decision Records

This directory contains the Architecture Decision Records (ADRs) for this guidance. ADRs are the authoritative record of significant architectural and technical decisions — what was decided, why, and what the consequences are. See the [governance document](../governance.md) for when an ADR is required and how it is approved.

## Records

| ADR | Title | Status |
|---|---|---|
| [0001](0001-use-architecture-decision-records.md) | Use Architecture Decision Records | Accepted |
| [0002](0002-adopt-clean-architecture.md) | Adopt Clean Architecture | Accepted |
| [0003](0003-adopt-domain-oriented-service-architecture.md) | Adopt Domain-Oriented Service Architecture | Accepted |
| [0004](0004-one-repository-per-domain.md) | One Repository Per Domain | Accepted |
| [0005](0005-standard-domain-module-layout.md) | Standard Domain Module Layout | Accepted |
| [0006](0006-adopt-trunk-based-development.md) | Adopt Trunk-Based Development | Accepted |
| [0007](0007-testing-trophy-and-test-classification.md) | Testing Trophy and Test Classification | Accepted |
| [0008](0008-standard-delivery-progression.md) | Standard Delivery Progression | Accepted |
| [0009](0009-containerization-with-oci-images.md) | Containerization with OCI Images | Accepted |
| [0010](0010-adopt-kubernetes-as-deployment-target.md) | Adopt Kubernetes as Deployment Target | Accepted |
| [0011](0011-adopt-opentelemetry-for-observability.md) | Adopt OpenTelemetry for Observability | Accepted |
| [0012](0012-jwt-for-service-to-service-authentication.md) | JWT for Service-to-Service Authentication | Accepted |
| [0013](0013-openapi-and-asyncapi-for-contract-authoring.md) | OpenAPI and AsyncAPI for Contract Authoring | Accepted |
| [0014](0014-schema-first-persistence.md) | Schema-First Persistence | Accepted |
| [0015](0015-dependency-management.md) | Dependency Management | Accepted |
| [0016](0016-oidc-and-oauth2-for-authentication-and-authorization.md) | OIDC and OAuth2 for Authentication and Authorization | Accepted |
| [0017](0017-api-versioning-via-url-path.md) | API Versioning via URL Path | Accepted |
| [0018](0018-adopt-a-service-catalog.md) | Adopt a Service Catalog | Accepted |
| [0019](0019-gitops-for-declarative-deployment.md) | GitOps for Declarative Deployment | Accepted |
| [0020](0020-entity-identity-surrogate-and-public-identifier.md) | Entity Identity: Relational Surrogate and Public Identifier | Accepted |

## Format

Each ADR follows the Michael Nygard format and is stored as a markdown file named `NNNN-title-with-dashes.md`, where `NNNN` is a zero-padded sequential number.

The canonical skeleton lives in [`template.md`](template.md) — copy it when authoring a new ADR. It carries the Nygard sections (Title / Date / Status / Context / Decision / Consequences) plus a **Reversibility** assessment. `template.md` is the single source of truth for the format; this section is only a pointer, so the two cannot drift.

## Conventions

- ADRs are numbered sequentially starting from `0001`
- Titles are short and descriptive — written as a noun phrase, not a question
- The Status field reflects the current state of the ADR
- When an ADR is superseded, update its `Status` to reference the superseding ADR — this is a lifecycle metadata update, not a change to the decision itself
- ADRs are immutable in substance after acceptance — corrections or reversals are new ADRs that supersede the original
- Every ADR includes a **Reversibility** assessment — whether the decision is a one-way or two-way door and the rough cost to reverse

## Tooling

[adr-tools](https://github.com/npryce/adr-tools) is a command-line tool that automates ADR creation, numbering, and supersession and is compatible with this format. Teams may use it or manage ADRs manually — the format and conventions above are the standard regardless of tooling.
