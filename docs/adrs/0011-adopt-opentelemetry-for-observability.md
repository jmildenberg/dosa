# 0011. Adopt OpenTelemetry for Observability

Date: 2026-06-22

## Status

Accepted

## Context

The Observability by Design principle requires that logs, metrics, and traces be part of how the system explains its behavior. Realizing it needs a concrete instrumentation standard. If each domain instruments against a specific vendor's SDK, observability becomes coupled to that backend — switching providers means re-instrumenting every service, and cross-domain correlation depends on every team having chosen the same vendor.

A request in a Domain-Oriented Service Architecture crosses domain boundaries, so trace context must propagate across services and through asynchronous flows for behavior to be followed end-to-end. That requires a shared, interoperable instrumentation model rather than per-domain choices.

## Decision

We will adopt OpenTelemetry as the instrumentation standard for logs, metrics, and traces. OpenTelemetry is a vendor-neutral protocol that exports to an approved observability backend, keeping instrumentation decoupled from the platform it runs on.

- **Structured logging** — logs are emitted as key-value pairs carrying source identity and correlation identifiers (trace and span IDs)
- **Metrics** — services expose request rate, error rate, latency percentiles, and saturation under a consistent naming convention
- **Distributed tracing** — trace context propagates across service boundaries and is carried in message metadata so asynchronous flows do not break the trace

## Consequences

- Instrumentation is decoupled from any single observability vendor, so the backend can change without re-instrumenting services
- A shared standard makes trace context propagate across domains, so a request touching multiple domains can be followed end-to-end
- Consistent metric naming and structured logs make health comparable and queryable across services
- Carrying trace context through messaging keeps asynchronous, event-driven flows observable rather than opaque at domain boundaries
- OpenTelemetry instrumentation, collectors, and exporters are additional components every service and the platform must run and maintain
- A vendor-neutral layer can lag provider-native instrumentation in features or convenience, a trade-off accepted for portability

## Reversibility

Two-way door. OpenTelemetry was chosen precisely to decouple instrumentation from any backend, so the costly part — re-instrumenting services — is what the decision avoids. The backend can be swapped freely, and switching the instrumentation standard itself, while a non-trivial sweep across services, carries no runtime or architectural coupling.
