#!/usr/bin/env bash
# Starts the 4 .NET services and the React admin UI. Press Ctrl+C to stop everything.
set -e
cd "$(dirname "$0")/.."
trap 'kill 0' EXIT
for svc in Products Orders Marketing Worker; do
  dotnet run --project "src/Services/$svc" &
done
(cd web/admin-ui && npm install --no-audit --no-fund && npm run dev) &
wait
