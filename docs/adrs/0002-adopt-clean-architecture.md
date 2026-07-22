# 0002. Adopt Clean Architecture

Date: 2026-06-21

## Status

Accepted

## Context

A consistent layering approach is needed to keep business logic isolated from infrastructure, frameworks, and external dependencies. Without a clear structure, codebases drift toward tightly coupled designs that are difficult to test, maintain, and evolve independently. The chosen model must support testability of domain logic without requiring infrastructure, and must allow infrastructure and framework choices to be replaced without affecting business logic.

## Decision

We will adopt Clean Architecture as the foundational layering model. Dependencies point inward — domain and application layers have no dependencies on infrastructure, frameworks, or external systems. The four layers are: Enterprise Business Rules (Domain), Application Business Rules (Application), Interface Adapters (Infrastructure), and Frameworks and Drivers (Host).

## Consequences

- Business logic is independently testable without infrastructure
- Framework and infrastructure choices are replaceable without touching the domain or application layers
- Layer dependency rules can be enforced by architectural rules testing tooling
- Cross-layer coupling is visible and deliberate rather than accidental
- Requires discipline to maintain layer boundaries as the codebase grows
- More initial structural overhead compared to simpler, unstructured approaches

## Reversibility

One-way door. Clean Architecture is a pervasive structural choice that every domain's layering, module layout, and dependency rules are built around; reversing it would mean re-organizing code across all repositories and unwinding assumptions baked into the standard module layout and architectural tests — a costly, system-wide undertaking.
