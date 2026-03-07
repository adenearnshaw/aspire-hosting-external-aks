# A10w.Aspire.Hosting.ExternalAks

Aspire Hosting Integration to setup an External Service Resource inside an AKS cluster with port forwarding.

[![Latest Release](https://img.shields.io/github/v/release/adenearnshaw/aspire-hosting-external-aks?style=flat-square&color=8957e5&logo=github)](https://github.com/adenearnshaw/aspire-hosting-external-aks/releases/latest) [![NuGet version](https://img.shields.io/nuget/v/A10w.Aspire.Hosting.ExternalAks?style=flat-square&logo=nuget)](https://www.nuget.org/packages/A10w.Aspire.Hosting.ExternalAks)
[![NuGet prerelease](https://img.shields.io/nuget/vpre/A10w.Aspire.Hosting.ExternalAks?style=flat-square&logo=nuget&label=prerelease)](https://www.nuget.org/packages/A10w.Aspire.Hosting.ExternalAks)
[![NuGet downloads](https://img.shields.io/nuget/dt/A10w.Aspire.Hosting.ExternalAks?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/A10w.Aspire.Hosting.ExternalAks) [![License](https://img.shields.io/github/license/adenearnshaw/aspire-hosting-external-aks?style=flat-square&color=dfb317)](https://github.com/adenearnshaw/aspire-hosting-external-aks/tree/main/LICENSE) 

This repository contains:

- a reusable Aspire hosting library package (`A10w.Aspire.Hosting.ExternalAks`)
- a sample AppHost that consumes the package locally for validation

## Library First

The library adds `AddExternalAksService(...)` to `IDistributedApplicationBuilder`.

It creates and wires:

- an executable resource that runs `kubectl port-forward`
- an external service resource at `http://localhost:{LocalPort}`
- a custom parent resource that represents the AKS integration in Aspire

## Prerequisites

- .NET SDK 10
- PowerShell (`pwsh`)
- `kubectl` installed and configured
- Access to the target AKS cluster/network

## Install The Package in your AppHost

```bash
aspire add A10w.Aspire.Hosting.ExternalAks
```

## Library Usage

```csharp
var documentsService = builder.AddExternalAksService("svc-documents", options =>
{
    options.KubernetesContext = "tst-services-cluster-uks-aks";
    options.KubernetesNamespace = "dev-documentsservice";
    options.KubernetesServiceName = "svc-documents";
    options.LocalPort = 8999;
    // options.RemotePort = 80; // optional
});
```

Required properties:

- `KubernetesContext`
- `KubernetesNamespace`
- `KubernetesServiceName`
- `LocalPort`

Optional property:

- `RemotePort` (default: `80`)


## Package Contents

`src/A10w.Aspire.Hosting.ExternalAks` is configured as a NuGet package and includes:

- `lib/net10.0/A10w.Aspire.Hosting.ExternalAks.dll`
- `contentFiles/any/any/Scripts/setup-port-forward.ps1`
- package readme: `PACKAGE-README.md`

## Sample App

The sample project demonstrates local package-based consumption and validation.

### Sample Structure

- `src/A10w.Aspire.Hosting.ExternalAks`: Reusable library package
- `sample/A10w.Aspire.Hosting.ExternalAks.AppHost`: Sample Aspire AppHost
- `sample/A10w.Aspire.Hosting.ExternalAks.Console`: Sample console app that consumes the external service
- `sample/A10w.Aspire.Hosting.ExternalAks.ServiceDefaults`: Shared service defaults for the sample
- `sample/test-local-package.ps1`: Helper script to repack and force-refresh local package consumption

### Run The Sample

1. Build and refresh local package consumption:

```powershell
pwsh sample/test-local-package.ps1
```

2. Run the sample AppHost:

```bash
dotnet run --project sample/A10w.Aspire.Hosting.ExternalAks.AppHost
```

### Development Workflow

When you make library changes and want to test from the sample:

1. Run `pwsh sample/test-local-package.ps1`
2. Run `dotnet run --project sample/A10w.Aspire.Hosting.ExternalAks.AppHost`
3. Repeat step 1 after each library change
