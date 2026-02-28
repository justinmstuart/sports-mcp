using sports_mcp.Models;

namespace sports_mcp.Tests.Models;

public class SportsExtensionsTests
{
    [Theory]
    [InlineData(Sports.Football, "football")]
    [InlineData(Sports.Basketball, "basketball")]
    [InlineData(Sports.Baseball, "baseball")]
    [InlineData(Sports.Soccer, "soccer")]
    [InlineData(Sports.Hockey, "hockey")]
    [InlineData(Sports.Golf, "golf")]
    [InlineData(Sports.Racing, "racing")]
    [InlineData(Sports.Tennis, "tennis")]
    public void ToSportString_ReturnsExpectedString(Sports sport, string expected)
    {
        var result = sport.ToSportString();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToSportString_AllEnumValues_AreMapped()
    {
        foreach (Sports sport in Enum.GetValues<Sports>())
        {
            var result = sport.ToSportString();

            Assert.NotEqual("unknown", result);
            Assert.NotEmpty(result);
        }
    }
}
