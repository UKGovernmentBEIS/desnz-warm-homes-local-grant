using System;
using GovUkDesignSystem.ModelBinders;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using HerPublicWebsite.BusinessLogic.ExternalServices.S3FileWriter;
using HerPublicWebsite.BusinessLogic.Services.EligiblePostcode;
using HerPublicWebsite.BusinessLogic.Services.QuestionFlow;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;
using HerPublicWebsite.BusinessLogic.Services.S3ReferralFileKeyGenerator;
using HerPublicWebsite.Data;
using HerPublicWebsite.ErrorHandling;
using HerPublicWebsite.ExternalServices.EmailSending;
using HerPublicWebsite.ExternalServices.GoogleAnalytics;
using HerPublicWebsite.Middleware;
using HerPublicWebsite.Services;
using HerPublicWebsite.Services.Cookies;
using HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;
using HerPublicWebsite.BusinessLogic.Services.CsvFileCreator;

namespace HerPublicWebsite
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment webHostEnvironment;
        
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            this.configuration = configuration;
            this.webHostEnvironment = webHostEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Hangfire services.
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(configuration.GetConnectionString("PostgreSQLConnection")));

            // Add the Hangfire processing server as IHostedService
            services.AddHangfireServer();

            services.AddScoped<CsvFileCreator>();
            services.AddScoped<IDataAccessProvider, DataAccessProvider>();
            services.AddScoped<EligiblePostcodeService>();
            services.AddScoped<QuestionnaireService>();
            services.AddScoped<QuestionnaireUpdater>();
            services.AddScoped<IQuestionFlowService, QuestionFlowService>();
            services.AddScoped<IRegularJobsService, RegularJobsService>();
            services.AddScoped<IS3FileWriter, S3FileWriter>();
            services.AddScoped<S3ReferralFileKeyGenerator>();
            
            services.AddMemoryCache();
            services.AddSingleton<StaticAssetsVersioningService>();
            // This allows encrypted cookies to be understood across multiple web server instances
            services.AddDataProtection().PersistKeysToDbContext<HerDbContext>();

            ConfigureEpcApi(services);
            ConfigureOsPlacesApi(services);
            ConfigureGovUkNotify(services);
            ConfigureCookieService(services);
            ConfigureDatabaseContext(services);
            ConfigureGoogleAnalyticsService(services);

            if (!webHostEnvironment.IsProduction())
            {
                services.Configure<BasicAuthMiddlewareConfiguration>(
                    configuration.GetSection(BasicAuthMiddlewareConfiguration.ConfigSection));
            }

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<ErrorHandlingFilter>();
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.ModelMetadataDetailsProviders.Add(new GovUkDataBindingErrorTextProvider());
            });

            // TODO: Replace this with a database based cache
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddHttpContextAccessor();
        }

        private void ConfigureGoogleAnalyticsService(IServiceCollection services)
        {
            services.Configure<GoogleAnalyticsConfiguration>(
                configuration.GetSection(GoogleAnalyticsConfiguration.ConfigSection));
            services.AddScoped<GoogleAnalyticsService, GoogleAnalyticsService>();
        }

        private void ConfigureDatabaseContext(IServiceCollection services)
        {
            var databaseConnectionString = configuration.GetConnectionString("PostgreSQLConnection");
            if (!webHostEnvironment.IsDevelopment())
            {
                databaseConnectionString = "TODO Get Azure DB string";
            }
            services.AddDbContext<HerDbContext>(opt =>
                opt.UseNpgsql(databaseConnectionString));
        }

        private void ConfigureCookieService(IServiceCollection services)
        {
            services.Configure<CookieServiceConfiguration>(
                configuration.GetSection(CookieServiceConfiguration.ConfigSection));
            // Change the default antiforgery cookie name so it doesn't include Asp.Net for security reasons
            services.AddAntiforgery(options => options.Cookie.Name = "Antiforgery");
            services.AddScoped<CookieService, CookieService>();
        }

        private void ConfigureEpcApi(IServiceCollection services)
        {
            services.Configure<EpbEpcConfiguration>(
                configuration.GetSection(EpbEpcConfiguration.ConfigSection));
            services.AddScoped<IEpcApi, DummyEpbEpcApi>();
        }

        private void ConfigureOsPlacesApi(IServiceCollection services)
        {
            //TODO BEISHER-248: Set up OS Places properly
            services.AddScoped<IOsPlacesApi, OsPlacesApi>();
        }

        private void ConfigureGovUkNotify(IServiceCollection services)
        {
            services.AddScoped<IEmailSender, GovUkNotifyApi>();
            services.Configure<GovUkNotifyConfiguration>(
                configuration.GetSection(GovUkNotifyConfiguration.ConfigSection));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!webHostEnvironment.IsDevelopment())
            {
                app.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandlingPath = "/error"
                });
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            ConfigureHttpBasicAuth(app);

            app.UseMiddleware<SecurityHeadersMiddleware>();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureHttpBasicAuth(IApplicationBuilder app)
        {
            if (!webHostEnvironment.IsProduction())
            {
                // Add HTTP Basic Authentication in our non-production environments to make sure people don't accidentally stumble across the site
                app.UseMiddleware<BasicAuthMiddleware>();
            }
        }
    }
}
