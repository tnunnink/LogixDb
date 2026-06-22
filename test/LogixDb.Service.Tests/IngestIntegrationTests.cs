using System.Net;
using System.Net.Http.Headers;
using LogixDb.Data;
using LogixDb.Service.Common;
using LogixDb.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace LogixDb.Service.Tests;

[TestFixture]
public class IngestIntegrationTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // We want to override the LogixConfig to use an in-memory SQLite database
                // Note: LogixDb registration depends on this config.
                // Depending on how AddLogixDb is implemented, we might need to re-register IDbManager
                // or just ensure the config is correct before it's used.
                
                services.AddSingleton(new LogixConfig
                {
                    DbConnection = "Data Source=IngestTests;Mode=Memory;Cache=Shared"
                });
            });
        });
        
        _client = _factory.CreateClient();
    }

    [Test]
    public async Task GetHealth_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task PostIngest_ValidFile_ReturnsAccepted()
    {
        // Arrange
        var source = TestSource.LocalTest();
        using var content = new MultipartFormDataContent();
        var fileContent = new StringContent(source.ToString());
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");
        content.Add(fileContent, "file", "test.l5x");

        // Act
        var response = await _client.PostAsync("/ingest", content);

        // Assert
        var result = await response.Content.ReadAsStringAsync();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Accepted), $"Response body: {result}");
        Assert.That(result, Does.Contain("Queued"));
        Assert.That(result, Does.Contain("test.l5x"));
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task PostIngest_InvalidExtension_ReturnsBadRequest()
    {
        // Arrange
        using var content = new MultipartFormDataContent();
        var fileContent = new StringContent("some content");
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
        content.Add(fileContent, "file", "test.txt");

        // Act
        var response = await _client.PostAsync("/ingest", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var result = await response.Content.ReadAsStringAsync();
        Assert.That(result, Does.Contain("Invalid file type"));
    }
}
