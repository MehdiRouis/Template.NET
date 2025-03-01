using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace TemplateBase.Api.Middleware
{
    public static class CustomHangfire
    {
        public static void UseCustomHangfire(this IApplicationBuilder app, IConfiguration configuration)
        {
            /*var manager = new RecurringJobManager();
            RecurringJob.AddOrUpdate<DiscordLoggerHelper>("DeleteLogs", x => x.DeleteLogs(), Cron.Hourly());
            RecurringJob.AddOrUpdate<DiscordLoggerHelper>("HangfireStatsHourly", x => x.LogStats(), Cron.Hourly());*/

            // Update if necessary server jobs
            if (configuration["ASPNETCORE_ENVIRONMENT"] == "Development")
            {
                app.UseHangfireDashboard("/api/hangfire", new DashboardOptions
                {
                    //Authorization = new[] { app.ApplicationServices.GetRequiredService<HangfireDashboardAuthorizationFilter>() }
                });
            }
        }
    }
}
