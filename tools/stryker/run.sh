#!/usr/bin/env bash
# stryker — mutation testing via dotnet-stryker (Stryker.NET)
set -uo pipefail
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
. "$REPO_ROOT/tools/_skip.sh"

require_cmd dotnet
dotnet restore "$REPO_ROOT/OrderKit.sln" >/dev/null 2>&1 || missing "dotnet restore failed (NuGet unreachable)"
require_restore

TOOL_DIR="$REPO_ROOT/.dotnet-tools"
export PATH="$TOOL_DIR:$PATH"
if ! dotnet stryker --help >/dev/null 2>&1; then
  dotnet tool install dotnet-stryker --tool-path "$TOOL_DIR" --version 4.0.0 \
    || missing "dotnet-stryker could not be installed (NuGet/tool feed unreachable)"
fi

if [ -z "${MSBUILD_EXE_PATH:-}" ]; then
  msbuild_dll="$(find /usr/share/dotnet/sdk -maxdepth 2 -name MSBuild.dll 2>/dev/null | sort -V | tail -1)"
  if [ -n "$msbuild_dll" ]; then
    export MSBUILD_EXE_PATH="$msbuild_dll"
  fi
fi

cd "$REPO_ROOT"
dotnet build OrderKit.sln -c Release --no-restore >/dev/null \
  || dotnet build OrderKit.sln -c Release

OUT_DIR="$(report_dir)"
CONFIG="$REPO_ROOT/tools/stryker/stryker-config.json"
FINDINGS_RC="1 2"

dotnet stryker --config-file "$CONFIG" --reporter json --reporter cleartext
rc=$?
accept_findings $rc || exit 1

report=""
for candidate in \
  "$REPO_ROOT/StrykerOutput"/*/reports/mutation-report.json \
  "$REPO_ROOT/StrykerOutput"/*/mutation-report.json \
  "$REPO_ROOT/mutation-report.json"; do
  if [ -f "$candidate" ]; then
    report="$candidate"
    break
  fi
done
if [ -z "$report" ]; then
  report="$(find "$REPO_ROOT" -name mutation-report.json -type f 2>/dev/null | head -1)"
fi
[ -n "$report" ] || exit 1

cp "$report" "$OUT_DIR/mutation-report.json"
python3 - "$OUT_DIR/mutation-report.json" "$OUT_DIR/stryker.json" <<'PY'
import json, sys
data = json.load(open(sys.argv[1]))
counts = {}
total = 0
for finfo in (data.get("files") or {}).values():
    for mut in finfo.get("mutants") or []:
        st = mut.get("status") or "Unknown"
        counts[st] = counts.get(st, 0) + 1
        total += 1
if total == 0:
    sys.exit("STRUCTURALLY ZERO: Stryker produced no mutants")
killed = counts.get("Killed", 0)
score = (killed / total * 100.0) if total else 0.0
json.dump({"tool": "stryker", "total_mutants": total, "mutation_score": score, "by_status": counts},
          open(sys.argv[2], "w"), indent=2)
print("stryker: %d mutants, score %.1f%%" % (total, score))
PY
exit $?
