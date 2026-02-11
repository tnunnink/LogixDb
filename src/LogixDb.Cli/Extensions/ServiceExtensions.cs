using CliFx;
using Microsoft.Extensions.DependencyInjection;

namespace LogixDb.Cli.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Configures the <see cref="CliApplicationBuilder"/> to include services and components required for Logix-related functionality.
    /// </summary>
    /// <param name="builder">The <see cref="CliApplicationBuilder"/> to be configured with Logix services.</param>
    /// <returns>The configured <see cref="CliApplicationBuilder"/> instance with registered Logix services.</returns>
    public static CliApplicationBuilder AddLogixServices(this CliApplicationBuilder builder)
    {
        return builder.UseTypeActivator(_ =>
        {
            var services = new ServiceCollection();

            //todo register the database services.
            
            return services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
        });
    }
}