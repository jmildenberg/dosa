# 0007. Testing Trophy and Test Classification

Date: 2026-06-22

## Status

Accepted

## Context

Teams need a shared model for how to invest in tests and a shared vocabulary for describing them. Two problems arise without one. First, the classic test pyramid pushes most investment into isolated unit tests, which over-specifies internal behavior and produces brittle suites that resist refactoring while giving limited confidence that components work together. Second, terms like "integration test" mean different things to different engineers — the scope, the runtime dependencies, and what is stubbed all vary by who wrote the test — making coverage impossible to reason about across domains.

[ADR-0002](0002-adopt-clean-architecture.md) makes the domain and application layers testable without infrastructure, and [ADR-0003](0003-adopt-domain-oriented-service-architecture.md) gives each domain a clear boundary. A testing model is needed that takes advantage of both: favoring tests that exercise real behavior across a domain slice, and naming test categories precisely enough that their scope is unambiguous.

## Decision

We will adopt the Testing Trophy as the test strategy and a fixed six-category classification as the shared vocabulary.

The Testing Trophy weights investment toward integration tests as the primary return on effort, over a broad base of static analysis and a focused set of unit tests, with end-to-end tests used selectively. Its layers, from foundation to peak, are Static Analysis, Unit, Integration, and End-to-End.

Every test is classified into exactly one category, defined by its runtime dependencies:

| Category | Trophy Layer | Runtime Dependencies |
|---|---|---|
| **Architectural** | — | None — validates code structure (layer dependencies, domain isolation, conventions) |
| **Unit** | Unit | None — isolates a single unit of business logic |
| **Integration** | Integration | One infrastructure element, which is the subject of the test; nothing is stubbed |
| **SubSystem** | Integration | All direct infrastructure dependencies live; upstream/downstream services stubbed |
| **System** | End-to-End | The deployed system live; external/ecosystem dependencies may be stubbed or sandboxed |
| **End to End** | End-to-End | Everything live, nothing stubbed — the system plus its surrounding ecosystem |

Integration and SubSystem together occupy the Trophy's integration band and carry the primary investment. Architectural sits outside the Trophy model — it runs like a unit test but validates structure rather than behavior, extending the model with a category it does not otherwise cover. System and End to End both sit in the End-to-End layer, distinguished by how much of the world is live: System validates the deployed system in isolation (external dependencies may be stubbed or pointed at sandboxes), while End to End validates a full user journey with the system and its surrounding ecosystem live and nothing stubbed. Because End to End spans multiple systems, its tests are owned and run outside any single domain repository; System is therefore the highest tier a domain repository houses.

## Consequences

- Investment is concentrated where confidence per test is highest, exercising real behavior across a domain slice rather than over-specifying internal units
- "Integration test" and every other category have a single, unambiguous meaning defined by runtime dependencies, so coverage can be reasoned about consistently across domains
- The Architectural category gives the layer and boundary rules from Clean Architecture and Domain-Oriented Service Architecture an executable enforcement mechanism that runs as fast as a unit test
- The SubSystem category aligns directly with the domain boundary, making "test a domain within its own boundary" a first-class, named activity
- The classification is fixed vocabulary; teams must place each test in exactly one category, which requires judgment at the edges (for example, deciding when a test crosses from Integration to SubSystem)
- Favoring integration-band tests requires infrastructure for stubbing upstream services and provisioning real backing dependencies in test, which is more setup than a unit-only suite

## Reversibility

Two-way door. The classification is shared vocabulary and a strategy for where to invest; reclassifying or reweighting tests is incremental and carries no runtime or topology coupling. Existing suites keep working under a revised model — the cost to reverse is re-labeling and re-balancing, not rebuilding.
