using System.Globalization;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;

namespace GolfStatistics;

internal sealed class SpreadsheetCellReader
{
    private const string ApplicationName = "GolfStatistics";
    private const string SpreadsheetId = "1Gk6J7C3EnG1hLkLVVJ_JMbPAJObPUfXPfNfbvb1J6Kg";
    private const string Range = "SheetInfo!B1";

    private readonly string _localDataDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        ApplicationName);

    private string CachePath => Path.Combine(_localDataDirectory, "2016-sheetinfo-b1.json");
    private string ClientSecretsPathFile => Path.Combine(_localDataDirectory, "oauth-client-secrets-path.txt");

    public string? GetClientSecretsPath()
    {
        var environmentPath = Environment.GetEnvironmentVariable("GOOGLE_OAUTH_CLIENT_SECRETS");
        if (!string.IsNullOrWhiteSpace(environmentPath) && File.Exists(environmentPath))
        {
            return environmentPath;
        }

        if (!File.Exists(ClientSecretsPathFile))
        {
            return null;
        }

        var savedPath = File.ReadAllText(ClientSecretsPathFile).Trim();
        return File.Exists(savedPath) ? savedPath : null;
    }

    public void SaveClientSecretsPath(string path)
    {
        Directory.CreateDirectory(_localDataDirectory);
        File.WriteAllText(ClientSecretsPathFile, path);
    }

    public string? ReadCachedValue()
    {
        if (!File.Exists(CachePath))
        {
            return null;
        }

        var cachedCell = JsonSerializer.Deserialize<CachedCell>(File.ReadAllText(CachePath))
            ?? throw new InvalidDataException($"The cached value at '{CachePath}' is invalid.");

        return cachedCell.Value;
    }

    public async Task<string> RetrieveAsync(string clientSecretsPath)
    {
        if (!File.Exists(clientSecretsPath))
        {
            throw new FileNotFoundException("The selected Google OAuth client-secrets file could not be found.", clientSecretsPath);
        }

        await using var secretsStream = File.OpenRead(clientSecretsPath);
        var clientSecrets = GoogleClientSecrets.FromStream(secretsStream).Secrets;
        SaveClientSecretsPath(clientSecretsPath);
        var tokenDirectory = Path.Combine(_localDataDirectory, "oauth");
        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            clientSecrets,
            new[] { SheetsService.Scope.SpreadsheetsReadonly },
            "user",
            CancellationToken.None,
            new FileDataStore(tokenDirectory, true));

        using var sheetsService = new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName
        });

        var response = await sheetsService.Spreadsheets.Values.Get(SpreadsheetId, Range).ExecuteAsync();
        var cellValue = response.Values?.FirstOrDefault()?.FirstOrDefault();
        if (cellValue is null)
        {
            throw new InvalidOperationException($"The cell {Range} in the workbook is empty or unavailable.");
        }

        var formattedValue = Convert.ToString(cellValue, CultureInfo.InvariantCulture)
            ?? throw new InvalidOperationException($"The cell {Range} did not contain a readable value.");
        var result = new CachedCell(SpreadsheetId, Range, formattedValue, DateTimeOffset.UtcNow);

        Directory.CreateDirectory(_localDataDirectory);
        await File.WriteAllTextAsync(CachePath, JsonSerializer.Serialize(result));
        return formattedValue;
    }

    private sealed record CachedCell(string SpreadsheetId, string Range, string Value, DateTimeOffset RetrievedAt);
}
