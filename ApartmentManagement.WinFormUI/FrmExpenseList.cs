#nullable disable
// FrmExpenseList.cs
// Gider Listesi Formu - Giderleri listeler ve yönetir
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
    /// Gider listesi formu - SiteManager ve ApartmentManager için
    /// </summary>
    public partial class FrmExpenseList : DevExpress.XtraEditors.XtraForm
    {
        private IExpense _expenseService;
        private IFlat _flatService;
        private ISite _siteService;
        private IApartment _apartmentService;
        private IBlock _blockService;
        private User _currentUser;

        // Controls
        private GridControl gcExpenses;
        private GridView gvExpenses;
        private ComboBoxEdit cmbSiteFilter;
        private ComboBoxEdit cmbApartmentFilter;
        private ComboBoxEdit cmbCategoryFilter;
        private DateEdit dtStartDate;
        private DateEdit dtEndDate;
        private SimpleButton btnFilter;
        private SimpleButton btnClear;
        private SimpleButton btnNewExpense;
        private SimpleButton btnEdit;
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
        /// FrmExpenseList constructor
        /// </summary>
        public FrmExpenseList(User user)
        {
            _currentUser = user;
            _expenseService = new SExpense();
            _flatService = new SFlat();
            _siteService = new SSite();
            _apartmentService = new SApartment();
            _blockService = new SBlock();
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
            this.Text = "Gider Yönetimi";
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
            lblTitle.Text = "📊 Gider Yönetimi";
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
                Color.FromArgb(239, 68, 68), "📋", "Toplam Gider", "0", ref lblTotalCount);
            this.Controls.Add(pnlCardTotal);

            pnlCardTotalAmount = CreateStatCard(leftMargin + cardWidth + cardSpacing, currentY, cardWidth, cardHeight,
                Color.FromArgb(234, 179, 8), "💵", "Toplam Tutar", "0 TL", ref lblTotalAmount);
            this.Controls.Add(pnlCardTotalAmount);

            pnlCardThisMonth = CreateStatCard(leftMargin + (cardWidth + cardSpacing) * 2, currentY, cardWidth, cardHeight,
                Color.FromArgb(59, 130, 246), "📅", "Bu Ay", "0 TL", ref lblThisMonth);
            this.Controls.Add(pnlCardThisMonth);

            pnlCardThisYear = CreateStatCard(leftMargin + (cardWidth + cardSpacing) * 3, currentY, cardWidth, cardHeight,
                Color.FromArgb(34, 197, 94), "📆", "Bu Yıl", "0 TL", ref lblThisYear);
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

            // Category Filter
            var lblCategory = new LabelControl();
            lblCategory.Text = "Kategori";
            lblCategory.Location = new Point(filterX, filterY);
            lblCategory.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblCategory);

            cmbCategoryFilter = new ComboBoxEdit();
            cmbCategoryFilter.Location = new Point(filterX, filterY + 18);
            cmbCategoryFilter.Size = new Size(150, 26);
            cmbCategoryFilter.Properties.Items.AddRange(new[] { "Tümü", "Bakım", "Temizlik", "Elektrik", "Su", "Güvenlik", "Yakıt", "Diğer" });
            cmbCategoryFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbCategoryFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
            cmbCategoryFilter.SelectedIndex = 0;
            pnlFilter.Controls.Add(cmbCategoryFilter);
            filterX += 170;

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
            btnNewExpense = new SimpleButton();
            btnNewExpense.Text = "+ Yeni Gider";
            btnNewExpense.Size = new Size(130, 35);
            btnNewExpense.Location = new Point(leftMargin, currentY);
            btnNewExpense.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnNewExpense.Appearance.BackColor = Color.FromArgb(34, 197, 94);
            btnNewExpense.Appearance.ForeColor = Color.White;
            btnNewExpense.Appearance.Options.UseBackColor = true;
            btnNewExpense.Appearance.Options.UseForeColor = true;
            btnNewExpense.Cursor = Cursors.Hand;
            btnNewExpense.Click += BtnNewExpense_Click;
            this.Controls.Add(btnNewExpense);

            btnEdit = new SimpleButton();
            btnEdit.Text = "✏️ Düzenle";
            btnEdit.Size = new Size(120, 35);
            btnEdit.Location = new Point(contentWidth - 240, currentY);
            btnEdit.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnEdit.Appearance.BackColor = Color.FromArgb(59, 130, 246);
            btnEdit.Appearance.ForeColor = Color.White;
            btnEdit.Appearance.Options.UseBackColor = true;
            btnEdit.Appearance.Options.UseForeColor = true;
            btnEdit.Cursor = Cursors.Hand;
            btnEdit.Enabled = false;
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

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
            gcExpenses = new GridControl();
            gvExpenses = new GridView(gcExpenses);
            gcExpenses.MainView = gvExpenses;
            gcExpenses.Location = new Point(leftMargin, currentY);
            gcExpenses.Size = new Size(contentWidth, this.Height - currentY - 50);
            gcExpenses.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // Grid View Settings
            gvExpenses.OptionsBehavior.Editable = false;
            gvExpenses.OptionsView.ShowGroupPanel = false;
            gvExpenses.OptionsView.ShowIndicator = false;
            gvExpenses.RowHeight = 40;
            gvExpenses.Appearance.HeaderPanel.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            gvExpenses.Appearance.Row.Font = new Font("Tahoma", 9F);
            gvExpenses.FocusedRowChanged += GvExpenses_FocusedRowChanged;
            gvExpenses.DoubleClick += GvExpenses_DoubleClick;

            this.Controls.Add(gcExpenses);

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
                List<Expense> expenseList;

                // Role göre filtreleme
                if (_currentUser?.Role == "ApartmentManager")
                {
                    // ApartmentManager: Sadece kendi apartmanındaki giderler
                    var userFlat = _flatService.GetResidentFlat(_currentUser.Id);
                    if (userFlat != null)
                    {
                        expenseList = _expenseService.GetByApartmentId(userFlat.ApartmentId);
                    }
                    else
                    {
                        expenseList = new List<Expense>();
                    }
                }
                else if (_currentUser?.Role == "SiteManager")
                {
                    // SiteManager: Site bazlı filtreleme
                    var selectedSite = cmbSiteFilter?.SelectedItem as FilterItem;
                    if (selectedSite?.Id.HasValue == true)
                    {
                        expenseList = _expenseService.GetBySiteId(selectedSite.Id.Value);
                    }
                    else
                    {
                        // Tüm siteler için
                        expenseList = _expenseService.GetAll();
                    }
                }
                else
                {
                    // Admin/SuperAdmin: Tüm giderler
                    expenseList = _expenseService.GetAll();
                }

                // Filtreleme
                var selectedApartment = cmbApartmentFilter?.SelectedItem as FilterItem;
                if (selectedApartment?.Id.HasValue == true)
                {
                    expenseList = expenseList.Where(e => e.ApartmentId == selectedApartment.Id.Value).ToList();
                }

                var categoryFilter = cmbCategoryFilter?.SelectedItem?.ToString();
                if (categoryFilter != null && categoryFilter != "Tümü")
                {
                    expenseList = expenseList.Where(e => e.Category == categoryFilter).ToList();
                }

                if (dtStartDate.EditValue is DateTime startDate)
                {
                    expenseList = expenseList.Where(e => e.Date >= startDate).ToList();
                }

                if (dtEndDate.EditValue is DateTime endDate)
                {
                    expenseList = expenseList.Where(e => e.Date <= endDate.AddDays(1)).ToList();
                }

                // Site ve Apartman bilgilerini al
                var sites = _siteService.GetAll();
                var apartments = _apartmentService.GetAll();

                // Display data
                var displayData = expenseList.Select(e => new
                {
                    e.Id,
                    Site = e.SiteId.HasValue ? sites.FirstOrDefault(s => s.Id == e.SiteId.Value)?.Name ?? "-" : "-",
                    Apartment = e.ApartmentId.HasValue ? apartments.FirstOrDefault(a => a.Id == e.ApartmentId.Value)?.Name ?? "-" : "-",
                    Aciklama = e.Description ?? "-",
                    Kategori = e.Category ?? "Diğer",
                    Tutar = e.Amount.ToString("N2") + " TL",
                    Tarih = e.Date.ToString("dd.MM.yyyy"),
                    Amount = e.Amount,
                    Date = e.Date,
                    SiteId = e.SiteId,
                    ApartmentId = e.ApartmentId
                }).ToList();

                gcExpenses.DataSource = displayData;
                gvExpenses.PopulateColumns();

                // Sütun ayarları
                ConfigureColumns();

                // İstatistikleri güncelle
                UpdateStatistics(expenseList);
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
            if (gvExpenses.Columns["Id"] != null) gvExpenses.Columns["Id"].Visible = false;
            if (gvExpenses.Columns["Amount"] != null) gvExpenses.Columns["Amount"].Visible = false;
            if (gvExpenses.Columns["Date"] != null) gvExpenses.Columns["Date"].Visible = false;
            if (gvExpenses.Columns["SiteId"] != null) gvExpenses.Columns["SiteId"].Visible = false;
            if (gvExpenses.Columns["ApartmentId"] != null) gvExpenses.Columns["ApartmentId"].Visible = false;

            // Site (SiteManager, Admin için)
            if (_currentUser?.Role == "SiteManager" || _currentUser?.Role == "SuperAdmin" || _currentUser?.Role == "Admin")
            {
                if (gvExpenses.Columns["Site"] != null)
                {
                    gvExpenses.Columns["Site"].VisibleIndex = visibleIndex++;
                    gvExpenses.Columns["Site"].Caption = "Site";
                    gvExpenses.Columns["Site"].Width = 120;
                }
            }

            if (gvExpenses.Columns["Apartment"] != null)
            {
                gvExpenses.Columns["Apartment"].VisibleIndex = visibleIndex++;
                gvExpenses.Columns["Apartment"].Caption = "Apartman";
                gvExpenses.Columns["Apartment"].Width = 150;
            }

            if (gvExpenses.Columns["Aciklama"] != null)
            {
                gvExpenses.Columns["Aciklama"].VisibleIndex = visibleIndex++;
                gvExpenses.Columns["Aciklama"].Caption = "Açıklama";
                gvExpenses.Columns["Aciklama"].Width = 300;
            }

            if (gvExpenses.Columns["Kategori"] != null)
            {
                gvExpenses.Columns["Kategori"].VisibleIndex = visibleIndex++;
                gvExpenses.Columns["Kategori"].Caption = "Kategori";
                gvExpenses.Columns["Kategori"].Width = 120;
            }

            if (gvExpenses.Columns["Tutar"] != null)
            {
                gvExpenses.Columns["Tutar"].VisibleIndex = visibleIndex++;
                gvExpenses.Columns["Tutar"].Caption = "Tutar";
                gvExpenses.Columns["Tutar"].Width = 120;
                gvExpenses.Columns["Tutar"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            }

            if (gvExpenses.Columns["Tarih"] != null)
            {
                gvExpenses.Columns["Tarih"].VisibleIndex = visibleIndex++;
                gvExpenses.Columns["Tarih"].Caption = "Tarih";
                gvExpenses.Columns["Tarih"].Width = 120;
            }
        }

        /// <summary>
        /// İstatistikleri günceller
        /// </summary>
        private void UpdateStatistics(List<Expense> expenseList)
        {
            int total = expenseList.Count;
            decimal totalAmount = expenseList.Sum(e => e.Amount);

            var now = DateTime.Now;
            var thisMonth = expenseList.Where(e => e.Date.Year == now.Year && e.Date.Month == now.Month).Sum(e => e.Amount);
            var thisYear = expenseList.Where(e => e.Date.Year == now.Year).Sum(e => e.Amount);

            lblTotalCount.Text = total.ToString();
            lblTotalAmount.Text = totalAmount.ToString("N2") + " TL";
            lblThisMonth.Text = thisMonth.ToString("N2") + " TL";
            lblThisYear.Text = thisYear.ToString("N2") + " TL";
        }

        /// <summary>
        /// Grid row seçimi değiştiğinde
        /// </summary>
        private void GvExpenses_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            bool hasSelection = gvExpenses.FocusedRowHandle >= 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        /// <summary>
        /// Grid double click
        /// </summary>
        private void GvExpenses_DoubleClick(object sender, EventArgs e)
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
            cmbCategoryFilter.SelectedIndex = 0;
            dtStartDate.EditValue = null;
            dtEndDate.EditValue = null;
            LoadData();
        }

        /// <summary>
        /// New Expense button click
        /// </summary>
        private void BtnNewExpense_Click(object sender, EventArgs e)
        {
            var frm = new FrmExpenseManagement(null, _currentUser);
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
            if (gvExpenses.FocusedRowHandle < 0) return;

            var row = gvExpenses.GetRow(gvExpenses.FocusedRowHandle);
            if (row == null) return;

            var idProp = row.GetType().GetProperty("Id");
            if (idProp == null) return;

            int expenseId = (int)idProp.GetValue(row);
            var expense = _expenseService.GetById(expenseId);

            if (expense != null)
            {
                var frm = new FrmExpenseManagement(expense, _currentUser);
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
            if (gvExpenses.FocusedRowHandle < 0) return;

            var row = gvExpenses.GetRow(gvExpenses.FocusedRowHandle);
            if (row == null) return;

            var idProp = row.GetType().GetProperty("Id");
            if (idProp == null) return;

            int expenseId = (int)idProp.GetValue(row);

            if (Swal.Confirm("Bu gider kaydını silmek istediğinize emin misiniz?", "Gider Sil",
                "Evet, Sil", Color.FromArgb(239, 68, 68)))
            {
                string result = _expenseService.Delete(expenseId);
                if (string.IsNullOrEmpty(result))
                {
                    Swal.Success("Gider başarıyla silindi.");
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

