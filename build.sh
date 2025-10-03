#!/usr/bin/env bash
set -euo pipefail
# Bootstrap Cake. Requires the Cake tool from NuGet, which is refused at this
# host's egress proxy -- see dataset.json blockedHosts.
dotnet tool restore
dotnet cake "$@"
