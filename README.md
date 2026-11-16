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
    ├── Alumni.API/                        # [SUNUM & CONTROLLER] Web API, MVC Controller ve View'lar
        ├── Alumni.API.csproj              # API katmanı proje dosyası (MVC & Swagger etkin)
        ├── Program.cs                     # Uygulama başlangıç yapılandırması (DI, Middleware, Swagger)
        ├── appsettings.json               # Konfigürasyon ve bağlantı dizesi ayarları
        │
        ├── Middlewares/                   # [GÜVENLİK, LOG & HATA YÖNETİMİ]
        │   ├── ExceptionGuardianMiddleware.cs # 5xx Global Exception Handler & logs/error.logs dosya loglayıcı
        │   ├── LoggerMiddleware.cs        # Formatlı istek/yanıt terminal loglayıcı (time|method|route|status|duration)
        │   └── NotFoundGuardianMiddleware.cs  # 404 Guardian (JSON reason hata dönüşü & 404.html yönlendirmesi)
        │
        ├── Controllers/                   # [CONTROLLER KATMANI]
        │   ├── HomeController.cs          # Modern portal ana sayfası & /404 endpoint'i
        │   ├── JobPostingsController.cs   # İş ilanları REST API (Doğrudan & İç İçe Rotalar)
        │   ├── JobPostingsViewController.cs# İş ilanları MVC Razor Controller
        │   ├── ApplicationsViewController.cs # Başvurular paneli Razor Controller
        │   ├── ApplicationsController.cs  # Başvurular REST API Controller
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
        ├── Views/                         # [VIEW GÖRÜNÜM KATMANI - Razor .cshtml]
        │   ├── Home/
        │   │   └── Index.cshtml           # Modern portal ana sayfası (Hero, İstatistikler)
        │   ├── JobPostings/
        │   │   ├── Index.cshtml           # İlan listesi ve filtreleme
        │   │   ├── Create.cshtml          # Yeni ilan yayınlama formu (Validation destekli)
        │   │   ├── Details.cshtml         # İlan detay ve başvuru kartı
        │   │   └── Apply.cshtml           # Başvuru formu
        │   ├── Applications/
        │   │   └── Index.cshtml           # Başvuru yönetim paneli
        │   ├── Users/
        │   │   ├── Index.cshtml           # Kullanıcı tablosu & hızlı kayıt görünümü
        │   │   ├── Create.cshtml          # Yeni kullanıcı ekleme sayfası
        │   │   ├── Details.cshtml         # Kullanıcı detay kartı görünümü
        │   │   └── Edit.cshtml            # Kullanıcı güncelleme sayfası
        │   └── Announcements/
        │       ├── Index.cshtml           # Duyurular listesi görünümü
        │       ├── Create.cshtml          # Duyuru oluşturma sayfası
        │       ├── Details.cshtml         # Duyuru detay sayfası
        │       └── Edit.cshtml            # Duyuru düzenleme sayfası
        │
        └── wwwroot/                       # [STATİK VARLIKLAR & HIZLI HTML ARAYÜZLERİ]
            ├── 404.html                   # 404 Guardian generic hata sayfası
            ├── index.html                 # Statik portal ana sayfası
            ├── users.html                 # Fetch API tabanlı kullanıcılar sayfası
            ├── job-postings.html          # Fetch API tabanlı iş ilanları sayfası
            └── announcements.html         # Fetch API tabanlı duyurular sayfası
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

## 🛡️ 4. Middleware & Koruma Mimarisi (Guardians & Logging)

Sistem mimarisi, gelen HTTP isteklerini güvenlik, izlenebilirlik ve hata toleransı sağlamak adına sıralı özel middleware katmanlarıyla donatılmıştır:

```text
HTTP Request
    │
    ▼
┌───────────────────────────────────┐
│ 1. ExceptionGuardianMiddleware     │ -> 5xx Hataları yakalar, logs/error.logs'a yazar, JSON 500 döner
└─────────────────┬─────────────────┘
                  ▼
┌───────────────────────────────────┐
│ 2. LoggerMiddleware               │ -> Terminale "time | method | route | status | duration" yazar
└─────────────────┬─────────────────┘
                  ▼
┌───────────────────────────────────┐
│ 3. NotFoundGuardianMiddleware     │ -> 404 durumunda API için detaylı JSON 'reason', tarayıcı için 404.html sunar
└─────────────────┬─────────────────┘
                  ▼
┌───────────────────────────────────┐
│ 4. Controllers & Static Endpoints │ -> İş mantığı, API endpoint'leri ve Razor/HTML görünümleri
└───────────────────────────────────┘
```

### 1. 5xx Guardian (`ExceptionGuardianMiddleware`)
* **Görevi:** Uygulama genelinde fırlatılan yakalanmamış istisnaları (Unhandled Exceptions) ele alır.
* **İstemci Yanıtı:** Standardize edilmiş anlaşılır `500 Internal Server Error` JSON modeli döner:
  ```json
  {
    "statusCode": 500,
    "error": "Internal Server Error",
    "message": "Sunucuda beklenmeyen bir hata meydana geldi.",
    "detail": "Hata detayı (Development modunda)",
    "path": "/api/...",
    "method": "POST",
    "timestamp": "2026-11-16T21:27:00Z"
  }
  ```
* **Dosya Loglama (`logs/error.logs`):** İstisna detaylarını (tarih-saat, HTTP metodu, rota, exception tipi, mesaj ve stack trace) `logs/error.logs` dosyasına yazar. Dizin yoksa otomatik oluşturulur; `.gitignore` ile repodan hariç tutulur.

### 2. Formatlı İstek Loglayıcı (`LoggerMiddleware`)
* **Görevi:** Gelen her HTTP isteğini işleme süresiyle birlikte konsola loglar.
* **Format:** `time | method | route | res type (status code) | duration`
* **Örnek Konsol Çıktısı:**
  ```text
  10:20:30 POST /api/users 201 18ms
  10:20:35 GET /job-postings 200 4ms
  10:21:02 GET /api/bilinmeyen-rota 404 1ms
  ```

### 3. 404 Guardian (`NotFoundGuardianMiddleware`) & Generic 404 Sayfası
* **Görevi:** Sunucuda tanımlı olmayan rotalara yapılan istekleri yakalar.
* **API / JSON İstekleri:** Açıklayıcı bir `reason` ve öneri içeren JSON formatında hata döner:
  ```json
  {
    "statusCode": 404,
    "error": "Not Found",
    "reason": "Belirtilen rota veya kaynak sunucuda bulunamadı: 'GET /api/bilinmeyen-rota'",
    "path": "/api/bilinmeyen-rota",
    "method": "GET",
    "timestamp": "2026-11-16T21:12:27Z",
    "suggestion": "Lütfen rota adını, parametreleri veya Swagger dokümantasyonunu (/swagger) kontrol ediniz."
  }
  ```
* **Tarayıcı / HTML İstekleri:** Kullanıcıyı modern tasarımlı `/404` (`wwwroot/404.html`) sayfasına yönlendirerek güvenli geri dönüş butonları sunar.

---

## 📘 5. Swagger Dokümantasyonu

* **Swagger UI Adresi:** **`http://localhost:5185/swagger`**
* Tüm REST API rotaları (`/api/users`, `/api/announcements`, `/api/auth` vb.) Swagger üzerinde şemaları, durum kodları ve parametreleriyle eksiksiz belgelenmiştir.
