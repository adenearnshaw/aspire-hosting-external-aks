using System.Globalization;
using System.IO;

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

        // Resolve script location relative to the AppHost project so execution works on case-sensitive file systems.
        var scriptsDirectory = Path.GetFullPath(Path.Combine(
            builder.AppHostDirectory,
            "..",
            "..",
            "src",
            "A10w.Aspire.Hosting.ExternalAks",
            "Scripts"));

        const string scriptFileName = "setup-port-forward.ps1";
        var scriptPath = Path.Combine(scriptsDirectory, scriptFileName);

        if (!Directory.Exists(scriptsDirectory))
        {
            throw new DirectoryNotFoundException($"Port-forward scripts directory was not found: '{scriptsDirectory}'.");
        }

        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException($"Port-forward script was not found at '{scriptPath}'.", scriptPath);
        }

        // Setup the port-forward executable resource.
        // This will run a kubectl port-forward command to expose the AKS service locally.
        var portForwardExecutable = builder.AddExecutable(
            $"{name}-port-forward",
            "pwsh",
            scriptsDirectory,
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
            options.RemotePort.ToString(CultureInfo.InvariantCulture));

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
                State = KnownResourceStates.Waiting,
                Properties = []
            })
            .WithEndpoint(
                name: "http",
                scheme: "http",
                port: options.LocalPort,
                isExternal: false,
                isProxied: false);

        service.WithParentRelationship(resourceBuilder);
        portForwardExecutable.WithParentRelationship(resourceBuilder);

        return resourceBuilder;
    }

}
