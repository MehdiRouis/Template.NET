using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Template.Api.Extensions;
using Template.Api.Security.Sessions;


JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices(builder.Configuration);
builder.Services.AddRateLimiter(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(o =>
{
    o.SaveToken = true;

    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = $"{(builder.Configuration["Project:Name"] ?? "base").ToLowerInvariant()}.identity",

        ValidateAudience = true,
        ValidAudience = $"{(builder.Configuration["Project:Name"] ?? "base").ToLowerInvariant()}.api",

        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"] ?? "base")),

        ClockSkew = TimeSpan.FromSeconds(10)
    };
    o.Events = new JwtBearerEvents
    {
        OnTokenValidated = JwtSessionHandler.OnTokenValidated
    };
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
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
