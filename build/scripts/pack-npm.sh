#!/usr/bin/env bash
# Pack + publish npm packages (@cleanarch/worker-types, @cleanarch/api-client).
set -euo pipefail
for p in packages/npm/@cleanarch/*/; do
  (cd "$p" && npm pack --pack-destination ../../../artifacts/npm && npm publish --access public)
done
