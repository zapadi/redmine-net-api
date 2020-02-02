
![Nuget](https://img.shields.io/nuget/dt/redmine-net-api)
![Redmine .NET Api](https://github.com/zapadi/redmine-net-api/workflows/Redmine%20.NET%20Api/badge.svg?branch=master)
![Appveyor last build status](https://ci.appveyor.com/api/projects/status/github/zapadi/redmine-net-api?branch=master&svg=true&passingText=master%20-%20OK&failingText=ups...)
[![NuGet package](https://img.shields.io/nuget/v/redmine-api.svg)](https://www.nuget.org/packages/redmine-api)  
<a href="https://www.buymeacoffee.com/vXCNnz9" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/lato-yellow.png" alt="Buy Me A Coffee" height="37" ></a>


# redmine-net-api   ![redmine-net-api logo](https://github.com/zapadi/redmine-net-api/blob/master/logo.png) 

redmine-net-api is a library for communicating with a Redmine project management application.

* Uses [Redmine's REST API.](http://www.redmine.org/projects/redmine/wiki/Rest_api/)
* Supports both XML and **JSON(requires .NET Framework 3.5 or higher)** formats.
* Supports GZipped responses from servers.
* This API provides access and basic CRUD operations (create, read, update, delete) for the resources described below:

|Resource  | Read   | Create   | Update   | Delete  |
|:---------|:------:|:--------:|:--------:|:-------:|
 Attachments|x|x|-|-
 Custom Fields|x|-|-|-
 Enumerations  |x|-|-|-
 Files |x|x|-|-
 Groups|x|x|x|x
 Issues  |x|x|x|x
 Issue Categories|x|x|x|x
 Issue Relations|x|x|x|x
 Issue Statuses|x|-|-|-
 News|x|-|-|-
 Projects|x|x|x|x
 Project Memberships|x|x|x|x
 Queries  |x|-|-|-
 Roles |x|-|-|-
 Time Entries |x|x|x|x
 Trackers |x|-|-|-
 Users |x|x|x|x
 Versions |x|x|x|x
 Wiki Pages |x|x|x|x

## WIKI

Please review the ![wiki](https://github.com/zapadi/redmine-net-api/wiki) pages on how to use **redmine-net-api**.

## Contributing
Contributions are really appreciated!

A good way to get started (flow):

1. Fork the redmine-net-api repository.
2. Create a new branch in your current repos from the 'master' branch.
3. 'Check out' the code with *Git*, *GitHub Desktop*, *SourceTree*, *GitKraken*, *etc*.
4. Push commits and create a Pull Request (PR) to redmine-net-api.

## License
[![redmine-net-api](https://img.shields.io/hexpm/l/plug.svg)]()

The API is released under Apache 2 open-source license. You can use it for both personal and commercial purposes, build upon it and modify it.

## Thanks

I would like to thank:

* JetBrains for my Open Source ReSharper licence, 

* AppVeyor for allowing free build CI services for Open Source projects
