#!/usr/bin/env bash
# OpenTelemetry (.NET) — trace spans emitted during dotnet test
set -uo pipefail
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
. "$REPO_ROOT/tools/_skip.sh"

require_cmd dotnet
dotnet restore "$REPO_ROOT/OrderKit.sln" >/dev/null 2>&1 || missing "dotnet restore failed (NuGet unreachable)"
require_restore

OUT_DIR="$(report_dir)"
export OTEL_REPORT_DIR="$OUT_DIR"

cd "$REPO_ROOT"
dotnet build OrderKit.sln -c Release --no-restore >/dev/null \
  || dotnet build OrderKit.sln -c Release

FINDINGS_RC="1"
dotnet test OrderKit.sln -c Release --no-build --filter "FullyQualifiedName~TelemetryExportTests"
rc=$?
accept_findings $rc || exit 1

[ -f "$OUT_DIR/otel-spans.json" ] || exit 1

python3 - "$OUT_DIR/otel-spans.json" "$OUT_DIR/opentelemetry.json" <<'PY'
import json, sys
data = json.load(open(sys.argv[1]))
spans = data.get("spans") or []
if not spans:
    sys.exit("STRUCTURALLY ZERO: no spans recorded")
paths = {s.get("name") for s in spans if s.get("name")}
json.dump({"tool": "opentelemetry", "span_count": len(spans), "unique_path_count": len(paths)},
          open(sys.argv[2], "w"), indent=2)
print("opentelemetry: %d spans, %d unique names" % (len(spans), len(paths)))
PY
exit $?
