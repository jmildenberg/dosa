# 0012. JWT for Service-to-Service Authentication

Date: 2026-06-22

## Status

Accepted

## Context

In a Domain-Oriented Service Architecture, domains call one another across the network. The Least-Privilege Boundaries principle requires that trust never be implied by network location alone — a call reaching a service is not, by itself, evidence the caller is authorized. A standard mechanism is needed for a service to prove its identity and scoped permissions on every inbound request, verifiable without a synchronous lookup to a central authority on each call.

The mechanism must be interoperable across the languages and frameworks domains are built in, and must carry the caller's identity and permissions in a form the receiving service can verify independently.

This applies directly to synchronous calls, which carry a live token. Asynchronous, event-driven flows are different: a queued message is not a live call and cannot carry a bearer token, so per-request token verification cannot apply. Trust and attribution for event-driven work therefore need their own decision rather than being forced onto the synchronous mechanism.

## Decision

We will authenticate all synchronous inter-service communication using JWT (JSON Web Tokens). Each service verifies the token on every inbound request; trust is never granted on the basis of network location. Tokens are scoped to the calling service's identity and the permissions it requires, consistent with Least-Privilege Boundaries.

**Asynchronous and event-driven flows** are handled separately, because a queued message carries no live token:

- **Connection trust** — a producer publishing to a channel and a consumer subscribing authenticate the broker connection using whatever the message broker natively supports (mTLS, SASL, or equivalent); this need not be the OIDC/OAuth2 mechanism used for synchronous calls. Anonymous publish or subscribe is not permitted. This is a deliberate, scoped exception to token-on-every-request, not a gap in it.
- **Actor attribution** — the originating actor is carried as a plain identity reference in the event envelope (for example an `actor_id`), not a verifiable token. Authorization was already enforced at publish time; the reference exists so the consumer's resulting action can be attributed in logs and audit trails.
- **No re-authorization at consume time** — a consumer acts on a published event without re-checking whether the originating actor's permissions have changed since. Authorization happens once, at publish time. Because there is no re-authorization step to catch a replayed or redelivered message, idempotent and retry-safe processing is the consumer's responsibility.

## Consequences

- Every synchronous inbound request is authenticated, so trust is established explicitly rather than assumed from network position
- Tokens are self-contained and verifiable without a per-call lookup to a central authority, keeping verification fast
- JWT is broadly supported across languages and frameworks, so the mechanism works uniformly regardless of a domain's implementation technology
- Scoping tokens to identity and required permissions makes least privilege enforceable at the call boundary
- Token issuance, signing keys, rotation, and expiry must be managed; a compromised signing key or overly long-lived token widens the blast radius
- Services must consistently validate token scope, not merely its signature, or the least-privilege intent is lost
- Event-driven trust is established without forcing a synthetic token onto messages — broker-native authentication secures the connection and an envelope reference preserves attribution — at the cost of a broker-authentication dependency that is not the OIDC/OAuth2 mechanism used elsewhere
- Authorizing only at publish time trades continuous re-checking for simplicity and matches how brokers actually work, but means a consumer cannot assume the originating actor's permissions still hold at consume time; retry-safety and idempotency carry that weight instead

## Reversibility

One-way door (costly). Every service verifies tokens on every synchronous inbound request, so the token format is a system-wide runtime contract between all communicating domains. Replacing it would require coordinated changes across producers and consumers simultaneously to avoid breaking inter-service trust — a synchronized, cross-domain migration rather than an incremental one. The asynchronous model adds a second such contract (broker-connection authentication and the envelope attribution field) with the same coordinated-migration cost on the messaging path.
