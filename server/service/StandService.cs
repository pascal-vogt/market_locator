namespace Service
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Database;
    using Database.Entities;
    using Microsoft.EntityFrameworkCore;

    public class StandService
    {
        private DatabaseContext DatabaseContext { get; set; }
        
        public StandService(DatabaseContext databaseContext)
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
    }
}