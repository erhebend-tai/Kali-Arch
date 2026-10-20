#!/usr/bin/env bash
# Pack + push NuGet packages to the feed (nuget.org or GitHub Packages).
set -euo pipefail
VERSION="${1:?usage: pack-nuget.sh <version>}"
for p in tools/analyzers/CleanArch.Analyzers \
         tools/codefixes/CleanArch.CodeFixes \
         shared/CleanArch.Shared.Contracts \
         shared/CleanArch.Shared.Domain; do
  dotnet pack "$p" -c Release -o artifacts/nuget /p:Version="$VERSION"
done
# dotnet nuget push artifacts/nuget/*.nupkg -k "$NUGET_API_KEY" -s "$NUGET_SOURCE"
