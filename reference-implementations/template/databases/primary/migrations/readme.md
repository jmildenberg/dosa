# Template :: Databases :: Primary :: Migrations

> [Template](../../../readme.md) :: [Databases](../../readme.md) :: [Primary](../readme.md) :: Migrations

Version-controlled schema migration scripts, applied by the tool configured in
[tooling/](../tooling/readme.md). Schema is authored here first; application code conforms to it.

Every migration follows **expand-contract** and deploys **ahead of** the code that needs it:

1. **Expand** — add the new shape alongside the old. Additive and backward-compatible with whatever code
   is currently live.
2. **Migrate** — the migration runs and succeeds *before* the new code version rolls out, giving an
   explicit checkpoint validated against the previous running code.
3. **Contract** — remove the old shape in a later, separate migration, once nothing depends on it.

A destructive change in one step — a dropped column, a narrowed type, a non-nullable column with no
default — is blocked by the breaking-change gate unless it is a deliberate, approved contract step.
Because every migration is additive, rollback is always a code-artifact rollback, never a schema
rollback.

---

References: [docs/adrs/0014-schema-first-persistence.md](../../../../../docs/adrs/0014-schema-first-persistence.md)
