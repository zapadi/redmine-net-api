# Changelog

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