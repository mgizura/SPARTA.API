using Microsoft.EntityFrameworkCore;

namespace SPARTA.Infrastructure.Repositories;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aquí se configurarán las entidades cuando se creen
    }
}


