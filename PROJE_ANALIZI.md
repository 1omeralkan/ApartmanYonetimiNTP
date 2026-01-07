# APARTMAN YÖNETİM SİSTEMİ - PROJE ANALİZİ

## 📋 PROJE GENEL BAKIŞ

Bu proje, **Apartman Yönetim Sistemi** adında bir Windows Forms uygulamasıdır. Sistem, site, blok, apartman, daire ve kullanıcı yönetimi yapabilen, rol tabanlı erişim kontrolü olan bir yönetim sistemidir.

---

## 🏗️ MİMARİ YAPISI

Proje **3-Katmanlı Mimari (3-Layer Architecture)** kullanılarak geliştirilmiştir:

### 1. **ApartmentManagement.DataAccess** (Veri Erişim Katmanı)
- **Teknoloji**: Entity Framework Core 8.0
- **Veritabanı**: PostgreSQL (Npgsql.EntityFrameworkCore.PostgreSQL 8.0.0)
- **Görev**: Veritabanı işlemleri, Entity tanımları, Migration'lar
- **İçerik**:
  - `ApartmentManagementContext.cs` - DbContext sınıfı
  - `Entities/` - Entity sınıfları (User, Site, Block, Apartment, Flat, Payment, Dues, Expense, FlatResident)
  - `Migrations/` - EF Core migration dosyaları

### 2. **ApartmentManagement.Business** (İş Mantığı Katmanı)
- **Teknoloji**: .NET 8.0 Class Library
- **Görev**: İş kuralları, servisler, interface'ler
- **İçerik**:
  - `Interfaces/` - Service interface'leri (IAuth, IUser, ISite, IBlock, IApartment, IFlat)
  - `Services/` - Service implementasyonları (SAuth, SUser, SSite, SBlock, SApartment, SFlat)
  - `Helpers/` - Yardımcı sınıflar (PasswordHelper)

### 3. **ApartmentManagement.WinFormUI** (Sunum Katmanı)
- **Teknoloji**: .NET 8.0 Windows Forms
- **UI Framework**: DevExpress WinForms (v25.1.3)
- **Görev**: Kullanıcı arayüzü, formlar, kullanıcı etkileşimleri
- **İçerik**:
  - Login/Register formları
  - Ana layout (Dashboard)
  - CRUD formları (Site, Block, Apartment, Flat, User yönetimi)
  - Helper sınıflar (Swal - SweetAlert benzeri, RoundedPanel)

---

## 🛠️ KULLANILAN TEKNOLOJİLER

### Backend Teknolojileri
- **.NET 8.0** - Ana framework
- **C#** - Programlama dili
- **Entity Framework Core 8.0** - ORM
- **PostgreSQL** - Veritabanı
- **Npgsql.EntityFrameworkCore.PostgreSQL 8.0.0** - EF Core PostgreSQL provider

### Frontend/UI Teknolojileri
- **Windows Forms** - UI framework
- **DevExpress WinForms 25.1.3** - Premium UI kontrol kütüphanesi
- **System.Windows.Forms.DataVisualization** - Grafik/chart desteği
- **System.Data.SqlClient 4.9.0** - SQL bağlantı desteği (muhtemelen eski kod için)

### Güvenlik
- **SHA-256** - Şifre hashleme (PasswordHelper)
- **Rol Tabanlı Erişim Kontrolü (RBAC)** - 5 farklı rol:
  - SuperAdmin
  - Admin
  - SiteManager
  - ApartmentManager
  - Resident

---

## 📊 VERİ MODELİ (Entity Yapısı)

### Ana Entity'ler:
1. **User** - Kullanıcı bilgileri (Ad, Soyad, Email, Telefon, TC No, Rol, vb.)
2. **Site** - Site bilgileri (İsim, Adres, Kod, Durum, İstatistikler)
3. **Block** - Blok bilgileri (Site'ye bağlı)
4. **Apartment** - Apartman bilgileri (Blok'a bağlı, Kat sayısı, Daire sayısı, vb.)
5. **Flat** - Daire bilgileri (Kapı no, Kat, Tip, Boş/Dolu durumu)
6. **FlatResident** - Daire sakinleri (User-Flat ilişkisi, Ev sahibi/Kiracı)
7. **Payment** - Ödemeler (Aidat, Demirbaş, vb.)
8. **Dues** - Aidat kayıtları (Aylık aidat takibi)
9. **Expense** - Giderler (Site/Blok/Apartman bazlı)

### İlişkiler:
- Site → Blocks (1-N)
- Block → Apartments (1-N)
- Apartment → Flats (1-N)
- Flat → FlatResidents (1-N)
- FlatResident → User (N-1)
- Flat → Payments (1-N)
- Flat → Dues (1-N)

---

## 🎨 UI STANDARTLARI (Kod İçinde Belirtilen)

Proje kodlarında şu standartlar belirtilmiş:

### Form Standartları:
- **Font**: Tahoma 8.25pt
- **Maksimum Boyut**: 770x700 piksel
- **AutoScroll**: true (tüm formlarda)
- **Form Border**: FixedSingle veya Sizable (form tipine göre)
- **Start Position**: CenterScreen

### Renk Paleti (FrmMainLayout):
- **Primary Dark**: Slate 900 (#0F172A)
- **Secondary Dark**: Slate 800 (#1E293B)
- **Accent Colors**: Blue, Green, Yellow, Red, Purple
- **Text Colors**: Slate 50 (Primary), Slate 400 (Secondary)
- **Background**: Slate 100 (#F1F5F9)

### DevExpress Ayarları:
- **Skin**: WXI (Windows 11 Premium Theme)
- **Form Skins**: Enabled

---

## 📁 PROJE YAPISI

```
NTP_Proje/
├── ApartmentManagement.sln
├── ApartmentManagement.DataAccess/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Site.cs
│   │   ├── Block.cs
│   │   ├── Apartment.cs
│   │   ├── Flat.cs
│   │   ├── FlatResident.cs
│   │   ├── Payment.cs
│   │   ├── Dues.cs
│   │   └── Expense.cs
│   ├── Migrations/
│   └── ApartmentManagementContext.cs
├── ApartmentManagement.Business/
│   ├── Interfaces/
│   │   ├── IAuth.cs
│   │   ├── IUser.cs
│   │   ├── ISite.cs
│   │   ├── IBlock.cs
│   │   ├── IApartment.cs
│   │   └── IFlat.cs
│   ├── Services/
│   │   ├── SAuth.cs
│   │   ├── SUser.cs
│   │   ├── SSite.cs
│   │   ├── SBlock.cs
│   │   ├── SApartment.cs
│   │   └── SFlat.cs
│   └── Helpers/
│       └── PasswordHelper.cs
├── ApartmentManagement.WinFormUI/
│   ├── FrmLogin.cs
│   ├── FrmRegister.cs
│   ├── FrmMainLayout.cs (Dashboard)
│   ├── FrmUserList.cs
│   ├── FrmUserManagement.cs
│   ├── FrmSiteManagement.cs (Yakında)
│   ├── FrmBlockManagement.cs
│   ├── FrmApartmentList.cs
│   ├── FrmApartmentManagement.cs
│   ├── FrmFlatList.cs
│   ├── FrmFlatManagement.cs
│   ├── Helpers/
│   │   ├── Swal.cs (SweetAlert benzeri)
│   │   └── RoundedPanel.cs
│   └── Program.cs
├── Net Uygulama Geliştirme Kontrol Listesi.pdf
└── net Uygulama Geliştirme Standartları.pdf
```

---

## 🔐 GÜVENLİK ÖZELLİKLERİ

1. **Şifre Hashleme**: SHA-256 ile şifre hashleme
2. **Email Unique**: Email adresleri unique index ile korunuyor
3. **Rol Tabanlı Erişim**: Her kullanıcının rolüne göre menü ve yetkiler
4. **Admin Seed**: İlk çalıştırmada otomatik admin kullanıcısı oluşturuluyor
   - Email: admin@gmail.com
   - Şifre: 123

---

## 🚀 ÖZELLİKLER

### Mevcut Özellikler:
- ✅ Kullanıcı girişi/kayıt sistemi
- ✅ Rol tabanlı dashboard
- ✅ Site yönetimi (CRUD)
- ✅ Blok yönetimi (CRUD)
- ✅ Apartman yönetimi (CRUD)
- ✅ Daire yönetimi (CRUD)
- ✅ Kullanıcı yönetimi (CRUD)
- ✅ Dashboard istatistikleri
- ✅ Modern UI (DevExpress WXI skin)
- ✅ SweetAlert benzeri bildirimler (Swal)
- ✅ Otomatik veritabanı migration

### Yakında Eklenecekler (Kod içinde belirtilen):
- ⏳ Site yönetimi modülü (UI)
- ⏳ Şifre sıfırlama özelliği
- ⏳ Ayarlar modülü

---

## 📝 PDF DOSYALARI HAKKINDA

Proje kök dizininde 2 adet PDF dosyası bulunmaktadır:
1. **Net Uygulama Geliştirme Kontrol Listesi.pdf** - Proje kontrol listesi
2. **net Uygulama Geliştirme Standartları.pdf** - Geliştirme standartları

Bu dosyalar projenin geliştirme kurallarını ve standartlarını içermektedir. PDF formatında oldukları için doğrudan metin olarak okunamazlar, ancak kod içinde belirtilen standartlardan bazıları:
- Tahoma 8.25pt font kullanımı
- Maksimum 770x700 form boyutu
- AutoScroll = true
- DevExpress kullanımı
- 3-katmanlı mimari

---

## 🔧 VERİTABANI BAĞLANTISI

**ApartmentManagementContext.cs** içinde:
```csharp
Host=localhost
Database=ApartmentManagementDb
Username=postgres
Password=1Sjklmn90.
```

⚠️ **Güvenlik Notu**: Şifre kod içinde hardcoded. Production'da appsettings.json veya environment variable kullanılmalı.

---

## 📦 NUGET PAKETLERİ

### DataAccess:
- Microsoft.EntityFrameworkCore (8.0.0)
- Microsoft.EntityFrameworkCore.Tools (8.0.0)
- Npgsql.EntityFrameworkCore.PostgreSQL (8.0.0)

### WinFormUI:
- DevExpress.Win (25.1.3)
- Microsoft.EntityFrameworkCore.Design (8.0.0)
- System.Data.SqlClient (4.9.0)
- System.Windows.Forms.DataVisualization (1.0.0-prerelease)

---

## 🎯 PROJE AMACI

Bu sistem, apartman yönetim şirketleri veya site yöneticileri için:
- Site, blok, apartman ve daire yönetimi
- Kullanıcı (sakin) yönetimi
- Ödeme ve aidat takibi
- Gider yönetimi
- Rol bazlı yetkilendirme

gibi işlevleri sağlamak için geliştirilmiştir.

---

## 📌 ÖNEMLİ NOTLAR

1. **Migration**: Program.cs içinde otomatik migration uygulanıyor
2. **Admin Seed**: İlk çalıştırmada admin kullanıcısı otomatik oluşturuluyor
3. **Exception Handling**: Servislerde try-catch blokları kullanılıyor
4. **UI Framework**: DevExpress kullanıldığı için lisans gerektirebilir
5. **Standartlar**: PDF dosyalarında detaylı standartlar olmalı, kod içinde bazı standartlar belirtilmiş

---

## 🔍 İYİLEŞTİRME ÖNERİLERİ

1. **Güvenlik**: 
   - Connection string'i appsettings.json'a taşı
   - Şifre hashleme için BCrypt veya Argon2 kullan (SHA-256 yerine)
   
2. **Mimari**:
   - Dependency Injection ekle
   - Repository Pattern kullan
   
3. **Kod Kalitesi**:
   - Unit testler ekle
   - XML documentation tamamla
   - Error logging ekle

4. **UI/UX**:
   - Responsive tasarım iyileştirmeleri
   - Loading indicators
   - Form validasyonları güçlendir

---

**Analiz Tarihi**: 2025-01-XX
**Proje Versiyonu**: 1.0
**.NET Versiyonu**: 8.0






