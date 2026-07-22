# Template :: Modules :: Hosts :: Worker

> [Template](../../../readme.md) :: [Modules](../../readme.md) :: [Hosts](../readme.md) :: Worker

In-process operator host — asynchronous processing. References
[Domain](../../domain/readme.md), [Application](../../application/readme.md), and
[Infrastructure](../../infrastructure/readme.md) directly, and consumes/produces events described in
[Contracts/messaging](../../contracts/messaging/readme.md).

Holds queue/broker consumer wiring and scheduled jobs — anything that runs without a direct inbound
request. Business logic still lives in [Application](../../application/readme.md); this host only
triggers it on a message or a schedule.
