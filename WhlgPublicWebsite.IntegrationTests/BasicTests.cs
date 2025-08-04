using System.Threading.Tasks;
using NUnit.Framework;

namespace Tests;

public class BasicTests
{
    private CustomWebApplicationFactory<WhlgPublicWebsite.Program>
        factory;

    [SetUp]
    public void SetUp()
    {
        factory = new CustomWebApplicationFactory<WhlgPublicWebsite.Program>();
    }

    [TearDown]
    public void TearDown()
    {
        factory.Dispose();
    }

    [DatapointSource]
    public string[] values = ["/"];

    [Theory]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.That(response.Content.Headers.ContentType.ToString(), Is.EqualTo("text/html; charset=utf-8"));
    }
}