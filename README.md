![](https://github.com/zapadi/redmine-net-api/blob/master/logo.png)
# redmine-net-api 

redmine-net-api is a library for communicating with a Redmine project management application.

* Uses [Redmine's REST API.](http://www.redmine.org/projects/redmine/wiki/Rest_api/)
* Supports both XML and JSON(requires .NET Framework 3.5 or higher) formats.
* Supports GZipped responses from servers.
* This API provides access and basic CRUD operations (create, read, update, delete) for the resources described below:
  * Attachments
  * Custom Fields
  * Enumerations  
  * Groups
  * Issues  
  * Issue Categories
  * Issue Relations
  * Issue Statuses
  * News(implementation for index only)
  * Projects
  * Project Memberships
  * Queries  
  * Roles
  * Time Entries
  * Trackers
  * Users
  * Versions
  * Wiki Pages

## Packages and Status

Package | Build status | Nuget
-------- | ------------ | -------
redmine-net20-api | ![alt text](https://ci.appveyor.com/api/projects/status/github/zapadi/redmine-net-api?branch=master&svg=true) | [![NuGet package](https://img.shields.io/nuget/v/redmine-api.svg)](https://www.nuget.org/packages/redmine-api.Web)  
redmine-net40-api | ![alt text](https://ci.appveyor.com/api/projects/status/github/zapadi/redmine-net-api?branch=master&svg=true) | [![NuGet package](https://img.shields.io/nuget/v/redmine-api.svg)](https://www.nuget.org/packages/redmine-api.Web)
redmine-net45-api | ![alt text](https://ci.appveyor.com/api/projects/status/github/zapadi/redmine-net-api?branch=master&svg=true) | [![NuGet package](https://img.shields.io/nuget/v/redmine-api.svg)](https://www.nuget.org/packages/redmine-api.Web)

## Contributing ##
Contributions are really appreciated!

A good way to get started (flow):

1. Fork the redmine-net-api repository.
2. Create a new branch in your current repos from the 'master' branch.
3. 'Check out' the code with Git or GitHub Desktop or SourceTree.
4. Push commits and create a Pull Request (PR) to redmine-net-api.

## License ##
The API is released under Apache 2 open-source license. You can use it for both personal and commercial purposes, build upon it and modify it.


