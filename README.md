# Sports MCP Server

A Model Context Protocol (MCP) server for retrieving sports scoreboard data.

## Prerequisites

- .NET 8.0 SDK or later

## Configuration

The application requires the Sports API base URL to be configured in `appsettings.json`:

```json
{
  "SportsApi": {
    "BaseUrl": "http://site.api.espn.com/apis/site/v2"
  }
}
```

### Environment Variables (Optional)

You can override the base URL using environment variables:

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

## Building and Running

1. Clone the repository
2. Build the project:
   ```bash
   dotnet build
   ```
3. Run the server:
   ```bash
   dotnet run
   ```

## Configuration Validation

The application validates that all required configuration values are present at startup. If the `BaseUrl` is missing or invalid, the application will fail to start with a clear error message.

## Tools Available

- **GetScoreboard**: Get sports scoreboard data based on sport, league, and optional date.
