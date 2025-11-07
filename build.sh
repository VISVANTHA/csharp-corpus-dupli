#!/usr/bin/env bash
set -euo pipefail
dotnet restore OrderKit.sln
dotnet build OrderKit.sln -c Release --no-restore
dotnet test  OrderKit.sln -c Release --no-build
