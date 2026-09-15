# C# Tool-Evaluation Corpus

`main` carries only this overview -- it holds no project code. Every other
branch in this repository is a complete, independently buildable project for
one exact combination of .NET version, bundler (`SDK-style MSBuild`/`Cake`/`.NET CLI`), package manager (`NuGet PackageReference`/`NuGet packages.config`/`Paket`/`Central Package Management`) and architecture (`Monolith`/`Microservices`).

This repository consolidates what used to be 65 separate small repositories
into one repository holding all 312 branches, so the full corpus for this
language lives in a single place.

## Branch naming

Every branch name encodes its own identity, so the name alone tells you what
it is without needing to open it:

```
CS_V{ver}_{BUNDLER}_{PKGMGR}_{ARCH}
```

## Versions in this corpus

13 .NET versions, 24 branches each (one per bundler x package manager x
architecture combination):

| .NET version | Branches |
|---|---|
| NETFX45 | 24 |
| NETFX46 | 24 |
| NETFX462 | 24 |
| NETFX47 | 24 |
| NETFX471 | 24 |
| NETFX472 | 24 |
| NETCOREAPP30 | 24 |
| NET5 | 24 |
| NET6 | 24 |
| NET7 | 24 |
| NET8 | 24 |
| NET9 | 24 |
| NET10 | 24 |

## Finding a branch

Each branch's own `README.md` describes its exact cell (version, bundler,
package manager, architecture) and its full tool roster. `dataset.json` on
each branch is the machine-readable answer key for that cell.

## History

Branches were moved into this repository by relocating their existing git
history under a new name -- every commit, author, date and
`Co-authored-by:` trailer is unchanged from before consolidation. Nothing
was squashed or rewritten, so tools that read commit history (churn,
ownership, `pydriller`, etc.) see the same signal they always did.
