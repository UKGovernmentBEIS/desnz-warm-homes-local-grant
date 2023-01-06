using System;
using System.Text.RegularExpressions;
using GovUkDesignSystem.ModelBinders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.ExternalServices.Bre;
using HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using HerPublicWebsite.BusinessLogic.Services;
using HerPublicWebsite.Data;
using HerPublicWebsite.DataStores;
using HerPublicWebsite.ErrorHandling;
using HerPublicWebsite.ExternalServices.EmailSending;
using HerPublicWebsite.ExternalServices.GoogleAnalytics;
using HerPublicWebsite.ExternalServices.PostcodesIo;
using HerPublicWebsite.Middleware;
using HerPublicWebsite.Services;
using HerPublicWebsite.Services.Cookies;

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
            services.AddScoped<AnswerService>();
            services.AddScoped<PropertyDataStore>();
            services.AddScoped<PropertyDataUpdater>();
            services.AddScoped<IQuestionFlowService, QuestionFlowService>();
            services.AddScoped<PostcodesIoApi>();
            services.AddMemoryCache();
            services.AddSingleton<StaticAssetsVersioningService>();
            services.AddScoped<RecommendationService>();
            services.AddScoped<IDataAccessProvider, DataAccessProvider>();
            services.AddDataProtection().PersistKeysToDbContext<HerDbContext>();

            ConfigureEpcApi(services);
            ConfigureBreApi(services);
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
                // In Gov.PaaS the Database URL is automatically put into the DATABASE_URL environment variable
                databaseConnectionString = DatabaseUrlToConnectionString(configuration["DATABASE_URL"]);
            }
            services.AddDbContext<HerDbContext>(opt =>
                opt.UseNpgsql(databaseConnectionString));
        }

        private string DatabaseUrlToConnectionString(string url)
        {
            // The DATABASE_URL environment variable has the following form
            // postgres://<username>:<password>@<host>:<port>/<name>
            var urlPattern = new Regex(@"postgres\:\/\/(.*)\:(.*)@(.*)\:(\d+)\/(.*)");
            var match = urlPattern.Match(url);
            if (!match.Success)
            {
                throw new Exception("Database URL was not in the expected format, cannot connect to database");
            }

            var username = match.Groups[1].Captures[0].Value;
            var password = match.Groups[2].Captures[0].Value;
            var host = match.Groups[3].Captures[0].Value;
            var port = match.Groups[4].Captures[0].Value;
            var name = match.Groups[5].Captures[0].Value;

            return $"Host={host};Port={port};Username={username};Password={password};Database={name}";
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
            services.AddScoped<IEpcApi, EpbEpcApi>();
        }

        private void ConfigureBreApi(IServiceCollection services)
        {
            services.Configure<BreConfiguration>(
                configuration.GetSection(BreConfiguration.ConfigSection));
            services.AddScoped<BreApi>();
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
