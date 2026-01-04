#nullable disable
// FrmResidentDashboard.cs
// Resident Dashboard - Sakinler için özel dashboard ekranı
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
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Resident Dashboard - Sakinler için özel dashboard
    /// </summary>
    public partial class FrmResidentDashboard : DevExpress.XtraEditors.XtraForm
    {
        private IFlat _flatService;
        private User _currentUser;
        private Flat _residentFlat;

        // Controls
        private Panel pnlContainer;
        private LabelControl lblTitle;
        private LabelControl lblSubtitle;
        private SimpleButton btnRefresh;

        // Stat Cards
        private Panel pnlCardPersonalInfo;
        private Panel pnlCardFlatInfo;
        private Panel pnlCardPendingDues;
        private Panel pnlCardRecentPayments;

        // Recent Payments List
        private GridControl gcRecentPayments;
        private GridView gvRecentPayments;

        // Quick Actions
        private SimpleButton btnViewProfile;
        private SimpleButton btnViewPayments;
        private SimpleButton btnViewDues;
        private SimpleButton btnViewAnnouncements;

        /// <summary>
        /// FrmResidentDashboard constructor
        /// </summary>
        public FrmResidentDashboard(User user)
        {
            _currentUser = user;
            _flatService = new SFlat();
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
            this.Text = "Sakin Paneli";
            this.Size = new Size(1100, 2000);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            int leftMargin = 20;
            int topMargin = 20;
            int currentY = topMargin;
            int contentWidth = this.Width - (leftMargin * 2);

            // Container Panel
            pnlContainer = new Panel();
            pnlContainer.Size = new Size(contentWidth, 2000);
            pnlContainer.Location = new Point(leftMargin, currentY);
            pnlContainer.BackColor = Color.Transparent;
            this.Controls.Add(pnlContainer);

            // ========== HEADER SECTION ==========
            lblTitle = new LabelControl();
            lblTitle.Text = "🏡 Sakin Paneli";
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(0, currentY);
            pnlContainer.Controls.Add(lblTitle);

            lblSubtitle = new LabelControl();
            lblSubtitle.Text = "Hoş geldiniz, " + (_currentUser?.FirstName ?? "") + " " + (_currentUser?.LastName ?? "");
            lblSubtitle.Appearance.Font = new Font("Tahoma", 9F);
            lblSubtitle.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            lblSubtitle.Location = new Point(0, currentY + 30);
            pnlContainer.Controls.Add(lblSubtitle);

            btnRefresh = new SimpleButton();
            btnRefresh.Text = "Yenile";
            btnRefresh.Size = new Size(90, 32);
            btnRefresh.Location = new Point(contentWidth - 90, currentY);
            btnRefresh.Appearance.Font = new Font("Tahoma", 9F);
            btnRefresh.Appearance.BackColor = Color.White;
            btnRefresh.Appearance.ForeColor = Color.FromArgb(59, 130, 246);
            btnRefresh.Appearance.BorderColor = Color.FromArgb(59, 130, 246);
            btnRefresh.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += (s, e) => LoadData();
            pnlContainer.Controls.Add(btnRefresh);

            currentY += 70;

            // ========== STAT CARDS SECTION ==========
            int cardWidth = (contentWidth - 60) / 4; // 4 kart, 3 boşluk (20px each)
            int cardHeight = 140;
            int cardSpacing = 20;

            // Kişisel Bilgiler Kartı
            pnlCardPersonalInfo = CreateStatCard(0, currentY, cardWidth, cardHeight, 
                Color.FromArgb(59, 130, 246), "👤", "Kişisel Bilgiler", 
                $"{_currentUser?.FirstName ?? ""} {_currentUser?.LastName ?? ""}", 
                _currentUser?.Email ?? "");
            pnlContainer.Controls.Add(pnlCardPersonalInfo);

            // Daire Bilgileri Kartı
            pnlCardFlatInfo = CreateStatCard(cardWidth + cardSpacing, currentY, cardWidth, cardHeight,
                Color.FromArgb(34, 197, 94), "🚪", "Daire Bilgileri",
                "Daire No", "Yükleniyor...");
            pnlContainer.Controls.Add(pnlCardFlatInfo);

            // Bekleyen Aidatlar Kartı
            pnlCardPendingDues = CreateStatCard((cardWidth + cardSpacing) * 2, currentY, cardWidth, cardHeight,
                Color.FromArgb(234, 179, 8), "⏳", "Bekleyen Aidat",
                "0", "TL");
            pnlContainer.Controls.Add(pnlCardPendingDues);

            // Son Ödemeler Kartı
            pnlCardRecentPayments = CreateStatCard((cardWidth + cardSpacing) * 3, currentY, cardWidth, cardHeight,
                Color.FromArgb(239, 68, 68), "💰", "Son Ödeme",
                "0", "TL");
            pnlContainer.Controls.Add(pnlCardRecentPayments);

            currentY += cardHeight + 30;

            // ========== QUICK ACTIONS SECTION ==========
            var lblQuickActions = new LabelControl();
            lblQuickActions.Text = "Hızlı İşlemler";
            lblQuickActions.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblQuickActions.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblQuickActions.Location = new Point(0, currentY);
            pnlContainer.Controls.Add(lblQuickActions);

            currentY += 35;

            int btnWidth = 200;
            int btnHeight = 40;
            int btnSpacing = 20;

            btnViewProfile = CreateQuickActionButton("👤 Profilim", 0, currentY, btnWidth, btnHeight, 
                Color.FromArgb(59, 130, 246), () => { Helpers.Swal.Info("Profil ekranı yakında!"); });
            pnlContainer.Controls.Add(btnViewProfile);

            btnViewPayments = CreateQuickActionButton("💳 Ödemelerim", btnWidth + btnSpacing, currentY, btnWidth, btnHeight,
                Color.FromArgb(34, 197, 94), () => { Helpers.Swal.Info("Ödeme ekranı yakında!"); });
            pnlContainer.Controls.Add(btnViewPayments);

            btnViewDues = CreateQuickActionButton("📋 Aidatlarım", (btnWidth + btnSpacing) * 2, currentY, btnWidth, btnHeight,
                Color.FromArgb(234, 179, 8), () => { Helpers.Swal.Info("Aidat ekranı yakında!"); });
            pnlContainer.Controls.Add(btnViewDues);

            btnViewAnnouncements = CreateQuickActionButton("📢 Duyurular", (btnWidth + btnSpacing) * 3, currentY, btnWidth, btnHeight,
                Color.FromArgb(239, 68, 68), () => { Helpers.Swal.Info("Duyuru ekranı yakında!"); });
            pnlContainer.Controls.Add(btnViewAnnouncements);

            currentY += btnHeight + 40;

            // ========== RECENT PAYMENTS SECTION ==========
            var lblRecentPayments = new LabelControl();
            lblRecentPayments.Text = "Son Ödemeler";
            lblRecentPayments.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblRecentPayments.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblRecentPayments.Location = new Point(0, currentY);
            pnlContainer.Controls.Add(lblRecentPayments);

            currentY += 35;

            // Grid Control
            gcRecentPayments = new GridControl();
            gvRecentPayments = new GridView(gcRecentPayments);
            gcRecentPayments.MainView = gvRecentPayments;
            gcRecentPayments.Location = new Point(0, currentY);
            gcRecentPayments.Size = new Size(contentWidth, 300);
            gcRecentPayments.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Grid View Settings
            gvRecentPayments.OptionsBehavior.Editable = false;
            gvRecentPayments.OptionsView.ShowGroupPanel = false;
            gvRecentPayments.OptionsView.ShowIndicator = false;
            gvRecentPayments.Appearance.HeaderPanel.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            gvRecentPayments.Appearance.Row.Font = new Font("Tahoma", 9F);

            pnlContainer.Controls.Add(gcRecentPayments);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Stat card oluşturur
        /// </summary>
        private Panel CreateStatCard(int x, int y, int width, int height, Color barColor, string icon, 
            string title, string value, string subtitle)
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
            lblIcon.Appearance.Font = new Font("Segoe UI Emoji", 32F);
            lblIcon.Location = new Point(15, 15);
            panel.Controls.Add(lblIcon);

            // Başlık
            var lblTitle = new LabelControl();
            lblTitle.Text = title;
            lblTitle.Appearance.Font = new Font("Tahoma", 9F);
            lblTitle.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            lblTitle.Location = new Point(15, height - 50);
            panel.Controls.Add(lblTitle);

            // Değer
            var lblValue = new LabelControl();
            lblValue.Text = value;
            lblValue.Appearance.Font = new Font("Tahoma", 16F, FontStyle.Bold);
            lblValue.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblValue.Location = new Point(15, height - 35);
            lblValue.Size = new Size(width - 30, 20);
            panel.Controls.Add(lblValue);

            // Alt başlık
            if (!string.IsNullOrEmpty(subtitle))
            {
                var lblSubtitle = new LabelControl();
                lblSubtitle.Text = subtitle;
                lblSubtitle.Appearance.Font = new Font("Tahoma", 8F);
                lblSubtitle.Appearance.ForeColor = Color.FromArgb(150, 150, 150);
                lblSubtitle.Location = new Point(15, height - 20);
                panel.Controls.Add(lblSubtitle);
            }

            return panel;
        }

        /// <summary>
        /// Hızlı işlem butonu oluşturur
        /// </summary>
        private SimpleButton CreateQuickActionButton(string text, int x, int y, int width, int height, 
            Color color, Action onClick)
        {
            var btn = new SimpleButton();
            btn.Text = text;
            btn.Size = new Size(width, height);
            btn.Location = new Point(x, y);
            btn.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btn.Appearance.BackColor = color;
            btn.Appearance.ForeColor = Color.White;
            btn.Appearance.Options.UseBackColor = true;
            btn.Appearance.Options.UseForeColor = true;
            btn.Cursor = Cursors.Hand;
            btn.Click += (s, e) => onClick?.Invoke();
            return btn;
        }

        /// <summary>
        /// Verileri yükler
        /// </summary>
        private void LoadData()
        {
            try
            {
                // Resident'ın daire bilgilerini al
                _residentFlat = _flatService.GetResidentFlat(_currentUser?.Id ?? 0);

                // Daire bilgileri kartını güncelle
                UpdateFlatInfoCard();

                // Bekleyen aidatları hesapla
                UpdatePendingDuesCard();

                // Son ödemeleri yükle
                LoadRecentPayments();
            }
            catch (Exception ex)
            {
                Helpers.Swal.Error("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Daire bilgileri kartını günceller
        /// </summary>
        private void UpdateFlatInfoCard()
        {
            if (_residentFlat != null)
            {
                var lblValue = pnlCardFlatInfo.Controls.OfType<LabelControl>()
                    .FirstOrDefault(l => l.Appearance.Font.Bold);
                if (lblValue != null)
                {
                    lblValue.Text = $"Daire {_residentFlat.DoorNumber}";
                }

                var lblSubtitle = pnlCardFlatInfo.Controls.OfType<LabelControl>()
                    .FirstOrDefault(l => !l.Appearance.Font.Bold && l.Location.Y > 100);
                if (lblSubtitle != null)
                {
                    string siteName = _residentFlat.Apartment?.Block?.Site?.Name ?? "";
                    string blockName = _residentFlat.Apartment?.Block?.Name ?? "";
                    string apartmentName = _residentFlat.Apartment?.Name ?? "";
                    lblSubtitle.Text = $"{siteName} - {blockName} - {apartmentName}";
                }
            }
            else
            {
                var lblValue = pnlCardFlatInfo.Controls.OfType<LabelControl>()
                    .FirstOrDefault(l => l.Appearance.Font.Bold);
                if (lblValue != null)
                {
                    lblValue.Text = "Daire yok";
                }
            }
        }

        /// <summary>
        /// Bekleyen aidatlar kartını günceller
        /// </summary>
        private void UpdatePendingDuesCard()
        {
            if (_residentFlat == null) return;

            try
            {
                using (var context = new DataAccess.ApartmentManagementContext())
                {
                    var pendingDues = context.Dues
                        .Where(d => d.FlatId == _residentFlat.Id && !d.IsPaid)
                        .Sum(d => (decimal?)d.Amount) ?? 0;

                    var lblValue = pnlCardPendingDues.Controls.OfType<LabelControl>()
                        .FirstOrDefault(l => l.Appearance.Font.Bold);
                    if (lblValue != null)
                    {
                        lblValue.Text = pendingDues.ToString("N2");
                    }
                }
            }
            catch
            {
                // Hata durumunda sessizce devam et
            }
        }

        /// <summary>
        /// Son ödemeleri yükler
        /// </summary>
        private void LoadRecentPayments()
        {
            if (_residentFlat == null)
            {
                gcRecentPayments.DataSource = null;
                return;
            }

            try
            {
                using (var context = new DataAccess.ApartmentManagementContext())
                {
                    var recentPayments = context.Payments
                        .Where(p => p.FlatId == _residentFlat.Id)
                        .OrderByDescending(p => p.Date)
                        .Take(10)
                        .Select(p => new
                        {
                            p.Id,
                            Tarih = p.Date.ToString("dd.MM.yyyy"),
                            Tip = p.Type ?? "Aidat",
                            Tutar = p.Amount.ToString("N2") + " TL"
                        })
                        .ToList();

                    gcRecentPayments.DataSource = recentPayments;
                    gvRecentPayments.PopulateColumns();

                    // Sütun ayarları
                    if (gvRecentPayments.Columns["Id"] != null)
                        gvRecentPayments.Columns["Id"].Visible = false;

                    if (gvRecentPayments.Columns["Tarih"] != null)
                    {
                        gvRecentPayments.Columns["Tarih"].Caption = "Tarih";
                        gvRecentPayments.Columns["Tarih"].Width = 120;
                    }

                    if (gvRecentPayments.Columns["Tip"] != null)
                    {
                        gvRecentPayments.Columns["Tip"].Caption = "Ödeme Tipi";
                        gvRecentPayments.Columns["Tip"].Width = 150;
                    }

                    if (gvRecentPayments.Columns["Tutar"] != null)
                    {
                        gvRecentPayments.Columns["Tutar"].Caption = "Tutar";
                        gvRecentPayments.Columns["Tutar"].Width = 120;
                        gvRecentPayments.Columns["Tutar"].AppearanceCell.TextOptions.HAlignment = 
                            DevExpress.Utils.HorzAlignment.Far;
                    }

                    // Son ödeme kartını güncelle
                    var lastPayment = recentPayments.FirstOrDefault();
                    if (lastPayment != null)
                    {
                        var lblValue = pnlCardRecentPayments.Controls.OfType<LabelControl>()
                            .FirstOrDefault(l => l.Appearance.Font.Bold);
                        if (lblValue != null)
                        {
                            lblValue.Text = lastPayment.Tutar;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.Swal.Error("Ödemeler yüklenirken hata oluştu: " + ex.Message);
            }
        }
    }
}

