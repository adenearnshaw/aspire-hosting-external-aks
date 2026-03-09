namespace Aspire.Hosting;

/// <summary>
/// Represents an external AKS service resource with service discovery capabilities.
/// </summary>
/// <remarks>
/// Implements <see cref="IResourceWithWaitSupport"/> so that other resources can use .WaitFor() to block
/// until the port-forward tunnel is running and the external service is reachable.
/// </remarks>
public sealed class ExternalAksServiceResource(string name): Resource(name), IResourceWithEndpoints, IResourceWithConnectionString, IResourceWithWaitSupport
{
    public required IResourceBuilder<ExternalServiceResource> AksResouce { get; init;}
	public IResourceBuilder<ExecutableResource>? PortForwardExecutable { get; init; }

	/// <summary>
	/// Gets or sets the local port that the service is forwarded to.
	/// </summary>
	public required int LocalPort { get; init; }

	/// <summary>
	/// Gets the connection string expression for the port-forwarded local endpoint.
	/// </summary>
	public ReferenceExpression ConnectionStringExpression =>
		ReferenceExpression.Create($"http://localhost:{LocalPort.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
}
