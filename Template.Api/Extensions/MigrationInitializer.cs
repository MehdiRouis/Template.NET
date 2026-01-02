using Template.Data;
using Microsoft.EntityFrameworkCore;

namespace Template.Api.Extensions
{
    public class MigrationInitializer
    {
        /// <summary>
        /// Initialize EFCORE migrations
        /// </summary>
        /// <param name="app"></param>
        public static void Initialize(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Database.Migrate();
        }
    }
}
