# Alumni - University Communication and Alumni Tracking Platform

Bu proje, **.NET 8** üzerinde **Clean Architecture** ve **MVC (Model-View-Controller)** mimari prensipleri doğrultusunda geliştirilmiş kurumsal bir üniversite mezun takip ve iletişim platformudur.

---

## 🏛️ 1. MVC Mimarisi (Model - View - Controller)

Uygulama, sorumlulukların ayrılması (*Separation of Concerns*) ilkesine dayanarak klasik **MVC** deseni ile katmanlı Clean Architecture mimarisini harmanlamaktadır:

```text
       ┌────────────────────────────────────────────────────────┐
       │                        BROWSER                         │
       └────────────────────┬──────────────▲────────────────────┘
                            │ HTTP Request │ HTML / JSON Response
                            ▼              │
       ┌────────────────────────────────────────────────────────┐
       │                      CONTROLLER                        │
       │  • UserController (/users)      -> MVC View Controller │
       │  • ApiUserController (/api/users)-> REST API Controller│
       │  • AnnouncementsViewController  -> MVC View Controller │
       │  • AnnouncementsController      -> REST API Controller │
       │  • AutoController (/, /hello)   -> Basic Route Cont.   │
       └──────────────┬───────────────────────────▲─────────────┘
                      │ İşlemi Yürüt              │ Modeli Gönder
                      ▼                           │
       ┌───────────────────────────┐  ┌───────────┴─────────────┐
       │           MODEL           │  │          VIEW           │
       │  • User (CRUD Functions)  │  │  • Razor Views (.cshtml)│
       │  • Announcement           │──┼─>• Users/Index, Create, │
       │  • DTOs (Data Transfer)   │  │    Details, Edit        │
       │  • InMemory Stores        │  │  • Announcements Views  │
       └───────────────────────────┘  └─────────────────────────┘
```

### 🔹 Model (M)
* **Konum:** `src/Alumni.Domain/Entities/` ve `src/Alumni.Business/DTOs/`
* **Görevi:** Veri yapılarını, iş nesnelerini ve iş kurallarını barındırır.
* **Örnek:** `User` modeli, veritabanı bağlantısı olmaksızın (*without database connection*) bellek içi `Create`, `GetAll`, `GetById`, `Update` ve `Delete` CRUD fonksiyonlarını içerir.

### 🔹 View (Görünüm - V)
* **Konum:** `src/Alumni.API/Views/`
* **Görevi:** Kullanıcıya sunulan arayüz katmanıdır. Razor (`.cshtml`) şablonları ve modern Bootstrap tasarımı kullanılarak dinamik HTML çıktısı üretir.
* **Rotalar:**
  * `Views/Users/Index.cshtml` → Kullanıcı listeleme ve hızlı ekleme formu
  * `Views/Users/Create.cshtml` → Yeni kullanıcı oluşturma sayfası
  * `Views/Users/Details.cshtml` → Kullanıcı profil kartı ve detayları
  * `Views/Users/Edit.cshtml` → Kullanıcı bilgilerini güncelleme formu
  * `Views/Announcements/...` → Duyuru görüntüleme ve CRUD sayfaları

### 🔹 Controller (Denetleyici - C)
* **Konum:** `src/Alumni.API/Controllers/`
* **Görevi:** Gelen HTTP isteklerini karşılar, Model katmanındaki metodları çağırır ve uygun View'ı veya JSON yanıtını döner.
* **Controller Yapısı:**
  * `UserController` (`/users`) → HTML View katmanını yöneten MVC Controller.
  * `ApiUserController` (`/api/users`) → REST API isteklerini karşılayan API Controller.
  * `AnnouncementsViewController` (`/announcements`) → Duyuru HTML View Controller.
  * `AnnouncementsController` (`/api/announcements`) → Duyuru REST API Controller.
  * `AutoController` (`/`, `/hello`, `/sum`) → Temel rota alıştırmaları controller'ı.

---

## 📂 2. Proje Dizin, Klasör ve Dosya Yapısı (Directories, Folders, Files)

```text
Alumni-Backend/
│
├── Alumni.sln                             # Visual Studio / .NET Ana Solution Çözüm Dosyası
├── README.md                              # Proje Dokümantasyonu ve MVC Mimarisi Açıklaması
├── .gitignore                             # Git kaynak kontrolü istisna tanımları
│
└── src/
    │
    ├── Alumni.Domain/                     # [MODEL KATMANI] Temel Varlıklar ve Arayüzler
    │   ├── Alumni.Domain.csproj           # Domain katmanı proje dosyası
    │   ├── Entities/                      # Veri Modelleri (Entities)
    │   │   ├── User.cs                    # User Modeli (Bellek içi CRUD fonksiyonları dahil)
    │   │   ├── Announcement.cs            # Duyuru Modeli
    │   │   ├── AlumniProfile.cs           # Mezun profil bilgileri
    │   │   ├── Academician.cs             # Akademisyen profil bilgileri
    │   │   ├── Department.cs              # Üniversite bölümleri modeli
    │   │   ├── Company.cs                 # Şirket bilgileri modeli
    │   │   └── EmploymentHistory.cs       # Mezun iş geçmişi ilişkisi
    │   ├── Enums/
    │   │   └── UserRole.cs                # Roller: Admin, Alumni, Academician
    │   └── Interfaces/                    # Veri erişim soyutlamaları (IRepository, IUserRepository vb.)
    │
    ├── Alumni.Business/                   # [İŞ MANTIĞI] Servisler, DTO'lar ve Doğrulamalar
    │   ├── Alumni.Business.csproj         # Business katmanı proje dosyası
    │   ├── DTOs/                          # Veri Transfer Nesneleri (Data Transfer Objects)
    │   │   ├── User/                      # User DTO'ları (Create, Update, Patch, UserDto)
    │   │   ├── Announcement/              # Duyuru DTO'ları
    │   │   ├── Auth/                      # Giriş/Kayıt DTO'ları
    │   │   ├── Alumni/                    # Mezun profili DTO'ları
    │   │   └── Academician/               # Akademisyen DTO'ları
    │   └── Services/                      # İş Mantığı ve Bellek İçi Veri Yöneticileri
    │       ├── InMemoryUserStore.cs       # Veritabanı olmadan User CRUD arayüzü ve implementasyonu
    │       ├── IAnnouncementStore.cs      # Duyuru CRUD arayüzü ve in-memory yöneticisi
    │       ├── AuthService.cs             # Kimlik doğrulama ve JWT servisi
    │       ├── AlumniService.cs           # Mezun servis mantığı
    │       └── AcademicianService.cs      # Akademisyen servis mantığı
    │
    ├── Alumni.DataAccess/                 # [VERİ ERİŞİM] DbContext ve Repository Deseni
    │   ├── Alumni.DataAccess.csproj       # DataAccess katmanı proje dosyası
    │   ├── Context/
    │   │   └── AlumniDbContext.cs         # EF Core DbContext ve ModelBuilder konfigürasyonları
    │   ├── Migrations/                    # EF Core SQLite veritabanı migration dosyaları
    │   └── Repositories/                  # Generic ve varlığa özel repository implementasyonları
    │
    └── Alumni.API/                        # [SUNUM & CONTROLLER] Web API, MVC Controller ve View'lar
        ├── Alumni.API.csproj              # API katmanı proje dosyası (MVC & Swagger etkin)
        ├── Program.cs                     # Uygulama başlangıç yapılandırması (DI, Middleware, Swagger)
        ├── appsettings.json               # Konfigürasyon ve bağlantı dizesi ayarları
        │
        ├── Controllers/                   # [CONTROLLER KATMANI]
        │   ├── UserController.cs          # MVC Controller -> /users (View tabanlı CRUD)
        │   ├── ApiUserController.cs       # API Controller -> /api/users (REST API CRUD)
        │   ├── AnnouncementsViewController.cs # MVC Controller -> /announcements
        │   ├── AnnouncementsController.cs # API Controller -> /api/announcements
        │   ├── AutoController.cs          # Temel Rotalar: /, /hello, /hello/{name}, /sum/{n1}/{n2}
        │   ├── AuthController.cs          # JWT Login / Register
        │   ├── AlumniController.cs        # Mezun API işlemleri
        │   ├── AcademicianController.cs   # Akademisyen API işlemleri
        │   ├── DepartmentController.cs    # Bölüm API işlemleri
        │   └── HealthController.cs        # Sağlık kontrolü (/api/health)
        │
        └── Views/                         # [VIEW GÖRÜNÜM KATMANI - Razor .cshtml]
            ├── Users/
            │   ├── Index.cshtml           # Kullanıcı tablosu & hızlı kayıt görünümü
            │   ├── Create.cshtml          # Yeni kullanıcı ekleme sayfası
            │   ├── Details.cshtml         # Kullanıcı detay kartı görünümü
            │   └── Edit.cshtml            # Kullanıcı güncelleme sayfası
            └── Announcements/
                ├── Index.cshtml           # Duyurular listesi görünümü
                ├── Create.cshtml          # Duyuru oluşturma sayfası
                ├── Details.cshtml         # Duyuru detay sayfası
                └── Edit.cshtml            # Duyuru düzenleme sayfası
```

---

## 🚦 3. Rotalar ve Uç Noktalar (Routes & Endpoints)

### A. Kullanıcı Yönetimi Rotaları (User & ApiUser)

| Controller | Method | Rota | Açıklama |
|---|---|---|---|
| **UserController** (View) | `GET` | `/users` | Kullanıcı listesi HTML arayüzü |
| **UserController** (View) | `GET` | `/users/create` | Yeni kullanıcı oluşturma sayfası |
| **UserController** (View) | `POST` | `/users/create` | Yeni kullanıcı oluşturma formu gönderimi |
| **UserController** (View) | `GET` | `/users/details/{id}` | Kullanıcı detay sayfası |
| **UserController** (View) | `GET` | `/users/edit/{id}` | Kullanıcı düzenleme sayfası |
| **UserController** (View) | `POST` | `/users/edit/{id}` | Güncellenen bilgileri kaydetme |
| **UserController** (View) | `POST` | `/users/delete/{id}` | Kullanıcıyı silme |
| **ApiUserController** (API) | `GET` | `/api/users` | Tüm kullanıcıları JSON olarak listeler |
| **ApiUserController** (API) | `GET` | `/api/users/{id}` | ID'ye göre kullanıcı detayını döner |
| **ApiUserController** (API) | `POST` | `/api/users` | Yeni kullanıcı kaydeder (JSON) |
| **ApiUserController** (API) | `PUT` | `/api/users/{id}` | Kullanıcıyı tamamen günceller |
| **ApiUserController** (API) | `PATCH` | `/api/users/{id}` | Kullanıcıyı kısmi günceller |
| **ApiUserController** (API) | `DELETE`| `/api/users/{id}` | Kullanıcıyı siler (204 No Content) |

### B. Temel Tahta Rotaları (AutoController)

* `GET /` → `"ok"`
* `GET /hello` → `"Hello, World!"`
* `GET /hello/{name}` → `"Hello, {Name}!"` *(Örn: `/hello/emre` → `"Hello, Emre!"`)*
* `GET /sum/{number1}/{number2}` → Toplam sonucu *(Örn: `/sum/5/3` → `"8"`)*

---

## 📘 4. Swagger Dokümantasyonu

* **Swagger UI Adresi:** **`http://localhost:5185/swagger`**
* Tüm REST API rotaları (`/api/users`, `/api/announcements`, `/api/auth` vb.) Swagger üzerinde şemaları, durum kodları ve parametreleriyle eksiksiz belgelenmiştir.
