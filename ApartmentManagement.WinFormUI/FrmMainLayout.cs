#nullable disable
// FrmMainLayout.cs
// Ana Layout Formu - Premium UI: Sidebar, Navbar, Footer ve Content
// Özellikler: Açılır-kapanır sidebar, animasyonlar, modern tasarım
using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;
using ApartmentManagement.DataAccess.Entities;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Ana layout formu - Premium Dashboard
    /// </summary>
    public class FrmMainLayout : DevExpress.XtraEditors.XtraForm
    {
        // === RENK PALETİ (Modern Dark Theme) ===
        private static readonly Color PRIMARY_DARK = Color.FromArgb(15, 23, 42);      // Slate 900
        private static readonly Color SECONDARY_DARK = Color.FromArgb(30, 41, 59);    // Slate 800
        private static readonly Color ACCENT_BLUE = Color.FromArgb(59, 130, 246);     // Blue 500
        private static readonly Color ACCENT_GREEN = Color.FromArgb(34, 197, 94);     // Green 500
        private static readonly Color ACCENT_YELLOW = Color.FromArgb(234, 179, 8);    // Yellow 500
        private static readonly Color ACCENT_RED = Color.FromArgb(239, 68, 68);       // Red 500
        private static readonly Color ACCENT_PURPLE = Color.FromArgb(168, 85, 247);   // Purple 500
        private static readonly Color TEXT_PRIMARY = Color.FromArgb(248, 250, 252);   // Slate 50
        private static readonly Color TEXT_SECONDARY = Color.FromArgb(148, 163, 184); // Slate 400
        private static readonly Color CONTENT_BG = Color.FromArgb(241, 245, 249);     // Slate 100
        private static readonly Color CARD_BG = Color.White;
        private static readonly Color HOVER_BG = Color.FromArgb(51, 65, 85);          // Slate 700

        // Layout Panels
        private Panel pnlNavbar;
        private Panel pnlSidebar;
        private Panel pnlFooter;
        private Panel pnlContent;

        // Sidebar Controls
        private SimpleButton btnToggleSidebar;
        private bool _sidebarExpanded = true;
        private int _sidebarExpandedWidth = 220;
        private int _sidebarCollapsedWidth = 60;
        private System.Windows.Forms.Timer _animationTimer;
        private int _targetWidth;

        // Navbar Controls
        private LabelControl lblLogo;
        private LabelControl lblUserName;
        private SimpleButton btnLogout;

        // Sidebar Menu Buttons
        private List<SimpleButton> _menuButtons = new List<SimpleButton>();

        // Current User
        private User _currentUser;

        /// <summary>
        /// FrmMainLayout constructor
        /// </summary>
        public FrmMainLayout(User user)
        {
            _currentUser = user;
            InitializeComponent();
            LoadSidebarMenu();
            ShowDashboard(); // Ana sayfa ilk gösterilsin
        }

        /// <summary>
        /// Form bileşenlerini başlatır
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = "Apartman Yönetim Sistemi";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = PRIMARY_DARK;
            this.Font = new Font("Tahoma", 8.25F);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(1000, 700);

            // Animation Timer
            _animationTimer = new System.Windows.Forms.Timer();
            _animationTimer.Interval = 10;
            _animationTimer.Tick += AnimationTimer_Tick;

            // ========== NAVBAR (Üst - 60px) ==========
            this.pnlNavbar = new Panel();
            this.pnlNavbar.Dock = DockStyle.Top;
            this.pnlNavbar.Height = 60;
            this.pnlNavbar.BackColor = PRIMARY_DARK;
            this.pnlNavbar.Padding = new Padding(10, 0, 20, 0);
            this.Controls.Add(this.pnlNavbar);

            // Toggle Sidebar Button
            this.btnToggleSidebar = new SimpleButton();
            this.btnToggleSidebar.Text = "☰";
            this.btnToggleSidebar.Size = new Size(45, 40);
            this.btnToggleSidebar.Location = new Point(15, 10);
            this.btnToggleSidebar.Appearance.Font = new Font("Tahoma", 14F);
            this.btnToggleSidebar.Appearance.BackColor = SECONDARY_DARK;
            this.btnToggleSidebar.Appearance.ForeColor = TEXT_PRIMARY;
            this.btnToggleSidebar.Appearance.Options.UseBackColor = true;
            this.btnToggleSidebar.Appearance.Options.UseForeColor = true;
            this.btnToggleSidebar.Cursor = Cursors.Hand;
            this.btnToggleSidebar.Click += BtnToggleSidebar_Click;
            this.pnlNavbar.Controls.Add(this.btnToggleSidebar);

            // Logo
            this.lblLogo = new LabelControl();
            this.lblLogo.Text = "🏢 APARTMAN YÖNETİM SİSTEMİ";
            this.lblLogo.Appearance.Font = new Font("Tahoma", 13F, FontStyle.Bold);
            this.lblLogo.Appearance.ForeColor = TEXT_PRIMARY;
            this.lblLogo.Location = new Point(80, 18);
            this.pnlNavbar.Controls.Add(this.lblLogo);

            // User Info - Doğrudan navbar'a ekle (anchor ile)
            this.lblUserName = new LabelControl();
            this.lblUserName.Text = _currentUser != null ? $"👤 {_currentUser.FirstName} {_currentUser.LastName}" : "👤 Kullanıcı";
            this.lblUserName.Appearance.Font = new Font("Tahoma", 9F);
            this.lblUserName.Appearance.ForeColor = TEXT_SECONDARY;
            this.lblUserName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.lblUserName.Location = new Point(this.pnlNavbar.Width - 220, 22);
            this.pnlNavbar.Controls.Add(this.lblUserName);

            // Logout Button - Doğrudan navbar'a ekle
            this.btnLogout = new SimpleButton();
            this.btnLogout.Text = "Çıkış";
            this.btnLogout.Size = new Size(70, 35);
            this.btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnLogout.Location = new Point(this.pnlNavbar.Width - 90, 12);
            this.btnLogout.Appearance.Font = new Font("Tahoma", 9F);
            this.btnLogout.Appearance.BackColor = ACCENT_RED;
            this.btnLogout.Appearance.ForeColor = TEXT_PRIMARY;
            this.btnLogout.Appearance.Options.UseBackColor = true;
            this.btnLogout.Appearance.Options.UseForeColor = true;
            this.btnLogout.Cursor = Cursors.Hand;
            this.btnLogout.Click += (s, e) => { this.Hide(); new FrmLogin().Show(); };
            this.pnlNavbar.Controls.Add(this.btnLogout);

            // ========== FOOTER (Alt - 35px) ==========
            this.pnlFooter = new Panel();
            this.pnlFooter.Dock = DockStyle.Bottom;
            this.pnlFooter.Height = 35;
            this.pnlFooter.BackColor = PRIMARY_DARK;
            this.Controls.Add(this.pnlFooter);

            var lblCopyright = new LabelControl();
            lblCopyright.Text = "© 2025 Apartman Yönetim Sistemi | v1.0 | Tüm Hakları Saklıdır";
            lblCopyright.Appearance.Font = new Font("Tahoma", 8F);
            lblCopyright.Appearance.ForeColor = TEXT_SECONDARY;
            lblCopyright.Dock = DockStyle.Fill;
            lblCopyright.AutoSizeMode = LabelAutoSizeMode.None;
            lblCopyright.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lblCopyright.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.pnlFooter.Controls.Add(lblCopyright);

            // ========== SIDEBAR (Sol - 220px/60px) ==========
            this.pnlSidebar = new Panel();
            this.pnlSidebar.Dock = DockStyle.Left;
            this.pnlSidebar.Width = _sidebarExpandedWidth;
            this.pnlSidebar.BackColor = SECONDARY_DARK;
            this.pnlSidebar.Padding = new Padding(0, 15, 0, 15);
            this.Controls.Add(this.pnlSidebar);

            // ========== CONTENT (Orta) ==========
            this.pnlContent = new Panel();
            this.pnlContent.Dock = DockStyle.Fill;
            this.pnlContent.BackColor = CONTENT_BG;
            this.pnlContent.Padding = new Padding(25);
            this.pnlContent.AutoScroll = true;
            this.Controls.Add(this.pnlContent);

            // Bring content to front
            this.pnlContent.BringToFront();

            this.ResumeLayout(false);
        }

        #region Sidebar Toggle Animation

        private void BtnToggleSidebar_Click(object sender, System.EventArgs e)
        {
            _sidebarExpanded = !_sidebarExpanded;
            _targetWidth = _sidebarExpanded ? _sidebarExpandedWidth : _sidebarCollapsedWidth;
            _animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, System.EventArgs e)
        {
            int step = 15;
            if (pnlSidebar.Width < _targetWidth)
            {
                pnlSidebar.Width = Math.Min(pnlSidebar.Width + step, _targetWidth);
            }
            else if (pnlSidebar.Width > _targetWidth)
            {
                pnlSidebar.Width = Math.Max(pnlSidebar.Width - step, _targetWidth);
            }

            if (pnlSidebar.Width == _targetWidth)
            {
                _animationTimer.Stop();
                LoadSidebarMenu(); // Menüyü yeniden yükle
            }
        }

        private void UpdateMenuButtonTexts()
        {
            foreach (var btn in _menuButtons)
            {
                if (btn.Tag is string[] texts && texts.Length >= 2)
                {
                    btn.Text = _sidebarExpanded ? texts[0] : texts[1];
                }
            }
        }

        #endregion

        #region Sidebar Menu

        private void LoadSidebarMenu()
        {
            pnlSidebar.Controls.Clear();
            _menuButtons.Clear();

            int buttonY = 20;
            int buttonHeight = 45;
            int spacing = 8;

            // Rol başlığı - Sadece sidebar açıkken göster
            if (_sidebarExpanded)
            {
                var lblRole = new LabelControl();
                lblRole.Text = GetRoleDisplayName(_currentUser?.Role);
                lblRole.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
                lblRole.Appearance.ForeColor = ACCENT_PURPLE;
                lblRole.Location = new Point(15, buttonY);
                this.pnlSidebar.Controls.Add(lblRole);
                buttonY += 35;

                // Separator
                var separator = CreateSeparator(buttonY);
                this.pnlSidebar.Controls.Add(separator);
                buttonY += 15;
            }
            else
            {
                buttonY = 15; // Kapalıyken daha az boşluk
            }

            // Menu Items based on role
            string role = _currentUser?.Role ?? "Resident";

            // Dashboard - Everyone
            AddMenuButton("🏠 Ana Sayfa", "🏠", buttonY, () => ShowDashboard(), true);
            buttonY += buttonHeight + spacing;

            if (role == "SuperAdmin" || role == "Admin")
            {
                AddMenuButton("🌐 Siteler", "🌐", buttonY, () => ShowContent(new FrmSiteList()));
                buttonY += buttonHeight + spacing;

                AddMenuButton("👥 Kullanıcılar", "👥", buttonY, () => ShowContent(new FrmUserList()));
                buttonY += buttonHeight + spacing;
            }

            if (role == "SuperAdmin" || role == "Admin" || role == "SiteManager")
            {
                AddMenuButton("🏢 Apartmanlar", "🏢", buttonY, () => ShowContent(new FrmApartmentList()));
                buttonY += buttonHeight + spacing;

                AddMenuButton("🏗 Bloklar", "🏗", buttonY, () => ShowContent(new FrmBlockList()));
                buttonY += buttonHeight + spacing;
            }

            if (role == "SuperAdmin" || role == "Admin" || role == "SiteManager" || role == "ApartmentManager")
            {
                AddMenuButton("🚪 Daireler", "🚪", buttonY, () => ShowContent(new FrmFlatList()));
                buttonY += buttonHeight + spacing;
            }

            // Onay Bekleyenler - Admin ve SiteManager için
            if (role == "SuperAdmin" || role == "Admin" || role == "SiteManager")
            {
                AddMenuButton("⏳ Onay Bekleyenler", "⏳", buttonY, () => ShowContent(new FrmPendingApprovals(_currentUser)));
                buttonY += buttonHeight + spacing;
            }

            // Separator before settings
            buttonY += 15;
            var separator2 = CreateSeparator(buttonY);
            this.pnlSidebar.Controls.Add(separator2);
            buttonY += 20;

            // Settings
            AddMenuButton("⚙️ Ayarlar", "⚙️", buttonY, () => Helpers.Swal.Info("Ayarlar yakında!"));
        }

        private Panel CreateSeparator(int y)
        {
            var sep = new Panel();
            sep.Size = new Size(_sidebarExpandedWidth - 30, 1);
            sep.Location = new Point(15, y);
            sep.BackColor = Color.FromArgb(71, 85, 105);
            return sep;
        }

        private void AddMenuButton(string fullText, string iconText, int y, Action onClick, bool isActive = false)
        {
            var btn = new SimpleButton();
            btn.Text = _sidebarExpanded ? fullText : iconText;
            btn.Tag = new string[] { fullText, iconText };
            
            // Sidebar durumuna göre boyut ve konum
            int btnWidth = _sidebarExpanded ? _sidebarExpandedWidth - 20 : _sidebarCollapsedWidth - 10;
            int btnX = _sidebarExpanded ? 10 : 5;
            
            btn.Size = new Size(btnWidth, 45);
            btn.Location = new Point(btnX, y);
            btn.Appearance.Font = new Font("Tahoma", _sidebarExpanded ? 10F : 14F);
            btn.Appearance.BackColor = isActive ? ACCENT_BLUE : SECONDARY_DARK;
            btn.Appearance.ForeColor = TEXT_PRIMARY;
            btn.Appearance.Options.UseBackColor = true;
            btn.Appearance.Options.UseForeColor = true;
            btn.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near; // Sola hizalı
            btn.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            if (_sidebarExpanded)
            {
                btn.Padding = new Padding(15, 0, 0, 0); // Sol taraftan padding ekle
            }
            btn.Cursor = Cursors.Hand;
            btn.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            
            // Hover effects
            Color normalColor = isActive ? ACCENT_BLUE : SECONDARY_DARK;
            btn.MouseEnter += (s, e) => {
                if (!isActive)
                    btn.Appearance.BackColor = HOVER_BG;
            };
            btn.MouseLeave += (s, e) => {
                if (!isActive)
                    btn.Appearance.BackColor = SECONDARY_DARK;
            };

            btn.Click += (s, e) => {
                // Reset all buttons
                foreach (var b in _menuButtons)
                {
                    b.Appearance.BackColor = Color.Transparent;
                }
                btn.Appearance.BackColor = ACCENT_BLUE;
                onClick?.Invoke();
            };

            this.pnlSidebar.Controls.Add(btn);
            _menuButtons.Add(btn);
        }

        #endregion

        #region Content Management

        private void ShowContent(Form form)
        {
            this.pnlContent.Controls.Clear();

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            form.BackColor = CONTENT_BG;

            this.pnlContent.Controls.Add(form);
            form.Show();
        }

        private void ShowDashboard()
        {
            this.pnlContent.Controls.Clear();

            // Merkez içerik container
            Panel contentContainer = new Panel();
            contentContainer.Size = new Size(1100, 2000); // AutoScroll için yeterli yükseklik
            contentContainer.Location = new Point((this.pnlContent.Width - 1100) / 2, 20);
            contentContainer.Anchor = AnchorStyles.Top;
            contentContainer.BackColor = Color.Transparent;
            this.pnlContent.Controls.Add(contentContainer);

            // Responsive ortalama
            this.pnlContent.Resize += (s, e) => {
                contentContainer.Location = new Point((this.pnlContent.Width - 1100) / 2, 20);
            };

            int currentY = 10;
            int totalWidth = 1100;

            // ===== 1. HEADER SECTION =====
            // Başlık ve Yenile Butonu
            var lblTitle = new LabelControl();
            lblTitle.Text = "Dashboard";
            lblTitle.Appearance.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = PRIMARY_DARK;
            lblTitle.Location = new Point(0, currentY);
            
            // Icon
            var lblTitleIcon = new LabelControl();
            lblTitleIcon.Text = "📊"; // Dashboard icon
            lblTitleIcon.Appearance.Font = new Font("Segoe UI Emoji", 18F);
            lblTitleIcon.Location = new Point(lblTitle.Width + 140, currentY); // Başlık yanı

            contentContainer.Controls.Add(lblTitle);
            // contentContainer.Controls.Add(lblTitleIcon); // İkon opsiyonel

            var lblSubtitle = new LabelControl();
            lblSubtitle.Text = "Sistem genel durumu ve istatistikleri";
            lblSubtitle.Appearance.Font = new Font("Segoe UI", 9F);
            lblSubtitle.Appearance.ForeColor = Color.Gray;
            lblSubtitle.Location = new Point(0, currentY + 35);
            contentContainer.Controls.Add(lblSubtitle);

            // Refresh Button
            var btnRefresh = new SimpleButton();
            btnRefresh.Text = "Yenile";
            btnRefresh.Size = new Size(90, 32);
            btnRefresh.Location = new Point(totalWidth - 90, currentY);
            btnRefresh.Appearance.Font = new Font("Segoe UI", 9F);
            btnRefresh.Appearance.BackColor = Color.White;
            btnRefresh.Appearance.ForeColor = ACCENT_BLUE;
            btnRefresh.Appearance.BorderColor = ACCENT_BLUE;
            btnRefresh.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += (s, e) => ShowDashboard(); // Reload
            contentContainer.Controls.Add(btnRefresh);

            currentY += 70;

            // ===== 2. STATS CARDS (TOP ROW) =====
            // 4 Adet yan yana. 
            // Genişlik: 1100 / 4 = 275. Boşluklar (3x20=60). (1100-60)/4 = 260px.
            int cardWidth = 260;
            int cardHeight = 100;
            int spacing = 20;

            // Kartlar görseldeki sıraya göre: Site, Blok, Apartman, Daire
            CreateModernStatCard("🏢", "TOPLAM SİTE", GetSiteCount().ToString(), ACCENT_BLUE, 
                0, currentY, cardWidth, cardHeight, contentContainer);

            CreateModernStatCard("🏗", "TOPLAM BLOK", GetBlockCount().ToString(), ACCENT_GREEN, 
                cardWidth + spacing, currentY, cardWidth, cardHeight, contentContainer);

            CreateModernStatCard("🏠", "TOPLAM APARTMAN", GetApartmentCount().ToString(), ACCENT_YELLOW, 
                (cardWidth + spacing) * 2, currentY, cardWidth, cardHeight, contentContainer);

            CreateModernStatCard("🚪", "TOPLAM DAİRE", GetFlatCount().ToString(), Color.FromArgb(0, 188, 212), // Cyan
                (cardWidth + spacing) * 3, currentY, cardWidth, cardHeight, contentContainer);

            currentY += cardHeight + 30;

            // ===== 3. MIDDLE SECTION (3 COLUMNS) =====
            // Görselde: "Daire Durum Dağılımı", "Site Analizi", "Son İşlemler"
            // Genişlikler: 350, 400, 310 (Toplam ~1100)
            
            int col1Width = 340;
            int col2Width = 400;
            int col3Width = 320;
            // Spacing: 20px
            
            int midHeight = 350;

            // --- Panel 1: Daire Durum Dağılımı ---
            var pnlStatus = CreateContentPanel("Genel Görünüm", "Daire Durum Dağılımı", col1Width, midHeight, 0, currentY, contentContainer);
            
            // Progress Bars logic
            int totalFlats = GetFlatCount();
            int emptyFlats = GetEmptyFlatCount();
            int occupiedFlats = totalFlats - emptyFlats; // Basitleştirilmiş
            // Diğer durumlar için veri yoksa 0 varsayalım
            
            // Boş
            AddProgressBar(pnlStatus, "Boş", emptyFlats, totalFlats, Color.Gray, 60);
            // Dolu
            AddProgressBar(pnlStatus, "Dolu", occupiedFlats, totalFlats, ACCENT_GREEN, 110);
            // Bakım (Dummy)
            AddProgressBar(pnlStatus, "Bakım", 0, totalFlats, ACCENT_YELLOW, 160);
            // Tadilat (Dummy)
            AddProgressBar(pnlStatus, "Tadilat", 0, totalFlats, ACCENT_RED, 210);

            // --- Panel 2: Site Analizi & Heatmap (Chart) ---
            var pnlChart = CreateContentPanel("Analiz", "Site Analizi & Heatmap", col2Width, midHeight, col1Width + spacing, currentY, contentContainer);
            
            // Chart Ekleme
            var chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            chart.Parent = pnlChart;
            chart.Location = new Point(10, 50);
            chart.Size = new Size(col2Width - 20, midHeight - 70);
            var area = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
            chart.ChartAreas.Add(area);
            
            var series = new System.Windows.Forms.DataVisualization.Charting.Series();
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.SplineArea;
            series.Color = Color.FromArgb(100, 34, 197, 94); // Transparent Green
            series.BorderColor = ACCENT_GREEN;
            series.BorderWidth = 2;
            
            // Dummy Trend Data
            series.Points.AddXY("Oca", 10);
            series.Points.AddXY("Şub", 15);
            series.Points.AddXY("Mar", 12);
            series.Points.AddXY("Nis", 25);
            series.Points.AddXY("May", 20);
            series.Points.AddXY("Haz", 35);
            chart.Series.Add(series);
            
            // Header buttons in chart panel (Analiz, Grafik)
            var btnChartAction = new SimpleButton();
            btnChartAction.Text = "Grafik";
            btnChartAction.Size = new Size(50, 20);
            btnChartAction.Location = new Point(col2Width - 60, 15);
            btnChartAction.Appearance.Font = new Font("Segoe UI", 7F);
            btnChartAction.Appearance.BackColor = Color.FromArgb(0, 188, 212); // Cyan
            btnChartAction.Appearance.ForeColor = Color.White;
            btnChartAction.Appearance.Options.UseBackColor = true;
            btnChartAction.Appearance.Options.UseForeColor = true;
            btnChartAction.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            pnlChart.Controls.Add(btnChartAction);


            // --- Panel 3: Son İşlemler (Siteler Listesi) ---
            var pnlList = CreateContentPanel("Hızlı Erişim", "Son İşlemler / Siteler", col3Width, midHeight, col1Width + col2Width + (spacing * 2), currentY, contentContainer);
            
            // Liste Ekleme (Basit Labels for demo or Grid)
            FlowLayoutPanel flowList = new FlowLayoutPanel();
            flowList.FlowDirection = FlowDirection.TopDown;
            flowList.WrapContents = false;
            flowList.AutoScroll = true;
            flowList.Size = new Size(col3Width - 20, midHeight - 60);
            flowList.Location = new Point(10, 50);
            pnlList.Controls.Add(flowList);

            // Dummy List Items similar to screenshot
            AddListItem(flowList, "Siteler", "Samet", "#06 • 24.10.2025", ACCENT_BLUE);
            AddListItem(flowList, "Siteler", "Alkan Sitesi", "#002 • 10.10.2025", ACCENT_BLUE);
            AddListItem(flowList, "Siteler", "Çalışır Sitesi", "#004 • 10.10.2025", ACCENT_BLUE);
            AddListItem(flowList, "Bloklar", "A Blok", "Samet • 24.10.2025", ACCENT_GREEN);

            currentY += midHeight + 30;

            // ===== 4. QUICK ACTIONS (Hızlı İşlemler) =====
            var lblActions = new LabelControl();
            lblActions.Text = "⚡ Hızlı İşlemler";
            lblActions.Appearance.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblActions.Appearance.ForeColor = PRIMARY_DARK;
            lblActions.Location = new Point(0, currentY);
            contentContainer.Controls.Add(lblActions);

            currentY += 40; // Biraz daha fazla boşluk

            int actionHeight = 110; // Biraz daha yüksek kartlar
            // 4 Buton, same grid as top cards
            CreateActionCard("Yeni Site", "Site ekle", "+", ACCENT_BLUE, 
                0, currentY, cardWidth, actionHeight, 
                () => DevExpress.XtraEditors.XtraMessageBox.Show("Eklendi"), contentContainer);

            CreateActionCard("Yeni Blok", "Blok ekle", "+", ACCENT_GREEN, 
                cardWidth + spacing, currentY, cardWidth, actionHeight, 
                () => ShowContent(new FrmBlockManagement()), contentContainer);

            CreateActionCard("Yeni Apartman", "Apartman ekle", "+", ACCENT_YELLOW, 
                (cardWidth + spacing) * 2, currentY, cardWidth, actionHeight, 
                () => ShowContent(new FrmApartmentManagement()), contentContainer);

            CreateActionCard("Daire Bul", "Daire ara", "🔍", Color.FromArgb(100, 100, 100), 
                (cardWidth + spacing) * 3, currentY, cardWidth, actionHeight, 
                () => ShowContent(new FrmFlatList()), contentContainer, true); // true = outline style
        }

        // --- Helper Methods ---

        private Panel CreateContentPanel(string topLabel, string title, int w, int h, int x, int y, Control parent)
        {
            // Panel container
            var pnl = new Helpers.RoundedPanel();
            pnl.Size = new Size(w, h);
            pnl.Location = new Point(x, y);
            pnl.BackColor = Color.White;
            pnl.BorderRadius = 10;
            pnl.BorderThickness = 0; // Shadow effect simulation handled by color/bg
            parent.Controls.Add(pnl);

            // Optional Top Label (e.g. "Genel Görünüm" outside or inside with icon)
            // Screenshot shows icon + label top left
            
            var lblHeader = new LabelControl();
            lblHeader.Text = title;
            lblHeader.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblHeader.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            lblHeader.Location = new Point(15, 15);
            pnl.Controls.Add(lblHeader);

            return pnl;
        }

        private void AddProgressBar(Panel parent, string label, int value, int total, Color color, int y)
        {
            // Label
            var lbl = new LabelControl();
            lbl.Text = label;
            lbl.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lbl.Location = new Point(20, y);
            parent.Controls.Add(lbl);

            // Value text (Right aligned)
            var lblVal = new LabelControl();
            int percent = total > 0 ? (value * 100 / total) : 0;
            lblVal.Text = $"{value} ({percent}%)";
            lblVal.Appearance.Font = new Font("Segoe UI", 8.25F);
            lblVal.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            lblVal.AutoSizeMode = LabelAutoSizeMode.None;
            lblVal.Size = new Size(100, 15);
            lblVal.Location = new Point(parent.Width - 120, y);
            parent.Controls.Add(lblVal);

            // Progress Bar Background
            var pnlBg = new Panel();
            pnlBg.Size = new Size(parent.Width - 40, 8);
            pnlBg.Location = new Point(20, y + 20);
            pnlBg.BackColor = Color.FromArgb(233, 236, 239); // Light Gray
            parent.Controls.Add(pnlBg);

            // Progress Bar Foreground
            var pnlFg = new Panel();
            int w = total > 0 ? (int)((float)value / total * pnlBg.Width) : 0;
            pnlFg.Size = new Size(Math.Max(w, 5), 8); // En az 5px görünmesi için
            if (value == 0) pnlFg.Width = 0;
            pnlFg.Location = new Point(0, 0); // Relative to bg
            pnlFg.BackColor = color;
            pnlBg.Controls.Add(pnlFg);
        }

        private void AddListItem(FlowLayoutPanel parent, string category, string title, string detail, Color iconColor)
        {
            Panel item = new Panel();
            item.Size = new Size(parent.Width - 5, 50);
            item.Margin = new Padding(0, 0, 0, 10);
            
            // Icon / Line
            Panel line = new Panel();
            line.Size = new Size(3, 40);
            line.Location = new Point(5, 5);
            line.BackColor = iconColor;
            item.Controls.Add(line);

            // Category
            LabelControl lblCat = new LabelControl();
            lblCat.Text = category;
            lblCat.Appearance.Font = new Font("Segoe UI", 7F, FontStyle.Bold);
            lblCat.Appearance.ForeColor = iconColor;
            lblCat.Location = new Point(15, 5);
            item.Controls.Add(lblCat);

            // Title
            LabelControl lblTit = new LabelControl();
            lblTit.Text = title;
            lblTit.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTit.Location = new Point(15, 18);
            item.Controls.Add(lblTit);

            // Detail
            LabelControl lblDet = new LabelControl();
            lblDet.Text = detail;
            lblDet.Appearance.Font = new Font("Segoe UI", 8F);
            lblDet.Appearance.ForeColor = Color.Gray;
            lblDet.Location = new Point(15, 34);
            item.Controls.Add(lblDet);

            parent.Controls.Add(item);
        }

        private void CreateModernStatCard(string icon, string title, string value, Color color,
            int x, int y, int width, int height, Panel parent)
        {
            var card = new Helpers.RoundedPanel();
            card.Size = new Size(width, height);
            card.Location = new Point(x, y);
            card.BackColor = Color.White;
            card.BorderRadius = 15;
            parent.Controls.Add(card);

            // Title
            var lblTitle = new LabelControl();
            lblTitle.Text = title;
            lblTitle.Appearance.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = color;
            lblTitle.Location = new Point(20, 20);
            card.Controls.Add(lblTitle);

            // Value
            var lblValue = new LabelControl();
            lblValue.Text = value;
            lblValue.Appearance.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblValue.Appearance.ForeColor = Color.FromArgb(50, 50, 50);
            lblValue.Location = new Point(20, 40);
            card.Controls.Add(lblValue);

            // Icon Circle (Right Side)
            var pnlIcon = new Helpers.RoundedPanel();
            pnlIcon.Size = new Size(50, 50);
            pnlIcon.Location = new Point(width - 65, 25);
            pnlIcon.BackColor = ControlPaint.Light(color, 0.8f); 
            pnlIcon.BorderRadius = 25;
            card.Controls.Add(pnlIcon);

            // İkon labelını pnlIcon'un child'ı yapıp tam ortaya yerleştir
            var lblIcon = new LabelControl();
            lblIcon.Text = icon;
            lblIcon.Appearance.Font = new Font("Segoe UI Emoji", 18F);
            lblIcon.Appearance.ForeColor = color;
            lblIcon.AutoSizeMode = LabelAutoSizeMode.None;
            lblIcon.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lblIcon.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            lblIcon.Size = new Size(50, 50); // Panel ile aynı boyut
            lblIcon.Location = new Point(0, 0); // Panel içinde (0,0) - tam merkez
            pnlIcon.Controls.Add(lblIcon); // Panel'in child'ı olarak ekle
        }

        private void CreateActionCard(string title, string subtitle, string icon, Color color,
            int x, int y, int width, int height, Action onClick, Panel parent, bool outline = false)
        {
            var card = new Helpers.RoundedPanel();
            card.Size = new Size(width, height);
            card.Location = new Point(x, y);
            card.Cursor = Cursors.Hand;
            card.Click += (s, e) => onClick?.Invoke();
            
            // Yeni Site için dolu mavi arka plan, diğerleri outline
            if (title == "Yeni Site")
            {
                card.BackColor = ACCENT_BLUE;
                card.BorderThickness = 0;
            }
            else
            {
                card.BackColor = Color.White;
                card.BorderColor = color;
                card.BorderThickness = 2;
            }

            card.BorderRadius = 12;
            parent.Controls.Add(card);

            // Hover efekti
            Color originalBg = card.BackColor;
            card.MouseEnter += (s, e) => {
                if (title == "Yeni Site")
                    card.BackColor = ControlPaint.Dark(ACCENT_BLUE, 0.1f);
                else
                    card.BackColor = ControlPaint.LightLight(color);
            };
            card.MouseLeave += (s, e) => card.BackColor = originalBg;

            // İkon
            var lblIcon = new LabelControl();
            lblIcon.Text = icon;
            lblIcon.Appearance.Font = new Font("Segoe UI", 30F, FontStyle.Bold);
            lblIcon.Appearance.ForeColor = (title == "Yeni Site") ? Color.White : color;
            lblIcon.AutoSizeMode = LabelAutoSizeMode.None;
            lblIcon.Size = new Size(width, 45);
            lblIcon.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lblIcon.Location = new Point(0, 15);
            lblIcon.Click += (s, e) => onClick?.Invoke();
            card.Controls.Add(lblIcon);

            // Başlık
            var lblTitle = new LabelControl();
            lblTitle.Text = title;
            lblTitle.Appearance.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = (title == "Yeni Site") ? Color.White : color;
            lblTitle.AutoSizeMode = LabelAutoSizeMode.None;
            lblTitle.Size = new Size(width - 10, 22);
            lblTitle.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lblTitle.Location = new Point(5, 62);
            lblTitle.Click += (s, e) => onClick?.Invoke();
            card.Controls.Add(lblTitle);
            
            // Alt yazı
            var lblSub = new LabelControl();
            lblSub.Text = subtitle;
            lblSub.Appearance.Font = new Font("Segoe UI", 8.5F);
            lblSub.Appearance.ForeColor = (title == "Yeni Site") 
                ? Color.FromArgb(200, 255, 255, 255)
                : Color.FromArgb(130, 130, 130);
            lblSub.AutoSizeMode = LabelAutoSizeMode.None;
            lblSub.Size = new Size(width - 10, 18);
            lblSub.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lblSub.Location = new Point(5, 86);
            lblSub.Click += (s, e) => onClick?.Invoke();
            card.Controls.Add(lblSub);
        }

        #endregion

        #region Helper Methods

        private int GetUserCount()
        {
            try { return new Business.Services.SUser().GetAll()?.Count ?? 0; }
            catch { return 0; }
        }

        private int GetApartmentCount()
        {
            try { return new Business.Services.SApartment().GetAll()?.Count ?? 0; }
            catch { return 0; }
        }

        private int GetFlatCount()
        {
            try { return new Business.Services.SFlat().GetAll()?.Count ?? 0; }
            catch { return 0; }
        }

        private int GetSiteCount()
        {
            try { return new Business.Services.SSite().GetAll()?.Count ?? 0; }
            catch { return 0; }
        }

        private int GetBlockCount()
        {
            try { return new Business.Services.SBlock().GetAll()?.Count ?? 0; }
            catch { return 0; }
        }

        private int GetEmptyFlatCount()
        {
            try { return new Business.Services.SFlat().GetAll()?.Where(f => f.IsEmpty).Count() ?? 0; }
            catch { return 0; }
        }

        private string GetRoleDisplayName(string role)
        {
            return role switch
            {
                "SuperAdmin" => "🔑 Super Admin",
                "Admin" => "👑 Admin",
                "SiteManager" => "🏢 Site Yöneticisi",
                "ApartmentManager" => "🏠 Apartman Yöneticisi",
                "Resident" => "🏡 Sakin",
                _ => "👤 Kullanıcı"
            };
        }

        #endregion
    }
}
