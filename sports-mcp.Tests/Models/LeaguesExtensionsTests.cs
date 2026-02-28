using sports_mcp.Models;

namespace sports_mcp.Tests.Models;

public class LeaguesExtensionsTests
{
    [Theory]
    [InlineData(Leagues.NFL, "nfl")]
    [InlineData(Leagues.CollegeFootball, "college-football")]
    [InlineData(Leagues.NBA, "nba")]
    [InlineData(Leagues.WNBA, "wnba")]
    [InlineData(Leagues.MenCollegeBasketball, "mens-college-basketball")]
    [InlineData(Leagues.WomenCollegeBasketball, "womens-college-basketball")]
    [InlineData(Leagues.MLB, "mlb")]
    [InlineData(Leagues.MenCollegeBaseball, "mens-college-baseball")]
    [InlineData(Leagues.EPL, "eng.1")]
    [InlineData(Leagues.MLS, "usa.1")]
    [InlineData(Leagues.UEFAEuropaLeague, "UEFA.EUROPA")]
    [InlineData(Leagues.UEFAChampionsLeague, "uefa.champions")]
    [InlineData(Leagues.NHL, "nhl")]
    [InlineData(Leagues.MenCollegeHockey, "mens-college-hockey")]
    [InlineData(Leagues.WomenCollegeHockey, "womens-college-hockey")]
    [InlineData(Leagues.PGA, "pga")]
    [InlineData(Leagues.LPGA, "lpga")]
    [InlineData(Leagues.EuropeanTour, "eur")]
    [InlineData(Leagues.NASCAR, "irl")]
    [InlineData(Leagues.F1, "f1")]
    [InlineData(Leagues.ATP, "atp")]
    [InlineData(Leagues.WTA, "wta")]
    public void ToLeagueString_ReturnsExpectedString(Leagues league, string expected)
    {
        var result = league.ToLeagueString();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToLeagueString_AllEnumValues_AreMapped()
    {
        foreach (Leagues league in Enum.GetValues<Leagues>())
        {
            var result = league.ToLeagueString();

            Assert.NotEqual("unknown", result);
            Assert.NotEmpty(result);
        }
    }
}
