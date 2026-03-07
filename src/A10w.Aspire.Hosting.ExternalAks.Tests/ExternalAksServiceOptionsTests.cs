using Aspire.Hosting;

namespace A10w.Aspire.Hosting.ExternalAks.Tests;

public class ExternalAksServiceOptionsTests
{
    [Fact]
    public void Options_CanBeCreatedWithRequiredProperties()
    {
        // Arrange & Act
        var options = new ExternalAksServiceOptions
        {
            KubernetesContext = "test-context",
            KubernetesNamespace = "test-namespace",
            KubernetesServiceName = "test-service",
            LocalPort = 8080
        };

        // Assert
        options.KubernetesContext.Should().Be("test-context");
        options.KubernetesNamespace.Should().Be("test-namespace");
        options.KubernetesServiceName.Should().Be("test-service");
        options.LocalPort.Should().Be(8080);
    }

    [Fact]
    public void RemotePort_DefaultsTo80()
    {
        // Arrange & Act
        var options = new ExternalAksServiceOptions
        {
            KubernetesContext = "test-context",
            KubernetesNamespace = "test-namespace",
            KubernetesServiceName = "test-service",
            LocalPort = 8080
        };

        // Assert
        options.RemotePort.Should().Be(80);
    }

    [Fact]
    public void RemotePort_CanBeSetToCustomValue()
    {
        // Arrange & Act
        var options = new ExternalAksServiceOptions
        {
            KubernetesContext = "test-context",
            KubernetesNamespace = "test-namespace",
            KubernetesServiceName = "test-service",
            LocalPort = 8080,
            RemotePort = 8443
        };

        // Assert
        options.RemotePort.Should().Be(8443);
    }
}
