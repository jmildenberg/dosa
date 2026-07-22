# DOSA

> DOSA

**Domain-Oriented Service Architecture** — the ReedTech engineering standard for how domains are designed,
built, secured, run, and governed.

This is **guidance, not a system**. The guidance itself (in [docs/](docs/)) contains no
application code — it describes how systems *should* be designed, built, operated, and
governed, and records the decisions behind that guidance so the reasoning is traceable over
time. Alongside it, the [reference architectures](reference-implementations/readme.md) provide
concrete, runnable realizations of the guidance in specific technology stacks.

## What this guidance covers — and what it doesn't

The guidance here is intentionally general and applies across the organization.
Concerns that legitimately differ by adopter are **not** defined here:

- **Specific tooling** — the concrete products used for CI/CD, scanning, identity,
  persistence, and so on
- **Compliance specifics** — the additional controls layered on top of the
  standard baseline security practices to meet particular data and regulatory obligations

This repository defines the standards those choices must satisfy.

## How this guidance is organized

| Document | Question it answers |
|---|---|
| [Architecture](docs/architecture.md) | **Why** — the principles that shape a system's structure and boundaries |
| [Engineering](docs/engineering.md) | **How to build** — the practices that put the principles into action |
| [Security](docs/security.md) | **How to secure** — the practices and gates that protect the system and its delivery |
| [Operations](docs/operations.md) | **How to run** — the run-time operating and reliability model *(draft)* |
| [Governance](docs/governance.md) | **Who decides** — the authority and accountability model |
| [Decision Records](docs/adrs/) | **What was decided, and why** — the authoritative log of significant decisions |
| [Reference Architectures](reference-implementations/readme.md) | **How to build it, concretely** — runnable realizations of the guidance per technology stack |

## Getting started

**New to the guidance?** Read in this order:

1. This page — what the guidance is and how it is organized
2. [Architecture](docs/architecture.md) — the principles everything else is grounded in
3. [Engineering](docs/engineering.md) — how those principles translate into day-to-day practice
4. [Security](docs/security.md) — how systems and their delivery are kept secure
5. [Governance](docs/governance.md) — how decisions are made and how the guidance evolves
6. [Decision Records](docs/adrs/) — the decisions in force and the reasoning behind them

**Adopting the guidance for a new domain or team?** Start from the Architecture
principles, then use the Engineering practices — particularly the standard domain
module layout and the delivery, security, and contract practices — to shape the
repository and pipeline. If a [reference architecture](reference-implementations/readme.md)
exists for your stack, clone its structure as a starting point.

## Architecture Decision Records

Significant architectural and technical decisions are captured as Architecture
Decision Records (ADRs) in [docs/adrs/](docs/adrs/). The
[governance document](docs/governance.md) defines when an ADR is required and how
it is approved, and the [ADR readme](docs/adrs/readme.md) describes the format and
lists the records in force.

## Contributing

This guidance is a living standard. Changes to the architecture, engineering, and
governance documents follow the ADR process defined in the
[governance document](docs/governance.md): a change is proposed, reviewed, and
approved before a foundational document is updated. Any team member may propose a
change.
