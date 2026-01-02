#nullable disable
// FrmAdminDashboard.cs
// Yönetici Paneli Formu - Ana yönetim ekranı
// Standart: Tahoma 8.25pt, AutoScroll = true
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Yönetici paneli formu
    /// </summary>
    public partial class FrmAdminDashboard : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// FrmAdminDashboard constructor - Formu başlatır
        /// </summary>
        public FrmAdminDashboard()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Form bileşenlerini başlatır
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings - Standart: Max 770x700, AutoScroll = true
            this.Text = "Yönetici Paneli";
            this.ClientSize = new Size(770, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            int startX = 30;
            int currentY = 30;
            int btnWidth = 200;
            int btnHeight = 40;
            int spacing = 15;

            // Title
            var lblTitle = new LabelControl();
            lblTitle.Text = "🏠 Yönetici Paneli";
            lblTitle.Appearance.Font = new Font("Tahoma", 14F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(startX, currentY);
            this.Controls.Add(lblTitle);

            // Logout Button
            var btnLogout = new SimpleButton();
            btnLogout.Text = "Çıkış Yap";
            btnLogout.Size = new Size(80, 28);
            btnLogout.Location = new Point(660, currentY);
            btnLogout.Appearance.Font = new Font("Tahoma", 8.25F);
            btnLogout.Appearance.BackColor = Color.FromArgb(220, 53, 69);
            btnLogout.Appearance.ForeColor = Color.White;
            btnLogout.Appearance.Options.UseBackColor = true;
            btnLogout.Appearance.Options.UseForeColor = true;
            btnLogout.Click += (s, e) => { this.Hide(); new FrmLogin().Show(); };
            this.Controls.Add(btnLogout);

            currentY += 60;

            // Info Label
            var lblInfo = new LabelControl();
            lblInfo.Text = "Yönetmek istediğiniz modülü seçin:";
            lblInfo.Appearance.Font = new Font("Tahoma", 8.25F);
            lblInfo.Appearance.ForeColor = Color.Gray;
            lblInfo.Location = new Point(startX, currentY);
            this.Controls.Add(lblInfo);

            currentY += 30;

            // Buttons Panel
            int col = 0;
            int row = 0;
            int colWidth = btnWidth + spacing;
            int rowHeight = btnHeight + spacing;

            // Kullanıcılar Button
            var btnUsers = CreateMenuButton("👥 Kullanıcılar", startX + col * colWidth, currentY + row * rowHeight, btnWidth, btnHeight);
            btnUsers.Click += (s, e) => { new FrmUserList().ShowDialog(); };
            this.Controls.Add(btnUsers);
            col++;

            // Daireler Button
            var btnFlats = CreateMenuButton("🚪 Daireler", startX + col * colWidth, currentY + row * rowHeight, btnWidth, btnHeight);
            btnFlats.Click += (s, e) => { new FrmFlatList().ShowDialog(); };
            this.Controls.Add(btnFlats);
            col++;

            // Apartmanlar Button
            var btnApartments = CreateMenuButton("🏢 Apartmanlar", startX + col * colWidth, currentY + row * rowHeight, btnWidth, btnHeight);
            btnApartments.Click += (s, e) => { new FrmApartmentList().ShowDialog(); };
            this.Controls.Add(btnApartments);
            col = 0;
            row++;

            // Bloklar Button
            var btnBlocks = CreateMenuButton("🏗 Bloklar", startX + col * colWidth, currentY + row * rowHeight, btnWidth, btnHeight);
            btnBlocks.Click += (s, e) => { new FrmBlockManagement().ShowDialog(); };
            this.Controls.Add(btnBlocks);
            col++;

            // Daire Yönetimi Button
            var btnFlatMgmt = CreateMenuButton("🛠 Daire Yönetimi", startX + col * colWidth, currentY + row * rowHeight, btnWidth, btnHeight);
            btnFlatMgmt.Click += (s, e) => { new FrmFlatManagement().ShowDialog(); };
            this.Controls.Add(btnFlatMgmt);
            col++;

            // Apartman Yönetimi Button
            var btnAptMgmt = CreateMenuButton("📋 Apartman Yönetimi", startX + col * colWidth, currentY + row * rowHeight, btnWidth, btnHeight);
            btnAptMgmt.Click += (s, e) => { new FrmApartmentManagement().ShowDialog(); };
            this.Controls.Add(btnAptMgmt);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Menü butonu oluşturur
        /// </summary>
        /// <param name="text">Buton metni</param>
        /// <param name="x">X konumu</param>
        /// <param name="y">Y konumu</param>
        /// <param name="width">Genişlik</param>
        /// <param name="height">Yükseklik</param>
        /// <returns>Oluşturulan buton</returns>
        private SimpleButton CreateMenuButton(string text, int x, int y, int width, int height)
        {
            var btn = new SimpleButton();
            btn.Text = text;
            btn.Size = new Size(width, height);
            btn.Location = new Point(x, y);
            btn.Appearance.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            btn.Appearance.BackColor = Color.FromArgb(66, 133, 244);
            btn.Appearance.ForeColor = Color.White;
            btn.Appearance.Options.UseBackColor = true;
            btn.Appearance.Options.UseForeColor = true;
            btn.Cursor = Cursors.Hand;
            return btn;
        }
    }
}
