# Template :: Documentation :: Contracts

> [Template](../../readme.md) :: [Documentation](../readme.md) :: Contracts

The authoritative OpenAPI and AsyncAPI specifications for what this domain publishes. These specs are
authored *first* — code in [modules/contracts/](../../modules/contracts/readme.md) is generated from or
verified against them, never the reverse.

- [api/](api/readme.md) — OpenAPI specs for synchronous request/response endpoints
- [messaging/](messaging/readme.md) — AsyncAPI specs for published and consumed events

A breaking change here is a breaking change for every consumer of this domain.

---

References: [docs/adrs/0013-openapi-and-asyncapi-for-contract-authoring.md](../../../../docs/adrs/0013-openapi-and-asyncapi-for-contract-authoring.md)
