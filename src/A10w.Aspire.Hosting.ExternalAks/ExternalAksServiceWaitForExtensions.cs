namespace Aspire.Hosting;

/// <summary>
/// Wait-for extensions for <see cref="ExternalAksServiceResource"/>.
/// </summary>
public static class ExternalAksServiceWaitForExtensions
{
    /// <summary>
    /// Configures the child external service to wait for the port-forward executable resource to start.
    /// </summary>
    /// <param name="builder">The external AKS service resource builder.</param>
    /// <returns>The same external AKS service resource builder for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the port-forward executable has not been configured.</exception>
    /// <remarks>
    /// This extension adds a <see cref="WaitAnnotation"/> to the external service resource, ensuring it waits
    /// for the port-forward executable to enter the Running state before attempting connections.
    /// </remarks>
    public static IResourceBuilder<ExternalAksServiceResource> WaitForPortForward(
        this IResourceBuilder<ExternalAksServiceResource> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var portForwardExecutable = builder.Resource.PortForwardExecutable;
        if (portForwardExecutable is null)
        {
            throw new InvalidOperationException("Port-forward executable is not configured for this external AKS service resource.");
        }

        // Manually annotate the external service resource with a WaitAnnotation since
        // ExternalServiceResource does not implement IResourceWithWaitSupport.
        builder.Resource.AksResouce.Resource.Annotations.Add(
            new WaitAnnotation(
                portForwardExecutable.Resource,
                WaitType.WaitUntilStarted));

        return builder;
    }
}
