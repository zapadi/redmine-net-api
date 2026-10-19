#!/usr/bin/env bash
# Prints the package validation baseline for a package version: the latest stable version of the package on nuget.org
# with the same major version and not higher than it; prints nothing if there is none (e.g. a new major version).
set -euo pipefail

id=$1
version=${2%%-*}
major=${version%%.*}
published=$("$(dirname "$0")/nuget-versions.sh" "$id")

baseline=
while read -r candidate; do
  if [[ $(printf '%s\n' "$candidate" "$version" | sort -V | tail -n 1) == "$version" ]]; then
    baseline=$candidate
  fi
done < <(awk -F. -v major="$major" '!/-/ && $1 == major' <<< "$published" | sort -V)

if [[ -n "$baseline" ]]; then
  echo "$baseline"
fi
