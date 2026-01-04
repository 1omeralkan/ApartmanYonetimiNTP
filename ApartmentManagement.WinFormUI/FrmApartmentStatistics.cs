#nullable disable
// FrmApartmentStatistics.cs
// Apartman İstatistikleri - ApartmentManager için detaylı istatistik dashboard'u
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
    /// Apartman İstatistikleri - ApartmentManager için detaylı istatistik dashboard'u
    /// </summary>
    public partial class FrmApartmentStatistics : DevExpress.XtraEditors.XtraForm
    {
        private IFlat _flatService;
        private IApartment _apartmentService;
        private IDues _duesService;
        private IExpense _expenseService;
        private IPayment _paymentService;
        private User _currentUser;
        private Apartment _currentApartment;

        // Controls
        private SimpleButton btnRefresh;
        private LabelControl lblTitle;

        // Stat Cards - Genel
        private Panel pnlCardTotalFlats;
        private Panel pnlCardOccupiedFlats;
        private Panel pnlCardEmptyFlats;
        private Panel pnlCardTotalResidents;
        private LabelControl lblTotalFlats;
        private LabelControl lblOccupiedFlats;
        private LabelControl lblEmptyFlats;
        private LabelControl lblTotalResidents;

        // Stat Cards - Aidat
        private Panel pnlCardTotalDues;
        private Panel pnlCardPaidDues;
        private Panel pnlCardPendingDues;
        private Panel pnlCardCollectionRate;
        private LabelControl lblTotalDues;
        private LabelControl lblPaidDues;
        private LabelControl lblPendingDues;
        private LabelControl lblCollectionRate;

        // Stat Cards - Gider
        private Panel pnlCardTotalExpenses;
        private Panel pnlCardThisMonthExpenses;
        private Panel pnlCardThisYearExpenses;
        private LabelControl lblTotalExpenses;
        private LabelControl lblThisMonthExpenses;
        private LabelControl lblThisYearExpenses;

        // Grids
        private GridControl gcRecentDues;
        private GridView gvRecentDues;
        private GridControl gcRecentExpenses;
        private GridView gvRecentExpenses;

        /// <summary>
        /// FrmApartmentStatistics constructor
        /// </summary>
        public FrmApartmentStatistics(User user)
        {
            _currentUser = user;
            _flatService = new SFlat();
            _apartmentService = new SApartment();
            _duesService = new SDues();
            _expenseService = new SExpense();
            _paymentService = new SPayment();
            InitializeComponent();
            LoadData();
        }

        /// <summary>
        /// Form bileşenlerini başlatır
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = "Apartman İstatistikleri";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            int leftMargin = 20;
            int currentY = 20;
            int contentWidth = this.Width - (leftMargin * 2);

            // ========== HEADER SECTION ==========
            lblTitle = new LabelControl();
            lblTitle.Text = "📊 Apartman İstatistikleri";
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblTitle);

            btnRefresh = new SimpleButton();
            btnRefresh.Text = "🔄 Yenile";
            btnRefresh.Size = new Size(100, 30);
            btnRefresh.Location = new Point(contentWidth - 100, currentY);
            btnRefresh.Appearance.Font = new Font("Tahoma", 9F);
            btnRefresh.Appearance.BackColor = Color.FromArgb(59, 130, 246);
            btnRefresh.Appearance.ForeColor = Color.White;
            btnRefresh.Appearance.Options.UseBackColor = true;
            btnRefresh.Appearance.Options.UseForeColor = true;
            btnRefresh.Click += BtnRefresh_Click;
            this.Controls.Add(btnRefresh);

            currentY += 50;

            // ========== GENEL İSTATİSTİKLER SECTION ==========
            var lblSectionGeneral = new LabelControl();
            lblSectionGeneral.Text = "Genel İstatistikler";
            lblSectionGeneral.Appearance.Font = new Font("Tahoma", 14F, FontStyle.Bold);
            lblSectionGeneral.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblSectionGeneral.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblSectionGeneral);

            currentY += 35;

            // Genel Stat Cards
            int cardWidth = (contentWidth - 60) / 4;
            int cardHeight = 100;
            int cardSpacing = 20;

            pnlCardTotalFlats = CreateStatCard(leftMargin, currentY, cardWidth, cardHeight,
                Color.FromArgb(59, 130, 246), "🚪", "Toplam Daire", "0", ref lblTotalFlats);
            this.Controls.Add(pnlCardTotalFlats);

            pnlCardOccupiedFlats = CreateStatCard(leftMargin + cardWidth + cardSpacing, currentY, cardWidth, cardHeight,
                Color.FromArgb(34, 197, 94), "✅", "Dolu", "0", ref lblOccupiedFlats);
            this.Controls.Add(pnlCardOccupiedFlats);

            pnlCardEmptyFlats = CreateStatCard(leftMargin + (cardWidth + cardSpacing) * 2, currentY, cardWidth, cardHeight,
                Color.FromArgb(234, 179, 8), "⭕", "Boş", "0", ref lblEmptyFlats);
            this.Controls.Add(pnlCardEmptyFlats);

            pnlCardTotalResidents = CreateStatCard(leftMargin + (cardWidth + cardSpacing) * 3, currentY, cardWidth, cardHeight,
                Color.FromArgb(168, 85, 247), "👥", "Toplam Sakin", "0", ref lblTotalResidents);
            this.Controls.Add(pnlCardTotalResidents);

            currentY += cardHeight + 40;

            // ========== AİDAT ÖZETİ SECTION ==========
            var lblSectionDues = new LabelControl();
            lblSectionDues.Text = "Aidat Özeti";
            lblSectionDues.Appearance.Font = new Font("Tahoma", 14F, FontStyle.Bold);
            lblSectionDues.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblSectionDues.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblSectionDues);

            currentY += 35;

            // Aidat Stat Cards
            pnlCardTotalDues = CreateStatCard(leftMargin, currentY, cardWidth, cardHeight,
                Color.FromArgb(59, 130, 246), "💰", "Toplam Aidat", "0 TL", ref lblTotalDues);
            this.Controls.Add(pnlCardTotalDues);

            pnlCardPaidDues = CreateStatCard(leftMargin + cardWidth + cardSpacing, currentY, cardWidth, cardHeight,
                Color.FromArgb(34, 197, 94), "✅", "Ödenen", "0 TL", ref lblPaidDues);
            this.Controls.Add(pnlCardPaidDues);

            pnlCardPendingDues = CreateStatCard(leftMargin + (cardWidth + cardSpacing) * 2, currentY, cardWidth, cardHeight,
                Color.FromArgb(239, 68, 68), "⏳", "Bekleyen", "0 TL", ref lblPendingDues);
            this.Controls.Add(pnlCardPendingDues);

            pnlCardCollectionRate = CreateStatCard(leftMargin + (cardWidth + cardSpacing) * 3, currentY, cardWidth, cardHeight,
                Color.FromArgb(234, 179, 8), "📊", "Tahsilat Oranı", "0%", ref lblCollectionRate);
            this.Controls.Add(pnlCardCollectionRate);

            currentY += cardHeight + 40;

            // ========== GİDER ÖZETİ SECTION ==========
            var lblSectionExpenses = new LabelControl();
            lblSectionExpenses.Text = "Gider Özeti";
            lblSectionExpenses.Appearance.Font = new Font("Tahoma", 14F, FontStyle.Bold);
            lblSectionExpenses.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblSectionExpenses.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblSectionExpenses);

            currentY += 35;

            // Gider Stat Cards
            pnlCardTotalExpenses = CreateStatCard(leftMargin, currentY, cardWidth, cardHeight,
                Color.FromArgb(239, 68, 68), "💸", "Toplam Gider", "0 TL", ref lblTotalExpenses);
            this.Controls.Add(pnlCardTotalExpenses);

            pnlCardThisMonthExpenses = CreateStatCard(leftMargin + cardWidth + cardSpacing, currentY, cardWidth, cardHeight,
                Color.FromArgb(59, 130, 246), "📅", "Bu Ay", "0 TL", ref lblThisMonthExpenses);
            this.Controls.Add(pnlCardThisMonthExpenses);

            pnlCardThisYearExpenses = CreateStatCard(leftMargin + (cardWidth + cardSpacing) * 2, currentY, cardWidth, cardHeight,
                Color.FromArgb(34, 197, 94), "📆", "Bu Yıl", "0 TL", ref lblThisYearExpenses);
            this.Controls.Add(pnlCardThisYearExpenses);

            currentY += cardHeight + 40;

            // ========== SON AİDATLAR GRID ==========
            var lblRecentDues = new LabelControl();
            lblRecentDues.Text = "Son Aidatlar";
            lblRecentDues.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblRecentDues.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblRecentDues.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblRecentDues);

            currentY += 30;

            gcRecentDues = new GridControl();
            gvRecentDues = new GridView(gcRecentDues);
            gcRecentDues.MainView = gvRecentDues;
            gcRecentDues.Location = new Point(leftMargin, currentY);
            gcRecentDues.Size = new Size((contentWidth - 20) / 2, 200);
            gcRecentDues.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            gvRecentDues.OptionsBehavior.Editable = false;
            gvRecentDues.OptionsView.ShowGroupPanel = false;
            gvRecentDues.OptionsView.ShowIndicator = false;
            gvRecentDues.RowHeight = 35;
            gvRecentDues.Appearance.HeaderPanel.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            gvRecentDues.Appearance.Row.Font = new Font("Tahoma", 8.5F);

            this.Controls.Add(gcRecentDues);

            // ========== SON GİDERLER GRID ==========
            var lblRecentExpenses = new LabelControl();
            lblRecentExpenses.Text = "Son Giderler";
            lblRecentExpenses.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblRecentExpenses.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblRecentExpenses.Location = new Point(leftMargin + (contentWidth - 20) / 2 + 20, currentY - 30);
            this.Controls.Add(lblRecentExpenses);

            gcRecentExpenses = new GridControl();
            gvRecentExpenses = new GridView(gcRecentExpenses);
            gcRecentExpenses.MainView = gvRecentExpenses;
            gcRecentExpenses.Location = new Point(leftMargin + (contentWidth - 20) / 2 + 20, currentY);
            gcRecentExpenses.Size = new Size((contentWidth - 20) / 2, 200);
            gcRecentExpenses.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            gvRecentExpenses.OptionsBehavior.Editable = false;
            gvRecentExpenses.OptionsView.ShowGroupPanel = false;
            gvRecentExpenses.OptionsView.ShowIndicator = false;
            gvRecentExpenses.RowHeight = 35;
            gvRecentExpenses.Appearance.HeaderPanel.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            gvRecentExpenses.Appearance.Row.Font = new Font("Tahoma", 8.5F);

            this.Controls.Add(gcRecentExpenses);

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
        /// Yenile butonuna tıklandığında
        /// </summary>
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// Verileri yükler
        /// </summary>
        private void LoadData()
        {
            try
            {
                // ApartmentManager'ın apartmanını bul
                var userFlat = _flatService.GetResidentFlat(_currentUser.Id);
                if (userFlat == null)
                {
                    Swal.Warning("Apartman bilgisi bulunamadı.");
                    return;
                }

                _currentApartment = _apartmentService.GetAll().FirstOrDefault(a => a.Id == userFlat.ApartmentId);
                if (_currentApartment == null)
                {
                    Swal.Warning("Apartman bilgisi bulunamadı.");
                    return;
                }

                // Genel İstatistikler
                var flats = _flatService.GetAllByApartmentId(_currentApartment.Id);
                int totalFlats = flats.Count;
                int occupiedFlats = flats.Count(f => !f.IsEmpty);
                int emptyFlats = flats.Count(f => f.IsEmpty);

                // Toplam sakin sayısı (FlatResident tablosundan)
                var context = new ApartmentManagement.DataAccess.ApartmentManagementContext();
                var flatIds = flats.Select(f => f.Id).ToList();
                int totalResidents = context.FlatResidents
                    .Where(fr => flatIds.Contains(fr.FlatId))
                    .Count();

                lblTotalFlats.Text = totalFlats.ToString();
                lblOccupiedFlats.Text = occupiedFlats.ToString();
                lblEmptyFlats.Text = emptyFlats.ToString();
                lblTotalResidents.Text = totalResidents.ToString();

                // Aidat Özeti
                var allDues = _duesService.GetByApartmentId(_currentApartment.Id);
                decimal totalDuesAmount = allDues.Sum(d => d.Amount);
                decimal paidDuesAmount = allDues.Where(d => d.IsPaid).Sum(d => d.Amount);
                decimal pendingDuesAmount = allDues.Where(d => !d.IsPaid).Sum(d => d.Amount);
                decimal collectionRate = totalDuesAmount > 0 ? (paidDuesAmount / totalDuesAmount) * 100 : 0;

                lblTotalDues.Text = totalDuesAmount.ToString("N2") + " TL";
                lblPaidDues.Text = paidDuesAmount.ToString("N2") + " TL";
                lblPendingDues.Text = pendingDuesAmount.ToString("N2") + " TL";
                lblCollectionRate.Text = collectionRate.ToString("N1") + "%";

                // Gider Özeti
                var expenses = _expenseService.GetByApartmentId(_currentApartment.Id);
                decimal totalExpenses = expenses.Sum(e => e.Amount);

                var now = DateTime.Now;
                decimal thisMonthExpenses = expenses
                    .Where(e => e.Date.Year == now.Year && e.Date.Month == now.Month)
                    .Sum(e => e.Amount);
                decimal thisYearExpenses = expenses
                    .Where(e => e.Date.Year == now.Year)
                    .Sum(e => e.Amount);

                lblTotalExpenses.Text = totalExpenses.ToString("N2") + " TL";
                lblThisMonthExpenses.Text = thisMonthExpenses.ToString("N2") + " TL";
                lblThisYearExpenses.Text = thisYearExpenses.ToString("N2") + " TL";

                // Son Aidatlar
                var recentDues = allDues
                    .OrderByDescending(d => d.Year)
                    .ThenByDescending(d => d.Month)
                    .Take(10)
                    .Select(d => new
                    {
                        d.Id,
                        FlatId = d.FlatId,
                        Tutar = d.Amount.ToString("N2") + " TL",
                        Ay = $"{d.Month:D2}/{d.Year}",
                        Durum = d.IsPaid ? "Ödendi" : "Bekliyor"
                    }).ToList();

                // Flat bilgilerini ekle
                var recentDuesWithFlat = recentDues.Select(d => new
                {
                    d.Id,
                    Daire = flats.FirstOrDefault(f => f.Id == d.FlatId)?.DoorNumber.ToString() ?? "-",
                    d.Tutar,
                    d.Ay,
                    d.Durum
                }).ToList();

                gcRecentDues.DataSource = recentDuesWithFlat;
                gvRecentDues.PopulateColumns();
                ConfigureDuesColumns();

                // Son Giderler
                var recentExpenses = expenses
                    .OrderByDescending(e => e.Date)
                    .Take(10)
                    .Select(e => new
                    {
                        e.Id,
                        Aciklama = e.Description ?? "-",
                        Kategori = e.Category ?? "Diğer",
                        Tutar = e.Amount.ToString("N2") + " TL",
                        Tarih = e.Date.ToString("dd.MM.yyyy")
                    }).ToList();

                gcRecentExpenses.DataSource = recentExpenses;
                gvRecentExpenses.PopulateColumns();
                ConfigureExpensesColumns();
            }
            catch (Exception ex)
            {
                Swal.Error("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Aidat sütunlarını yapılandırır
        /// </summary>
        private void ConfigureDuesColumns()
        {
            if (gvRecentDues.Columns["Id"] != null) gvRecentDues.Columns["Id"].Visible = false;

            int visibleIndex = 0;
            if (gvRecentDues.Columns["Daire"] != null)
            {
                gvRecentDues.Columns["Daire"].VisibleIndex = visibleIndex++;
                gvRecentDues.Columns["Daire"].Caption = "Daire";
                gvRecentDues.Columns["Daire"].Width = 150;
            }
            if (gvRecentDues.Columns["Tutar"] != null)
            {
                gvRecentDues.Columns["Tutar"].VisibleIndex = visibleIndex++;
                gvRecentDues.Columns["Tutar"].Caption = "Tutar";
                gvRecentDues.Columns["Tutar"].Width = 150;
                gvRecentDues.Columns["Tutar"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            }
            if (gvRecentDues.Columns["Ay"] != null)
            {
                gvRecentDues.Columns["Ay"].VisibleIndex = visibleIndex++;
                gvRecentDues.Columns["Ay"].Caption = "Ay/Yıl";
                gvRecentDues.Columns["Ay"].Width = 120;
            }
            if (gvRecentDues.Columns["Durum"] != null)
            {
                gvRecentDues.Columns["Durum"].VisibleIndex = visibleIndex++;
                gvRecentDues.Columns["Durum"].Caption = "Durum";
                gvRecentDues.Columns["Durum"].Width = 120;
            }
        }

        /// <summary>
        /// Gider sütunlarını yapılandırır
        /// </summary>
        private void ConfigureExpensesColumns()
        {
            if (gvRecentExpenses.Columns["Id"] != null) gvRecentExpenses.Columns["Id"].Visible = false;

            int visibleIndex = 0;
            if (gvRecentExpenses.Columns["Aciklama"] != null)
            {
                gvRecentExpenses.Columns["Aciklama"].VisibleIndex = visibleIndex++;
                gvRecentExpenses.Columns["Aciklama"].Caption = "Açıklama";
                gvRecentExpenses.Columns["Aciklama"].Width = 300;
            }
            if (gvRecentExpenses.Columns["Kategori"] != null)
            {
                gvRecentExpenses.Columns["Kategori"].VisibleIndex = visibleIndex++;
                gvRecentExpenses.Columns["Kategori"].Caption = "Kategori";
                gvRecentExpenses.Columns["Kategori"].Width = 120;
            }
            if (gvRecentExpenses.Columns["Tutar"] != null)
            {
                gvRecentExpenses.Columns["Tutar"].VisibleIndex = visibleIndex++;
                gvRecentExpenses.Columns["Tutar"].Caption = "Tutar";
                gvRecentExpenses.Columns["Tutar"].Width = 150;
                gvRecentExpenses.Columns["Tutar"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            }
            if (gvRecentExpenses.Columns["Tarih"] != null)
            {
                gvRecentExpenses.Columns["Tarih"].VisibleIndex = visibleIndex++;
                gvRecentExpenses.Columns["Tarih"].Caption = "Tarih";
                gvRecentExpenses.Columns["Tarih"].Width = 120;
            }
        }
    }
}

