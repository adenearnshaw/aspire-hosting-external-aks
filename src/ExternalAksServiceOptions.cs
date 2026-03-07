namespace Aspire.Hosting;

/// <summary>
/// Configuration for an external AKS service exposed locally through <c>kubectl port-forward</c>.
/// </summary>
public sealed class ExternalAksServiceOptions
{
    /// <summary>
    /// Gets or sets the <c>kubectl</c> context used for the port-forward command.
    /// </summary>
    public required string KubernetesContext { get; set; }

    /// <summary>
    /// Gets or sets the Kubernetes namespace containing the target service.
    /// </summary>
    public required string KubernetesNamespace { get; set; }

    /// <summary>
    /// Gets or sets the Kubernetes service name to forward to.
    /// </summary>
    public required string KubernetesServiceName { get; set; }

    /// <summary>
    /// Gets or sets the local port that <c>kubectl port-forward</c> binds to.
    /// </summary>
    public required int LocalPort { get; set; }

    /// <summary>
    /// Gets or sets the remote service port in Kubernetes.
    /// </summary>
    public int RemotePort { get; set; } = 80;

    /// <summary>
    /// Validates the configured values before starting resource creation.
    /// </summary>
    internal void Validate()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(KubernetesContext);
        ArgumentException.ThrowIfNullOrWhiteSpace(KubernetesNamespace);
        ArgumentException.ThrowIfNullOrWhiteSpace(KubernetesServiceName);

        if (LocalPort is < 1 or > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(LocalPort), LocalPort, "LocalPort must be between 1 and 65535.");
        }

        if (RemotePort is < 1 or > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(RemotePort), RemotePort, "RemotePort must be between 1 and 65535.");
        }
    }
}
