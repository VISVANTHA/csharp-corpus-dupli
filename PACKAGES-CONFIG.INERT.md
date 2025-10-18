# This cell cannot exist

`dotnet restore` has never supported packages.config. NuGet's packages.config path is implemented in MSBuild.exe / nuget.exe only.

The branch ships both artifacts the cell definition demands -- an SDK-style
project *and* a `packages.config` -- so the contradiction is visible in the
tree rather than resolved silently. `dataset.json` records
`branchFunctional: false` with this reason.

Precedent: the Python grid corpus shipped 12 uv branches per family with
complete uv configuration and `uv.lock.MISSING`, rather than quietly backing
them with pip. A branch labelled one thing that ran another underneath is the
exact silent success this corpus exists to catch.
