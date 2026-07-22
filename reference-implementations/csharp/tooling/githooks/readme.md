# C# :: Tooling :: GitHooks

> [C#](../../readme.md) :: [Tooling](../readme.md) :: GitHooks

Git hook scripts (e.g. pre-commit formatting/lint checks) and the installer that wires them into a
contributor's local checkout. Fast, local feedback before a change is pushed — the authoritative gate is
still [CI](../ci/readme.md); a hook that can be skipped locally must never be the only place a check runs.
