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
    public DbSet<JobPosting> JobPostings { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }

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

        // JobPosting (İş İlanları)
        modelBuilder.Entity<JobPosting>(e =>
        {
            e.HasKey(jp => jp.Id);
            e.Property(jp => jp.Title).HasMaxLength(250).IsRequired();
            e.Property(jp => jp.Description).IsRequired();
            e.Property(jp => jp.Location).HasMaxLength(150);
            e.Property(jp => jp.EmploymentType).HasMaxLength(50);
            e.HasOne(jp => jp.Company)
                .WithMany(c => c.JobPostings)
                .HasForeignKey(jp => jp.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // JobApplication (İş Başvuruları)
        modelBuilder.Entity<JobApplication>(e =>
        {
            e.HasKey(ja => ja.Id);
            e.Property(ja => ja.Status).HasMaxLength(50).IsRequired();
            e.HasOne(ja => ja.JobPosting)
                .WithMany(jp => jp.Applications)
                .HasForeignKey(ja => ja.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(ja => ja.User)
                .WithMany()
                .HasForeignKey(ja => ja.UserId)
                .OnDelete(DeleteBehavior.Cascade);
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

        modelBuilder.Entity<Company>().HasData(
            new Company { Id = 1, Name = "Aselsan", Sector = "Savunma Sanayi", City = "Ankara", Country = "Türkiye", IsVerified = true },
            new Company { Id = 2, Name = "Baykar Teknoloji", Sector = "Havacılık", City = "İstanbul", Country = "Türkiye", IsVerified = true },
            new Company { Id = 3, Name = "Trendyol", Sector = "E-Ticaret / Teknoloji", City = "İstanbul", Country = "Türkiye", IsVerified = true }
        );

        modelBuilder.Entity<JobPosting>().HasData(
            new JobPosting
            {
                Id = 1,
                CompanyId = 1,
                Title = "Kıdemli .NET Backend Mühendisi",
                Description = "Savunma ve haberleşme sistemleri projelerimizde görevlendirilmek üzere C#, .NET 8 ve Clean Architecture deneyimli takım arkadaşları arıyoruz.",
                Requirements = ".NET 8+, EF Core, SQL Server/PostgreSQL, Mikroservis mimarisi",
                Location = "Ankara / Hibrit",
                EmploymentType = "Tam Zamanlı",
                SalaryRange = "80.000 - 120.000 TL",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc)
            },
            new JobPosting
            {
                Id = 2,
                CompanyId = 3,
                Title = "Full Stack Web Geliştirici",
                Description = "Yüksek trafikli e-ticaret platformumuzun mikro frontend ve API altyapısında çalışacak mühendisler arıyoruz.",
                Requirements = "ASP.NET Core Web API, React/Angular, Redis, Docker",
                Location = "İstanbul / Uzaktan",
                EmploymentType = "Tam Zamanlı",
                SalaryRange = "70.000 - 110.000 TL",
                IsActive = true,
                CreatedAt = new DateTime(2026, 2, 1, 12, 0, 0, DateTimeKind.Utc)
            },
            new JobPosting
            {
                Id = 3,
                CompanyId = 2,
                Title = "Yazılım Stajyeri (2026 Yaz Dönemi)",
                Description = "Üniversitelerin Bilgisayar veya Yazılım Mühendisliği 3./4. sınıf öğrencilerine yönelik yaz stajı programı.",
                Requirements = "Nesne yönelimli programlama, Algoritma bilgisi, C# veya Python temeli",
                Location = "İstanbul (Özdemir Bayraktar Milli Teknoloji Merkezi)",
                EmploymentType = "Staj",
                SalaryRange = "Asgari Ücret Üzeri",
                IsActive = true,
                CreatedAt = new DateTime(2026, 3, 10, 9, 30, 0, DateTimeKind.Utc)
            }
        );
    }
}
