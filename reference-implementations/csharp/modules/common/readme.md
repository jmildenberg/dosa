# C# :: Modules :: Common

> [C#](../../readme.md) :: [Modules](../readme.md) :: Common

Shared kernel — small, stable, cross-cutting types and utilities (e.g. a `Result` type, common value
objects) with no business logic and no dependency on any other module here.

**Depends on:** nothing. **Depended on by:** any module that needs it, but sparingly — anything with real
behavior belongs in Domain or Application instead, not here. If a type in Common starts encoding a
business rule, that is a sign it has outgrown this module.

## Ensure

`ToDo.Common.Ensure` is a set of extension-method argument guards, one static class per guarded type
(`ObjectExtensions`, `StringExtensions`, `GuidExtensions`, `DateTimeExtensions`, `DateTimeOffsetExtensions`).
Each guard throws `ArgumentException` (with `paramName` filled in automatically via
`CallerArgumentExpression`) and returns the value unchanged, so calls can be chained inline:

```csharp
var id = request.Id.EnsureNonNull().EnsureNonEmpty();
```

- `EnsureNonNull` — reference types and nullable value types (`T? where T : class` / `struct`). Null only;
  never checks for empty/default.
- `EnsureNonEmpty` (`string`) — null or `""`.
- `EnsureNonBlank` (`string`) — null, `""`, or whitespace-only.
- `EnsureNonEmpty` (`Guid`) — equals `Guid.Empty`.
- `EnsureNonEmpty` (`DateTime`, `DateTimeOffset`) — equals `default`. Note `default` and `MinValue` are the
  same value for these types, so a value deliberately set to `MinValue` is indistinguishable from one that
  was never set.

Null and empty/default are separate, composable checks — `EnsureNonNull` never absorbs an empty check, so
callers choose exactly which guarantees they need.
