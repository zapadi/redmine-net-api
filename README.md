# ![Redmine .NET API](https://raw.githubusercontent.com/zapadi/redmine-net-api/master/logo.png) redmine-net-api

[![NuGet](https://img.shields.io/nuget/v/redmine-api.svg)](https://www.nuget.org/packages/redmine-api) 
[![NuGet Downloads](https://img.shields.io/nuget/dt/redmine-api)](https://www.nuget.org/packages/redmine-api) 
[![License](https://img.shields.io/github/license/zapadi/redmine-net-api)](LICENSE)
[![Contributors](https://img.shields.io/github/contributors/zapadi/redmine-net-api)](https://github.com/zapadi/redmine-net-api/graphs/contributors)


A modern and flexible .NET client library to interact with [Redmine](https://www.redmine.org)'s REST API. Supports XML and JSON formats with GZipped responses for improved performance.


## 🚀 Features

- Full REST API support with CRUD operations
- Supports both XML and JSON data formats
- Handles GZipped server responses transparently
- Easy integration via NuGet package
- Actively maintained and community-driven

| Resource             | Read | Create | Update | Delete |
|----------------------|:----:|:------:|:------:|:------:|
| Attachments          | ✅   | ✅     | ❌     | ❌     |
| Custom Fields        | ✅   | ❌     | ❌     | ❌     |
| Enumerations         | ✅   | ❌     | ❌     | ❌     |
| Files                | ✅   | ✅     | ❌     | ❌     |
| Groups               | ✅   | ✅     | ✅     | ✅     |
| Issues               | ✅   | ✅     | ✅     | ✅     |
| Issue Categories     | ✅   | ✅     | ✅     | ✅     |
| Issue Relations      | ✅   | ✅     | ✅     | ✅     |
| Issue Statuses       | ✅   | ❌     | ❌     | ❌     |
| My Account           | ✅   | ❌     | ✅     | ❌     |
| News                 | ✅   | ✅     | ✅     | ✅     |
| Projects             | ✅   | ✅     | ✅     | ✅     |
| Project Memberships  | ✅   | ✅     | ✅     | ✅     |
| Queries              | ✅   | ❌     | ❌     | ❌     |
| Roles                | ✅   | ❌     | ❌     | ❌     |
| Search               | ✅   |        |        |        |
| Time Entries         | ✅   | ✅     | ✅     | ✅     |
| Trackers             | ✅   | ❌     | ❌     | ❌     |
| Users                | ✅   | ✅     | ✅     | ✅     |
| Versions             | ✅   | ✅     | ✅     | ✅     |
| Wiki Pages           | ✅   | ✅     | ✅     | ✅     |


## 📦 Installation

Add the package via NuGet:

```bash
dotnet add package Redmine.Net.Api
```

Or via Package Manager Console:

```powershell
Install-Package Redmine.Net.Api
```


## 🧑‍💻 Usage Example

```csharp
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var options = new RedmineManagerOptions
        {
            BaseAddress = "https://your-redmine-url",
            ApiKey = "your-api-key"
        };

        var manager = new RedmineManager(options);

        // Retrieve an issue asynchronously
        var issue = await manager.GetObjectAsync<Issue>(12345);
        Console.WriteLine($"Issue subject: {issue.Subject}");
    }
}
```
Explore more usage examples on the [Wiki](https://github.com/zapadi/redmine-net-api/wiki).


## 📚 Documentation

Detailed API reference, guides, and tutorials are available in the [GitHub Wiki](https://github.com/zapadi/redmine-net-api/wiki).


## 🙌 Contributing

We welcome contributions! Here's how you can help:

1. Fork the repository
2. Create a new branch (`git checkout -b feature/my-feature`)
3. Make your changes and commit (`git commit -m 'Add some feature'`)
4. Push to your fork (`git push origin feature/my-feature`)
5. Open a Pull Request

See the [CONTRIBUTING.md](CONTRIBUTING.md) for detailed guidelines.


## 🤝 Contributors

Thanks to all contributors!

<a href="https://github.com/zapadi/redmine-net-api/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=zapadi/redmine-net-api" />
</a>


## 📝 License

This project is licensed under the [Apache License 2.0](LICENSE).


## ☕ Support

If you find this project useful, consider ![[buying me a coffee](https://cdn.buymeacoffee.com/buttons/lato-yellow.png)](https://www.buymeacoffee.com/vXCNnz9) to support development.

