# PROJE KURALLARI VE STANDARTLARI

> **ÖNEMLİ:** Bu dosya projenin tüm geliştirme kurallarını ve standartlarını içerir. Her geliştirme yaparken bu kurallara uyulmalıdır.

---

## 📋 GENEL STANDARTLAR

### 1. Hata Yönetimi
- ✅ Service ve Forms'larda hata değişkeni **global tanımlanmamalı**
- ✅ Service'lerde: `string error = null;` şeklinde tanımlanıp, exception mesajı atanmalı
- ✅ Try-catch blokları doğru yazılmalı

### 2. İsimlendirme Kuralları

#### Class İsimleri
- ✅ **PascalCase** kullanılmalı (her kelimenin ilk harfi büyük)
- ✅ Örnek: `FrmHoiz`, `SCommon`, `IMusteri`
- ✅ Class isimleri ile dosya isimleri **birebir aynı** olmalı
  - Örnek: `SCommon` class'ı → `SCommon.cs` dosyası

#### Method İsimleri
- ✅ **PascalCase** kullanılmalı
- ✅ Örnek: `GetMusteriBilgi`, `MusteriEkle`, `MusteriBul`

#### Parametre İsimleri
- ✅ **camelCase** kullanılmalı (ilk harf küçük, sonraki kelimelerin ilk harfleri büyük)
- ✅ Örnek: `subeKod`, `opAdi`, `kisaAd`

#### Değişken İsimleri
- ✅ **Private değişkenler** class'ın **ilk başında** tanımlanmalı
- ✅ Property içinde kullanılan değişkenler **'_' karakteriyle başlamalı**
  - Örnek: `private decimal _bakiye`, `private int _adreskod`
- ✅ **Public değişkenler**: camelCase (kisaAd, sicilNo)
- ✅ **Exception değişkenleri**: `ex`, `ex1`, `ex2`

### 3. Kod Dokümantasyonu
- ✅ **Tüm method ve class'larda XML documentation** olmalı
- ✅ Format: `/// <summary>`, `/// <param>`, `/// <returns>`
- ✅ Örnek:
```csharp
/// <summary>
/// Data table'ı update eder
/// </summary>
/// <param name="ci">ClientInfo</param>
/// <param name="dt">DataTable</param>
/// <returns>String döner, hata yoksa null döner</returns>
```

### 4. Açıklama Satırları (Comments)
- ✅ Çok satırlı açıklamalar için:
```csharp
/*
 * Açıklama satırı (Created by ........., DD/MM/YYYY)
 * Açıklama satırı (Fonksiyonu, uyarılar)
 * Açıklama satırı (Edited by ........., DD/MM/YYYY, Neden edit edildiği)
 */
```
- ✅ Tek satırlı açıklamalar: `// ............` veya `/* */`

### 5. Girintili Yazma (Indentation)
- ✅ **4 space** kullanılmalı (.NET editor default)
- ✅ Örnek:
```csharp
for(int i=0; i<5; i++)
{
    // ...
}

if (a < b)
{
    // ...
}
```

### 6. Özel Fonksiyonlar
- ✅ Rapor dosyaları path'i: `CommonFunction.GetReportDirectoryPath`
- ✅ Şablon path'i: `CommonFunction.GetTemplateDirectoryPath`

---

## 📱 FORMS STANDARTLARI

### 1. Form İsimlendirme
- ✅ Format: `Modul[.AltModül].Forms.kisa_ad`
- ✅ Örnek: `Musteri.Kisi.Forms.kshvz`
- ✅ Helper class: `F[kisa_ad]` (Örnek: `FKshvz`)
- ✅ Design class: `Frm[kisa_ad]` (Örnek: `FrmKshvz`)

### 2. Form Özellikleri
- ✅ **Maksimum boyut: 770x700 piksel**
- ✅ **AutoScroll = true** (zorunlu)
- ✅ **Font: Tahoma**
- ✅ **Font-Size: 8.25**
- ✅ **Start Position: CenterScreen**

### 3. Form Görsel Tasarım
- ✅ **Info (readonly/disabled) alanlar**: `Web.LightYellow` arka plan
- ✅ **Labellar**: sağa veya sola yanaşık olabilir

### 4. Form İşlevsellik
- ✅ **Interface çağrılarında**: `if(error != null)` kontrolü **zorunlu**
- ✅ **DML işlemleri**: `DMLManager` kullanılmalı
- ✅ **DataGridView double-click**: Düzeltme/Edit işlemi açmalı
- ✅ **Direkt SQL yasak**: Formlarda direkt SQL sorgusu yazılmamalı
- ✅ **DLL Referansları**: Service, Business, Util.DataAccess DLL'leri direkt referans edilmemeli

### 5. Form Kontrolleri
- ✅ **Kontrol isimleri**: İlk harf büyük olmalı (Örnek: 'Sorgula', 'Arama Yap')
- ✅ **Sadece User Control (uc) kullanılmalı**

### 6. Form Versiyonlama
- ✅ **Assembly ve dosya versiyonları** verilmeli
- ✅ **kul_ekran tablosuna** kayıt yaparken versiyon belirtilmeli
- ✅ **Form Text property**: `kul_ekran.menudeki_adi` ile aynı olmalı
- ✅ **kul_ekran.menudeki_adi**: Büyük harfle başlayıp küçük devam etmeli (Örnek: "Anasayfa")

### 7. İkonlar
- ✅ **Sadece ortak (standart) ikonlar** kullanılmalı

---

## 🔧 SERVICE STANDARTLARI

### 1. Service İsimlendirme
- ✅ Format: `Modul.Service` (Örnek: `Common.Service`)
- ✅ Class isimleri **'S' ile başlamalı** (Örnek: `SCommon`)

### 2. Service Yapısı
- ✅ **sMan** object'i **using** ile kullanılmalı
- ✅ **Tüm metodlar string döndürmeli**
- ✅ **Class seviyesinde değişken tanımlanmamalı** (tüm değişkenler method içinde)

### 3. Hata Yönetimi
- ✅ `string error = null;` şeklinde tanımlanmalı
- ✅ Exception mesajı error'a atanmalı
- ✅ Try-catch blokları doğru yazılmalı

### 4. Stored Procedure (SP) Kullanımı
- ✅ **sMan.ExecuteSP kullanılmamalı**
- ✅ **SP DLL** SPBuilder'dan oluşturulmalı

### 5. Service Katmanı Mantığı
- ✅ Client tarafından ilgili modülle alakalı istekleri karşılayan katman
- ✅ **Başka servislerden kullanılacaksa**: İçerik Business Object katmanında yazılıp buradan çağrılmalı
- ✅ **Sadece bu servise özel ise**: Doğrudan bu katmanda geliştirilebilir

---

## 🔌 INTERFACE STANDARTLARI

### 1. Interface İsimlendirme
- ✅ Format: `Modul.Interface`
- ✅ Class isimleri **'I' ile başlamalı** (Örnek: `IMusteri`)

---

## 🎨 KONTROL İSİMLENDİRME STANDARTLARI

### Standart Windows Forms Kontrolleri
- ✅ **Label**: `lblAd`, `lblSoyad`, `lblSubeAd`
- ✅ **LinkLabel**: `llbAd`, `llbSoyad`
- ✅ **Button**: `btnKaydet`, `btnDuzelt`, `btnSil`, `btnKapat`
- ✅ **TextBox**: `txtAd`, `txtSoyad`, `txtSubeAd`
- ✅ **CheckBox**: `chkSpor`, `chkKultur`
- ✅ **RadioButton**: `rbtnEvli`, `rbtnBekar`
- ✅ **GroupBox**: `grpMedeniHal`
- ✅ **Panel**: `pnlKimlik`, `pnlAdres`
- ✅ **ListBox**: `lstKategoriTip`
- ✅ **ComboBox**: `cmbSubeAd`, `cmbAdresKod`
- ✅ **ListView**: `lviewGorusme`
- ✅ **TreeView**: `tviewOrganizasyon`
- ✅ **TabControl**: `tabMusteriTanim`
- ✅ **DateTimePicker**: `dtpTarih`, `dtpIseGirisTarih`
- ✅ **DataGridView**: `grdSube`
- ✅ **RichTextBox**: `rtxtAciklama`
- ✅ **ProgressBar**: `progbarFileUpload`
- ✅ **Timer**: `timerKayit`
- ✅ **ErrorProvider**: `eprvSubeKod`

### DevExpress Kontrolleri
- ✅ **TextEdit**: `txtAd`
- ✅ **DateEdit**: `dateDogumTarih`
- ✅ **LookUpEdit**: `lueSubeKod`
- ✅ **MemoEdit**: `memoAciklama`
- ✅ **SpinEdit**: `spinAdres`
- ✅ **CheckEdit**: `chkFutbol`, `chkBasketbol`
- ✅ **ComboBoxEdit**: `cmbSubeKod`
- ✅ **SimpleButton**: `btnKaydet`
- ✅ **GridControl**: `grdParametre`
- ✅ **GridView**: `grdwParametre`
- ✅ **BarManager**: `barmngMuhasebe`
- ✅ **NavBarControl**: `navbarMuhasebe`
- ✅ **XtraTabControl**: `xtabMusteri`

---

## 📦 NAMESPACE STANDARTLARI

### Format
- ✅ `Firat.Modul.Forms`
- ✅ `Firat.Modul.Service`
- ✅ `Firat.Modul.Interface`
- ✅ `Firat.Modul.Business`
- ✅ `Firat.Modul.SP`
- ✅ `Firat.Modul.Helper`

### Örnekler
- `Firat.Musteri.Forms`
- `Firat.Musteri.Service`
- `Firat.Musteri.Interface`
- `Firat.Musteri.Business`
- `Firat.Hesap.Forms`
- `Firat.Hesap.Genel.Service`
- `Firat.Hesap.Detay.Service`

---

## 💾 VERİTABANI VE ORACLE STANDARTLARI

### Oracle .NET Provider Değişken İsimleri
- ✅ **OracleConnection**: `conn`
- ✅ **OracleCommand**: `cmd`
- ✅ **OracleTransaction**: `trans`
- ✅ **OracleParameter**: `prm`
- ✅ **OracleDataAdapter**: `da`
- ✅ **OracleDataReader**: `dr`
- ✅ **OracleCommandBuilder**: `cb`

### System.Data Değişken İsimleri
- ✅ **DataSet**: `ds`, `dsEkran`, `dsOperator`
- ✅ **DataTable**: `dt`, `dtEkran`, `dtOperator`
- ✅ **DataView**: `dv`, `dvEkran`, `dvOperator`
- ✅ **DataRow**: `drow`, `drowKisi`, `drowOperator`
- ✅ **DataColumn**: `dcol`, `dcolSubeKod`, `dcolSubeAd`

### Dataset/Datatable Standartları
- ✅ **Mümkün mertebe parametrelerle geçilmeli** (size yüksek)
- ✅ **Client'e gönderilen DataTable'larda**: `rownum<100` kontrolü olmalı

---

## 🏗️ SPOBJECT KATMANI STANDARTLARI

### 1. Amaç
- ✅ Orta katmanda kullanılan SP'leri çağıran class'lar bu katmanda yazılır

### 2. Özel Durumlar
- ✅ .Net-Oracle type mismatch nedeniyle, Oracle'dan `rowtype` veya özel tip dönen SP'lerin **body**'si bu katmanda yazılır
- ✅ Bu SP'lerin **SELECT** kısmı .Net tarafında yapılır

### 3. İsimlendirme
- ✅ İsimler **birebir aynı** olmalı (database'deki ile)

### 4. Connection
- ✅ **OracleConnection bu katmanda açılmamalı**
- ✅ Connection bilgisi parametre olarak çağıran katmandan (Service veya Business Object) gelmelidir

### 5. Proje Yapısı
- ✅ Her modül **ayrı proje** olmalı
- ✅ Her package **ayrı .cs dosyası** (class) olmalı
- ✅ Örnek yapı:
```
SPRating/
  Properties/
  References/
  p_rating.cs
  t_dml_rating_delete.cs
  t_dml_rating_insert.cs
  t_dml_rating_update.cs
  t_rating.cs
```

---

## 📝 KODLAMA STANDARTLARI

### 1. String Concatenation
- ✅ **StringBuilder kullanılmalı** (string class yerine)
- ❌ **Yanlış:**
```csharp
string sonuc;
for (int i=0; i<10; i++)
{
    sonuc += i.ToString();
}
```
- ✅ **Doğru:**
```csharp
StringBuilder sonuc = new StringBuilder();
for (int i=0; i<10; i++)
{
    sonuc.Append(i.ToString());
}
```

### 2. Sayısal Değerler
- ✅ Hesap No, Kisino, Vergi No gibi sayısal değerlerde **long tipi** kullanılmalı

### 3. For Döngüsü Counter'ları
- ✅ `i`, `j`, `k` kullanılmalı

### 4. Class Nesneleri
- ✅ **OpenFileDialog**: `ofdUpload`, `ofdDownload`
- ✅ **PrintDialog**: `pdDekont`, `pdFis`

### 5. User Control Standartları
- ✅ **Property/Metot isimleri 'X' ile başlamalı** (IntelliSense için)
- ✅ **Veritabanı kontrolleri**: `xEkranParam` property'si olmalı
- ✅ **Property set eden metot**: `xSetParams` isimli olmalı
- ✅ **Value property**: `xValue` isimli olmalı (set edildiğinde Text de değişmeli)

---

## 📊 CLASS İSİMLENDİRME ÖRNEKLERİ

### Musteri Modülü Örneği
- ✅ **Form Helper**: `FMusteri`
- ✅ **Service**: `SMusteri`
- ✅ **Interface**: `IMusteri`
- ✅ **Business**: `BMusteri`
- ✅ **SP**: `SpMusteri` (database'deki package name'leri ile aynı: T_MUSTERI, P_MUSTERI)
- ✅ **Helper**: `HMusteri`

---

## ⚠️ ÖNEMLİ HATIRLATMALAR

1. ✅ **Service ve Forms'larda hata değişkeni global tanımlanmamalı**
2. ✅ **Form boyutu max 770x700**
3. ✅ **AutoScroll = true** (tüm formlarda)
4. ✅ **Font: Tahoma 8.25pt**
5. ✅ **Interface çağrılarında if(error!=null) kontrolü zorunlu**
6. ✅ **DML işlemleri için DMLManager kullanılmalı**
7. ✅ **Direkt SQL formlarda yasak**
8. ✅ **String concatenation için StringBuilder kullanılmalı**
9. ✅ **XML documentation zorunlu**
10. ✅ **Class isimleri ile dosya isimleri aynı olmalı**

---

**Son Güncelleme:** 2025-01-XX
**Versiyon:** 1.0



