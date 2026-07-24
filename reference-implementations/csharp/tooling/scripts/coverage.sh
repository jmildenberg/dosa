#!/usr/bin/env bash
# Runs every test project's coverlet (Microsoft.Testing.Platform) coverage collector and merges
# the results into a single ReportGenerator HTML report at artifacts/coverage/report/. File
# prefixes come from each project's TestingPlatformCommandLineArguments (see Directory.Build.props).
set -euo pipefail

cd "$(dirname "${BASH_SOURCE[0]}")/../.."

CONFIGURATION="${1:-Debug}"
OUTPUT_DIR="artifacts/coverage"

rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

dotnet tool restore
dotnet test ToDo.slnx -c "$CONFIGURATION" -- \
    --coverlet \
    --coverlet-output-format cobertura \
    --results-directory "$OUTPUT_DIR"

dotnet reportgenerator \
    -reports:"$OUTPUT_DIR/*.xml" \
    -targetdir:"$OUTPUT_DIR/report" \
    -reporttypes:Html \
    -assemblyfilters:+ToDo.*

echo "Coverage reports written to $OUTPUT_DIR"
echo "Merged report: $OUTPUT_DIR/report/index.html"
