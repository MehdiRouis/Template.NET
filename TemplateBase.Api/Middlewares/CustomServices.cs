using AutoMapper;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TemplateBase.Api.Models.AutoMapping;
using TemplateBase.Core;
using TemplateBase.Data;

namespace TemplateBase.Api.Middlewares
{
    public static class CustomServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            /////DBCONTEXT
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Context")));
            services.AddScoped<IDbContext, ApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

            //HANGFIRE
            services.AddHangfire(options => options.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(configuration.GetConnectionString("Context"))));

            //REPOSITORY
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            //HTTP CONTEXT ACCESSOR
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //AUTOMAPPER
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BaseMapping>();
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            return services;
        }
    }
}
