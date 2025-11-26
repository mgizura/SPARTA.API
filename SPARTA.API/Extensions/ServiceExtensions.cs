using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SPARTA.Domain.Interfaces.Users;
using SPARTA.Infrastructure.Repositories;
using SPARTA.Service.Feature.Users.CaseUse;
using SPARTA.Service.Feature.Users.Interfaces;

namespace SPARTA.API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureSqlServer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
    }

    public static void ConfigureAutoMapper(this IServiceCollection services)
    {
        // Aquí se registrarán los perfiles de AutoMapper cuando se creen
        // services.AddAutoMapper(typeof(MappingProfile));
    }

    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        var jwtIssuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
        var jwtAudience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp", policy =>
            {
                policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });
    }

    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
    }
}

