using Microsoft.Extensions.DependencyInjection;

internal class AppState
{
    private readonly IServiceProvider _services;
    private readonly string? _connectionString;

    public bool IsLoading { get; private set; } = true;
    public string? Error { get; private set; }
    public string? StatusCode { get; private set; }
    public string Endpoint { get; private set; } = "";
    public string? Response { get; private set; }
    public int ExitCode { get; private set; } = 0;

	public string? ConnectionString => _connectionString;

    public AppState(IServiceProvider services, string? connectionString)
    {
        _services = services;
        _connectionString = connectionString;
    }

    public async Task TestConnection()
    {
        IsLoading = true;
        Error = null;
        
        if (string.IsNullOrEmpty(_connectionString))
        {
            Error = "Connection string for 'svc-documents' is not configured.";
            ExitCode = 1;
            IsLoading = false;
            return;
        }

        var httpClientFactory = _services.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient();
        
        Endpoint = $"{_connectionString.TrimEnd('/')}/healthz";

        try
        {
            var response = await httpClient.GetAsync(Endpoint);
            StatusCode = response.StatusCode.ToString();
            
            if (response.IsSuccessStatusCode)
            {
                Response = await response.Content.ReadAsStringAsync();
                ExitCode = 0;
            }
            else
            {
                Error = $"Health check failed with status code: {response.StatusCode}";
                ExitCode = 1;
            }
        }
        catch (HttpRequestException ex)
        {
            Error = $"Failed to connect: {ex.Message}";
            ExitCode = 1;
        }
        catch (Exception ex)
        {
            Error = $"Unexpected error: {ex.Message}";
            ExitCode = 1;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
