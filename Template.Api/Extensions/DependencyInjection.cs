using Template.Core;
using Template.Data;
using Microsoft.EntityFrameworkCore;

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

            // ===== Domain Services =====
            /*services.AddScoped<IJwtSigningService, JwtSigningService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IPermissionService, PermissionService>();*/

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
