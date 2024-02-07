using System;
using System.IO;
using Amazon;
using Amazon.S3;
using Community.Microsoft.Extensions.Caching.PostgreSql;
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
using Microsoft.AspNetCore.HttpOverrides;
using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using HerPublicWebsite.BusinessLogic.ExternalServices.S3FileWriter;
using HerPublicWebsite.BusinessLogic.Services.EligiblePostcode;
using HerPublicWebsite.BusinessLogic.Services.QuestionFlow;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;
using HerPublicWebsite.BusinessLogic.Services.S3ReferralFileKeyGenerator;
using HerPublicWebsite.Data;
using HerPublicWebsite.ErrorHandling;
using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using HerPublicWebsite.ExternalServices.GoogleAnalytics;
using HerPublicWebsite.Middleware;
using HerPublicWebsite.Services;
using HerPublicWebsite.Services.Cookies;
using HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;
using HerPublicWebsite.BusinessLogic.Services.CsvFileCreator;
using Microsoft.AspNetCore.Http;
using HerPublicWebsite.BusinessLogic.Services.ReferralFollowUps;
using GlobalConfiguration = HerPublicWebsite.BusinessLogic.GlobalConfiguration;

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
            services.AddScoped<IEligiblePostcodeService, EligiblePostcodeService>();
            services.AddScoped<IReferralFollowUpService, ReferralFollowUpService>();
            services.AddScoped<IGuidService, GuidService>();
            services.AddScoped<QuestionnaireService>();
            services.AddScoped<QuestionnaireUpdater>();
            services.AddScoped<IQuestionFlowService, QuestionFlowService>();
            services.AddScoped<IUnsubmittedReferralRequestsService, UnsubmittedReferralRequestsService>();
            services.AddScoped<IWorkingDayHelperService, WorkingDayHelperService>();

            services.AddMemoryCache();
            services.AddSingleton<StaticAssetsVersioningService>();
            // This allows encrypted cookies to be understood across multiple web server instances
            services.AddDataProtection().PersistKeysToDbContext<HerDbContext>();

            ConfigureReferralFollowUpNotificationService(services);
            ConfigureS3Client(services);
            ConfigureS3FileWriter(services);
            ConfigureEpcApi(services);
            ConfigureOsPlacesApi(services);
            ConfigureGovUkNotify(services);
            ConfigureCookieService(services);
            ConfigureDatabaseContext(services);
            ConfigureGoogleAnalyticsService(services);

            if (!webHostEnvironment.IsDevelopment() && !webHostEnvironment.IsProduction())
            {
                services.Configure<BasicAuthMiddlewareConfiguration>(
                    configuration.GetSection(BasicAuthMiddlewareConfiguration.ConfigSection));
            }

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<ErrorHandlingFilter>();
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.ModelMetadataDetailsProviders.Add(new GovUkDataBindingErrorTextProvider());
            })
                .AddSessionStateTempDataProvider();

            services.AddDistributedPostgreSqlCache(setup =>
            {
                setup.ConnectionString = configuration.GetConnectionString("PostgreSQLConnection");
                setup.TableName = configuration.GetSection("SessionCache")["TableName"];
                setup.SchemaName = configuration.GetSection("SessionCache")["SchemaName"];
            });

            services.AddSession(options =>
            {
                // If this changes, make sure to update the message on the session expiry page
                options.IdleTimeout = TimeSpan.FromHours(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services.AddHsts(options =>
            {
                // Recommendation for MaxAge is at least one year, and a maximum of 2 years
                // If Preload is enabled, IncludeSubdomains should be set to true, and MaxAge should be set to 2 years
                options.MaxAge = TimeSpan.FromDays(365);
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
            services.AddDbContext<HerDbContext>(opt =>
                opt.UseNpgsql(databaseConnectionString));
        }

        private void ConfigureCookieService(IServiceCollection services)
        {
            services.Configure<CookieServiceConfiguration>(
                configuration.GetSection(CookieServiceConfiguration.ConfigSection));
            
            // Change the default antiforgery cookie name so it doesn't include Asp.Net for security reasons
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = "Antiforgery";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
            services.AddScoped<CookieService, CookieService>();
        }

        private void ConfigureEpcApi(IServiceCollection services)
        {
            services.Configure<EpbEpcConfiguration>(
                configuration.GetSection(EpbEpcConfiguration.ConfigSection));
            services.AddScoped<IEpcApi, EpbEpcApi>();
        }

        private void ConfigureOsPlacesApi(IServiceCollection services)
        {
            services.Configure<OsPlacesConfiguration>(
                configuration.GetSection(OsPlacesConfiguration.ConfigSection));
            services.AddScoped<IOsPlacesApi, OsPlacesApi>();
        }

        private void ConfigureGovUkNotify(IServiceCollection services)
        {
            services.AddScoped<IEmailSender, GovUkNotifyApi>();
            services.Configure<GovUkNotifyConfiguration>(
                configuration.GetSection(GovUkNotifyConfiguration.ConfigSection));
        }

        private void ConfigureS3Client(IServiceCollection services)
        {
            var s3Config = new S3Configuration();
            configuration.GetSection(S3Configuration.ConfigSection).Bind(s3Config);

            if (webHostEnvironment.IsDevelopment())
            {
                services.AddScoped(_ =>
                {
                    // For local development connect to a local instance of Minio
                    var clientConfig = new AmazonS3Config
                    {
                        AuthenticationRegion = s3Config.Region,
                        ServiceURL = s3Config.LocalDevOnly_ServiceUrl,
                        ForcePathStyle = true,
                    };
                    return new AmazonS3Client(s3Config.LocalDevOnly_AccessKey, s3Config.LocalDevOnly_SecretKey, clientConfig);
                });
            }
            else
            {
                services.AddScoped(_ => new AmazonS3Client(RegionEndpoint.GetBySystemName(s3Config.Region)));
            }
        }

        private void ConfigureS3FileWriter(IServiceCollection services)
        {
            services.Configure<S3Configuration>(
                configuration.GetSection(S3Configuration.ConfigSection));
            services.AddScoped<IS3FileWriter, S3FileWriter>();
            services.AddScoped<S3ReferralFileKeyGenerator>();
        }
        
        private void ConfigureReferralFollowUpNotificationService(IServiceCollection services)
        {
            services.Configure<GlobalConfiguration>(
                configuration.GetSection(GlobalConfiguration.ConfigSection));
            services.Configure<ReferralRequestNotificationConfiguration>(
                configuration.GetSection(ReferralRequestNotificationConfiguration.ConfigSection));
            services.AddScoped<IReferralFollowUpNotificationService, ReferralFollowUpNotificationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Use forwarded headers, so the app knows it's using HTTPS and sets the HSTS headers
            // AWS ALB will automatically add `X-Forwarded-For` and `X-Forwarded-Proto`
            var forwardedHeaderOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };

            // TODO: We know all traffic to the container is from AWS, but ideally we
            // would still specify the IP and networks of the ALB here
            forwardedHeaderOptions.KnownNetworks.Clear();
            forwardedHeaderOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(forwardedHeaderOptions);

            if (!webHostEnvironment.IsDevelopment())
            {
                app.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandlingPath = "/error"
                });
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            if (webHostEnvironment.IsDevelopment())
            {
                // In production we terminate TLS at the load balancer and redirect there
                app.UseHttpsRedirection();
            }

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/robots.txt"))
                {
                    var robotsTxtPath = Path.Combine(env.ContentRootPath, "robots.txt");
                    var output = "User-agent: * \nAllow: /questionnaire/ \nDisallow: /";
                    if (File.Exists(robotsTxtPath))
                    {
                        output = await File.ReadAllTextAsync(robotsTxtPath);
                    }
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync(output);
                }
                else await next();
            });

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
            if (!webHostEnvironment.IsDevelopment() && !webHostEnvironment.IsProduction())
            {
                // Add HTTP Basic Authentication in our non-local-development and non-production environments
                // to make sure people don't accidentally stumble across the site
                app.UseMiddleware<BasicAuthMiddleware>();
            }
        }
    }
}
