# DOSA :: Documentation :: Governance

> [DOSA](../readme.md) :: [Documentation](./readme.md) :: Governance

This document defines how decisions are made, standards are upheld, and ownership is maintained across teams and domains. Where the [architecture document](architecture.md) establishes principles and the [engineering document](engineering.md) defines practices, this document establishes the authority and accountability model that keeps both trustworthy over time.

## Decision Making

The Technical Architecture Group (TAG) is the organization-level body of architects — which may include some team leads — accountable for architectural coherence across domains.

Architectural and technical decisions of significant scope are made collaboratively between the TAG and team leads. No significant architectural decision is made unilaterally — decisions that affect shared principles, cross-domain interfaces, or system-wide standards are reviewed with the relevant team leads before acceptance.

Day-to-day decisions within a domain are made by the owning team within the bounds of the established principles and practices. Teams do not need approval for decisions that operate within those bounds.

Significant decisions — those that introduce new patterns, deviate from established principles, affect cross-domain interfaces, or shape the direction of the architecture — are captured as Architecture Decision Records. The Architecture Decision Records section below defines when an ADR is required and how it is approved.

## Architecture Decision Records

An ADR is required when a decision meets one or more of the following conditions:

- It introduces a pattern or approach not covered by existing principles or practices
- It selects a technology or tool that affects more than one domain or team
- It deviates from an established principle or practice and is intended to become the new standard
- It changes the content of the architecture, engineering, or governance documents
- It affects a cross-domain interface, shared platform capability, or system-wide standard

An ADR is not required for decisions that operate within established bounds — implementation choices, library selections local to a single domain, or feature work that follows existing patterns.

**Proposing an ADR**: Any team member may propose an ADR. A proposal documents the context, the options considered, the decision, and the reasoning behind it.

**Approval**: Proposed ADRs are reviewed by the relevant team leads and approved by a member of the TAG whose architectural remit covers the decision. Where a decision spans multiple areas, no single remit clearly applies, or approval is blocked or contested, it escalates to the TAG, which resolves it collectively. No single individual is the sole approver. Changes to foundational documents are not applied until the ADR is accepted.

**Lifecycle**: An ADR moves through the following states:

| State | Meaning |
|---|---|
| **Proposed** | Drafted and under review |
| **Accepted** | Approved and in effect |
| **Superseded** | Replaced by a newer ADR — references its successor |
| **Deprecated** | No longer applicable but retained for historical context |

Accepted ADRs are immutable in substance after acceptance. Lifecycle metadata such as `Status` may be updated to reflect superseded or deprecated state, but a change to an accepted decision is a new ADR that supersedes the original. The format and template for ADRs are maintained in `docs/adrs/`.

## Domain Ownership

Each domain is assigned to a primary owning team. Teams may own multiple domains. Ownership is documented within the domain repository and is the authoritative record of who is accountable for that domain. This record lives in a known location — `docs/ownership.md` — capturing the owning team, escalation and support contacts, and the accountability scope below. Review and approval of changes is enforced through a code-ownership file (`CODEOWNERS`) that routes pull requests on owned paths — in particular the published contracts — to the owning team as required reviewers.

Ownership is mutable — as the organization evolves, domain ownership may transfer between teams. Domains should be sufficiently well-documented to make ownership transfer low-risk; a domain whose behavior, contracts, and operational characteristics are clearly recorded can be handed off without loss of institutional knowledge.

The owning team is responsible for:

- **Published contracts**: Reviewing and approving changes to the domain's published interfaces, including API and event contracts
- **Data integrity**: Maintaining the domain's data as the single source of truth and governing access to it
- **Operational health**: Ensuring the domain meets its operational and resilience obligations
- **Inter-domain access**: Reviewing and approving requests from other domains to access this domain's resources or data in ways that fall outside published contracts
- **Evidencing tests**: Owning the tests that evidence its published contracts, data integrity, and inter-domain access — ownership of a guarantee includes ownership of its proof

## Standards Enforcement

Standards are enforced through three complementary layers:

- **Pipeline gates**: Automated enforcement of build, test, security, and contract compatibility standards. Gates are defined in the [engineering](engineering.md) and [security](security.md) documents and apply to every build. A gate failure blocks promotion — standards are not deferred.
- **Promotion audit**: Promotions must be auditable. Each promotion produces a record — actor, artifact version, approval, target environment, and timestamp — tied to the approval gates above. This record is the accountability and compliance evidence that a promotion was authorized and traceable; it is distinct from observability data, which exists for diagnosis and may be sampled or dropped. The promotion mechanism that emits this record is defined in the [engineering document](engineering.md).
- **Code review**: The primary human enforcement layer. Reviewers are responsible for evaluating correctness, architectural conformance, domain boundary respect, and contract changes, as defined in the [engineering document](engineering.md). Automated tooling assists but does not replace reviewer judgment.
- **Retrospectives**: Teams conduct regular retrospectives to evaluate adherence to principles and practices, surface systemic friction, and identify areas for improvement. Recurring themes are escalated to the TAG and team leads for broader discussion.

The TAG and team leads are collectively responsible for identifying and addressing systemic drift — patterns of deviation too subtle to be caught by any single gate or review.

## Definition of Done

Standards are upheld not only by pipeline gates and review but by a shared Definition of Done — a per-change checklist confirming that principle-level obligations are actually met before work is considered complete. It is how the principles in the architecture, engineering, and security documents get applied consistently rather than remembered unevenly. A change is not done unless:

- **Security**: new or changed endpoints enforce authorization, introduce no unused or broader-than-necessary scopes, and validate inbound payloads against their contract schema at the trust boundary (see the [security document](security.md))
- **Observability**: new endpoints and event handlers emit a trace span and structured log entries consistent with the shared conventions (see [Observability Implementation](engineering.md#7-observability-implementation))
- **Resilience**: new outbound calls have defined timeout and retry behavior, and new consumers of retryable operations are idempotent (see [Service Operability](engineering.md#6-service-operability))
- **Contracts**: any new or changed published contract has a corresponding specification update and passes conformance and breaking-change checks, and is published to the contract catalog as part of the same change rather than as a later follow-up (see [API and Contract Design](engineering.md#9-api-and-contract-design))
- **Delivery**: the change is expressed as a new versioned build promoted through the pipeline — never a manual, out-of-band change to a running environment

The specific checklist is expected to be refined as the guidance and its reference architectures mature, but maintaining a Definition of Done and enforcing it at review and merge time is a governance requirement, not an optional team preference. It complements the enforcement layers above: gates catch what is automatable, and the Definition of Done ensures the obligations that resist full automation are checked deliberately on every change.

## Exception Process

Exceptions to established principles or practices are occasionally warranted. The exception process ensures they are deliberate, visible, and resolved promptly.

- **Approval**: Exceptions require approval appropriate to their scope. For direct access to a domain's resources or data outside published contracts, the owning domain may approve a narrow, time-limited exception. Exceptions that are long-lived, cross-domain in impact, or establish a new standard are reviewed by the relevant team leads and approved by a member of the TAG whose architectural remit covers the deviation, before it is introduced. Where a deviation spans multiple areas, no single remit clearly applies, or approval is blocked or contested, it escalates to the TAG, which resolves it collectively. No single individual is the sole approver
- **Scope**: An exception applies only to the specific case for which it was approved — it does not establish a precedent or permit broader deviation
- **Duration**: Exceptions are time-limited and may not become permanent without going through the ADR process to formally update the standard
- **Resolution**: If resolving the exception requires a code change, that change must be delivered by the next release
- **Documentation**: Approved exceptions are recorded with their scope, rationale, duration, and resolution plan

## Document Governance

The architecture and engineering documents are living standards. Changes to them follow the same ADR process as significant system decisions — a proposed change is documented, reviewed by the relevant team leads, and approved by a member of the TAG whose architectural remit covers the change before the document is updated. Where a change spans multiple areas, no single remit clearly applies, or approval is blocked or contested, it escalates to the TAG, which resolves it collectively. No single individual is the sole approver.

This ensures changes to principles and practices are deliberate, reasoned, and traceable. A change to a foundational document that bypasses review is itself a governance violation.

ADRs are maintained in the `docs/adrs/` directory and serve as the authoritative record of both system decisions and changes to the standards documents.
