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
