# Changelog

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