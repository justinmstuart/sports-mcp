namespace sports_mcp.Models;

public enum Leagues
{
    NFL,
    CollegeFootball,
    NBA,
    WNBA,
    MenCollegeBasketball,
    WomenCollegeBasketball,
    MLB,
    MenCollegeBaseball,
    EPL,
    MLS,
    UEFAEuropaLeague,
    UEFAChampionsLeague,
    NHL,
    MenCollegeHockey,
    WomenCollegeHockey,
    PGA,
    LPGA,
    EuropeanTour,
    NASCAR,
    F1,
    ATP,
    WTA,
}

public static class LeaguesExtensions
{
    public static string ToLeagueString(this Leagues league)
    {
        switch (league)
        {
            case Leagues.NFL:
                return "nfl";
            case Leagues.CollegeFootball:
                return "college-football";
            case Leagues.NBA:
                return "nba";
            case Leagues.WNBA:
                return "wnba";
            case Leagues.MenCollegeBasketball:
                return "mens-college-basketball";
            case Leagues.WomenCollegeBasketball:
                return "womens-college-basketball";
            case Leagues.MLB:
                return "mlb";
            case Leagues.MenCollegeBaseball:
                return "mens-college-baseball";
            case Leagues.EPL:
                return "eng.1";
            case Leagues.MLS:
                return "usa.1";
            case Leagues.UEFAEuropaLeague:
                return "UEFA.EUROPA";
            case Leagues.UEFAChampionsLeague:
                return "uefa.champions";
            case Leagues.NHL:
                return "nhl";
            case Leagues.MenCollegeHockey:
                return "mens-college-hockey";
            case Leagues.WomenCollegeHockey:
                return "womens-college-hockey";
            case Leagues.PGA:
                return "pga";
            case Leagues.LPGA:
                return "lpga";
            case Leagues.EuropeanTour:
                return "eur";
            case Leagues.NASCAR:
                return "irl";
            case Leagues.F1:
                return "f1";
            case Leagues.ATP:
                return "atp";
            case Leagues.WTA:
                return "wta";
            default:
                return "unknown";
        }
    }
}
