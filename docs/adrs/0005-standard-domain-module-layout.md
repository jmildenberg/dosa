# 0005. Standard Domain Module Layout

Date: 2026-06-22

## Status

Accepted

## Context

[ADR-0002](0002-adopt-clean-architecture.md) established Clean Architecture as the layering model, [ADR-0003](0003-adopt-domain-oriented-service-architecture.md) established that services are organized into domains, and [ADR-0004](0004-one-repository-per-domain.md) established that each domain is its own repository. What remains undecided is how code is physically organized *within* a domain repository.

Without a standard internal layout, each domain invents its own structure. The layer boundaries that Clean Architecture depends on become implicit and inconsistently named, making them harder to enforce with tooling and harder for engineers to navigate when moving between domains. The inter-domain communication model from Domain-Oriented Service Architecture — where domains interact only through published contracts — needs a concrete structural home, or it degrades into convention that is easy to bypass.

A standard layout is needed that expresses the Clean Architecture dependency rule structurally, gives published contracts a dedicated location, and produces one recognizable structure across implementation technologies — the same folders in the same places whether a domain is built in C#, Java, or another stack — without forcing any language away from its native build-artifact and package conventions.

## Decision

We will adopt a single **canonical repository layout** for every domain, standardized across implementation technologies. Two things are deliberately separated:

- **Folder structure and names are standardized** — the same areas in the same places, in kebab-case, in every language. This is the navigational map.
- **Build-artifact, assembly, and package names stay idiomatic** to each technology. A standardized `modules/domain/` folder may contain a `CSharp.Domain` project or a Maven `:domain` module — the folder is the standard, the artifact name is not.

The canonical layout:

```text
<domain>/
├── docs/            # adrs/, architecture/, contracts/ (OpenAPI + AsyncAPI specs — source of truth), operations/
├── databases/       # one dir per data source: migrations/, seeds/, tooling/ (schema-upgrade tool config) — source of truth for persistence
├── deploy/          # docker/ (image build assets), helm/ (deployment chart)
├── modules/         # all code areas
│   ├── domain/          # entities + rules
│   ├── application/     # use cases / orchestration
│   ├── common/          # shared kernel — cross-cutting types/utilities, no business logic
│   ├── contracts/       # api/, messaging/ — published interface types
│   ├── clients/         # language-targeted client SDKs (java/, csharp/, go/, etc.) for calling this domain
│   ├── infrastructure/  # persistence, messaging, external adapters
│   ├── frontend/        # presentation role (optional)
│   └── hosts/           # api/, worker/, web/, mcp/, shared/
├── tests/           # architecture/, system/ (repo-wide)
├── tooling/         # local-dev helpers, git-hook installers
└── README.md
```

The code modules preserve the Clean Architecture dependency rule (dependencies point inward):

| Module | Responsibility | Depends on |
|---|---|---|
| **Domain** | Core business entities and domain rules | Nothing |
| **Application** | Use cases and orchestration | Domain |
| **Common** | Shared kernel — cross-cutting types and utilities with no business logic | Nothing |
| **Contracts** | Published request, response, and event schema types the domain exposes; carries no Domain internals | Nothing |
| **Clients** | Consumer-facing implementation for invoking this domain | Contracts |
| **Infrastructure** | Adapters for external services, messaging, and persistence | Application, Domain |
| **Database** | Schema definitions and migrations; source of truth for persistence | Nothing |

A module may be extended into multiple focused subfolders nested plainly under it, with no prefix repeating the module's own name — flat, GitHub-idiomatic nesting reads faster than a bespoke naming scheme (Infrastructure into persistence and messaging; Contracts by protocol: `contracts/api/`, `contracts/messaging/`; Clients by target language: `clients/java/`, `clients/csharp/`, `clients/go/`). The dependency rules of the base module apply to all of its subfolders.

**Roles and hosts.** A domain contains only the hosts it needs — each a deployable entry point under `modules/hosts/` — divided by whether the role operates on domain state:

- **In-process operators** — `hosts/api` (domain-logic) and `hosts/worker` (asynchronous processing) reference Domain, Application, and Infrastructure directly.
- **External consumers** — `hosts/web` (request-shaping / backend-for-frontend) and `hosts/mcp` (Model Context Protocol server) reference only Contracts and Clients — the same published-interface access any other domain has — so they cannot reach domain state even in principle. The presentation role (`frontend/`) reaches the domain only through the request-shaping host. The MCP host is not a special case: it is just another external-consumer host of the standard layout. It holds no logic of its own (no model or agent runtime lives here); it exposes the published surface to MCP clients, and the agent itself lives in whatever client connects.

Inter-domain dependencies flow exclusively through the producing domain's **Clients** module, which depends only on **Contracts**. A consuming domain therefore references published contracts and never another domain's internals.

**Tests.** Repo-wide architectural and system tests live under `tests/`. Per-module unit tests follow the ecosystem's convention — inside the module for JVM (`src/test`), a separate test project for .NET — the one placement build tooling dictates rather than the standard mandating.

## Consequences

- The Clean Architecture dependency rule is expressed structurally, making layer violations visible and enforceable by architectural rules testing
- Published contracts have a dedicated, dependency-free home, so consumers can reference a domain's interface without pulling in its business logic
- Inter-domain coupling is forced through the Clients/Contracts boundary, keeping cross-domain access deliberate and consistent with Domain-Oriented Service Architecture
- Engineers moving between domains encounter a familiar structure, lowering the cost of ownership transfer
- Standardizing the folder structure while leaving build-artifact names idiomatic gives one recognizable map across languages without forcing any stack away from its native conventions — an engineer recognizes the same shape in a C# and a Java domain
- The role/host model gives the domain's runnable components a shared vocabulary, and the operator/consumer split makes the external-consumer boundary (`hosts/web` and `hosts/mcp` have no domain-state access) structurally enforceable rather than conventional
- Per-module unit-test placement is the one thing the layout cannot unify — it follows each build tool's convention — so the standard covers the repo-wide test areas and leaves that placement to the ecosystem
- Domains must maintain the separation even when it feels heavier than a single combined module — the structure carries overhead that is only justified at domain scale
- Module extensions introduce naming and organizational decisions that teams must make consistently for the dependency rules to remain legible
- Existing reference implementations predate this standardized layout and do not conform until brought onto it; realizing the standard is what the reference examples are (re)built to demonstrate

## Reversibility

One-way door. The layout is the structural expression of Clean Architecture and the clients/contracts boundary across every domain, now including the standardized folder structure and the host roles; changing it would require re-organizing modules, re-pointing inter-domain dependencies, and relocating folders in every repository, and the architectural tests that enforce the layout would all need rewriting. The cost compounds with the number of domains already built to it.
