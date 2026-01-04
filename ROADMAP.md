# 🗺️ Apartman Yönetim Sistemi - Geliştirme Roadmap

## 📋 Genel Bakış
Bu roadmap, rol bazlı ekranların geliştirilmesi için adım adım plan içerir.

---

## 🎯 FAZE 1: YÜKSEK ÖNCELİK (Temel İşlevsellik)

### 1.1 Resident Dashboard (Sakin Paneli)
**Hedef:** Sakinler için özel dashboard ekranı
**Roller:** Resident
**Özellikler:**
- Kişisel bilgiler özeti
- Daire bilgileri kartı
- Son ödemeler özeti
- Bekleyen aidatlar
- Son duyurular
- Hızlı işlemler butonları

**Dosyalar:**
- `FrmResidentDashboard.cs` (Yeni)
- `FrmMainLayout.cs` (Güncelleme - Resident için dashboard gösterimi)

**Tahmini Süre:** 2-3 saat

---

### 1.2 Kişisel Bilgilerim (Profil Ekranı)
**Hedef:** Tüm kullanıcılar için profil yönetim ekranı
**Roller:** Tüm roller (SuperAdmin, Admin, SiteManager, ApartmentManager, Resident)
**Özellikler:**
- Kişisel bilgileri görüntüleme/düzenleme
- Şifre değiştirme
- Acil durum bilgileri
- Bildirim tercihleri (Email, SMS)
- Profil fotoğrafı (opsiyonel)

**Dosyalar:**
- `FrmProfile.cs` (Yeni)
- `FrmMainLayout.cs` (Güncelleme - Ayarlar menüsüne ekleme)

**Tahmini Süre:** 2-3 saat

---

### 1.3 Aidat Yönetimi
**Hedef:** SiteManager ve ApartmentManager için aidat yönetimi
**Roller:** SiteManager, ApartmentManager
**Özellikler:**
- Aidat listesi (filtreleme: Site/Apartman, Tarih, Durum)
- Yeni aidat oluşturma
- Aidat düzenleme/silme
- Toplu aidat oluşturma
- Aidat özet istatistikleri

**Dosyalar:**
- `FrmDuesList.cs` (Yeni)
- `FrmDuesManagement.cs` (Yeni)
- `FrmMainLayout.cs` (Güncelleme - Menüye ekleme)

**Tahmini Süre:** 4-5 saat

---

### 1.4 Ödeme Takibi
**Hedef:** Tüm roller için ödeme takibi (rol bazlı filtreleme)
**Roller:** 
- **Resident:** Sadece kendi ödemeleri
- **ApartmentManager:** Apartmanındaki tüm ödemeler
- **SiteManager:** Site'deki tüm ödemeler
- **Admin/SuperAdmin:** Tüm ödemeler

**Özellikler:**
- Ödeme listesi (filtreleme: Tarih, Durum, Daire, Tip)
- Ödeme detayları
- Ödeme onaylama (Manager rolleri için)
- Ödeme geçmişi
- Ödeme istatistikleri

**Dosyalar:**
- `FrmPaymentList.cs` (Yeni)
- `FrmPaymentDetail.cs` (Yeni)
- `FrmMainLayout.cs` (Güncelleme - Menüye ekleme)

**Tahmini Süre:** 4-5 saat

---

### 1.5 Ayarlar Ekranı
**Hedef:** Tüm roller için genel ayarlar
**Roller:** Tüm roller
**Özellikler:**
- Bildirim ayarları (Email, SMS)
- Dil tercihi (Türkçe/İngilizce)
- Tema ayarları (Açık/Koyu)
- Şifre değiştirme (hızlı erişim)
- Sistem bildirimleri

**Dosyalar:**
- `FrmSettings.cs` (Yeni)
- `FrmMainLayout.cs` (Güncelleme - Ayarlar menüsünü aktif etme)

**Tahmini Süre:** 2-3 saat

---

## 🎯 FAZE 2: ORTA ÖNCELİK (İş Mantığı)

### 2.1 Gider Yönetimi
**Hedef:** SiteManager ve ApartmentManager için gider yönetimi
**Roller:** SiteManager, ApartmentManager
**Özellikler:**
- Gider listesi (filtreleme: Tarih, Tip, Site/Apartman)
- Yeni gider ekleme
- Gider düzenleme/silme
- Gider kategorileri
- Gider raporları

**Dosyalar:**
- `FrmExpenseList.cs` (Yeni)
- `FrmExpenseManagement.cs` (Yeni)
- `FrmMainLayout.cs` (Güncelleme - Menüye ekleme)

**Tahmini Süre:** 3-4 saat

---

### 2.2 Duyuru Yönetimi
**Hedef:** SiteManager ve ApartmentManager için duyuru sistemi
**Roller:** 
- **SiteManager:** Site duyuruları
- **ApartmentManager:** Apartman duyuruları
- **Resident:** Duyuruları görüntüleme

**Özellikler:**
- Duyuru listesi
- Yeni duyuru oluşturma
- Duyuru düzenleme/silme
- Duyuru kategorileri
- Önemli duyurular (pin)
- Duyuru okundu işaretleme

**Dosyalar:**
- `FrmAnnouncementList.cs` (Yeni)
- `FrmAnnouncementManagement.cs` (Yeni)
- `FrmMainLayout.cs` (Güncelleme - Menüye ekleme)

**Tahmini Süre:** 3-4 saat

---

### 2.3 Site/Apartman İstatistikleri
**Hedef:** İlgili roller için istatistik dashboard'ları
**Roller:** 
- **SiteManager:** Site istatistikleri
- **ApartmentManager:** Apartman istatistikleri

**Özellikler:**
- Genel istatistik kartları (Toplam Daire, Dolu/Boş, Toplam Sakin)
- Aidat özeti (Tahsilat oranı, Bekleyen tutar)
- Gider özeti (Aylık giderler, Kategorilere göre)
- Grafikler (Aidat trendi, Gider dağılımı)
- Son aktiviteler

**Dosyalar:**
- `FrmSiteStatistics.cs` (Yeni)
- `FrmApartmentStatistics.cs` (Yeni)
- `FrmMainLayout.cs` (Güncelleme - Dashboard'a link ekleme)

**Tahmini Süre:** 4-5 saat

---

### 2.4 Şikayet/Talep Sistemi
**Hedef:** Resident için şikayet/talep oluşturma ve takip
**Roller:** 
- **Resident:** Şikayet/talep oluşturma ve görüntüleme
- **SiteManager/ApartmentManager:** Şikayet/talepleri yönetme

**Özellikler:**
- Şikayet/talep listesi
- Yeni şikayet/talep oluşturma
- Şikayet/talep detayları
- Durum takibi (Beklemede, İnceleniyor, Çözüldü)
- Yorum ekleme
- Dosya ekleme (fotoğraf, belge)

**Dosyalar:**
- `FrmComplaintList.cs` (Yeni)
- `FrmComplaintManagement.cs` (Yeni)
- `FrmMainLayout.cs` (Güncelleme - Menüye ekleme)

**Tahmini Süre:** 4-5 saat

---

## 🎯 FAZE 3: DÜŞÜK ÖNCELİK (Gelişmiş Özellikler)

### 3.1 Raporlar/Analitik
**Hedef:** Admin için detaylı raporlar
**Roller:** SuperAdmin, Admin
**Özellikler:**
- Finansal raporlar (Aidat, Gider, Gelir-Gider analizi)
- Kullanıcı raporları (Aktif kullanıcılar, Yeni kayıtlar)
- Site/Apartman performans raporları
- PDF/Excel export
- Tarih aralığı filtreleme

**Dosyalar:**
- `FrmReports.cs` (Yeni)
- `FrmMainLayout.cs` (Güncelleme - Menüye ekleme)

**Tahmini Süre:** 5-6 saat

---

### 3.2 Sistem Logları
**Hedef:** SuperAdmin için sistem aktivite logları
**Roller:** SuperAdmin
**Özellikler:**
- Kullanıcı aktivite logları
- Sistem hata logları
- Giriş/Çıkış logları
- İşlem geçmişi
- Log filtreleme ve arama

**Dosyalar:**
- `FrmSystemLogs.cs` (Yeni)
- `FrmMainLayout.cs` (Güncelleme - Menüye ekleme)

**Tahmini Süre:** 3-4 saat

---

## 📊 İLERLEME TAKİBİ

### Tamamlanan
- ✅ Temel CRUD ekranları (Site, Block, Apartment, Flat, User)
- ✅ Onay Bekleyenler ekranı
- ✅ Ana Layout ve Sidebar

### Devam Eden
- ⏳ Faze 1 başlatılmayı bekliyor

### Planlanan
- 📅 Faze 1: Yüksek Öncelik (5 ekran)
- 📅 Faze 2: Orta Öncelik (4 ekran)
- 📅 Faze 3: Düşük Öncelik (2 ekran)

---

## 🎯 TOPLAM TAHMİNİ SÜRE

- **Faze 1:** ~15-19 saat
- **Faze 2:** ~14-18 saat
- **Faze 3:** ~8-10 saat
- **TOPLAM:** ~37-47 saat

---

## 📝 NOTLAR

1. Her faz tamamlandıktan sonra test edilecek
2. Roller bazlı erişim kontrolü her ekranda uygulanacak
3. Tüm ekranlar mevcut tasarım standartlarına uygun olacak
4. Database migration'ları gerekli entity'ler için oluşturulacak

---

## 🚀 BAŞLATMA

Roadmap hazır! "başla" yazdığınızda Faze 1.1 (Resident Dashboard) ile başlayacağız.

