#nullable disable
using DevExpress.XtraBars;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    public partial class FrmAdminDashboard : DevExpress.XtraEditors.XtraForm
    {
        private Panel pnlSidebar;
        private Panel pnlContent;
        private Panel pnlHeader;
        private LabelControl lblAppTitle;
        private LabelControl lblUserName;

        public FrmAdminDashboard()
        {
            InitializeComponent();
            this.Text = "Premium Apartman Yönetimi";
            this.WindowState = FormWindowState.Maximized;
            ShowWelcomePanel();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.ClientSize = new Size(1200, 700);
            this.BackColor = Color.FromArgb(245, 245, 250);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(1000, 600);

            // Header Panel
            this.pnlHeader = new Panel();
            this.pnlHeader.Dock = DockStyle.Top;
            this.pnlHeader.Height = 60;
            this.pnlHeader.BackColor = Color.White;
            this.pnlHeader.Padding = new Padding(20, 0, 20, 0);
            this.Controls.Add(this.pnlHeader);

            // App Title in Header
            this.lblAppTitle = new LabelControl();
            this.lblAppTitle.Text = "🏢 Apartman Yönetim Sistemi";
            this.lblAppTitle.Appearance.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblAppTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            this.lblAppTitle.Appearance.Options.UseFont = true;
            this.lblAppTitle.Appearance.Options.UseForeColor = true;
            this.lblAppTitle.Location = new Point(20, 18);
            this.pnlHeader.Controls.Add(this.lblAppTitle);

            // User Name in Header
            this.lblUserName = new LabelControl();
            this.lblUserName.Text = "👤 Yönetici: Admin";
            this.lblUserName.Appearance.Font = new Font("Segoe UI", 10F);
            this.lblUserName.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            this.lblUserName.Appearance.Options.UseFont = true;
            this.lblUserName.Appearance.Options.UseForeColor = true;
            this.lblUserName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.lblUserName.Location = new Point(this.ClientSize.Width - 180, 22);
            this.pnlHeader.Controls.Add(this.lblUserName);

            // Sidebar Panel
            this.pnlSidebar = new Panel();
            this.pnlSidebar.Dock = DockStyle.Left;
            this.pnlSidebar.Width = 250;
            this.pnlSidebar.BackColor = Color.FromArgb(30, 30, 46);
            this.pnlSidebar.Padding = new Padding(0, 20, 0, 20);
            this.Controls.Add(this.pnlSidebar);
            this.pnlSidebar.BringToFront();

            // Sidebar Menu Items
            CreateMenuButton("🏠  Ana Sayfa", 0, null);
            CreateMenuButton("🏗️  Siteler", 1, () => LoadForm(new FrmSiteList()));
            CreateMenuButton("🏢  Bloklar", 2, () => LoadForm(new FrmBlockList()));
            CreateMenuButton("🏠  Apartmanlar", 3, () => LoadForm(new FrmApartmentList()));
            CreateMenuButton("🚪  Daireler", 4, () => LoadForm(new FrmFlatList()));
            CreateMenuButton("👥  Sakinler", 5, null);
            CreateMenuButton("👤  Kullanıcılar", 6, () => LoadForm(new FrmUserList()));
            CreateMenuButton("💰  Finansal Takip", 7, null);
            CreateMenuButton("⚙️  Ayarlar", 8, null);
            
            // Logout Button (at bottom)
            var btnLogout = new SimpleButton();
            btnLogout.Text = "🚪  Çıkış Yap";
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.Height = 50;
            btnLogout.Appearance.Font = new Font("Segoe UI", 11F);
            btnLogout.Appearance.BackColor = Color.FromArgb(220, 53, 69);
            btnLogout.Appearance.ForeColor = Color.White;
            btnLogout.Appearance.Options.UseBackColor = true;
            btnLogout.Appearance.Options.UseForeColor = true;
            btnLogout.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Click += (s, e) => Application.Restart();
            this.pnlSidebar.Controls.Add(btnLogout);

            // Content Panel
            this.pnlContent = new Panel();
            this.pnlContent.Dock = DockStyle.Fill;
            this.pnlContent.BackColor = Color.FromArgb(245, 245, 250);
            this.pnlContent.Padding = new Padding(20);
            this.Controls.Add(this.pnlContent);
            this.pnlContent.BringToFront();

            this.ResumeLayout(false);
        }

        private void CreateMenuButton(string text, int index, System.Action onClick)
        {
            var btn = new SimpleButton();
            btn.Text = text;
            btn.Size = new Size(250, 50);
            btn.Location = new Point(0, index * 55);
            btn.Appearance.Font = new Font("Segoe UI", 11F);
            btn.Appearance.BackColor = Color.FromArgb(30, 30, 46);
            btn.Appearance.ForeColor = Color.White;
            btn.Appearance.Options.UseBackColor = true;
            btn.Appearance.Options.UseForeColor = true;
            btn.AppearanceHovered.BackColor = Color.FromArgb(99, 102, 241);
            btn.AppearanceHovered.ForeColor = Color.White;
            btn.AppearanceHovered.Options.UseBackColor = true;
            btn.AppearanceHovered.Options.UseForeColor = true;
            btn.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            btn.Cursor = Cursors.Hand;
            btn.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            
            if (onClick != null)
            {
                btn.Click += (s, e) => onClick();
            }
            
            this.pnlSidebar.Controls.Add(btn);
        }

        private void ShowWelcomePanel()
        {
            // Clear content
            this.pnlContent.Controls.Clear();

            // Welcome Title
            var lblWelcome = new LabelControl();
            lblWelcome.Text = "Hoş Geldiniz!";
            lblWelcome.Appearance.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblWelcome.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblWelcome.Appearance.Options.UseFont = true;
            lblWelcome.Appearance.Options.UseForeColor = true;
            lblWelcome.Location = new Point(30, 30);
            this.pnlContent.Controls.Add(lblWelcome);

            // Subtitle
            var lblSubtitle = new LabelControl();
            lblSubtitle.Text = "Apartman yönetim panelinize hoş geldiniz. Sol menüden işlemlerinizi gerçekleştirebilirsiniz.";
            lblSubtitle.Appearance.Font = new Font("Segoe UI", 11F);
            lblSubtitle.Appearance.ForeColor = Color.Gray;
            lblSubtitle.Appearance.Options.UseFont = true;
            lblSubtitle.Appearance.Options.UseForeColor = true;
            lblSubtitle.Location = new Point(30, 75);
            this.pnlContent.Controls.Add(lblSubtitle);

            // Summary Cards Container
            var pnlCards = new FlowLayoutPanel();
            pnlCards.Location = new Point(30, 130);
            pnlCards.Size = new Size(900, 200);
            pnlCards.FlowDirection = FlowDirection.LeftToRight;
            pnlCards.WrapContents = false;
            this.pnlContent.Controls.Add(pnlCards);

            // Create summary cards
            CreateSummaryCard(pnlCards, "🏗️", "Siteler", "Toplam site sayısı", Color.FromArgb(99, 102, 241));
            CreateSummaryCard(pnlCards, "🏢", "Bloklar", "Toplam blok sayısı", Color.FromArgb(16, 185, 129));
            CreateSummaryCard(pnlCards, "🚪", "Daireler", "Toplam daire sayısı", Color.FromArgb(245, 158, 11));
            CreateSummaryCard(pnlCards, "👥", "Sakinler", "Toplam sakin sayısı", Color.FromArgb(236, 72, 153));
        }

        private void CreateSummaryCard(FlowLayoutPanel container, string icon, string title, string subtitle, Color accentColor)
        {
            var card = new Panel();
            card.Size = new Size(200, 150);
            card.Margin = new Padding(0, 0, 20, 0);
            card.BackColor = Color.White;
            card.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                // Top accent bar
                using (var brush = new SolidBrush(accentColor))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, card.Width, 5);
                }
            };

            var lblIcon = new LabelControl();
            lblIcon.Text = icon;
            lblIcon.Appearance.Font = new Font("Segoe UI Emoji", 32F);
            lblIcon.Appearance.Options.UseFont = true;
            lblIcon.Location = new Point(20, 25);
            card.Controls.Add(lblIcon);

            var lblTitle = new LabelControl();
            lblTitle.Text = title;
            lblTitle.Appearance.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Appearance.Options.UseFont = true;
            lblTitle.Appearance.Options.UseForeColor = true;
            lblTitle.Location = new Point(20, 85);
            card.Controls.Add(lblTitle);

            var lblSubtitle = new LabelControl();
            lblSubtitle.Text = subtitle;
            lblSubtitle.Appearance.Font = new Font("Segoe UI", 9F);
            lblSubtitle.Appearance.ForeColor = Color.Gray;
            lblSubtitle.Appearance.Options.UseFont = true;
            lblSubtitle.Appearance.Options.UseForeColor = true;
            lblSubtitle.Location = new Point(20, 115);
            card.Controls.Add(lblSubtitle);

            container.Controls.Add(card);
        }

        private void LoadForm(Control form)
        {
            // Clear content panel
            this.pnlContent.Controls.Clear();

            if (form is Form f)
            {
                f.TopLevel = false;
                f.FormBorderStyle = FormBorderStyle.None;
                f.Dock = DockStyle.Fill;
                this.pnlContent.Controls.Add(f);
                f.Show();
            }
            else
            {
                form.Dock = DockStyle.Fill;
                this.pnlContent.Controls.Add(form);
            }
        }
    }
}
