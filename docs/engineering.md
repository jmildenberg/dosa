# DOSA :: Documentation :: Engineering

> [DOSA](../readme.md) :: [Documentation](./readme.md) :: Engineering

This document defines the engineering practices for building, delivering, and operating software in systems built on this guidance. Where the [architecture document](architecture.md) establishes the principles that shape a system's structure and boundaries, this document defines how those principles are put into practice by development teams.

The practices here are organized to follow the natural flow of software development — from how code is written and reviewed, through how it is structured, packaged, and delivered, to how it is operated and secured. Each practice is grounded in the architectural principles established in the architecture document and should be read in that context. The [governance document](governance.md) defines how these standards are maintained, enforced, and evolved over time.

## Practices

### 1. Testing

#### Test Driven Development

Test Driven Development is a practice where tests are written before the code that satisfies them. The discipline follows a short, repeating cycle: write a failing test that expresses the intended behavior, write the minimum code to make it pass, then refactor under the protection of that test. This cycle keeps code purposeful and prevents over-engineering.

Within this architectural approach, TDD is particularly well-suited to the domain and application layers established by [Clean Architecture](architecture.md#1-clean-architecture). Because those layers have no external dependencies, they can be exercised directly in tests without infrastructure, making the test-first cycle fast and focused. TDD at the domain layer produces code shaped by its intended behavior rather than by implementation convenience.

Tests written through TDD serve as living documentation of intent — they express what the system is supposed to do, not just verify that it does it.

#### Testing Trophy

The Testing Trophy is a model for thinking about the composition and balance of a test suite. Rather than investing heavily in a large base of isolated unit tests, the Trophy favors integration tests as the primary investment, on the basis that they provide the highest confidence per test written — exercising real behavior across multiple components without the brittleness of end-to-end tests.

The four layers, from foundation to peak:

- **Static Analysis**: 
  Type checking, linting, and code analysis catch errors before tests run. The broadest and cheapest layer.
- **Unit Tests**: 
  Isolate and verify the **transformations and decisions** — the pure calculation and branching — that live in domain and application logic. Unit tests exist for **fault isolation**: a failure points at a single transformation or decision rather than a collaboration. These pure units are the *target* of unit testing rather than every class arbitrarily, and because they are pure they are tested with **direct inputs and outputs** — heavy mocking is a smell that a unit with dependencies is being unit-tested, which the integration band covers better.
- **Integration Tests**: 
  Verify that components work correctly together — the primary investment. These tests exercise real behavior across domain logic, application orchestration, and infrastructure adapters without requiring a fully deployed system. Integration and contract tests trade *fault isolation* for far greater *line-of-code coverage*: a single test exercises wide swaths of real collaboration. That breadth is **architecturally** valuable — it verifies the boundaries and wiring that unit tests deliberately skip — which is why the Trophy weights investment here.
- **End-to-End Tests**: 
  Validate critical user paths through the fully deployed system. Valuable for high-confidence verification of key scenarios but expensive to maintain; used selectively.

This aligns naturally with Clean Architecture — integration tests that exercise use cases from the application layer down provide meaningful coverage without over-specifying internal behavior.

#### Test Classification

The Testing Trophy defines the philosophy; this classification gives teams the concrete vocabulary to apply it consistently. Without shared naming, the meaning of "integration test" varies by developer — these categories make the scope of any test unambiguous.

| Stage | Category | Trophy Layer | Runtime Dependencies | Purpose | Examples |
|---|---|---|---|---|---|
| CI | **Architectural** | — | None — validates code structure, not runtime behavior | Validate architectural rules and design constraints: layer dependencies, domain isolation, structural conventions | ArchUnit, ArchUnitNet, import-linter, dependency-cruiser, ts-arch |
| CI | **Unit** | Unit | None — isolates a single transformation or decision (pure logic) | Isolate and verify the transformations and decisions in domain/application logic, for fault isolation | JUnit, xUnit/NUnit, pytest, Jest/Vitest (direct input/output assertions; mocks only where a collaborator is genuinely unavoidable) |
| CI | **Integration** | Integration | One infrastructure element, which is the subject of the test; nothing is stubbed | Verify collaboration between code and a single external resource provisioned by the system | Testcontainers, repository↔DB tests, message-broker round-trips |
| CI | **SubSystem** | Integration | All direct infrastructure dependencies live; upstream/downstream services stubbed | Validate a full domain slice in-process, within its own boundary | @SpringBootTest + MockMvc, ASP.NET Core WebApplicationFactory, FastAPI TestClient, supertest |
| CD | **System** | End-to-End | The deployed system live; external/ecosystem dependencies may be stubbed or sandboxed | Validate the deployed system in isolation, post-deployment — the highest tier housed within a domain repository | Smoke tests, acceptance/verification tests, API tests, browser/UI tests |
| CD | **End to End** | End-to-End | Everything live, nothing stubbed — the system plus its surrounding ecosystem | Validate a complete user journey through the deployed system and its ecosystem | Playwright, Selenium, Cypress, BrowserStack |

Integration and SubSystem both sit within the Trophy's integration band and together represent the primary test investment. Architectural sits outside the Trophy model — it runs like a unit test (fast, no runtime dependencies) but validates the structure of the codebase rather than its behavior, extending the Trophy with a category the model does not explicitly cover.

Two boundaries cut across these categories:

- **CI vs CD**: The CI categories (Architectural, Unit, Integration, SubSystem) run pre-deployment against code and provisioned dependencies; they gate the merge and build. The CD categories (System, End to End) run post-deployment against a live, deployed environment.
- **System vs End to End**: Both sit in the End-to-End Trophy layer, distinguished by *how much of the world is live*. System exercises the deployed system in isolation — downstream and third-party dependencies may be stubbed or pointed at sandboxes — and includes browser/UI, smoke, and API tests. End to End exercises the full journey with the surrounding ecosystem live and nothing stubbed. The browser is a delivery mechanism for both; what differs is scope, not tooling.
- **Where the tests live**: Architectural through System are housed within the domain repository, so System is the highest tier a domain repo contains. End to End spans multiple systems, so its tests are owned and run *outside* any single domain repository — above the individual repo, against the assembled ecosystem.

#### Testing the Agent-Driven Host

The classification above assumes deterministic behavior — the same input produces the same output, verifiable by assertion. The [agent-driven host](architecture.md#2-domain-oriented-service-architecture) does not fit that assumption, because its behavior is backed by non-deterministic model output. It is tested at the same tiers as any other host, but its behavioral tests are evaluated differently: against a **golden dataset** — a fixed set of representative inputs paired with an expected *outcome shape* (which tools should be invoked, what class of response is acceptable) rather than an exact expected output — scored by a **rubric**, whether through an automated grader, a human reviewer, or both, with a threshold score gating the suite rather than an all-or-nothing pass. The rubric, dataset composition, and grading mechanism are choices for the owning team; what this guidance requires is that the agent-driven host has *some* systematic, repeatable evaluation in place of deterministic assertions rather than being exempted from testing.

### 2. Code Review

Code review is the primary human gate for correctness, architectural conformance, and domain boundary respect. Automated tooling assists with conformance checking, but review is where judgment is applied — catching what tools cannot.

#### Branching Strategy

This guidance specifies trunk-based development. All work branches from and merges back to main, keeping the integration surface small and integration problems visible early. This model requires discipline around keeping main releasable at all times. Trunk-based development is the shared standard across the organization; the specific release flow built on top of it — how release candidates are cut and promoted — is a team-level choice and varies by group.

#### Pull Request Scope

Pull requests should be small and purposeful, scoped to the user story or work item they represent. A PR that is difficult to review in a single sitting is a signal that it covers too much. Small PRs reduce review friction, make issues easier to isolate, and keep the integration history readable.

#### Review Expectations

Reviewers are responsible for evaluating:

- **Correctness**: Does the code do what it is intended to do, and is that intent evidenced by tests? Review judges demonstrated behavior, not asserted behavior.
- **Architectural Conformance**: Does the code respect layer dependencies, domain boundaries, and the principles established in the architecture document? Automated tooling assists here, but reviewers apply judgment where tooling cannot.
- **Contract Changes**: Does the PR modify a published interface — an API contract or event schema? If so, it should be treated with the scrutiny of Contract Compatibility — is the change backward compatible, versioned appropriately, and deliberate?
- **Domain Boundary Respect**: Does the PR introduce any cross-domain data access or coupling that bypasses published contracts?

#### Merge Requirements

A pull request may not be merged until:

- At least one independent approver — never the author — has reviewed and approved. This is a fixed floor: the count is never zero, preserving the separation-of-duties baseline. Additional reviewers are warranted in proportion to the change's blast radius — published contracts, cross-domain coupling, and security-sensitive paths weigh heavier and should draw the right additional reviewers (for example, the domain owner routed via `CODEOWNERS`). Ordinary internal-logic changes sit at the floor.
- The PR build passes, including all test cases

### 3. Domain Structure

Domain structure is the physical expression of [Domain-Oriented Service Architecture](architecture.md#2-domain-oriented-service-architecture) in code. The repository is the unit that realizes a domain's code boundary, making cross-domain coupling visible and deliberate.

#### One Repository Per Domain

Each domain is realized as its own repository, as decided in [ADR-0004](adrs/0004-one-repository-per-domain.md). The repository *is* the physical, structurally enforced expression of a domain's code boundary — a dependency on another domain cannot be introduced without explicitly referencing its published packages. This topology has concrete mechanics:

- **Self-contained repository**: A domain's code, contracts, schema, and pipeline definition live together in a single repository owned by one team.
- **Independent pipeline**: Each repository carries its own CI/CD pipeline and releases on its own lifecycle, decoupled from other domains.
- **Shared libraries via the artifact repository**: Common reusable libraries and platform packages are consumed as standard package dependencies from an artifact repository, never by reaching across repositories into another domain's source.
- **Coordinated cross-repo PRs**: A change that spans domains is made as coordinated pull requests across the affected repositories, each reviewed by its owning team, rather than as a single edit that crosses a boundary.

#### Repository and Project Layout

Each domain follows the same **canonical layout** — one recognizable structure regardless of implementation language. Two things are deliberately separated: the **folder structure and names are standardized** (kebab-case, the same areas in the same places in every language — the navigational map), while the **build-artifact, assembly, and package names inside them stay idiomatic** to each ecosystem (a `modules/domain/` folder may contain a `ToDo.Domain` project). A domain includes only the areas it needs:

```text
<domain>/                             # one repository per domain; kebab-case, named for the domain
├── docs/
│   ├── adrs/                         # this domain's architecture decision records
│   ├── architecture/                 # design docs for this domain
│   ├── contracts/                    # OpenAPI + AsyncAPI specs — source of truth, contract-first
│   └── operations/                   # runbooks, on-call docs, postmortems
├── databases/
│   └── <data-source>/
│       ├── migrations/               # schema-first, versioned migrations (one dir per data source)
│       └── seeds/                    # reference/seed data
├── deploy/
│   ├── docker/                       # image build assets per host; local dev compose
│   └── helm/                         # the domain's deployment chart
├── modules/                          # all code areas (standardized root across every language)
│   ├── domain/                       # entities + rules — no outward dependencies
│   ├── application/                  # use cases / orchestration — depends on domain
│   ├── contracts/
│   │   ├── contracts-api/            # sync (OpenAPI) types
│   │   └── contracts-messaging/      # async (AsyncAPI) types
│   ├── clients/
│   │   └── clients-api/              # consumer SDK for calling this domain — depends on contracts
│   ├── infrastructure/               # persistence, messaging, external adapters — depends on app + domain
│   ├── frontend/                     # presentation role (optional) — own source/build
│   └── hosts/                        # deployable entry points, one folder per host role
│       ├── host-api/                 # domain-logic host       (in-process operator)
│       ├── host-worker/              # asynchronous host       (in-process operator)
│       ├── host-web/                 # request-shaping / BFF    (external consumer)
│       ├── host-agent/               # agent-driven host        (external consumer)
│       └── host-shared/              # cross-cutting host boilerplate (auth, error handling, logging)
├── tests/
│   ├── architectural-tests/          # repo-wide dependency-rule / fitness-function checks
│   └── system-tests/                 # deployed-environment tests: acceptance, smoke, browser
├── tooling/                          # local-dev helpers, git-hook installers, scripts
└── README.md
```

The core code areas carry these responsibilities:

| Module | Purpose |
|---|---|
| **Domain** | Core business entities and domain rules. No dependencies on other modules. |
| **Application** | Use cases and orchestration. Depends on Domain. |
| **Contracts** | Published interface objects — request, response, and event schema types that define what this domain exposes. Authored as the source of truth, independent of Domain internals. |
| **Clients** | Consumer-facing implementation for invoking this domain. Depends on Contracts. Other domains reference this module to call this domain. |
| **Infrastructure** | Adapters for external services, messaging, and persistence. Depends on Application and Domain. |
| **Host** | Deployable entry points — one per host role (API, worker, request-shaping, agent-driven). Dependencies vary by role (see Roles and Hosts). |
| **Database** | Schema migration scripts. Owns the database schema as the source of truth for persistence. |

Folder names are standardized as shown; the build-artifact, assembly, or package name inside each folder follows the implementation technology's conventions. The layout above is the base structure — any area may be extended into multiple focused modules using its name as a prefix, as Contracts and Clients already are (split by protocol) and as Infrastructure may be (persistence and messaging). The dependency rules that apply to a base module apply equally to all of its extensions.

#### The Same Layout Across Technologies

The folder structure is identical across languages; only the build-artifact identity inside each folder differs. Using the real artifact names from the two reference stacks, with the standardized folders applied:

| Canonical folder | C# — project inside (`.slnx`) | Java — module |
|---|---|---|
| `modules/domain/` | `ToDo.Domain.csproj` | `domain` |
| `modules/application/` | `ToDo.Application.csproj` | `application` |
| `modules/contracts/contracts-api/` | `ToDo.Contracts.Api.csproj` | `contracts-api` |
| `modules/contracts/contracts-messaging/` | `ToDo.Contracts.Messaging.csproj` | `contracts-messaging` |
| `modules/clients/clients-api/` | `ToDo.Clients.Api.csproj` | `clients-api` |
| `modules/infrastructure/` | `ToDo.Infrastructure.csproj` | `infrastructure` |
| `modules/hosts/host-api/` | `ToDo.Host.Api.csproj` | `host-api` |
| `modules/hosts/host-worker/` | `ToDo.Host.Worker.csproj` | `host-worker` |
| `modules/hosts/host-web/` | `ToDo.Host.Web.csproj` | `host-web` |
| `modules/hosts/host-agent/` | `ToDo.Host.Agent.csproj` | `host-agent` |
| `modules/hosts/host-shared/` | `ToDo.Host.Shared.csproj` | `host-shared` |
| `modules/frontend/` | `ToDo.Frontend` (Angular) | `frontend` |

The folder is the standard; the assembly or package name — `ToDo.Domain`, or a Maven `groupId:artifactId` — stays idiomatic, so no language renames its build artifacts.

**Tests.** Repo-wide tests are standardized under `tests/` (`architectural-tests/`, `system-tests/`). Per-module unit tests are the one placement build tooling dictates, and they stay idiomatic: in Java they live inside each module (`modules/<area>/src/test/...`); in C# they are separate test projects (a test cannot share an assembly with its subject), conventionally under `tests/` as `<Project>.Tests`. The folder map is consistent; where a module's own unit tests physically sit is not.

The **C# reference already realizes this layout** — `modules/` with the standardized folder names (keeping its idiomatic `.csproj` names), `host-shared` for the cross-cutting boilerplate, and the agent-driven `host-agent` — so it is a valid starting point to clone. The **Java reference** predates the standardized layout and its alignment is still pending: it groups hosts, contracts, and clients under parent folders and moves its repo-wide tests up to `tests/`, and it renames its `*-defaults` host boilerplate to `host-shared` and adds the agent-driven host to match.

#### Roles and Hosts

The [roles](architecture.md#2-domain-oriented-service-architecture) a domain may contain map to the `hosts` and `presentation` areas above, included only when the domain needs them. What distinguishes them is what each may access:

| Role | May access |
|---|---|
| **Domain-logic host** (in-process operator) | Domain, Application, Infrastructure directly |
| **Asynchronous-processing host** (in-process operator) | Domain, Application, Infrastructure directly |
| **Presentation** | None — reaches the domain only via the request-shaping host |
| **Request-shaping / BFF host** (external consumer) | Contracts and Clients only |
| **Agent-driven host** (external consumer) | Contracts and Clients only |

The operator/consumer distinction is structurally enforced by the dependency rules below. In-process operators (domain-logic and worker hosts) reference Domain, Application, and Infrastructure directly. External consumers (request-shaping and agent-driven hosts) may reference only the Contracts and Clients modules — the same published-interface access any other domain has — so they cannot reach domain state even in principle. An external-consumer host that referenced Domain or Infrastructure would violate the dependency rule and should fail an Architectural test (see [Testing](#1-testing)).

The presentation role is typically served by the request-shaping host rather than running as a standalone entry point, in which case it is built and deployed with that host rather than scaling independently. Hosting it separately is a domain-level choice where there is a specific reason, not a requirement either way.

#### Dependency Rules

Dependencies within a domain follow the Clean Architecture rule — they point inward:

- **Domain** has no dependencies on other modules
- **Application** depends on Domain
- **Contracts** is independent — it carries no Domain internals so consumers can reference it without pulling in business logic
- **Clients** depends on Contracts
- **Infrastructure** depends on Application and Domain
- **Host** depends according to its role — an in-process-operator host (API, worker) depends on Infrastructure and Application; an external-consumer host (request-shaping/BFF, agent-driven) depends only on Contracts and Clients, never on Domain, Application, or Infrastructure
- **Database** is standalone — schema migration scripts with no module dependencies

#### Inter-Domain Dependencies

When one domain depends on another, it references the producing domain's Clients module. This enforces that all inter-domain communication flows through published contracts — a domain cannot reach into another domain's internals, only its published interface.

#### Shared Platform

Cross-cutting concerns that span domains — identity, messaging infrastructure, and similar platform capabilities — may be organized across one or more shared platform repositories. Domains interface with platform services the same way they interface with other domains: through published Contracts and Clients packages. Common reusable libraries — base types, utilities, and shared primitives — are distributed through an artifact repository and consumed as standard package dependencies.

### 4. Containerization

Containerization is the standard packaging and delivery mechanism for all runnable artifacts in systems built on this guidance. Container images are built to the OCI standard, making them portable across compliant runtimes — Docker, containerd, Podman, or otherwise. Kubernetes is the deployment target; images are built with that runtime contract in mind.

#### One Image Per Host

Each Host module produces its own container image. A domain with both an API host and a worker host produces two independent images, each deployable and scalable on its own lifecycle. Hosts are never bundled into a shared image.

#### Image Build Standards

Container images should be built using multi-stage builds — separating the build environment from the runtime image. The runtime image should be minimal, containing only what is required to run the artifact. Images should run as a non-root user by default.

#### Image Distribution

Built images are published to a container registry and tagged in a way that makes the version and origin traceable. The same image artifact that passes CI is the artifact that is deployed — images are never rebuilt per environment. Environment-specific behavior is expressed through configuration, not through image variants.

#### Kubernetes Alignment

Images should be built with the Kubernetes runtime contract in mind:

- **Health endpoints**: Hosts must expose liveness and readiness endpoints for Kubernetes probes.
- **Graceful shutdown**: Hosts must handle termination signals and drain in-flight work before exiting.
- **Statelessness**: Hosts carry no local state; all state lives in external dependencies.
- **Resource boundaries**: Images should be deployable with explicit CPU and memory limits.

### 5. Continuous Delivery

Continuous Delivery is the practice of keeping every change to main in a releasable state and automating the path from code to production through a defined environment progression. The pipeline is the sole mechanism for building and deploying artifacts — nothing is built or deployed outside of it.

#### Pipeline Structure

Every merge to main triggers a CI pipeline that:

- Builds all modules
- Runs all Architectural, Unit, Integration, and SubSystem tests — the CI test categories defined in Testing. System and End to End tests run post-deployment against a live environment, not in CI.
- Publishes versioned container images to the registry on success
- Publishes the domain's authored contracts and catalog metadata to the central developer portal alongside the image (see API and Contract Design)

The same image produced by the pipeline is the image that flows through all environments — it is never rebuilt between stages.

#### Environment Progression

Changes flow through three standard environments, with an optional fourth for user acceptance testing:

| Environment | Trigger | Purpose |
|---|---|---|
| **Development** | Merge to main | Continuous integration target; validates every change against a live environment |
| **Staging** | Release candidate promoted | Pre-production staging; accepts changes promoted for release |
| **UAT** *(optional)* | Approved from Staging | User acceptance testing; release candidate validation. Used where a dedicated acceptance stage is required |
| **Production** | Approved from Staging or UAT | Live system |

Each promotion toward Production requires an explicit approval gate — from Staging directly to Production, or from Staging through UAT to Production where the optional UAT stage is used. Promotion from main to Development is fully automated.

#### Release Process

Commits are merged to main continuously. When a set of changes is ready for release, a release candidate is promoted into the Staging environment, beginning the promotion pipeline. The specific mechanism for cutting and promoting a release candidate is a team-level choice, as established in the branching strategy under Code Review.

#### Deployment Strategy

Deployments to all environments use Kubernetes rolling updates — new instances are brought up before old instances are terminated, maintaining availability throughout the deployment. Rollback behavior is a delivery-platform responsibility and must be explicitly defined, whether through automation or an operational procedure. Progressive or risk-managed rollout beyond a plain rolling update is achieved through feature flags (see Progressive Delivery below) rather than specialized blue-green or canary deployment infrastructure, unless a domain has a specific need that rolling updates plus flags cannot meet.

#### Declarative Deployment

Deployment state is expressed declaratively in version control and reconciled to each environment by the delivery platform, rather than pushed imperatively. A domain's runtime manifests are packaged as a single versioned unit — versioned together with, and published alongside, the images it deploys — and each environment records which version it should be running. Deploying is a change to that recorded version; the platform converges the environment to match.

- **Independent scaling, coordinated release**: each Host still produces its own image and scales on its own lifecycle (see Containerization). A *release*, however, bundles a tested combination of the domain's host images and their manifests into one versioned unit that is promoted and rolled back together — independent scaling at runtime, coordinated versioning at release.
- **Roll back as a unit**: because a domain releases as one versioned unit, reverting the recorded version reverts all of the domain's hosts to their previous tested combination in a single action, never one host in isolation. Rollback is always re-recording a previous version, never a hand-patched change to a running environment.
- The recorded-version change is itself the promotion audit record (see Promotion Audit below).

The practice is tool-agnostic. A Helm chart published as a versioned OCI artifact, reconciled by a GitOps controller such as Argo CD, is one conformant realization — named as an example, not mandated.

#### Progressive Delivery

Incomplete or risky work merges to main behind a feature flag rather than living on a long-lived branch, and new behavior is rolled out progressively rather than all at once. This is what makes the trunk-based model (see Code Review) safe in practice, and it reinforces the immutable-artifact rule: the same deployed image behaves differently based on flag state, not because a different build was shipped. Flag-guarded code paths are removed once the flag has served its purpose, so flags do not accumulate as permanent branching in the code.

#### Pipeline Tooling

The practices defined here are pipeline-agnostic and do not depend on any specific CI/CD platform. Azure DevOps is one conformant implementation; the same practices apply equally to other pipeline tooling.

#### Pipeline Observability

The delivery pipeline is itself observed, not only the running system. Pipeline runs and promotions emit telemetry — events and metrics — so build and deployment failures, durations, and delivery metrics over time are visible for troubleshooting and dashboards. This telemetry is diagnostic: sampled, droppable, and mutable like any runtime signal, and it reuses the observability backend established in Observability Implementation where it fits.

#### Promotion Audit

Each promotion emits an auditable record, distinct from pipeline observability. Where observability data is for diagnosis and may be dropped, the audit record is accountability evidence and is retained. Each record captures the actor, the artifact version promoted, the approval that authorized it, the target environment, and the timestamp. The audit trail attaches to the approval gates defined in Environment Progression.

### 6. Service Operability

Service Operability defines how services are built to be deployed, configured, and operated reliably across environments. These practices draw from twelve-factor app principles, adapted to a containerized, Kubernetes-based deployment model.

#### Externalized Configuration

All configuration is externalized from the image. A service reads its configuration from the environment at startup — no environment-specific values, connection strings, or credentials are baked into the image or the codebase.

- **Non-sensitive configuration** is supplied via environment variables or Kubernetes ConfigMaps
- **Sensitive values** — credentials, API keys, connection strings — are supplied via an approved platform secrets-management capability and injected at runtime
- The same image runs in every environment; environment differences are expressed entirely through configuration

#### Replaceable Backing Services

Databases, queues, storage, and external APIs are treated as attached resources — connected via configuration, not hardcoded into the service. A backing service should be replaceable by changing configuration without requiring a code change or a new image. This keeps services environment-agnostic and supports parity across Development, Staging, UAT, and Production.

Config-binding is necessary but not sufficient: a service can externalize its connection string and still be impossible to move if its code depends on a vendor-only feature. Replaceability is therefore also a design-time discipline:

- **Prefer standards-based interfaces** — SQL, S3-compatible object APIs, AMQP, JDBC/ODBC, OpenTelemetry. Building against a standard is what makes a backing service genuinely *replaceable* rather than merely *rebindable*.
- **Isolate proprietary capability behind an adapter** — where a vendor-specific feature is used, it is a deliberate choice confined to the Interface Adapters / Frameworks layer of [Clean Architecture](architecture.md#1-clean-architecture), so it does not leak into domain or application logic and the migration blast radius stays contained.
- **Be honest about the limits** — full agnosticism is not always achievable. Where no standard exists, the aim is *contained, deliberate* coupling, not a fantasy of universal portability.

This is complementary to but distinct from Dev/Prod Parity: parity says use the same backing-service *type* across environments; this practice says do not lock to that type's *proprietary features*. Both hold at once.

#### Dev/Prod Parity

The gap between development and production environments should be kept as small as possible. The same image artifact flows through all environments as established in Continuous Delivery. Backing service types should be consistent across environments — using a lightweight substitute in development that differs from production in behavior introduces risk that only surfaces in later environments. This parity expectation applies consistently across Development, Staging, UAT, and Production.

#### Local Development Parity

An engineer should be able to run a domain's full stack — or a close approximation of it — locally, resembling production closely enough that "works on my machine" is not a recurring failure mode. This matters more where a domain spans several hosts, and potentially several implementation technologies, in one repository (see Roles and Hosts): the local environment has to stand up every host and its backing services together, using the same backing-service types as production per Dev/Prod Parity. Containerized local environments — a shared compose definition or equivalent — are the usual means; the specific tooling is a team choice.

#### Operational Resilience

Operational Resilience is the engineering practice complement to the [Resilience by Design](architecture.md#6-resilience-by-design) architecture principle. It covers how services implement and expose their runtime health:

- **Liveness**: Services expose a liveness endpoint that indicates whether the process is healthy and should continue running.
- **Readiness**: Services expose a readiness endpoint that indicates whether the service is prepared to accept traffic. A service that is starting up, warming up, or temporarily degraded should report not ready.
- **Graceful Shutdown**: Services handle termination signals by stopping acceptance of new work, completing in-flight requests within a defined drain window, and releasing resources cleanly before exiting.
- **Timeout and Retry Behavior**: Outbound calls to backing services and upstream dependencies implement explicit timeouts and retry policies consistent with the Timeout and Retry Contracts defined in the Resilience by Design principle.
- **Idempotency**: Operations exposed to retry or message redelivery must be designed to produce the same result when executed more than once. Idempotency is a conscious design decision for any operation that may be retried at the call site or redelivered by messaging infrastructure. No single mechanism is mandated — the right fit depends on the operation's own shape, and the common techniques trade off roughly from least to most trust-dependent:
  - **Naturally idempotent operations** — model the operation as setting absolute state rather than applying a delta, so repetition has no additional effect and there is nothing to track
  - **Domain-level unique key** — a client-supplied, domain-meaningful identifier enforced by a database uniqueness constraint, so a repeat is rejected by the constraint itself rather than an ephemeral lookup
  - **Optimistic concurrency** — a version or concurrency token on the resource; a write that does not match the expected current version is rejected as a conflict
  - **Client-supplied idempotency key** — a general-purpose fallback for operations that fit none of the above: the service records the key against its result and returns the original result if the key reappears. The most trust-dependent option, since the service cannot independently verify correct key reuse
- **Graceful Degradation**: Services should define their behavior under partial failure in advance — what functionality is reduced, what cached or default responses are returned, and how unavailability is communicated to consumers. This is a design decision, not a runtime improvisation.
- **Backpressure and Load Shedding**: Services should implement mechanisms to signal capacity limits to callers and shed excess load explicitly, rather than accepting unbounded demand and degrading silently under it. The shedding *policy* — when to shed, how much, token-bucket versus concurrency cap versus queue limit — is an implementation detail, but the wire *signal* is a contract concern and is standardized for synchronous HTTP: respond **429 Too Many Requests** when the caller should back off (rate or quota exceeded), and **503 Service Unavailable** when the service is shedding load or overloaded. Both responses SHOULD carry a `Retry-After` header, and services SHOULD expose rate-limit headers so callers can adapt proactively. The retry side of this contract — see Timeout and Retry Behavior — MUST honor `Retry-After`, 429, and 503 rather than blindly retrying. Asynchronous and messaging backpressure is handled at the broker (consumer prefetch, flow control, dead-letter), not through these HTTP signals.
- **Recovery Validation**: Failure scenarios — dependency outages, latency spikes, message loss, and redelivery — should be exercised through deliberate testing. Resilience behavior is validated, not assumed.

### 7. Observability Implementation

Observability Implementation is the engineering practice complement to the [Observability by Design](architecture.md#3-observability-by-design) architecture principle. Where that principle establishes that observability is a design concern, this section defines how it is implemented. OpenTelemetry is the instrumentation standard — a vendor-neutral protocol that exports to an approved observability backend, keeping instrumentation decoupled from the platform it runs on.

#### Structured Logging

All application logs are structured. Log entries are emitted as key-value pairs rather than free-form strings, making them queryable and consistent across services.

- Log entries should include context that identifies the source — service name, version, and environment — without requiring the reader to parse the message
- Correlation identifiers — trace ID and span ID — should be included automatically so log entries can be linked to the distributed trace they belong to
- Log levels should be used deliberately: **Debug** for diagnostic detail, **Info** for meaningful state changes, **Warning** for degraded but recoverable conditions, **Error** for failures that require attention
- Sensitive data — credentials, personal information, tokens — must never appear in log output

#### Metrics

Services should expose metrics that make the health and behavior of important workloads visible over time. At minimum:

- **Request rate**: Volume of inbound requests or messages processed
- **Error rate**: Rate of failed operations, by error type where meaningful
- **Latency**: Processing duration at meaningful percentiles (p50, p95, p99)
- **Saturation**: Queue depth, thread-pool or worker utilization, or other indicators of how close a service is to its limits

Metric names should follow a consistent naming convention across services to support cross-domain comparison and alerting.

#### Distributed Tracing

All inbound requests and outbound calls should participate in a distributed trace. OpenTelemetry trace context is propagated across service boundaries so that a request touching multiple domains can be followed end-to-end.

- Spans should be named to reflect the operation they represent, not implementation details
- Inter-domain calls made through the Clients module should propagate trace context automatically
- Async flows — messages and events — should carry trace context in their metadata so the trace is not broken at domain boundaries

### 8. Security

Security is a first-class concern with its own foundational document — see the [security document](security.md). It is the practice complement to the [Least-Privilege Boundaries](architecture.md#4-least-privilege-boundaries) architecture principle, defining how authentication and authorization, secure pipeline gates, encryption, input validation, threat modeling, and incident response are implemented and verified. Specific tooling choices are recorded separately by each adopter; the security standard itself is tool-agnostic.

Several of those practices execute within the engineering lifecycle described in this document, and the security document is the authoritative source for each:

- **Pipeline gates** run in the CI pipeline (see [Continuous Delivery](#5-continuous-delivery)) — SAST, SCA, image scanning, code quality, and the sensitive-data and least-privilege scope checks — with DAST running as a deploy-stage gate against a live environment. The gates and their pass/fail meaning are defined in the security document; a gate failure blocks promotion.
- **Input validation** is implemented at the Interface Adapters layer against the same OpenAPI and AsyncAPI contracts described in [API and Contract Design](#9-api-and-contract-design).
- **Secrets** are injected at runtime through externalized configuration (see [Service Operability](#6-service-operability)), never baked into an image.
- **Dependency vulnerability scanning** is the SCA gate, addressed further in [Dependency Management](#10-dependency-management).

Refer to the [security document](security.md) for the security standard itself; this section only marks where it intersects the engineering practices.

### 9. API and Contract Design

API and Contract Design is the engineering practice complement to the [Contract Compatibility](architecture.md#5-contract-compatibility) architecture principle. Contracts are authored as the source of truth — OpenAPI for synchronous interfaces, AsyncAPI for asynchronous event contracts — and code is verified to conform to them. Contract changes are deliberate edits to the specification, reviewed and versioned as such.

A contract has two states, and the compatibility discipline applies only to the second:

- **Draft (pre-release, pre-v1)**: The contract is authored first as a design artifact to align on before code or schema work begins, but it is still being learned and evolved. There are no committed consumers yet, so breaking changes to its structure are *expected and acceptable* — backward-compatibility guarantees and breaking-change pipeline gates do not yet bind. This churn is normal design iteration, not a violation.
- **Published (v1 and beyond)**: Once a contract is released and consumers depend on it, the full discipline applies — Backward Compatibility, Explicit Versioning, and breaking-change detection as defined below. This is where the compatibility guarantees switch on.

The breaking-change rules below are therefore scoped to published contracts; pre-release drafts are expected to change shape freely.

#### OpenAPI Contracts

Synchronous API contracts are defined using OpenAPI specifications. The specification is authored first and lives in the Contracts module as the authoritative definition of what the domain exposes. Code is written to conform to the specification, not the other way around.

- Specifications are reviewed and versioned alongside the code that implements them
- Breaking changes to a specification follow the Backward Compatibility and Explicit Versioning expectations defined in Contract Compatibility

#### AsyncAPI Contracts

Asynchronous event and message contracts are defined using AsyncAPI specifications, following the same contract-first discipline as synchronous APIs. The AsyncAPI specification is the source of truth for the shape, semantics, and versioning of events published by a domain.

#### Standard Error Shape

Synchronous error responses use a single, organization-wide error shape rather than a per-domain convention, so a consumer never has to learn a different error format for each domain it calls. The shape carries at least a machine-readable type, a title, the response status, and a human-readable detail; a domain may add its own extension fields on top — for example a field-level validation array — but never omits or renames the base fields. RFC 9457 `application/problem+json` is the recommended realization, named as an example rather than mandated: what is required is one consistent error contract across domains, defined once and reused. The backpressure wire contract in [Service Operability](#6-service-operability) — 429 and 503 with `Retry-After` — is this same error shape applied to the shed-load case.

#### Contract Validation

Published contracts are verified through executable tests that assert the running implementation conforms to its specification:

- **API conformance**: Conformance tests validate that the implemented API matches the OpenAPI specification — requests, responses, and error shapes are verified against the contract. A mismatch between code and contract is a test failure.
- **Event conformance**: Where tooling supports it, the same principle applies to AsyncAPI — published messages are validated against the AsyncAPI specification.
- **Breaking change detection**: Contract changes are automatically compared against the previously published version using schema diffing tooling. A change that removes, renames, or incompatibly modifies an existing element is treated as a breaking change and fails the pipeline.

These tests collectively enforce Contract-First Authoring and the Backward Compatibility expectations of the Contract Compatibility principle — drift and breaking changes are caught before release, not after.

#### Contract Catalog

Because each domain lives in its own repository, published contracts are scattered by design and there is otherwise no way to discover what already exists. Each domain therefore publishes its contracts and accompanying metadata — ownership, version, and status — to a central developer portal so engineers and architects can see what is available before they build. A published spec lets a person *judge* fit; it does not certify fit.

- The single source of truth stays in the domain repository; the catalog copy is **generated** by the domain's pipeline on release, so it cannot drift from the authored contract.
- Metadata is sourced from existing ownership data (`docs/ownership.md` and `CODEOWNERS`) rather than maintained separately.
- The practice is tool-agnostic — Backstage is one example of a developer portal that fits, named as an example and not mandated.

#### Deprecation Lifecycle

A deprecated contract should follow a deliberate path: announce the deprecation with a clear timeline, maintain the deprecated version alongside its replacement during a migration window, and remove it only after consumers have migrated. Deprecation notices should be communicated through the contract itself — via specification metadata or versioned documentation — so consumers have discoverable, actionable information.

#### API Versioning

API major versions are expressed in the URL path (for example `/v1/...`), as decided in [ADR-0017](adrs/0017-api-versioning-via-url-path.md). Only the major version appears in the path; minor and patch changes are backward-compatible and transparent to callers. Regardless of mechanism, the expectations of the Contract Compatibility principle apply: versioning is explicit, breaking changes are deliberate, and consumers are given a migration path.

#### Schema-First Persistence

A domain's persistence schema is an internal contract, subject to the same contract-first discipline as its external interfaces. The schema is the source of truth for the structure of a domain's data — it is authored first as an explicit schema definition (DDL for relational stores) and the application code conforms to it, rather than the schema being generated from code.

Schema changes are managed through a schema migration tool. Each change is a deliberate, version-controlled schema definition script reviewed like any other contract change. Code-first or auto-generated schema approaches are not used — they invert the contract-first discipline by making the schema a byproduct of the code rather than the authoritative definition.

This mirrors the Contract-First Authoring principle applied to the data layer: the schema definition is the contract, migrations are its versioned history, and the owning domain is the sole authority over its schema.

#### Schema Evolution

Schema changes follow an expand-contract pattern so the schema can change without breaking the code running against it:

1. **Expand** — add the new shape alongside the old, without removing anything current code relies on.
2. **Migrate** — backfill data and move application code onto the new shape.
3. **Contract** — once nothing depends on the old shape, remove it in a separate, later change.

Each step is additive and backward-compatible at deploy time: the old and new shapes both work against whatever code is live when the migration runs. This is what lets a schema migration deploy *ahead of* the code that depends on it — the migration runs while the previous code version is still live, and because it is additive that code keeps working. It is also why rollback is always a code-artifact rollback, never a schema rollback: the previous code is guaranteed to still function against the now-expanded schema. A schema breaking-change gate — mirroring the contract breaking-change detection above — blocks a migration containing a destructive change (a dropped column, a narrowed type, a non-nullable column added without a default) that is not part of a deliberate, approved contract step.

#### Entity Identity

A persisted entity carries two identifiers with separate, non-overlapping roles. Its **public identifier** — a UUID, preferably time-ordered, assigned by the domain when the entity is created — is the identity it presents everywhere above persistence: on API and event contracts, in references between aggregates, and to any external system. It is stable for the entity's life, globally unique, independent of any one database instance, and carries no ordering or business meaning. Its **relational surrogate** — a compact sequential key assigned by the database on insert — is the primary key and the sole target of foreign keys and joins inside the domain's database, and never leaves it.

Identity and equality are always taken from the public identifier; the surrogate is a persistence detail (it is unset until the entity is inserted) and is never carried on a contract, an event, or a cross-aggregate reference. Aggregates reference one another by public identifier, not by surrogate, which keeps each aggregate independent of another's storage and keeps references stable across database restores, replicas, and resharding. The surrogate is modelled explicitly rather than hidden behind an ORM's shadow-key feature, so the mapping is uniform across implementation languages. See [ADR-0020](adrs/0020-entity-identity-surrogate-and-public-identifier.md).

#### Reliable Event Publishing

A host that both writes domain state and publishes an event describing that write faces a dual-write problem: the database write and the event publish are two separate operations, and a failure between them can leave one done without the other. Where this risk is worth addressing, the transactional outbox pattern — writing the event as a row in the same transaction as the domain write, then relaying unpublished rows to the broker separately — is the recommended technique. It is recommended, not mandated: it adds operational moving parts (a relay process with its own failure modes) that not every domain needs. Where it is used, the relay should be a simple poller rather than a change-data-capture stream tied to a specific database product, consistent with the replaceable-backing-services discipline in Service Operability.

### 10. Dependency Management

Dependency Management defines how external and internal package dependencies are introduced, versioned, and maintained. Poorly managed dependencies are a source of security vulnerabilities, licensing risk, and build inconsistency — these practices address all three.

#### Centralized Version Management

Dependency versions are managed centrally rather than specified independently per module. This ensures all modules within a domain use consistent versions of shared dependencies, eliminating version conflicts and making upgrades a single, deliberate change.

#### Package Lock Files

Where the package ecosystem supports it, lock files are committed to version control and treated as part of the build contract. They guarantee that every build — local, CI, and across all environments — resolves the exact same dependency graph. Builds that bypass the lock file are not permitted.

#### Release Versions Only

Only stable release versions of dependencies are permitted. Pre-release, alpha, beta, and release candidate versions introduce instability and are not appropriate for production systems. Dependencies must have a published stable release before they can be introduced.

#### Vulnerability Scanning

Dependencies are scanned for known vulnerabilities as part of the CI pipeline through SCA tooling, as established in the [security document](security.md). Vulnerability scanning is also run on a scheduled basis independent of builds, so newly disclosed vulnerabilities in existing dependencies are detected promptly rather than only when code changes.

#### License Compliance

Dependencies must use licenses that permit commercial use — permissive open-source licenses such as MIT, Apache-2.0, and BSD are acceptable by default. Copyleft licenses (such as GPL and LGPL) and any license restricting commercial use require explicit review before adoption, and commercial software must be properly purchased and licensed. License compliance is evaluated when a dependency is introduced and re-evaluated when a dependency is upgraded, as license terms can change between versions.

#### Package Sources

Third-party dependencies are resolved from public package registries. Internal and shared platform packages are resolved from private repositories. Both sources are explicitly configured — dependencies may not be resolved from uncontrolled or unapproved sources.
