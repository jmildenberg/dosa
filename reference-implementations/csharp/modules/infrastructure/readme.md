# C# :: Modules :: Infrastructure

> [C#](../../readme.md) :: [Modules](../readme.md) :: Infrastructure

Persistence, messaging, and external-service adapters that implement the ports Application defines —
the layer that translates between the domain's language and the outside world.

**Depends on:** Application and Domain. **Depended on by:** the in-process Hosts (`hosts/api`,
`hosts/worker`), which wire this module in at startup.

May be extended into focused subfolders as needed (e.g. `persistence/`, `messaging/`) — one subfolder per
concern, no subfolder that leaks a Domain internal past this layer.

---

References: [docs/adrs/0002-adopt-clean-architecture.md](../../../../docs/adrs/0002-adopt-clean-architecture.md)
