# Template :: Deploy :: Docker

> [Template](../../readme.md) :: [Deploy](../readme.md) :: Docker

Image build assets — one Dockerfile per [Host](../../modules/hosts/readme.md), never one shared image for
the whole domain (see [ADR-0009](../../../../docs/adrs/0009-containerization-with-oci-images.md)).

Each Dockerfile is a multi-stage build: a build stage with the full toolchain, and a minimal runtime
stage that ships only what the host needs to run, as a non-root user by default. The image built here is
the same immutable artifact that is promoted, unrebuilt, through every environment in the
[delivery progression](../../../../docs/adrs/0008-standard-delivery-progression.md).
