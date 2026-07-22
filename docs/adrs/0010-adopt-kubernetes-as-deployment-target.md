# 0010. Adopt Kubernetes as Deployment Target

Date: 2026-06-22

## Status

Accepted

## Context

[ADR-0009](0009-containerization-with-oci-images.md) establishes OCI container images as the unit of delivery, but does not say where those images run. A runtime platform is needed that can schedule containers, manage their lifecycle, perform health-checked rolling deployments, and scale hosts independently — the operational behavior the engineering practices assume.

The platform should not tie delivery to a single cloud provider. A provider-specific orchestration service would couple every domain to one vendor and undermine the portability that OCI images were chosen to preserve.

## Decision

We will adopt Kubernetes as the standard deployment target. Kubernetes is a cloud-agnostic, widely adopted orchestration platform that runs OCI images on any compliant infrastructure, keeping the delivery model portable across cloud providers and on-premises environments.

Hosts are built to the Kubernetes runtime contract:

- **Health endpoints** — liveness and readiness probes are exposed for scheduling and traffic decisions
- **Graceful shutdown** — termination signals are handled and in-flight work is drained before exit
- **Statelessness** — hosts carry no local state; all state lives in external backing services
- **Resource boundaries** — images are deployable with explicit CPU and memory limits

Deployments use rolling updates — new instances are brought up before old instances are terminated. Rollback behavior is provided by the delivery platform and must be explicitly defined, whether through automation or an operational procedure.

## Consequences

- A single, cloud-agnostic platform runs every artifact, so domains share one operational model and are not coupled to a specific cloud provider
- Rolling updates give availability-preserving deployments as a platform capability, and rollback behavior is defined by the delivery platform rather than improvised per domain
- The runtime contract (probes, graceful shutdown, statelessness, resource limits) gives hosts a clear, uniform set of operational obligations
- Independent, health-checked scaling per Host follows naturally from the one-image-per-Host decision
- Kubernetes carries significant operational complexity; teams depend on a platform capability to run and maintain clusters
- Building to the Kubernetes runtime contract is a real constraint on hosts — statelessness in particular must be designed for, not retrofitted

## Reversibility

One-way door. The choice runs deep: hosts are built to the Kubernetes runtime contract (probes, graceful shutdown, statelessness, resource limits), deployment and rollback behavior depend on platform capabilities, and operational tooling assumes the cluster model. Migrating to a different runtime would mean re-meeting a new platform's contract across every host and rebuilding the operational layer — among the costliest reversals in the set. Cloud-agnostic Kubernetes mitigates provider lock-in but not the dependency on Kubernetes itself.
