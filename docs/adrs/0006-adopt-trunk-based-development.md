# 0006. Adopt Trunk-Based Development

Date: 2026-06-22

## Status

Accepted

## Context

A branching model is needed that keeps integration continuous and main always releasable, supporting the Continuous Delivery practice where every change to main flows automatically toward a live environment. Long-lived branching models — such as GitFlow or per-feature branches that live for days or weeks — accumulate divergence, surface integration conflicts late, and make it difficult to know whether main is in a releasable state at any given moment.

At the same time, the organization spans multiple teams with different release cadences and obligations. A single mandated release flow would not fit every group. What must be consistent is the integration model; how a release is cut and promoted on top of it can reasonably vary.

## Decision

We will adopt trunk-based development as the shared branching standard across the organization. All work branches from and merges back to main through short-lived branches, keeping the integration surface small and integration problems visible early. Main is kept releasable at all times.

Trunk-based development is the organization-wide constant. The specific release flow built on top of it — how release candidates are cut, branched, and promoted — is a team-level choice and may vary by group.

## Consequences

- Integration happens continuously, so conflicts surface early while they are small rather than at a late merge
- Main is always in a releasable state, which is the precondition the Continuous Delivery pipeline depends on
- Short-lived branches pair naturally with the small, purposeful pull requests expected in the Code Review practice
- A single integration model applies everywhere, so engineers moving between teams encounter the same branching discipline
- Teams retain flexibility to choose a release flow that fits their cadence and obligations, without fragmenting the integration model
- Keeping main continuously releasable requires discipline — incomplete work must be integrated behind feature flags or kept small enough to merge safely
- The model assumes strong automated verification on every merge; without it, the burden of keeping main releasable falls back on manual coordination

## Reversibility

Two-way door (moderate). Trunk-based development is a workflow convention rather than a structural commitment; a team could move to a longer-lived branching model without changing code or runtime. The cost is retraining and re-tooling CI gates, and it works against the Continuous Delivery model the rest of the guidance assumes, so reversing is more disruptive than purely local.
