# DOSA :: Documentation

> [DOSA](../readme.md) :: Documentation

The ReedTech Architecture Guidance documents. See the [root readme](../readme.md)
for purpose, scope, and onboarding.

| Document | Question it answers |
|---|---|
| [Architecture](architecture.md) | **Why** — the principles that shape a system's structure and boundaries |
| [Engineering](engineering.md) | **How to build** — the practices that put the principles into action |
| [Security](security.md) | **How to secure** — the practices and gates that protect the system and its delivery |
| [Operations](operations.md) | **How to run** — the run-time operating and reliability model *(draft)* |
| [Governance](governance.md) | **Who decides** — the authority and accountability model |
| [Decision Records](adrs/) | **What was decided, and why** — the authoritative log of significant decisions |

## Standard version

The guidance is versioned as a whole so adopters can state exactly what they conform to. The current
**org standard is v1.0** (ADRs 0001–0020), effective 2026-07-09.

The version is semantic at the standard level: a **major** bump removes or reverses guidance (a
superseding ADR that breaks existing conformance); a **minor** bump adds guidance (a new ADR or a
material new section); a **patch** clarifies without changing obligations. Each business unit's
[conformance ledger](../business-units/) records the standard version its assessment was made against,
so a "Conformant" stance is anchored to a specific revision rather than a moving target.
