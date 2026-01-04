#nullable disable
// FrmPaymentList.cs
// Ödeme Listesi Formu - Ödemeleri listeler ve yönetir
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
    /// Ödeme listesi formu - Tüm roller için (rol bazlı filtreleme)
    /// </summary>
    public partial class FrmPaymentList : DevExpress.XtraEditors.XtraForm
    {
        private IPayment _paymentService;
        private IFlat _flatService;
        private ISite _siteService;
        private IApartment _apartmentService;
        private User _currentUser;

        // Controls
        private GridControl gcPayments;
        private GridView gvPayments;
        private ComboBoxEdit cmbSiteFilter;
        private ComboBoxEdit cmbApartmentFilter;
        private ComboBoxEdit cmbTypeFilter;
        private DateEdit dtStartDate;
        private DateEdit dtEndDate;
        private SimpleButton btnFilter;
        private SimpleButton btnClear;
        private SimpleButton btnViewDetail;
        private SimpleButton btnDelete;
        private LabelControl lblTitle;

        // Stat Cards
        private Panel pnlCardTotal;
        private Panel pnlCardTotalAmount;
        private Panel pnlCardThisMonth;
        private Panel pnlCardThisYear;
        private LabelControl lblTotalCount;
        private LabelControl lblTotalAmount;
        private LabelControl lblThisMonth;
        private LabelControl lblThisYear;

        /// <summary>
        /// FrmPaymentList constructor
        /// </summary>
        public FrmPaymentList(User user)
        {
            _currentUser = user;
            _paymentService = new SPayment();
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
            this.Text = "Ödeme Takibi";
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
            lblTitle.Text = "💳 Ödeme Takibi";
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
                Color.FromArgb(59, 130, 246), "📋", "Toplam Ödeme", "0", ref lblTotalCount);
            this.Controls.Add(pnlCardTotal);

            pnlCardTotalAmount = CreateStatCard(leftMargin + cardWidth + cardSpacing, currentY, cardWidth, cardHeight,
                Color.FromArgb(34, 197, 94), "💵", "Toplam Tutar", "0 TL", ref lblTotalAmount);
            this.Controls.Add(pnlCardTotalAmount);

            pnlCardThisMonth = CreateStatCard(leftMargin + (cardWidth + cardSpacing) * 2, currentY, cardWidth, cardHeight,
                Color.FromArgb(234, 179, 8), "📅", "Bu Ay", "0 TL", ref lblThisMonth);
            this.Controls.Add(pnlCardThisMonth);

            pnlCardThisYear = CreateStatCard(leftMargin + (cardWidth + cardSpacing) * 3, currentY, cardWidth, cardHeight,
                Color.FromArgb(239, 68, 68), "📆", "Bu Yıl", "0 TL", ref lblThisYear);
            this.Controls.Add(pnlCardThisYear);

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

            // Site Filter (SiteManager, Admin için)
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

            // Apartment Filter (ApartmentManager, SiteManager, Admin için)
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
            lblType.Text = "Ödeme Tipi";
            lblType.Location = new Point(filterX, filterY);
            lblType.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblType);

            cmbTypeFilter = new ComboBoxEdit();
            cmbTypeFilter.Location = new Point(filterX, filterY + 18);
            cmbTypeFilter.Size = new Size(120, 26);
            cmbTypeFilter.Properties.Items.AddRange(new[] { "Tümü", "Aidat", "Demirbaş", "Yakıt", "Diğer" });
            cmbTypeFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbTypeFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
            cmbTypeFilter.SelectedIndex = 0;
            pnlFilter.Controls.Add(cmbTypeFilter);
            filterX += 140;

            // Start Date
            var lblStartDate = new LabelControl();
            lblStartDate.Text = "Başlangıç";
            lblStartDate.Location = new Point(filterX, filterY);
            lblStartDate.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblStartDate);

            dtStartDate = new DateEdit();
            dtStartDate.Location = new Point(filterX, filterY + 18);
            dtStartDate.Size = new Size(120, 26);
            dtStartDate.Properties.Appearance.Font = new Font("Tahoma", 9F);
            dtStartDate.Properties.DisplayFormat.FormatString = "dd.MM.yyyy";
            dtStartDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dtStartDate.Properties.EditFormat.FormatString = "dd.MM.yyyy";
            dtStartDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dtStartDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            dtStartDate.Properties.Mask.EditMask = "dd.MM.yyyy";
            dtStartDate.Properties.NullText = "Başlangıç";
            pnlFilter.Controls.Add(dtStartDate);
            filterX += 140;

            // End Date
            var lblEndDate = new LabelControl();
            lblEndDate.Text = "Bitiş";
            lblEndDate.Location = new Point(filterX, filterY);
            lblEndDate.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblEndDate);

            dtEndDate = new DateEdit();
            dtEndDate.Location = new Point(filterX, filterY + 18);
            dtEndDate.Size = new Size(120, 26);
            dtEndDate.Properties.Appearance.Font = new Font("Tahoma", 9F);
            dtEndDate.Properties.DisplayFormat.FormatString = "dd.MM.yyyy";
            dtEndDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dtEndDate.Properties.EditFormat.FormatString = "dd.MM.yyyy";
            dtEndDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dtEndDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            dtEndDate.Properties.Mask.EditMask = "dd.MM.yyyy";
            dtEndDate.Properties.NullText = "Bitiş";
            pnlFilter.Controls.Add(dtEndDate);
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
            btnViewDetail = new SimpleButton();
            btnViewDetail.Text = "👁️ Detay";
            btnViewDetail.Size = new Size(120, 35);
            btnViewDetail.Location = new Point(contentWidth - 240, currentY);
            btnViewDetail.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnViewDetail.Appearance.BackColor = Color.FromArgb(59, 130, 246);
            btnViewDetail.Appearance.ForeColor = Color.White;
            btnViewDetail.Appearance.Options.UseBackColor = true;
            btnViewDetail.Appearance.Options.UseForeColor = true;
            btnViewDetail.Cursor = Cursors.Hand;
            btnViewDetail.Enabled = false;
            btnViewDetail.Click += BtnViewDetail_Click;
            this.Controls.Add(btnViewDetail);

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
            gcPayments = new GridControl();
            gvPayments = new GridView(gcPayments);
            gcPayments.MainView = gvPayments;
            gcPayments.Location = new Point(leftMargin, currentY);
            gcPayments.Size = new Size(contentWidth, this.Height - currentY - 50);
            gcPayments.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // Grid View Settings
            gvPayments.OptionsBehavior.Editable = false;
            gvPayments.OptionsView.ShowGroupPanel = false;
            gvPayments.OptionsView.ShowIndicator = false;
            gvPayments.RowHeight = 40;
            gvPayments.Appearance.HeaderPanel.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            gvPayments.Appearance.Row.Font = new Font("Tahoma", 9F);
            gvPayments.FocusedRowChanged += GvPayments_FocusedRowChanged;
            gvPayments.DoubleClick += GvPayments_DoubleClick;

            this.Controls.Add(gcPayments);

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
                List<Payment> paymentList;

                // Role göre filtreleme
                if (_currentUser?.Role == "Resident")
                {
                    // Resident: Sadece kendi ödemeleri
                    paymentList = _paymentService.GetByUserId(_currentUser.Id);
                }
                else if (_currentUser?.Role == "ApartmentManager")
                {
                    // ApartmentManager: Apartmanındaki tüm ödemeler
                    var userFlat = _flatService.GetResidentFlat(_currentUser.Id);
                    if (userFlat != null)
                    {
                        paymentList = _paymentService.GetByApartmentId(userFlat.ApartmentId);
                    }
                    else
                    {
                        paymentList = new List<Payment>();
                    }
                }
                else if (_currentUser?.Role == "SiteManager")
                {
                    // SiteManager: Site'deki tüm ödemeler
                    var selectedSite = cmbSiteFilter?.SelectedItem as FilterItem;
                    if (selectedSite?.Id.HasValue == true)
                    {
                        paymentList = _paymentService.GetBySiteId(selectedSite.Id.Value);
                    }
                    else
                    {
                        // Tüm siteler için
                        paymentList = _paymentService.GetAll();
                    }
                }
                else
                {
                    // Admin/SuperAdmin: Tüm ödemeler
                    paymentList = _paymentService.GetAll();
                }

                // Filtreleme
                var selectedApartment = cmbApartmentFilter?.SelectedItem as FilterItem;
                if (selectedApartment?.Id.HasValue == true)
                {
                    paymentList = paymentList.Where(p => p.Flat?.ApartmentId == selectedApartment.Id.Value).ToList();
                }

                var typeFilter = cmbTypeFilter?.SelectedItem?.ToString();
                if (typeFilter != null && typeFilter != "Tümü")
                {
                    paymentList = paymentList.Where(p => p.Type == typeFilter).ToList();
                }

                if (dtStartDate.EditValue is DateTime startDate)
                {
                    paymentList = paymentList.Where(p => p.Date >= startDate).ToList();
                }

                if (dtEndDate.EditValue is DateTime endDate)
                {
                    paymentList = paymentList.Where(p => p.Date <= endDate.AddDays(1)).ToList();
                }

                // Display data
                var displayData = paymentList.Select(p => new
                {
                    p.Id,
                    Site = p.Flat?.Apartment?.Block?.Site?.Name ?? "-",
                    Block = p.Flat?.Apartment?.Block?.Name ?? "-",
                    Apartment = p.Flat?.Apartment?.Name ?? "-",
                    DaireNo = p.Flat?.DoorNumber ?? 0,
                    Kat = p.Flat?.Floor ?? 0,
                    Tutar = p.Amount.ToString("N2") + " TL",
                    Tip = p.Type ?? "Aidat",
                    Tarih = p.Date.ToString("dd.MM.yyyy"),
                    FlatId = p.FlatId,
                    Amount = p.Amount,
                    Date = p.Date
                }).ToList();

                gcPayments.DataSource = displayData;
                gvPayments.PopulateColumns();

                // Sütun ayarları
                ConfigureColumns();

                // İstatistikleri güncelle
                UpdateStatistics(paymentList);
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
            if (gvPayments.Columns["Id"] != null) gvPayments.Columns["Id"].Visible = false;
            if (gvPayments.Columns["FlatId"] != null) gvPayments.Columns["FlatId"].Visible = false;
            if (gvPayments.Columns["Amount"] != null) gvPayments.Columns["Amount"].Visible = false;
            if (gvPayments.Columns["Date"] != null) gvPayments.Columns["Date"].Visible = false;

            // Site (SiteManager, Admin için)
            if (_currentUser?.Role == "SiteManager" || _currentUser?.Role == "SuperAdmin" || _currentUser?.Role == "Admin")
            {
                if (gvPayments.Columns["Site"] != null)
                {
                    gvPayments.Columns["Site"].VisibleIndex = visibleIndex++;
                    gvPayments.Columns["Site"].Caption = "Site";
                    gvPayments.Columns["Site"].Width = 120;
                }
            }

            if (gvPayments.Columns["Block"] != null)
            {
                gvPayments.Columns["Block"].VisibleIndex = visibleIndex++;
                gvPayments.Columns["Block"].Caption = "Blok";
                gvPayments.Columns["Block"].Width = 100;
            }

            if (gvPayments.Columns["Apartment"] != null)
            {
                gvPayments.Columns["Apartment"].VisibleIndex = visibleIndex++;
                gvPayments.Columns["Apartment"].Caption = "Apartman";
                gvPayments.Columns["Apartment"].Width = 120;
            }

            if (gvPayments.Columns["DaireNo"] != null)
            {
                gvPayments.Columns["DaireNo"].VisibleIndex = visibleIndex++;
                gvPayments.Columns["DaireNo"].Caption = "Daire No";
                gvPayments.Columns["DaireNo"].Width = 80;
                gvPayments.Columns["DaireNo"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }

            if (gvPayments.Columns["Kat"] != null)
            {
                gvPayments.Columns["Kat"].VisibleIndex = visibleIndex++;
                gvPayments.Columns["Kat"].Caption = "Kat";
                gvPayments.Columns["Kat"].Width = 60;
                gvPayments.Columns["Kat"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }

            if (gvPayments.Columns["Tutar"] != null)
            {
                gvPayments.Columns["Tutar"].VisibleIndex = visibleIndex++;
                gvPayments.Columns["Tutar"].Caption = "Tutar";
                gvPayments.Columns["Tutar"].Width = 120;
                gvPayments.Columns["Tutar"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            }

            if (gvPayments.Columns["Tip"] != null)
            {
                gvPayments.Columns["Tip"].VisibleIndex = visibleIndex++;
                gvPayments.Columns["Tip"].Caption = "Ödeme Tipi";
                gvPayments.Columns["Tip"].Width = 120;
            }

            if (gvPayments.Columns["Tarih"] != null)
            {
                gvPayments.Columns["Tarih"].VisibleIndex = visibleIndex++;
                gvPayments.Columns["Tarih"].Caption = "Tarih";
                gvPayments.Columns["Tarih"].Width = 120;
            }
        }

        /// <summary>
        /// İstatistikleri günceller
        /// </summary>
        private void UpdateStatistics(List<Payment> paymentList)
        {
            int total = paymentList.Count;
            decimal totalAmount = paymentList.Sum(p => p.Amount);

            var now = DateTime.Now;
            var thisMonth = paymentList.Where(p => p.Date.Year == now.Year && p.Date.Month == now.Month).Sum(p => p.Amount);
            var thisYear = paymentList.Where(p => p.Date.Year == now.Year).Sum(p => p.Amount);

            lblTotalCount.Text = total.ToString();
            lblTotalAmount.Text = totalAmount.ToString("N2") + " TL";
            lblThisMonth.Text = thisMonth.ToString("N2") + " TL";
            lblThisYear.Text = thisYear.ToString("N2") + " TL";
        }

        /// <summary>
        /// Grid row seçimi değiştiğinde
        /// </summary>
        private void GvPayments_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            bool hasSelection = gvPayments.FocusedRowHandle >= 0;
            btnViewDetail.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        /// <summary>
        /// Grid double click
        /// </summary>
        private void GvPayments_DoubleClick(object sender, EventArgs e)
        {
            BtnViewDetail_Click(sender, e);
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
            dtStartDate.EditValue = null;
            dtEndDate.EditValue = null;
            LoadData();
        }

        /// <summary>
        /// View Detail button click
        /// </summary>
        private void BtnViewDetail_Click(object sender, EventArgs e)
        {
            if (gvPayments.FocusedRowHandle < 0) return;

            var row = gvPayments.GetRow(gvPayments.FocusedRowHandle);
            if (row == null) return;

            var idProp = row.GetType().GetProperty("Id");
            if (idProp == null) return;

            int paymentId = (int)idProp.GetValue(row);
            var payment = _paymentService.GetById(paymentId);

            if (payment != null)
            {
                var frm = new FrmPaymentDetail(payment);
                frm.ShowDialog();
            }
        }

        /// <summary>
        /// Delete button click
        /// </summary>
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (gvPayments.FocusedRowHandle < 0) return;

            var row = gvPayments.GetRow(gvPayments.FocusedRowHandle);
            if (row == null) return;

            var idProp = row.GetType().GetProperty("Id");
            if (idProp == null) return;

            int paymentId = (int)idProp.GetValue(row);

            if (Swal.Confirm("Bu ödeme kaydını silmek istediğinize emin misiniz?", "Ödeme Sil",
                "Evet, Sil", Color.FromArgb(239, 68, 68)))
            {
                string result = _paymentService.Delete(paymentId);
                if (string.IsNullOrEmpty(result))
                {
                    Swal.Success("Ödeme başarıyla silindi.");
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

