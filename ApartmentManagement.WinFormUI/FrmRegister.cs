#nullable disable
// FrmRegister.cs
// Kayıt Formu - Yeni kullanıcı kayıt işlemlerini yönetir
// Standart: Tahoma 8.25pt, AutoScroll = true
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Kullanıcı kayıt formu
    /// </summary>
    public partial class FrmRegister : DevExpress.XtraEditors.XtraForm
    {
        private readonly IAuth _authService;
        private Panel pnlMain;
        private bool _isAdminMode;
        
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

        /// <summary>
        /// FrmRegister constructor - Formu başlatır
        /// </summary>
        /// <param name="isAdminMode">Admin panelinden mi açılıyor?</param>
        public FrmRegister(bool isAdminMode = false)
        {
            _isAdminMode = isAdminMode;
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
            this.ClientSize = new Size(700, 700);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = _isAdminMode ? "Yeni Kullanıcı Ekle" : "Kayıt Ol";
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

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
            lblTitle.Text = _isAdminMode ? "👤 Yeni Kullanıcı Ekle" : "👤 Kayıt Ol";
            lblTitle.Appearance.Font = new Font("Tahoma", 14F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftX, currentY);
            this.pnlMain.Controls.Add(lblTitle);

            // Ana Sayfa Link (Sadece Normal Modda)
            if (!_isAdminMode)
            {
                var lblHome = new LabelControl();
                lblHome.Text = "🏠 Ana Sayfa";
                lblHome.Appearance.Font = new Font("Tahoma", 8.25F);
                lblHome.Appearance.ForeColor = Color.FromArgb(99, 102, 241);
                lblHome.Cursor = Cursors.Hand;
                lblHome.Location = new Point(550, currentY + 5);
                lblHome.Click += (s, e) => { this.Hide(); new FrmLogin().Show(); };
                this.pnlMain.Controls.Add(lblHome);
            }

            currentY += 50;

            // ========== KİŞİSEL BİLGİLER ==========
            AddSectionHeader("👤 Kişisel Bilgiler", leftX, currentY);
            currentY += 28;

            // Ad - Soyad
            AddFieldLabel("Ad *", leftX, currentY);
            AddFieldLabel("Soyad *", rightX, currentY);
            currentY += 18;
            this.txtFirstName = AddTextEdit(leftX, currentY, fieldWidth);
            this.txtLastName = AddTextEdit(rightX, currentY, fieldWidth);
            currentY += 35;

            // TC Kimlik No - Cinsiyet
            AddFieldLabel("TC Kimlik No", leftX, currentY);
            AddFieldLabel("Cinsiyet", rightX, currentY);
            currentY += 18;
            this.txtTcNo = AddTextEdit(leftX, currentY, fieldWidth);
            this.cmbGender = new ComboBoxEdit();
            this.cmbGender.Location = new Point(rightX, currentY);
            this.cmbGender.Size = new Size(fieldWidth, 24);
            this.cmbGender.Properties.Items.AddRange(new string[] { "Seçiniz", "Erkek", "Kadın", "Diğer" });
            this.cmbGender.SelectedIndex = 0;
            this.cmbGender.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbGender.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            this.pnlMain.Controls.Add(this.cmbGender);
            currentY += 35;

            // Doğum Tarihi
            AddFieldLabel("Doğum Tarihi", leftX, currentY);
            currentY += 18;
            this.dtBirthDate = new DateEdit();
            this.dtBirthDate.Location = new Point(leftX, currentY);
            this.dtBirthDate.Size = new Size(fieldWidth, 24);
            this.dtBirthDate.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            this.dtBirthDate.Properties.DisplayFormat.FormatString = "dd.MM.yyyy";
            this.dtBirthDate.Properties.EditFormat.FormatString = "dd.MM.yyyy";
            this.dtBirthDate.Properties.Mask.EditMask = "dd.MM.yyyy";
            this.pnlMain.Controls.Add(this.dtBirthDate);
            currentY += 45;

            // ========== İLETİŞİM BİLGİLERİ ==========
            AddSectionHeader("📞 İletişim Bilgileri", leftX, currentY);
            currentY += 28;

            // Email
            AddFieldLabel("Email *", leftX, currentY);
            currentY += 18;
            this.txtEmail = AddTextEdit(leftX, currentY, 630);
            currentY += 35;

            // Telefon
            AddFieldLabel("Telefon", leftX, currentY);
            currentY += 18;
            this.txtPhone = AddTextEdit(leftX, currentY, 630);
            currentY += 35;

            // Adres
            AddFieldLabel("Adres", leftX, currentY);
            currentY += 18;
            this.txtAddress = new MemoEdit();
            this.txtAddress.Location = new Point(leftX, currentY);
            this.txtAddress.Size = new Size(630, 50);
            this.txtAddress.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            this.pnlMain.Controls.Add(this.txtAddress);
            currentY += 60;

            // ========== ACİL DURUM İLETİŞİM ==========
            AddSectionHeader("🆘 Acil Durum İletişim", leftX, currentY);
            currentY += 28;

            // Acil Durum Kişi - Telefon
            AddFieldLabel("Acil Durum Kişi", leftX, currentY);
            AddFieldLabel("Acil Durum Telefon", rightX, currentY);
            currentY += 18;
            this.txtEmergencyContact = AddTextEdit(leftX, currentY, fieldWidth);
            this.txtEmergencyPhone = AddTextEdit(rightX, currentY, fieldWidth);
            currentY += 45;

            // ========== GÜVENLİK ==========
            AddSectionHeader("🔒 Güvenlik", leftX, currentY);
            currentY += 28;

            // Şifre - Şifre Tekrar
            AddFieldLabel("Şifre *", leftX, currentY);
            AddFieldLabel("Şifre (Tekrar) *", rightX, currentY);
            currentY += 18;
            this.txtPassword = AddTextEdit(leftX, currentY, fieldWidth, true);
            this.txtPasswordConfirm = AddTextEdit(rightX, currentY, fieldWidth, true);
            currentY += 45;

            // Kayıt Ol Button
            this.btnRegister = new SimpleButton();
            this.btnRegister.Text = _isAdminMode ? "Kullanıcıyı Kaydet" : "Kayıt Ol";
            this.btnRegister.Size = new Size(630, 32);
            this.btnRegister.Location = new Point(leftX, currentY);
            this.btnRegister.Appearance.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            this.btnRegister.Appearance.BackColor = Color.FromArgb(66, 133, 244);
            this.btnRegister.Appearance.ForeColor = Color.White;
            this.btnRegister.Appearance.Options.UseBackColor = true;
            this.btnRegister.Appearance.Options.UseForeColor = true;
            this.btnRegister.Cursor = Cursors.Hand;
            this.btnRegister.Click += BtnRegister_Click;
            this.pnlMain.Controls.Add(this.btnRegister);
            currentY += 42;

            // Separator (Sadece Normal)
            if (!_isAdminMode)
            {
                var lblSeperator = new LabelControl();
                lblSeperator.Text = "───────────────────── veya ─────────────────────";
                lblSeperator.Appearance.Font = new Font("Tahoma", 8.25F);
                lblSeperator.Appearance.ForeColor = Color.Gray;
                lblSeperator.AutoSizeMode = LabelAutoSizeMode.None;
                lblSeperator.Size = new Size(630, 18);
                lblSeperator.Location = new Point(leftX, currentY);
                lblSeperator.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                this.pnlMain.Controls.Add(lblSeperator);
                currentY += 25;

                // Login Link
                var lblLogin = new LabelControl();
                lblLogin.Text = "Zaten hesabın var mı? Giriş yap";
                lblLogin.Appearance.Font = new Font("Tahoma", 8.25F);
                lblLogin.Appearance.ForeColor = Color.FromArgb(99, 102, 241);
                lblLogin.Cursor = Cursors.Hand;
                lblLogin.AutoSizeMode = LabelAutoSizeMode.None;
                lblLogin.Size = new Size(630, 18);
                lblLogin.Location = new Point(leftX, currentY);
                lblLogin.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                lblLogin.Click += (s, e) => { this.Hide(); new FrmLogin().Show(); };
                this.pnlMain.Controls.Add(lblLogin);
            }

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Bölüm başlığı ekler
        /// </summary>
        /// <param name="text">Başlık metni</param>
        /// <param name="x">X konumu</param>
        /// <param name="y">Y konumu</param>
        private void AddSectionHeader(string text, int x, int y)
        {
            var lbl = new LabelControl();
            lbl.Text = text;
            lbl.Appearance.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            lbl.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lbl.Location = new Point(x, y);
            this.pnlMain.Controls.Add(lbl);
        }

        /// <summary>
        /// Alan etiketi ekler
        /// </summary>
        /// <param name="text">Etiket metni</param>
        /// <param name="x">X konumu</param>
        /// <param name="y">Y konumu</param>
        private void AddFieldLabel(string text, int x, int y)
        {
            var lbl = new LabelControl();
            lbl.Text = text;
            lbl.Appearance.Font = new Font("Tahoma", 8.25F);
            lbl.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            if (text.Contains("*"))
            {
                lbl.Appearance.ForeColor = Color.FromArgb(220, 53, 69);
            }
            lbl.Location = new Point(x, y);
            this.pnlMain.Controls.Add(lbl);
        }

        /// <summary>
        /// TextEdit kontrolü ekler
        /// </summary>
        /// <param name="x">X konumu</param>
        /// <param name="y">Y konumu</param>
        /// <param name="width">Genişlik</param>
        /// <param name="isPassword">Şifre alanı mı</param>
        /// <returns>Oluşturulan TextEdit</returns>
        private TextEdit AddTextEdit(int x, int y, int width, bool isPassword = false)
        {
            var txt = new TextEdit();
            txt.Location = new Point(x, y);
            txt.Size = new Size(width, 24);
            txt.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            txt.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            if (isPassword)
            {
                txt.Properties.PasswordChar = '●';
            }
            this.pnlMain.Controls.Add(txt);
            return txt;
        }

        /// <summary>
        /// Kayıt butonuna tıklandığında çalışır
        /// </summary>
        /// <param name="sender">Gönderen nesne</param>
        /// <param name="e">Event argümanları</param>
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
                    if (_isAdminMode)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Kullanıcı başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Clear fields or just close? Usually close for dashboard action
                        // this.Close() might close the whole app if it's the main form? No, it's inside pnlContent.
                        // But wait, ShowContent clears pnlContent. 
                        // If I close 'this', pnlContent will be empty. 
                        // Better to just clear fields for new entry, or maybe show a list?
                        // Let's reload content? 
                        // For now, let's just clear fields to allow adding another one.
                        txtFirstName.Text = "";
                        txtLastName.Text = "";
                        txtEmail.Text = "";
                        txtPhone.Text = "";
                        txtTcNo.Text = "";
                        txtAddress.Text = "";
                        txtEmergencyContact.Text = "";
                        txtEmergencyPhone.Text = "";
                        txtPassword.Text = "";
                        txtPasswordConfirm.Text = "";
                        cmbGender.SelectedIndex = 0;
                        dtBirthDate.EditValue = null;
                    }
                    else
                    {
                        Helpers.Swal.Success("Kayıt başarılı! Giriş yapabilirsiniz.");
                        this.Hide();
                        new FrmLogin().Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Swal.Error(ex.Message);
            }
        }
    }
}
