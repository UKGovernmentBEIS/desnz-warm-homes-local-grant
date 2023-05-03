using Hangfire;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

            // Migrate the database if it's out of date
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<HerDbContext>();
            dbContext.Database.Migrate();

            // Run nightly tasks at 00:30 UTC daily
            app
                .Services
                .GetService<IRecurringJobManager>()
                .AddOrUpdate<RegularJobsService>(
                    "Nightly tasks",
                    rjs => rjs.RunNightlyTasksAsync(),
                    "30 0 * * *");
            
            app.Run();
        }
    }
}