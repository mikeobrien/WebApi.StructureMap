Web API StructureMap Integration
=============

[![Nuget](http://img.shields.io/nuget/v/WebApi.StructureMap.svg?style=flat)](http://www.nuget.org/packages/WebApi.StructureMap/) [![Nuget downloads](http://img.shields.io/nuget/dt/WebApi.StructureMap.svg?style=flat)](http://www.nuget.org/packages/WebApi.StructureMap/) [![TeamCity Build Status](https://img.shields.io/teamcity/http/build.mikeobrien.net/s/webapistructuremap.svg?style=flat)](http://build.mikeobrien.net/viewType.html?buildTypeId=webapistructuremap&guest=1)

This library integrates [StructureMap](http://structuremap.github.io/) with the [Web API](http://www.asp.net/web-api). 

Installation
------------

    PM> Install-Package WebApi.StructureMap  

Usage
------------

To register StructureMap, simply call the `UseStructureMap<T>()` extension method on startup, specifying your registry:

```csharp
public class Global : System.Web.HttpApplication
{
    protected void Application_Start(object sender, EventArgs e)
    {
        GlobalConfiguration.Configuration.UseStructureMap<Registry>();
        ...
    }
}
```

You can also configure StructureMap with the configuration DSL:

```csharp
GlobalConfiguration.Configuration.UseStructureMap(x =>{    x.AddRegistry<Registry1>();    x.AddRegistry<Registry2>();});
```

The following objects are automatically injected into request scoped nested containers:

- `HttpRequestMessage`
- `HttpControllerDescriptor`
- `HttpRequestContext`
- `IHttpRouteData`

There are two convenience methods provided for `HttpActionContext` and `HttpActionExecutedContext` that simplify service location in action filters. 

```csharp
public class SomeFilter : ActionFilterAttribute{    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)    {        actionExecutedContext.GetService<SomeService>().DoSomething();    }}public class SomeService{    public SomeService(        HttpResponseMessage response,        IDependency dependency) { ... }    public void DoSomething() { ... }}
```

These convenience methods will create an instance from the request scoped nested container. They will also pass the values of properties on `HttpActionContext` and `HttpActionExecutedContext` so your services can depend on them instead of `HttpActionContext` and `HttpActionExecutedContext`.

| Source | Injected |
| ----- | ----- |
| `HttpActionExecutedContext` | `HttpActionExecutedContext ` |
|  | `HttpActionContext` |
|  | `HttpResponseMessage` |
| `HttpActionContext` | `HttpActionContext ` |
|  | `HttpActionDescriptor` |
|  | `HttpControllerContext` |
|  | `ModelStateDictionary` |

License
------------

MIT License