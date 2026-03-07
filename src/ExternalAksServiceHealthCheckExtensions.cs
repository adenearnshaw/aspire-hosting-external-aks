namespace Aspire.Hosting;

/// <summary>
/// Health check extensions for <see cref="ExternalAksServiceResource"/>.
/// </summary>
public static class ExternalAksServiceHealthCheckExtensions
{
    /// <summary>
    /// Adds an HTTP health check to the child external service resource so the parent reflects child health.
    /// </summary>
    /// <param name="builder">The external AKS service resource builder.</param>
    /// <param name="path">The HTTP path to probe.</param>
    /// <returns>The same external AKS service resource builder for fluent chaining.</returns>
    public static IResourceBuilder<ExternalAksServiceResource> WithHttpHealthCheck(
        this IResourceBuilder<ExternalAksServiceResource> builder,
        string path)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        builder.Resource.AksResouce.WithHttpHealthCheck(path);

        return builder;
    }

}
