#!/usr/bin/env bash
# Registers this container as a self-hosted GitHub Actions runner.
set -euo pipefail
: "${GH_URL:?set GH_URL}" "${GH_TOKEN:?set GH_RUNNER_TOKEN}"
./config.sh --unattended --url "$GH_URL" --token "$GH_TOKEN" \
  --name "${RUNNER_NAME:-cleanarch-agent}" --work _work
./run.sh
