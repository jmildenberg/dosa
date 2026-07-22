# 0008. Standard Delivery Progression

Date: 2026-06-22

## Status

Accepted

## Context

[ADR-0006](0006-adopt-trunk-based-development.md) keeps main continuously releasable, which is only valuable if there is a defined, automated path from a merge to production. Without a standard progression, each domain wires its own route to production, environments mean different things across teams, and the artifact that was tested is not necessarily the artifact that ships — environment-specific rebuilds reintroduce the risk that testing was supposed to remove.

A standard delivery progression is needed: a fixed set of environments with clear promotion rules, a single mechanism for building and deploying, and a guarantee that the same artifact flows from the first environment to the last. The progression must also accommodate teams with differing release obligations — some require a dedicated user-acceptance stage, others do not.

## Decision

We will adopt a standard delivery progression of three environments, with an optional fourth for user acceptance testing.

| Environment | Trigger | Promotion |
|---|---|---|
| **Development** | Merge to main | Automatic |
| **Staging** | Release candidate promoted | Manual approval to advance |
| **UAT** *(optional)* | Approved from Staging | Manual approval to advance |
| **Production** | Approved from Staging or UAT | — |

The pipeline is the sole mechanism for building and deploying artifacts; nothing is built or deployed outside it. A merge to main is built once into a versioned image, and that same image is promoted unchanged through every environment — it is never rebuilt between stages. Environment differences are expressed entirely through configuration.

Promotion from main to Development is fully automated. Every promotion toward Production requires an explicit approval gate — from Staging directly to Production, or from Staging through UAT to Production where the optional UAT stage is used.

## Consequences

- There is one defined path from merge to production, so the route a change takes is the same across every domain
- Building once and promoting the same image removes environment-specific rebuilds, so the artifact validated in early environments is exactly the artifact that reaches Production
- Automated promotion to Development gives continuous, live validation of every change, realizing the value of keeping main releasable
- Approval gates toward Production keep human judgment at the points where it matters, without slowing integration
- The optional UAT stage lets teams with an acceptance obligation add it without imposing it on teams that do not need it
- Expressing all environment differences through configuration requires disciplined externalized configuration and secrets management; nothing environment-specific may be baked into the image
- A fixed progression constrains teams from inventing ad hoc environments or bypass routes, which is the intended trade-off but removes local flexibility

## Reversibility

Two-way door (moderate). The progression is a pipeline and environment convention; changing the environment set, triggers, or promotion gates is a pipeline reconfiguration rather than a code or architecture change. The build-once-promote-unchanged guarantee is widely assumed, so altering it touches every domain's pipeline, keeping the cost more than trivial.
