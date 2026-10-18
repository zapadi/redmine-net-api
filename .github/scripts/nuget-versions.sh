#!/usr/bin/env bash
# Prints the versions of a package published on nuget.org, one per line; prints nothing if the package does not exist.
set -euo pipefail

id=$(tr '[:upper:]' '[:lower:]' <<< "$1")
response=$(curl -sSL --retry 3 -w '\n%{http_code}' "https://api.nuget.org/v3-flatcontainer/$id/index.json")
status=${response##*$'\n'}

case "$status" in
  200) jq -r '.versions[]' <<< "${response%$'\n'*}" | sort -V ;;
  404) ;;
  *) echo "nuget.org returned HTTP $status for $1" >&2; exit 1 ;;
esac
