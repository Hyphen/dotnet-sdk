![Hyphen AI](https://raw.githubusercontent.com/Hyphen/hyphen-design-tokens/refs/heads/main/assets/images/hyphen-symbol-rgb.svg)

[![tests](https://github.com/Hyphen/dotnet-sdk/actions/workflows/tests.yaml/badge.svg)](https://github.com/Hyphen/dotnet-sdk/actions/workflows/tests.yaml)
[![NuGet](https://img.shields.io/nuget/v/Hyphen.Sdk?logo=nuget)](https://www.nuget.org/packages/Hyphen.Sdk)
[![NuGet](https://img.shields.io/nuget/vpre/Hyphen.Sdk?logo=nuget)](https://www.nuget.org/packages/Hyphen.Sdk)
[![license](https://img.shields.io/github/license/Hyphen/dotnet-sdk)](https://github.com/hyphen/dotnet-sdk/blob/main/LICENSE)

# Hyphen .NET SDK

The Hyphen .NET SDK is a .NET library that allows developers to easily integrate Hyphen features into their application, including:

- [Secret Management Service](https://hyphen.ai/env) ("ENV")
- [Short Code Service](https://hyphen.ai/link) ("Link")
- [Geo Information Service](https://hyphen.ai/net-info) ("net-info")
- [Feature Flag Service](https://hyphen.ai/toggle) ("Toggle")

The library is compatible with .NET Standard 2.0 and .NET 8.

# Table of Contents

- [Installation](#installation)
- [Usage](#usage)
  - [Configuring the SDK](#configuring-the-sdk)
  - [ENV (Secret Management Service)](#env-secret-management-service)
    - [Adding ENV](#adding-env)
    - [Configuring ENV](#configuring-env)
    - [Using ENV](#using-env)
  - [Link (Short Code Service)](#link-short-code-service)
    - [Adding Link](#adding-link)
    - [Configuring Link](#configuring-link)
    - [Using Link](#using-link)
  - [net-info (Geo Information Service)](#net-info-geo-information-service)
    - [Adding net-info](#adding-net-info)
    - [Configuring net-info](#configuring-net-info)
    - [Using net-info](#using-net-info)
  - [Toggle (Feature Flag Service)](#toggle-feature-flag-service)
    - [Adding Toggle](#adding-toggle)
    - [Configuring Toggle](#configuring-toggle)
    - [Using Toggle](#using-toggle)
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
> Link     | `ILink`    | `LinkOptions`
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

## Link (Short Code Service)

### Adding Link

The Hyphen .NET SDK provides an `ILink` service interface which allows you to manage short code links and associated QR codes.

Short code links allow you to provide a link from a domain you own to any other URL. Short code links can be more memorable and easier to read aloud than the longer, deeper links that they send the user to. These links are typically used in marketing materials and promotions.

QR codes allow you to create a QR code that can be scanned which will send the user to your short code link. They can embed a logo in their center for customization.

You can read more about Link:

* [Website](https://hyphen.ai/link)
* [Creating short links](https://docs.hyphen.ai/docs/create-short-link)
* [Creating QR codes](https://docs.hyphen.ai/docs/create-a-qr-code)

### Configuring Link

The `LinkOptions` class has the following properties that can be used to configure Link:

Property          | Description
------------------|------------
`ApiKey`          | Sets the API used to talk to Link (defaults to the value in environment variable `HYPHEN_API_KEY` if not set).
`BaseUriTemplate` | Sets the URI template of the Link service (defaults to `https://api.hyphen.ai/api/organizations/{organizationId}/link/codes/`). When constructing the base URI, the service replaces the text `{organizationId}` with your organization ID.
`OrganizationId`  | Sets the organization ID used to perform Link operations (defaults to the value in environment variable `HYPHEN_ORGANIZATION_ID` if not set).

### Using Link

The APIs for link are grouped into two categories: APIs which manage short code links, and APIs which manage QR codes.

#### Short Code Link APIs

The `ILink` interface contains the following APIs related to short code links:

Method              | Description
--------------------|------------
`CreateShortCode`   | Creates a short code
`DeleteShortCode`   | Deletes a short code
`GetShortCode`      | Gets information about a specific short code
`GetShortCodes`     | Gets a list of short codes that match optional search criteria
`GetShortCodeStats` | Gets statistics about short code usage
`GetTags`           | Gets the list of tags used by all short codes in your organization
`UpdateShortCode`   | Updates a short code

Several APIs return one or more `ShortCodeResult` instances, which include the following properties:

Property       | Description
---------------|------------
`Code`         | The "code" portion of the short link (e.g.,`"abc123"` from `https://domain.com/abc123`)
`Domain`       | The "domain" portion of the short link (e.g., `"domain.com"` from `https://domain.com/abc123`)
`Id`           | The ID of the short code (in `code_123456789012345678901234` format)
`LongUrl`      | The URL that the short code sends users to
`Organization` | The organization that owns the short code
`Tags`         | The tags associated with the short code
`Title`        | The title of the short code

The `GetShortCodeStats` API returns a collection of information about usage of your short codes. That includes information about the browsers using the short code, the devices using the short code, the countries where the short code usage took place in, referrers that sent users to the short code, and a daily click count summary for the requested time range.

_Note that the `Get` APIs which may return 404s will return `null` values if the request was for an unknown short code ID or organization ID. The `Create` and `Update` APIs will throw exceptions if the short code ID or organization ID are unknown._

#### QR Code APIs

The `ILink` interface contains the following APIs related to QR codes:

Method         | Description
---------------|------------
`CreateQrCode` | Creates a QR code
`DeleteQrCode` | Deletes a QR code
`GetQrCode`    | Gets information about a specific QR code
`GetQrCodes`   | Gets a list of QR codes that match the optional search criteria

Several APIs return one or more `QrCodeResult` instances, which include the following properties:

Property | Description
---------|------------
`Id`     | The ID of the QR code (in `lqr_123456789012345678901234` format)
`Link`   | The link for the short code that the QR code points to
`QrCode` | The QR code (in `data:image/png;base64,BASE64_ENCODED_IMAGE` format)
`Title`  | The title of the QR code

The `QrCodeResult` also includes two helper methods to decode the `QrCode` property value:

Method           | Description
-----------------|------------
`GetQrCodeBytes` | Get the QR code in `byte[]` format
`SaveQrCode`     | Save the QR to a file on disk

_Note that QR codes are always provided in PNG format, so when saving them to disk, you should use a `.png` file extension._

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

## Toggle (Feature Flag Service)

The Hyphen .NET SDK provides an `IToggle` service interface which allows you to evaluate feature flags in your application. You can read more about it:

* [WebSite](https://hyphen.ai/toggle)
* [Quick Start Guide](https://docs.hyphen.ai/docs/toggle-quickstart)

### Adding Toggle

Add the `IToggle` service to your application:

```csharp
builder.Services.AddToggle();
```

### Configuring Toggle

The `ToggleOptions` class has the following properties that can be used to configure Toggle:

Property              | Description
----------------------|------------
`ApplicationId`       | Sets the application ID for the evaluation (defaults to the value in environment variable `HYPHEN_APP_ID` if not set)
`BaseUris`            | Sets the base URIs for evaluation requests (defaults to `https://{OrganizationId}.toggle.hyphen.cloud` and `https://toggle.hyphen.cloud` if not set)
`CacheKeyFactory`     | Sets the cache key algorithm (defaults to cache keys that include the toggle key + either the user ID or the user email address)
`CachePolicyFactory`  | Sets the cache policy algorithm (defaults to caching evaluations for 30 seconds)
`DefaultContext`      | Sets the default context for the evaluation request (see the `ToggleContext` description below)
`DefaultTargetingKey` | Sets the default targeting key (defaults to `{ApplicationId}-{Environment}-{RandomValue]` if not set)
`Environment`         | Sets the runtime environment (defaults to `development` if not set)
`ProjectPublicKey`    | Sets the project public key for the evaluation requests (defaults to the value in environment variable `HYPHEN_PROJECT_PUBLIC_KEY` if not set)

The `ToggleContext` object contains the context used by the evaluation engine. It contains the following properties:

Property           | Description
-------------------|------------
`CustomAttributes` | Set this to send custom attributes
`IPAddress`        | Set this to send an IP address
`TargetingKey`     | Set this to send a specific targeting key
`User`             | Set this to describe the current application user

### Using Toggle

The `IToggle` interface contains a single method: `Evaluate`. Every evaluation requires a toggle key to request, and optionally allows you to specify the default value to be used if the evaluation fails. You can also configure the request using `EvaluateParams`, which contains the following properties:

Property  | Description
----------|------------
`Cache`   | Set this to enable or disable caching of the evaluation
`Context` | Set this to override the `ToggleContext` used to make the evaluation request

The `Evaluate` methods are generic, and the generic type will be expected type of the evaluation value (e.g., if you are expecting a boolean result from the evaluation, you would call `Evaluate<bool>`).

The fundamental types supported by Toggle map onto the following .NET types:

Toggle Type | .NET Type(s)
------------|-------------
`boolean`   | `bool`
`object`    | Any custom object type that can be deserialized from the JSON returned by the evaluation (*)
`number`    | Any numeric type (e.g., `int`, `long`, `float`, `double`, `decimal`, etc.)
`string`    | `string`

_(*) If the object type does not map to a single object shape, you can request it with `Evaluate<string>` and then perform any post-processing on the result that is appropriate for your application._

The return type from `Evaluate<T>` is `ToggleEvaluation<T?>`, which has the following properties:

Property    | Description
------------|------------
`Exception` | Contains the exception that occurred while processing the evaluation (if any)
`Key`       | Contains the key of the toggle request
`Reason`    | Contains the reason for the returned value
`Value`     | Contains the value of the evaluation

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
