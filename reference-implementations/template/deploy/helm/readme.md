# Template :: Deploy :: Helm

> [Template](../../readme.md) :: [Deploy](../readme.md) :: Helm

Kubernetes deployment chart(s) for this domain's [Hosts](../../modules/hosts/readme.md)
(see [ADR-0010](../../../../docs/adrs/0010-adopt-kubernetes-as-deployment-target.md)) — one release per
host, health-checked rolling deployments, environment-specific values layered on top of a shared base
rather than forked per environment.

Kubernetes was chosen because it is cloud-agnostic and runs the OCI images built in
[docker/](../docker/readme.md) on any compliant infrastructure, keeping delivery portable across
providers.
