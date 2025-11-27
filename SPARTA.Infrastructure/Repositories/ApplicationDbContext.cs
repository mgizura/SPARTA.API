using Microsoft.EntityFrameworkCore;
using SPARTA.Domain.Entities.Users;
using SPARTA.Domain.Entities.Provinces;

namespace SPARTA.Infrastructure.Repositories;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Province> Provinces { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.Username).HasColumnName("Username").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasColumnName("Email").HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("PasswordHash").HasMaxLength(500).IsRequired();
            entity.Property(e => e.FirstName).HasColumnName("FirstName").HasMaxLength(200);
            entity.Property(e => e.LastName).HasColumnName("LastName").HasMaxLength(200);
            entity.Property(e => e.IsActive).HasColumnName("IsActive").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");

            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Province Configuration
        modelBuilder.Entity<Province>(entity =>
        {
            entity.ToTable("Provinces");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.Name).HasColumnName("Name").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Code).HasColumnName("Code").HasMaxLength(50);
            entity.Property(e => e.IsActive).HasColumnName("IsActive").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");

            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.Code)
                .IsUnique()
                .HasFilter("[Code] IS NOT NULL");
        });
    }
}


