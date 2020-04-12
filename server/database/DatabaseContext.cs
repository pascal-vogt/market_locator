namespace Database
{
    using Database.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class DatabaseContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<RoleAssignment> RoleAssignment { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Stand> Stand { get; set; }

        public DatabaseContext(IOptionsMonitor<AppConfig> appConfig)
        {
            this.AppConfig = appConfig;
        }

        public IOptionsMonitor<AppConfig> AppConfig { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(this.AppConfig.CurrentValue.ConnectionString);
        }
    }
}