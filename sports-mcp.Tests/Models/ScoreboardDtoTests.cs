using System.Text.Json;

using sports_mcp.Models;

namespace sports_mcp.Tests.Models;

public class ScoreboardDtoTests
{
    private static JsonElement ParseJson(string json) =>
        JsonDocument.Parse(json).RootElement;

    // ── LeagueDtoExtensions ─────────────────────────────────────────────────

    [Fact]
    public void LeagueDtoFromJsonElement_ParsesAllFields()
    {
        var json = """
            {
              "id": "46",
              "name": "National Basketball Association",
              "abbreviation": "NBA",
              "slug": "nba"
            }
            """;

        var result = LeagueDtoExtensions.FromJsonElement(ParseJson(json));

        Assert.Equal("46", result.Id);
        Assert.Equal("National Basketball Association", result.Name);
        Assert.Equal("NBA", result.Abbreviation);
        Assert.Equal("nba", result.Slug);
    }

    // ── GameStatusDtoExtensions ─────────────────────────────────────────────

    [Fact]
    public void GameStatusDtoFromJsonElement_ParsesCompletedGame()
    {
        var json = """
            {
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
            }
            """;

        var result = GameStatusDtoExtensions.FromJsonElement(ParseJson(json));

        Assert.Equal(0.0, result.Clock);
        Assert.Equal("0:00", result.DisplayClock);
        Assert.Equal(4, result.Period);
        Assert.Equal("3", result.Type.Id);
        Assert.Equal("STATUS_FINAL", result.Type.Name);
        Assert.Equal("post", result.Type.State);
        Assert.True(result.Type.Completed);
        Assert.Equal("Final", result.Type.Description);
        Assert.Equal("Final", result.Type.Detail);
        Assert.Equal("Final", result.Type.ShortDetail);
    }

    [Fact]
    public void GameStatusDtoFromJsonElement_ParsesInProgressGame()
    {
        var json = """
            {
              "clock": 300.0,
              "displayClock": "5:00",
              "period": 3,
              "type": {
                "id": "2",
                "name": "STATUS_IN_PROGRESS",
                "state": "in",
                "completed": false,
                "description": "In Progress",
                "detail": "5:00 - 3rd Quarter",
                "shortDetail": "5:00 - 3rd Qtr"
              }
            }
            """;

        var result = GameStatusDtoExtensions.FromJsonElement(ParseJson(json));

        Assert.Equal(300.0, result.Clock);
        Assert.Equal(3, result.Period);
        Assert.False(result.Type.Completed);
        Assert.Equal("in", result.Type.State);
    }

    // ── VenueDtoExtensions ──────────────────────────────────────────────────

    [Fact]
    public void VenueDtoFromJsonElement_ParsesWithStateAndCountry()
    {
        var json = """
            {
              "id": "1893",
              "fullName": "Spectrum Center",
              "address": {
                "city": "Charlotte",
                "state": "NC"
              }
            }
            """;

        var result = VenueDtoExtensions.FromJsonElement(ParseJson(json));

        Assert.Equal("1893", result.Id);
        Assert.Equal("Spectrum Center", result.FullName);
        Assert.NotNull(result.Address);
        Assert.Equal("Charlotte", result.Address.City);
        Assert.Equal("NC", result.Address.State);
        Assert.Null(result.Address.Country);
    }

    [Fact]
    public void VenueDtoFromJsonElement_ParsesInternationalVenueWithCountry()
    {
        var json = """
            {
              "id": "999",
              "fullName": "Wembley Stadium",
              "address": {
                "city": "London",
                "country": "GB"
              }
            }
            """;

        var result = VenueDtoExtensions.FromJsonElement(ParseJson(json));

        Assert.Equal("Wembley Stadium", result.FullName);
        Assert.NotNull(result.Address);
        Assert.Equal("London", result.Address.City);
        Assert.Null(result.Address.State);
        Assert.Equal("GB", result.Address.Country);
    }

    [Fact]
    public void VenueDtoFromJsonElement_ParsesWithoutAddress()
    {
        var json = """
            {
              "id": "100",
              "fullName": "TBD Arena"
            }
            """;

        var result = VenueDtoExtensions.FromJsonElement(ParseJson(json));

        Assert.Equal("TBD Arena", result.FullName);
        Assert.Null(result.Address);
    }

    // ── TeamCompetitorDtoExtensions ─────────────────────────────────────────

    [Fact]
    public void TeamCompetitorDtoFromJsonElement_ParsesWinner()
    {
        var json = """
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
                { "name": "overall", "type": "total", "summary": "1-2" },
                { "name": "Home", "type": "home", "summary": "1-0" }
              ]
            }
            """;

        var result = TeamCompetitorDtoExtensions.FromJsonElement(ParseJson(json));

        Assert.Equal("30", result.Id);
        Assert.Equal("home", result.HomeAway);
        Assert.True(result.Winner);
        Assert.Equal("117", result.Score);
        Assert.Equal("Charlotte Hornets", result.Team.DisplayName);
        Assert.Equal("CHA", result.Team.Abbreviation);
        Assert.Equal("008ca8", result.Team.Color);
        Assert.Equal(2, result.Records.Count);
        Assert.Equal("1-2", result.Records[0].Summary);
    }

    [Fact]
    public void TeamCompetitorDtoFromJsonElement_WinnerIsFalseWhenPropertyMissing()
    {
        var json = """
            {
              "id": "25",
              "homeAway": "away",
              "score": "105",
              "team": {
                "id": "25",
                "displayName": "Oklahoma City Thunder",
                "abbreviation": "OKC",
                "logo": "https://a.espncdn.com/logo.png",
                "color": "007ac1"
              }
            }
            """;

        var result = TeamCompetitorDtoExtensions.FromJsonElement(ParseJson(json));

        Assert.False(result.Winner);
        Assert.Empty(result.Records);
    }

    [Fact]
    public void TeamCompetitorDtoFromJsonElement_WinnerIsFalseWhenPropertyIsFalse()
    {
        var json = """
            {
              "id": "25",
              "homeAway": "away",
              "winner": false,
              "score": "115",
              "team": {
                "id": "25",
                "displayName": "Oklahoma City Thunder",
                "abbreviation": "OKC",
                "logo": "https://a.espncdn.com/logo.png",
                "color": "007ac1"
              }
            }
            """;

        var result = TeamCompetitorDtoExtensions.FromJsonElement(ParseJson(json));

        Assert.False(result.Winner);
    }

    // ── GameEventDtoExtensions ──────────────────────────────────────────────

    [Fact]
    public void GameEventDtoFromJsonElement_ParsesCompletedGame()
    {
        var json = """
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
                        "logo": "https://logo.url",
                        "color": "008ca8"
                      }
                    }
                  ]
                }
              ]
            }
            """;

        var result = GameEventDtoExtensions.FromJsonElement(ParseJson(json));

        Assert.Equal("401591906", result.Id);
        Assert.Equal("Oklahoma City Thunder at Charlotte Hornets", result.Name);
        Assert.Equal("OKC @ CHA", result.ShortName);
        Assert.Equal(2024, result.Season.Year);
        Assert.Equal(1, result.Season.Type);
        Assert.Equal("preseason", result.Season.Slug);
        Assert.Null(result.Week);
        Assert.Equal("Spectrum Center", result.Venue!.FullName);
        Assert.True(result.Status.Type.Completed);
        Assert.Single(result.Competitors);
        Assert.Equal("Charlotte Hornets", result.Competitors[0].Team.DisplayName);
    }

    [Fact]
    public void GameEventDtoFromJsonElement_ParsesWeekWhenPresent()
    {
        var json = """
            {
              "id": "401671782",
              "name": "Team A at Team B",
              "shortName": "A @ B",
              "date": "2023-09-10T17:00Z",
              "season": { "year": 2024, "type": 2, "slug": "regular-season" },
              "week": { "number": 1 },
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
                  "competitors": []
                }
              ]
            }
            """;

        var result = GameEventDtoExtensions.FromJsonElement(ParseJson(json));

        Assert.NotNull(result.Week);
        Assert.Equal(1, result.Week.Number);
    }

    [Fact]
    public void GameEventDtoFromJsonElement_HandlesNoCompetitions()
    {
        var json = """
            {
              "id": "999",
              "name": "TBD at TBD",
              "shortName": "TBD",
              "date": "2026-06-01T00:00Z",
              "season": { "year": 2026, "type": 2, "slug": "regular-season" },
              "competitions": []
            }
            """;

        var result = GameEventDtoExtensions.FromJsonElement(ParseJson(json));

        Assert.Equal("999", result.Id);
        Assert.Empty(result.Competitors);
        Assert.Null(result.Venue);
    }

    // ── ScoreboardResponseDtoExtensions ────────────────────────────────────

    [Fact]
    public void ScoreboardResponseDtoFromJsonElement_ParsesLeagueAndEvents()
    {
        var json = """
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
                      "competitors": []
                    }
                  ]
                }
              ]
            }
            """;

        var result = ScoreboardResponseDtoExtensions.FromJsonElement(ParseJson(json));

        Assert.NotNull(result.League);
        Assert.Equal("NBA", result.League.Abbreviation);
        Assert.Single(result.Events);
        Assert.Equal("401591906", result.Events[0].Id);
    }

    [Fact]
    public void ScoreboardResponseDtoFromJsonElement_HandlesEmptyLeaguesAndEvents()
    {
        var json = """
            {
              "leagues": [],
              "events": []
            }
            """;

        var result = ScoreboardResponseDtoExtensions.FromJsonElement(ParseJson(json));

        Assert.Null(result.League);
        Assert.Empty(result.Events);
    }

    [Fact]
    public void ScoreboardResponseDtoFromJsonElement_HandlesNoLeaguesOrEvents()
    {
        var json = "{}";

        var result = ScoreboardResponseDtoExtensions.FromJsonElement(ParseJson(json));

        Assert.Null(result.League);
        Assert.Empty(result.Events);
    }

    [Fact]
    public void ScoreboardResponseDtoFromJsonElement_ParsesMultipleEvents()
    {
        var eventJson = """
            {
              "id": "{id}",
              "name": "Team A at Team B",
              "shortName": "A @ B",
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
                  "competitors": []
                }
              ]
            }
            """;

        var event1 = eventJson.Replace("{id}", "1");
        var event2 = eventJson.Replace("{id}", "2");

        var json = $$"""
            {
              "leagues": [{ "id": "46", "name": "NBA", "abbreviation": "NBA", "slug": "nba" }],
              "events": [{{event1}}, {{event2}}]
            }
            """;

        var result = ScoreboardResponseDtoExtensions.FromJsonElement(ParseJson(json));

        Assert.Equal(2, result.Events.Count);
        Assert.Equal("1", result.Events[0].Id);
        Assert.Equal("2", result.Events[1].Id);
    }
}
