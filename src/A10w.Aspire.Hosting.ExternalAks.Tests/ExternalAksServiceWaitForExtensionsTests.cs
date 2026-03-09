using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;

namespace A10w.Aspire.Hosting.ExternalAks.Tests;

public class ExternalAksServiceWaitForExtensionsTests
{
    [Fact]
    public void WaitForPortForward_WithNullBuilder_ThrowsArgumentNullException()
    {
        IResourceBuilder<ExternalAksServiceResource> builder = null!;

        var act = () => builder.WaitForPortForward();

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("builder");
    }

    [Fact]
    public void WaitForPortForward_WithoutPortForwardExecutable_ThrowsInvalidOperationException()
    {
        var appBuilder = DistributedApplication.CreateBuilder();
        var externalService = appBuilder.AddExternalService("external", "http://localhost:9000");
        var externalAksResource = new ExternalAksServiceResource("external-aks")
        {
            AksResouce = externalService,
            LocalPort = 9000
        };
        var resourceBuilder = appBuilder.AddResource(externalAksResource);

        var act = () => resourceBuilder.WaitForPortForward();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Port-forward executable is not configured*");
    }

    [Fact]
    public void WaitForPortForward_WithPortForwardExecutable_ReturnsSameBuilder()
    {
        var appBuilder = DistributedApplication.CreateBuilder();
        var externalService = appBuilder.AddExternalService("external", "http://localhost:9001");
        var executable = appBuilder.AddExecutable("port-forward", "pwsh", appBuilder.AppHostDirectory, "-NoLogo");
        var externalAksResource = new ExternalAksServiceResource("external-aks")
        {
            AksResouce = externalService,
            PortForwardExecutable = executable,
            LocalPort = 9001
        };
        var resourceBuilder = appBuilder.AddResource(externalAksResource);

        var result = resourceBuilder.WaitForPortForward();

        result.Should().BeSameAs(resourceBuilder);
    }

    [Fact]
    public void WaitForPortForward_AddsWaitAnnotationToExternalService()
    {
        var appBuilder = DistributedApplication.CreateBuilder();
        var externalService = appBuilder.AddExternalService("external", "http://localhost:9002");
        var executable = appBuilder.AddExecutable("port-forward", "pwsh", appBuilder.AppHostDirectory, "-NoLogo");
        var externalAksResource = new ExternalAksServiceResource("external-aks")
        {
            AksResouce = externalService,
            PortForwardExecutable = executable,
            LocalPort = 9002
        };
        var resourceBuilder = appBuilder.AddResource(externalAksResource);

        resourceBuilder.WaitForPortForward();

        var waitAnnotation = externalService.Resource.Annotations
            .OfType<WaitAnnotation>()
            .FirstOrDefault();

        waitAnnotation.Should().NotBeNull();
        waitAnnotation!.Resource.Should().BeSameAs(executable.Resource);
        waitAnnotation.WaitType.Should().Be(WaitType.WaitUntilStarted);
    }
}
