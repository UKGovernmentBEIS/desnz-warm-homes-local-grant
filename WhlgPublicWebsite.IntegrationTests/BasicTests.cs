using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Tests;

public class BasicTests
{
    private CustomWebApplicationFactory<WhlgPublicWebsite.Program>
        factory;

    private MockDatabase mockDatabase;

    [OneTimeSetUp]
    public async Task GlobalSetUp()
    {
        mockDatabase = new MockDatabase();
        await mockDatabase.CreateDatabaseAsync();
    }

    [SetUp]
    public void Setup()
    {
        factory = new CustomWebApplicationFactory<WhlgPublicWebsite.Program>(mockDatabase);
    }

    [OneTimeTearDown]
    public async Task GlobalTearDown()
    {
        await mockDatabase.DisposeAsync();
    }

    [DatapointSource]
    public string[] values = ["/"];

    [Theory]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync(new Uri($"https://localhost/"));

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.That(response.Content.Headers.ContentType.ToString(), Is.EqualTo("text/html; charset=utf-8"));
    }
}