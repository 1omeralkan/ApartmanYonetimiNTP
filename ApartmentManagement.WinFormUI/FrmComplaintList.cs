#nullable disable
// FrmComplaintList.cs
// Şikayet/Talep Listesi Formu - Şikayet/talepleri listeler ve yönetir
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
    /// Şikayet/Talep listesi formu - Resident, SiteManager ve ApartmentManager için
    /// </summary>
    public partial class FrmComplaintList : DevExpress.XtraEditors.XtraForm
    {
        private IComplaint _complaintService;
        private IFlat _flatService;
        private ISite _siteService;
        private IApartment _apartmentService;
        private IUser _userService;
        private User _currentUser;

        // Controls
        private GridControl gcComplaints;
        private GridView gvComplaints;
        private ComboBoxEdit cmbSiteFilter;
        private ComboBoxEdit cmbApartmentFilter;
        private ComboBoxEdit cmbTypeFilter;
        private ComboBoxEdit cmbStatusFilter;
        private SimpleButton btnFilter;
        private SimpleButton btnClear;
        private SimpleButton btnNewComplaint;
        private SimpleButton btnView;
        private SimpleButton btnUpdateStatus;
        private SimpleButton btnDelete;
        private LabelControl lblTitle;

        // Stat Cards
        private Panel pnlCardTotal;
        private Panel pnlCardPending;
        private Panel pnlCardInProgress;
        private Panel pnlCardResolved;
        private LabelControl lblTotalCount;
        private LabelControl lblPendingCount;
        private LabelControl lblInProgressCount;
        private LabelControl lblResolvedCount;

        /// <summary>
        /// FrmComplaintList constructor
        /// </summary>
        public FrmComplaintList(User user)
        {
            _currentUser = user;
            _complaintService = new SComplaint();
            _flatService = new SFlat();
            _siteService = new SSite();
            _apartmentService = new SApartment();
            _userService = new SUser();
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
            this.Text = "Şikayet/Talep Yönetimi";
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            int leftMargin = 20;
            int currentY = 20;
            int contentWidth = this.Width - (leftMargin * 2);

            // ========== HEADER SECTION ==========
            lblTitle = new LabelControl();
            lblTitle.Text = "📝 Şikayet/Talep Yönetimi";
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
                Color.FromArgb(59, 130, 246), "📋", "Toplam", "0", ref lblTotalCount);
            this.Controls.Add(pnlCardTotal);

            pnlCardPending = CreateStatCard(leftMargin + cardWidth + cardSpacing, currentY, cardWidth, cardHeight,
                Color.FromArgb(234, 179, 8), "⏳", "Beklemede", "0", ref lblPendingCount);
            this.Controls.Add(pnlCardPending);

            pnlCardInProgress = CreateStatCard(leftMargin + (cardWidth + cardSpacing) * 2, currentY, cardWidth, cardHeight,
                Color.FromArgb(59, 130, 246), "🔍", "İnceleniyor", "0", ref lblInProgressCount);
            this.Controls.Add(pnlCardInProgress);

            pnlCardResolved = CreateStatCard(leftMargin + (cardWidth + cardSpacing) * 3, currentY, cardWidth, cardHeight,
                Color.FromArgb(34, 197, 94), "✅", "Çözüldü", "0", ref lblResolvedCount);
            this.Controls.Add(pnlCardResolved);

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

            // Type Filter
            var lblType = new LabelControl();
            lblType.Text = "Tip";
            lblType.Location = new Point(filterX, filterY);
            lblType.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblType);

            cmbTypeFilter = new ComboBoxEdit();
            cmbTypeFilter.Location = new Point(filterX, filterY + 18);
            cmbTypeFilter.Size = new Size(120, 26);
            cmbTypeFilter.Properties.Items.AddRange(new[] { "Tümü", "Şikayet", "Talep" });
            cmbTypeFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbTypeFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
            cmbTypeFilter.SelectedIndex = 0;
            pnlFilter.Controls.Add(cmbTypeFilter);
            filterX += 140;

            // Status Filter
            var lblStatus = new LabelControl();
            lblStatus.Text = "Durum";
            lblStatus.Location = new Point(filterX, filterY);
            lblStatus.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblStatus);

            cmbStatusFilter = new ComboBoxEdit();
            cmbStatusFilter.Location = new Point(filterX, filterY + 18);
            cmbStatusFilter.Size = new Size(150, 26);
            cmbStatusFilter.Properties.Items.AddRange(new[] { "Tümü", "Beklemede", "İnceleniyor", "Çözüldü", "Reddedildi" });
            cmbStatusFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbStatusFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
            cmbStatusFilter.SelectedIndex = 0;
            pnlFilter.Controls.Add(cmbStatusFilter);
            filterX += 170;

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
            // Resident için sadece yeni şikayet/talep oluşturma
            if (_currentUser?.Role == "Resident")
            {
                btnNewComplaint = new SimpleButton();
                btnNewComplaint.Text = "+ Yeni Şikayet/Talep";
                btnNewComplaint.Size = new Size(180, 35);
                btnNewComplaint.Location = new Point(leftMargin, currentY);
                btnNewComplaint.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
                btnNewComplaint.Appearance.BackColor = Color.FromArgb(34, 197, 94);
                btnNewComplaint.Appearance.ForeColor = Color.White;
                btnNewComplaint.Appearance.Options.UseBackColor = true;
                btnNewComplaint.Appearance.Options.UseForeColor = true;
                btnNewComplaint.Cursor = Cursors.Hand;
                btnNewComplaint.Click += BtnNewComplaint_Click;
                this.Controls.Add(btnNewComplaint);

                btnView = new SimpleButton();
                btnView.Text = "👁️ Detay";
                btnView.Size = new Size(120, 35);
                btnView.Location = new Point(contentWidth - 120, currentY);
                btnView.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
                btnView.Appearance.BackColor = Color.FromArgb(59, 130, 246);
                btnView.Appearance.ForeColor = Color.White;
                btnView.Appearance.Options.UseBackColor = true;
                btnView.Appearance.Options.UseForeColor = true;
                btnView.Cursor = Cursors.Hand;
                btnView.Enabled = false;
                btnView.Click += BtnView_Click;
                this.Controls.Add(btnView);
            }
            else
            {
                // Manager rolleri için yönetim butonları
                btnNewComplaint = new SimpleButton();
                btnNewComplaint.Text = "+ Yeni Şikayet/Talep";
                btnNewComplaint.Size = new Size(180, 35);
                btnNewComplaint.Location = new Point(leftMargin, currentY);
                btnNewComplaint.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
                btnNewComplaint.Appearance.BackColor = Color.FromArgb(34, 197, 94);
                btnNewComplaint.Appearance.ForeColor = Color.White;
                btnNewComplaint.Appearance.Options.UseBackColor = true;
                btnNewComplaint.Appearance.Options.UseForeColor = true;
                btnNewComplaint.Cursor = Cursors.Hand;
                btnNewComplaint.Click += BtnNewComplaint_Click;
                this.Controls.Add(btnNewComplaint);

                btnView = new SimpleButton();
                btnView.Text = "👁️ Detay";
                btnView.Size = new Size(120, 35);
                btnView.Location = new Point(contentWidth - 360, currentY);
                btnView.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
                btnView.Appearance.BackColor = Color.FromArgb(59, 130, 246);
                btnView.Appearance.ForeColor = Color.White;
                btnView.Appearance.Options.UseBackColor = true;
                btnView.Appearance.Options.UseForeColor = true;
                btnView.Cursor = Cursors.Hand;
                btnView.Enabled = false;
                btnView.Click += BtnView_Click;
                this.Controls.Add(btnView);

                btnUpdateStatus = new SimpleButton();
                btnUpdateStatus.Text = "✏️ Durum Güncelle";
                btnUpdateStatus.Size = new Size(150, 35);
                btnUpdateStatus.Location = new Point(contentWidth - 240, currentY);
                btnUpdateStatus.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
                btnUpdateStatus.Appearance.BackColor = Color.FromArgb(234, 179, 8);
                btnUpdateStatus.Appearance.ForeColor = Color.White;
                btnUpdateStatus.Appearance.Options.UseBackColor = true;
                btnUpdateStatus.Appearance.Options.UseForeColor = true;
                btnUpdateStatus.Cursor = Cursors.Hand;
                btnUpdateStatus.Enabled = false;
                btnUpdateStatus.Click += BtnUpdateStatus_Click;
                this.Controls.Add(btnUpdateStatus);

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
            gcComplaints = new GridControl();
            gvComplaints = new GridView(gcComplaints);
            gcComplaints.MainView = gvComplaints;
            gcComplaints.Location = new Point(leftMargin, currentY);
            gcComplaints.Size = new Size(contentWidth, this.Height - currentY - 50);
            gcComplaints.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // Grid View Settings
            gvComplaints.OptionsBehavior.Editable = false;
            gvComplaints.OptionsView.ShowGroupPanel = false;
            gvComplaints.OptionsView.ShowIndicator = false;
            gvComplaints.RowHeight = 50;
            gvComplaints.Appearance.HeaderPanel.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            gvComplaints.Appearance.Row.Font = new Font("Tahoma", 9F);
            gvComplaints.FocusedRowChanged += GvComplaints_FocusedRowChanged;
            gvComplaints.CustomDrawCell += GvComplaints_CustomDrawCell;
            gvComplaints.DoubleClick += GvComplaints_DoubleClick;

            this.Controls.Add(gcComplaints);

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
                List<Complaint> complaintList;

                // Role göre filtreleme
                if (_currentUser?.Role == "Resident")
                {
                    // Resident: Sadece kendi şikayet/taleplerini görüntüle
                    complaintList = _complaintService.GetByUserId(_currentUser.Id);
                }
                else if (_currentUser?.Role == "ApartmentManager")
                {
                    // ApartmentManager: Kendi apartmanındaki şikayet/talepler
                    var userFlat = _flatService.GetResidentFlat(_currentUser.Id);
                    if (userFlat != null)
                    {
                        complaintList = _complaintService.GetByApartmentId(userFlat.ApartmentId);
                    }
                    else
                    {
                        complaintList = new List<Complaint>();
                    }
                }
                else if (_currentUser?.Role == "SiteManager")
                {
                    // SiteManager: Site bazlı filtreleme
                    var selectedSite = cmbSiteFilter?.SelectedItem as FilterItem;
                    if (selectedSite?.Id.HasValue == true)
                    {
                        complaintList = _complaintService.GetBySiteId(selectedSite.Id.Value);
                    }
                    else
                    {
                        // Tüm siteler için
                        complaintList = _complaintService.GetAll();
                    }
                }
                else
                {
                    // Admin/SuperAdmin: Tüm şikayet/talepler
                    complaintList = _complaintService.GetAll();
                }

                // Filtreleme
                var selectedApartment = cmbApartmentFilter?.SelectedItem as FilterItem;
                if (selectedApartment?.Id.HasValue == true)
                {
                    complaintList = complaintList.Where(c => c.ApartmentId == selectedApartment.Id.Value).ToList();
                }

                var typeFilter = cmbTypeFilter?.SelectedItem?.ToString();
                if (typeFilter != null && typeFilter != "Tümü")
                {
                    complaintList = complaintList.Where(c => c.Type == typeFilter).ToList();
                }

                var statusFilter = cmbStatusFilter?.SelectedItem?.ToString();
                if (statusFilter != null && statusFilter != "Tümü")
                {
                    complaintList = complaintList.Where(c => c.Status == statusFilter).ToList();
                }

                // Display data
                var displayData = complaintList.Select(c => new
                {
                    c.Id,
                    Baslik = c.Title ?? "-",
                    Aciklama = c.Description ?? "-",
                    Tip = c.Type ?? "Şikayet",
                    Durum = c.Status ?? "Beklemede",
                    Olusturan = c.CreatedByUser != null ? $"{c.CreatedByUser.FirstName} {c.CreatedByUser.LastName}" : "-",
                    Atanan = c.AssignedToUser != null ? $"{c.AssignedToUser.FirstName} {c.AssignedToUser.LastName}" : "-",
                    Tarih = c.CreatedDate.ToString("dd.MM.yyyy"),
                    Status = c.Status,
                    SiteId = c.SiteId,
                    ApartmentId = c.ApartmentId
                }).ToList();

                gcComplaints.DataSource = displayData;
                gvComplaints.PopulateColumns();

                // Sütun ayarları
                ConfigureColumns();

                // İstatistikleri güncelle
                UpdateStatistics(complaintList);
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
            if (gvComplaints.Columns["Id"] != null) gvComplaints.Columns["Id"].Visible = false;
            if (gvComplaints.Columns["Status"] != null) gvComplaints.Columns["Status"].Visible = false;
            if (gvComplaints.Columns["SiteId"] != null) gvComplaints.Columns["SiteId"].Visible = false;
            if (gvComplaints.Columns["ApartmentId"] != null) gvComplaints.Columns["ApartmentId"].Visible = false;

            if (gvComplaints.Columns["Baslik"] != null)
            {
                gvComplaints.Columns["Baslik"].VisibleIndex = visibleIndex++;
                gvComplaints.Columns["Baslik"].Caption = "Başlık";
                gvComplaints.Columns["Baslik"].Width = 200;
            }

            if (gvComplaints.Columns["Aciklama"] != null)
            {
                gvComplaints.Columns["Aciklama"].VisibleIndex = visibleIndex++;
                gvComplaints.Columns["Aciklama"].Caption = "Açıklama";
                gvComplaints.Columns["Aciklama"].Width = 300;
            }

            if (gvComplaints.Columns["Tip"] != null)
            {
                gvComplaints.Columns["Tip"].VisibleIndex = visibleIndex++;
                gvComplaints.Columns["Tip"].Caption = "Tip";
                gvComplaints.Columns["Tip"].Width = 100;
            }

            if (gvComplaints.Columns["Durum"] != null)
            {
                gvComplaints.Columns["Durum"].VisibleIndex = visibleIndex++;
                gvComplaints.Columns["Durum"].Caption = "Durum";
                gvComplaints.Columns["Durum"].Width = 120;
            }

            if (gvComplaints.Columns["Olusturan"] != null)
            {
                gvComplaints.Columns["Olusturan"].VisibleIndex = visibleIndex++;
                gvComplaints.Columns["Olusturan"].Caption = "Oluşturan";
                gvComplaints.Columns["Olusturan"].Width = 150;
            }

            if (_currentUser?.Role != "Resident" && gvComplaints.Columns["Atanan"] != null)
            {
                gvComplaints.Columns["Atanan"].VisibleIndex = visibleIndex++;
                gvComplaints.Columns["Atanan"].Caption = "Atanan";
                gvComplaints.Columns["Atanan"].Width = 150;
            }

            if (gvComplaints.Columns["Tarih"] != null)
            {
                gvComplaints.Columns["Tarih"].VisibleIndex = visibleIndex++;
                gvComplaints.Columns["Tarih"].Caption = "Tarih";
                gvComplaints.Columns["Tarih"].Width = 120;
            }
        }

        /// <summary>
        /// İstatistikleri günceller
        /// </summary>
        private void UpdateStatistics(List<Complaint> complaintList)
        {
            int total = complaintList.Count;
            int pending = complaintList.Count(c => c.Status == "Beklemede");
            int inProgress = complaintList.Count(c => c.Status == "İnceleniyor");
            int resolved = complaintList.Count(c => c.Status == "Çözüldü");

            lblTotalCount.Text = total.ToString();
            lblPendingCount.Text = pending.ToString();
            lblInProgressCount.Text = inProgress.ToString();
            lblResolvedCount.Text = resolved.ToString();
        }

        /// <summary>
        /// Grid row seçimi değiştiğinde
        /// </summary>
        private void GvComplaints_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            bool hasSelection = gvComplaints.FocusedRowHandle >= 0;
            if (btnView != null) btnView.Enabled = hasSelection;
            if (btnUpdateStatus != null) btnUpdateStatus.Enabled = hasSelection;
            if (btnDelete != null) btnDelete.Enabled = hasSelection;
        }

        /// <summary>
        /// Custom draw cell - Durum için
        /// </summary>
        private void GvComplaints_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column?.FieldName == "Durum")
            {
                var row = gvComplaints.GetRow(e.RowHandle);
                if (row != null)
                {
                    var statusProp = row.GetType().GetProperty("Status");
                    string status = statusProp != null ? statusProp.GetValue(row)?.ToString() : "";

                    if (status == "Beklemede")
                    {
                        e.Appearance.BackColor = Color.FromArgb(234, 179, 8);
                        e.Appearance.ForeColor = Color.White;
                    }
                    else if (status == "İnceleniyor")
                    {
                        e.Appearance.BackColor = Color.FromArgb(59, 130, 246);
                        e.Appearance.ForeColor = Color.White;
                    }
                    else if (status == "Çözüldü")
                    {
                        e.Appearance.BackColor = Color.FromArgb(34, 197, 94);
                        e.Appearance.ForeColor = Color.White;
                    }
                    else if (status == "Reddedildi")
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
        private void GvComplaints_DoubleClick(object sender, EventArgs e)
        {
            BtnView_Click(sender, e);
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
            cmbTypeFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndex = 0;
            LoadData();
        }

        /// <summary>
        /// New Complaint button click
        /// </summary>
        private void BtnNewComplaint_Click(object sender, EventArgs e)
        {
            var frm = new FrmComplaintManagement(null, _currentUser);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        /// <summary>
        /// View button click
        /// </summary>
        private void BtnView_Click(object sender, EventArgs e)
        {
            if (gvComplaints.FocusedRowHandle < 0) return;

            var row = gvComplaints.GetRow(gvComplaints.FocusedRowHandle);
            if (row == null) return;

            var idProp = row.GetType().GetProperty("Id");
            if (idProp == null) return;

            int complaintId = (int)idProp.GetValue(row);
            var complaint = _complaintService.GetById(complaintId);

            if (complaint != null)
            {
                var frm = new FrmComplaintManagement(complaint, _currentUser);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        /// <summary>
        /// Update Status button click
        /// </summary>
        private void BtnUpdateStatus_Click(object sender, EventArgs e)
        {
            if (gvComplaints.FocusedRowHandle < 0) return;

            var row = gvComplaints.GetRow(gvComplaints.FocusedRowHandle);
            if (row == null) return;

            var idProp = row.GetType().GetProperty("Id");
            if (idProp == null) return;

            int complaintId = (int)idProp.GetValue(row);
            var complaint = _complaintService.GetById(complaintId);

            if (complaint != null)
            {
                var frm = new FrmComplaintStatusUpdate(complaint, _currentUser);
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
            if (gvComplaints.FocusedRowHandle < 0) return;

            var row = gvComplaints.GetRow(gvComplaints.FocusedRowHandle);
            if (row == null) return;

            var idProp = row.GetType().GetProperty("Id");
            if (idProp == null) return;

            int complaintId = (int)idProp.GetValue(row);

            if (Swal.Confirm("Bu şikayet/talebi silmek istediğinize emin misiniz?", "Şikayet/Talep Sil",
                "Evet, Sil", Color.FromArgb(239, 68, 68)))
            {
                string result = _complaintService.Delete(complaintId);
                if (string.IsNullOrEmpty(result))
                {
                    Swal.Success("Şikayet/Talep başarıyla silindi.");
                    LoadData();
                }
                else
                {
                    Swal.Error("Silme hatası: " + result);
                }
            }
        }
    }
}

