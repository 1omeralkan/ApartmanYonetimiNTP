#nullable disable
// FrmAnnouncementList.cs
// Duyuru Listesi Formu - Duyuruları listeler ve yönetir
// Standart: Tahoma 8.25pt, AutoScroll = true
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Duyuru listesi formu - SiteManager, ApartmentManager ve Resident için
    /// </summary>
    public partial class FrmAnnouncementList : DevExpress.XtraEditors.XtraForm
    {
        private IAnnouncement _announcementService;
        private IFlat _flatService;
        private ISite _siteService;
        private IApartment _apartmentService;
        private User _currentUser;

        // Controls
        private GridControl gcAnnouncements;
        private GridView gvAnnouncements;
        private ComboBoxEdit cmbSiteFilter;
        private ComboBoxEdit cmbApartmentFilter;
        private ComboBoxEdit cmbCategoryFilter;
        private ComboBoxEdit cmbStatusFilter;
        private SimpleButton btnFilter;
        private SimpleButton btnClear;
        private SimpleButton btnNewAnnouncement;
        private SimpleButton btnEdit;
        private SimpleButton btnDelete;
        private SimpleButton btnTogglePin;
        private SimpleButton btnToggleActive;
        private LabelControl lblTitle;

        // Stat Cards
        private Panel pnlCardTotal;
        private Panel pnlCardActive;
        private Panel pnlCardPinned;
        private LabelControl lblTotalCount;
        private LabelControl lblActiveCount;
        private LabelControl lblPinnedCount;

        /// <summary>
        /// FrmAnnouncementList constructor
        /// </summary>
        public FrmAnnouncementList(User user)
        {
            _currentUser = user;
            _announcementService = new SAnnouncement();
            _flatService = new SFlat();
            _siteService = new SSite();
            _apartmentService = new SApartment();
            InitializeComponent();
            LoadFilters();
            LoadData();
        }

        /// <summary>
        /// Form bileşenlerini başlatır
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = "Duyuru Yönetimi";
            this.Size = new Size(1300, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            int leftMargin = 20;
            int currentY = 20;
            int contentWidth = this.Width - (leftMargin * 2);

            // ========== HEADER SECTION ==========
            lblTitle = new LabelControl();
            lblTitle.Text = "📢 Duyuru Yönetimi";
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblTitle);

            currentY += 50;

            // ========== STAT CARDS SECTION ==========
            int cardWidth = (contentWidth - 40) / 3;
            int cardHeight = 100;
            int cardSpacing = 20;

            pnlCardTotal = CreateStatCard(leftMargin, currentY, cardWidth, cardHeight,
                Color.FromArgb(59, 130, 246), "📋", "Toplam Duyuru", "0", ref lblTotalCount);
            this.Controls.Add(pnlCardTotal);

            pnlCardActive = CreateStatCard(leftMargin + cardWidth + cardSpacing, currentY, cardWidth, cardHeight,
                Color.FromArgb(34, 197, 94), "✅", "Aktif", "0", ref lblActiveCount);
            this.Controls.Add(pnlCardActive);

            pnlCardPinned = CreateStatCard(leftMargin + (cardWidth + cardSpacing) * 2, currentY, cardWidth, cardHeight,
                Color.FromArgb(234, 179, 8), "📌", "Önemli", "0", ref lblPinnedCount);
            this.Controls.Add(pnlCardPinned);

            currentY += cardHeight + 30;

            // ========== FILTER PANEL ==========
            var pnlFilter = new Panel();
            pnlFilter.Location = new Point(leftMargin, currentY);
            pnlFilter.Size = new Size(contentWidth, 100);
            pnlFilter.BackColor = Color.FromArgb(240, 242, 245);
            this.Controls.Add(pnlFilter);

            int filterY = 10;
            int filterX = 10;
            int filterSpacing = 15;

            // Site Filter (SiteManager için)
            if (_currentUser?.Role == "SiteManager" || _currentUser?.Role == "SuperAdmin" || _currentUser?.Role == "Admin")
            {
                var lblSite = new LabelControl();
                lblSite.Text = "Site";
                lblSite.Location = new Point(filterX, filterY);
                lblSite.Appearance.Font = new Font("Tahoma", 8F);
                pnlFilter.Controls.Add(lblSite);

                cmbSiteFilter = new ComboBoxEdit();
                cmbSiteFilter.Location = new Point(filterX, filterY + 18);
                cmbSiteFilter.Size = new Size(150, 26);
                cmbSiteFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                cmbSiteFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
                cmbSiteFilter.SelectedIndexChanged += CmbSiteFilter_SelectedIndexChanged;
                pnlFilter.Controls.Add(cmbSiteFilter);
                filterX += 170;
            }

            // Apartment Filter
            if (_currentUser?.Role != "Resident")
            {
                var lblApartment = new LabelControl();
                lblApartment.Text = "Apartman";
                lblApartment.Location = new Point(filterX, filterY);
                lblApartment.Appearance.Font = new Font("Tahoma", 8F);
                pnlFilter.Controls.Add(lblApartment);

                cmbApartmentFilter = new ComboBoxEdit();
                cmbApartmentFilter.Location = new Point(filterX, filterY + 18);
                cmbApartmentFilter.Size = new Size(150, 26);
                cmbApartmentFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                cmbApartmentFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
                pnlFilter.Controls.Add(cmbApartmentFilter);
                filterX += 170;
            }

            // Category Filter
            var lblCategory = new LabelControl();
            lblCategory.Text = "Kategori";
            lblCategory.Location = new Point(filterX, filterY);
            lblCategory.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblCategory);

            cmbCategoryFilter = new ComboBoxEdit();
            cmbCategoryFilter.Location = new Point(filterX, filterY + 18);
            cmbCategoryFilter.Size = new Size(150, 26);
            cmbCategoryFilter.Properties.Items.AddRange(new[] { "Tümü", "Genel", "Bakım", "Toplantı", "Acil", "Diğer" });
            cmbCategoryFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbCategoryFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
            cmbCategoryFilter.SelectedIndex = 0;
            pnlFilter.Controls.Add(cmbCategoryFilter);
            filterX += 170;

            // Status Filter
            var lblStatus = new LabelControl();
            lblStatus.Text = "Durum";
            lblStatus.Location = new Point(filterX, filterY);
            lblStatus.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblStatus);

            cmbStatusFilter = new ComboBoxEdit();
            cmbStatusFilter.Location = new Point(filterX, filterY + 18);
            cmbStatusFilter.Size = new Size(120, 26);
            cmbStatusFilter.Properties.Items.AddRange(new[] { "Tümü", "Aktif", "Pasif" });
            cmbStatusFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbStatusFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
            cmbStatusFilter.SelectedIndex = 0;
            pnlFilter.Controls.Add(cmbStatusFilter);
            filterX += 140;

            // Filter Button
            btnFilter = new SimpleButton();
            btnFilter.Text = "🔍 Ara";
            btnFilter.Size = new Size(100, 30);
            btnFilter.Location = new Point(contentWidth - 220, filterY + 18);
            btnFilter.Appearance.Font = new Font("Tahoma", 10F);
            btnFilter.Appearance.BackColor = Color.FromArgb(66, 133, 244);
            btnFilter.Appearance.ForeColor = Color.White;
            btnFilter.Appearance.Options.UseBackColor = true;
            btnFilter.Appearance.Options.UseForeColor = true;
            btnFilter.Click += BtnFilter_Click;
            pnlFilter.Controls.Add(btnFilter);

            // Clear Button
            btnClear = new SimpleButton();
            btnClear.Text = "⊗ Temizle";
            btnClear.Size = new Size(100, 30);
            btnClear.Location = new Point(contentWidth - 110, filterY + 18);
            btnClear.Appearance.Font = new Font("Tahoma", 10F);
            btnClear.Click += BtnClear_Click;
            pnlFilter.Controls.Add(btnClear);

            currentY += 120;

            // ========== ACTION BUTTONS ==========
            // Sadece Manager rolleri için yönetim butonları
            if (_currentUser?.Role != "Resident")
            {
                btnNewAnnouncement = new SimpleButton();
                btnNewAnnouncement.Text = "+ Yeni Duyuru";
                btnNewAnnouncement.Size = new Size(130, 35);
                btnNewAnnouncement.Location = new Point(leftMargin, currentY);
                btnNewAnnouncement.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
                btnNewAnnouncement.Appearance.BackColor = Color.FromArgb(34, 197, 94);
                btnNewAnnouncement.Appearance.ForeColor = Color.White;
                btnNewAnnouncement.Appearance.Options.UseBackColor = true;
                btnNewAnnouncement.Appearance.Options.UseForeColor = true;
                btnNewAnnouncement.Cursor = Cursors.Hand;
                btnNewAnnouncement.Click += BtnNewAnnouncement_Click;
                this.Controls.Add(btnNewAnnouncement);

                btnEdit = new SimpleButton();
                btnEdit.Text = "✏️ Düzenle";
                btnEdit.Size = new Size(120, 35);
                btnEdit.Location = new Point(contentWidth - 480, currentY);
                btnEdit.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
                btnEdit.Appearance.BackColor = Color.FromArgb(59, 130, 246);
                btnEdit.Appearance.ForeColor = Color.White;
                btnEdit.Appearance.Options.UseBackColor = true;
                btnEdit.Appearance.Options.UseForeColor = true;
                btnEdit.Cursor = Cursors.Hand;
                btnEdit.Enabled = false;
                btnEdit.Click += BtnEdit_Click;
                this.Controls.Add(btnEdit);

                btnTogglePin = new SimpleButton();
                btnTogglePin.Text = "📌 Pin";
                btnTogglePin.Size = new Size(100, 35);
                btnTogglePin.Location = new Point(contentWidth - 360, currentY);
                btnTogglePin.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
                btnTogglePin.Appearance.BackColor = Color.FromArgb(234, 179, 8);
                btnTogglePin.Appearance.ForeColor = Color.White;
                btnTogglePin.Appearance.Options.UseBackColor = true;
                btnTogglePin.Appearance.Options.UseForeColor = true;
                btnTogglePin.Cursor = Cursors.Hand;
                btnTogglePin.Enabled = false;
                btnTogglePin.Click += BtnTogglePin_Click;
                this.Controls.Add(btnTogglePin);

                btnToggleActive = new SimpleButton();
                btnToggleActive.Text = "✅ Aktif/Pasif";
                btnToggleActive.Size = new Size(120, 35);
                btnToggleActive.Location = new Point(contentWidth - 240, currentY);
                btnToggleActive.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
                btnToggleActive.Appearance.BackColor = Color.FromArgb(34, 197, 94);
                btnToggleActive.Appearance.ForeColor = Color.White;
                btnToggleActive.Appearance.Options.UseBackColor = true;
                btnToggleActive.Appearance.Options.UseForeColor = true;
                btnToggleActive.Cursor = Cursors.Hand;
                btnToggleActive.Enabled = false;
                btnToggleActive.Click += BtnToggleActive_Click;
                this.Controls.Add(btnToggleActive);

                btnDelete = new SimpleButton();
                btnDelete.Text = "🗑️ Sil";
                btnDelete.Size = new Size(100, 35);
                btnDelete.Location = new Point(contentWidth - 120, currentY);
                btnDelete.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
                btnDelete.Appearance.BackColor = Color.FromArgb(239, 68, 68);
                btnDelete.Appearance.ForeColor = Color.White;
                btnDelete.Appearance.Options.UseBackColor = true;
                btnDelete.Appearance.Options.UseForeColor = true;
                btnDelete.Cursor = Cursors.Hand;
                btnDelete.Enabled = false;
                btnDelete.Click += BtnDelete_Click;
                this.Controls.Add(btnDelete);
            }

            currentY += 50;

            // ========== GRID CONTROL ==========
            gcAnnouncements = new GridControl();
            gvAnnouncements = new GridView(gcAnnouncements);
            gcAnnouncements.MainView = gvAnnouncements;
            gcAnnouncements.Location = new Point(leftMargin, currentY);
            gcAnnouncements.Size = new Size(contentWidth, this.Height - currentY - 50);
            gcAnnouncements.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // Grid View Settings
            gvAnnouncements.OptionsBehavior.Editable = false;
            gvAnnouncements.OptionsView.ShowGroupPanel = false;
            gvAnnouncements.OptionsView.ShowIndicator = false;
            gvAnnouncements.RowHeight = 50;
            gvAnnouncements.Appearance.HeaderPanel.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            gvAnnouncements.Appearance.Row.Font = new Font("Tahoma", 9F);
            gvAnnouncements.FocusedRowChanged += GvAnnouncements_FocusedRowChanged;
            gvAnnouncements.CustomDrawCell += GvAnnouncements_CustomDrawCell;
            gvAnnouncements.DoubleClick += GvAnnouncements_DoubleClick;

            this.Controls.Add(gcAnnouncements);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Stat card oluşturur
        /// </summary>
        private Panel CreateStatCard(int x, int y, int width, int height, Color barColor, string icon,
            string title, string defaultValue, ref LabelControl countLabel)
        {
            var panel = new Panel();
            panel.Size = new Size(width, height);
            panel.Location = new Point(x, y);
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.None;

            // Üst renkli çizgi
            var topBar = new Panel();
            topBar.Size = new Size(width, 4);
            topBar.Location = new Point(0, 0);
            topBar.BackColor = barColor;
            panel.Controls.Add(topBar);

            // İkon
            var lblIcon = new LabelControl();
            lblIcon.Text = icon;
            lblIcon.Appearance.Font = new Font("Segoe UI Emoji", 24F);
            lblIcon.Location = new Point(15, 15);
            panel.Controls.Add(lblIcon);

            // Başlık
            var lblTitle = new LabelControl();
            lblTitle.Text = title;
            lblTitle.Appearance.Font = new Font("Tahoma", 9F);
            lblTitle.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            lblTitle.Location = new Point(15, height - 35);
            panel.Controls.Add(lblTitle);

            // Değer
            countLabel = new LabelControl();
            countLabel.Text = defaultValue;
            countLabel.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            countLabel.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            countLabel.Location = new Point(15, height - 55);
            countLabel.Size = new Size(width - 30, 25);
            panel.Controls.Add(countLabel);

            return panel;
        }

        /// <summary>
        /// Helper class for filter dropdowns
        /// </summary>
        private class FilterItem
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        /// <summary>
        /// Filtreleri yükler
        /// </summary>
        private void LoadFilters()
        {
            // Site Filter
            if (cmbSiteFilter != null)
            {
                var sites = _siteService.GetAll();
                cmbSiteFilter.Properties.Items.Clear();
                cmbSiteFilter.Properties.Items.Add(new FilterItem { Id = null, Name = "Tümü" });
                foreach (var site in sites)
                {
                    cmbSiteFilter.Properties.Items.Add(new FilterItem { Id = site.Id, Name = site.Name });
                }
                cmbSiteFilter.SelectedIndex = 0;
            }

            // Apartment Filter
            if (cmbApartmentFilter != null)
            {
                var apartments = _apartmentService.GetAll();
                cmbApartmentFilter.Properties.Items.Clear();
                cmbApartmentFilter.Properties.Items.Add(new FilterItem { Id = null, Name = "Tümü" });
                foreach (var apartment in apartments)
                {
                    cmbApartmentFilter.Properties.Items.Add(new FilterItem { Id = apartment.Id, Name = apartment.Name });
                }
                cmbApartmentFilter.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Site filter değiştiğinde
        /// </summary>
        private void CmbSiteFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSiteFilter?.SelectedItem is FilterItem selectedSite && selectedSite.Id.HasValue && cmbApartmentFilter != null)
            {
                var apartments = _apartmentService.GetAllBySiteId(selectedSite.Id.Value);
                cmbApartmentFilter.Properties.Items.Clear();
                cmbApartmentFilter.Properties.Items.Add(new FilterItem { Id = null, Name = "Tümü" });
                foreach (var apartment in apartments)
                {
                    cmbApartmentFilter.Properties.Items.Add(new FilterItem { Id = apartment.Id, Name = apartment.Name });
                }
                cmbApartmentFilter.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Verileri yükler
        /// </summary>
        private void LoadData()
        {
            try
            {
                List<Announcement> announcementList;

                // Role göre filtreleme
                if (_currentUser?.Role == "Resident")
                {
                    // Resident: Sadece aktif duyuruları görüntüle
                    var userFlat = _flatService.GetResidentFlat(_currentUser.Id);
                    if (userFlat != null)
                    {
                        announcementList = _announcementService.GetActiveAnnouncements(null, userFlat.ApartmentId);
                    }
                    else
                    {
                        announcementList = new List<Announcement>();
                    }
                }
                else if (_currentUser?.Role == "ApartmentManager")
                {
                    // ApartmentManager: Kendi apartmanındaki duyurular
                    var userFlat = _flatService.GetResidentFlat(_currentUser.Id);
                    if (userFlat != null)
                    {
                        announcementList = _announcementService.GetByApartmentId(userFlat.ApartmentId);
                    }
                    else
                    {
                        announcementList = new List<Announcement>();
                    }
                }
                else if (_currentUser?.Role == "SiteManager")
                {
                    // SiteManager: Site bazlı filtreleme
                    var selectedSite = cmbSiteFilter?.SelectedItem as FilterItem;
                    if (selectedSite?.Id.HasValue == true)
                    {
                        announcementList = _announcementService.GetBySiteId(selectedSite.Id.Value);
                    }
                    else
                    {
                        // Tüm siteler için
                        announcementList = _announcementService.GetAll();
                    }
                }
                else
                {
                    // Admin/SuperAdmin: Tüm duyurular
                    announcementList = _announcementService.GetAll();
                }

                // Filtreleme
                var selectedApartment = cmbApartmentFilter?.SelectedItem as FilterItem;
                if (selectedApartment?.Id.HasValue == true)
                {
                    announcementList = announcementList.Where(a => a.ApartmentId == selectedApartment.Id.Value).ToList();
                }

                var categoryFilter = cmbCategoryFilter?.SelectedItem?.ToString();
                if (categoryFilter != null && categoryFilter != "Tümü")
                {
                    announcementList = announcementList.Where(a => a.Category == categoryFilter).ToList();
                }

                var statusFilter = cmbStatusFilter?.SelectedItem?.ToString();
                if (statusFilter == "Aktif")
                {
                    announcementList = announcementList.Where(a => a.IsActive).ToList();
                }
                else if (statusFilter == "Pasif")
                {
                    announcementList = announcementList.Where(a => !a.IsActive).ToList();
                }

                // Display data
                var displayData = announcementList.Select(a => new
                {
                    a.Id,
                    Baslik = a.Title ?? "-",
                    Icerik = a.Content ?? "-",
                    Kategori = a.Category ?? "Genel",
                    Olusturan = a.CreatedByUser != null ? $"{a.CreatedByUser.FirstName} {a.CreatedByUser.LastName}" : "-",
                    Tarih = a.CreatedDate.ToString("dd.MM.yyyy"),
                    Durum = a.IsActive ? "Aktif" : "Pasif",
                    IsPinned = a.IsPinned ? "📌" : "",
                    IsActive = a.IsActive,
                    SiteId = a.SiteId,
                    ApartmentId = a.ApartmentId
                }).ToList();

                gcAnnouncements.DataSource = displayData;
                gvAnnouncements.PopulateColumns();

                // Sütun ayarları
                ConfigureColumns();

                // İstatistikleri güncelle
                UpdateStatistics(announcementList);
            }
            catch (Exception ex)
            {
                Swal.Error("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Sütunları yapılandırır
        /// </summary>
        private void ConfigureColumns()
        {
            int visibleIndex = 0;

            // Gizlenecek sütunlar
            if (gvAnnouncements.Columns["Id"] != null) gvAnnouncements.Columns["Id"].Visible = false;
            if (gvAnnouncements.Columns["IsActive"] != null) gvAnnouncements.Columns["IsActive"].Visible = false;
            if (gvAnnouncements.Columns["SiteId"] != null) gvAnnouncements.Columns["SiteId"].Visible = false;
            if (gvAnnouncements.Columns["ApartmentId"] != null) gvAnnouncements.Columns["ApartmentId"].Visible = false;

            // IsPinned (Pin ikonu)
            if (gvAnnouncements.Columns["IsPinned"] != null)
            {
                gvAnnouncements.Columns["IsPinned"].VisibleIndex = visibleIndex++;
                gvAnnouncements.Columns["IsPinned"].Caption = "";
                gvAnnouncements.Columns["IsPinned"].Width = 40;
                gvAnnouncements.Columns["IsPinned"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }

            if (gvAnnouncements.Columns["Baslik"] != null)
            {
                gvAnnouncements.Columns["Baslik"].VisibleIndex = visibleIndex++;
                gvAnnouncements.Columns["Baslik"].Caption = "Başlık";
                gvAnnouncements.Columns["Baslik"].Width = 250;
            }

            if (gvAnnouncements.Columns["Icerik"] != null)
            {
                gvAnnouncements.Columns["Icerik"].VisibleIndex = visibleIndex++;
                gvAnnouncements.Columns["Icerik"].Caption = "İçerik";
                gvAnnouncements.Columns["Icerik"].Width = 400;
            }

            if (gvAnnouncements.Columns["Kategori"] != null)
            {
                gvAnnouncements.Columns["Kategori"].VisibleIndex = visibleIndex++;
                gvAnnouncements.Columns["Kategori"].Caption = "Kategori";
                gvAnnouncements.Columns["Kategori"].Width = 120;
            }

            if (gvAnnouncements.Columns["Olusturan"] != null)
            {
                gvAnnouncements.Columns["Olusturan"].VisibleIndex = visibleIndex++;
                gvAnnouncements.Columns["Olusturan"].Caption = "Oluşturan";
                gvAnnouncements.Columns["Olusturan"].Width = 150;
            }

            if (gvAnnouncements.Columns["Tarih"] != null)
            {
                gvAnnouncements.Columns["Tarih"].VisibleIndex = visibleIndex++;
                gvAnnouncements.Columns["Tarih"].Caption = "Tarih";
                gvAnnouncements.Columns["Tarih"].Width = 120;
            }

            if (gvAnnouncements.Columns["Durum"] != null)
            {
                gvAnnouncements.Columns["Durum"].VisibleIndex = visibleIndex++;
                gvAnnouncements.Columns["Durum"].Caption = "Durum";
                gvAnnouncements.Columns["Durum"].Width = 100;
            }
        }

        /// <summary>
        /// İstatistikleri günceller
        /// </summary>
        private void UpdateStatistics(List<Announcement> announcementList)
        {
            int total = announcementList.Count;
            int active = announcementList.Count(a => a.IsActive);
            int pinned = announcementList.Count(a => a.IsPinned);

            lblTotalCount.Text = total.ToString();
            lblActiveCount.Text = active.ToString();
            lblPinnedCount.Text = pinned.ToString();
        }

        /// <summary>
        /// Grid row seçimi değiştiğinde
        /// </summary>
        private void GvAnnouncements_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            bool hasSelection = gvAnnouncements.FocusedRowHandle >= 0;
            if (btnEdit != null) btnEdit.Enabled = hasSelection;
            if (btnDelete != null) btnDelete.Enabled = hasSelection;
            if (btnTogglePin != null) btnTogglePin.Enabled = hasSelection;
            if (btnToggleActive != null) btnToggleActive.Enabled = hasSelection;
        }

        /// <summary>
        /// Custom draw cell - Pin ve Durum için
        /// </summary>
        private void GvAnnouncements_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column?.FieldName == "Durum")
            {
                var row = gvAnnouncements.GetRow(e.RowHandle);
                if (row != null)
                {
                    var isActiveProp = row.GetType().GetProperty("IsActive");
                    bool isActive = isActiveProp != null && (bool)isActiveProp.GetValue(row);

                    if (isActive)
                    {
                        e.Appearance.BackColor = Color.FromArgb(34, 197, 94);
                        e.Appearance.ForeColor = Color.White;
                    }
                    else
                    {
                        e.Appearance.BackColor = Color.FromArgb(239, 68, 68);
                        e.Appearance.ForeColor = Color.White;
                    }
                    e.Appearance.Options.UseBackColor = true;
                    e.Appearance.Options.UseForeColor = true;
                }
            }
        }

        /// <summary>
        /// Grid double click
        /// </summary>
        private void GvAnnouncements_DoubleClick(object sender, EventArgs e)
        {
            if (_currentUser?.Role != "Resident")
            {
                BtnEdit_Click(sender, e);
            }
        }

        /// <summary>
        /// Filter button click
        /// </summary>
        private void BtnFilter_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// Clear button click
        /// </summary>
        private void BtnClear_Click(object sender, EventArgs e)
        {
            if (cmbSiteFilter != null) cmbSiteFilter.SelectedIndex = 0;
            if (cmbApartmentFilter != null) cmbApartmentFilter.SelectedIndex = 0;
            cmbCategoryFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndex = 0;
            LoadData();
        }

        /// <summary>
        /// New Announcement button click
        /// </summary>
        private void BtnNewAnnouncement_Click(object sender, EventArgs e)
        {
            var frm = new FrmAnnouncementManagement(null, _currentUser);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        /// <summary>
        /// Edit button click
        /// </summary>
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (gvAnnouncements.FocusedRowHandle < 0) return;

            var row = gvAnnouncements.GetRow(gvAnnouncements.FocusedRowHandle);
            if (row == null) return;

            var idProp = row.GetType().GetProperty("Id");
            if (idProp == null) return;

            int announcementId = (int)idProp.GetValue(row);
            var announcement = _announcementService.GetById(announcementId);

            if (announcement != null)
            {
                var frm = new FrmAnnouncementManagement(announcement, _currentUser);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        /// <summary>
        /// Delete button click
        /// </summary>
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (gvAnnouncements.FocusedRowHandle < 0) return;

            var row = gvAnnouncements.GetRow(gvAnnouncements.FocusedRowHandle);
            if (row == null) return;

            var idProp = row.GetType().GetProperty("Id");
            if (idProp == null) return;

            int announcementId = (int)idProp.GetValue(row);

            if (Swal.Confirm("Bu duyuruyu silmek istediğinize emin misiniz?", "Duyuru Sil",
                "Evet, Sil", Color.FromArgb(239, 68, 68)))
            {
                string result = _announcementService.Delete(announcementId);
                if (string.IsNullOrEmpty(result))
                {
                    Swal.Success("Duyuru başarıyla silindi.");
                    LoadData();
                }
                else
                {
                    Swal.Error("Silme hatası: " + result);
                }
            }
        }

        /// <summary>
        /// Toggle Pin button click
        /// </summary>
        private void BtnTogglePin_Click(object sender, EventArgs e)
        {
            if (gvAnnouncements.FocusedRowHandle < 0) return;

            var row = gvAnnouncements.GetRow(gvAnnouncements.FocusedRowHandle);
            if (row == null) return;

            var idProp = row.GetType().GetProperty("Id");
            if (idProp == null) return;

            int announcementId = (int)idProp.GetValue(row);

            string result = _announcementService.TogglePin(announcementId);
            if (string.IsNullOrEmpty(result))
            {
                Swal.Success("Pin durumu değiştirildi.");
                LoadData();
            }
            else
            {
                Swal.Error("İşlem hatası: " + result);
            }
        }

        /// <summary>
        /// Toggle Active button click
        /// </summary>
        private void BtnToggleActive_Click(object sender, EventArgs e)
        {
            if (gvAnnouncements.FocusedRowHandle < 0) return;

            var row = gvAnnouncements.GetRow(gvAnnouncements.FocusedRowHandle);
            if (row == null) return;

            var idProp = row.GetType().GetProperty("Id");
            if (idProp == null) return;

            int announcementId = (int)idProp.GetValue(row);

            string result = _announcementService.ToggleActive(announcementId);
            if (string.IsNullOrEmpty(result))
            {
                Swal.Success("Aktif/Pasif durumu değiştirildi.");
                LoadData();
            }
            else
            {
                Swal.Error("İşlem hatası: " + result);
            }
        }
    }
}

