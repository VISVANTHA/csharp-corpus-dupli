# This cell cannot exist

packages.config is only supported by non-SDK-style (legacy) projects. An SDK-style csproj ignores it entirely: NuGet restore reads PackageReference and the packages.config file is inert.

The branch ships both artifacts the cell definition demands -- an SDK-style
project *and* a `packages.config` -- so the contradiction is visible in the
tree rather than resolved silently. `dataset.json` records
`branchFunctional: false` with this reason.

Precedent: the Python grid corpus shipped 12 uv branches per family with
complete uv configuration and `uv.lock.MISSING`, rather than quietly backing
them with pip. A branch labelled one thing that ran another underneath is the
exact silent success this corpus exists to catch.
