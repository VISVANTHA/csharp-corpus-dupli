#!/usr/bin/env bash
# opentelemetry
set -uo pipefail
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
. "$REPO_ROOT/tools/_skip.sh"

# --- why this tool has the status it has on this family -------------------
# netstandard2.0 minimum, so it clears at net461 and not here -- the same one-patch boundary that holds Coverlet out. Also: tracing is not path coverage. The TypeScript corpus shipped an OTel bootstrap that registered a no-op processor, exported nothing and exited 0 -- a structurally zero result.
skip "netstandard2.0 minimum, so it clears at net461 and not here -- the same one-patch boundary that holds Coverlet out"

exit 0
