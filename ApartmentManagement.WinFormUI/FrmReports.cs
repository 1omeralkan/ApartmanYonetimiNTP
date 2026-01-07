#nullable disable
// FrmReports.cs
// Raporlar / Analitik ekranı - Yalın ama şık finansal ve kullanıcı raporları
// Standart: Tahoma 8.25pt, AutoScroll = true
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.Business.Services;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Admin / SuperAdmin için temel raporlar ve analitik ekranı
    /// </summary>
    public class FrmReports : XtraForm
    {
        private readonly IPayment _paymentService;
        private readonly IDues _duesService;
        private readonly IExpense _expenseService;
        private readonly IUser _userService;

        private readonly User _currentUser;

        // Stat cards
        private LabelControl lblTotalDues;
        private LabelControl lblTotalPayments;
        private LabelControl lblTotalExpenses;
        private LabelControl lblBalance;

        // Grids
        private GridControl gcPayments;
        private GridView gvPayments;

        private GridControl gcDuesSummary;
        private GridView gvDuesSummary;

        private GridControl gcUserSummary;
        private GridView gvUserSummary;

        public FrmReports(User user)
        {
            _currentUser = user;

            _paymentService = new SPayment();
            _duesService = new SDues();
            _expenseService = new SExpense();
            _userService = new SUser();

            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = "Raporlar / Analitik";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            int leftMargin = 20;
            int currentY = 20;
            int contentWidth = this.Width - (leftMargin * 2);

            // ========== HEADER ==========
            var lblTitle = new LabelControl();
            lblTitle.Text = "📈 Raporlar / Analitik";
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblTitle);

            var lblSubtitle = new LabelControl();
            lblSubtitle.Text = "Finansal özetler, kullanıcı dağılımı ve son hareketler";
            lblSubtitle.Appearance.Font = new Font("Tahoma", 9F);
            lblSubtitle.Appearance.ForeColor = Color.FromArgb(120, 120, 130);
            lblSubtitle.Location = new Point(leftMargin, currentY + 35);
            this.Controls.Add(lblSubtitle);

            var btnRefresh = new SimpleButton();
            btnRefresh.Text = "🔄 Yenile";
            btnRefresh.Size = new Size(110, 32);
            btnRefresh.Location = new Point(contentWidth - 110, currentY + 10);
            btnRefresh.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnRefresh.Appearance.BackColor = Color.FromArgb(59, 130, 246);
            btnRefresh.Appearance.ForeColor = Color.White;
            btnRefresh.Appearance.Options.UseBackColor = true;
            btnRefresh.Appearance.Options.UseForeColor = true;
            btnRefresh.Click += (s, e) => LoadData();
            this.Controls.Add(btnRefresh);

            currentY += 70;

            // ========== FİNANSAL ÖZET KARTLAR ==========
            int cardWidth = (contentWidth - 60) / 4;
            int cardHeight = 100;
            int cardSpacing = 20;

            Panel pnlTotalDues = CreateStatCard(leftMargin, currentY, cardWidth, cardHeight,
                Color.FromArgb(59, 130, 246), "💰", "Toplam Tahakkuk (Aidat)", "0 TL", ref lblTotalDues);
            this.Controls.Add(pnlTotalDues);

            Panel pnlTotalPayments = CreateStatCard(leftMargin + cardWidth + cardSpacing, currentY, cardWidth, cardHeight,
                Color.FromArgb(34, 197, 94), "✅", "Toplam Ödeme", "0 TL", ref lblTotalPayments);
            this.Controls.Add(pnlTotalPayments);

            Panel pnlTotalExpenses = CreateStatCard(leftMargin + (cardWidth + cardSpacing) * 2, currentY, cardWidth, cardHeight,
                Color.FromArgb(239, 68, 68), "💸", "Toplam Gider", "0 TL", ref lblTotalExpenses);
            this.Controls.Add(pnlTotalExpenses);

            Panel pnlBalance = CreateStatCard(leftMargin + (cardWidth + cardSpacing) * 3, currentY, cardWidth, cardHeight,
                Color.FromArgb(234, 179, 8), "⚖️", "Net Bakiye", "0 TL", ref lblBalance);
            this.Controls.Add(pnlBalance);

            currentY += cardHeight + 40;

            // ========== ÖDEMELER GRID (SOL) & AİDAT ÖZETİ (SAĞ) ==========
            int gridHeight = 260;
            int halfWidth = (contentWidth - 20) / 2;

            var lblPayments = new LabelControl();
            lblPayments.Text = "Son Ödemeler";
            lblPayments.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblPayments.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblPayments.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblPayments);

            gcPayments = new GridControl();
            gvPayments = new GridView(gcPayments);
            gcPayments.MainView = gvPayments;
            gcPayments.Location = new Point(leftMargin, currentY + 25);
            gcPayments.Size = new Size(halfWidth, gridHeight);
            gcPayments.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            gvPayments.OptionsBehavior.Editable = false;
            gvPayments.OptionsView.ShowGroupPanel = false;
            gvPayments.OptionsView.ShowIndicator = false;
            gvPayments.RowHeight = 30;
            gvPayments.Appearance.HeaderPanel.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            gvPayments.Appearance.Row.Font = new Font("Tahoma", 8.5F);

            this.Controls.Add(gcPayments);

            var lblDuesSummary = new LabelControl();
            lblDuesSummary.Text = "Aidat Özeti (Yıl / Ay)";
            lblDuesSummary.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblDuesSummary.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblDuesSummary.Location = new Point(leftMargin + halfWidth + 20, currentY);
            this.Controls.Add(lblDuesSummary);

            gcDuesSummary = new GridControl();
            gvDuesSummary = new GridView(gcDuesSummary);
            gcDuesSummary.MainView = gvDuesSummary;
            gcDuesSummary.Location = new Point(leftMargin + halfWidth + 20, currentY + 25);
            gcDuesSummary.Size = new Size(halfWidth, gridHeight);
            gcDuesSummary.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            gvDuesSummary.OptionsBehavior.Editable = false;
            gvDuesSummary.OptionsView.ShowGroupPanel = false;
            gvDuesSummary.OptionsView.ShowIndicator = false;
            gvDuesSummary.RowHeight = 30;
            gvDuesSummary.Appearance.HeaderPanel.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            gvDuesSummary.Appearance.Row.Font = new Font("Tahoma", 8.5F);

            this.Controls.Add(gcDuesSummary);

            currentY += gridHeight + 40;

            // ========== KULLANICI DAĞILIMI GRID ==========
            var lblUsers = new LabelControl();
            lblUsers.Text = "Kullanıcı Dağılımı (Role göre)";
            lblUsers.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblUsers.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblUsers.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblUsers);

            gcUserSummary = new GridControl();
            gvUserSummary = new GridView(gcUserSummary);
            gcUserSummary.MainView = gvUserSummary;
            gcUserSummary.Location = new Point(leftMargin, currentY + 25);
            gcUserSummary.Size = new Size(contentWidth, 220);
            gcUserSummary.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            gvUserSummary.OptionsBehavior.Editable = false;
            gvUserSummary.OptionsView.ShowGroupPanel = false;
            gvUserSummary.OptionsView.ShowIndicator = false;
            gvUserSummary.RowHeight = 30;
            gvUserSummary.Appearance.HeaderPanel.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            gvUserSummary.Appearance.Row.Font = new Font("Tahoma", 8.5F);

            this.Controls.Add(gcUserSummary);

            this.ResumeLayout(false);
        }

        private Panel CreateStatCard(int x, int y, int width, int height, Color color, string icon, string title, string initialValue, ref LabelControl valueLabel)
        {
            var pnl = new RoundedPanel();
            pnl.Location = new Point(x, y);
            pnl.Size = new Size(width, height);
            pnl.BackColor = Color.White;
            pnl.BorderRadius = 10;
            pnl.BorderThickness = 0;

            var lblIcon = new LabelControl();
            lblIcon.Text = icon;
            lblIcon.Appearance.Font = new Font("Segoe UI Emoji", 20F);
            lblIcon.Location = new Point(15, 25);
            pnl.Controls.Add(lblIcon);

            var lblTitle = new LabelControl();
            lblTitle.Text = title;
            lblTitle.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(75, 85, 99);
            lblTitle.Location = new Point(60, 20);
            pnl.Controls.Add(lblTitle);

            valueLabel = new LabelControl();
            valueLabel.Text = initialValue;
            valueLabel.Appearance.Font = new Font("Tahoma", 13F, FontStyle.Bold);
            valueLabel.Appearance.ForeColor = color;
            valueLabel.Location = new Point(60, 45);
            pnl.Controls.Add(valueLabel);

            return pnl;
        }

        private void LoadData()
        {
            try
            {
                // Ödemeler
                var payments = _paymentService.GetAll();

                // Aidatlar
                var dues = _duesService.GetAll();

                // Giderler
                var expenses = _expenseService.GetAll();

                // Finansal özet
                decimal totalDues = dues.Sum(d => d.Amount);
                decimal totalPayments = payments.Sum(p => p.Amount);
                decimal totalExpenses = expenses.Sum(e => e.Amount);
                decimal balance = totalPayments - totalExpenses;

                lblTotalDues.Text = $"{totalDues:N2} TL";
                lblTotalPayments.Text = $"{totalPayments:N2} TL";
                lblTotalExpenses.Text = $"{totalExpenses:N2} TL";
                lblBalance.Text = $"{balance:N2} TL";

                // Son 50 ödeme
                var paymentList = payments
                    .OrderByDescending(p => p.Date)
                    .Take(50)
                    .Select(p => new
                    {
                        Tarih = p.Date.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                        Tip = p.Type,
                        Tutar = p.Amount,
                        Site = p.Flat?.Apartment?.Block?.Site?.Name,
                        Apartman = p.Flat?.Apartment?.Name,
                        Daire = p.Flat != null ? $"{p.Flat.Floor}. Kat / No: {p.Flat.DoorNumber}" : string.Empty
                    })
                    .ToList();

                gcPayments.DataSource = paymentList;
                gvPayments.BestFitColumns();

                // Aidat özeti: Yıl + Ay bazlı tahakkuk
                var duesSummary = dues
                    .GroupBy(d => new { d.Year, d.Month })
                    .Select(g => new
                    {
                        Yıl = g.Key.Year,
                        Ay = g.Key.Month,
                        ToplamAidat = g.Sum(x => x.Amount)
                    })
                    .OrderByDescending(x => x.Yıl)
                    .ThenByDescending(x => x.Ay)
                    .ToList();

                gcDuesSummary.DataSource = duesSummary;
                gvDuesSummary.BestFitColumns();

                // Kullanıcı dağılımı
                var users = _userService.GetAll();
                var userSummary = users
                    .GroupBy(u => u.Role)
                    .Select(g => new
                    {
                        Rol = g.Key,
                        Toplam = g.Count(),
                        Aktif = g.Count(u => u.IsApproved),
                        Pasif = g.Count(u => !u.IsApproved)
                    })
                    .OrderByDescending(x => x.Toplam)
                    .ToList();

                gcUserSummary.DataSource = userSummary;
                gvUserSummary.BestFitColumns();
            }
            catch (Exception ex)
            {
                Swal.Error("Raporlar yüklenirken hata oluştu: " + ex.Message);
            }
        }
    }
}


