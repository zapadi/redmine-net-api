Contributions are really appreciated!

A good way to get started (flow):

1. Fork the redmine-net-api repository.
2. Create a new branch in your current repos from the `N.x` branch of the major version you are changing (e.g. `4.x`).
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

- `N.x` (e.g. `4.x`, `5.x`): the release line of `redmine-api` major version `N`. Each `N.x` branch publishes all packages: `4.x` publishes stable `redmine-api` `4.*` releases; `5.x` publishes `5.0.0-alpha.N`, then `5.0.0-beta.N`, then `5.0.0` and stable `5.*` releases.
- `master`: the default branch, a mirror of the latest stable line. It never publishes. It is reset to the latest stable `N.x` (e.g. to `4.x` when the 4.x work is done, and to `5.x` once 5.0.0 is stable).
- Feature branches never publish: the release workflow runs only on `N.x` branches, and the publish workflow fails on any other branch.

`redmine-api`'s major version equals the major version of its branch (`4.x` publishes `4.*`, `5.x` publishes `5.*`). On a stable line, do not mark `redmine-api` commits as breaking (`!` or a `BREAKING CHANGE:` footer): breaking changes go to the next major's `N.x` branch.

`redmine-api.http` has its own [SemVer](https://semver.org/) version, starting at 0.1.0. So that NuGet versions never collide, each `redmine-api.http` version range belongs to exactly one branch: `4.x` owns `0.*` (below 1.0.0 a breaking change bumps the minor version, so it stays in `0.*`); if `5.x` ships `redmine-api.http`, it starts at 1.0.0.

Pull requests target the `N.x` branch of the major version they change.

Every `N.x` branch must contain the release infrastructure: `.github/workflows/release.yml`, `publish.yml`, `build-and-test.yml`, `.github/scripts/` and its own `release-please-config.json` and `.release-please-manifest.json`. A branch releases and publishes only the packages listed in its own `release-please-config.json`, and the publish workflow fails for any other package. A package released for the first time needs no manifest entry: release-please uses its `initial-version` (e.g. `"initial-version": "0.1.0"` for `redmine-api.http` on `4.x`).

#### Prereleases of a new major

A new major's branch (e.g. `5.x`) sets, at the top level of its `release-please-config.json`:

```json
"versioning": "prerelease",
"prerelease-type": "alpha",
"prerelease": true
```

Start the first alpha with a `Release-As: 5.0.0-alpha.1` footer on a commit that touches only `src/redmine-net-api`. An empty commit would apply the footer to every package, including `redmine-api.http`. Start `redmine-api.http` on `5.x` the same way, with `Release-As: 1.0.0-alpha.1` on a commit that touches only `src/redmine-net-api.Http`.

Later `fix`, `feat` and breaking commits bump only the prerelease number (`5.0.0-alpha.2`, ...). To move to beta, set `"prerelease-type": "beta"` and start it with `Release-As: 5.0.0-beta.1` (changing the type alone does not change an existing alpha version).

To release the stable version, remove `versioning`, `prerelease-type` and `prerelease` from `release-please-config.json`, push a commit that touches only `src/redmine-net-api` with `Release-As: 5.0.0` (and one for `redmine-api.http` with `Release-As: 1.0.0`, if it ships from `5.x`) and merge the release pull request. From then on the branch publishes stable `5.*` releases.

#### Resetting master

When a stable line is the latest one (e.g. `4.x` when the 4.x work is done, `5.x` once 5.0.0 is released), reset `master` to it, e.g. `git push --force-with-lease origin 5.x:master`. `master` has no release workflow trigger, so this publishes nothing.

### Releases

Releases are automated with [release-please](https://github.com/googleapis/release-please). The release workflow runs only on `N.x` branches, using that branch's config and manifest:

1. Every push to the branch updates that branch's release pull request. A package is included only if commits since its last release changed files in its folder: `fix` bumps the patch version, `feat` the minor version and a breaking change (`feat!:` or a `BREAKING CHANGE:` footer) the major version (the minor version while a package is below 1.0.0). The first `redmine-api.http` release is 0.1.0. Changes outside the package folders (e.g. `Directory.Packages.props`, `src/Directory.Build.props`) do not trigger a release on their own.
2. Merging the release pull request bumps `<Version>` in the csproj, tags each released package (e.g. `redmine-api-v4.66.0`), creates the GitHub releases and publishes those packages to NuGet from the released commit.
3. To force a version, add a `Release-As: x.y.z` footer to a commit that touches only that package's folder. Do not use an empty commit: it applies the footer to every package, which would move `redmine-api.http` out of its version range.

`redmine-api.http` depends on the `redmine-api` version in core's csproj. Core changes that `redmine-api.http` needs must be committed as `feat`/`fix` (or carry a `Release-As` footer) so that `redmine-api` is released before or together with `redmine-api.http`.

The release pull request is opened with the workflow's `GITHUB_TOKEN`, so CI does not run on it, and the repository setting "Allow GitHub Actions to create and approve pull requests" must be enabled. If status checks are required, close and reopen the pull request, or run **Build and Test** manually on the `release-please--branches--*` branch.

Tags (e.g. `redmine-api-v4.65.1`, `redmine-api-v5.0.0-alpha.1`, `redmine-api.http-v0.1.0`) stay unique across branches because each package's version range is owned by one branch only.

### Manual publish

Run the **Publish to NuGet** workflow, select the `N.x` branch in "Use workflow from" and choose the package. It publishes the `<Version>` from that branch's csproj and fails if that version is already on NuGet or the package is not in the branch's `release-please-config.json`. Use it only to recover a failed release (prefer "Re-run failed jobs" on the failed release run) or after bumping `<Version>` by hand, together with `.release-please-manifest.json`.

### NuGet publishing

Publishing uses the `nuget` GitHub environment. If deployment branch rules are configured for it (Settings > Environments > nuget), they must allow `*.x`.

Packages are pushed with nuget.org Trusted Publishing. Automatic releases call `publish.yml` from `release.yml`, so create a policy for both workflow files (`release.yml` and `publish.yml`, environment `nuget`) for each package id: `redmine-api`, `redmine-api-signed`, `redmine-api.http` and `redmine-api.http-signed`.

Package validation compares each package with the latest stable version on nuget.org that has the same major version and is not higher than the version being packed (e.g. 4.65.1 is compared with 4.65.0, never with a 5.x version). A new major version (e.g. 5.0.0, including its prereleases such as 5.0.0-alpha.1) has no baseline, so breaking changes are allowed. Below 1.0.0 the minor version must also match (e.g. 0.1.1 is compared with 0.1.0, 0.2.0 has no baseline). Locally, pass the baseline explicitly:

```bash
dotnet pack src/redmine-net-api/redmine-net-api.csproj -c Release -p:PackageValidationBaselineVersion=4.65.0
```

`CHANGELOG.md` is maintained by hand, with a section per package.
