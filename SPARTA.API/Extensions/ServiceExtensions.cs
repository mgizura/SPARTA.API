using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SPARTA.Infrastructure.Repositories;

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
}

