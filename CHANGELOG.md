# Changelog

## [Unreleased]

🚀 Features
*	Added Project.ProjectCustomFields to set and read project custom field values
*	A project read from Redmine now re-sends its custom field values (ProjectCustomFields) on update
*	WithWebClient is now honored; the function must return a new WebClient on every call (the library disposes it after each request and WebClientOptions are not applied to it)
*	RedmineApiException now includes Method, Endpoint, ResponseHeaders and IsTransient (timeouts, connection failures, HTTP 408, 429, 502, 503 and 504); IsTransient was previously always false

⚠️ Deprecations
*	Project.CustomFieldValues is obsolete; use ProjectCustomFields
*	Deprecated .NET Framework 2.0 and 4.0 targets: RedmineManager, IRedmineManager and RedmineManagerOptionsBuilder are marked [Obsolete] on those targets and will be removed in the next version (builds with TreatWarningsAsErrors will fail)

🛠 Fixes
*	Fixed Project custom field values being ignored by Redmine (XML now uses custom_fields; values are also sent on create)
*	Fixed invalid JSON for multi-value Project custom field values
*	Fixed Project XML create not associating issue custom fields (now sent as issue_custom_field_ids)
*	Fixed XML for empty custom field values (Values == null) being sent without an id
*	Fixed Equals on types with list properties (operator precedence made unequal objects compare equal)
*	Fixed JSON watcher_user_ids (Issue) and user_ids (Group) not being sent as arrays
*	Fixed Version status and sharing being sent when unset
*	Fixed invalid JSON for UpdateIssueAttachment
*	Fixed ChangeSet revision not being read from XML
*	Fixed XML dates and decimal values depending on the current culture (now invariant)
*	Fixed memory leak from XmlSerializer instances created on every array write
*	Fixed WithWebClient being ignored
*	Fixed DefaultHeaders removing authentication headers
*	UseProxy = true now uses the system proxy when no Proxy is configured
*	Fixed 422 validation errors not being thrown for Update, Delete and other calls without a result
*	Fixed async archive/unarchive project using the wrong HTTP method (now PUT) and project/repository action URLs being escaped
*	Fixed Count and Get modifying the caller's RequestOptions; CountAsync now sends limit=1
*	Fixed custom IRedmineWebClientOptions implementations not working
*	Unexpected request cancellation now throws instead of returning an empty result
*	Sync DownloadFile accepts absolute URLs (e.g. attachment ContentUrl), resolves relative addresses against the base address like WebClient, and no longer deadlocks on UI threads
*	WebClientOptions.Timeout now also bounds async API calls on .NET Framework; downloads are not cut off while reading the response body
*	Creating a RedmineManager no longer resets ServicePointManager.CheckCertificateRevocationList
*	Fixed obsolete async GetCurrentUserAsync and CountAsync ignoring their arguments
*	Empty responses now return null instead of throwing
*	Search filters set to false are no longer sent as enabled
*	Unknown XML/JSON elements, null nested JSON objects and unknown enum values (newer Redmine or plugins) no longer break deserialization
*	Fixed XML collection reader hanging on empty items
*	Clone now copies all fields and honours resetId (Issue.Clone(false) now keeps Id)
*	GetHashCode is now consistent with Equals for Issue and Membership
*	RequestOptions ContentType, Accept, UserAgent and Headers are now applied to the request; these properties were previously ignored, so values set on them are now sent
*	WebClientOptions.DefaultHeaders no longer override per-request or authentication headers

🧪 Tests
*	Unit tests now run in CI on net10.0 and fail the build on failure
*	Moved test projects to Microsoft.Testing.Platform with TRX reports

📦 Chore
*	Converted the solution to .slnx
*	Set minimum .NET SDK to 10.0.102 (global.json)
*	Added Central Package Management (Directory.Packages.props)
*	Moved signing and release notes to Directory.Build.targets; signed packages are now actually strong-named
*	CodeQL uses build-mode none (no build or SDK needed)
*	Modernized GitHub workflows (least-privilege permissions, concurrency, timeouts, latest actions)
*	Publishing to NuGet uses Trusted Publishing (OIDC) instead of an API key
*	Added package validation against the last published version (4.65.0); CI packs both packages on every build so breaking API changes fail the build
*	Split the HTTP transport from the Redmine API client internally (no behavior change)
*	CI also runs unit tests on net481 (Windows)
*	Removed deprecated PackageIconUrl and the unused PackageLicense property; local `dotnet pack` now writes to the repository's artifacts folder
*	Dependabot updates Newtonsoft.Json in its own group
*	All XmlSerializer instances now come from a single XmlSerializerCache
*	Added build provenance attestations for NuGet packages
*	Added Dependabot for GitHub Actions and NuGet
*	Removed AppVeyor

## [v4.50.0]

🚀 Features
*	Added download progress tracking (DownloadFileAsync with progress reporting) (#214)
*	Added hierarchical children support for issues (IssueChild) (#392)
*	Added extra properties to ApiResponseMessage and RedmineApiException
*	Introduced RedmineApiExceptionHelper and HttpConstants
*	Added GetProjectIssueCategories (sync & async) extensions
*	Implemented ProjectIssueCategory read/write overrides
*	Extended RedmineKeys with ISSUE_CUSTOM_FIELDS
*	Added custom field values property to the Project
*	Introduced InternalVisibleTo attribute
*	Added StatusCode property to ApiResponseMessage
*	Introduced EnumHelper and ToLowerInvariant enum extensions
*	Added RequestOptions static helpers and Headers property
*	Added generic Version enum parsing
*	Introduced IdentifiableName extensions, conversion operators, and factory method
*	Added IssueStatus constructor overload
*	Enabled serialization for the project default assignee and version
*	Added static methods for creating IssueCustomField with single/multiple values
*	Added GetMyAccountAsync extension
*	Added support for document categories

🛠 Fixes

*	Fixed IssueCustomField XML parsing when the element is empty
*	Fixed GetProjectNews URI escaping
*	Fixed deserialization of the project (issue) custom fields
*	Fixed User status serialization
*	Fixed ToInvariantString returning incorrect type
*	Fixed PageResult when total items = 0
*	Fixed Membership XML deserialization
*	Fixed Project custom field type handling
*	Fixed unnecessary URI escaping in RedmineManagesExtension
*	Fixed ProjectNews extension URL
*	Fixed ProjectMembership XML roles serialization
*	Fixed ReplaceEndings to respect the replacement parameter
*	Fixed SearchAsync extension
*	Fixed CreateWikiPage sync/async extensions
*	Fixed upload API URL to support ownerId


🔄 Refactors
*	Replaced ToString(CultureInvariant) with ToInvariantString()
*	Refactored integration tests, deserialization tests, and RedmineApiUrlsTests
*	Refactored XmlWriterExtensions
*	Refactored ApiResponseMessage extensions
*	Refactored internal RedmineApiWebClient send (a)sync logic
*	Removed redundant StatusCode handling (later reintroduced properly)
*	Changed Content type to string
*	Refactored IssueRelation & Version to use EnumHelper
*	Removed redundant properties from IRedmineApiClientOptions
*	Updated IssueCategory to not serialize the project id
*	Updated ProjectMembership to only serialize userId on create

🧪 Tests
*	Added integration support for local appsettings (non-CI)
*	Added hierarchical issue children deserialization tests
*	Added download progress tests
*	Added AssertDateTime helper
*	Added wiki XML deserialization tests
*	Refactored and cleaned up multiple test suites
*	Disabled time entry EqualExpressionComparison warning
*	Removed obsolete files and order logic
*	Upgraded xUnit to v3


📦 Chore
*	Simplified debugger display name
*	Disabled ServiceManager SYSLIB0014 warning

## [v4.4.0]

Added:
* Added ParentTitle to wiki page

Breaking Changes:

* Changed ChangeSet revision type from int to string

## [v4.3.0]

Added:
* Added WikiPageTitle, EstimatedHours & SpentHours to version
* Added IsAdmin, TwoFactorAuthenticationScheme, PasswordChangedOn, UpdatedOn to user
* Added IsActive to time entry activity
* Added IssuesVisibility, TimeEntriesVisibility, UsersVisibility & IsAssignable to role
* Added DefaultAssignee & DefaultVersion to project
* Added MyAccount type
* Added attachments & comments to news
* Added AllowedStatuses to issue
* Added search type

Fixes:
* Issue Relations Read Error for Copied Issues (Relation Type : copied_to) (#288)


## [v4.2.3]

Fixes:
* The only milliseconds component is set to Timeout. (#284)

## [v4.2.2]

Fixes:

* GetObjectsAsync<T> raises ArgumentNullException when should return null (#280)

## [v4.2.1]

* Small fixes.

## [v4.2.0]

* Small refactoring

## [v4.1.0]

Fixes: 

* Assigning IssueCustomFields to a project should be supported (#277)
* How to add a custom field to a project (#276)
* Wrong encoding of special characters in URLs causes 404 (#274)

## [v4.0.2]

Fixes: Add #236 to current version. 

## [v4.0.1]

Fixes:

* JSON serialization exception for issues with uploads (missing WriteStart/EndObject calls) (#271) (thanks muffmolch)

## [v4.0.0]

Features:

* Add support for .NET Standard 2.0 and 2.1

Fixes:

* Trackers - Cannot retreive List of trackers: Malformed objects (#265) (thanks NecatiMeral)
* IssueRelation - `relation_type` cannot be parsed (#263) (thanks NecatiMeral)
* Issue with 'relates" relation (#262) (thanks NecatiMeral)
* RedmineManager.GetObjects<>(params string[]) only retrieves 25 objects (#260)
* Unexpected ArgumentNullException in RedmineManager.cs:581 (#259)
* Type Issue and its property ParentIssue have no matching base-type (#258)
* Cannot set the name of the project (#257)
* Cant create new issue (#256)
* Help me with create issues api redmine. Error is The property or indexer 'Identifiable<IdentifiableName>.Id' cannot be used in this context because the set accessor is inaccessible (#254)
* Version 3.0.6.1 makes IssueCustomField.Info readonly breaking existing usage (#253)
* Empty response on CreateOrUpdateWikiPage (#245)
* Cannot set the status of a project (#255)
* Could not deserialize null!' When update WikiPage (#225) 

Breaking Changes:

* Split CreateOrUpdateWikiPage into CreateWikiPage & UpdateWikiPage
* Add IdentifiableName.Create<T>(id) in order to create identifiablename types with id. 