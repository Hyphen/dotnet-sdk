![Hyphen AI](logo.png)

[![tests](https://github.com/Hyphen/dotnet-sdk/actions/workflows/tests.yaml/badge.svg)](https://github.com/Hyphen/dotnet-sdk/actions/workflows/tests.yaml)
[![NuGet](https://img.shields.io/nuget/v/Hyphen.Sdk?logo=nuget)](https://www.nuget.org/packages/Hyphen.Sdk)
[![NuGet](https://img.shields.io/nuget/vpre/Hyphen.Sdk?logo=nuget)](https://www.nuget.org/packages/Hyphen.Sdk)
[![license](https://img.shields.io/github/license/Hyphen/dotnet-sdk)](https://github.com/hyphen/dotnet-sdk/blob/main/LICENSE)

# Hyphen .NET SDK

The Hyphen .NET SDK is a .NET library that allows developers to easily integrate Hyphen features into their application, including:

<!--
- [Feature Flag Service](https://hyphen.ai/toggle) ("Toggle")
- [Secret Management Service](https://hyphen.ai/env) ("ENV")
- [Short Code Service](https://hyphen.ai/link) ("Link")
-->
- [Geo Information Service](https://hyphen.ai/net-info) ("net-info")

The library is compatible with .NET Standard 2.0 and .NET 8.

# Table of Contents

- [Installation](#installation)
- [Usage](#usage)
  - [Configuring the SDK](#configuring-the-sdk)
  - [net-info (Geo Information Service)](#net-info-geo-information-service)
- [Contributing](#contributing)
- [License and Copyright](#license-and-copyright)

# Installation

To install the Hyphen .NET SDK, you can use the .NET SDK command line (or your favorite IDE). From a terminal prompt, run:

```
dotnet add package Hyphen.Sdk
```

# Usage

## Configuring the SDK

The Hyphen SDK is designed to integrate with the host builder system developed by Microsoft, which is usable from command line applications as well as ASP.NET web applications. This system uses dependency injection to provide service instances to consuming code. We have provided sample applications in the [`samples`](./samples) folder.

You add each service you'd like to have access to via the host builder. For example, add access to the `INetInfo` service instance:

> ```csharp
> builder.Services.AddNetInfo();
> ```

You configure the services via options objects. You could bind configuration via `appsettings.json`:

> ```csharp
> builder
> 	.Configuration
> 	.SetBasePath(AppContext.BaseDirectory)
> 	.AddJsonFile("appsettings.json", optional: false);
>
> builder.Services.Configure<NetInfoOptions>(builder.Configuration.GetSection("YOUR_SECTION_NAME"));
> ```
>
> `appsettings.json`:
>
> ```json
> {
> 	"YOUR_SECTION_NAME": {
> 		"ApiKey": "PUT_YOUR_API_KEY_HERE"
> 	}
> }
> ```

or via code:

> ```csharp
> builder.Services.Configure<NetInfoOptions>(options =>
> {
> 	options.ApiKey = "PUT_YOUR_API_KEY_HERE";
> });
> ```

Then you can either retrieve a service instance via the host:

> ```csharp
> var netInfo = host.Services.GetRequiredService<INetInfo>();
> ```

or (more commonly) through constructor injection, for any service class you write:

> ```csharp
> public MyClass(INetInfo netInfo)
> {
> 	// Use the `netInfo` value in any method or property
> }
> ```

The service instance types (and their configuration options object) include:

Service  | Interface  | Options
---------|------------|--------
net-info | `INetInfo` | `NetInfoOptions`

For more information on these patterns, see the Microsoft documentation:

- [Dependency Injection](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection)
- [Options Pattern](https://learn.microsoft.com/dotnet/core/extensions/options)

## net-info (Geo Information Service)

The Hyphen .NET SDK provides an `INetInfo` service interface which allows you to fetch geo information about an IP address. This can be useful for debugging or logging purposes. You can read more about it:

* [Website](https://hyphen.ai/net-info)
* [https://net.info](https://net.info) (Web service URI used by the SDK)
* [Quick Start Guide](https://docs.hyphen.ai/docs/netinfo-quickstart)

The `INetInfo` interface contains the following methods:

Method                     | Description
---------------------------|------------
`GetIPInfo()`              | Retrieves information about the current computer's public IP address
`GetIPInfo(string ip)`     | Retrieves information about the given IP address
`GetIPInfos(string[] ips)` | Retrieves information about several IP addresses

_There are also overloads of each method that accept a `CancellationToken`._

The first two overloads return a single instance of `NetInfoResult`, while the multi-IP overload returns `NetInfoResult[]` (one for each requested IP address).

The `NetInfoResult` class includes the following properties:

Property       | Description
---------------|------------
`IP`           | The IP address that was requested
`Type`         | The result type (one of `Public`, `Private`, or `Error`)
`Location`     | The location information (only available for type `Public`)
`ErrorMessage` | The error that occurred (only available for type `Error`)

The `Location` value includes the latitude/longitude, region information (country/region/city/postal code) and time zone (in TZ identifier form like `America/New_York`, suitable for passing to [`TimeZoneInfo.FindSystemTimeZoneById`](https://learn.microsoft.com/dotnet/api/system.timezoneinfo.findsystemtimezonebyid)).

_Note that some geographic mappings are relatively precise and will include all the information, while others may be missing one or more fields. In this case, the specific location properties may be empty rather than filled._

# Contributing

We welcome contributions to the Hyphen .NET SDK! If you have an idea for a new feature, bug fix, or improvement, please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Run the tests to ensure everything is working correctly: `dotnet test`.
4. Make your changes and commit them with a clear message. In the following format:
   - `feat: <describe the feature>`
   - `fix: <describe the bug fix>`
   - `chore: upgrading xxx to version x.x.x`
5. Push your changes to your forked repository.
6. Create a pull request to the main repository.
7. Describe your changes and why they are necessary.

# License and Copyright

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
The copyright for this project is held by Hyphen, Inc. All rights reserved.
