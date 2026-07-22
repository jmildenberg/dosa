# 0015. Dependency Management

Date: 2026-06-22

## Status

Accepted

## Context

External and internal package dependencies are a standing source of three risks: security vulnerabilities introduced through transitive packages, licensing exposure from terms that prohibit commercial use, and build inconsistency when versions resolve differently across machines and environments. Without a standard approach, each domain manages dependencies its own way, version drift between modules causes conflicts, and vulnerable or improperly licensed packages enter the system unnoticed.

A consistent policy is needed for how dependencies are versioned, locked, sourced, scanned, and licensed, complementing the security pipeline gates already required by the Security Practices.

## Decision

We will manage dependencies under a single standard policy:

- **Centralized version management** — dependency versions are managed centrally rather than per module, so all modules in a domain resolve consistent versions and upgrades are a single deliberate change
- **Committed lock files** — where the ecosystem supports them, lock files are committed and treated as part of the build contract; builds that bypass the lock file are not permitted
- **Release versions only** — only stable releases are used; pre-release, alpha, beta, and release-candidate versions are not introduced
- **Vulnerability scanning** — dependencies are scanned via SCA in the CI pipeline and on a schedule independent of builds, so newly disclosed vulnerabilities are caught promptly
- **License compliance** — dependencies must use licenses that permit commercial use (permissive licenses such as MIT, Apache-2.0, and BSD by default); copyleft and any commercial-use-restricting license require review, and commercial software must be properly licensed; compliance is re-evaluated on upgrade
- **Controlled sources** — third-party packages resolve from public registries and internal packages from private repositories; both are explicitly configured, and uncontrolled or unapproved sources are not permitted

## Consequences

- Consistent versions across modules eliminate a class of version-conflict and "works on one machine" failures
- Committed lock files guarantee every build — local, CI, and across environments — resolves an identical dependency graph
- Scheduled scanning surfaces newly disclosed vulnerabilities in existing dependencies, not only those introduced by a code change
- A clear license policy keeps licensing exposure visible and decided at introduction and upgrade rather than discovered later
- Controlled sources prevent dependencies from entering through uncontrolled channels, reducing supply-chain risk
- Centralized versions and the release-only rule add friction to fast-moving upgrades and exclude pre-release packages a team might otherwise want
- License and source policies require enforcement tooling and periodic review to remain effective

## Reversibility

Two-way door. The policy governs how dependencies are versioned, locked, scanned, and sourced; relaxing or revising any of these rules is a configuration and process change with no structural or runtime coupling. Individual rules can be tightened or loosened independently, and reversing the policy leaves existing code untouched.
