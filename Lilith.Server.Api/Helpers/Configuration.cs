using Microsoft.Extensions.Configuration;

namespace Lilith.Server.Helpers;

public class ConfigurationLoader
{
    public IConfiguration LoadConfiguration()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();

        return configuration;
    }
}
