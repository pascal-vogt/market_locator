namespace Database
{
    using Database.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class DatabaseContext : DbContext
    {
        public DbSet<Action> Action { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<RoleAssignment> RoleAssignment { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Stand> Stand { get; set; }
        public DbSet<AllowedGlobalActions> AllowedGlobalActions { get; set; }
        public DbSet<AllowedStandSpecificActions> AllowedStandSpecificActions { get; set; }
        public DbSet<GoogleDocImportedRow> GoogleDocImportedRow { get; set; }

        public DatabaseContext(IOptionsMonitor<AppConfig> appConfig)
        {
            this.ConnectionString = appConfig.CurrentValue.ConnectionString;
        }
        
        public DatabaseContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(this.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
        }
    }
}