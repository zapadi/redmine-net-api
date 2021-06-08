
![Nuget](https://img.shields.io/nuget/dt/redmine-net-api)
![Redmine .NET Api](https://github.com/zapadi/redmine-net-api/workflows/Redmine%20.NET%20Api/badge.svg?branch=master)
![Appveyor last build status](https://ci.appveyor.com/api/projects/status/github/zapadi/redmine-net-api?branch=master&svg=true&passingText=master%20-%20OK&failingText=ups...)
[![NuGet package](https://img.shields.io/nuget/v/redmine-api.svg)](https://www.nuget.org/packages/redmine-api)  
<a href="https://www.buymeacoffee.com/vXCNnz9" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/lato-yellow.png" alt="Buy Me A Coffee" height="37" ></a>


# redmine-net-api   ![redmine-net-api logo](https://github.com/zapadi/redmine-net-api/blob/master/logo.png) 

redmine-net-api is a library for communicating with a Redmine project management application.

* Uses [Redmine's REST API.](http://www.redmine.org/projects/redmine/wiki/Rest_api/)
* Supports both XML and **JSON** formats.
* Supports GZipped responses from servers.
* This API provides access and basic CRUD operations (create, read, update, delete) for the resources described below:

|Resource  | Read   | Create   | Update   | Delete  |
|:---------|:------:|:----------:|:---------:|:-------:|
 Attachments        |   &check;  |  &check;  | &cross; |   &cross;|
 Custom Fields      |   &check;      |    &cross;     |   &cross;     |   &cross;
 Enumerations       |   &check;      |    &cross;     |   &cross;     |   &cross;
 Files              |&check;|&check;|&cross;|&cross;
 Groups             |&check;|&check;|&check;|&check;
 Issues             |&check;|&check;|&check;|&check;
 Issue Categories   |&check;|&check;|&check;|&check;
 Issue Relations    |&check;|&check;|&check;|&check;
 Issue Statuses     |&check;|&cross;|&cross;|&cross;
 My account         |&check;|&cross;|&check;|&cross;
 News               |&check;|&cross;|&cross;|&cross;
 Projects           |&check;|&check;|&check;|&check;
 Project Memberships|&check;|&check;|&check;|&check;
 Queries            |&check;|&cross;|&cross;|&cross;
 Roles              |&check;|&cross;|&cross;|&cross;
 Search             |
 Time Entries       |&check;|&check;|&check;|&check;
 Trackers           |&check;|&cross;|&cross;|&cross;
 Users              |&check;|&check;|&check;|&check;
 Versions           |&check;|&check;|&check;|&check;
 Wiki Pages         |&check;|&check;|&check;|&check;
 

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
