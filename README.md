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

## Single-cell Google Sheets proof of concept

The app displays the cached value from `SheetInfo!B1` in the 2016 workbook. Click **Retrieve from Google** to retrieve and cache a fresh value using read-only Google Sheets access.

To enable retrieval:

1. Enable the Google Sheets API in a Google Cloud project.
2. Create an OAuth client ID for a Desktop app and download its client-secrets JSON file.
3. Click **Retrieve from Google** and select that JSON file when prompted. Keep the file outside the repository.
4. Ensure the Google account used at sign-in has access to the workbook. The app remembers the selected file path locally; alternatively, set `GOOGLE_OAUTH_CLIENT_SECRETS` to override it.

The OAuth token and cached value are stored under the current user's local application data directory. Displaying the cached value does not require sign-in or network access. The first retrieval opens Google sign-in; later retrievals reuse the saved token unless it is no longer valid.
