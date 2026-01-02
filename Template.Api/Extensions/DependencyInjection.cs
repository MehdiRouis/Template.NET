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
            var connectionString = configuration["Database:ConnectionString"] ?? "";

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
