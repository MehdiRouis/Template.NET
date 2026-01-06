using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Template.Api.Application.Authentication;
using Template.Api.Application.Common.Validation;
using Template.Core;
using Template.Data;
using Template.Services.Security;
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

            // ===== Security Services =====
            services.AddScoped<IHashService, Argon2idHashService>();

            // ===== Domain Services =====
            services.AddScoped<IUserService, UserService>();

            // ===== Validators =====
            var assembly = Assembly.GetExecutingAssembly();

            var concreteValidators = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestValidator<>))
                    .Select(i => new { Service = i, Implementation = t })
                )
                .Where(x => !x.Implementation.IsGenericTypeDefinition &&
                            !x.Implementation.Name.StartsWith("RequestValidatorProvider"));

            foreach (var v in concreteValidators)
                services.AddScoped(v.Service, v.Implementation);

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
