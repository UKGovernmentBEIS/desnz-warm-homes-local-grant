using System.Globalization;
using System.Threading;
using Hangfire;
using HerPublicWebsite.BusinessLogic.Services.PolicyTeamUpdate;
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
            
            const string culture = "en-GB";
            CultureInfo.CurrentCulture = new CultureInfo(culture, false);
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = new CultureInfo(culture, false);

            var app = builder.Build();

            startup.Configure(app, app.Environment);

            // Migrate the database if it's out of date
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<HerDbContext>();
            dbContext.Database.Migrate();

            var recurringJobManager = app.Services.GetService<IRecurringJobManager>();

            // Remove deprecated nightly tasks service
            recurringJobManager.RemoveIfExists("Nightly tasks");
            
            recurringJobManager.AddOrUpdate<ReferralFollowUpNotificationService>(
                "Get referrals passed ten day working threshold with no follow up",
                service => service.SendReferralFollowUpNotifications(),
                "30 0 * * *"); // at 00:30 every day
            
            recurringJobManager.AddOrUpdate<UnsubmittedReferralRequestsService>(
                "Write unsubmitted referral requests to csv",
                service => service.WriteUnsubmittedReferralRequestsToCsv(),
                "45 0 * * *"); // at 00:45 every day
            
            recurringJobManager.AddOrUpdate<PolicyTeamUpdateService>(
                "Send policy team update email",
                service => service.SendPolicyTeamUpdate(),
                "0 7 * * 1"); // at 07:00 on Monday
                
            recurringJobManager.AddOrUpdate<PendingReferralNotificationService>(
                "Send monthly pending referral report",
                service => service.SendPendingReferralNotifications(),
                "15 7 1 * *"); // at 07:15 on 1st of the month

            app.Run();
        }
    }
}