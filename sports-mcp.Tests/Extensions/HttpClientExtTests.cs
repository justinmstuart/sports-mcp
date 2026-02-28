using System.Net;
using System.Text;

using Extensions;

namespace sports_mcp.Tests.Extensions;

public class HttpClientExtTests
{
    private static HttpClient CreateClient(string responseBody, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handler = new MockHttpMessageHandler(responseBody, statusCode);
        return new HttpClient(handler) { BaseAddress = new Uri("http://test.espn.com/") };
    }

    [Fact]
    public async Task ReadJsonDocumentAsync_ValidJson_ReturnsDocument()
    {
        var json = """{"leagues":[{"id":"46","name":"NBA","abbreviation":"NBA","slug":"nba"}],"events":[]}""";
        using var client = CreateClient(json);

        using var doc = await client.ReadJsonDocumentAsync("sports/basketball/nba/scoreboard");

        Assert.Equal("46", doc.RootElement.GetProperty("leagues")[0].GetProperty("id").GetString());
    }

    [Fact]
    public async Task ReadJsonDocumentAsync_HttpError_ThrowsHttpRequestException()
    {
        using var client = CreateClient("Not Found", HttpStatusCode.NotFound);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => client.ReadJsonDocumentAsync("sports/basketball/nba/scoreboard"));
    }

    [Fact]
    public async Task ReadJsonDocumentAsync_UsesProvidedRelativeUri()
    {
        var json = "{}";
        string? capturedUri = null;

        var handler = new CapturingHttpMessageHandler(req =>
        {
            capturedUri = req.RequestUri?.PathAndQuery;
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
        });

        using var client = new HttpClient(handler) { BaseAddress = new Uri("http://test.espn.com/") };

        using var _ = await client.ReadJsonDocumentAsync("sports/basketball/nba/scoreboard");

        Assert.Equal("/sports/basketball/nba/scoreboard", capturedUri);
    }

    private sealed class MockHttpMessageHandler(string responseBody, HttpStatusCode statusCode) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }

    private sealed class CapturingHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(handler(request));
    }
}
