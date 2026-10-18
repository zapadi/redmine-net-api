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

Releases are automated with [release-please](https://github.com/googleapis/release-please) (`release-please-config.json`, `.release-please-manifest.json`):

1. Every push to `master` updates a single release pull request. A package is included only if commits since its last release changed files in its folder: `fix` bumps the patch version, `feat` the minor version and a breaking change (`feat!:` or a `BREAKING CHANGE:` footer) the major version (the minor version while a package is below 1.0.0). The first `redmine-api.http` release is 0.1.0. Changes outside the package folders (e.g. `Directory.Packages.props`, `src/Directory.Build.props`) do not trigger a release on their own.
2. Merging the release pull request bumps `<Version>` in the csproj, tags each released package (e.g. `redmine-api-v4.66.0`), creates the GitHub releases and publishes those packages to NuGet.
3. To force a version, add a `Release-As: x.y.z` footer to a commit that touches the package folder.

The release pull request is opened with the workflow's `GITHUB_TOKEN`, so CI does not run on it automatically, and the repository setting "Allow GitHub Actions to create and approve pull requests" must be enabled.

To publish manually, run the **Publish to NuGet** workflow on `master` and choose the package. It publishes the `<Version>` from the csproj and fails if that version is already on NuGet.

Package validation compares each package with its latest stable version on nuget.org. Locally, pass the baseline explicitly:

```bash
dotnet pack src/redmine-net-api/redmine-net-api.csproj -c Release -p:PackageValidationBaselineVersion=4.65.0
```

`CHANGELOG.md` is maintained by hand, with a section per package.
