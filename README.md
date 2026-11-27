# GolfStatistics

A Windows desktop app for collecting golf data from Google Drive and presenting statistics. The app is built with C#/.NET 11 and WinUI 3.

## Requirements

- Windows 10 version 2004 (build 19041) or later
- .NET 11 SDK
- Visual Studio Code with the recommended C# Dev Kit extension

## Build and run in VS Code

Open this repository folder in VS Code. Restore and build with **Terminal > Run Build Task** (`Ctrl+Shift+B`), or run:

```powershell
dotnet build .\GolfStatistics.csproj
```

Press **F5** to build and launch the app with the debugger. The project is unpackaged and restores the Windows App SDK through NuGet.

## Current functionality

**Current status:** The app is a proof of concept. It currently reads one cell from a Google Sheets workbook; broader golf-data collection and statistics are not implemented yet.

On launch, the app displays the locally cached value of `SheetInfo!B1` from the 2016 workbook, if one is available. Select **Retrieve from Google** to sign in and fetch the latest value. Access is read-only and limited to Google Sheets. An empty or unavailable cell is reported as a retrieval failure.

The OAuth token, cached cell value, and remembered client-secrets path are stored under `%LOCALAPPDATA%\GolfStatistics` (including the `oauth` subdirectory for the token). The cached value can be displayed without signing in or network access.

## Google Sheets setup

To retrieve the cell:

1. Enable the Google Sheets API in a Google Cloud project.
2. Create an OAuth client ID for a Desktop app and download its client-secrets JSON file.
3. Click **Retrieve from Google** and select that JSON file when prompted. Keep the file outside the repository.
4. Ensure the Google account used at sign-in has access to the workbook. The app remembers the selected file path locally; alternatively, set `GOOGLE_OAUTH_CLIENT_SECRETS` to override it.

The first retrieval opens Google sign-in. Later retrievals reuse the saved token unless it is no longer valid.
