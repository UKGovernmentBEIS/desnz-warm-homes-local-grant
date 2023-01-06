using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HerPublicWebsite.Data;

namespace HerPublicWebsite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Hide that we are using Kestrel for security reasons
            builder.WebHost.ConfigureKestrel(serverOptions => serverOptions.AddServerHeader = false);
            
            var startup = new Startup(builder.Configuration, builder.Environment);
            startup.ConfigureServices(builder.Services);

            var app = builder.Build();

            startup.Configure(app, app.Environment);
            
            // Migrate the database for local dev and for instance 0 on GOV.PaaS.
            // As we use rolling deployments there shouldn't be any chance of multiple instances of this running at the
            // same time anyway, but it's easy to check the instance index for extra safety.
            if (app.Environment.IsDevelopment() || app.Configuration["CF_INSTANCE_INDEX"] == "0")
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<HerDbContext>();
                dbContext.Database.Migrate();
            }

            app.Run();
        }
    }
}