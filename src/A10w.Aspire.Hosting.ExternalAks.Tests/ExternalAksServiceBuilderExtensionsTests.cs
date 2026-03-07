using Aspire.Hosting;
using Aspire.Hosting.Testing;

namespace A10w.Aspire.Hosting.ExternalAks.Tests;

public class ExternalAksServiceBuilderExtensionsTests
{
    // Note: Full integration tests that actually create resources are skipped because
    // they require the PowerShell scripts directory structure from the AppHost project.
    // These tests focus on validation and error handling instead.

    [Fact]
    public void AddExternalAksService_WithNullBuilder_ThrowsArgumentNullException()
    {
        // Arrange
        IDistributedApplicationBuilder builder = null!;

        // Act
        var act = () => builder.AddExternalAksService("test", options =>
        {
            options.KubernetesContext = "test-context";
            options.KubernetesNamespace = "test-namespace";
            options.KubernetesServiceName = "svc-test";
            options.LocalPort = 8080;
        });

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("builder");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddExternalAksService_WithInvalidName_ThrowsArgumentException(string? name)
    {
        // Arrange
        var appBuilder = DistributedApplication.CreateBuilder();

        // Act
        var act = () => appBuilder.AddExternalAksService(name!, options =>
        {
            options.KubernetesContext = "test-context";
            options.KubernetesNamespace = "test-namespace";
            options.KubernetesServiceName = "svc-test";
            options.LocalPort = 8080;
        });

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddExternalAksService_WithNullConfigureAction_ThrowsArgumentNullException()
    {
        // Arrange
        var appBuilder = DistributedApplication.CreateBuilder();

        // Act
        var act = () => appBuilder.AddExternalAksService("test", null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("configure");
    }

    [Fact]
    public void AddExternalAksService_WithInvalidOptions_ThrowsValidationException()
    {
        // Arrange
        var appBuilder = DistributedApplication.CreateBuilder();

        // Act
        var act = () => appBuilder.AddExternalAksService("test", options =>
        {
            options.KubernetesContext = ""; // Invalid
            options.KubernetesNamespace = "test-namespace";
            options.KubernetesServiceName = "svc-test";
            options.LocalPort = 8080;
        });

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddExternalAksService_WithInvalidPortRange_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var appBuilder = DistributedApplication.CreateBuilder();

        // Act
        var act = () => appBuilder.AddExternalAksService("test", options =>
        {
            options.KubernetesContext = "test-context";
            options.KubernetesNamespace = "test-namespace";
            options.KubernetesServiceName = "svc-test";
            options.LocalPort = 99999; // Invalid port
        });

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void AddExternalAksService_WithMissingNamespace_ThrowsArgumentException()
    {
        // Arrange
        var appBuilder = DistributedApplication.CreateBuilder();

        // Act
        var act = () => appBuilder.AddExternalAksService("test", options =>
        {
            options.KubernetesContext = "test-context";
            options.KubernetesNamespace = ""; // Invalid
            options.KubernetesServiceName = "svc-test";
            options.LocalPort = 8080;
        });

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddExternalAksService_WithMissingServiceName_ThrowsArgumentException()
    {
        // Arrange
        var appBuilder = DistributedApplication.CreateBuilder();

        // Act
        var act = () => appBuilder.AddExternalAksService("test", options =>
        {
            options.KubernetesContext = "test-context";
            options.KubernetesNamespace = "test-namespace";
            options.KubernetesServiceName = ""; // Invalid
            options.LocalPort = 8080;
        });

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
