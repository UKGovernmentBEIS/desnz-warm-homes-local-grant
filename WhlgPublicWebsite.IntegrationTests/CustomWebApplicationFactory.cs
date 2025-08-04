using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhlgPublicWebsite.Data;

namespace Tests;

public class CustomWebApplicationFactory<TProgram>(MockDatabase mockDatabase) : WebApplicationFactory<TProgram>
    where TProgram : class
{
    // public async Task StartPostgresContainerAsync()
    // {
    //     _postgresContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
    //         .WithDatabase(new PostgreSqlTestcontainerConfiguration
    //         {
    //             Database = "testdb",
    //             Username = "postgres",
    //             Password = "postgres"
    //         })
    //         .WithImage("postgres:15")
    //         .WithCleanUp(true)
    //         .WithName("test-postgres")
    //         .Build();
    //
    //     await _postgresContainer.StartAsync();
    // }
    //
    // public new async Task DisposeAsync()
    // {
    //     // await _postgresContainer.DisposeAsync();
    // }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<WhlgDbContext>));

            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbConnection));

            services.Remove(dbConnectionDescriptor);
            
            services.AddDbContext<WhlgDbContext>(options =>
            {
                options.UseNpgsql(mockDatabase.GetConnectionString());
            });
        });

        builder.UseEnvironment("Development");
        
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var settings = new Dictionary<string, string>
            {
                { "ConnectionStrings:PostgreSQLConnection", mockDatabase.GetConnectionString() }
            };
            config.AddInMemoryCollection(settings);
        });
    }
}