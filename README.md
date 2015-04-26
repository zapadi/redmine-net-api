# redmine-net-api

redmine-net-api is a library for communicating with a Redmine project management application

* Uses [Redmine's REST API.](http://www.redmine.org/projects/redmine/wiki/Rest_api/)
* Supports both XML and JSON(requires .NET Framework 3.5 or higher) formats
* Supports GZipped responses from servers
* This API provides access and basic CRUD operations (create, update, delete) for the resources described below
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

**Authentication**

Most of the time, the API requires authentication. To enable the API-style authentication, you have to check Enable REST API in Administration -> Settings -> Authentication. Then, authentication can be done in 2 different ways:
using your regular login/password via HTTP Basic authentication.
using your API key which is a handy way to avoid putting a password in a script. The API key may be attached to each request in one of the following way:
  * passed in as a "key" parameter
  * passed in as a username with a random password via HTTP Basic authentication
  * passed in as a "X-Redmine-API-Key" HTTP header (added in Redmine 1.1.0)
You can find your API key on your account page ( /my/account ) when logged in, on the right-hand pane of the default layout.

**User Impersonation**

As of Redmine 2.2.0, you can impersonate user through the REST API by setting the X-Redmine-Switch-User header of your API request. It must be set to a user login (eg. X-Redmine-Switch-User: jsmith). This only works when using the API with an administrator account, this header will be ignored when using the API with a regular user account.

If the login specified with the X-Redmine-Switch-User header does not exist or is not active, you will receive a 412 error response.


<br />
## Help me help you ##
Your feedback is crucial. If you find anything that could improve the API, let me know. I will gladly receive your input and make the proper adjustments.

**If you find this API useful let others know about it and/or rate it.**<br />
Your contribution is always welcome.

<br />
## Licence ##
The API is released under Apache 2 open-source license. You can use it for both personal and commercial purposes, build upon it and modify it.


