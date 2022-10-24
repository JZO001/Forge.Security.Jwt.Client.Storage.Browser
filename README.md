# Forge.Security.Jwt.Client.Storage.Browser
Forge.Security.Jwt.Client.Storage.Browser is a library extension that provides access to the browsers local and session storage APIs for WASM applications.


## Installing

To install the package add the following line to you csproj file replacing x.x.x with the latest version number:

```
<PackageReference Include="Forge.Security.Jwt.Client.Storage.Browser" Version="x.x.x" />
```

You can also install via the .NET CLI with the following command:

```
dotnet add package Forge.Security.Jwt.Client.Storage.Browser
```

If you're using Visual Studio you can also install via the built in NuGet package manager.

## Setup

You will need to register the local storage services with the service collection in your _Startup.cs_ file in Blazor Server.

```c#
public void ConfigureServices(IServiceCollection services)
{
	// ... preinitialization steps

	// always add this code after the "Forge.Security.Jwt.Client" library initialization
    services.AddForgeJwtClientAuthenticationCoreWithLocalStorage();
	services.AddForgeJwtClientAuthenticationCoreWithSessionStorage();
}
``` 

Or in your _Program.cs_ file in Blazor WebAssembly.

```c#
public static async Task Main(string[] args)
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("app");

    builder.Services.AddForgeJwtClientAuthenticationCoreWithLocalStorage();
    builder.Services.AddForgeJwtClientAuthenticationCoreWithSessionStorage();

    await builder.Build().RunAsync();
}
```

### Registering services as Singleton
If you would like to register Storage Provider services as singletons, it is possible by using the following method:

```csharp
builder.Services.AddForgeJwtClientAuthenticationCoreAsSingletonWithLocalStorage();
builder.Services.AddForgeJwtClientAuthenticationCoreAsSingletonWithSessionStorage();
```

This method is not recommended in the most cases, try to avoid using it.


Please also check the following projects in my repositories:
- Forge.Yoda
- Forge.Security.Jwt.Service
- Forge.Security.Jwt.Service.Storage.SqlServer
- Forge.Security.Jwt.Client
- Forge.Security.Jwt.Client.Storage.Browser
- Forge.Wasm.BrowserStorages
- Forge.Wasm.BrowserStorages.NewtonSoft.Json
