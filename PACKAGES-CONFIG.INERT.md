# This cell cannot exist

packages.config does not exist on .NET Core. On the .NET Framework families this was the ONE workable packages.config cell, because Cake could drive MSBuild.exe and nuget.exe over a legacy csproj. Neither exists here, so the cell that survived the whole Framework line dies at the crossing.

The branch ships both artifacts the cell definition demands -- an SDK-style
project *and* a `packages.config` -- so the contradiction is visible in the
tree rather than resolved silently. `dataset.json` records
`branchFunctional: false` with this reason.

Precedent: the Python grid corpus shipped 12 uv branches per family with
complete uv configuration and `uv.lock.MISSING`, rather than quietly backing
them with pip. A branch labelled one thing that ran another underneath is the
exact silent success this corpus exists to catch.
