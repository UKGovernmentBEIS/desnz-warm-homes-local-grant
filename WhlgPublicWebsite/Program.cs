using System.Globalization;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WhlgPublicWebsite.BusinessLogic.Services.PolicyTeamUpdate;
using WhlgPublicWebsite.BusinessLogic.Services.RegularJobs;
using WhlgPublicWebsite.Data;

namespace WhlgPublicWebsite
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
            
            CultureInfo.CurrentCulture = new CultureInfo("en-GB", false);

            var app = builder.Build();

            startup.Configure(app, app.Environment);

            // Migrate the database if it's out of date. Ideally we wouldn't do this on app startup for our deployed
            // environments, because we're risking multiple containers attempting to run the migrations concurrently and
            // getting into a mess. However, we very rarely add migrations at this point, so in practice it's easier to
            // risk it and keep an eye on the deployment: we should be doing rolling deployments anyway which makes it
            // very unlikely we run into concurrency issues. If that changes though we should look at moving migrations
            // to a deployment pipeline step, and only doing the following locally (PC-1151).
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WhlgDbContext>();
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