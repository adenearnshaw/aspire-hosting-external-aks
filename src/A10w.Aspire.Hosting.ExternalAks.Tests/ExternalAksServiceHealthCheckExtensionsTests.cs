using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;

namespace A10w.Aspire.Hosting.ExternalAks.Tests;

public class ExternalAksServiceHealthCheckExtensionsTests
{
    // Note: Health check extension tests require full resource creation which needs
    // the PowerShell scripts directory. These tests validate the extension method contracts.

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void WithHttpHealthCheck_WithInvalidPath_ThrowsArgumentException(string? path)
    {
        // Arrange - Create a mock resource builder using an in-memory resource
        var appBuilder = DistributedApplication.CreateBuilder();
        var mockExternalService = appBuilder.AddExternalService("mock", "http://localhost:9000");
        var mockResource = new ExternalAksServiceResource("test")
        {
            AksResouce = mockExternalService,
            LocalPort = 9000
        };
        var mockBuilder = appBuilder.AddResource(mockResource);

        // Act
        var act = () => mockBuilder.WithHttpHealthCheck(path!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
