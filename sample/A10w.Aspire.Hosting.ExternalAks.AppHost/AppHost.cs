var builder = DistributedApplication.CreateBuilder(args);

var documentsService = builder.AddExternalAksService("svc-documents", options =>
	{
		options.KubernetesContext = "tst-services-cluster-uks-aks";
		options.KubernetesNamespace = "dev-documentsservice";
		options.KubernetesServiceName = "svc-documents";
		options.LocalPort = 8999;
	})
    .WithHttpHealthCheck("/healthz")
    .WithHttpHealthCheck("/healthcheck");

var consoleApp = builder.AddProject<Projects.A10w_Aspire_Hosting_ExternalAks_Console>("documents-console")
	.WithReference(documentsService);

builder.Build().Run();
