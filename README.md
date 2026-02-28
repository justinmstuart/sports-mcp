# Sports MCP Server

A Model Context Protocol (MCP) server that provides real-time sports scores, schedules, and game information from ESPN's sports API. This server enables AI assistants to answer questions about game results, schedules, team performance, and sports outcomes across professional and college leagues.

## Features

- 🏀 **Live Scores & Results**: Get real-time scores and final results for games
- 📅 **Schedule Information**: View upcoming games and past game results
- 🏆 **Comprehensive League Coverage**: Support for 27+ sports leagues
- 📊 **Detailed Game Data**: Team stats, venue information, broadcast details, and more
- 🔧 **MCP Compatible**: Works with any MCP-compatible AI assistant

## ⚠️ Important Notice

**This project relies on ESPN's undocumented API.** The ESPN API used by this server is not officially documented or supported by ESPN. As such:

- The API may change without notice, potentially breaking functionality
- The API could become inaccessible or restricted at any time
- There are no guarantees of service availability or data accuracy
- Use this project at your own risk for non-commercial purposes

This is an unofficial tool built for educational and personal use.

## Prerequisites

- .NET 8.0 SDK or later
- MCP-compatible client (e.g., Claude Desktop, Cline, etc.)

## Installation

### 1. Clone and Build

```bash
git clone <repository-url>
cd sports-mcp
dotnet build
```

### 2. Configure Settings

Copy the example configuration and update with the ESPN API endpoint:

```bash
cp appsettings.example.json appsettings.json
```

Edit `appsettings.json`:

```json
{
  "SportsApi": {
    "BaseUrl": "http://site.api.espn.com/apis/site/v2"
  }
}
```

### 3. Configure Your MCP Client

Add the server to your MCP client configuration. For Claude Desktop, edit your `claude_desktop_config.json`:

**Windows**: `%APPDATA%\Claude\claude_desktop_config.json`
**macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`

```json
{
  "mcpServers": {
    "sports": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\path\\to\\sports-mcp\\sports-mcp.csproj"
      ]
    }
  }
}
```

Restart your MCP client after updating the configuration.

## Usage

Once configured, you can ask your AI assistant natural language questions about sports:

- "What were the NBA scores yesterday?"
- "Show me today's NFL games"
- "Who won the Lakers game last night?"
- "What's the schedule for Premier League this weekend?"
- "Give me the scores for all NBA games on January 25, 2026"

## Available Tools

### GetScoreboard

Retrieves sports scores, schedules, and game information.

**Parameters:**

- `sport` (required): The sport type (Football, Basketball, Soccer, Baseball, Hockey, Tennis, Golf, Racing)
- `league` (required): The specific league or competition
- `date` (optional): The date for results or schedule (ISO format: YYYY-MM-DD). Defaults to current date if omitted.

**Returns:**

- List of games/events with scores, team information, status, venue, and broadcast details
- League information
- Team records and statistics
- Game status (scheduled, in-progress, final)

## Supported Sports & Leagues

### 🏈 Football

- **NFL**: National Football League
- **CollegeFootball**: NCAA College Football

### 🏀 Basketball

- **NBA**: National Basketball Association
- **WNBA**: Women's National Basketball Association
- **MenCollegeBasketball**: NCAA Men's College Basketball
- **WomenCollegeBasketball**: NCAA Women's College Basketball

### ⚾ Baseball

- **MLB**: Major League Baseball
- **MenCollegeBaseball**: NCAA Men's College Baseball

### ⚽ Soccer

- **EPL**: English Premier League
- **MLS**: Major League Soccer
- **UEFAChampionsLeague**: UEFA Champions League
- **UEFAEuropaLeague**: UEFA Europa League

### 🏒 Hockey

- **NHL**: National Hockey League
- **MenCollegeHockey**: NCAA Men's College Hockey
- **WomenCollegeHockey**: NCAA Women's College Hockey

### 🎾 Tennis

- **ATP**: Association of Tennis Professionals
- **WTA**: Women's Tennis Association

### ⛳ Golf

- **PGA**: PGA Tour
- **LPGA**: LPGA Tour
- **EuropeanTour**: DP World Tour (European Tour)

### 🏎️ Racing

- **NASCAR**: NASCAR Cup Series
- **F1**: Formula 1

## Configuration

### Using appsettings.json

```json
{
  "SportsApi": {
    "BaseUrl": "http://site.api.espn.com/apis/site/v2"
  }
}
```

### Using Environment Variables

You can override the configuration using environment variables:

**Windows (PowerShell):**

```powershell
$env:SportsApi__BaseUrl="http://site.api.espn.com/apis/site/v2"
```

**Windows (Command Prompt):**

```cmd
set SportsApi__BaseUrl=http://site.api.espn.com/apis/site/v2
```

**Linux/macOS:**

```bash
export SportsApi__BaseUrl="http://site.api.espn.com/apis/site/v2"
```

## Development

### Building

```bash
dotnet build
```

### Running Locally

```bash
dotnet run
```

### Testing

The project includes a comprehensive unit test suite in the `sports-mcp.Tests` project. Tests are written using [xUnit](https://xunit.net/) and cover all major components of the server.

#### Running Tests

```bash
dotnet test
```

To run with detailed output:

```bash
dotnet test --verbosity normal
```

#### Test Coverage

The test suite covers:

- **Sports & League enums** (`Models/SportsExtensionsTests.cs`, `Models/LeaguesExtensionsTests.cs`): Verifies every `Sports` and `Leagues` enum value maps to the correct ESPN API path segment.
- **Scoreboard DTO parsing** (`Models/ScoreboardDtoTests.cs`): Validates JSON-to-DTO deserialization for all data classes including league info, game status, venue details, team competitors, game events, and full scoreboard responses. Edge cases such as missing optional fields, empty event lists, and international venues are tested.
- **HTTP client extension** (`Extensions/HttpClientExtTests.cs`): Tests successful JSON document retrieval, HTTP error propagation, and that request URIs are constructed correctly.
- **GetScoreboard tool** (`Tools/SportsScoreboardToolTests.cs`): End-to-end tests using a mock HTTP handler that verify the correct URL is built from sport/league/date parameters, that valid responses are deserialized and serialised as indented JSON, and that HTTP or deserialization errors produce a well-formed error JSON response.

#### Testing Approach

The server communicates over stdio using the MCP protocol, so its individual components are tested in isolation:

- **Static tool methods** (`SportsScoreboardTool.GetScoreboard`) accept `HttpClient` and `IOptions<SportsApiOptions>` as explicit parameters, making it straightforward to supply a test double without any DI container.
- **HTTP layer** is faked using a lightweight in-process `HttpMessageHandler` subclass. This avoids network calls and third-party mock libraries.
- **Configuration** is supplied via `Microsoft.Extensions.Options.Options.Create()`, keeping tests self-contained.
- **Internal types** (e.g., `HttpClientExt`) are exposed to the test project through `[InternalsVisibleTo("sports-mcp.Tests")]` in the main project.

#### Project Structure

```
sports-mcp.Tests/
├── Extensions/
│   └── HttpClientExtTests.cs   # HTTP client extension tests
├── Models/
│   ├── LeaguesExtensionsTests.cs  # League enum mapping tests
│   ├── ScoreboardDtoTests.cs      # DTO JSON parsing tests
│   └── SportsExtensionsTests.cs   # Sport enum mapping tests
└── Tools/
    └── SportsScoreboardToolTests.cs  # GetScoreboard tool tests
```

### Project Structure

```
sports-mcp/
├── Extensions/          # HTTP client extensions
├── Models/             # Data models and enums
│   ├── Sports.cs       # Sport type enums
│   ├── Leagues.cs      # League enums
│   └── ScoreboardDto.cs # Response DTOs
├── Options/            # Configuration options
├── Tools/              # MCP tool implementations
│   └── SportsScoreboardTool.cs
├── Program.cs          # Application entry point
└── appsettings.json    # Configuration file
```

## Response Format

The server returns structured JSON data including:

```json
{
  "Events": [
    {
      "Id": "401810505",
      "Name": "Sacramento Kings at Detroit Pistons",
      "ShortName": "SAC @ DET",
      "Date": "2026-01-25T20:00:00Z",
      "Status": {
        "Clock": 0,
        "DisplayClock": "0:00",
        "Period": 4,
        "Type": {
          "State": "post",
          "Completed": true,
          "Detail": "Final"
        }
      },
      "Competitors": [
        {
          "Team": {
            "DisplayName": "Detroit Pistons",
            "Abbreviation": "DET",
            "Logo": "https://..."
          },
          "Score": "139",
          "Winner": true,
          "HomeAway": "home",
          "Records": [...]
        }
      ],
      "Venue": {
        "FullName": "Little Caesars Arena",
        "Address": {
          "City": "Detroit",
          "State": "MI"
        }
      }
    }
  ],
  "League": {
    "Name": "National Basketball Association",
    "Abbreviation": "NBA"
  }
}
```

## Troubleshooting

### "Failed to parse scoreboard data" Error

This error can occur if the API response structure changes or contains unexpected data. The most recent fix handles cases where scheduled games don't have a `winner` property yet.

### Server Not Appearing in MCP Client

1. Verify the path in your MCP client configuration is correct
2. Ensure .NET 8.0 SDK is installed and in your PATH
3. Check that `appsettings.json` exists and is properly configured
4. Restart your MCP client after configuration changes
5. Check client logs for startup errors

### API Timeout or Connection Issues

- Verify internet connectivity
- Check if ESPN API endpoint is accessible: `http://site.api.espn.com/apis/site/v2`
- Ensure no firewall is blocking the connection

## Data Source

This server retrieves data from ESPN's public sports API endpoints. The data is provided as-is from ESPN's services.

## License

This project is licensed under the [MIT License](LICENSE).

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## Acknowledgments

- Built using the [Model Context Protocol](https://modelcontextprotocol.io)
- Sports data provided by [ESPN API](https://www.espn.com)
