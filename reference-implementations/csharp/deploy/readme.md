# C# :: Deploy

> [C#](../readme.md) :: Deploy

Packaging and deployment assets for this domain's [Hosts](../modules/hosts/readme.md) — one deployable
image per host, never bundled together, so each is deployable and scalable on its own lifecycle.

| Area | Contents |
|---|---|
| [docker/](docker/readme.md) | Image build assets |
| [helm/](helm/readme.md) | Kubernetes deployment chart(s) |

The delivery progression these artifacts flow through — the same image built once and promoted
environment to environment — is defined at the org level.

---

References: [docs/adrs/0008-standard-delivery-progression.md](../../../docs/adrs/0008-standard-delivery-progression.md), [docs/adrs/0009-containerization-with-oci-images.md](../../../docs/adrs/0009-containerization-with-oci-images.md), [docs/adrs/0010-adopt-kubernetes-as-deployment-target.md](../../../docs/adrs/0010-adopt-kubernetes-as-deployment-target.md)
