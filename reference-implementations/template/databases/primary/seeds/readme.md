# Template :: Databases :: Primary :: Seeds

> [Template](../../../readme.md) :: [Databases](../../readme.md) :: [Primary](../readme.md) :: Seeds

Reference and fixture data loaded after [migrations](../migrations/readme.md) run — lookup/reference
tables a fresh environment needs to be usable, and any fixture data local development or
[system tests](../../../tests/system/readme.md) rely on. Not a substitute for a migration: anything that
defines schema structure belongs in [migrations/](../migrations/readme.md), not here.
