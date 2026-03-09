using System.Net.Sockets;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aspire.Hosting;

/// <summary>
/// Health check that verifies a kubectl port-forward tunnel is actively accepting TCP connections
/// on the configured local port. Reports <see cref="HealthStatus.Unhealthy"/> while the tunnel
/// is not yet established (e.g. waiting for Azure device login), and <see cref="HealthStatus.Healthy"/>
/// once the port is bound and forwarding traffic.
/// </summary>
internal sealed class PortForwardHealthCheck(int localPort) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        using var tcpClient = new TcpClient();

        try
        {
            await tcpClient.ConnectAsync("localhost", localPort, cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Port-forward on localhost:{localPort} is not yet accepting connections.", ex);
        }
    }
}
