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
        private DatabaseContext DatabaseContext { get; set; }
        
        public StandService(
            DatabaseContext databaseContext
        )
        {
            this.DatabaseContext = databaseContext;
        }
        
        public async Task<IEnumerable<Stand>> GetAll()
        {
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