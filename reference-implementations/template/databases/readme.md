# Template :: Databases

> [Template](../readme.md) :: Databases

Source of truth for persistence, one directory per data source
(see [ADR-0005](../../../docs/adrs/0005-standard-domain-module-layout.md)). This domain has one:
[primary/](primary/readme.md) — add another sibling directory only when the domain genuinely owns a
second, independent data source, not as a way to organize schema within one database.

Schema is authored first as explicit DDL, and application code conforms to it — not generated from an
ORM (see [ADR-0014, Schema-First Persistence](../../../docs/adrs/0014-schema-first-persistence.md)).
