# 0003. Adopt Domain-Oriented Service Architecture

Date: 2026-06-21

## Status

Accepted

## Context

A service decomposition model is needed that aligns with business capabilities and team ownership while avoiding the operational overhead of pure microservices and the coupling risks of a monolith. Services need clear boundaries, stable interfaces, and independent deployment lifecycles. The model must support teams owning multiple services without creating excessive inter-service dependencies.

## Decision

We will adopt Domain-Oriented Service Architecture (DOSA). Services are grouped into domains aligned with business capabilities. Each domain has clear logical, code, data, and operational boundaries. Domains interact through published contracts — synchronous API contracts for request/response and asynchronous events for decoupled workflows. Each domain is the single source of truth for its data.

## Consequences

- Service boundaries align with business capabilities, making ownership and responsibility clear
- Data ownership is explicit — domains cannot reach into another domain's storage directly
- Inter-domain coupling is visible and flows through published contracts
- Asynchronous, event-driven communication reduces runtime coupling between domains
- Teams may own multiple domains, requiring clear ownership documentation within each domain repository
- Requires explicit contract management and versioning discipline as the system evolves

## Reversibility

One-way door. The domain decomposition shapes service boundaries, data ownership, team structure, and inter-domain contracts; re-drawing those boundaries means merging or splitting services, migrating data ownership, and renegotiating contracts across teams — a deep, organization-wide change rather than a local one.
