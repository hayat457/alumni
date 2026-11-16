# Domain-Driven Design (DDD) Documentation - Alumni Platform

Bu doküman, sistemin **Kariyer ve İlan Yönetimi (Career & Job Management)** alt alanına ait veritabanı tablolarını, varlık ilişkilerini (Relations), doğrudan ve iç içe (Direct & Nested) rotalarını detaylandırmaktadır.

---

## 1. Bounded Context: Kariyer ve İş İlanı Yönetimi

```text
┌─────────────────────────────────────────────────────────────────────────────┐
│                       CAREER & OPPORTUNITY CONTEXT                          │
│                                                                             │
│   ┌─────────────┐       1 : N       ┌─────────────────┐                     │
│   │   Company   │──────────────────>│   JobPosting    │                     │
│   └─────────────┘                   └────────┬────────┘                     │
│                                              │ 1 : N                        │
│                                              ▼                              │
│   ┌─────────────┐       1 : N       ┌─────────────────┐                     │
│   │    User     │──────────────────>│ JobApplication  │                     │
│   └─────────────┘                   └─────────────────┘                     │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 2. Tablo Şemaları ve Varlıklar (Entities & Schemas)

### A. `JobPostings` Tablosu (İş İlanları)
Şirketlerin mezunlar ve öğrenciler için yayımladığı açık pozisyonları temsil eder.

| Kolon Adı | Veri Tipi | Kısıtlar (Constraints) | Açıklama |
|---|---|---|---|
| `Id` | `int` | Primary Key, Auto-Increment | Benzersiz ilan ID'si |
| `CompanyId` | `int` | Foreign Key (`Companies.Id`), Required | İlanı yayımlayan şirket (**Zorunlu / Required**) |
| `Title` | `nvarchar(120)` | Required, Max: 120 | Pozisyon / İlan başlığı (**Zorunlu / Required, Maks 120 Karakter**) |
| `Description`| `nvarchar(max)` | Nullable, Optional | İş ve rol tanımı (**Opsiyonel / Optional**) |
| `Requirements`| `nvarchar(max)` | Nullable | Aranan teknik nitelikler |
| `Location` | `nvarchar(150)` | Nullable, Optional | Şehir / Lokasyon / City (**Opsiyonel / Optional**) |
| `EmploymentType` | `nvarchar(50)` | Default: `'Tam Zamanlı'` | Tam Zamanlı, Yarı Zamanlı, Staj, Uzaktan |
| `SalaryRange`| `nvarchar(100)` | Nullable | Maaş aralığı |
| `IsActive` | `bit` | Default: `1` (True) | İlanın aktiflik durumu |
| `CreatedAt` | `datetime2` | Not Null, Default: UTC Now | İlanın yayımlanma tarihi |
| `ClosingDate`| `datetime2` | Nullable | Son başvuru tarihi |

### B. `JobApplications` Tablosu (Başvurular)
Kullanıcıların/mezunların bir iş ilanına yaptığı başvuruları temsil eder.

| Kolon Adı | Veri Tipi | Kısıtlar (Constraints) | Açıklama |
|---|---|---|---|
| `Id` | `int` | Primary Key, Auto-Increment | Benzersiz başvuru ID'si |
| `JobPostingId`| `int` | Foreign Key (`JobPostings.Id`), Cascade | Başvurulan iş ilanı |
| `UserId` | `int` | Foreign Key (`Users.Id`), Cascade | Başvuran kullanıcı |
| `ApplicantName`| `nvarchar(150)`| Nullable | Adayın ad soyad bilgisi |
| `ApplicantEmail`| `nvarchar(256)`| Nullable | Aday iletişim e-postası |
| `CoverLetter`| `nvarchar(max)` | Nullable | Aday ön yazısı / motivasyon mektubu |
| `ResumeUrl` | `nvarchar(500)` | Nullable | Özgeçmiş / CV / LinkedIn linki |
| `Status` | `nvarchar(50)` | Default: `'Beklemede'` | Beklemede, İncelendi, Mülakat, Kabul, Red |
| `AppliedAt` | `datetime2` | Not Null, Default: UTC Now | Başvuru zamanı |

---

## 3. Doğrulama ve Alan Kuralları (Validation & Domain Rules)

`JobPosting` ilan oluşturma (`POST /api/job-postings`) ve güncelleme (`PUT /api/job-postings/{id}`) işlemleri DataAnnotations ve Controller düzeyinde aşağıdaki kurallara göre doğrulanır; kural ihlallerinde **`400 Bad Request`** dönülür:

* **`title`**:
  * **Zorunlu (Required)**: Boş veya yalnızca boşluklardan oluşamaz (`[Required]`, `!string.IsNullOrWhiteSpace`).
  * **Maksimum Uzunluk**: En fazla **120 karakter** olabilir (`[StringLength(120)]`).
* **`company` (`CompanyId`)**:
  * **Zorunlu (Required)**: İlan bir şirkete bağlanmalıdır (`[Required]`, `[Range(1, int.MaxValue)]`). Veritabanında mevcut geçerli bir şirket olmalıdır; aksi takdirde `400 Bad Request` döner.
* **`city` / `Location`**:
  * **Opsiyonel (Optional)**: Belirtilmesi zorunlu değildir, boş bırakılabilir.
* **`description`**:
  * **Opsiyonel (Optional)**: Belirtilmesi zorunlu değildir, boş bırakılabilir.

---

## 4. Varlık İlişkileri (Relations & Integrity)

1. **Company ──< JobPosting (1:N):**
   * Bir şirketin birden fazla iş ilanı olabilir.
   * `OnDelete(DeleteBehavior.Restrict)`: İlanı olan bir şirket yanlışlıkla silinemez.
2. **JobPosting ──< JobApplication (1:N):**
   * Bir ilana birden fazla aday başvurabilir.
   * `OnDelete(DeleteBehavior.Cascade)`: Bir ilan silindiğinde ilişkili başvurular da temizlenir.
3. **User ──< JobApplication (1:N):**
   * Bir kullanıcı birden fazla farklı ilana başvuru yapabilir.

---

## 4. Doğrudan ve İç İçe Rota Mimarisi (Direct & Nested Routes)

### 🔹 Doğrudan Rotalar (Direct Routes)
Kaynaklara doğrudan erişim ve temel CRUD işlemleri sağlar:

* `GET /api/job-postings` → Tüm iş ilanlarını filtreleyerek getirir
* `GET /api/job-postings/{id}` → İlan detayını getirir
* `POST /api/job-postings` → Yeni ilan oluşturur
* `PUT /api/job-postings/{id}` → İlanı günceller
* `DELETE /api/job-postings/{id}` → İlanı siler
* `GET /api/applications` → Tüm başvuruları listeler
* `GET /api/applications/{id}` → Başvuru detayını döner
* `PATCH /api/applications/{id}/status` → Başvuru durumunu günceller (`Beklemede` -> `Kabul`)
* `DELETE /api/applications/{id}` → Başvuruyu geri çeker

### 🔹 İç İçe Rotalar (Nested Routes)
Üst-alt kaynak hiyerarşisini REST standartlarına uygun olarak modeller:

* **Şirkete göre ilanlar:**
  `GET /api/companies/{companyId}/job-postings`
* **İlana yapılan başvurular:**
  `GET /api/job-postings/{jobPostingId}/applications`
* **İlana yeni başvuru yapma:**
  `POST /api/job-postings/{jobPostingId}/applications`

---

## 5. Seed Data (Başlangıç Verileri)

* **Şirketler:** Aselsan, Baykar Teknoloji, Trendyol
* **İş İlanları:**
  1. *Kıdemli .NET Backend Mühendisi* (Aselsan)
  2. *Full Stack Web Geliştirici* (Trendyol)
  3. *Yazılım Stajyeri (2026 Yaz Dönemi)* (Baykar Teknoloji)

---

## 6. Arayüzler (UI Layer)

* **`/job-postings`** → İlan kartları, arama, filtreleme ve doğrudan başvuru arayüzü
* **`/job-postings/create`** → Şirket seçimiyle yeni iş ilanı oluşturma formu
* **`/job-postings/{id}/apply`** → Aday başvuru formu
* **`/applications`** → Alınan tüm başvuruların durum kartlarıyla listelendiği panel
