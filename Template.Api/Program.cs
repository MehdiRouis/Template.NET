using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Template.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(o =>
{
    o.SaveToken = true;

    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = $"{(builder.Configuration["Project:Name"] ?? "base").ToLower()}.identity",

        ValidateAudience = true,
        ValidAudience = $"{(builder.Configuration["Project:Name"] ?? "base").ToLower()}.api",

        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"] ?? "base")),

        ClockSkew = TimeSpan.FromSeconds(10)
    };
});

builder.Services.AddRateLimiter(limiterOptions =>
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    MigrationInitializer.Initialize(app);
}

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/error");
}
else
{
    app.UseExceptionHandler("/error");
    app.UseStatusCodePages();
    app.UseHsts();
}

app.UseRateLimiter();
app.MapControllers();
app.Run();
