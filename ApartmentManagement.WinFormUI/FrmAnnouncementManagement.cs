#nullable disable
// FrmAnnouncementManagement.cs
// Duyuru Yönetim Formu - Duyuru ekleme ve düzenleme
// Standart: Tahoma 8.25pt, AutoScroll = true
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Duyuru yönetim formu - Duyuru ekleme ve düzenleme
    /// </summary>
    public partial class FrmAnnouncementManagement : DevExpress.XtraEditors.XtraForm
    {
        private IAnnouncement _announcementService;
        private IApartment _apartmentService;
        private ISite _siteService;
        private IFlat _flatService;
        private Announcement _currentAnnouncement;
        private User _currentUser;
        private bool _isEditMode;

        // Controls
        private ComboBoxEdit cmbSite;
        private ComboBoxEdit cmbApartment;
        private TextEdit txtTitle;
        private ComboBoxEdit cmbCategory;
        private MemoEdit txtContent;
        private CheckEdit chkIsPinned;
        private CheckEdit chkIsActive;
        private DateEdit dtExpiryDate;
        private SimpleButton btnSave;
        private SimpleButton btnCancel;

        /// <summary>
        /// FrmAnnouncementManagement constructor
        /// </summary>
        public FrmAnnouncementManagement(Announcement announcement, User user)
        {
            _currentAnnouncement = announcement;
            _currentUser = user;
            _isEditMode = announcement != null;
            _announcementService = new SAnnouncement();
            _apartmentService = new SApartment();
            _siteService = new SSite();
            _flatService = new SFlat();
            InitializeComponent();
            LoadFilters();
            if (_isEditMode)
            {
                LoadData();
            }
            else
            {
                // Yeni duyuru için varsayılan değerler
                chkIsActive.Checked = true;
                chkIsPinned.Checked = false;
            }
        }

        /// <summary>
        /// Form bileşenlerini başlatır
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = _isEditMode ? "Duyuru Düzenle" : "Yeni Duyuru Ekle";
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            int formHeight = Math.Min((int)(screenHeight * 0.8), 800);
            this.ClientSize = new Size(900, formHeight);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            int leftMargin = 40;
            int rightMargin = 40;
            int topMargin = 25;
            int currentY = topMargin;
            int contentWidth = this.Width - leftMargin - rightMargin;
            int fieldHeight = 32;

            // ========== HEADER SECTION ==========
            var lblTitle = new LabelControl();
            lblTitle.Text = _isEditMode ? "Duyuru Düzenle" : "Yeni Duyuru Ekle";
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblTitle);

            var lblSubtitle = new LabelControl();
            lblSubtitle.Text = _isEditMode ? "Duyuru bilgilerini düzenleyin" : "Yeni bir duyuru oluşturun";
            lblSubtitle.Appearance.Font = new Font("Tahoma", 9F);
            lblSubtitle.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            lblSubtitle.Location = new Point(leftMargin, currentY + 30);
            this.Controls.Add(lblSubtitle);

            var btnBack = new SimpleButton();
            btnBack.Text = "Geri Dön";
            btnBack.Size = new Size(110, 36);
            btnBack.Location = new Point(this.Width - rightMargin - 110, currentY + 5);
            btnBack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBack.Appearance.Font = new Font("Tahoma", 9F);
            btnBack.Appearance.BackColor = Color.FromArgb(241, 245, 249);
            btnBack.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            btnBack.Appearance.BorderColor = Color.FromArgb(226, 232, 240);
            btnBack.Appearance.Options.UseBackColor = true;
            btnBack.Appearance.Options.UseForeColor = true;
            btnBack.Appearance.Options.UseBorderColor = true;
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnBack);

            currentY += 75;

            // ========== FORM PANEL ==========
            var pnlForm = new Helpers.RoundedPanel();
            pnlForm.Size = new Size(contentWidth, 650);
            pnlForm.Location = new Point(leftMargin, currentY);
            pnlForm.BackColor = Color.White;
            pnlForm.BorderRadius = 12;
            pnlForm.BorderThickness = 1;
            pnlForm.BorderColor = Color.FromArgb(226, 232, 240);
            pnlForm.Padding = new Padding(35);
            this.Controls.Add(pnlForm);

            int panelY = 30;
            int panelLeft = 35;

            // ========== DUYURU BİLGİLERİ SECTION ==========
            var lblSectionAnnouncement = new LabelControl();
            lblSectionAnnouncement.Text = "Duyuru Bilgileri";
            lblSectionAnnouncement.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblSectionAnnouncement.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblSectionAnnouncement.Location = new Point(panelLeft, panelY);
            pnlForm.Controls.Add(lblSectionAnnouncement);
            panelY += 40;

            // Site (SiteManager için)
            if (_currentUser?.Role == "SiteManager" || _currentUser?.Role == "SuperAdmin" || _currentUser?.Role == "Admin")
            {
                AddFieldLabel(pnlForm, "Site", panelLeft, panelY, false);
                panelY += 20;
                cmbSite = new ComboBoxEdit();
                cmbSite.Location = new Point(panelLeft, panelY);
                cmbSite.Size = new Size(contentWidth - 70, fieldHeight);
                cmbSite.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                cmbSite.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
                cmbSite.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
                cmbSite.Properties.NullText = "Site seçiniz (Opsiyonel)";
                cmbSite.SelectedIndexChanged += CmbSite_SelectedIndexChanged;
                pnlForm.Controls.Add(cmbSite);
                panelY += 45;
            }

            // Apartment (ApartmentManager için otomatik, SiteManager için seçilebilir)
            if (_currentUser?.Role == "SiteManager" || _currentUser?.Role == "SuperAdmin" || _currentUser?.Role == "Admin")
            {
                AddFieldLabel(pnlForm, "Apartman", panelLeft, panelY, false);
                panelY += 20;
                cmbApartment = new ComboBoxEdit();
                cmbApartment.Location = new Point(panelLeft, panelY);
                cmbApartment.Size = new Size(contentWidth - 70, fieldHeight);
                cmbApartment.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                cmbApartment.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
                cmbApartment.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
                cmbApartment.Properties.NullText = "Apartman seçiniz (Opsiyonel)";
                pnlForm.Controls.Add(cmbApartment);
                panelY += 45;
            }

            // Title
            AddFieldLabel(pnlForm, "Başlık *", panelLeft, panelY, true);
            panelY += 20;
            txtTitle = new TextEdit();
            txtTitle.Location = new Point(panelLeft, panelY);
            txtTitle.Size = new Size(contentWidth - 70, fieldHeight);
            txtTitle.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            txtTitle.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            txtTitle.Properties.NullText = "Duyuru başlığı";
            pnlForm.Controls.Add(txtTitle);
            panelY += 45;

            // Category
            AddFieldLabel(pnlForm, "Kategori *", panelLeft, panelY, true);
            panelY += 20;
            cmbCategory = new ComboBoxEdit();
            cmbCategory.Location = new Point(panelLeft, panelY);
            cmbCategory.Size = new Size(contentWidth - 70, fieldHeight);
            cmbCategory.Properties.Items.AddRange(new[] { "Genel", "Bakım", "Toplantı", "Acil", "Diğer" });
            cmbCategory.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbCategory.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            cmbCategory.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            cmbCategory.Properties.NullText = "Kategori seçiniz";
            pnlForm.Controls.Add(cmbCategory);
            panelY += 45;

            // Content
            AddFieldLabel(pnlForm, "İçerik *", panelLeft, panelY, true);
            panelY += 20;
            txtContent = new MemoEdit();
            txtContent.Location = new Point(panelLeft, panelY);
            txtContent.Size = new Size(contentWidth - 70, 150);
            txtContent.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            txtContent.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            txtContent.Properties.NullText = "Duyuru içeriği";
            pnlForm.Controls.Add(txtContent);
            panelY += 170;

            // IsPinned
            chkIsPinned = new CheckEdit();
            chkIsPinned.Text = "Önemli Duyuru (Pin)";
            chkIsPinned.Location = new Point(panelLeft, panelY);
            chkIsPinned.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            chkIsPinned.Properties.Caption = "Önemli Duyuru (Pin)";
            pnlForm.Controls.Add(chkIsPinned);
            panelY += 35;

            // IsActive
            chkIsActive = new CheckEdit();
            chkIsActive.Text = "Aktif";
            chkIsActive.Location = new Point(panelLeft, panelY);
            chkIsActive.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            chkIsActive.Properties.Caption = "Aktif";
            pnlForm.Controls.Add(chkIsActive);
            panelY += 35;

            // ExpiryDate (Opsiyonel)
            AddFieldLabel(pnlForm, "Son Geçerlilik Tarihi", panelLeft, panelY, false);
            panelY += 20;
            dtExpiryDate = new DateEdit();
            dtExpiryDate.Location = new Point(panelLeft, panelY);
            dtExpiryDate.Size = new Size(contentWidth - 70, fieldHeight);
            dtExpiryDate.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            dtExpiryDate.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            dtExpiryDate.Properties.DisplayFormat.FormatString = "dd.MM.yyyy";
            dtExpiryDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dtExpiryDate.Properties.EditFormat.FormatString = "dd.MM.yyyy";
            dtExpiryDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dtExpiryDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            dtExpiryDate.Properties.Mask.EditMask = "dd.MM.yyyy";
            dtExpiryDate.Properties.NullText = "Son geçerlilik tarihi (Opsiyonel)";
            pnlForm.Controls.Add(dtExpiryDate);
            panelY += 50;

            // ========== BUTTONS ==========
            btnCancel = new SimpleButton();
            btnCancel.Text = "İptal";
            btnCancel.Size = new Size(130, 38);
            btnCancel.Location = new Point(panelLeft, panelY);
            btnCancel.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnCancel.Appearance.BackColor = Color.FromArgb(241, 245, 249);
            btnCancel.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            btnCancel.Appearance.BorderColor = Color.FromArgb(226, 232, 240);
            btnCancel.Appearance.Options.UseBackColor = true;
            btnCancel.Appearance.Options.UseForeColor = true;
            btnCancel.Appearance.Options.UseBorderColor = true;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            pnlForm.Controls.Add(btnCancel);

            btnSave = new SimpleButton();
            btnSave.Text = _isEditMode ? "Güncelle" : "Kaydet";
            btnSave.Size = new Size(160, 38);
            btnSave.Location = new Point(contentWidth - 70 - 160, panelY);
            btnSave.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnSave.Appearance.BackColor = Color.FromArgb(59, 130, 246);
            btnSave.Appearance.ForeColor = Color.White;
            btnSave.Appearance.Options.UseBackColor = true;
            btnSave.Appearance.Options.UseForeColor = true;
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += BtnSave_Click;
            pnlForm.Controls.Add(btnSave);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Alan etiketi ekler
        /// </summary>
        private void AddFieldLabel(Control parent, string text, int x, int y, bool required)
        {
            var lbl = new LabelControl();
            lbl.Text = text;
            if (required)
            {
                lbl.Appearance.ForeColor = Color.FromArgb(239, 68, 68);
            }
            else
            {
                lbl.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
            }
            lbl.Appearance.Font = new Font("Tahoma", 8.5F, FontStyle.Regular);
            lbl.Location = new Point(x, y);
            parent.Controls.Add(lbl);
        }

        /// <summary>
        /// Filtreleri yükler
        /// </summary>
        private void LoadFilters()
        {
            // Site Filter
            if (cmbSite != null)
            {
                var sites = _siteService.GetAll();
                cmbSite.Properties.Items.Clear();
                cmbSite.Properties.Items.Add("Seçiniz");
                foreach (var site in sites)
                {
                    cmbSite.Properties.Items.Add(site.Name);
                }
                if (cmbSite.Properties.Items.Count > 0)
                {
                    cmbSite.SelectedIndex = 0;
                }
            }

            // Apartment Filter
            if (cmbApartment != null)
            {
                var apartments = _apartmentService.GetAll();
                cmbApartment.Properties.Items.Clear();
                cmbApartment.Properties.Items.Add("Seçiniz");
                foreach (var apartment in apartments)
                {
                    cmbApartment.Properties.Items.Add(apartment.Name);
                }
                if (cmbApartment.Properties.Items.Count > 0)
                {
                    cmbApartment.SelectedIndex = 0;
                }
            }

            // ApartmentManager için otomatik apartman seçimi
            if (_currentUser?.Role == "ApartmentManager" && cmbApartment == null)
            {
                var userFlat = _flatService.GetResidentFlat(_currentUser.Id);
                if (userFlat != null)
                {
                    // ApartmentManager için apartman otomatik seçilecek
                }
            }
        }

        /// <summary>
        /// Site seçimi değiştiğinde
        /// </summary>
        private void CmbSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSite?.SelectedItem == null || cmbSite.SelectedIndex == 0 || cmbApartment == null) return;

            var selectedSiteName = cmbSite.SelectedItem.ToString();
            var sites = _siteService.GetAll();
            var selectedSite = sites.FirstOrDefault(s => s.Name == selectedSiteName);
            if (selectedSite == null) return;

            var apartments = _apartmentService.GetAllBySiteId(selectedSite.Id);
            cmbApartment.Properties.Items.Clear();
            cmbApartment.Properties.Items.Add("Seçiniz");
            foreach (var apartment in apartments)
            {
                cmbApartment.Properties.Items.Add(apartment.Name);
            }
            if (cmbApartment.Properties.Items.Count > 0)
            {
                cmbApartment.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Verileri yükler
        /// </summary>
        private void LoadData()
        {
            if (_currentAnnouncement == null) return;

            var announcement = _announcementService.GetById(_currentAnnouncement.Id);
            if (announcement == null) return;

            // Site seçimi
            if (cmbSite != null && announcement.SiteId.HasValue)
            {
                var site = _siteService.GetAll().FirstOrDefault(s => s.Id == announcement.SiteId.Value);
                if (site != null)
                {
                    for (int i = 0; i < cmbSite.Properties.Items.Count; i++)
                    {
                        if (cmbSite.Properties.Items[i].ToString() == site.Name)
                        {
                            cmbSite.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }

            // Apartment seçimi
            if (cmbApartment != null && announcement.ApartmentId.HasValue)
            {
                var apartment = _apartmentService.GetAll().FirstOrDefault(a => a.Id == announcement.ApartmentId.Value);
                if (apartment != null)
                {
                    for (int i = 0; i < cmbApartment.Properties.Items.Count; i++)
                    {
                        if (cmbApartment.Properties.Items[i].ToString() == apartment.Name)
                        {
                            cmbApartment.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }

            // Diğer alanlar
            txtTitle.Text = announcement.Title ?? "";
            cmbCategory.SelectedItem = announcement.Category ?? "Genel";
            txtContent.Text = announcement.Content ?? "";
            chkIsPinned.Checked = announcement.IsPinned;
            chkIsActive.Checked = announcement.IsActive;
            if (announcement.ExpiryDate.HasValue)
            {
                dtExpiryDate.EditValue = announcement.ExpiryDate.Value;
            }
        }

        /// <summary>
        /// Kaydet butonuna tıklandığında
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                Swal.Warning("Lütfen başlık girin.");
                return;
            }

            if (cmbCategory?.SelectedItem == null)
            {
                Swal.Warning("Lütfen bir kategori seçin.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtContent.Text))
            {
                Swal.Warning("Lütfen içerik girin.");
                return;
            }

            try
            {
                int? siteId = null;
                int? apartmentId = null;

                // Site seçimi (SiteManager için)
                if (cmbSite != null && cmbSite.SelectedItem != null && cmbSite.SelectedIndex > 0)
                {
                    var siteName = cmbSite.SelectedItem.ToString();
                    var sites = _siteService.GetAll();
                    var selectedSite = sites.FirstOrDefault(s => s.Name == siteName);
                    if (selectedSite != null)
                    {
                        siteId = selectedSite.Id;
                    }
                }

                // Apartment seçimi
                if (cmbApartment != null && cmbApartment.SelectedItem != null && cmbApartment.SelectedIndex > 0)
                {
                    var apartmentName = cmbApartment.SelectedItem.ToString();
                    var apartments = _apartmentService.GetAll();
                    var selectedApartment = apartments.FirstOrDefault(a => a.Name == apartmentName);
                    if (selectedApartment != null)
                    {
                        apartmentId = selectedApartment.Id;
                    }
                }
                else if (_currentUser?.Role == "ApartmentManager")
                {
                    // ApartmentManager için otomatik apartman
                    var userFlat = _flatService.GetResidentFlat(_currentUser.Id);
                    if (userFlat != null)
                    {
                        apartmentId = userFlat.ApartmentId;
                    }
                }

                if (_isEditMode)
                {
                    // Update
                    _currentAnnouncement.SiteId = siteId;
                    _currentAnnouncement.ApartmentId = apartmentId;
                    _currentAnnouncement.Title = txtTitle.Text.Trim();
                    _currentAnnouncement.Category = cmbCategory.SelectedItem.ToString();
                    _currentAnnouncement.Content = txtContent.Text.Trim();
                    _currentAnnouncement.IsPinned = chkIsPinned.Checked;
                    _currentAnnouncement.IsActive = chkIsActive.Checked;
                    _currentAnnouncement.ExpiryDate = dtExpiryDate.EditValue as DateTime?;

                    string result = _announcementService.Update(_currentAnnouncement);
                    if (!string.IsNullOrEmpty(result))
                    {
                        Swal.Error("Güncelleme hatası: " + result);
                        return;
                    }

                    Swal.Success("Duyuru başarıyla güncellendi.");
                }
                else
                {
                    // Add
                    var newAnnouncement = new Announcement
                    {
                        SiteId = siteId,
                        ApartmentId = apartmentId,
                        Title = txtTitle.Text.Trim(),
                        Category = cmbCategory.SelectedItem.ToString(),
                        Content = txtContent.Text.Trim(),
                        IsPinned = chkIsPinned.Checked,
                        IsActive = chkIsActive.Checked,
                        ExpiryDate = dtExpiryDate.EditValue as DateTime?,
                        CreatedByUserId = _currentUser.Id
                    };

                    string result = _announcementService.Add(newAnnouncement);
                    if (!string.IsNullOrEmpty(result))
                    {
                        Swal.Error("Ekleme hatası: " + result);
                        return;
                    }

                    Swal.Success("Duyuru başarıyla oluşturuldu.");
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Swal.Error("İşlem hatası: " + ex.Message);
            }
        }
    }
}

