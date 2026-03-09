using System.Globalization;
using System.Reflection;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.DependencyInjection;

namespace Aspire.Hosting;

public static class ExternalAksServiceBuilderExtensions
{
    /// <summary>
    /// Adds an external service backed by a kubectl port-forward process.
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="name">The logical service name in the Aspire model.</param>
    /// <param name="configure">Configures the Kubernetes port-forward settings.</param>
    /// <returns>The external service resource builder.</returns>
    /// <example>
    /// <code>
    /// var documentsService = builder.AddExternalAksService("svc-documents", options =>
    /// {
    ///     options.KubernetesContext = "tst-services-cluster-uks-aks";
    ///     options.KubernetesNamespace = "dev-documentsservice";
    ///     options.KubernetesServiceName = "svc-documents";
    ///     options.LocalPort = 8999;
    /// });
    /// </code>
    /// </example>
    public static IResourceBuilder<ExternalAksServiceResource> AddExternalAksService(
        this IDistributedApplicationBuilder builder,
        string name,
        Action<ExternalAksServiceOptions> configure)
    {
        // Build the port-forward settings once to keep resource creation consistent.
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(configure);

        // Seed required properties with placeholders before user configuration runs.
        var options = new ExternalAksServiceOptions
        {
            KubernetesContext = string.Empty,
            KubernetesNamespace = string.Empty,
            KubernetesServiceName = string.Empty,
            LocalPort = 0
        };

        configure(options);
        options.Validate();

        // Extract embedded PowerShell script to a temporary location.
        const string scriptFileName = "setup-port-forward.ps1";
        const string embeddedResourceName = "A10w.Aspire.Hosting.ExternalAks.Scripts.setup-port-forward.ps1";
        
        var assembly = Assembly.GetExecutingAssembly();
        var tempDirectory = Path.Combine(Path.GetTempPath(), "A10w.Aspire.Hosting.ExternalAks", Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);
        
        var scriptPath = Path.Combine(tempDirectory, scriptFileName);
        
        using (var stream = assembly.GetManifestResourceStream(embeddedResourceName))
        {
            if (stream == null)
            {
                throw new InvalidOperationException($"Embedded resource '{embeddedResourceName}' not found. Available resources: {string.Join(", ", assembly.GetManifestResourceNames())}");
            }
            
            using (var fileStream = File.Create(scriptPath))
            {
                stream.CopyTo(fileStream);
            }
        }

        // Register a TCP health check for this port-forward instance. The key is scoped to the
        // resource name so multiple AddExternalAksService calls each get their own check.
        var healthCheckKey = $"port-forward-{name}-{options.LocalPort}";
        builder.Services.AddHealthChecks()
            .AddCheck(healthCheckKey, new PortForwardHealthCheck(options.LocalPort));

        // Setup the port-forward executable resource.
        // This will run a kubectl port-forward command to expose the AKS service locally.
        var portForwardExecutable = builder.AddExecutable(
                $"{name}-port-forward",
                "pwsh",
                tempDirectory,
                scriptFileName,
                "-KubeContext",
                options.KubernetesContext,
                "-Namespace",
                options.KubernetesNamespace,
                "-ServiceName",
                options.KubernetesServiceName,
                "-LocalPort",
                options.LocalPort.ToString(CultureInfo.InvariantCulture),
                "-RemotePort",
                options.RemotePort.ToString(CultureInfo.InvariantCulture))
			.WithIconName("arrowForward", IconVariant.Regular)
            // Associate the TCP health check so the executable shows Unhealthy until
            // the tunnel is bound, even though the pwsh process itself is Running.
            .WithHealthCheck(healthCheckKey);

        // Create the external service resource that represents the AKS service in the Aspire model.
        var service = builder.AddExternalService($"{name}-ext", $"http://localhost:{options.LocalPort}/");

        // Define the custom resource that ties everything together. 
        // This resource will be responsible for managing the lifecycle of the port-forward process and representing the AKS service in Aspire.
        var resourceName = name;

        var aksResource = new ExternalAksServiceResource(resourceName)
        {
			AksResouce = service,
            PortForwardExecutable = portForwardExecutable,
            LocalPort = options.LocalPort
        };

        var resourceBuilder = builder.AddResource(aksResource)
            .WithInitialState(new CustomResourceSnapshot
            {
                ResourceType = "ExternalAksService",
                State = KnownResourceStates.NotStarted,
                Properties = []
            })
            .WithEndpoint(
                name: "http",
                scheme: "http",
                port: options.LocalPort,
                isExternal: false,
                isProxied: false);

            // Ensure the external service is not considered ready until the port-forward process is ready.
            resourceBuilder.WaitForPortForward();

        // Custom resources do not have a built-in lifecycle. We subscribe to the initialize event
        // and start a background task that drives the state machine by watching the port-forward
        // executable's log output and notification stream.
        var portForwardResourceName = portForwardExecutable.Resource.Name;
        resourceBuilder.OnInitializeResource(async (resource, initializeEvent, cancellationToken) =>
        {
            await initializeEvent.Notifications.PublishUpdateAsync(aksResource, snapshot => snapshot with
            {
                State = KnownResourceStates.Starting
            });

            var resourceLoggerService = initializeEvent.Services.GetRequiredService<ResourceLoggerService>();

            // Log watcher: detect Azure Device Login prompt and surface it as a warning state
            // so the user knows to authenticate via the link in the resource's console output.
            _ = Task.Run(async () =>
            {
                await foreach (var logBatch in resourceLoggerService.WatchAsync(portForwardExecutable.Resource).WithCancellation(cancellationToken))
                {
                    foreach (var logLine in logBatch)
                    {
                        if (logLine.Content.Contains("To sign in", StringComparison.OrdinalIgnoreCase))
                        {
                            await initializeEvent.Notifications.PublishUpdateAsync(aksResource, snapshot => snapshot with
                            {
                                State = new ResourceStateSnapshot("Login Required", KnownResourceStateStyles.Warn)
                            });
                        }
                    }
                }
            }, cancellationToken);

            // Notification watcher: wait for the port-forward executable's TCP health check to pass,
            // which means the tunnel is established. Only then do we publish Running on the parent,
            // unblocking any .WaitFor() callers on the ExternalAksServiceResource.
            _ = Task.Run(async () =>
            {
                await initializeEvent.Notifications.WaitForResourceHealthyAsync(
                    portForwardResourceName,
                    cancellationToken);

                await initializeEvent.Notifications.PublishUpdateAsync(aksResource, snapshot => snapshot with
                {
                    State = KnownResourceStates.Running
                });
            }, cancellationToken);
        });

        service.WithParentRelationship(resourceBuilder);
        portForwardExecutable.WithParentRelationship(resourceBuilder);

        return resourceBuilder;
    }

}
