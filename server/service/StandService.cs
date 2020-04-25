namespace Service
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Database;
    using Database.Entities;
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Services;
    using Google.Apis.Sheets.v4;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class StandService
    {
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string ApplicationName = "MarketLocator";
        private readonly string SheetName = "Marktstände";
        private readonly List<Stand> StandsFromGoogleSheet;
        
        private DatabaseContext DatabaseContext { get; set; }
        
        public StandService(
            DatabaseContext databaseContext, IOptionsMonitor<AppConfig> appConfig
        )
        {
            this.DatabaseContext = databaseContext;
            
            GoogleCredential credential;
            using (var stream = new FileStream("google_api_secret.json", FileMode.Open, FileAccess.Read))
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
            var request = service.Spreadsheets.Values.Get(appConfig.CurrentValue.SpreadsheetId, range);
            var response = request.Execute();
            var values = response.Values;
            var stands = new List<Stand>();
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    var name = row[0] as string;
                    var oeffentlichPrivat = row[1] as string;
                    var wochentage = row[2] as string;
                    if (wochentage == null)
                    {
                        wochentage = "";
                    }
                    var strasseLatLong = row[3] as string;
                    var tel = row[4] as string;
                    var typ = row[5] as string;
                    var bildUrl = row[6] as string;
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
            }

            this.StandsFromGoogleSheet = stands;
        }
        
        public async Task<IEnumerable<Stand>> GetAll()
        {
            if (this.StandsFromGoogleSheet != null && this.StandsFromGoogleSheet.Count > 0)
            {
                return this.StandsFromGoogleSheet;
            }
            
            return await this.DatabaseContext.Stand
                .AsQueryable()
                .OrderBy(o => o.Title)
                .ToListAsync();
        }

        public async Task<Stand> GetById(long standId)
        {
            return await this.DatabaseContext.Stand
                .AsQueryable()
                .FirstOrDefaultAsync(o => o.Id == standId);
        }
        
        public async Task<Stand> GetByIdForUpdate(long standId)
        {
            return await this.DatabaseContext.Stand
                .AsTracking()
                .FirstOrDefaultAsync(o => o.Id == standId);
        }

        public async Task<Stand> Update(Stand standFromDb, Stand stand)
        {
            standFromDb.Email = stand.Email;
            standFromDb.Homepage = stand.Homepage;
            standFromDb.Latitude = stand.Latitude;
            standFromDb.Longitude = stand.Longitude;
            standFromDb.Phone = stand.Phone;
            standFromDb.Summary = stand.Summary;
            standFromDb.Title = stand.Title;
            standFromDb.OpenMo = stand.OpenMo;
            standFromDb.OpenTu = stand.OpenTu;
            standFromDb.OpenWe = stand.OpenWe;
            standFromDb.OpenTh = stand.OpenTh;
            standFromDb.OpenFr = stand.OpenFr;
            standFromDb.OpenSa = stand.OpenSa;
            standFromDb.OpenSu = stand.OpenSu;

            await this.DatabaseContext.SaveChangesAsync();

            return standFromDb;
        }

        public async Task<Stand> Create(Stand stand)
        {
            stand.Id = null;
            await this.DatabaseContext.Stand.AddAsync(stand);

            return stand;
        }
    }
}