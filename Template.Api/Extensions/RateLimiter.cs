using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;

namespace Template.Api.Extensions
{
    public static class RateLimiter
    {
        public static IServiceCollection AddRateLimiter(this IServiceCollection services, IConfiguration configuration)
        {

            // ===== RateLimiter =====
            services.AddRateLimiter(limiterOptions =>
            {
                limiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                limiterOptions.AddFixedWindowLimiter("fixed", opt =>
                {
                    opt.Window = TimeSpan.FromSeconds(10);
                    opt.PermitLimit = 3;
                    opt.QueueLimit = 0;
                });

                limiterOptions.AddSlidingWindowLimiter("sliding", opt =>
                {
                    opt.Window = TimeSpan.FromSeconds(15);
                    opt.SegmentsPerWindow = 3;
                    opt.PermitLimit = 15;
                    opt.QueueLimit = 0;
                });

                limiterOptions.AddTokenBucketLimiter("token", opt =>
                {
                    opt.TokenLimit = 20;
                    opt.TokensPerPeriod = 5;
                    opt.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
                    opt.QueueLimit = 0;
                });

                limiterOptions.AddConcurrencyLimiter("concurrency", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.QueueLimit = 0;
                });
            });

            return services;
        }
    }
}
