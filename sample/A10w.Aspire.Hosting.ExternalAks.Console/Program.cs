using Aspire.Hosting.ExternalAks.Console;
using Hex1b;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Add Aspire service defaults (service discovery, resilience, telemetry, health checks)
builder.AddServiceDefaults();

// Add HttpClient for making requests to the documents service
builder.Services.AddHttpClient();

// Get the connection string from the configuration
var connectionString = builder.Configuration.GetConnectionString("svc-documents");

using var host = builder.Build();

// Create application state
var state = new AppState(host.Services, connectionString);

if (!CanUseInteractiveConsole())
{
    await state.TestConnection();
    WriteNonInteractiveSummary(state);
    Environment.ExitCode = state.ExitCode;
    return;
}

// Create Hex1b app
var app = new Hex1bApp(ctx => MainView.Build(ctx, state));

// Start the connection test
_ = Task.Run(async () =>
{
    await Task.Delay(500); // Brief delay to show UI first
    await state.TestConnection();
});

await app.RunAsync();

static bool CanUseInteractiveConsole()
{
    return !Console.IsInputRedirected
        && !Console.IsOutputRedirected
        && !Console.IsErrorRedirected;
}

static void WriteNonInteractiveSummary(AppState state)
{
    Console.WriteLine("AKS External Service Connection Test");
    Console.WriteLine($"Connection String: {state.ConnectionString ?? "NOT CONFIGURED"}");

    if (!string.IsNullOrWhiteSpace(state.Error))
    {
        Console.WriteLine($"Error: {state.Error}");
        return;
    }

    Console.WriteLine($"Status Code: {state.StatusCode}");
    Console.WriteLine($"Endpoint: {state.Endpoint}");

    if (!string.IsNullOrWhiteSpace(state.Response))
    {
        Console.WriteLine("Response:");
        Console.WriteLine(state.Response);
    }
}
