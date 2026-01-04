#nullable disable
// FrmDuesList.cs
// Aidat Listesi Formu - Aidatları listeler ve yönetir
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
    /// Aidat listesi formu - SiteManager ve ApartmentManager için
    /// </summary>
    public partial class FrmDuesList : DevExpress.XtraEditors.XtraForm
    {
        private IDues _duesService;
        private IFlat _flatService;
        private ISite _siteService;
        private IApartment _apartmentService;
        private User _currentUser;

        // Controls
        private GridControl gcDues;
        private GridView gvDues;
        private ComboBoxEdit cmbSiteFilter;
        private ComboBoxEdit cmbApartmentFilter;
        private ComboBoxEdit cmbStatusFilter;
        private ComboBoxEdit cmbMonthFilter;
        private ComboBoxEdit cmbYearFilter;
        private SimpleButton btnFilter;
        private SimpleButton btnClear;
        private SimpleButton btnNewDues;
        private SimpleButton btnEdit;
        private SimpleButton btnDelete;
        private SimpleButton btnMarkAsPaid;
        private SimpleButton btnBulkCreate;
        private LabelControl lblTitle;

        // Stat Cards
        private Panel pnlCardTotal;
        private Panel pnlCardPending;
        private Panel pnlCardPaid;
        private Panel pnlCardTotalAmount;
        private LabelControl lblTotalCount;
        private LabelControl lblPendingCount;
        private LabelControl lblPaidCount;
        private LabelControl lblTotalAmount;

        /// <summary>
        /// FrmDuesList constructor
        /// </summary>
        public FrmDuesList(User user)
        {
            _currentUser = user;
            _duesService = new SDues();
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
            this.Text = "Aidat Yönetimi";
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
            lblTitle.Text = "💰 Aidat Yönetimi";
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblTitle);

            currentY += 50;

            // ========== STAT CARDS SECTION ==========
            int cardWidth = (contentWidth - 60) / 4;
            int cardHeight = 100;
            int cardSpacing = 20;

            pnlCardTotal = CreateStatCard(leftMargin, currentY, cardWidth, cardHeight,
                Color.FromArgb(59, 130, 246), "📋", "Toplam Aidat", "0", ref lblTotalCount);
            this.Controls.Add(pnlCardTotal);

            pnlCardPending = CreateStatCard(leftMargin + cardWidth + cardSpacing, currentY, cardWidth, cardHeight,
                Color.FromArgb(234, 179, 8), "⏳", "Bekleyen", "0", ref lblPendingCount);
            this.Controls.Add(pnlCardPending);

            pnlCardPaid = CreateStatCard(leftMargin + (cardWidth + cardSpacing) * 2, currentY, cardWidth, cardHeight,
                Color.FromArgb(34, 197, 94), "✅", "Ödenen", "0", ref lblPaidCount);
            this.Controls.Add(pnlCardPaid);

            pnlCardTotalAmount = CreateStatCard(leftMargin + (cardWidth + cardSpacing) * 3, currentY, cardWidth, cardHeight,
                Color.FromArgb(239, 68, 68), "💵", "Toplam Tutar", "0 TL", ref lblTotalAmount);
            this.Controls.Add(pnlCardTotalAmount);

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
                cmbSiteFilter.Size = new Size(180, 26);
                cmbSiteFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                cmbSiteFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
                cmbSiteFilter.SelectedIndexChanged += CmbSiteFilter_SelectedIndexChanged;
                pnlFilter.Controls.Add(cmbSiteFilter);
                filterX += 200;
            }

            // Apartment Filter
            var lblApartment = new LabelControl();
            lblApartment.Text = "Apartman";
            lblApartment.Location = new Point(filterX, filterY);
            lblApartment.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblApartment);

            cmbApartmentFilter = new ComboBoxEdit();
            cmbApartmentFilter.Location = new Point(filterX, filterY + 18);
            cmbApartmentFilter.Size = new Size(180, 26);
            cmbApartmentFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbApartmentFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
            pnlFilter.Controls.Add(cmbApartmentFilter);
            filterX += 200;

            // Status Filter
            var lblStatus = new LabelControl();
            lblStatus.Text = "Durum";
            lblStatus.Location = new Point(filterX, filterY);
            lblStatus.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblStatus);

            cmbStatusFilter = new ComboBoxEdit();
            cmbStatusFilter.Location = new Point(filterX, filterY + 18);
            cmbStatusFilter.Size = new Size(150, 26);
            cmbStatusFilter.Properties.Items.AddRange(new[] { "Tümü", "Bekleyen", "Ödenen" });
            cmbStatusFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbStatusFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
            cmbStatusFilter.SelectedIndex = 0;
            pnlFilter.Controls.Add(cmbStatusFilter);
            filterX += 170;

            // Month Filter
            var lblMonth = new LabelControl();
            lblMonth.Text = "Ay";
            lblMonth.Location = new Point(filterX, filterY);
            lblMonth.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblMonth);

            cmbMonthFilter = new ComboBoxEdit();
            cmbMonthFilter.Location = new Point(filterX, filterY + 18);
            cmbMonthFilter.Size = new Size(100, 26);
            cmbMonthFilter.Properties.Items.AddRange(new[] { "Tümü", "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
                "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" });
            cmbMonthFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbMonthFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
            cmbMonthFilter.SelectedIndex = 0;
            pnlFilter.Controls.Add(cmbMonthFilter);
            filterX += 120;

            // Year Filter
            var lblYear = new LabelControl();
            lblYear.Text = "Yıl";
            lblYear.Location = new Point(filterX, filterY);
            lblYear.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblYear);

            cmbYearFilter = new ComboBoxEdit();
            cmbYearFilter.Location = new Point(filterX, filterY + 18);
            cmbYearFilter.Size = new Size(100, 26);
            var years = new List<string> { "Tümü" };
            for (int year = DateTime.Now.Year; year >= DateTime.Now.Year - 5; year--)
            {
                years.Add(year.ToString());
            }
            cmbYearFilter.Properties.Items.AddRange(years);
            cmbYearFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbYearFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
            cmbYearFilter.SelectedIndex = 0;
            pnlFilter.Controls.Add(cmbYearFilter);
            filterX += 120;

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
            btnNewDues = new SimpleButton();
            btnNewDues.Text = "+ Yeni Aidat";
            btnNewDues.Size = new Size(130, 35);
            btnNewDues.Location = new Point(leftMargin, currentY);
            btnNewDues.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnNewDues.Appearance.BackColor = Color.FromArgb(34, 197, 94);
            btnNewDues.Appearance.ForeColor = Color.White;
            btnNewDues.Appearance.Options.UseBackColor = true;
            btnNewDues.Appearance.Options.UseForeColor = true;
            btnNewDues.Cursor = Cursors.Hand;
            btnNewDues.Click += BtnNewDues_Click;
            this.Controls.Add(btnNewDues);

            btnBulkCreate = new SimpleButton();
            btnBulkCreate.Text = "📦 Toplu Oluştur";
            btnBulkCreate.Size = new Size(150, 35);
            btnBulkCreate.Location = new Point(leftMargin + 140, currentY);
            btnBulkCreate.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnBulkCreate.Appearance.BackColor = Color.FromArgb(59, 130, 246);
            btnBulkCreate.Appearance.ForeColor = Color.White;
            btnBulkCreate.Appearance.Options.UseBackColor = true;
            btnBulkCreate.Appearance.Options.UseForeColor = true;
            btnBulkCreate.Cursor = Cursors.Hand;
            btnBulkCreate.Click += BtnBulkCreate_Click;
            this.Controls.Add(btnBulkCreate);

            btnEdit = new SimpleButton();
            btnEdit.Text = "✏️ Düzenle";
            btnEdit.Size = new Size(120, 35);
            btnEdit.Location = new Point(contentWidth - 360, currentY);
            btnEdit.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnEdit.Appearance.BackColor = Color.FromArgb(59, 130, 246);
            btnEdit.Appearance.ForeColor = Color.White;
            btnEdit.Appearance.Options.UseBackColor = true;
            btnEdit.Appearance.Options.UseForeColor = true;
            btnEdit.Cursor = Cursors.Hand;
            btnEdit.Enabled = false;
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            btnMarkAsPaid = new SimpleButton();
            btnMarkAsPaid.Text = "✅ Ödendi İşaretle";
            btnMarkAsPaid.Size = new Size(150, 35);
            btnMarkAsPaid.Location = new Point(contentWidth - 230, currentY);
            btnMarkAsPaid.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnMarkAsPaid.Appearance.BackColor = Color.FromArgb(34, 197, 94);
            btnMarkAsPaid.Appearance.ForeColor = Color.White;
            btnMarkAsPaid.Appearance.Options.UseBackColor = true;
            btnMarkAsPaid.Appearance.Options.UseForeColor = true;
            btnMarkAsPaid.Cursor = Cursors.Hand;
            btnMarkAsPaid.Enabled = false;
            btnMarkAsPaid.Click += BtnMarkAsPaid_Click;
            this.Controls.Add(btnMarkAsPaid);

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

            currentY += 50;

            // ========== GRID CONTROL ==========
            gcDues = new GridControl();
            gvDues = new GridView(gcDues);
            gcDues.MainView = gvDues;
            gcDues.Location = new Point(leftMargin, currentY);
            gcDues.Size = new Size(contentWidth, this.Height - currentY - 50);
            gcDues.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // Grid View Settings
            gvDues.OptionsBehavior.Editable = false;
            gvDues.OptionsView.ShowGroupPanel = false;
            gvDues.OptionsView.ShowIndicator = false;
            gvDues.RowHeight = 40;
            gvDues.Appearance.HeaderPanel.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            gvDues.Appearance.Row.Font = new Font("Tahoma", 9F);
            gvDues.FocusedRowChanged += GvDues_FocusedRowChanged;
            gvDues.CustomDrawCell += GvDues_CustomDrawCell;
            gvDues.DoubleClick += GvDues_DoubleClick;

            this.Controls.Add(gcDues);

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
            // Site Filter (SiteManager için)
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
            var apartments = _apartmentService.GetAll();
            cmbApartmentFilter.Properties.Items.Clear();
            cmbApartmentFilter.Properties.Items.Add(new FilterItem { Id = null, Name = "Tümü" });
            foreach (var apartment in apartments)
            {
                cmbApartmentFilter.Properties.Items.Add(new FilterItem { Id = apartment.Id, Name = apartment.Name });
            }
            cmbApartmentFilter.SelectedIndex = 0;
        }

        /// <summary>
        /// Site filter değiştiğinde
        /// </summary>
        private void CmbSiteFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSiteFilter?.SelectedItem is FilterItem selectedSite && selectedSite.Id.HasValue)
            {
                // Apartmanları site'e göre filtrele
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
                List<Dues> duesList;

                // Role göre filtreleme
                if (_currentUser?.Role == "ApartmentManager")
                {
                    // ApartmentManager: Sadece kendi apartmanındaki aidatlar
                    var userFlat = _flatService.GetResidentFlat(_currentUser.Id);
                    if (userFlat != null)
                    {
                        duesList = _duesService.GetByApartmentId(userFlat.ApartmentId);
                    }
                    else
                    {
                        duesList = new List<Dues>();
                    }
                }
                else if (_currentUser?.Role == "SiteManager")
                {
                    // SiteManager: Site bazlı filtreleme
                    var selectedSite = cmbSiteFilter?.SelectedItem as FilterItem;
                    if (selectedSite?.Id.HasValue == true)
                    {
                        duesList = _duesService.GetBySiteId(selectedSite.Id.Value);
                    }
                    else
                    {
                        // Tüm siteler için (SiteManager'ın tüm siteleri görmesi için)
                        duesList = _duesService.GetAll();
                    }
                }
                else
                {
                    // Admin/SuperAdmin: Tüm aidatlar
                    duesList = _duesService.GetAll();
                }

                // Filtreleme
                var selectedApartment = cmbApartmentFilter?.SelectedItem as FilterItem;
                if (selectedApartment?.Id.HasValue == true)
                {
                    duesList = duesList.Where(d => d.Flat?.ApartmentId == selectedApartment.Id.Value).ToList();
                }

                var statusFilter = cmbStatusFilter?.SelectedItem?.ToString();
                if (statusFilter == "Bekleyen")
                {
                    duesList = duesList.Where(d => !d.IsPaid).ToList();
                }
                else if (statusFilter == "Ödenen")
                {
                    duesList = duesList.Where(d => d.IsPaid).ToList();
                }

                var monthFilter = cmbMonthFilter?.SelectedIndex ?? 0;
                if (monthFilter > 0)
                {
                    duesList = duesList.Where(d => d.Month == monthFilter).ToList();
                }

                var yearFilter = cmbYearFilter?.SelectedItem?.ToString();
                if (yearFilter != null && yearFilter != "Tümü" && int.TryParse(yearFilter, out int year))
                {
                    duesList = duesList.Where(d => d.Year == year).ToList();
                }

                // Display data
                var displayData = duesList.Select(d => new
                {
                    d.Id,
                    Site = d.Flat?.Apartment?.Block?.Site?.Name ?? "-",
                    Block = d.Flat?.Apartment?.Block?.Name ?? "-",
                    Apartment = d.Flat?.Apartment?.Name ?? "-",
                    DaireNo = d.Flat?.DoorNumber ?? 0,
                    Kat = d.Flat?.Floor ?? 0,
                    Tutar = d.Amount.ToString("N2") + " TL",
                    Ay = GetMonthName(d.Month),
                    Yil = d.Year,
                    Durum = d.IsPaid ? "Ödendi" : "Bekliyor",
                    IsPaid = d.IsPaid,
                    FlatId = d.FlatId,
                    Month = d.Month,
                    Year = d.Year,
                    Amount = d.Amount
                }).ToList();

                gcDues.DataSource = displayData;
                gvDues.PopulateColumns();

                // Sütun ayarları
                ConfigureColumns();

                // İstatistikleri güncelle
                UpdateStatistics(duesList);
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
            if (gvDues.Columns["Id"] != null) gvDues.Columns["Id"].Visible = false;
            if (gvDues.Columns["IsPaid"] != null) gvDues.Columns["IsPaid"].Visible = false;
            if (gvDues.Columns["FlatId"] != null) gvDues.Columns["FlatId"].Visible = false;
            if (gvDues.Columns["Month"] != null) gvDues.Columns["Month"].Visible = false;
            if (gvDues.Columns["Year"] != null) gvDues.Columns["Year"].Visible = false;
            if (gvDues.Columns["Amount"] != null) gvDues.Columns["Amount"].Visible = false;

            // Site (SiteManager için)
            if (_currentUser?.Role == "SiteManager" || _currentUser?.Role == "SuperAdmin" || _currentUser?.Role == "Admin")
            {
                if (gvDues.Columns["Site"] != null)
                {
                    gvDues.Columns["Site"].VisibleIndex = visibleIndex++;
                    gvDues.Columns["Site"].Caption = "Site";
                    gvDues.Columns["Site"].Width = 120;
                }
            }

            if (gvDues.Columns["Block"] != null)
            {
                gvDues.Columns["Block"].VisibleIndex = visibleIndex++;
                gvDues.Columns["Block"].Caption = "Blok";
                gvDues.Columns["Block"].Width = 100;
            }

            if (gvDues.Columns["Apartment"] != null)
            {
                gvDues.Columns["Apartment"].VisibleIndex = visibleIndex++;
                gvDues.Columns["Apartment"].Caption = "Apartman";
                gvDues.Columns["Apartment"].Width = 120;
            }

            if (gvDues.Columns["DaireNo"] != null)
            {
                gvDues.Columns["DaireNo"].VisibleIndex = visibleIndex++;
                gvDues.Columns["DaireNo"].Caption = "Daire No";
                gvDues.Columns["DaireNo"].Width = 80;
                gvDues.Columns["DaireNo"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }

            if (gvDues.Columns["Kat"] != null)
            {
                gvDues.Columns["Kat"].VisibleIndex = visibleIndex++;
                gvDues.Columns["Kat"].Caption = "Kat";
                gvDues.Columns["Kat"].Width = 60;
                gvDues.Columns["Kat"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }

            if (gvDues.Columns["Tutar"] != null)
            {
                gvDues.Columns["Tutar"].VisibleIndex = visibleIndex++;
                gvDues.Columns["Tutar"].Caption = "Tutar";
                gvDues.Columns["Tutar"].Width = 120;
                gvDues.Columns["Tutar"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            }

            if (gvDues.Columns["Ay"] != null)
            {
                gvDues.Columns["Ay"].VisibleIndex = visibleIndex++;
                gvDues.Columns["Ay"].Caption = "Ay";
                gvDues.Columns["Ay"].Width = 100;
            }

            if (gvDues.Columns["Yil"] != null)
            {
                gvDues.Columns["Yil"].VisibleIndex = visibleIndex++;
                gvDues.Columns["Yil"].Caption = "Yıl";
                gvDues.Columns["Yil"].Width = 80;
                gvDues.Columns["Yil"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }

            if (gvDues.Columns["Durum"] != null)
            {
                gvDues.Columns["Durum"].VisibleIndex = visibleIndex++;
                gvDues.Columns["Durum"].Caption = "Durum";
                gvDues.Columns["Durum"].Width = 100;
            }
        }

        /// <summary>
        /// İstatistikleri günceller
        /// </summary>
        private void UpdateStatistics(List<Dues> duesList)
        {
            int total = duesList.Count;
            int pending = duesList.Count(d => !d.IsPaid);
            int paid = duesList.Count(d => d.IsPaid);
            decimal totalAmount = duesList.Sum(d => d.Amount);

            lblTotalCount.Text = total.ToString();
            lblPendingCount.Text = pending.ToString();
            lblPaidCount.Text = paid.ToString();
            lblTotalAmount.Text = totalAmount.ToString("N2") + " TL";
        }

        /// <summary>
        /// Ay adını döndürür
        /// </summary>
        private string GetMonthName(int month)
        {
            string[] months = { "", "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
                "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };
            return month >= 1 && month <= 12 ? months[month] : month.ToString();
        }

        /// <summary>
        /// Grid row seçimi değiştiğinde
        /// </summary>
        private void GvDues_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            bool hasSelection = gvDues.FocusedRowHandle >= 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
            btnMarkAsPaid.Enabled = hasSelection && !IsSelectedRowPaid();
        }

        /// <summary>
        /// Seçili satırın ödenip ödenmediğini kontrol eder
        /// </summary>
        private bool IsSelectedRowPaid()
        {
            if (gvDues.FocusedRowHandle < 0) return false;
            var row = gvDues.GetRow(gvDues.FocusedRowHandle);
            if (row == null) return false;
            var isPaidProp = row.GetType().GetProperty("IsPaid");
            return isPaidProp != null && (bool)isPaidProp.GetValue(row);
        }

        /// <summary>
        /// Custom draw cell - Durum badge'i için
        /// </summary>
        private void GvDues_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column?.FieldName == "Durum")
            {
                var row = gvDues.GetRow(e.RowHandle);
                if (row != null)
                {
                    var isPaidProp = row.GetType().GetProperty("IsPaid");
                    bool isPaid = isPaidProp != null && (bool)isPaidProp.GetValue(row);

                    if (isPaid)
                    {
                        e.Appearance.BackColor = Color.FromArgb(34, 197, 94);
                        e.Appearance.ForeColor = Color.White;
                    }
                    else
                    {
                        e.Appearance.BackColor = Color.FromArgb(234, 179, 8);
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
        private void GvDues_DoubleClick(object sender, EventArgs e)
        {
            BtnEdit_Click(sender, e);
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
            cmbApartmentFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndex = 0;
            cmbMonthFilter.SelectedIndex = 0;
            cmbYearFilter.SelectedIndex = 0;
            LoadData();
        }

        /// <summary>
        /// New Dues button click
        /// </summary>
        private void BtnNewDues_Click(object sender, EventArgs e)
        {
            var frm = new FrmDuesManagement(null, _currentUser);
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
            if (gvDues.FocusedRowHandle < 0) return;

            var row = gvDues.GetRow(gvDues.FocusedRowHandle);
            if (row == null) return;

            var idProp = row.GetType().GetProperty("Id");
            if (idProp == null) return;

            int duesId = (int)idProp.GetValue(row);
            var dues = _duesService.GetById(duesId);

            if (dues != null)
            {
                var frm = new FrmDuesManagement(dues, _currentUser);
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
            if (gvDues.FocusedRowHandle < 0) return;

            var row = gvDues.GetRow(gvDues.FocusedRowHandle);
            if (row == null) return;

            var idProp = row.GetType().GetProperty("Id");
            if (idProp == null) return;

            int duesId = (int)idProp.GetValue(row);

            if (Swal.Confirm("Bu aidat kaydını silmek istediğinize emin misiniz?", "Aidat Sil",
                "Evet, Sil", Color.FromArgb(239, 68, 68)))
            {
                string result = _duesService.Delete(duesId);
                if (string.IsNullOrEmpty(result))
                {
                    Swal.Success("Aidat başarıyla silindi.");
                    LoadData();
                }
                else
                {
                    Swal.Error("Silme hatası: " + result);
                }
            }
        }

        /// <summary>
        /// Mark as paid button click
        /// </summary>
        private void BtnMarkAsPaid_Click(object sender, EventArgs e)
        {
            if (gvDues.FocusedRowHandle < 0) return;

            var row = gvDues.GetRow(gvDues.FocusedRowHandle);
            if (row == null) return;

            var idProp = row.GetType().GetProperty("Id");
            if (idProp == null) return;

            int duesId = (int)idProp.GetValue(row);

            if (Swal.Confirm("Bu aidatı ödendi olarak işaretlemek istediğinize emin misiniz?", "Aidat Ödendi",
                "Evet, Ödendi İşaretle", Color.FromArgb(34, 197, 94)))
            {
                string result = _duesService.MarkAsPaid(duesId);
                if (string.IsNullOrEmpty(result))
                {
                    Swal.Success("Aidat ödendi olarak işaretlendi.");
                    LoadData();
                }
                else
                {
                    Swal.Error("İşlem hatası: " + result);
                }
            }
        }

        /// <summary>
        /// Bulk create button click
        /// </summary>
        private void BtnBulkCreate_Click(object sender, EventArgs e)
        {
            var frm = new FrmDuesBulkCreate(_currentUser);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }
    }
}

