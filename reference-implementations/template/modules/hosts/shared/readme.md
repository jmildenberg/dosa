# Template :: Modules :: Hosts :: Shared

> [Template](../../../readme.md) :: [Modules](../../readme.md) :: [Hosts](../readme.md) :: Shared

Wiring shared by two or more hosts — things like common startup/config plumbing, health-check
endpoints, or logging setup that every host needs but that isn't itself a use case and so doesn't belong
in [Application](../../application/readme.md).

Keep this thin. If something here starts making decisions rather than wiring existing pieces together,
it belongs in [Application](../../application/readme.md) or [Infrastructure](../../infrastructure/readme.md)
instead.
