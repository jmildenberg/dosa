# 0016. OIDC and OAuth2 for Authentication and Authorization

Date: 2026-06-22

## Status

Accepted

## Context

[ADR-0012](0012-jwt-for-service-to-service-authentication.md) established JWT as the token format services verify on every request, but JWT is only a format — it does not define how a token is issued, how a user or client authenticates, or how access is authorized. As a result the security model covered token *verification* and machine identity, but left two questions unanswered: how human and external-client identity is established, and how access is granted and scoped.

The Least-Privilege Boundaries principle requires that both user and service access be scoped to the responsibilities a caller actually performs. Realizing it needs a standard for authentication and delegated authorization at the system edge. Implementing this per domain — custom login, ad hoc session handling, or bespoke token issuance — would fragment identity, duplicate sensitive credential handling across domains, and make scoping inconsistent. The alternatives at the standard level (SAML, or session-based custom authentication) are either heavier and less suited to API and machine-to-machine access, or non-standard.

Two edge cases also need a standard answer within this model: a browser single-page application is a **public client** that cannot safely hold long-lived tokens, and an **agent-driven host** acts both autonomously and on a user's behalf, so its identity must not silently escalate privilege.

## Decision

We will adopt OpenID Connect (OIDC) for interactive user authentication and OAuth2 for authorization and machine-to-machine access, issued by a central, OIDC-compliant identity provider operated as a shared platform capability. Domains consume tokens and never handle credentials directly.

- **Authentication (OIDC)** — interactive users authenticate against the identity provider, which establishes identity and issues signed tokens.
- **Authorization (OAuth2)** — access is delegated through OAuth2 flows: the authorization code flow for user-facing access and the client credentials flow for machine-to-machine and service clients. Service-to-service tokens (ADR-0012) are obtained through the client credentials flow.
- **Token format** — issued access tokens are JWTs, verified by services exactly as in service-to-service authentication, so one token format spans the system.
- **Authorization model** — coarse-grained access is expressed as OAuth2 scopes carried in the access token and enforced at the API boundary; finer-grained decisions within a domain are made from role and claim information in the token. Both are scoped to the minimum a caller requires.
- **SPA / BFF token custody** — a browser single-page application never holds OAuth2 tokens. The request-shaping host (backend-for-frontend) performs the authorization-code flow server-side on the SPA's behalf, holds the tokens server-side, and issues the browser only an HttpOnly, Secure session cookie; it attaches the real access token when it calls the domain-logic host. Tokens are never placed in browser-accessible storage.
- **Agent-driven host identity** — the agent-driven host authenticates as its own service identity for autonomous actions, and when acting on a user's behalf propagates and re-scopes the originating user's token rather than escalating to its own broader identity, so its tool calls never carry more privilege than the invoking user.

## Consequences

- Authentication and token issuance are standardized and centralized, so domains never handle credentials and do not implement their own login
- The identity model is unified end to end — OIDC authenticates interactive users, OAuth2 issues access tokens for user-facing and machine clients, JWT is the format throughout (ADR-0012), and every service verifies on each request
- OAuth2 scopes plus in-domain role and claim checks make least privilege enforceable for both users and services at the granularity each decision requires
- The client credentials flow gives service-to-service and machine-client authentication a defined issuance mechanism rather than leaving token origin unspecified
- A central identity provider is a critical shared dependency that must be highly available and carefully operated; its compromise has system-wide blast radius
- Teams must integrate against standard OIDC/OAuth2 flows and validate scopes and claims correctly, not merely token signatures, or the authorization intent is lost
- The authorization model is scope- and role-based by decision; a future move to centralized policy evaluation would be a separate decision
- Keeping OAuth2 tokens out of the browser — held server-side behind the request-shaping host — removes the cross-site-scripting token-theft exposure at the one layer running on infrastructure outside our control, and makes token custody a defining responsibility of that host rather than incidental response shaping
- Constraining the agent-driven host to its own identity for autonomous actions and to the user's re-scoped token when acting on their behalf keeps least privilege intact for AI-driven calls, which would otherwise tend toward broad standing access

## Reversibility

One-way door. A central OIDC-compliant identity provider becomes a system-wide dependency that every domain integrates against for authentication and token issuance; replacing it, or moving identity back into domains, would mean re-integrating every service's auth flows and migrating all clients and users at once. The standard protocols (OIDC/OAuth2) keep the IdP itself swappable, but the dependency on a central, standards-based identity model is deeply embedded.
