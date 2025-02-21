# OrderKit - CS-023

One branch of the C# tool-evaluation corpus. Test **data**, not a product: tool
execution is not part of the build, and evaluation happens later.

> **This cell cannot exist.** `dotnet restore` has never supported packages.config. NuGet's packages.config path is implemented in MSBuild.exe / nuget.exe only.
>
> The branch ships the artifacts the cell definition demands, plus this notice. It is *not* quietly backed by a working configuration -- a branch labelled one thing that ran another underneath is the exact silent success this corpus exists to catch.

## Branch

`CS-023`

## Cell

| Axis | Value |
|---|---|
| Bundler | .NET CLI |
| Package manager | NuGet (packages.config) |
| Architecture | Monolith |

Projects:

- `OrderKit` (`src/OrderKit`)

## Target framework

`net45` - .NET Framework 4.5

The oldest TFM named anywhere in the source metric sheet. Out of support since 12 January 2016; retained deliberately as the corpus floor.

## Language version

C# 5, taken as the default of the target framework. `LangVersion` is deliberately
never written to any project file, which is what keeps the family axis one number
instead of two.

The domain layer is written to that language subset exactly, and the lock is
verified by execution rather than asserted:

```
csc -langversion:5   ->  clean
csc -langversion:4   ->  error CS8025: Feature 'async function' is not available
```

## Supported tools

5 of the 19 tools in the roster are expected to fire on this branch. The rest exit
3 (cannot run here) or 4 (absent from this host), each with its reason in both
`run.sh` and `trigger.yaml`.

**A tool that does not fire is a result, not a failure.** What a family with *no*
firing tools cannot do is distinguish "correctly detected nothing" from "the scan
never ran" - so the source-level tools below are the built-in negative control:

- Lizard
- jscpd
- Semgrep
- PyDriller
- git log --numstat

| Tool | Status | Why |
|---|---|---|
| Lizard | active | Python tokeniser. Class B: own parser, never loads a compiler, so the TFM is irrelevant  |
| jscpd | active | Node, own tokeniser. Class B. |
| Dolos | not-installed | Node, tree-sitter C# grammar. Class B, and the sheet's declaration ('C# language-version |
| Semgrep | active | Python, tree-sitter C# grammar. Class B. |
| Opengrep | not-installed | Standalone binary, absent from this host. Class B when present. NOTE: the sheet declares |
| Trivy | not-installed | Standalone Go binary, absent from this host. Class C: reads manifests/lockfiles. |
| PyDriller | active | Python, reads git only. Class C: never touches the source language. |
| git log --numstat | active | stdlib git. Class C. Counts Co-authored-by trailers, not just the author field -- the Ty |
| diff-cover | skipped | Class C: reads Cobertura/OpenCover XML. No coverage report can be produced on this famil |
| Roslyn analyzers (cognitive complexity) | not-installed | The sheet says 'Roslyn', which is a compiler platform, not a package. Resolved here to S |
| Roslyn SAST analyzers | not-installed | The sheet says 'roslyn sast', also not a package. Resolved here to SecurityCodeScan + Mi |
| Roslyn SemanticModel.AnalyzeDataFlow | not-installed | PROPOSED REPAIR, not in the source sheet. The sheet assigns Data Flow to Stryker.NET, a  |
| dotnet list package --vulnerable | skipped | Requires PackageReference and an SDK-style project. Structurally impossible on the packa |
| Coverlet | skipped | Coverlet ships netstandard2.0; its .NET Framework floor is net461. This family is below  |
| AltCover | not-installed | Declares .NET Framework 2.0+ via a net20 recorder, so it is the one coverage tool in the |
| MiniCover | skipped | MiniCover 3.10.0 declares .NET SDK 8.0/9.0/10.0 only -- verified against its own MiniCov |
| Stryker.NET | skipped | Requires an SDK-style project and a modern SDK to run. Also: the sheet makes it primary  |
| OpenTelemetry (.NET) + OTLP exporter | skipped | netstandard2.0 minimum, below this family. Also: tracing is not path coverage. The TypeS |
| unilyze | absent | NOT A TOOL. The only package of this name anywhere is PyPI `unilyze` 0.2.1, 'Get detaile |

## Build

```bash
./build.sh
```

Restore will not succeed on a host where NuGet is unreachable; see `dataset.json`
`blockedHosts`.

## Test

```bash
./build.sh          # the test step is wired into the build
```

19 tests are declared. They are **not** executed in this corpus's build, because no
test framework can be restored.

## Tool entry points

```bash
python3 tools/tool_integration.py            # banner
python3 tools/tool_integration.py --verify   # every runner present and executable
python3 tools/tool_integration.py --run      # run them all, compare to dataset.json
python3 tools/full_check.py                  # cross-file consistency audit
```

## What is expected to fire

`dataset.json` asserts the answer rather than describing it, so a platform that
reports nothing for a given tool can be *proven* correct instead of assumed correct.
Every runner's expected exit code is recorded there, and
`tool_integration.py --run` exits **2** if any tool produces a result the dataset
does not already record - a new finding, as distinct from a measurement.
