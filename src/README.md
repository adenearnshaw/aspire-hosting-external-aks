# A10w.Aspire.Hosting.ExternalAks

`A10w.Aspire.Hosting.ExternalAks` is a .NET Aspire hosting extension that lets your AppHost represent an external service running in AKS by starting a local `kubectl port-forward` process.

## What It Provides

- `AddExternalAksService(...)` extension for Aspire AppHost
- A custom Aspire resource for the external AKS-backed service
- Local endpoint and connection string support for service discovery
- Included PowerShell helper script used to run `kubectl port-forward`

## Installation

```bash
dotnet add package A10w.Aspire.Hosting.ExternalAks
```

## Basic Usage

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var documentsService = builder.AddExternalAksService("svc-documents", options =>
{
    options.KubernetesContext = "tst-services-cluster-uks-aks";
    options.KubernetesNamespace = "dev-documentsservice";
    options.KubernetesServiceName = "svc-documents";
    options.LocalPort = 8999;
    // options.RemotePort = 80; // optional, defaults to 80
});

builder.Build().Run();
```

## Configuration

Required options:

- `KubernetesContext`
- `KubernetesNamespace`
- `KubernetesServiceName`
- `LocalPort`

Optional options:

- `RemotePort` (default: `80`)

## Prerequisites

- PowerShell (`pwsh`) available on your machine
- `kubectl` installed and on `PATH`
- Access to the target AKS cluster and namespace

## Health Checks

You can add health checks in AppHost as usual. In this package, `WithHttpHealthCheck(...)` on the external AKS resource forwards to the child external service resource.

## Notes

- The package includes `Scripts/setup-port-forward.ps1` under `contentFiles/any/any/Scripts`.
- The extension validates option values before creating resources.
- On startup, the custom resource transitions to `Running` through Aspire lifecycle notifications.
