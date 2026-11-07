Contributions are really appreciated!

A good way to get started (flow):

1. Fork the redmine-net-api repository.
2. Create a new branch in your current repos from the 'master' branch.
3. 'Check out' the code with *Git*, *GitHub Desktop* or *SourceTree*.
4. Push commits and create a Pull Request (PR) to redmine-net-api.

## Commit messages

Use [Conventional Commits](https://www.conventionalcommits.org/) (e.g. `fix: ...`, `feat(http): ...`). They decide the next version of each package.

## Versions and releases

The repository ships two NuGet packages, each with its own [SemVer](https://semver.org/) version (plus a strong-named `-signed` variant of each):

| Package | Project | Version |
| --- | --- | --- |
| `redmine-api` | `src/redmine-net-api` | `<Version>` in `redmine-net-api.csproj` |
| `redmine-api.http` | `src/redmine-net-api.Http` | `<Version>` in `redmine-net-api.Http.csproj` |

### Branches

- `master`: current development.
- `N.x` (e.g. `4.x`, `5.x`): release and maintenance branch of major version `N`. Current `redmine-api` 4.x releases (4.66.0 onwards) are made from `4.x`. Create a new `N.x` branch from the last release of that major when `master` moves on (e.g. `git switch -c 5.x redmine-api-v5.3.0`; tags in this form exist from 4.66.0 onwards).

Pull requests target `master`, or the `N.x` branch for a change to that major version.

Every release branch must contain the release infrastructure: `.github/workflows/release.yml`, `publish.yml`, `build-and-test.yml`, `.github/scripts/` and its own `release-please-config.json` and `.release-please-manifest.json`. A branch releases and publishes only the packages listed in its own `release-please-config.json`; the publish workflow fails for any other package.

`redmine-api.http` is released only from `master`, so it is listed only in `master`'s config. When this work is merged into `master`, add `src/redmine-net-api.Http` to `master`'s config (component `redmine-api.http`, `initial-version` `0.1.0`, `extra-files` `redmine-net-api.Http.csproj`) and manifest (`0.0.0`), and set the `redmine-api` version line for `master` (e.g. `"release-as": "5.0.0"` on the core package for the next major, removed after that release).

### Releases

Releases are automated with [release-please](https://github.com/googleapis/release-please). The release workflow runs only on `master` and `*.x` branches, using that branch's config and manifest:

1. Every push to the branch updates that branch's release pull request. A package is included only if commits since its last release changed files in its folder: `fix` bumps the patch version, `feat` the minor version and a breaking change (`feat!:` or a `BREAKING CHANGE:` footer) the major version (the minor version while a package is below 1.0.0). The first `redmine-api.http` release is 0.1.0. Changes outside the package folders (e.g. `Directory.Packages.props`, `src/Directory.Build.props`) do not trigger a release on their own.
2. Merging the release pull request bumps `<Version>` in the csproj, tags each released package (e.g. `redmine-api-v4.66.0`), creates the GitHub releases and publishes those packages to NuGet from the released commit.
3. To force a version, add a `Release-As: x.y.z` footer to a commit that touches the package folder.

`redmine-api.http` depends on the `redmine-api` version in core's csproj. Core changes that `redmine-api.http` needs must be committed as `feat`/`fix` (or carry a `Release-As` footer) so that `redmine-api` is released before or together with `redmine-api.http`.

The release pull request is opened with the workflow's `GITHUB_TOKEN`, so CI does not run on it, and the repository setting "Allow GitHub Actions to create and approve pull requests" must be enabled. If status checks are required, close and reopen the pull request, or run **Build and Test** manually on the `release-please--branches--*` branch.

Tags (e.g. `redmine-api-v4.65.1`, `redmine-api-v5.0.0`) stay unique across branches because each major version is released from one branch only.

### Manual publish

Run the **Publish to NuGet** workflow, select the branch in "Use workflow from" and choose the package. It publishes the `<Version>` from that branch's csproj and fails if that version is already on NuGet or the package is not in the branch's `release-please-config.json`. Use it only to recover a failed release (prefer "Re-run failed jobs" on the failed release run) or after bumping `<Version>` by hand, together with `.release-please-manifest.json`.

### NuGet publishing

Publishing uses the `nuget` GitHub environment. If deployment branch rules are configured for it (Settings > Environments > nuget), they must allow `master` and `*.x`.

Packages are pushed with nuget.org Trusted Publishing. Automatic releases call `publish.yml` from `release.yml`, so create a policy for both workflow files (`release.yml` and `publish.yml`, environment `nuget`) for each package id: `redmine-api`, `redmine-api-signed`, `redmine-api.http` and `redmine-api.http-signed`.

Package validation compares each package with the latest stable version on nuget.org that has the same major version and is not higher than the version being packed (e.g. 4.65.1 is compared with 4.65.0, never with a 5.x version). A new major version (e.g. 5.0.0) has no baseline, so breaking changes are allowed. Below 1.0.0 the minor version must also match (e.g. 0.1.1 is compared with 0.1.0, 0.2.0 has no baseline). Locally, pass the baseline explicitly:

```bash
dotnet pack src/redmine-net-api/redmine-net-api.csproj -c Release -p:PackageValidationBaselineVersion=4.65.0
```

`CHANGELOG.md` is maintained by hand, with a section per package.
