# DOSA :: Documentation :: Operations and Reliability

> [DOSA](../readme.md) :: [Documentation](./readme.md) :: Operations and Reliability

This document defines how systems built on this guidance are operated and kept reliable once
running. Where the [engineering document](engineering.md) defines how software is *built* —
including the build-time operability expectations in its Service Operability practice — this
document defines the *run-time* operating model. It builds on the
[Resilience by Design](architecture.md#6-resilience-by-design) and
[Observability by Design](architecture.md#3-observability-by-design) principles, draws its
security-incident requirements from the [security document](security.md), and is governed by the
same [governance](governance.md) model.

## Service Level Objectives

Every host defines at least one SLO appropriate to its role — an API host's latency and
error-rate objective looks different from a worker's queue-processing-lag objective — because a
host with no stated expectation for "healthy" cannot be meaningfully monitored. SLIs are drawn
from the observability instrumentation defined in the engineering document.

Each SLO carries an **error budget** — the allowable shortfall over its measurement window. The
budget has a proposed policy consequence: when a host's error budget is substantially spent,
reliability work takes priority over new feature work for that host until the budget recovers,
giving an objective trigger for that trade-off rather than an ad hoc argument each time it arises.

*Targets, measurement windows, and the exact budget-exhaustion policy are for the SRE team and
owning teams to set.*

## Monitoring and Alerting

Alerting is built on the observability instrumentation defined in the engineering document.

- **Alert on SLO burn rate**, not raw threshold crossings alone — a brief spike that does not
  threaten the SLO should not page anyone.
- **Every dashboard covers the four golden signals at minimum** — latency, traffic, errors, and
  saturation — so an on-call engineer unfamiliar with a specific domain can still orient quickly.
  Host-specific panels (queue depth for a worker, and so on) are added on top.
- **Every page links to a runbook** (see Runbooks) — an alert that can page a human with no
  corresponding runbook is treated as incomplete.
- **Alerts page the owning team**, not a shared, undifferentiated pool — a domain has a single
  owning team (see [Domain Ownership](governance.md#domain-ownership)). An escalation chain for
  unacknowledged pages is expected; its rotation and timing are an organizational decision.

*Severity levels, thresholds, routing, and tooling are for the SRE team to set.*

## Deployment and Rollout Health

Deployment health is a monitored signal distinct from application health. Under the
declarative-deployment model (see [Continuous Delivery](engineering.md#5-continuous-delivery)),
whether an environment has actually converged to the version it is meant to be running — and
whether that reconciliation is stuck or has drifted — is its own alertable condition that a
healthy running application will not surface on its own. A rollback is verified the same way a
forward promotion is: smoke tests run against the environment after it completes, not assumed
successful because the platform reports the change as applied.

## Health Checks

Readiness and liveness probes are defined at build time (see
[Service Operability](engineering.md#6-service-operability)) and are also the first operational
signal on-call checks: a host failing readiness has already been pulled from traffic, which is
often the fastest evidence of what is wrong before deeper investigation. A readiness probe should
reflect the host's ability to operate against the current state of its dependencies, including
compatibility with the current database schema — this is what makes schema-ahead-of-code
deployment (see [API and Contract Design](engineering.md#9-api-and-contract-design)) hold in
practice.

## Incident Management

When a system fails in production, the review focuses on systemic causes rather than individual
fault. A blameless postmortem covers: timeline, impact, root cause(s), what went well, what did
not, and concrete follow-up actions with owners. Where other domains depend on the one
experiencing an incident, a proactive path to inform them (a status channel, a notification) is
expected rather than leaving them to discover it through their own alerting.

Security incidents follow this same process with additional requirements — evidence preservation
before remediation and a legal/disclosure escalation path — defined in the
[security document](security.md#10-security-incident-response).

*On-call model, severity classification, escalation timing, and communication channels are for
the SRE team to set.*

## Runbooks

Every domain maintains runbooks for its alertable conditions. A runbook covers what the alert
means, the likely causes ranked by probability, the diagnostic steps to identify which is active,
and the remediation for each. Runbooks are versioned and reviewed like code — a stale runbook is
a defect. Disaster recovery is a runbook, not a one-time setup: who executes a restore, in what
order, and how it is verified afterward is exactly what a runbook exists for.

## Synthetic Monitoring

A scheduled, black-box check exercises each externally reachable host from outside the cluster,
the way a real caller would, alerting independently of the host's own health checks. This catches
failures the host cannot see itself — DNS, ingress, certificate, or network-path problems at the
layer most visible to actual users. Tool and check frequency are a deployment choice.

## Capacity and Resource Management

- **Bounded resources**: every host declares explicit CPU and memory requests and limits, so no
  host runs unbounded and one domain cannot starve others sharing a cluster.
- **Autoscaling**: horizontal autoscaling is the standard mechanism for per-host scaling —
  utilization-based, or driven by a custom metric such as queue depth for a worker.
- **Disruption budgets**: every host expresses the minimum availability it can tolerate during
  *voluntary* disruption (node drains, cluster upgrades), so routine platform maintenance cannot
  take a low-replica host to zero.
- **Inbound protection**: request-rate limiting is applied at the ingress layer rather than
  reimplemented in each host; the standard shed-load wire signals (429/503 with `Retry-After`)
  are defined in [Service Operability](engineering.md#6-service-operability).

Capacity *planning* — forecasting and the budget decisions that follow — is an organizational
process; the telemetry and SLO data above are its inputs.

*Planning cadence and ownership are for the SRE team to set.*

## Disaster Recovery and Business Continuity

Each domain owns its own data stores and is independently recoverable. The DR posture — that
backup and restore exist and are exercised through a restore runbook (above) — is a standing
expectation; recovery objectives (RTO/RPO) and backup mechanics are governed by a broader
organizational policy rather than defined here.

*Recovery objectives, backup standards, and failover expectations are for the SRE team and
business unit to set.*

## Operational Readiness

Before a host enters production it should meet a readiness bar: defined SLOs and dashboards,
liveness and readiness probes, resource requests and limits with a disruption budget, at least
one runbook, synthetic monitoring for any externally reachable surface, and security gates passed
(see the [security document](security.md)). This is the operational complement to the
[Definition of Done](governance.md#definition-of-done) in governance.

*The precise readiness checklist is for the SRE team to finalize.*
