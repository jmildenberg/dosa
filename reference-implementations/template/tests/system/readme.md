# Template :: Tests :: System

> [Template](../../readme.md) :: [Tests](../readme.md) :: System

Tests that exercise this domain end-to-end through its published surface — a running
[hosts/api](../../modules/hosts/api/readme.md) or [hosts/worker](../../modules/hosts/worker/readme.md)
and real [Infrastructure](../../modules/infrastructure/readme.md) adapters, driven through
[Contracts](../../modules/contracts/readme.md) the way a real consumer would. This is the peak of the
[Testing Trophy](../../../../docs/adrs/0007-testing-trophy-and-test-classification.md) — used
selectively, for the paths where only a real run through the whole stack gives confidence.
