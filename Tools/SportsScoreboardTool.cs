using Extensions;

using Microsoft.Extensions.Options;

using ModelContextProtocol.Server;

using Options;

using System.ComponentModel;
using System.Text.Json;

using sports_mcp.Models;

namespace Tools;


[McpServerToolType]
public static class SportsScoreboardTool
{
    [McpServerTool, Description("Get sports scoreboard data based on sport,league and an optional date.")]
    public static async Task<string> GetScoreboard(
        HttpClient client,
        IOptions<SportsApiOptions> options,
        [Description("The sport to get scoreboard for.")] Sports sport,
        [Description("The league to get scoreboard for.")] Leagues league,
        [Description("The date to get scoreboard for.")] DateTime? date = null)
    {
        var dateString = date?.ToString("yyyyMMdd") ?? null;
        var sportLower = sport.ToSportString().ToLower();
        var leagueLower = league.ToLeagueString().ToLower();
        var url = $"sports/{sportLower}/{leagueLower}/scoreboard{(dateString is not null ? $"?dates={dateString}" : "")}";
        var fullUrl = $"{client.BaseAddress}{url}";

        try
        {
            Console.Error.WriteLine($"Requesting URL: {url}");
            Console.Error.WriteLine($"Full URL: {fullUrl}");

            using var jsonDocument = await client.ReadJsonDocumentAsync(url);

            return JsonSerializer.Serialize(jsonDocument.RootElement, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting scoreboard: {ex.Message}");
            Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");

            return JsonSerializer.Serialize(new { error = ex.Message, url = fullUrl });
        }
    }
}