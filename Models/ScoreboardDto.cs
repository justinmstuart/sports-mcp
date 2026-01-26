using System.Text.Json;

namespace sports_mcp.Models;

public class ScoreboardResponseDto
{
    public List<GameEventDto> Events { get; set; } = new();
    public LeagueDto? League { get; set; }
}

public static class ScoreboardResponseDtoExtensions
{
    public static ScoreboardResponseDto FromJsonElement(JsonElement root)
    {
        var dto = new ScoreboardResponseDto();

        // Extract league
        if (root.TryGetProperty("leagues", out var leagues) && leagues.GetArrayLength() > 0)
        {
            dto.League = LeagueDtoExtensions.FromJsonElement(leagues[0]);
        }

        // Extract events
        if (root.TryGetProperty("events", out var events))
        {
            foreach (var eventElement in events.EnumerateArray())
            {
                dto.Events.Add(GameEventDtoExtensions.FromJsonElement(eventElement));
            }
        }

        return dto;
    }
}

public class LeagueDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

public static class LeagueDtoExtensions
{
    public static LeagueDto FromJsonElement(JsonElement league)
    {
        return new LeagueDto
        {
            Id = league.GetProperty("id").GetString() ?? string.Empty,
            Name = league.GetProperty("name").GetString() ?? string.Empty,
            Abbreviation = league.GetProperty("abbreviation").GetString() ?? string.Empty,
            Slug = league.GetProperty("slug").GetString() ?? string.Empty
        };
    }
}

public class GameEventDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public SeasonDto Season { get; set; } = new();
    public WeekDto? Week { get; set; }
    public List<TeamCompetitorDto> Competitors { get; set; } = new();
    public GameStatusDto Status { get; set; } = new();
    public VenueDto? Venue { get; set; }
}

public static class GameEventDtoExtensions
{
    public static GameEventDto FromJsonElement(JsonElement eventElement)
    {
        var gameEvent = new GameEventDto
        {
            Id = eventElement.GetProperty("id").GetString() ?? string.Empty,
            Name = eventElement.GetProperty("name").GetString() ?? string.Empty,
            ShortName = eventElement.GetProperty("shortName").GetString() ?? string.Empty,
            Date = eventElement.GetProperty("date").GetDateTime()
        };

        // Season
        if (eventElement.TryGetProperty("season", out var season))
        {
            gameEvent.Season = new SeasonDto
            {
                Year = season.GetProperty("year").GetInt32(),
                Type = season.GetProperty("type").GetInt32(),
                Slug = season.GetProperty("slug").GetString() ?? string.Empty
            };
        }

        // Week
        if (eventElement.TryGetProperty("week", out var week))
        {
            gameEvent.Week = new WeekDto { Number = week.GetProperty("number").GetInt32() };
        }

        // Extract from first competition
        if (eventElement.TryGetProperty("competitions", out var competitions) && competitions.GetArrayLength() > 0)
        {
            var competition = competitions[0];

            // Status
            if (competition.TryGetProperty("status", out var status))
            {
                gameEvent.Status = GameStatusDtoExtensions.FromJsonElement(status);
            }

            // Venue
            if (competition.TryGetProperty("venue", out var venue))
            {
                gameEvent.Venue = VenueDtoExtensions.FromJsonElement(venue);
            }

            // Competitors
            if (competition.TryGetProperty("competitors", out var competitors))
            {
                foreach (var competitor in competitors.EnumerateArray())
                {
                    gameEvent.Competitors.Add(TeamCompetitorDtoExtensions.FromJsonElement(competitor));
                }
            }
        }

        return gameEvent;
    }
}

public class SeasonDto
{
    public int Year { get; set; }
    public int Type { get; set; }
    public string Slug { get; set; } = string.Empty;
}

public class WeekDto
{
    public int Number { get; set; }
}

public class TeamCompetitorDto
{
    public string Id { get; set; } = string.Empty;
    public string HomeAway { get; set; } = string.Empty;
    public bool Winner { get; set; }
    public TeamDto Team { get; set; } = new();
    public string Score { get; set; } = string.Empty;
    public List<RecordDto> Records { get; set; } = new();
}

public static class TeamCompetitorDtoExtensions
{
    public static TeamCompetitorDto FromJsonElement(JsonElement competitor)
    {
        var competitorDto = new TeamCompetitorDto
        {
            Id = competitor.GetProperty("id").GetString() ?? string.Empty,
            HomeAway = competitor.GetProperty("homeAway").GetString() ?? string.Empty,
            Winner = competitor.TryGetProperty("winner", out var winner) && winner.ValueKind == JsonValueKind.True,
            Score = competitor.GetProperty("score").GetString() ?? string.Empty
        };

        if (competitor.TryGetProperty("team", out var team))
        {
            competitorDto.Team = new TeamDto
            {
                Id = team.GetProperty("id").GetString() ?? string.Empty,
                DisplayName = team.GetProperty("displayName").GetString() ?? string.Empty,
                Abbreviation = team.GetProperty("abbreviation").GetString() ?? string.Empty,
                Logo = team.GetProperty("logo").GetString() ?? string.Empty,
                Color = team.GetProperty("color").GetString() ?? string.Empty
            };
        }

        if (competitor.TryGetProperty("records", out var records))
        {
            foreach (var record in records.EnumerateArray())
            {
                competitorDto.Records.Add(new RecordDto
                {
                    Name = record.GetProperty("name").GetString() ?? string.Empty,
                    Type = record.GetProperty("type").GetString() ?? string.Empty,
                    Summary = record.GetProperty("summary").GetString() ?? string.Empty
                });
            }
        }

        return competitorDto;
    }
}

public class TeamDto
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class RecordDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}

public class GameStatusDto
{
    public double Clock { get; set; }
    public string DisplayClock { get; set; } = string.Empty;
    public int Period { get; set; }
    public StatusTypeDto Type { get; set; } = new();
}

public static class GameStatusDtoExtensions
{
    public static GameStatusDto FromJsonElement(JsonElement status)
    {
        var statusDto = new GameStatusDto
        {
            Clock = status.GetProperty("clock").GetDouble(),
            DisplayClock = status.GetProperty("displayClock").GetString() ?? string.Empty,
            Period = status.GetProperty("period").GetInt32()
        };

        if (status.TryGetProperty("type", out var statusType))
        {
            statusDto.Type = new StatusTypeDto
            {
                Id = statusType.GetProperty("id").GetString() ?? string.Empty,
                Name = statusType.GetProperty("name").GetString() ?? string.Empty,
                State = statusType.GetProperty("state").GetString() ?? string.Empty,
                Completed = statusType.GetProperty("completed").GetBoolean(),
                Description = statusType.GetProperty("description").GetString() ?? string.Empty,
                Detail = statusType.GetProperty("detail").GetString() ?? string.Empty,
                ShortDetail = statusType.GetProperty("shortDetail").GetString() ?? string.Empty
            };
        }

        return statusDto;
    }
}

public class StatusTypeDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public string ShortDetail { get; set; } = string.Empty;
}

public class VenueDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public VenueAddressDto? Address { get; set; }
}

public static class VenueDtoExtensions
{
    public static VenueDto FromJsonElement(JsonElement venue)
    {
        var venueDto = new VenueDto
        {
            Id = venue.GetProperty("id").GetString() ?? string.Empty,
            FullName = venue.GetProperty("fullName").GetString() ?? string.Empty
        };

        if (venue.TryGetProperty("address", out var address))
        {
            venueDto.Address = new VenueAddressDto
            {
                City = address.GetProperty("city").GetString() ?? string.Empty,
                State = address.TryGetProperty("state", out var state) ? state.GetString() : null,
                Country = address.TryGetProperty("country", out var country) ? country.GetString() : null
            };
        }

        return venueDto;
    }
}

public class VenueAddressDto
{
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? Country { get; set; }
}
