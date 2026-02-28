using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Options;

using Options;

using sports_mcp.Models;

using Tools;

namespace sports_mcp.Tests.Tools;

public class SportsScoreboardToolTests
{
    private static readonly string ValidNbaScoreboardJson = """
        {
          "leagues": [
            {
              "id": "46",
              "name": "National Basketball Association",
              "abbreviation": "NBA",
              "slug": "nba"
            }
          ],
          "events": [
            {
              "id": "401591906",
              "name": "Oklahoma City Thunder at Charlotte Hornets",
              "shortName": "OKC @ CHA",
              "date": "2023-10-15T21:00Z",
              "season": { "year": 2024, "type": 1, "slug": "preseason" },
              "competitions": [
                {
                  "status": {
                    "clock": 0.0,
                    "displayClock": "0:00",
                    "period": 4,
                    "type": {
                      "id": "3",
                      "name": "STATUS_FINAL",
                      "state": "post",
                      "completed": true,
                      "description": "Final",
                      "detail": "Final",
                      "shortDetail": "Final"
                    }
                  },
                  "venue": {
                    "id": "1893",
                    "fullName": "Spectrum Center",
                    "address": { "city": "Charlotte", "state": "NC" }
                  },
                  "competitors": [
                    {
                      "id": "30",
                      "homeAway": "home",
                      "winner": true,
                      "score": "117",
                      "team": {
                        "id": "30",
                        "displayName": "Charlotte Hornets",
                        "abbreviation": "CHA",
                        "logo": "https://a.espncdn.com/i/teamlogos/nba/500/scoreboard/cha.png",
                        "color": "008ca8"
                      },
                      "records": [
                        { "name": "overall", "type": "total", "summary": "1-2" }
                      ]
                    },
                    {
                      "id": "25",
                      "homeAway": "away",
                      "winner": false,
                      "score": "115",
                      "team": {
                        "id": "25",
                        "displayName": "Oklahoma City Thunder",
                        "abbreviation": "OKC",
                        "logo": "https://a.espncdn.com/i/teamlogos/nba/500/scoreboard/okc.png",
                        "color": "007ac1"
                      },
                      "records": []
                    }
                  ]
                }
              ]
            }
          ]
        }
        """;

    private static HttpClient CreateHttpClient(string responseBody, HttpStatusCode statusCode = HttpStatusCode.OK, string? baseUrl = null)
    {
        var handler = new MockHttpMessageHandler(responseBody, statusCode);
        return new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl ?? "http://test.espn.com/")
        };
    }

    private static IOptions<SportsApiOptions> CreateOptions(string baseUrl = "http://test.espn.com/") =>
        Microsoft.Extensions.Options.Options.Create(new SportsApiOptions { BaseUrl = baseUrl });

    [Fact]
    public async Task GetScoreboard_ValidResponse_ReturnsSerializedScoreboard()
    {
        using var client = CreateHttpClient(ValidNbaScoreboardJson);
        var options = CreateOptions();

        var result = await SportsScoreboardTool.GetScoreboard(client, options, Sports.Basketball, Leagues.NBA);

        var json = JsonDocument.Parse(result);
        Assert.Equal("NBA", json.RootElement.GetProperty("League").GetProperty("Abbreviation").GetString());
        Assert.Equal(1, json.RootElement.GetProperty("Events").GetArrayLength());
    }

    [Fact]
    public async Task GetScoreboard_ValidResponse_ReturnsCorrectEventData()
    {
        using var client = CreateHttpClient(ValidNbaScoreboardJson);
        var options = CreateOptions();

        var result = await SportsScoreboardTool.GetScoreboard(client, options, Sports.Basketball, Leagues.NBA);

        var json = JsonDocument.Parse(result);
        var firstEvent = json.RootElement.GetProperty("Events")[0];
        Assert.Equal("401591906", firstEvent.GetProperty("Id").GetString());
        Assert.Equal("Oklahoma City Thunder at Charlotte Hornets", firstEvent.GetProperty("Name").GetString());
        Assert.Equal("OKC @ CHA", firstEvent.GetProperty("ShortName").GetString());
    }

    [Fact]
    public async Task GetScoreboard_ValidResponse_ReturnsCompetitorData()
    {
        using var client = CreateHttpClient(ValidNbaScoreboardJson);
        var options = CreateOptions();

        var result = await SportsScoreboardTool.GetScoreboard(client, options, Sports.Basketball, Leagues.NBA);

        var json = JsonDocument.Parse(result);
        var competitors = json.RootElement.GetProperty("Events")[0].GetProperty("Competitors");
        Assert.Equal(2, competitors.GetArrayLength());

        var homeTeam = competitors[0];
        Assert.Equal("home", homeTeam.GetProperty("HomeAway").GetString());
        Assert.True(homeTeam.GetProperty("Winner").GetBoolean());
        Assert.Equal("117", homeTeam.GetProperty("Score").GetString());
        Assert.Equal("Charlotte Hornets", homeTeam.GetProperty("Team").GetProperty("DisplayName").GetString());
    }

    [Fact]
    public async Task GetScoreboard_WithDate_IncludesDateInRequest()
    {
        string? capturedUrl = null;

        var handler = new CapturingHttpMessageHandler(req =>
        {
            capturedUrl = req.RequestUri?.ToString();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(ValidNbaScoreboardJson, Encoding.UTF8, "application/json")
            };
        });

        using var client = new HttpClient(handler) { BaseAddress = new Uri("http://test.espn.com/") };
        var options = CreateOptions();

        await SportsScoreboardTool.GetScoreboard(client, options, Sports.Basketball, Leagues.NBA, new DateTime(2026, 1, 25));

        Assert.NotNull(capturedUrl);
        Assert.Contains("dates=20260125", capturedUrl);
    }

    [Fact]
    public async Task GetScoreboard_WithoutDate_DoesNotIncludeDateInRequest()
    {
        string? capturedUrl = null;

        var handler = new CapturingHttpMessageHandler(req =>
        {
            capturedUrl = req.RequestUri?.ToString();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(ValidNbaScoreboardJson, Encoding.UTF8, "application/json")
            };
        });

        using var client = new HttpClient(handler) { BaseAddress = new Uri("http://test.espn.com/") };
        var options = CreateOptions();

        await SportsScoreboardTool.GetScoreboard(client, options, Sports.Basketball, Leagues.NBA);

        Assert.NotNull(capturedUrl);
        Assert.DoesNotContain("dates=", capturedUrl);
    }

    [Fact]
    public async Task GetScoreboard_WithSportAndLeague_BuildsCorrectUrl()
    {
        string? capturedUrl = null;

        var handler = new CapturingHttpMessageHandler(req =>
        {
            capturedUrl = req.RequestUri?.ToString();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(ValidNbaScoreboardJson, Encoding.UTF8, "application/json")
            };
        });

        using var client = new HttpClient(handler) { BaseAddress = new Uri("http://test.espn.com/") };
        var options = CreateOptions();

        await SportsScoreboardTool.GetScoreboard(client, options, Sports.Football, Leagues.NFL);

        Assert.NotNull(capturedUrl);
        Assert.Contains("sports/football/nfl/scoreboard", capturedUrl);
    }

    [Fact]
    public async Task GetScoreboard_HttpError_ReturnsErrorJson()
    {
        using var client = CreateHttpClient("Not Found", HttpStatusCode.NotFound);
        var options = CreateOptions();

        var result = await SportsScoreboardTool.GetScoreboard(client, options, Sports.Basketball, Leagues.NBA);

        var json = JsonDocument.Parse(result);
        Assert.True(json.RootElement.TryGetProperty("error", out _));
    }

    [Fact]
    public async Task GetScoreboard_InvalidJson_ReturnsErrorJson()
    {
        using var client = CreateHttpClient("not valid json at all");
        var options = CreateOptions();

        var result = await SportsScoreboardTool.GetScoreboard(client, options, Sports.Basketball, Leagues.NBA);

        var json = JsonDocument.Parse(result);
        Assert.True(json.RootElement.TryGetProperty("error", out _));
    }

    [Fact]
    public async Task GetScoreboard_EmptyEventsResponse_ReturnsEmptyEventsList()
    {
        var emptyJson = """
            {
              "leagues": [
                { "id": "46", "name": "NBA", "abbreviation": "NBA", "slug": "nba" }
              ],
              "events": []
            }
            """;

        using var client = CreateHttpClient(emptyJson);
        var options = CreateOptions();

        var result = await SportsScoreboardTool.GetScoreboard(client, options, Sports.Basketball, Leagues.NBA);

        var json = JsonDocument.Parse(result);
        Assert.Equal(0, json.RootElement.GetProperty("Events").GetArrayLength());
        Assert.Equal("NBA", json.RootElement.GetProperty("League").GetProperty("Abbreviation").GetString());
    }

    [Fact]
    public async Task GetScoreboard_ResultIsFormattedJson()
    {
        using var client = CreateHttpClient(ValidNbaScoreboardJson);
        var options = CreateOptions();

        var result = await SportsScoreboardTool.GetScoreboard(client, options, Sports.Basketball, Leagues.NBA);

        Assert.Contains("\n", result);
        Assert.Contains("  ", result);
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
