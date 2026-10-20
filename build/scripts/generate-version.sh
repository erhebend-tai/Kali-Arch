#!/usr/bin/env bash
# Semantic version from git tags: v1.4.2 -> 1.4.2, no tag -> 1.0.0-ci.<sha>
set -euo pipefail
if git describe --tags --exact-match 2>/dev/null; then
  git describe --tags --exact-match | sed 's/^v//'
else
  echo "1.0.0-ci.$(git rev-parse --short HEAD)"
fi
