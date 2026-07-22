# 0019. GitOps for Declarative Deployment

Date: 2026-07-07

## Status

Accepted

## Context

[ADR-0009](0009-containerization-with-oci-images.md) established OCI images as the unit of delivery, [ADR-0010](0010-adopt-kubernetes-as-deployment-target.md) established Kubernetes as where they run, and [ADR-0008](0008-standard-delivery-progression.md) established the environment progression and the rule that the tested artifact is the shipped artifact. What none of them decided is *how* deployment state is expressed and applied to a cluster, and how a domain's set of hosts is released and rolled back together.

Two models are available. In an **imperative** model, CI pushes changes to the cluster directly (`kubectl apply`, `helm upgrade`) at the end of a pipeline; the cluster's actual state is whatever the last push left, with no single source of truth and no automatic correction of drift. In a **declarative / GitOps** model, the desired state is recorded in version control and a controller continuously reconciles the cluster to match it.

The imperative model has real weaknesses for this architecture: a manual or out-of-band change to a running cluster silently persists; there is no durable record of what should be running where; and rollback is an ad hoc reverse-push rather than a defined operation. A domain also produces several host images ([ADR-0005](0005-standard-domain-module-layout.md)) that must be released as one tested combination, which the imperative model does not express as a unit.

## Decision

We will deploy through a **declarative GitOps** model.

- **Packaging.** A domain's runtime manifests are packaged as a single **Helm chart per domain**, versioned with the domain's release and published as an OCI artifact alongside its images. The chart is the one deployable unit for the domain, regardless of how many hosts it contains.
- **Desired state in Git.** A **separate GitOps repository**, outside any domain repository, records which chart version should run in each environment, together with environment-specific configuration. It is the single source of truth for what is deployed where.
- **Reconciliation, not push.** A GitOps controller — Argo CD is the reference implementation — watches the GitOps repository and continuously reconciles each target cluster and namespace to the declared state. CI publishes artifacts; it never applies changes to a cluster directly. An out-of-band manual change is automatically reverted back to the declared state.
- **Deploy and roll back as a unit.** Deploying is a change to the recorded chart version; rolling back is reverting it. Either action moves *all* of a domain's hosts to the referenced tested combination in one step — never one host in isolation — while each host image still scales independently at runtime.
- **The record is the audit trail.** Because every promotion is a change to the GitOps repository, its commit history — author, approval, timestamp, target environment — is the retained promotion audit record, distinct from droppable pipeline telemetry.

## Consequences

- The desired state of every environment is reproducible and inspectable in version control, and configuration drift is corrected automatically rather than accumulating silently
- Rollback becomes a defined, uniform operation — revert the version reference — rather than an ad hoc reverse-deployment, and it reverts the domain as a whole so hosts cannot drift out of their tested combination
- The GitOps commit history provides an accountability trail for promotions without a separate audit mechanism
- Promotion and rollback are decoupled from CI: the pipeline's job ends at publishing artifacts, and what runs where is governed entirely by the GitOps repository
- Operating this adds moving parts — a GitOps repository and a reconciler to run and secure — and a per-domain Helm chart to maintain, overhead that is only justified at the scale Kubernetes already implies
- A rollback still requires verification (the reverted version is re-tested against the current environment), because a version that reconciles cleanly is not automatically known to be healthy against dependencies that have since changed
- Helm and Argo CD are named as the conformant realization; the decision is the GitOps model and per-domain versioned packaging, and another chart or reconciler that preserves those properties would conform

## Reversibility

Two-way door, with moderate cost. The GitOps model is a delivery-platform choice, not an application-code one — no domain's source depends on it, so moving to a different deployment model later means rewiring pipelines and the GitOps repository rather than changing applications. The cost grows with the number of domains wired to the GitOps repository and the operational habits built around it, but it does not compound into the code the way the module layout ([ADR-0005](0005-standard-domain-module-layout.md)) does.
