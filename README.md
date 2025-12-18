![Hyphen AI](https://raw.githubusercontent.com/Hyphen/hyphen-design-tokens/refs/heads/main/assets/images/hyphen-symbol-rgb.svg)

[![tests](https://github.com/Hyphen/dotnet-sdk/actions/workflows/tests.yaml/badge.svg)](https://github.com/Hyphen/dotnet-sdk/actions/workflows/tests.yaml)
[![NuGet](https://img.shields.io/nuget/v/Hyphen.Sdk?logo=nuget)](https://www.nuget.org/packages/Hyphen.Sdk)
[![NuGet](https://img.shields.io/nuget/vpre/Hyphen.Sdk?logo=nuget)](https://www.nuget.org/packages/Hyphen.Sdk)
[![license](https://img.shields.io/github/license/Hyphen/dotnet-sdk)](https://github.com/hyphen/dotnet-sdk/blob/main/LICENSE)

# Hyphen .NET SDK

The Hyphen .NET SDK is a .NET library that allows developers to easily integrate Hyphen features into their application, including:

- [Secret Management Service](https://hyphen.ai/env) ("ENV")
- [Geo Information Service](https://hyphen.ai/net-info) ("net-info")

The library is compatible with .NET Standard 2.0 and .NET 8.

# Table of Contents

- [Installation](#installation)
- [Usage](#usage)
  - [Configuring the SDK](#configuring-the-sdk)
  - [ENV (Secret Management Service)](#env-secret-management-service)
    - [Adding ENV](#adding-env)
    - [Configuring ENV](#configuring-env)
    - [Using ENV](#using-env)
  - [net-info (Geo Information Service)](#net-info-geo-information-service)
    - [Adding net-info](#adding-net-info)
    - [Configuring net-info](#configuring-net-info)
    - [Using net-info](#using-net-info)
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

```csharp
builder.Services.AddNetInfo();
```

You configure the services via options objects. There are at least three ways you can configuration those options:

1. You can bind configuration via a settings file, like `appsettings.json`:

   ```csharp
   builder
       .Configuration
       .SetBasePath(AppContext.BaseDirectory)
       .AddJsonFile("appsettings.json", optional: false);

   builder.Services.Configure<NetInfoOptions>(builder.Configuration.GetSection("YOUR_SECTION_NAME"));
   ```

   `appsettings.json`:

   ```json
   {
     "YOUR_SECTION_NAME": {
       "ApiKey": "PUT_YOUR_API_KEY_HERE"
     }
   }
   ```

2. You can configure options via code:

   ```csharp
   builder.Services.Configure<NetInfoOptions>(options =>
   {
       options.ApiKey = "PUT_YOUR_API_KEY_HERE";
   });
   ```

3. (Most commonly) You can use environment variables loaded by ENV:

   ```csharp
   builder.Services.AddHyphenEnv();
   builder.Services.AddOptions<NetInfoOptions>().Configure<IEnv>((options, env) =>
   {
       options.ApiKey = env.GetString("API_KEY_ENV_NAME", required: true);
   });
   ```

To retrieves the service, there are at least two options:

1. Retrieve a service instance via the host:

   ```csharp
   var netInfo = host.Services.GetRequiredService<INetInfo>();
   ```

2. (More commonly) Retrieve a service through constructor injection, for any service class you write:

   ```csharp
   public MyClass(INetInfo netInfo)
   {
       // Use the `netInfo` value in any method or property
   }
   ```

The service instance types (and their configuration options object) include:

> Service  | Interface  | Options
> ---------|------------|--------
> ENV      | `IEnv`     | `EnvOptions`
> net-info | `INetInfo` | `NetInfoOptions`

For more information on these patterns, see the Microsoft documentation:

- [Dependency Injection](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection)
- [Options Pattern](https://learn.microsoft.com/dotnet/core/extensions/options)

## ENV (Secret Management Service)

The Hyphen .NET SDK provides an `IEnv` service interface which allows you to load and access environment variables stored in `.env` files on disk. Those files are typically downloaded from ENV using the [Hyphen CLI](https://docs.hyphen.ai/docs/cli). You can read more about ENV:

* [Website](https://hyphen.ai/env)
* [Quick Start Guide](https://docs.hyphen.ai/docs/env-secrets-management)

### Adding ENV

Add the `IEnv` service to your application:

```csharp
builder.Services.AddHyphenEnv();
```

This will automatically load your `.env` files when the application starts up, making the values available via environment variables.

### Configuring ENV

By default, this will load environment variables from the following files, located in the same folder as your application binary:

* `.env`
* `.env.local`

The `EnvOptions` class has the following properties that can be used to configure ENV:

Property      | Description
--------------|------------
`Environment` | Also loads `.env.{Environment}` and `.env.{Environment}.local`, if they exist
`Local`       | Set this to `false` to stop loading `.local` files (defaults to `true`)
`Path`        | Set this to override the path where the `.env` files will be loaded from

Note that the files are loaded in the following order, and values in later files can override values from earlier files:

File                       | Condition
---------------------------|----------
`.env`                     | Always
`.env.local`               | If `Local` is not `false`
`.env.{Environment}`       | If `Environment` is set
`.env.{Environment}.local` | If `Environment` is set and `Local` is not `false`

_Note: Files which are not present are skipped. No files are considered required._

### Using ENV

The environment variables are automatically loaded and placed into your application's environment without any additional requirements beyond registering the service.

The service does provide some helper methods for accessing environment variable values, including verifying that values are appropriate for a specific .NET type:

Method       | Description
-------------|------------
`GetBool`    | Gets a `bool` environment variable<br />Valid values:<br />• For `true`: `"true"`, `"yes"`, `"on"`, `"1"`<br />• For `false`: `"false"`, `"no"`, `"off"`, `"0"`
`GetDecimal` | Gets a `decimal` environment variable
`GetDouble`  | Gets a `double` environment variable
`GetInt`     | Gets a `int` environment variable
`GetLong`    | Gets a `long` environment variable
`GetString`  | Gets a `string` environment variable

All functions have overloads with a `required` parameter, which can change them from returning `null` for missing/invalid values to throwing `ArgumentException` instead. The numeric functions also include overloads that allow passing `NumberStyles` and `IFormatProvider` to influence the parsing of the numbers.

_Note that the `Get` functions on `IEnv` can be used to access any environment variable, not just the ones loaded from your `.env` files._

## net-info (Geo Information Service)

The Hyphen .NET SDK provides an `INetInfo` service interface which allows you to fetch geo information about an IP address. This can be useful for debugging or logging purposes. You can read more about it:

* [Website](https://hyphen.ai/net-info)
* [Quick Start Guide](https://docs.hyphen.ai/docs/netinfo-quickstart)

### Adding net-info

Add the `INetInfo` service to your application:

```csharp
builder.Services.AddNetInfo();
```

### Configuring net-info

The `NetInfoOptions` class has the following properties that can be used to configure net-info:

Property  | Description
----------|------------
`ApiKey`  | Sets the API used to talk to net-info (defaults to the value in environment variable `HYPHEN_API_KEY` if not set)
`BaseUri` | Sets the URI of the net-info service (defaults to `https://net.info`)

### Using net-info

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
