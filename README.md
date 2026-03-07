# aspire-hosting-externalaks

A sample + library repository for integrating external AKS services into a .NET Aspire AppHost.

This repository contains a reusable Aspire hosting package and a sample AppHost showing how to consume it via a local NuGet package.

## Repository Structure

- `src/A10w.Aspire.Hosting.ExternalAks`: Reusable library package
- `sample/A10w.Aspire.Hosting.ExternalAks.AppHost`: Sample Aspire AppHost
- `sample/A10w.Aspire.Hosting.ExternalAks.Console`: Sample console app that consumes the external service
- `sample/A10w.Aspire.Hosting.ExternalAks.ServiceDefaults`: Shared service defaults for the sample
- `sample/test-local-package.ps1`: Helper script to repack and force-refresh local package consumption

## Library Overview

The library adds `AddExternalAksService(...)` to `IDistributedApplicationBuilder`.

It creates:

- an executable resource that runs `kubectl port-forward`
- an external service resource at `http://localhost:{LocalPort}`
- a custom parent resource that groups and represents the AKS integration in Aspire

## Quick Start (Sample)

1. Build and refresh local package consumption:

```powershell
pwsh sample/test-local-package.ps1
```

2. Run the sample AppHost:

```bash
dotnet run --project sample/A10w.Aspire.Hosting.ExternalAks.AppHost
```

## Configuration Example

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

## Prerequisites

- .NET SDK 10
- PowerShell (`pwsh`)
- `kubectl` installed and configured
- Access to the target AKS cluster/network

## Packaging Notes

`src/A10w.Aspire.Hosting.ExternalAks` is configured as a NuGet package and includes:

- `lib/net10.0/A10w.Aspire.Hosting.ExternalAks.dll`
- `contentFiles/any/any/Scripts/setup-port-forward.ps1`
- package readme: `PACKAGE-README.md`

## Development Workflow

When you make library changes and want to test from the sample:

1. Run `pwsh sample/test-local-package.ps1`
2. Run `dotnet run --project sample/A10w.Aspire.Hosting.ExternalAks.AppHost`
3. Repeat step 1 after each library change
