namespace Aspire.Hosting;

public sealed record ExternalAksServiceOptions(
    string KubeContext,
    string KubernetesNamespace,
    string ServiceName,
    int LocalPort,
    int RemotePort);
