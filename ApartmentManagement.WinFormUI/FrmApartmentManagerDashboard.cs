#nullable disable
// FrmApartmentManagerDashboard.cs
// ApartmentManager Dashboard - Apartman yöneticileri için özel dashboard ekranı
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
    /// ApartmentManager Dashboard - Apartman yöneticileri için özel dashboard
    /// </summary>
    public partial class FrmApartmentManagerDashboard : DevExpress.XtraEditors.XtraForm
    {
        private IFlat _flatService;
        private IApartment _apartmentService;
        private IDues _duesService;
        private User _currentUser;
        private Flat _managerFlat;
        private Apartment _managerApartment;

        // Controls
        private Panel pnlContainer;
        private LabelControl lblTitle;
        private LabelControl lblSubtitle;
        private SimpleButton btnRefresh;

        // Stat Cards
        private Panel pnlCardApartmentInfo;
        private Panel pnlCardTotalFlats;
        private Panel pnlCardOccupiedFlats;
        private Panel pnlCardEmptyFlats;
        private Panel pnlCardPendingDues;
        private Panel pnlCardTotalDuesAmount;

        // Quick Actions
        private SimpleButton btnViewFlats;
        private SimpleButton btnViewDues;
        private SimpleButton btnViewPayments;

        /// <summary>
        /// FrmApartmentManagerDashboard constructor
        /// </summary>
        public FrmApartmentManagerDashboard(User user)
        {
            _currentUser = user;
            _flatService = new SFlat();
            _apartmentService = new SApartment();
            _duesService = new SDues();
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
            this.Text = "Apartman Yöneticisi Paneli";
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
            lblTitle.Text = "🏠 Apartman Yöneticisi Paneli";
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

            // ========== STAT CARDS SECTION (6 KART) ==========
            int cardWidth = (contentWidth - 100) / 3; // 3 kart yan yana, 2 satır
            int cardHeight = 140;
            int cardSpacing = 20;

            // İlk satır - 3 kart
            pnlCardApartmentInfo = CreateStatCard(0, currentY, cardWidth, cardHeight,
                Color.FromArgb(59, 130, 246), "🏢", "Apartman", "Yükleniyor...", "");
            pnlContainer.Controls.Add(pnlCardApartmentInfo);

            pnlCardTotalFlats = CreateStatCard(cardWidth + cardSpacing, currentY, cardWidth, cardHeight,
                Color.FromArgb(34, 197, 94), "🚪", "Toplam Daire", "0", "");
            pnlContainer.Controls.Add(pnlCardTotalFlats);

            pnlCardOccupiedFlats = CreateStatCard((cardWidth + cardSpacing) * 2, currentY, cardWidth, cardHeight,
                Color.FromArgb(234, 179, 8), "✅", "Dolu Daire", "0", "");
            pnlContainer.Controls.Add(pnlCardOccupiedFlats);

            currentY += cardHeight + 20;

            // İkinci satır - 3 kart
            pnlCardEmptyFlats = CreateStatCard(0, currentY, cardWidth, cardHeight,
                Color.FromArgb(239, 68, 68), "🔓", "Boş Daire", "0", "");
            pnlContainer.Controls.Add(pnlCardEmptyFlats);

            pnlCardPendingDues = CreateStatCard(cardWidth + cardSpacing, currentY, cardWidth, cardHeight,
                Color.FromArgb(234, 179, 8), "⏳", "Bekleyen Aidat", "0", "");
            pnlContainer.Controls.Add(pnlCardPendingDues);

            pnlCardTotalDuesAmount = CreateStatCard((cardWidth + cardSpacing) * 2, currentY, cardWidth, cardHeight,
                Color.FromArgb(59, 130, 246), "💵", "Toplam Aidat", "0 TL", "");
            pnlContainer.Controls.Add(pnlCardTotalDuesAmount);

            currentY += cardHeight + 40;

            // ========== QUICK ACTIONS SECTION ==========
            var lblQuickActions = new LabelControl();
            lblQuickActions.Text = "Hızlı İşlemler";
            lblQuickActions.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblQuickActions.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblQuickActions.Location = new Point(0, currentY);
            pnlContainer.Controls.Add(lblQuickActions);

            currentY += 35;

            int btnWidth = 250;
            int btnHeight = 50;
            int btnSpacing = 20;

            btnViewFlats = CreateQuickActionButton("🚪 Daireler", 0, currentY, btnWidth, btnHeight,
                Color.FromArgb(59, 130, 246), () => { Helpers.Swal.Info("Daireler ekranına yönlendiriliyorsunuz..."); });
            pnlContainer.Controls.Add(btnViewFlats);

            btnViewDues = CreateQuickActionButton("💰 Aidat Yönetimi", btnWidth + btnSpacing, currentY, btnWidth, btnHeight,
                Color.FromArgb(34, 197, 94), () => { Helpers.Swal.Info("Aidat yönetimi ekranına yönlendiriliyorsunuz..."); });
            pnlContainer.Controls.Add(btnViewDues);

            btnViewPayments = CreateQuickActionButton("💳 Ödemeler", (btnWidth + btnSpacing) * 2, currentY, btnWidth, btnHeight,
                Color.FromArgb(234, 179, 8), () => { Helpers.Swal.Info("Ödemeler ekranı yakında!"); });
            pnlContainer.Controls.Add(btnViewPayments);

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
            btn.Appearance.Font = new Font("Tahoma", 10F, FontStyle.Bold);
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
                // Manager'ın daire bilgilerini al
                _managerFlat = _flatService.GetResidentFlat(_currentUser?.Id ?? 0);
                if (_managerFlat != null && _managerFlat.ApartmentId > 0)
                {
                    _managerApartment = _apartmentService.GetById(_managerFlat.ApartmentId);
                }

                // Kartları güncelle
                UpdateCards();
            }
            catch (Exception ex)
            {
                Helpers.Swal.Error("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Kartları günceller
        /// </summary>
        private void UpdateCards()
        {
            if (_managerApartment == null)
            {
                // Apartman bilgisi yok
                UpdateCardValue(pnlCardApartmentInfo, "Apartman atanmamış");
                UpdateCardValue(pnlCardTotalFlats, "0");
                UpdateCardValue(pnlCardOccupiedFlats, "0");
                UpdateCardValue(pnlCardEmptyFlats, "0");
                UpdateCardValue(pnlCardPendingDues, "0");
                UpdateCardValue(pnlCardTotalDuesAmount, "0 TL");
                return;
            }

            // Apartman bilgisi
            string apartmentInfo = $"{_managerApartment.Name}";
            if (_managerApartment.Block != null)
            {
                apartmentInfo += $"\n{_managerApartment.Block.Name}";
                if (_managerApartment.Block.Site != null)
                {
                    apartmentInfo += $" - {_managerApartment.Block.Site.Name}";
                }
            }
            UpdateCardValue(pnlCardApartmentInfo, apartmentInfo);

            // Daire istatistikleri
            var flats = _flatService.GetAllByApartmentId(_managerApartment.Id);
            int totalFlats = flats.Count;
            int occupiedFlats = flats.Count(f => !f.IsEmpty);
            int emptyFlats = flats.Count(f => f.IsEmpty);

            UpdateCardValue(pnlCardTotalFlats, totalFlats.ToString());
            UpdateCardValue(pnlCardOccupiedFlats, occupiedFlats.ToString());
            UpdateCardValue(pnlCardEmptyFlats, emptyFlats.ToString());

            // Aidat istatistikleri
            var dues = _duesService.GetByApartmentId(_managerApartment.Id);
            int pendingDues = dues.Count(d => !d.IsPaid);
            decimal totalDuesAmount = dues.Sum(d => d.Amount);

            UpdateCardValue(pnlCardPendingDues, pendingDues.ToString());
            UpdateCardValue(pnlCardTotalDuesAmount, totalDuesAmount.ToString("N2") + " TL");
        }

        /// <summary>
        /// Kart değerini günceller
        /// </summary>
        private void UpdateCardValue(Panel card, string value)
        {
            var lblValue = card.Controls.OfType<LabelControl>()
                .FirstOrDefault(l => l.Appearance.Font.Bold);
            if (lblValue != null)
            {
                lblValue.Text = value;
            }
        }
    }
}

