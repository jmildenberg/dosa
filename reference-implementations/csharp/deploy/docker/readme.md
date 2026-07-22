# C# :: Deploy :: Docker

> [C#](../../readme.md) :: [Deploy](../readme.md) :: Docker

One Dockerfile per [Host](../../modules/hosts/readme.md), each a multi-stage build: a build stage with
the full toolchain, and a minimal runtime stage that ships only what the host needs to run, as a
non-root user by default. The image built here is the same immutable artifact that is promoted,
unrebuilt, through every environment in the delivery progression.

---

References: [docs/adrs/0008-standard-delivery-progression.md](../../../../docs/adrs/0008-standard-delivery-progression.md), [docs/adrs/0009-containerization-with-oci-images.md](../../../../docs/adrs/0009-containerization-with-oci-images.md)
