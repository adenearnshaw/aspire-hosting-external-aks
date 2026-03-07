using Aspire.Hosting;
using Aspire.Hosting.Testing;

namespace A10w.Aspire.Hosting.ExternalAks.Tests;

public class ExternalAksServiceResourceTests
{
    [Fact]
    public void Constructor_CreatesResourceWithName()
    {
        // Arrange
        var appBuilder = DistributedApplication.CreateBuilder();
        var mockService = appBuilder.AddExternalService("mock-external", "http://localhost:8080");

        // Act
        var resource = new ExternalAksServiceResource("test-resource")
        {
            AksResouce = mockService,
            LocalPort = 8080
        };

        // Assert
        resource.Name.Should().Be("test-resource");
    }

    [Fact]
    public void LocalPort_ReturnsConfiguredValue()
    {
        // Arrange
        var appBuilder = DistributedApplication.CreateBuilder();
        var mockService = appBuilder.AddExternalService("mock-external", "http://localhost:8080");

        var resource = new ExternalAksServiceResource("test-resource")
        {
            AksResouce = mockService,
            LocalPort = 9000
        };

        // Act & Assert
        resource.LocalPort.Should().Be(9000);
    }

    [Fact]
    public void ConnectionStringExpression_IncludesLocalPort()
    {
        // Arrange
        var appBuilder = DistributedApplication.CreateBuilder();
        var mockService = appBuilder.AddExternalService("mock-external", "http://localhost:8080");

        var resource = new ExternalAksServiceResource("test-resource")
        {
            AksResouce = mockService,
            LocalPort = 7777
        };

        // Act
        var connectionString = resource.ConnectionStringExpression.ValueExpression;

        // Assert
        connectionString.Should().Contain("7777");
        connectionString.Should().Contain("localhost");
    }

    [Fact]
    public void ConnectionStringExpression_UsesHttpScheme()
    {
        // Arrange
        var appBuilder = DistributedApplication.CreateBuilder();
        var mockService = appBuilder.AddExternalService("mock-external", "http://localhost:8080");

        var resource = new ExternalAksServiceResource("test-resource")
        {
            AksResouce = mockService,
            LocalPort = 8080
        };

        // Act
        var connectionString = resource.ConnectionStringExpression.ValueExpression;

        // Assert
        connectionString.Should().StartWith("http://");
    }

    [Fact]
    public void PortForwardExecutable_CanBeSet()
    {
        // Arrange
        var appBuilder = DistributedApplication.CreateBuilder();
        var mockService = appBuilder.AddExternalService("mock-external", "http://localhost:8080");
        var executable = appBuilder.AddExecutable("test-exe", "pwsh", ".", "-Version");

        // Act
        var resource = new ExternalAksServiceResource("test-resource")
        {
            AksResouce = mockService,
            LocalPort = 8080,
            PortForwardExecutable = executable
        };

        // Assert
        resource.PortForwardExecutable.Should().NotBeNull();
        resource.PortForwardExecutable.Should().BeSameAs(executable);
    }

    [Fact]
    public void AksResource_IsRequired()
    {
        // Arrange
        var appBuilder = DistributedApplication.CreateBuilder();
        var mockService = appBuilder.AddExternalService("mock-external", "http://localhost:8080");

        // Act
        var resource = new ExternalAksServiceResource("test-resource")
        {
            AksResouce = mockService,
            LocalPort = 8080
        };

        // Assert
        resource.AksResouce.Should().NotBeNull();
    }
}
