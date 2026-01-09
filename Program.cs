using System.Net.Http.Headers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Options;

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    ContentRootPath = AppContext.BaseDirectory
});

// Disable console logging to prevent interference with MCP stdio protocol
// But keep logging to stderr for debugging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Configure SportsApiOptions with validation
builder.Services.AddOptions<SportsApiOptions>()
    .Bind(builder.Configuration.GetSection("SportsApi"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

builder.Services.AddSingleton<HttpClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<SportsApiOptions>>().Value;
    var handler = new HttpClientHandler();
    var client = new HttpClient(handler) { BaseAddress = new Uri(options.BaseUrl) };
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");
    return client;
});

var app = builder.Build();

await app.RunAsync();
