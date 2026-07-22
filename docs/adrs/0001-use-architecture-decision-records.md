# 0001. Use Architecture Decision Records

Date: 2026-06-21

## Status

Accepted

## Context

As the system grows and teams evolve, the reasoning behind significant architectural and technical decisions becomes difficult to reconstruct. Without a deliberate record, decisions appear arbitrary, are relitigated unnecessarily, and context is lost when team members change. A lightweight, structured approach to capturing decisions is needed that is low-friction enough to be adopted consistently.

## Decision

We will use Architecture Decision Records to capture significant architectural and technical decisions. ADRs follow the Michael Nygard format and are stored in `docs/adrs/`. The governance document defines when an ADR is required and how it is approved.

## Consequences

- Significant decisions are recorded with their context and reasoning, making them traceable over time
- New team members can understand why the system is the way it is without relying on institutional memory
- Decisions can be revisited deliberately through the supersession process rather than informally or silently
- Teams have a clear, low-friction process for proposing and reviewing decisions
- The ADR directory grows over time and requires periodic review to ensure superseded and deprecated records are accurately reflected

## Reversibility

Two-way door. ADRs are a lightweight documentation practice with no runtime or structural coupling; abandoning or replacing the format would cost little beyond the convention itself, leaving the existing records as a historical artifact.
