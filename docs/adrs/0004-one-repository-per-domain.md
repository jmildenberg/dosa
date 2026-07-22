# 0004. One Repository Per Domain

Date: 2026-06-21

## Status

Accepted

## Context

Given the adoption of Domain-Oriented Service Architecture, a decision is needed on how domain code is physically organized. The choice of repository structure directly affects how domain boundaries are enforced, how CI/CD pipelines are scoped, and how cross-domain dependencies are managed.

## Decision

Each domain will have its own repository. Domain boundaries are structurally enforced — a dependency on another domain cannot be introduced without explicitly referencing its published packages through an artifact repository.

## Consequences

- Domain boundaries are enforced at the repository level, making cross-domain coupling explicit and deliberate
- Each domain has an independent CI/CD pipeline and deployment lifecycle
- Access control and ownership are clear at the repository level
- Shared and platform libraries are distributed through an artifact repository
- Cross-domain changes require coordinated pull requests across repositories
- More repositories to manage as the number of domains grows

## Reversibility

One-way door (costly). The repository topology determines pipeline scoping, access control, artifact distribution, and the physical enforcement of domain boundaries. Consolidating into fewer repositories — or splitting differently — would mean rewiring every CI/CD pipeline, re-establishing boundaries that the repo split currently enforces for free, and rewriting cross-domain dependency flows; the cost grows with the number of domains.
