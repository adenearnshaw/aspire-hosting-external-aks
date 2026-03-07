namespace Aspire.Hosting;

public sealed class ExternalAksServiceOptionsBuilder
{
    private string kubeContext = "tst-services-cluster-uks-aks";
    private string kubernetesNamespace = "dev-documentsservice";
    private string serviceName = "svc-documents";
    private int localPort = 8999;
    private int remotePort = 80;

    /// <summary>
    /// Sets the Kubernetes context used for the port-forward.
    /// </summary>
    /// <param name="value">The kubectl context name.</param>
    /// <returns>The builder for chaining.</returns>
    public ExternalAksServiceOptionsBuilder WithKubeContext(string value)
    {
        // Keep context configuration explicit to avoid surprises during local runs.
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        kubeContext = value;

        return this;
    }

    /// <summary>
    /// Sets the Kubernetes namespace used for the port-forward.
    /// </summary>
    /// <param name="value">The Kubernetes namespace.</param>
    /// <returns>The builder for chaining.</returns>
    public ExternalAksServiceOptionsBuilder WithNamespace(string value)
    {
        // Namespace must be explicit so we do not forward from the wrong environment.
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        kubernetesNamespace = value;

        return this;
    }

    /// <summary>
    /// Sets the Kubernetes service name used for the port-forward.
    /// </summary>
    /// <param name="value">The service name without or with the svc/ prefix.</param>
    /// <returns>The builder for chaining.</returns>
    public ExternalAksServiceOptionsBuilder WithServiceName(string value)
    {
        // Service name is required to build a stable port-forward target.
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        serviceName = value;

        return this;
    }

    /// <summary>
    /// Sets the local port that the port-forward will bind to.
    /// </summary>
    /// <param name="value">The local port number.</param>
    /// <returns>The builder for chaining.</returns>
    public ExternalAksServiceOptionsBuilder WithLocalPort(int value)
    {
        // Validate port range early to avoid confusing kubectl errors.
        localPort = value;

        return this;
    }

    /// <summary>
    /// Sets the remote port exposed by the Kubernetes service.
    /// </summary>
    /// <param name="value">The remote port number.</param>
    /// <returns>The builder for chaining.</returns>
    public ExternalAksServiceOptionsBuilder WithRemotePort(int value)
    {
        // Allow override when the service does not expose port 80.
        remotePort = value;

        return this;
    }

    /// <summary>
    /// Builds the validated options used to create the port-forward resources.
    /// </summary>
    /// <returns>The configured options.</returns>
    public ExternalAksServiceOptions Build()
    {
        // Validate once so callers get a clear failure before Aspire starts.
        if (localPort is < 1 or > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(localPort), localPort, "Local port must be between 1 and 65535.");
        }

        if (remotePort is < 1 or > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(remotePort), remotePort, "Remote port must be between 1 and 65535.");
        }

        return new ExternalAksServiceOptions(kubeContext, kubernetesNamespace, serviceName, localPort, remotePort);
    }
}
