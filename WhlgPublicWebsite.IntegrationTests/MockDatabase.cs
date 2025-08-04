using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace Tests;

public class MockDatabase
{
    private readonly PostgreSqlContainer postgresContainer = new PostgreSqlBuilder().Build();

    public async Task CreateDatabaseAsync()
    {
        await postgresContainer.StartAsync();
    }
    
    public string GetConnectionString()
    {
        return postgresContainer.GetConnectionString();
    }
    
    public async Task DisposeAsync()
    {
        await postgresContainer.DisposeAsync();
    }
}