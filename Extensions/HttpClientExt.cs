using System.Text.Json;

namespace Extensions;

internal static class HttpClientExt
{
    public static async Task<JsonDocument> ReadJsonDocumentAsync(this HttpClient client, string requestUri)
    {
        Console.Error.WriteLine($"Making request to: {requestUri}");
        Console.Error.WriteLine($"Base address: {client.BaseAddress}");
        Console.Error.WriteLine($"Headers: {string.Join(", ", client.DefaultRequestHeaders.Select(h => $"{h.Key}={string.Join(",", h.Value)}"))}");
        
        using var response = await client.GetAsync(requestUri);
        
        Console.Error.WriteLine($"Response status: {response.StatusCode}");
        Console.Error.WriteLine($"Response headers: {string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}"))}");
        
        response.EnsureSuccessStatusCode();
        return await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
    }
}