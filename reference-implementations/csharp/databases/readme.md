# C# :: Databases

> [C#](../readme.md) :: Databases

Source of truth for persistence, one directory per data source. This domain has one:
[primary/](primary/readme.md) — add another sibling directory only when the domain genuinely owns a
second, independent data source, not as a way to organize schema within one database.

Schema is authored first as explicit DDL, and application code conforms to it — not generated from an
ORM.

---

References: [docs/adrs/0005-standard-domain-module-layout.md](../../../docs/adrs/0005-standard-domain-module-layout.md), [docs/adrs/0014-schema-first-persistence.md](../../../docs/adrs/0014-schema-first-persistence.md)
