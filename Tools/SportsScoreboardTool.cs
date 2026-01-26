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
    [McpServerTool, Description("Get sports game results, scores, schedules, matches, fixtures, standings, and outcomes for professional and college sports leagues. Use this for any queries about game results, who won, final scores, match outcomes, team performance, game schedules, upcoming games, or any sports-related questions about specific dates or current events.")]
    public static async Task<string> GetScoreboard(
        HttpClient client,
        IOptions<SportsApiOptions> options,
        [Description("The sport type (e.g., for basketball, football, soccer, baseball, hockey, tennis, golf queries)")] Sports sport,
        [Description("The specific league or competition (e.g., NBA, NFL, Premier League, MLB, NHL, ATP, PGA for professional; NCAA for college sports)")] Leagues league,
        [Description("The date for game results or schedule. Use for queries like 'yesterday', 'last night', 'today', or specific dates. Leave empty for current/latest results.")] DateTime? date = null)
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

            try
            {
                var scoreboardDto = ScoreboardResponseDtoExtensions.FromJsonElement(jsonDocument.RootElement);
                return JsonSerializer.Serialize(scoreboardDto, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception deserializationEx)
            {
                Console.Error.WriteLine($"Error deserializing scoreboard data: {deserializationEx.Message}");
                return JsonSerializer.Serialize(new { error = "Failed to parse scoreboard data", details = deserializationEx.Message, url = fullUrl });
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting scoreboard: {ex.Message}");
            Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");

            return JsonSerializer.Serialize(new { error = ex.Message, url = fullUrl });
        }
    }
}