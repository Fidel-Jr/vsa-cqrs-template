using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using VsaCqrsTemplate.Common.Abstractions;
using VsaCqrsTemplate.Common.Extensions;
using VsaCqrsTemplate.Features.Auth;
using VsaCqrsTemplate.Infrastructure.Persistence;
using VsaCqrsTemplate.Infrastructure.Persistence.Auth;
using VsaCqrsTemplate.Infrastructure.Security;
using VsaCqrsTemplate.Seeders;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).UseSnakeCaseNamingConvention());

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredUniqueChars = 1;

})
.AddEntityFrameworkStores<AppDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

// JWT token generation
builder.Services.AddScoped<IJwtService, JwtService>();

var key = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing");


builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!))
            };
    });
builder.Services.AddAuthorization();

// --- Tenancy services ---
builder.Services.AddCqrs(typeof(Program).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
// OpenAPI
builder.Services.AddOpenApi();

builder.Services.AddEndpoints();


var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

await db.Database.MigrateAsync();
await RoleSeeder.SeedAsync(
    scope.ServiceProvider);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

    app.Use(async (context, next) =>
    {
        var logger = context.RequestServices
            .GetRequiredService<ILogger<Program>>();
        logger.LogInformation(">>> REQUEST: {Method} {Path}",
            context.Request.Method, context.Request.Path);
        await next();
    });

app.MapEndpoints();

app.Run();