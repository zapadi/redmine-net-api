#!/usr/bin/env bash
# Prints a package version's section of CHANGELOG.md (the lines between its heading and the next "## " heading); fails if
# the section is missing.
set -euo pipefail

package=$1
version=$2

case "$package" in
  redmine-api) heading="## [v$version]" ;;
  *) heading="## [$package v$version]" ;;
esac

notes=$(awk -v heading="$heading" '{ sub(/\r$/, "") } $0 == heading { found = 1; next } found && /^## / { exit } found' "$(dirname "$0")/../../CHANGELOG.md")

if [[ -z "${notes//[[:space:]]/}" ]]; then
  echo "CHANGELOG.md has no '$heading' section." >&2
  exit 1
fi

echo "$notes"
