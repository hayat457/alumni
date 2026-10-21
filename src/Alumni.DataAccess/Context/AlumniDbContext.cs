using Alumni.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Alumni.DataAccess.Context;

public class AlumniDbContext : DbContext
{
    public AlumniDbContext(DbContextOptions<AlumniDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<AlumniProfile> AlumniProfiles { get; set; }
    public DbSet<Academician> Academicians { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<EmploymentHistory> EmploymentHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).HasMaxLength(256).IsRequired();
            e.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            e.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.Role).HasConversion<string>();
        });

        // Department
        modelBuilder.Entity<Department>(e =>
        {
            e.HasKey(d => d.Id);
            e.HasIndex(d => d.Code).IsUnique();
            e.Property(d => d.Name).HasMaxLength(200).IsRequired();
            e.Property(d => d.Code).HasMaxLength(20).IsRequired();
        });

        // AlumniProfile
        modelBuilder.Entity<AlumniProfile>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasIndex(a => a.StudentNumber).IsUnique();
            e.Property(a => a.StudentNumber).HasMaxLength(20).IsRequired();
            e.HasOne(a => a.User)
                .WithOne(u => u.AlumniProfile)
                .HasForeignKey<AlumniProfile>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(a => a.Department)
                .WithMany(d => d.Alumni)
                .HasForeignKey(a => a.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Academician
        modelBuilder.Entity<Academician>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Title).HasMaxLength(50).IsRequired();
            e.HasOne(a => a.User)
                .WithOne(u => u.Academician)
                .HasForeignKey<Academician>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(a => a.Department)
                .WithMany(d => d.Academicians)
                .HasForeignKey(a => a.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Company
        modelBuilder.Entity<Company>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).HasMaxLength(200).IsRequired();
        });

        // EmploymentHistory
        modelBuilder.Entity<EmploymentHistory>(e =>
        {
            e.HasKey(eh => eh.Id);
            e.Property(eh => eh.Position).HasMaxLength(200).IsRequired();
            e.HasOne(eh => eh.AlumniProfile)
                .WithMany(a => a.EmploymentHistories)
                .HasForeignKey(eh => eh.AlumniProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(eh => eh.Company)
                .WithMany(c => c.EmploymentHistories)
                .HasForeignKey(eh => eh.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed Data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Bilgisayar Mühendisliği", Code = "BM", IsActive = true },
            new Department { Id = 2, Name = "Elektrik-Elektronik Mühendisliği", Code = "EEM", IsActive = true },
            new Department { Id = 3, Name = "Makine Mühendisliği", Code = "MM", IsActive = true },
            new Department { Id = 4, Name = "İşletme", Code = "ISL", IsActive = true }
        );
    }
}

