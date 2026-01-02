using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Template.Data;

public sealed class DesignTimeDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = configuration["Database:ApplicationContext"] ?? "";
        string provider = configuration["Database:Provider"] ?? "PostgreSQL";

        var options = new DbContextOptionsBuilder<ApplicationDbContext>();

        switch (provider.ToLowerInvariant())
        {
            case "postgresql":
            case "pgsql":
                options.UseNpgsql(connectionString);
                break;

            case "mysql":
                options.UseMySQL(connectionString);
                break;

            default:
                throw new InvalidOperationException($"Unsupported DB provider: {provider}");
        }

        return new ApplicationDbContext(options.Options);
    }
}
