# Alumni - University Communication and Alumni Tracking Platform (Backend API)

🏛️ Architecture and Design Decisions

During the system analysis and design phases, the following key decisions were made for Alumni:
- **Clean Architecture:** Based on the "Separation of Concerns" principle, the project is built on a sustainable and flexible layered structure where dependencies flow from the outside in.
- **Flexible Data Integration (SIS Readiness):** For future official integration with the university's Student Information System (SIS), data access is abstracted via Interfaces; a mock data infrastructure is set up to run until real data is connected.
- **Database Normalization:** Relational integrity is ensured using Entity Framework Core (Code-First); a robust MSSQL schema is designed covering user roles, department details, companies, and employment histories.

🛠️ Tech Stack

The project relies on industry-standard enterprise backend technologies:

**Backend:**
- **C# (.NET 8.0)** - High-performance and type-safe object-oriented core language
- **ASP.NET Core Web API** - RESTful service architecture

**Database & ORM:**
- **MS SQL Server** - Relational database management system
- **Entity Framework Core** - Database querying and Code-First modeling

**Security & Authorization:**
- **JWT (JSON Web Token)** - Secure and role-based access control (Admin, Alumni, Academician)

**DevOps & Tools:**
- **Swagger (OpenAPI)** - API endpoint documentation and testing
- **Git & GitHub** - Version control and repository management

📂 Project Directory Structure

```text
Alumni-Backend/
│
├── src/
│   ├── Alumni.Domain/         # Database models (Entity) and Interfaces
│   ├── Alumni.DataAccess/     # EF Core configurations, DbContext, and Repository pattern
│   ├── Alumni.Business/       # Business rules, validations, and data services
│   └── Alumni.API/            # Controllers, JWT settings, and Swagger (Main Entry Point)
│
├── Alumni.sln                 # Visual Studio main solution file
└── README.md                  # Project documentation
