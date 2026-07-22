# DOSA :: Documentation :: Security

> [DOSA](../readme.md) :: [Documentation](./readme.md) :: Security

This document defines the security practices for systems built on this guidance — how security is implemented, verified, and operated across the development and delivery lifecycle. Where the [architecture document](architecture.md#4-least-privilege-boundaries) establishes the structural intent through the Least-Privilege Boundaries principle, this document defines how that intent is put into practice and enforced.

Security is treated as a first-class concern with its own foundational document rather than a subsection of engineering, because it is an accountable discipline in its own right: the practices here are owned, reviewed, and evolved as a distinct standard. The [engineering document](engineering.md) is where several of these practices *execute* — the CI pipeline runs the security gates, Interface Adapters implement input validation, externalized configuration injects secrets — but the security standard itself lives here and those documents reference it rather than restating it. The [operations document](operations.md) owns the general incident-response process that the security incident track below extends, and the [governance document](governance.md) owns the exception and compliance model these practices sit within.

Specific tooling choices are recorded separately by each business unit; the practices defined here are tool-agnostic. Where a concrete product is named, it is an example of a conformant implementation, not a mandate.

## 1. Secure Pipeline Gates

Security verification is integrated into the delivery pipeline, not applied after the fact. Gates fall into two categories by *when* they can run: pre-merge gates that analyze code and dependencies, and deploy-stage gates that require a running target. A gate failure blocks promotion — security findings are not deferred to a later stage.

### Pre-Merge Gates

These run in the CI pipeline (see [Continuous Delivery](engineering.md#5-continuous-delivery)) before an image is promoted:

- **SAST (Static Application Security Testing)**: Analyses source code for security vulnerabilities before the code runs
- **SCA (Software Composition Analysis)**: Scans dependencies for known vulnerabilities — addressed further in [Dependency Management](engineering.md#10-dependency-management)
- **Container Image Scanning**: Scans built images for vulnerabilities in base images and installed packages
- **Code Quality Analysis**: Enforces quality thresholds that surface security-relevant issues such as error handling gaps and unsafe patterns
- **Sensitive-data logging scan**: Checks for unmarked sensitive-data patterns — credentials, tokens, personal information, payment data — in log and trace output. This enforces the prohibition on sensitive data in logs (see [Observability Implementation](engineering.md#7-observability-implementation)) and in token claims (see Token Lifecycle below) as an automated gate rather than a guideline
- **Least-privilege scope check**: Flags an identity or client registration carrying OAuth2 scopes it does not exercise, enforcing the Least Privilege Implementation section below as a checkable property rather than a one-time design decision

### Deploy-Stage Gates

Some verification requires an actually running target and therefore cannot run pre-merge:

- **DAST (Dynamic Application Security Testing)**: Exercises a deployed environment — typically Staging, before a release candidate is eligible for Production promotion — for vulnerabilities visible only at runtime. This is where browser-facing hardening is verified: response security headers (such as Content-Security-Policy and HSTS), cookie flags, and CORS configuration are verifiable only against a genuinely running deployment, not static source. DAST is a delivery-pipeline gate, not a per-pull-request one.

## 2. Authentication and Authorization

Interactive user authentication is handled through OpenID Connect (OIDC), and access is delegated through OAuth2. Machine and service clients obtain tokens through the OAuth2 client credentials flow. A central, OIDC-compliant identity provider — a shared platform capability (see [Domain Structure](engineering.md#3-domain-structure)) — issues the tokens domains consume rather than domains implementing their own authentication.

- **Authentication (OIDC)**: Interactive users authenticate against the identity provider, which establishes identity and issues signed tokens. Domains never handle credentials directly.
- **Authorization (OAuth2)**: Access is delegated through OAuth2 flows — the authorization code flow for user-facing access and the client credentials flow for machine-to-machine and service clients. Issued access tokens are JWTs, verified by services exactly as described in Service-to-Service Authentication.
- **Scopes and roles**: Coarse-grained access is expressed as OAuth2 scopes carried in the access token and enforced at the API boundary. Finer-grained decisions within a domain are made from role and claim information in the token. Both are scoped to the minimum a caller requires, consistent with the Least-Privilege Boundaries principle.

This unifies the identity model: OIDC authenticates interactive users, OAuth2 issues access for user-facing and machine clients, JWT is the token format throughout, and every service verifies the token on each request, as decided in [ADR-0016](adrs/0016-oidc-and-oauth2-for-authentication-and-authorization.md).

### SPA and BFF Token Handling

A browser-based single-page application is a public client and does not hold OAuth2 tokens directly. Where a domain has a presentation role served by a [request-shaping host (BFF)](architecture.md#2-domain-oriented-service-architecture), token custody belongs to that host:

- The SPA authenticates through the request-shaping host, which performs the OIDC authorization-code flow server-side on the SPA's behalf.
- The host holds the resulting tokens server-side and issues the browser only an HttpOnly, Secure session cookie — the SPA never sees, stores, or transmits an access or refresh token.
- When the SPA calls the host, the session cookie authenticates the browser session; the host attaches the real access token when it calls the domain-logic host on the user's behalf.

Tokens held in browser storage are exposed to theft through any cross-site-scripting vulnerability in the SPA. Keeping tokens server-side means a compromised SPA has no bearer token to exfiltrate — least privilege and no-implicit-trust applied to the one layer that runs on infrastructure outside our control. This custody responsibility is a defining reason the request-shaping host exists as a distinct role, not merely a response shaper.

### Agent-Driven Host Identity

The agent-driven host authenticates as its own service identity for actions it takes autonomously. When it acts on a user's behalf, it propagates and re-scopes the originating user's token rather than escalating to its own broader identity, so an agent's tool calls never carry more privilege than the user who invoked them. Like any external consumer, it reaches domain state only through published contracts, verified on every call.

## 3. Service-to-Service Authentication

All inter-service communication is authenticated using JWT. Services verify the token on every inbound request — trust is never implied by network location alone. Tokens are scoped to the calling service's identity and the permissions it requires, consistent with the Least-Privilege Boundaries principle. Service-to-service tokens are obtained through the OAuth2 client credentials flow described above.

## 4. Async and Event-Driven Identity

The JWT model above assumes a live synchronous call that can carry a bearer token. An asynchronous message or event cannot — there is no live token attached to a queued message the way there is to a request. Applying the same mechanism to async flows does not work, so identity for event-driven work is handled through two separate concerns, both of which still uphold the intent that trust is never implied:

- **The publish/consume connection**: whether a producer may publish to a channel and a consumer may subscribe is service-to-service trust for the broker connection. This need not use the OIDC/OAuth2 mechanism used for synchronous calls; whatever the messaging platform natively supports for authenticated connections is acceptable, provided it is genuinely authenticated — anonymous publish or consume is not permitted. This is a deliberate, scoped exception to the synchronous identity model, not a gap in it.
- **Attribution of the originating actor**: who or what caused an event to be published is carried as a plain identity reference in the event envelope — an actor identifier — not a cryptographically verifiable token. Authorization was already fully enforced at publish time; the reference exists so the consumer's resulting action can be attributed in logs and audit trails. It is not re-verified by the consumer.

Authorization happens once, at publish time, and is not revisited at consume time. A consumer acts on a published event without re-checking whether the originating actor's permissions have changed in the interim. This makes idempotent, retry-safe processing (see [Resilience by Design](architecture.md#6-resilience-by-design)) especially important for event consumers, since there is no re-authorization step to catch a redelivered or replayed message.

## 5. Token Lifecycle

JWTs are verified by services without a synchronous lookup to a central authority (see [ADR-0012](adrs/0012-jwt-for-service-to-service-authentication.md)); a self-contained token is valid until it expires. The following requirements keep that design safe:

- **Lifetime**: Service and user access tokens are short-lived and bounded. A leaked or compromised token's window is small by design. Exact lifetimes are an implementation choice.
- **Revocation**: Short lifetimes are the primary revocation mitigation, preserving the no-per-call-lookup design. Introspection or a deny-list is reserved for high-value or emergency cases, explicitly trading away the no-lookup property only where the value justifies the cost.
- **No sensitive data in claims**: A JWT payload is base64-encoded, not encrypted, and tokens are logged, cached, and forwarded. Claims carry identity and authorization scope only — never PII or secrets. This is the same prohibition enforced on log output, backstopped by the sensitive-data logging scan in Secure Pipeline Gates.
- **Key rotation**: Signing keys are rotated, and verification tolerates rotation by resolving the active key (key id / JWKS) rather than pinning a single static key.
- **Acquisition by path**: Service-to-service clients re-acquire a token through the client credentials flow when it expires — no refresh tokens. Interactive-user (OIDC) refresh tokens, where used, are stored securely.

## 6. Least Privilege Implementation

The Least-Privilege Boundaries architecture principle is implemented in practice through:

- **Service identities**: Each service runs under its own identity, scoped to the operations it performs
- **Database access**: Database users are scoped to the minimum operations required — a service that only reads does not hold write permissions
- **Secrets access**: Services are granted access only to the secrets they require, enforced through an approved platform secrets-management capability
- **Container permissions**: Service containers run as non-root users with no unnecessary capabilities
- **Scope minimization**: OAuth2 scopes and token claims carry only what a caller requires, verified by the least-privilege scope check in Secure Pipeline Gates

Secrets themselves are supplied through an approved platform secrets-management capability and injected at runtime via externalized configuration (see [Service Operability](engineering.md#6-service-operability)) — never committed to a repository, hardcoded, or baked into a build artifact. This applies to every runnable artifact, including any static front-end bundle, which must never embed a secret since it ships to the client.

## 7. Input Validation at Trust Boundaries

Every inbound payload — a synchronous request or an asynchronous event — is validated against its published contract schema before it is used, at every trust boundary, not only the outermost one. Where SAST and SCA catch injection risks in the codebase after the fact, this is the corresponding prevention-by-design requirement: a service never trusts that an inbound payload matches its contract simply because the caller is authenticated.

- **Schema validation**: Inbound payloads are validated against the OpenAPI or AsyncAPI specification that defines the interface (see [API and Contract Design](engineering.md#9-api-and-contract-design)). A payload that fails validation is rejected before it reaches application logic. This is implemented at the Interface Adapters layer of [Clean Architecture](architecture.md#1-clean-architecture) — the same layer that verifies tokens — so validation never leaks into domain or application logic.
- **Parameterized data access**: Persistence access uses parameterized or prepared statements exclusively. A string-concatenated or interpolated query is never an acceptable data-access pattern, regardless of implementation technology.

## 8. Encryption in Transit and at Rest

- **In transit**: TLS is required for every call — external traffic reaching the system and every internal, service-to-service call between domains — with no exemption for traffic that stays inside a cluster or network boundary. Network controls establish which paths are possible and authentication establishes who is on them; TLS ensures the traffic on those paths cannot be read or tampered with. The same expectation applies to messaging-broker connections: authenticated broker connections run over an encrypted transport, not a plaintext one with credentials layered on top.
- **At rest**: Every data store and secrets-management capability a system uses encrypts data at rest. The specific mechanism — transparent storage encryption, a managed key service, or a data store's native encryption — is a business-unit and platform choice tied to the concrete infrastructure in use. The requirement is that encryption at rest exists; the product and key-management scheme that provide it are not defined here.

## 9. Threat Modeling

Systems undergo threat modeling on a defined, recurring cadence, not only when a structural change happens to trigger one. Threats evolve independently of the codebase, so a one-time or change-triggered model goes stale. The interval, methodology (such as STRIDE or attack trees), and who performs it — the owning team, a central security function, or both — are a business-unit and organizational choice. What this guidance requires is that some recurring cadence is defined and actually followed, rather than left informal or skipped indefinitely.

## 10. Security Incident Response

A security incident — unauthorized access, credential compromise, or a suspected breach — follows the general incident-response process defined in the [operations document](operations.md), and adds two requirements a general availability incident does not have:

- **Evidence preservation before remediation, where feasible**: A security incident is not always safe to fix forward immediately the way an availability incident is. Acting before the scope is understood can destroy the evidence needed to determine what was actually accessed, so evidence is preserved first where it is feasible to do so.
- **A defined escalation path for legal, compliance, and disclosure obligations**: Distinct from the general on-call escalation chain. This guidance does not define retention or disclosure specifics — those vary by jurisdiction and data classification — only that a defined path for them exists.

## 11. Compliance Posture

Compliance requirements vary by product. Security practices should be designed to support the compliance posture required by the product adopting this guidance. Where specific compliance frameworks apply, the controls defined here serve as a baseline and additional requirements are layered on top, recorded in the adopting business unit's own documentation rather than here.
