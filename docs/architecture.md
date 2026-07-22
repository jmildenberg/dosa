# DOSA :: Documentation :: Architecture

> [DOSA](../readme.md) :: [Documentation](./readme.md) :: Architecture

This document defines the architectural principles for systems built on this guidance. Principles establish the structure, boundaries, and intent that guide how a system is designed, built, and evolved — they are the foundation against which technical decisions are made and evaluated. The [engineering document](engineering.md) defines the practices that put these principles into action. The [governance document](governance.md) defines the authority and accountability model that keeps both trustworthy over time.

## Principles

These principles define the structure, boundaries, and intent of a system built on this guidance. They establish the shared vocabulary we want teams to use as the guidance evolves.

Several lenses shape this guidance, and they are easier to hold together with a map of what each governs. Clean Architecture shapes the *internal* structure of a component, keeping business logic independent of frameworks and delivery mechanisms. Domain-Oriented Service Architecture shapes the *boundaries between* domains, aligning system structure with business capabilities and ownership. Continuous Delivery shapes *how change flows* into production. Twelve-factor shapes *runtime behavior*, so a component behaves predictably across environments. The first two are architecture principles defined here; the latter two are engineering practices, defined in the [engineering document](engineering.md). Read together they describe one system from the inside out: the shape of the code, the shape of the boundaries between teams, the way change moves, and the way the running system behaves.

Held together, these lenses can create pressure toward big design up front — boundaries, interface stability, and contract-first authoring all reward thinking ahead. That tension is intended to be a guide, not a gate: the draft-versus-published distinction in §5 Contract Compatibility is the release valve, allowing a contract to be shaped iteratively while still in draft rather than front-loading a perfect design before any code exists.

### 1. Clean Architecture

Clean Architecture is a software design philosophy adopted here as a foundational principle that organizes code into layers with one strict rule: dependencies point inward. By isolating core business logic from frameworks, user interfaces, databases, and external systems, it keeps the system easier to maintain, scale, and test.

- **Enterprise Business Rules**:
  Contains the core business entities and domain rules that represent enterprise-wide concepts and behaviors.
- **Application Business Rules**:
  Orchestrates use cases, coordinates the flow of data to and from the domain, and defines the contracts that support those use cases.
- **Interface Adapters**:
  Translates between the internal application model and the formats required by databases, APIs, messaging systems, and other external dependencies.
- **Frameworks and Drivers**:
  Represents the outermost technical layer where hosting, persistence, delivery mechanisms, and runtime-specific glue code live.

### 2. Domain-Oriented Service Architecture

Domain-Oriented Service Architecture is a pattern adopted here as a foundational principle that groups services into logical collections based on the business domain they support, rather than treating each technical component as an isolated microservice. The focus is on aligning system boundaries with business capabilities, team ownership, and operational intent.

- **Logical Boundary**:
  Codebases are structured around business capabilities so ownership, language, and responsibility remain clear within a domain.
- **Code Boundary**:
  A domain may contain multiple internal components, but it is treated as a cohesive service boundary with a stable external interface(s).
- **Repository Boundary**:
  Each domain is realized as its own repository, making the Code Boundary a physical, structurally enforced one. The repository is the unit that gives the boundary teeth: cross-domain coupling cannot be introduced silently, because reaching another domain means referencing its published packages rather than its source. This states the *why* — boundary enforcement; the concrete topology is deferred to engineering.
- **Inter-Domain Communication**:
  Domains should interact through well-defined interfaces - synchronous API contracts for request/response interactions and asynchronous events for decoupled, eventual workflows - preferring asynchronous, event-driven communication where it reduces runtime coupling between domains.
- **Operational Boundary**:
  Domains may rely on shared platform capabilities such as identity, file storage, or messaging, while still preserving clear logical separation through contracts, permissions, and ownership boundaries.
- **Data Boundary**:
  Each domain owns its data and is the single source of truth for it; other domains access that data through published contracts rather than reaching into another domain's storage directly.
- **Replicated and Cached Data**:
  Domains may keep cached or replicated copies of data they consume, but the owning domain remains the source of truth; such copies should be treated as read-only, refreshed through published contracts or events, and tolerant of eventual consistency. Maintaining such a copy by consuming another domain's published events — rather than calling it synchronously each time the data is needed — is the preferred pattern where the interaction does not require an immediate response; the local copy is a projection of the owning domain's data, never a second source of truth for it.
- **Exceptions and Special Access**:
  Exceptions to domain data ownership, such as analytics, migrations, or operational support access, may be approved by the owning domain only when they are narrow, time-limited, and do not establish a new integration standard. Exceptions that are long-lived, cross-domain in impact, or create a reusable pattern also require Technical Architecture Group (TAG) approval through the governance exception process.

A domain is realized by one or more runnable **roles**, each serving a distinct concern and included only when the domain actually needs it. Naming these roles gives teams a shared vocabulary for the components a domain is composed of, independent of the technology each is built with:

- **Domain-logic host**: The system of record — owns the domain's persistent state and core business rules, and exposes its published contracts. Every domain has one.
- **Asynchronous-processing host (worker)**: Background and event-driven work — event handlers, scheduled jobs, long-running tasks.
- **Presentation**: A user-facing experience, where the domain has one.
- **Request-shaping host (backend-for-frontend)**: Sits between a presentation surface and the domain, shaping and aggregating calls on its behalf and owning that surface's session and token custody (see the [security document](security.md)).
- **Agent-driven host**: An AI-agent interaction surface — tool use, orchestration, and LLM-backed workflows — where the domain exposes one.

These roles divide into two categories by whether they operate on domain state directly, and that determines what each may access:

- **In-process operators**: The domain-logic and asynchronous-processing hosts operate on domain state directly and share the same business rules and application logic as referenced dependencies, while still deploying on their own lifecycle.
- **External consumers**: The request-shaping and agent-driven hosts never touch domain state directly. They consume the domain's published contracts, calling it exactly as any other domain would, with no privileged internal access. This is what keeps them thin, and it is enforced structurally by the module dependency rules in the [engineering document](engineering.md#3-domain-structure).

Because the agent-driven host is backed by non-deterministic model behavior, failure — timeouts, malformed output, unreliable results — is a routine operating condition for it rather than an exceptional one, which its design and testing must account for (see [Resilience by Design](#6-resilience-by-design)).

### 3. Observability by Design
 
Observability by Design is the principle that a system should be instrumented as part of its architecture rather than treated as a post-deployment concern. Logs, metrics, and traces are part of how a distributed system explains its behavior, supports diagnosis, and validates operational health. The same expectation extends to the delivery process that produces the system: the pipeline that builds, tests, and promotes a change should be observable in its own right, so that delivery is diagnosable rather than opaque — instrument-not-afterthought applies to the pipeline as much as to the runtime.

- **Logs**:
  Structured application events should provide enough context to explain meaningful state changes, failures, and decisions.
- **Metrics**:
  System and application measurements should expose the health, performance, and behavior of important workloads over time.
- **Traces**:
  Request and workflow tracing should make it possible to follow behavior across services, processes, and dependency boundaries.

### 4. Least-Privilege Boundaries

Least-Privilege Boundaries is the principle that users, services, processes, and integrations should receive only the minimum access required to perform their intended role. It keeps security boundaries explicit and reduces the blast radius of failures, misuse, and compromise. The [security document](security.md) defines the practices that implement and verify this principle.

- **User and Service Access**:
  Identities should be scoped to the responsibilities they actually perform rather than broad default permissions.
- **Dependency Access**:
  Access to databases, queues, storage, and external APIs should be constrained to the minimum capabilities required by the consuming components.
- **Boundary Preservation**:
  Security permissions should reinforce architectural boundaries instead of bypassing them for convenience.

### 5. Contract Compatibility

Contract Compatibility is the principle that the published interfaces between domains and services should remain stable for their consumers as the system evolves. It is what keeps the bounded contexts and domain-oriented service boundaries trustworthy over time, ensuring that internal change does not silently break the components that depend on a contract. Contracts take more than one form - synchronous API contracts (request/response interfaces) and asynchronous event contracts (message and event schemas) - and both are published interfaces subject to the same compatibility expectations.

- **Contract-First Authoring**:
  Contracts are authored as the source of truth before the application code or schema that fulfills them, and code is verified to conform, rather than the contract being generated as a byproduct of code. Contract-first concerns *authoring order and source of truth*, not a perfect first draft. A draft (pre-v1) contract is expected to be shaped iteratively — breaking changes are normal and the compatibility guarantees below are not yet binding, because no consumer depends on it. Once a contract is published (v1 and beyond) and consumers depend on it, Backward Compatibility, Explicit Versioning, and breaking-change detection apply in full.
- **Discoverability**:
  Published contracts must be discoverable, not only compatible and versioned. A contract that consumers cannot find is, in practice, not reusable; published contracts should be surfaced through a central catalog so teams can see what already exists before they build against it.
- **Backward Compatibility**:
  Changes to a published contract should preserve existing consumers, favoring additive change over breaking change wherever possible.
- **Explicit Versioning**:
  When a breaking change is unavoidable, it should be expressed through deliberate, discoverable versioning rather than modification.
- **Deprecation Lifecycle**:
  Retiring a contract should follow a clear path of announcement, overlap, and removal so consumers can migrate without disruption.
- **Compatibility Verification**:
  Published contracts should be versioned and tested in ways that make breaking changes visible before release, whether through automated contract tests, schema validation, or other compatibility checks.

### 6. Resilience by Design

Resilience by Design is the principle that a system should be built to anticipate and contain failure rather than assume availability. The domain boundaries established by Domain-Oriented Service Architecture and the preference for asynchronous, event-driven communication already reduce runtime coupling significantly; this principle makes explicit the design expectations that complete the resilience model.

- **Failure Isolation**:
  Domain boundaries serve as the primary containment unit for failure; a fault within one domain should not propagate unchecked into consumers or dependencies. Designing for this means treating inter-domain calls as inherently unreliable and making failure an expected, handled outcome rather than an exception.
- **Idempotency**:
  Operations exposed to retry or broker redelivery must produce the same result when executed more than once. Idempotency is the prerequisite that makes retry safe regardless of whether it is triggered at the call site or by messaging infrastructure.
- **Timeout and Retry Contracts**:
  Synchronous inter-domain and external calls should define explicit timeout and retry limits rather than inheriting platform defaults or blocking indefinitely. For asynchronous flows, retry is delegated to the broker through redelivery and dead-letter handling; services consuming those flows are responsible for idempotent processing.
- **Graceful Degradation**:
  Services should define their behavior under partial failure — returning cached results, reduced functionality, or explicit unavailability — rather than propagating errors to consumers. Expected degraded behavior is a design decision, not a runtime improvisation.
- **Backpressure and Load Shedding**:
  Services should protect themselves under excess load by applying backpressure or shedding work explicitly rather than accepting unbounded demand and failing under it. The decision that a service will shed load is architectural, and the *signal* it raises — the wire contract by which a callee tells a caller to back off — is a contract concern that callers must be able to honor; the *policy* behind it, when and how much to shed, is an implementation detail.
- **Recovery Validation**:
  Resilience behavior should be validated through testing, not assumed. Failure scenarios — dependency outages, latency spikes, message loss — should be exercised deliberately as part of the verification strategy, consistent with the testing expectations established in Contract Compatibility.

## Glossary

- **Agent-Driven Host**: A domain role providing an AI-agent interaction surface — tool use, orchestration, and LLM-backed workflows. An external consumer of the domain's published contracts, not an operator of domain state.
- **Backpressure**: A mechanism by which a service signals that it cannot accept additional work at the current rate, allowing callers to slow or shed load rather than overwhelming the service.
- **Backend-for-Frontend (BFF)**: A domain role, also called the request-shaping host, that sits between a presentation surface and the domain, shaping and aggregating calls on its behalf and owning that surface's session and token custody.
- **Blast Radius**: The scope of impact when a failure, security incident, or error propagates through the system.
- **Boundary**: An explicit separation that defines the ownership, responsibility, and interface of a domain. Takes several forms: logical, code, repository, data, and operational.
- **Breaking Change**: A change to a published contract that is incompatible with existing consumers.
- **Consumer**: A domain or service that depends on another domain's published contract.
- **Contract**: A published interface between domains or services. Takes two forms: synchronous API contracts (request/response) and asynchronous event contracts (message schemas).
- **Dead-Letter Queue (DLQ)**: A queue that receives messages that could not be successfully processed after exhausting retry attempts, enabling inspection and recovery without message loss.
- **Developer Portal (Service Catalog)**: The single, central, discoverable index of the organization's published contracts and services and their metadata (ownership, version, status). This guidance also refers to it as the *central catalog*, *contract catalog*, or *service catalog* — all name the same capability, of which a developer portal (e.g. Backstage) is one realization.
- **Discoverability**: The property that published contracts can be found by potential consumers — typically through the central developer portal (see *Developer Portal*) — so that teams can see what already exists before building against it.
- **Domain**: A logical grouping of services and capabilities aligned to a single business capability, with clear ownership over its code, data, and external interfaces.
- **Event-Carried State Transfer**: The pattern of maintaining a local read-only copy of another domain's data by consuming its published events, rather than calling it synchronously each time the data is needed. The local copy is a projection, never a second source of truth.
- **Eventual Consistency**: A consistency model in which replicated or cached data will converge to the source of truth over time but may be temporarily stale.
- **External Consumer**: A domain role that never operates on domain state directly and reaches the domain only through its published contracts — the request-shaping and agent-driven hosts. Contrast with *In-Process Operator*.
- **Idempotency**: The property of an operation that produces the same result regardless of how many times it is executed with the same input.
- **In-Process Operator**: A domain role that operates on domain state directly, sharing the domain's business rules and application logic as referenced dependencies — the domain-logic and asynchronous-processing hosts. Contrast with *External Consumer*.
- **Published Interface**: A contract explicitly made available by a domain for consumption by other domains. Both API contracts and event schemas are published interfaces.
- **Role**: A runnable component of a domain serving a distinct concern — domain-logic host, asynchronous-processing host, presentation, request-shaping host, or agent-driven host. A domain includes only the roles it needs. Roles are either *in-process operators* or *external consumers*.
- **Source of Truth**: The authoritative owner of a piece of data, responsible for its correctness; other domains access that data through the owning domain's published contracts.
