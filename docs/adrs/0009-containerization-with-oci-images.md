# 0009. Containerization with OCI Images

Date: 2026-06-22

## Status

Accepted

## Context

A standard packaging and delivery format is needed for every runnable artifact. Without one, each Host is built and shipped its own way, the runtime environment differs between local, CI, and production, and the delivery progression in [ADR-0008](0008-standard-delivery-progression.md) — which depends on promoting one immutable artifact through every environment — has no concrete artifact to promote.

The format should be portable rather than tied to a single vendor's runtime, so the orchestration and hosting decisions made elsewhere are not locked to one tool. It should also produce minimal, traceable, and secure-by-default artifacts.

## Decision

We will containerize every runnable artifact, building images to the OCI standard so they are portable across compliant runtimes (Docker, containerd, Podman, or otherwise).

- Each **Host** module ([ADR-0005](0005-standard-domain-module-layout.md)) produces its own image; hosts are never bundled together, so each is deployable and scalable on its own lifecycle.
- Images are built with multi-stage builds, separating the build environment from a minimal runtime image, and run as a non-root user by default.
- Images are published to a container registry, tagged so version and origin are traceable. The same image that passes CI is the image that is deployed — it is never rebuilt per environment, consistent with the build-once promotion model in ADR-0008.

## Consequences

- Every artifact is packaged and delivered the same way, giving the delivery progression a single immutable unit to promote
- OCI compliance keeps artifacts portable across runtimes, so hosting and orchestration choices are not locked to one vendor
- One image per Host means each entry point is versioned, deployed, and scaled independently
- Multi-stage, minimal, non-root images reduce attack surface and image size by default rather than by later hardening
- Traceable tagging makes it possible to know exactly what is running in any environment
- Containerization adds build tooling and registry infrastructure that every domain must operate

## Reversibility

One-way door (costly). OCI images are the unit of delivery the pipeline, registry, and Kubernetes runtime are all built around; moving away from container packaging would mean rebuilding the entire build-and-deploy chain. Because the standard is the deliberately portable, vendor-neutral one, the lock-in is to the format rather than a vendor, which softens the cost — but it remains a deep, cross-cutting reversal.
