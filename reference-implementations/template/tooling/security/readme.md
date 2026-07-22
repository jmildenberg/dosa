# Template :: Tooling :: Security

> [Template](../../readme.md) :: [Tooling](../readme.md) :: Security

Local configuration for the [Secure Pipeline Gates](../../../../docs/security.md#1-secure-pipeline-gates)
this domain runs in [CI](../ci/readme.md): SAST, SCA (dependency scanning), container image scanning, and
DAST configuration/suppression files, plus any local pre-merge equivalents wired through
[githooks/](../githooks/readme.md). This is configuration only — the standard the gates enforce is
defined once in [Security](../../../../docs/security.md), not restated here.
