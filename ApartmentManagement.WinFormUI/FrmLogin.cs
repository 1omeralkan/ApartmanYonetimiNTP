    #nullable disable
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    public partial class FrmLogin : DevExpress.XtraEditors.XtraForm
    {
        private readonly IAuth _authService;
        private TextEdit txtEmail;
        private TextEdit txtPassword;
        private CheckEdit chkRememberMe;
        private SimpleButton btnLogin;

        public FrmLogin()
        {
            InitializeComponent();
            _authService = new SAuth();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.ClientSize = new Size(450, 450);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = "Giriş Yap";
            this.BackColor = Color.FromArgb(248, 249, 250);

            int fieldWidth = 380;
            int startX = 35;
            int currentY = 30;

            // Title
            var lblTitle = new LabelControl();
            lblTitle.Text = "🔓 Giriş Yap";
            lblTitle.Appearance.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(startX, currentY);
            this.Controls.Add(lblTitle);

            // Ana Sayfa Button
            var btnHome = new SimpleButton();
            btnHome.Text = "🏠 Ana Sayfa";
            btnHome.Size = new Size(100, 32);
            btnHome.Location = new Point(310, currentY);
            btnHome.Appearance.Font = new Font("Segoe UI", 9F);
            btnHome.Appearance.BackColor = Color.FromArgb(240, 240, 240);
            btnHome.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            btnHome.Appearance.Options.UseBackColor = true;
            btnHome.Appearance.Options.UseForeColor = true;
            btnHome.Cursor = Cursors.Hand;
            btnHome.Click += (s, e) => Application.Exit();
            this.Controls.Add(btnHome);

            currentY += 70;

            // Email
            var lblEmail = new LabelControl();
            lblEmail.Text = "Email";
            lblEmail.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEmail.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            lblEmail.Location = new Point(startX, currentY);
            this.Controls.Add(lblEmail);
            currentY += 25;

            this.txtEmail = new TextEdit();
            this.txtEmail.EditValue = "admin@gmail.com";
            this.txtEmail.Location = new Point(startX, currentY);
            this.txtEmail.Size = new Size(fieldWidth, 38);
            this.txtEmail.Properties.Appearance.Font = new Font("Segoe UI", 12F);
            this.txtEmail.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.Controls.Add(this.txtEmail);
            currentY += 55;

            // Şifre
            var lblPassword = new LabelControl();
            lblPassword.Text = "Şifre";
            lblPassword.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPassword.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            lblPassword.Location = new Point(startX, currentY);
            this.Controls.Add(lblPassword);
            currentY += 25;

            this.txtPassword = new TextEdit();
            this.txtPassword.EditValue = "123";
            this.txtPassword.Location = new Point(startX, currentY);
            this.txtPassword.Size = new Size(fieldWidth, 38);
            this.txtPassword.Properties.Appearance.Font = new Font("Segoe UI", 12F);
            this.txtPassword.Properties.PasswordChar = '●';
            this.txtPassword.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.Controls.Add(this.txtPassword);

            // Şifre göster ikonu (küçük label)
            var lblShowPassword = new LabelControl();
            lblShowPassword.Text = "👁";
            lblShowPassword.Appearance.Font = new Font("Segoe UI", 10F);
            lblShowPassword.Appearance.ForeColor = Color.Gray;
            lblShowPassword.Cursor = Cursors.Hand;
            lblShowPassword.Size = new Size(25, 20);
            lblShowPassword.Location = new Point(startX + fieldWidth - 30, currentY + 9);
            bool isPasswordVisible = false;
            lblShowPassword.Click += (s, e) =>
            {
                isPasswordVisible = !isPasswordVisible;
                txtPassword.Properties.PasswordChar = isPasswordVisible ? '\0' : '●';
                lblShowPassword.Appearance.ForeColor = isPasswordVisible ? Color.FromArgb(99, 102, 241) : Color.Gray;
            };
            this.Controls.Add(lblShowPassword);
            lblShowPassword.BringToFront();
            currentY += 50;

            // Beni hatırla checkbox
            this.chkRememberMe = new CheckEdit();
            this.chkRememberMe.Text = "Beni hatırla";
            this.chkRememberMe.Location = new Point(startX, currentY);
            this.chkRememberMe.Size = new Size(120, 22);
            this.chkRememberMe.Properties.Appearance.Font = new Font("Segoe UI", 9F);
            this.Controls.Add(this.chkRememberMe);

            // Şifremi unuttum link
            var lblForgot = new LabelControl();
            lblForgot.Text = "Şifremi unuttum";
            lblForgot.Appearance.Font = new Font("Segoe UI", 9F);
            lblForgot.Appearance.ForeColor = Color.FromArgb(99, 102, 241);
            lblForgot.Cursor = Cursors.Hand;
            lblForgot.Location = new Point(310, currentY + 3);
            lblForgot.Click += (s, e) => Swal.Info("Şifre sıfırlama özelliği yakında eklenecek.");
            this.Controls.Add(lblForgot);
            currentY += 50;

            // Giriş Yap Button
            this.btnLogin = new SimpleButton();
            this.btnLogin.Text = "Giriş Yap";
            this.btnLogin.Location = new Point(startX, currentY);
            this.btnLogin.Size = new Size(fieldWidth, 45);
            this.btnLogin.Appearance.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.btnLogin.Appearance.BackColor = Color.FromArgb(66, 133, 244);
            this.btnLogin.Appearance.ForeColor = Color.White;
            this.btnLogin.Appearance.Options.UseBackColor = true;
            this.btnLogin.Appearance.Options.UseForeColor = true;
            this.btnLogin.Cursor = Cursors.Hand;
            this.btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(this.btnLogin);
            currentY += 55;

            // Separator "veya"
            var lblSeperator = new LabelControl();
            lblSeperator.Text = "────────────── veya ──────────────";
            lblSeperator.Appearance.Font = new Font("Segoe UI", 9F);
            lblSeperator.Appearance.ForeColor = Color.Gray;
            lblSeperator.AutoSizeMode = LabelAutoSizeMode.None;
            lblSeperator.Size = new Size(fieldWidth, 20);
            lblSeperator.Location = new Point(startX, currentY);
            lblSeperator.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Controls.Add(lblSeperator);
            currentY += 30;

            // Kayıt ol link
            var lblRegister = new LabelControl();
            lblRegister.Text = "Hesabın yok mu? Kayıt ol";
            lblRegister.Appearance.Font = new Font("Segoe UI", 10F);
            lblRegister.Appearance.ForeColor = Color.FromArgb(99, 102, 241);
            lblRegister.Cursor = Cursors.Hand;
            lblRegister.AutoSizeMode = LabelAutoSizeMode.None;
            lblRegister.Size = new Size(fieldWidth, 25);
            lblRegister.Location = new Point(startX, currentY);
            lblRegister.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lblRegister.Click += (s, e) => { this.Hide(); new FrmRegister().Show(); };
            this.Controls.Add(lblRegister);
            currentY += 35;

            // Copyright
            var lblCopyright = new LabelControl();
            lblCopyright.Text = "© 2025 Apartman Yönetim Sistemi";
            lblCopyright.Appearance.Font = new Font("Segoe UI", 8F);
            lblCopyright.Appearance.ForeColor = Color.Gray;
            lblCopyright.AutoSizeMode = LabelAutoSizeMode.None;
            lblCopyright.Size = new Size(fieldWidth, 20);
            lblCopyright.Location = new Point(startX, currentY);
            lblCopyright.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Controls.Add(lblCopyright);

            this.ResumeLayout(false);
        }

        private void BtnLogin_Click(object sender, System.EventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Swal.Warning("Lütfen email ve şifre girin.");
                return;
            }

            var user = _authService.Login(email, password);

            if (user != null)
            {
                this.Hide();
                // Role-based redirect
                if (user.Role == "SuperAdmin" || user.Role == "Admin" || 
                    user.Role == "SiteManager" || user.Role == "ApartmentManager")
                {
                    new FrmAdminDashboard().Show();
                }
                else // Resident
                {
                    Swal.Info("Sakin paneli henüz yapılmadı. Yakında gelecek!");
                    this.Show();
                }
            }
            else
            {
                Swal.Error("E-posta veya şifre hatalı!", "Giriş Hatası");
            }
        }
    }
}
