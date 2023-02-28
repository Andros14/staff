using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StaffDaemon.Services.Slack;
using System.Linq;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource.AppendRequest;

namespace StaffDaemon.Services.GoogleSheets
{
    public class GoogleSheetsService : IGoogleSheetsService
    {
        public GoogleSheetsService(IConfiguration configuration)
        {
            var section = configuration.GetSection("GoogleClientSertPath");
            var secretFilePath = section.Value;

            if (!File.Exists(secretFilePath))
            {
                throw new FileNotFoundException(secretFilePath);
            }

            var secretJson = File.ReadAllText(secretFilePath);
            var credential = GoogleCredential.FromJson(secretJson);
            credential = credential.CreateScoped(new[] { SheetsService.Scope.Spreadsheets });

            sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
            });
        }

        private readonly SheetsService sheetsService;

        public async Task<RangeData> ReadDataAsync(string spreadsheetId, string rangeName)
        {
            var request = sheetsService.Spreadsheets.Values.Get(spreadsheetId, rangeName);
            var response = await request.ExecuteAsync();

            return new RangeData(response.Values);
        }

        public async Task<string> AppendDataAsync(IList<IList<object>> data, string spreadsheetId, string rangeName)
        {
            var valueRange = new ValueRange()
            {
                Values = data
            };
            var request = sheetsService.Spreadsheets.Values.Append(valueRange, spreadsheetId, rangeName);
            request.ValueInputOption = ValueInputOptionEnum.USERENTERED;
            var response = await request.ExecuteAsync();
            return response.Updates.UpdatedRange;
        }
    }
}
