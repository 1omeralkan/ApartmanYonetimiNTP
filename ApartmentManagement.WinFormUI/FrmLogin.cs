    #nullable disable
// FrmLogin.cs
// Giriş Formu - Kullanıcı kimlik doğrulama işlemlerini yönetir
// Standart: Tahoma 8.25pt, AutoScroll = true
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Helpers;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Kullanıcı giriş formu
    /// </summary>
    public partial class FrmLogin : DevExpress.XtraEditors.XtraForm
    {
        private readonly IAuth _authService;
        private TextEdit txtEmail;
        private TextEdit txtPassword;
        private CheckEdit chkRememberMe;
        private SimpleButton btnLogin;

        /// <summary>
        /// FrmLogin constructor - Formu başlatır
        /// </summary>
        public FrmLogin()
        {
            InitializeComponent();
            _authService = new SAuth();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        /// <summary>
        /// Form bileşenlerini başlatır
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings - Standart: Max 770x700, AutoScroll = true
            this.ClientSize = new Size(450, 450);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = "Giriş Yap";
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            int fieldWidth = 380;
            int startX = 35;
            int currentY = 30;

            // Title
            var lblTitle = new LabelControl();
            lblTitle.Text = "🔓 Giriş Yap";
            lblTitle.Appearance.Font = new Font("Tahoma", 14F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(startX, currentY);
            this.Controls.Add(lblTitle);

            // Ana Sayfa Button
            var btnHome = new SimpleButton();
            btnHome.Text = "🏠 Ana Sayfa";
            btnHome.Size = new Size(100, 28);
            btnHome.Location = new Point(310, currentY);
            btnHome.Appearance.Font = new Font("Tahoma", 8.25F);
            btnHome.Appearance.BackColor = Color.FromArgb(240, 240, 240);
            btnHome.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            btnHome.Appearance.Options.UseBackColor = true;
            btnHome.Appearance.Options.UseForeColor = true;
            btnHome.Cursor = Cursors.Hand;
            btnHome.Click += (s, e) => Application.Exit();
            this.Controls.Add(btnHome);

            currentY += 60;

            // Email
            var lblEmail = new LabelControl();
            lblEmail.Text = "Email";
            lblEmail.Appearance.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            lblEmail.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            lblEmail.Location = new Point(startX, currentY);
            this.Controls.Add(lblEmail);
            currentY += 22;

            this.txtEmail = new TextEdit();
            this.txtEmail.EditValue = "admin@gmail.com";
            this.txtEmail.Location = new Point(startX, currentY);
            this.txtEmail.Size = new Size(fieldWidth, 28);
            this.txtEmail.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            this.txtEmail.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.Controls.Add(this.txtEmail);
            currentY += 40;

            // Şifre
            var lblPassword = new LabelControl();
            lblPassword.Text = "Şifre";
            lblPassword.Appearance.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            lblPassword.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            lblPassword.Location = new Point(startX, currentY);
            this.Controls.Add(lblPassword);
            currentY += 22;

            this.txtPassword = new TextEdit();
            this.txtPassword.EditValue = "123";
            this.txtPassword.Location = new Point(startX, currentY);
            this.txtPassword.Size = new Size(fieldWidth, 28);
            this.txtPassword.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            this.txtPassword.Properties.PasswordChar = '●';
            this.txtPassword.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.Controls.Add(this.txtPassword);

            // Şifre göster ikonu
            var lblShowPassword = new LabelControl();
            lblShowPassword.Text = "👁";
            lblShowPassword.Appearance.Font = new Font("Tahoma", 8.25F);
            lblShowPassword.Appearance.ForeColor = Color.Gray;
            lblShowPassword.Cursor = Cursors.Hand;
            lblShowPassword.Size = new Size(25, 20);
            lblShowPassword.Location = new Point(startX + fieldWidth - 30, currentY + 4);
            bool isPasswordVisible = false;
            lblShowPassword.Click += (s, e) =>
            {
                isPasswordVisible = !isPasswordVisible;
                txtPassword.Properties.PasswordChar = isPasswordVisible ? '\0' : '●';
                lblShowPassword.Appearance.ForeColor = isPasswordVisible ? Color.FromArgb(99, 102, 241) : Color.Gray;
            };
            this.Controls.Add(lblShowPassword);
            lblShowPassword.BringToFront();
            currentY += 40;

            // Beni hatırla checkbox
            this.chkRememberMe = new CheckEdit();
            this.chkRememberMe.Text = "Beni hatırla";
            this.chkRememberMe.Location = new Point(startX, currentY);
            this.chkRememberMe.Size = new Size(120, 20);
            this.chkRememberMe.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            this.Controls.Add(this.chkRememberMe);

            // Şifremi unuttum link
            var lblForgot = new LabelControl();
            lblForgot.Text = "Şifremi unuttum";
            lblForgot.Appearance.Font = new Font("Tahoma", 8.25F);
            lblForgot.Appearance.ForeColor = Color.FromArgb(99, 102, 241);
            lblForgot.Cursor = Cursors.Hand;
            lblForgot.Location = new Point(310, currentY + 2);
            lblForgot.Click += (s, e) => Swal.Info("Şifre sıfırlama özelliği yakında eklenecek.");
            this.Controls.Add(lblForgot);
            currentY += 40;

            // Giriş Yap Button
            this.btnLogin = new SimpleButton();
            this.btnLogin.Text = "Giriş Yap";
            this.btnLogin.Location = new Point(startX, currentY);
            this.btnLogin.Size = new Size(fieldWidth, 35);
            this.btnLogin.Appearance.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            this.btnLogin.Appearance.BackColor = Color.FromArgb(66, 133, 244);
            this.btnLogin.Appearance.ForeColor = Color.White;
            this.btnLogin.Appearance.Options.UseBackColor = true;
            this.btnLogin.Appearance.Options.UseForeColor = true;
            this.btnLogin.Cursor = Cursors.Hand;
            this.btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(this.btnLogin);
            currentY += 45;

            // Separator "veya"
            var lblSeperator = new LabelControl();
            lblSeperator.Text = "────────────── veya ──────────────";
            lblSeperator.Appearance.Font = new Font("Tahoma", 8.25F);
            lblSeperator.Appearance.ForeColor = Color.Gray;
            lblSeperator.AutoSizeMode = LabelAutoSizeMode.None;
            lblSeperator.Size = new Size(fieldWidth, 18);
            lblSeperator.Location = new Point(startX, currentY);
            lblSeperator.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Controls.Add(lblSeperator);
            currentY += 25;

            // Kayıt ol link
            var lblRegister = new LabelControl();
            lblRegister.Text = "Hesabın yok mu? Kayıt ol";
            lblRegister.Appearance.Font = new Font("Tahoma", 8.25F);
            lblRegister.Appearance.ForeColor = Color.FromArgb(99, 102, 241);
            lblRegister.Cursor = Cursors.Hand;
            lblRegister.AutoSizeMode = LabelAutoSizeMode.None;
            lblRegister.Size = new Size(fieldWidth, 20);
            lblRegister.Location = new Point(startX, currentY);
            lblRegister.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lblRegister.Click += (s, e) => { this.Hide(); new FrmRegister().Show(); };
            this.Controls.Add(lblRegister);
            currentY += 30;

            // Copyright
            var lblCopyright = new LabelControl();
            lblCopyright.Text = "© 2025 Apartman Yönetim Sistemi";
            lblCopyright.Appearance.Font = new Font("Tahoma", 8.25F);
            lblCopyright.Appearance.ForeColor = Color.Gray;
            lblCopyright.AutoSizeMode = LabelAutoSizeMode.None;
            lblCopyright.Size = new Size(fieldWidth, 18);
            lblCopyright.Location = new Point(startX, currentY);
            lblCopyright.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Controls.Add(lblCopyright);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Giriş butonuna tıklandığında çalışır
        /// </summary>
        /// <param name="sender">Gönderen nesne</param>
        /// <param name="e">Event argümanları</param>
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
                Logger.Log("INFO", "LOGIN_SUCCESS", $"Giriş yapıldı: {email}", user.Id);
                this.Hide();
                // Tüm roller için yeni layout'a yönlendir
                new FrmMainLayout(user).Show();
            }
            else
            {
                // Check if user exists but not approved
                try
                {
                    var userService = new ApartmentManagement.Business.Services.SUser();
                    var existingUser = userService.GetAll().FirstOrDefault(u => u.Email == email);
                    if (existingUser != null && !existingUser.IsApproved)
                    {
                        Swal.Warning("Hesabınız henüz admin tarafından onaylanmadı. Lütfen onay bekleyin.", "Onay Bekleniyor");
                        Logger.Log("WARNING", "LOGIN_PENDING_APPROVAL", $"Onay bekleyen hesapla giriş denemesi: {email}", existingUser.Id);
                    }
                    else
                    {
                        Swal.Error("E-posta veya şifre hatalı!", "Giriş Hatası");
                        Logger.Log("WARNING", "LOGIN_FAILED", $"Başarısız giriş denemesi: {email}");
                    }
                }
                catch
                {
                    Swal.Error("E-posta veya şifre hatalı!", "Giriş Hatası");
                    Logger.Log("ERROR", "LOGIN_FAILED", $"Başarısız giriş denemesi (exception): {email}");
                }
            }
        }
    }
}
