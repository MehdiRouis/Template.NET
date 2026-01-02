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
            // ===== Database =====
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("ApplicationContext"))
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
