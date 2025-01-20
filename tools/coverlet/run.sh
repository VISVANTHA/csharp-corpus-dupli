#!/usr/bin/env bash
# coverlet
set -uo pipefail
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
. "$REPO_ROOT/tools/_skip.sh"

# --- why this tool has the status it has on this family -------------------
# Coverlet ships netstandard2.0; its .NET Framework floor is net461. This family is below that floor.
skip "Coverlet ships netstandard2.0; its .NET Framework floor is net461"

exit 0
