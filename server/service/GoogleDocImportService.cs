namespace Service
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Database;
    using Database.Entities;
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Services;
    using Google.Apis.Sheets.v4;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class GoogleDocImportService
    {
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string ApplicationName = "MarketLocator";
        private readonly string SheetName = "Marktstände";
        private readonly List<Stand> StandsFromGoogleSheet;
        
        private DatabaseContext DatabaseContext { get; set; }
        private IOptionsMonitor<AppConfig> AppConfig { get; set; }
        
        public GoogleDocImportService(
            DatabaseContext databaseContext,
            IOptionsMonitor<AppConfig> appConfig
        ) {
            this.DatabaseContext = databaseContext;
            this.AppConfig = appConfig;
        }

        private async Task SyncIfNeeded()
        {
            if (await this.IsSyncNeeded())
            {
                await this.Import();
            }
        }

        private async Task<bool> IsSyncNeeded()
        {
            // maybe we can use a timestamp later, for now we just populate it once and stop caring about it later
            return !(await this.DatabaseContext.GoogleDocImportedRow.AnyAsync());
        }

        private async Task Import() 
        {
            GoogleCredential credential;
            await using (var stream = new FileStream("google_api_secret.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }
            
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            
            var range = $"{SheetName}!A:G";
            var request = service.Spreadsheets.Values.Get(this.AppConfig.CurrentValue.SpreadsheetId, range);
            var response = request.Execute();
            var values = response.Values;

            foreach (var row in values)
            {
                await this.DatabaseContext.GoogleDocImportedRow.AddAsync(new GoogleDocImportedRow
                {
                    Col0 = row.Count > 0 ? row[0] as string : "",
                    Col1 = row.Count > 1 ? row[1] as string : "",
                    Col2 = row.Count > 2 ? row[2] as string : "",
                    Col3 = row.Count > 3 ? row[3] as string : "",
                    Col4 = row.Count > 4 ? row[4] as string : "",
                    Col5 = row.Count > 5 ? row[5] as string : "",
                    Col6 = row.Count > 6 ? row[6] as string : ""
                });
            }

            await this.DatabaseContext.SaveChangesAsync();
        }

        public async Task<List<Stand>> GetStands()
        {
            await this.SyncIfNeeded();
            var stands = new List<Stand>();
            foreach (var row in this.DatabaseContext.GoogleDocImportedRow.AsQueryable())
            {
                var name = row.Col0;
                var oeffentlichPrivat = row.Col1;
                var wochentage = row.Col2;
                if (wochentage == null)
                {
                    wochentage = "";
                }
                var strasseLatLong = row.Col3;
                var tel = row.Col4;
                var typ = row.Col5;
                var bildUrl = row.Col6;
                double? latitude = null;
                double? longitude = null;
                if (Regex.IsMatch(strasseLatLong, @"^\s*[-+]?([1-8]?\d(\.\d+)?|90(\.0+)?),\s*[-+]?(180(\.0+)?|((1[0-7]\d)|([1-9]?\d))(\.\d+)?)\s*$"))
                {
                    var coords = strasseLatLong.Split(",");
                    latitude = double.Parse(coords[0].Trim(), CultureInfo.InvariantCulture);
                    longitude = double.Parse(coords[1].Trim(), CultureInfo.InvariantCulture);
                }

                if (latitude.HasValue && longitude.HasValue)
                {
                    stands.Add(new Stand
                    {
                        Phone = tel,
                        Title = name,
                        OpenMo = wochentage.Contains("Montag"),
                        OpenTu = wochentage.Contains("Dienstag"),
                        OpenWe = wochentage.Contains("Mittwoch"),
                        OpenTh = wochentage.Contains("Donnerstag"),
                        OpenFr = wochentage.Contains("Freitag"),
                        OpenSa = wochentage.Contains("Samstag"),
                        OpenSu = wochentage.Contains("Sontag"),
                        Latitude = latitude.Value,
                        Longitude = longitude.Value
                    });   
                }
            }

            return stands;
        }
    }
}