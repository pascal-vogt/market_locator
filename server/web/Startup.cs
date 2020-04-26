using Service;

namespace Web
{
    using System.Linq;
    using Authorization;
    using Database;
    using Database.Entities;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            services.Configure<AppConfig>(this.Configuration.GetSection("AppConfig"));
            services.AddScoped<DatabaseContext>();
            services.AddScoped<SessionService>();
            services.AddScoped<StandService>();
            services.AddScoped<GoogleDocImportService>();

            services.AddAuthorization(options =>
            {
                var connectionString = this.Configuration.GetSection("AppConfig").Get<AppConfig>().ConnectionString;
                using var context = new DatabaseContext(connectionString);
                foreach(var action in context.Action) 
                {
                    options.AddPolicy(action.Code, policy => policy.Requirements.Add(new CanPerformActionRequirement(action.Code)));
                }
            });
            services.AddScoped<IAuthorizationHandler, CanPerformActionHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}