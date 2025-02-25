#!/usr/bin/env bash
# coverlet
set -uo pipefail
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
. "$REPO_ROOT/tools/_skip.sh"

# --- why this tool has the status it has on this family -------------------
# Coverlet ships netstandard2.0, whose .NET Framework floor is net461. This family is net46 -- ONE PATCH RELEASE below it. Coverlet is the primary for 18 metrics (statement 5, branch 7, delta 6), so the whole coverage block turns on a single patch digit rather than on anything about the language or the tool.
skip "Coverlet ships netstandard2.0, whose .NET Framework floor is net461"

exit 0
