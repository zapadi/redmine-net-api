#!/usr/bin/env bash
# Prints the package validation baseline for a package version: the latest stable version of the package on nuget.org
# that is not higher than it and has the same major version (same major and minor for 0.x, where the minor version is the
# breaking boundary); prints nothing if there is none (e.g. a new major version).
set -euo pipefail

id=$1
version=${2%%-*}
IFS=. read -r major minor _ <<< "$version"
published=$("$(dirname "$0")/nuget-versions.sh" "$id")

baseline=
while read -r candidate; do
  if [[ $(printf '%s\n' "$candidate" "$version" | sort -V | tail -n 1) == "$version" ]]; then
    baseline=$candidate
  fi
done < <(awk -F. -v major="$major" -v minor="$minor" '!/-/ && $1 == major && (major != 0 || $2 == minor)' <<< "$published" | sort -V)

if [[ -n "$baseline" ]]; then
  echo "$baseline"
fi
