Contributions are really appreciated!

A good way to get started (flow):

1. Fork the redmine-net-api repository.
2. Create a new branch in your current repos from the `N.x` branch of the major version you are changing (e.g. `4.x`).
3. 'Check out' the code with *Git*, *GitHub Desktop* or *SourceTree*.
4. Push commits and create a Pull Request (PR) to redmine-net-api.

## Commit messages

Use [Conventional Commits](https://www.conventionalcommits.org/) (e.g. `fix: ...`, `feat(http): ...`).

## Versions and releases

The repository ships two NuGet packages, each with its own [SemVer](https://semver.org/) version (plus a strong-named `-signed` variant of each):

| Package | Project | Version |
| --- | --- | --- |
| `redmine-api` | `src/redmine-net-api` | `<Version>` in `redmine-net-api.csproj` |
| `redmine-api.http` | `src/redmine-net-api.Http` | `<Version>` in `redmine-net-api.Http.csproj` |

### Branches

- `N.x` (e.g. `4.x`, `5.x`): the release line of `redmine-api` major version `N`. Each `N.x` branch publishes all packages: `4.x` publishes stable `redmine-api` `4.*` releases; `5.x` publishes `5.0.0-alpha.N`, then `5.0.0-beta.N`, then `5.0.0` and stable `5.*` releases.
- `master`: the default branch, a mirror of the latest stable line. It never publishes. It is reset to the latest stable `N.x` (e.g. to `4.x` when the 4.x work is done, and to `5.x` once 5.0.0 is stable).
- Feature branches never publish: the release workflow runs only on `N.x` branches and fails on any other branch.

`redmine-api`'s major version equals the major version of its branch (`4.x` publishes `4.*`, `5.x` publishes `5.*`). On a stable line, do not make breaking changes to `redmine-api`: they go to the next major's `N.x` branch.

`redmine-api.http` has its own [SemVer](https://semver.org/) version, starting at 0.1.0. So that NuGet versions never collide, each `redmine-api.http` version range belongs to exactly one branch: `4.x` owns `0.*` (below 1.0.0 a breaking change bumps the minor version, so it stays in `0.*`); if `5.x` ships `redmine-api.http`, it starts at 1.0.0.

Pull requests target the `N.x` branch of the major version they change.

Every `N.x` branch must contain the release infrastructure: `.github/workflows/release.yml`, `build-and-test.yml` and `.github/scripts/`.

#### Resetting master

When a stable line is the latest one (e.g. `4.x` when the 4.x work is done, `5.x` once 5.0.0 is released), reset `master` to it, e.g. `git push --force-with-lease origin 5.x:master`. `master` has no release workflow trigger, so this publishes nothing.

### Releases

A package's version is the `<Version>` in its csproj, set by hand. To release:

1. In the pull request, bump `<Version>` following [SemVer](https://semver.org/) (`MAJOR.MINOR.PATCH` or `MAJOR.MINOR.PATCH-prerelease`, without build metadata) and add the matching section to `CHANGELOG.md`: `## [v4.67.0]` for `redmine-api`, `## [redmine-api.http v0.2.0]` for `redmine-api.http`.
2. Merge the pull request into the `N.x` branch.

On every push to an `N.x` branch, the release workflow compares each package's `<Version>` with nuget.org. Every package whose version is not on nuget.org yet, as both the package and its `-signed` variant, is tested, packed and published from the pushed commit (only the missing ids are packed and pushed). Then, once that version is on nuget.org (the publish succeeded, or it was published earlier), every package version without a GitHub release gets one: the workflow tags the pushed commit `<package-id>-v<version>` (e.g. `redmine-api-v4.67.0`, `redmine-api.http-v0.1.0`) and creates the release with the package's `CHANGELOG.md` section. An existing tag is never moved: the release is attached to it. A `redmine-api` release becomes the repository's latest release only if it is stable and not lower than any stable `redmine-api` tag (so a fix on an older line never does); prereleases and `redmine-api.http` releases never do. If every version is already on nuget.org and has a release, the workflow does nothing. It fails before publishing if a version is not valid SemVer, if a package has no `CHANGELOG.md` section for its version, or if `redmine-api`'s major version differs from the branch's.

A prerelease is just a prerelease version, e.g. `<Version>5.0.0-alpha.1</Version>`, then `5.0.0-beta.1` and `5.0.0`. Versions containing `-` get a GitHub prerelease.

`redmine-api.http` depends on the `redmine-api` version in core's csproj at the same commit. If it needs unreleased core changes, bump both versions in the same pull request; both are published by the same run.

Tags (e.g. `redmine-api-v4.65.1`, `redmine-api-v5.0.0-alpha.1`, `redmine-api.http-v0.1.0`) stay unique across branches because each package's version range is owned by one branch only.

### Manual release

Run the **Release** workflow manually, selecting the `N.x` branch in "Use workflow from", to recover from a failed release (or re-run the failed run). Like a push, it publishes whatever of the branch's versions is still missing on nuget.org, including a missing `-signed` variant, and then creates the missing tags and GitHub releases. It does nothing if everything is already published and released.

### NuGet publishing

Publishing uses the `nuget` GitHub environment. If deployment branch rules are configured for it (Settings > Environments > nuget), they must allow `*.x`.

Packages are pushed with nuget.org Trusted Publishing. Create a policy for the workflow file `release.yml` with environment `nuget` for each package id: `redmine-api`, `redmine-api-signed`, `redmine-api.http` and `redmine-api.http-signed`.

Package validation compares each package with the latest stable version on nuget.org that has the same major version and is not higher than the version being packed (e.g. 4.65.1 is compared with 4.65.0, never with a 5.x version). A new major version (e.g. 5.0.0, including its prereleases such as 5.0.0-alpha.1) has no baseline, so breaking changes are allowed. Below 1.0.0 the minor version must also match (e.g. 0.1.1 is compared with 0.1.0, 0.2.0 has no baseline). Locally, pass the baseline explicitly:

```bash
dotnet pack src/redmine-net-api/redmine-net-api.csproj -c Release -p:PackageValidationBaselineVersion=4.65.0
```

`CHANGELOG.md` is maintained by hand, with a section per package.
