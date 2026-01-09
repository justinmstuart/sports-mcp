namespace sports_mcp.Models;

public enum Sports
{
    Football,
    Basketball,
    Baseball,
    Soccer,
    Hockey,
    Golf,
    Racing,
    Tennis
}

public static class SportsExtensions
{
    public static string ToSportString(this Sports sport)
    {
        switch (sport)
        {
            case Sports.Football:
                return "football";
            case Sports.Basketball:
                return "basketball";
            case Sports.Baseball:
                return "baseball";
            case Sports.Soccer:
                return "soccer";
            case Sports.Hockey:
                return "hockey";
            case Sports.Golf:
                return "golf";
            case Sports.Racing:
                return "racing";
            case Sports.Tennis:
                return "tennis";
            default:
                return "unknown";
        }
    }
}
