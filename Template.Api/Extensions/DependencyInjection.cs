using Microsoft.EntityFrameworkCore;
using Template.Api.Application.Authentication;
using Template.Core;
using Template.Data;
using Template.Services.Users;

namespace Template.Api.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {

            var provider = configuration["Database:Provider"] ?? "PostgreSQL";
            var connectionString = configuration["Database:ApplicationContext"] ?? "";

            bool isDesignTime = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName?.StartsWith("Microsoft.EntityFrameworkCore.Design") == true);

            if (isDesignTime && string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = "Host=localhost;Database=dev;Username=dev;Password=dev";
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
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
            });


            // ===== Database =====
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("ApplicationContext"))
                       .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySQL(configuration.GetConnectionString("ApplicationContext") ?? "")
                       .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll));

            // ===== Repositories =====
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // ===== Application Services =====
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITokenService, TokenService>();

            // ===== Domain Services =====
            services.AddScoped<IUserService, UserService>();

            // ===== Controllers & JSON =====
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            // ===== Swagger =====
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }
}
