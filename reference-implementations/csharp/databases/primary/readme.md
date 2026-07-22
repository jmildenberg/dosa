# C# :: Databases :: Primary

> [C#](../../readme.md) :: [Databases](../readme.md) :: Primary

This domain's primary data source. Everything about its schema lives here, not scattered across
[modules/infrastructure](../../modules/infrastructure/readme.md) — infrastructure code reads and writes
against this schema, it does not define it.

| Area | Contents |
|---|---|
| [migrations/](migrations/readme.md) | Version-controlled, expand-contract schema changes |
| [seeds/](seeds/readme.md) | Reference/fixture data loaded after migrations run |
| [tooling/](tooling/readme.md) | Configuration for the migration tool that applies the above |
