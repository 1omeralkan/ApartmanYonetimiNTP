#nullable disable
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    public partial class FrmRegister : DevExpress.XtraEditors.XtraForm
    {
        private readonly IAuth _authService;
        private Panel pnlMain;
        
        // Kişisel Bilgiler
        private TextEdit txtFirstName;
        private TextEdit txtLastName;
        private TextEdit txtTcNo;
        private ComboBoxEdit cmbGender;
        private DateEdit dtBirthDate;
        
        // İletişim Bilgileri
        private TextEdit txtEmail;
        private TextEdit txtPhone;
        private MemoEdit txtAddress;
        
        // Acil Durum İletişim
        private TextEdit txtEmergencyContact;
        private TextEdit txtEmergencyPhone;
        
        // Güvenlik
        private TextEdit txtPassword;
        private TextEdit txtPasswordConfirm;
        
        private SimpleButton btnRegister;

        public FrmRegister()
        {
            InitializeComponent();
            _authService = new SAuth();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.ClientSize = new Size(700, 750);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = "Kayıt Ol";
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;

            // Main Panel
            this.pnlMain = new Panel();
            this.pnlMain.Dock = DockStyle.Fill;
            this.pnlMain.AutoScroll = true;
            this.pnlMain.BackColor = Color.FromArgb(248, 249, 250);
            this.Controls.Add(this.pnlMain);

            int leftX = 30;
            int rightX = 360;
            int fieldWidth = 300;
            int currentY = 20;

            // Title
            var lblTitle = new LabelControl();
            lblTitle.Text = "👤 Kayıt Ol";
            lblTitle.Appearance.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftX, currentY);
            this.pnlMain.Controls.Add(lblTitle);

            // Ana Sayfa Link
            var lblHome = new LabelControl();
            lblHome.Text = "🏠 Ana Sayfa";
            lblHome.Appearance.Font = new Font("Segoe UI", 10F);
            lblHome.Appearance.ForeColor = Color.FromArgb(99, 102, 241);
            lblHome.Cursor = Cursors.Hand;
            lblHome.Location = new Point(550, currentY + 8);
            lblHome.Click += (s, e) => { this.Hide(); new FrmLogin().Show(); };
            this.pnlMain.Controls.Add(lblHome);

            currentY += 60;

            // ========== KİŞİSEL BİLGİLER ==========
            AddSectionHeader("👤 Kişisel Bilgiler", leftX, currentY);
            currentY += 35;

            // Ad - Soyad
            AddFieldLabel("Ad *", leftX, currentY);
            AddFieldLabel("Soyad *", rightX, currentY);
            currentY += 22;
            this.txtFirstName = AddTextEdit(leftX, currentY, fieldWidth);
            this.txtLastName = AddTextEdit(rightX, currentY, fieldWidth);
            currentY += 45;

            // TC Kimlik No - Cinsiyet
            AddFieldLabel("TC Kimlik No", leftX, currentY);
            AddFieldLabel("Cinsiyet", rightX, currentY);
            currentY += 22;
            this.txtTcNo = AddTextEdit(leftX, currentY, fieldWidth);
            this.cmbGender = new ComboBoxEdit();
            this.cmbGender.Location = new Point(rightX, currentY);
            this.cmbGender.Size = new Size(fieldWidth, 32);
            this.cmbGender.Properties.Items.AddRange(new string[] { "Seçiniz", "Erkek", "Kadın", "Diğer" });
            this.cmbGender.SelectedIndex = 0;
            this.cmbGender.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbGender.Properties.Appearance.Font = new Font("Segoe UI", 11F);
            this.pnlMain.Controls.Add(this.cmbGender);
            currentY += 45;

            // Doğum Tarihi
            AddFieldLabel("Doğum Tarihi", leftX, currentY);
            currentY += 22;
            this.dtBirthDate = new DateEdit();
            this.dtBirthDate.Location = new Point(leftX, currentY);
            this.dtBirthDate.Size = new Size(fieldWidth, 32);
            this.dtBirthDate.Properties.Appearance.Font = new Font("Segoe UI", 11F);
            this.dtBirthDate.Properties.DisplayFormat.FormatString = "dd.MM.yyyy";
            this.dtBirthDate.Properties.EditFormat.FormatString = "dd.MM.yyyy";
            this.dtBirthDate.Properties.Mask.EditMask = "dd.MM.yyyy";
            this.pnlMain.Controls.Add(this.dtBirthDate);
            currentY += 55;

            // ========== İLETİŞİM BİLGİLERİ ==========
            AddSectionHeader("📞 İletişim Bilgileri", leftX, currentY);
            currentY += 35;

            // Email
            AddFieldLabel("Email *", leftX, currentY);
            currentY += 22;
            this.txtEmail = AddTextEdit(leftX, currentY, 630);
            currentY += 45;

            // Telefon
            AddFieldLabel("Telefon", leftX, currentY);
            currentY += 22;
            this.txtPhone = AddTextEdit(leftX, currentY, 630);
            currentY += 45;

            // Adres
            AddFieldLabel("Adres", leftX, currentY);
            currentY += 22;
            this.txtAddress = new MemoEdit();
            this.txtAddress.Location = new Point(leftX, currentY);
            this.txtAddress.Size = new Size(630, 60);
            this.txtAddress.Properties.Appearance.Font = new Font("Segoe UI", 11F);
            this.pnlMain.Controls.Add(this.txtAddress);
            currentY += 75;

            // ========== ACİL DURUM İLETİŞİM ==========
            AddSectionHeader("🆘 Acil Durum İletişim", leftX, currentY);
            currentY += 35;

            // Acil Durum Kişi - Telefon
            AddFieldLabel("Acil Durum Kişi", leftX, currentY);
            AddFieldLabel("Acil Durum Telefon", rightX, currentY);
            currentY += 22;
            this.txtEmergencyContact = AddTextEdit(leftX, currentY, fieldWidth);
            this.txtEmergencyPhone = AddTextEdit(rightX, currentY, fieldWidth);
            currentY += 55;

            // ========== GÜVENLİK ==========
            AddSectionHeader("🔒 Güvenlik", leftX, currentY);
            currentY += 35;

            // Şifre - Şifre Tekrar
            AddFieldLabel("Şifre *", leftX, currentY);
            AddFieldLabel("Şifre (Tekrar) *", rightX, currentY);
            currentY += 22;
            this.txtPassword = AddTextEdit(leftX, currentY, fieldWidth, true);
            this.txtPasswordConfirm = AddTextEdit(rightX, currentY, fieldWidth, true);
            currentY += 55;

            // Kayıt Ol Button
            this.btnRegister = new SimpleButton();
            this.btnRegister.Text = "Kayıt Ol";
            this.btnRegister.Size = new Size(630, 45);
            this.btnRegister.Location = new Point(leftX, currentY);
            this.btnRegister.Appearance.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.btnRegister.Appearance.BackColor = Color.FromArgb(66, 133, 244);
            this.btnRegister.Appearance.ForeColor = Color.White;
            this.btnRegister.Appearance.Options.UseBackColor = true;
            this.btnRegister.Appearance.Options.UseForeColor = true;
            this.btnRegister.Cursor = Cursors.Hand;
            this.btnRegister.Click += BtnRegister_Click;
            this.pnlMain.Controls.Add(this.btnRegister);
            currentY += 55;

            // Separator
            var lblSeperator = new LabelControl();
            lblSeperator.Text = "───────────────────── veya ─────────────────────";
            lblSeperator.Appearance.Font = new Font("Segoe UI", 9F);
            lblSeperator.Appearance.ForeColor = Color.Gray;
            lblSeperator.AutoSizeMode = LabelAutoSizeMode.None;
            lblSeperator.Size = new Size(630, 20);
            lblSeperator.Location = new Point(leftX, currentY);
            lblSeperator.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.pnlMain.Controls.Add(lblSeperator);
            currentY += 30;

            // Login Link
            var lblLogin = new LabelControl();
            lblLogin.Text = "Zaten hesabın var mı? Giriş yap";
            lblLogin.Appearance.Font = new Font("Segoe UI", 10F);
            lblLogin.Appearance.ForeColor = Color.FromArgb(99, 102, 241);
            lblLogin.Cursor = Cursors.Hand;
            lblLogin.AutoSizeMode = LabelAutoSizeMode.None;
            lblLogin.Size = new Size(630, 25);
            lblLogin.Location = new Point(leftX, currentY);
            lblLogin.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lblLogin.Click += (s, e) => { this.Hide(); new FrmLogin().Show(); };
            this.pnlMain.Controls.Add(lblLogin);

            this.ResumeLayout(false);
        }

        private void AddSectionHeader(string text, int x, int y)
        {
            var lbl = new LabelControl();
            lbl.Text = text;
            lbl.Appearance.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lbl.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lbl.Location = new Point(x, y);
            this.pnlMain.Controls.Add(lbl);
        }

        private void AddFieldLabel(string text, int x, int y)
        {
            var lbl = new LabelControl();
            lbl.Text = text;
            lbl.Appearance.Font = new Font("Segoe UI", 9F);
            lbl.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            if (text.Contains("*"))
            {
                lbl.Appearance.ForeColor = Color.FromArgb(220, 53, 69);
            }
            lbl.Location = new Point(x, y);
            this.pnlMain.Controls.Add(lbl);
        }

        private TextEdit AddTextEdit(int x, int y, int width, bool isPassword = false)
        {
            var txt = new TextEdit();
            txt.Location = new Point(x, y);
            txt.Size = new Size(width, 32);
            txt.Properties.Appearance.Font = new Font("Segoe UI", 11F);
            txt.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            if (isPassword)
            {
                txt.Properties.PasswordChar = '●';
            }
            this.pnlMain.Controls.Add(txt);
            return txt;
        }

        private void BtnRegister_Click(object sender, System.EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                Swal.Warning("Lütfen zorunlu alanları doldurun (Ad, Soyad, Email, Şifre).");
                return;
            }

            if (!txtEmail.Text.Contains("@"))
            {
                Swal.Warning("Geçerli bir e-posta adresi girin.");
                return;
            }

            if (txtPassword.Text != txtPasswordConfirm.Text)
            {
                Swal.Error("Şifreler eşleşmiyor!");
                return;
            }

            if (txtPassword.Text.Length < 8)
            {
                Swal.Warning("Şifre en az 8 karakter olmalı.");
                return;
            }

            // Register
            try
            {
                var user = _authService.RegisterFull(
                    txtFirstName.Text.Trim(),
                    txtLastName.Text.Trim(),
                    txtTcNo.Text?.Trim(),
                    cmbGender.SelectedIndex > 0 ? cmbGender.EditValue?.ToString() : null,
                    dtBirthDate.EditValue as DateTime?,
                    txtEmail.Text.Trim(),
                    txtPhone.Text?.Trim(),
                    txtAddress.Text?.Trim(),
                    txtEmergencyContact.Text?.Trim(),
                    txtEmergencyPhone.Text?.Trim(),
                    txtPassword.Text
                );

                if (user != null)
                {
                    Swal.Success("Kayıt başarılı! Giriş yapabilirsiniz.");
                    this.Hide();
                    new FrmLogin().Show();
                }
            }
            catch (Exception ex)
            {
                Swal.Error(ex.Message);
            }
        }
    }
}
