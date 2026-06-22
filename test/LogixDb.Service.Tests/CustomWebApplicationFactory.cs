using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace LogixDb.Service.Tests;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var customSettings = new Dictionary<string, string?>
        {
            ["LogixConfig:DbConnection"] = TestEnvironment.Database.Connection.ToString()
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(customSettings)
            .Build();

        // UseConfiguration ensures these settings are available BEFORE WebApplication.CreateBuilder(args)
        // finishes its internal configuration setup in Program.cs
        builder.UseConfiguration(configuration);

        builder.ConfigureAppConfiguration((_, config) =>
        {
            // Optional: Also add it here to ensure it's present in the final app configuration
            config.AddInMemoryCollection(customSettings);
        });
    }
}