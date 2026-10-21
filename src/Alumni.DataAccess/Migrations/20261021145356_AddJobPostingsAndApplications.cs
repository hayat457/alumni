using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Alumni.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddJobPostingsAndApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobPostings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Requirements = table.Column<string>(type: "TEXT", nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    EmploymentType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SalaryRange = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ClosingDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompanyId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPostings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPostings_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JobPostingId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ApplicantName = table.Column<string>(type: "TEXT", nullable: true),
                    ApplicantEmail = table.Column<string>(type: "TEXT", nullable: true),
                    CoverLetter = table.Column<string>(type: "TEXT", nullable: true),
                    ResumeUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplications_JobPostings_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPostings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobApplications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "City", "Country", "IsVerified", "Name", "Sector", "WebsiteUrl" },
                values: new object[,]
                {
                    { 1, "Ankara", "Türkiye", true, "Aselsan", "Savunma Sanayi", null },
                    { 2, "İstanbul", "Türkiye", true, "Baykar Teknoloji", "Havacılık", null },
                    { 3, "İstanbul", "Türkiye", true, "Trendyol", "E-Ticaret / Teknoloji", null }
                });

            migrationBuilder.InsertData(
                table: "JobPostings",
                columns: new[] { "Id", "ClosingDate", "CompanyId", "CreatedAt", "Description", "EmploymentType", "IsActive", "Location", "Requirements", "SalaryRange", "Title" },
                values: new object[,]
                {
                    { 1, null, 1, new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Savunma ve haberleşme sistemleri projelerimizde görevlendirilmek üzere C#, .NET 8 ve Clean Architecture deneyimli takım arkadaşları arıyoruz.", "Tam Zamanlı", true, "Ankara / Hibrit", ".NET 8+, EF Core, SQL Server/PostgreSQL, Mikroservis mimarisi", "80.000 - 120.000 TL", "Kıdemli .NET Backend Mühendisi" },
                    { 2, null, 3, new DateTime(2026, 2, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Yüksek trafikli e-ticaret platformumuzun mikro frontend ve API altyapısında çalışacak mühendisler arıyoruz.", "Tam Zamanlı", true, "İstanbul / Uzaktan", "ASP.NET Core Web API, React/Angular, Redis, Docker", "70.000 - 110.000 TL", "Full Stack Web Geliştirici" },
                    { 3, null, 2, new DateTime(2026, 3, 10, 9, 30, 0, 0, DateTimeKind.Utc), "Üniversitelerin Bilgisayar veya Yazılım Mühendisliği 3./4. sınıf öğrencilerine yönelik yaz stajı programı.", "Staj", true, "İstanbul (Özdemir Bayraktar Milli Teknoloji Merkezi)", "Nesne yönelimli programlama, Algoritma bilgisi, C# veya Python temeli", "Asgari Ücret Üzeri", "Yazılım Stajyeri (2026 Yaz Dönemi)" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_JobPostingId",
                table: "JobApplications",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_UserId",
                table: "JobApplications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_CompanyId",
                table: "JobPostings",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobApplications");

            migrationBuilder.DropTable(
                name: "JobPostings");

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
