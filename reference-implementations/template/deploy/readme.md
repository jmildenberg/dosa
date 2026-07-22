# Template :: Deploy

> [Template](../readme.md) :: Deploy

Packaging and deployment assets for this domain's [Hosts](../modules/hosts/readme.md) — one deployable
image per host, never bundled together, so each is deployable and scalable on its own lifecycle
(see [ADR-0009](../../../docs/adrs/0009-containerization-with-oci-images.md)).

| Area | Carries forward |
|---|---|
| [docker/](docker/readme.md) | [ADR-0009 (Containerization with OCI Images)](../../../docs/adrs/0009-containerization-with-oci-images.md) |
| [helm/](helm/readme.md) | [ADR-0010 (Adopt Kubernetes as Deployment Target)](../../../docs/adrs/0010-adopt-kubernetes-as-deployment-target.md) |

The delivery progression these artifacts flow through — the same image built once and promoted
environment to environment — is defined in [ADR-0008 (Standard Delivery Progression)](../../../docs/adrs/0008-standard-delivery-progression.md).
