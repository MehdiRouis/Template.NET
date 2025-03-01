using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateBase.Api.Middleware;
using TemplateBase.Api.Middlewares;
using TemplateBase.Core;
using TemplateBase.Data;

namespace TemplateBase.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });

            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .WithOrigins(Configuration.GetSection("SafeOrigins:AppOrigins").GetChildren().Select(x => x.Value).ToArray())
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .Build();
            }));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //Supprime le slash de fin sur les urls
            services.Configure<RouteOptions>(options => options.AppendTrailingSlash = false);

            //Configure l'ajout de session
            services.AddSession();

            // Retirer les URI des claims
            JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

            //Authentication scheme
            services.AddAuthentication("Api")
                .AddJwtBearer("Api", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = Configuration["Jwt:Api:ActivateRefreshToken"] == "True" ? true : false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Api:Token"])),
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            /*if (!JwtAuthenticationValidation.IsAuthenticationValid(context))
                                context.Fail("Unauthorized");*/
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/api/hubs")))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Base", policy =>
                    policy.AddAuthenticationSchemes("Api")
                          .RequireAuthenticatedUser());
            });

            services.AddControllersWithViews();

            services.AddSignalR();

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = 3;
                options.Queues = new[] { "critical", "default", "low" };
            });

            //Global Environnement in appsettings
            services.Configure<ProjectEnvironment>(Configuration.GetSection("Project"));

            services.AddServices(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/error");
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseStatusCodePages();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                InitializeMigrations(app);
            }


            app.UseMiddleware<RemoteIpAddressMiddleware>();
            app.UseCors("CorsPolicy");

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseSecurityHeaders();
            // For proxy server compatibility
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseSession();
            app.UseWebSockets();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseCustomHangfire(Configuration);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(name: "name", areaName: "areaName", pattern: "areaName/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        /// <summary>
        /// Initialize EFCORE migrations
        /// </summary>
        /// <param name="app"></param>
        private static void InitializeMigrations(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
            if (serviceScope == null) return;
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }
    }
}
