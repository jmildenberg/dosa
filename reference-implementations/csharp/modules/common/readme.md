# C# :: Modules :: Common

> [C#](../../readme.md) :: [Modules](../readme.md) :: Common

Shared kernel — small, stable, cross-cutting types and utilities (e.g. a `Result` type, common value
objects) with no business logic and no dependency on any other module here.

**Depends on:** nothing. **Depended on by:** any module that needs it, but sparingly — anything with real
behavior belongs in Domain or Application instead, not here. If a type in Common starts encoding a
business rule, that is a sign it has outgrown this module.
