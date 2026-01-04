#nullable disable
// FrmProfile.cs
// Profil Yönetim Formu - Kullanıcının kendi bilgilerini yönetmesi
// Standart: Tahoma 8.25pt, AutoScroll = true
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using ApartmentManagement.Business.Helpers;
using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Profil yönetim formu - Kullanıcının kendi bilgilerini yönetmesi
    /// </summary>
    public partial class FrmProfile : DevExpress.XtraEditors.XtraForm
    {
        private IUser _userService;
        private User _currentUser;

        // Controls - Kişisel Bilgiler
        private TextEdit txtFirstName;
        private TextEdit txtLastName;
        private TextEdit txtEmail;
        private TextEdit txtPhone;
        private TextEdit txtTcNo;
        private ComboBoxEdit cmbGender;
        private DateEdit dtBirthDate;
        private MemoEdit txtAddress;

        // Controls - Acil Durum Bilgileri
        private TextEdit txtEmergencyContact;
        private TextEdit txtEmergencyPhone;

        // Controls - Bildirim Tercihleri
        private CheckEdit chkEmailNotifications;
        private CheckEdit chkSMSNotifications;

        // Controls - Şifre Değiştirme
        private TextEdit txtCurrentPassword;
        private TextEdit txtNewPassword;
        private TextEdit txtNewPasswordRepeat;

        // Buttons
        private SimpleButton btnSave;
        private SimpleButton btnChangePassword;
        private SimpleButton btnCancel;

        /// <summary>
        /// FrmProfile constructor
        /// </summary>
        public FrmProfile(User user)
        {
            _currentUser = user;
            _userService = new SUser();
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
            this.Text = "Profilim";
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            int formHeight = Math.Min((int)(screenHeight * 0.8), 900);
            this.ClientSize = new Size(950, formHeight);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.AutoScrollMinSize = new Size(0, 1200);
            this.Font = new Font("Tahoma", 8.25F);

            int leftMargin = 40;
            int rightMargin = 40;
            int topMargin = 25;
            int currentY = topMargin;
            int contentWidth = this.Width - leftMargin - rightMargin;
            int columnWidth = (contentWidth - 30) / 2;
            int fieldHeight = 32;

            // ========== HEADER SECTION ==========
            var lblTitle = new LabelControl();
            lblTitle.Text = "👤 Profilim";
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblTitle);

            var lblSubtitle = new LabelControl();
            lblSubtitle.Text = "Kişisel bilgilerinizi görüntüleyin ve düzenleyin";
            lblSubtitle.Appearance.Font = new Font("Tahoma", 9F);
            lblSubtitle.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            lblSubtitle.Location = new Point(leftMargin, currentY + 30);
            this.Controls.Add(lblSubtitle);

            var btnBack = new SimpleButton();
            btnBack.Text = "Geri Dön";
            btnBack.Size = new Size(110, 36);
            btnBack.Location = new Point(this.Width - rightMargin - 110, currentY + 5);
            btnBack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBack.Appearance.Font = new Font("Tahoma", 9F);
            btnBack.Appearance.BackColor = Color.FromArgb(241, 245, 249);
            btnBack.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            btnBack.Appearance.BorderColor = Color.FromArgb(226, 232, 240);
            btnBack.Appearance.Options.UseBackColor = true;
            btnBack.Appearance.Options.UseForeColor = true;
            btnBack.Appearance.Options.UseBorderColor = true;
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnBack);

            currentY += 75;

            // ========== FORM PANEL ==========
            var pnlForm = new Helpers.RoundedPanel();
            int panelHeight = 1200;
            pnlForm.Size = new Size(contentWidth, panelHeight);
            pnlForm.Location = new Point(leftMargin, currentY);
            pnlForm.BackColor = Color.White;
            pnlForm.BorderRadius = 12;
            pnlForm.BorderThickness = 1;
            pnlForm.BorderColor = Color.FromArgb(226, 232, 240);
            pnlForm.Padding = new Padding(35);
            this.Controls.Add(pnlForm);

            int panelY = 30;
            int panelLeft = 35;

            // ========== KİŞİSEL BİLGİLER SECTION ==========
            var lblSectionPersonal = new LabelControl();
            lblSectionPersonal.Text = "Kişisel Bilgiler";
            lblSectionPersonal.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblSectionPersonal.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblSectionPersonal.Location = new Point(panelLeft, panelY);
            pnlForm.Controls.Add(lblSectionPersonal);

            panelY += 40;

            int leftColX = panelLeft;
            int rightColX = panelLeft + columnWidth + 30;
            int startFieldY = panelY;

            // LEFT COLUMN
            int leftY = startFieldY;

            AddFieldLabel(pnlForm, "Ad *", leftColX, leftY, true);
            leftY += 20;
            this.txtFirstName = AddTextEdit(pnlForm, leftColX, leftY, columnWidth, fieldHeight);
            leftY += 45;

            AddFieldLabel(pnlForm, "E-posta *", leftColX, leftY, true);
            leftY += 20;
            this.txtEmail = AddTextEdit(pnlForm, leftColX, leftY, columnWidth, fieldHeight);
            this.txtEmail.Properties.ReadOnly = true; // Email değiştirilemez
            this.txtEmail.BackColor = Color.FromArgb(248, 249, 250);
            leftY += 45;

            AddFieldLabel(pnlForm, "TC Kimlik No", leftColX, leftY, false);
            leftY += 20;
            this.txtTcNo = AddTextEdit(pnlForm, leftColX, leftY, columnWidth, fieldHeight);
            this.txtTcNo.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtTcNo.Properties.Mask.EditMask = "00000000000";
            leftY += 45;

            AddFieldLabel(pnlForm, "Doğum Tarihi", leftColX, leftY, false);
            leftY += 20;
            this.dtBirthDate = new DateEdit();
            this.dtBirthDate.Location = new Point(leftColX, leftY);
            this.dtBirthDate.Size = new Size(columnWidth, fieldHeight);
            this.dtBirthDate.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            this.dtBirthDate.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.dtBirthDate.Properties.DisplayFormat.FormatString = "dd.MM.yyyy";
            this.dtBirthDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtBirthDate.Properties.EditFormat.FormatString = "dd.MM.yyyy";
            this.dtBirthDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtBirthDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            this.dtBirthDate.Properties.Mask.EditMask = "dd.MM.yyyy";
            this.dtBirthDate.Properties.NullText = "gg.aa.yyyy";
            pnlForm.Controls.Add(this.dtBirthDate);
            leftY += 45;

            // RIGHT COLUMN
            int rightY = startFieldY;

            AddFieldLabel(pnlForm, "Soyad *", rightColX, rightY, true);
            rightY += 20;
            this.txtLastName = AddTextEdit(pnlForm, rightColX, rightY, columnWidth, fieldHeight);
            rightY += 45;

            AddFieldLabel(pnlForm, "Telefon", rightColX, rightY, false);
            rightY += 20;
            this.txtPhone = AddTextEdit(pnlForm, rightColX, rightY, columnWidth, fieldHeight);
            this.txtPhone.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Simple;
            this.txtPhone.Properties.Mask.EditMask = "0000 000 00 00";
            rightY += 45;

            AddFieldLabel(pnlForm, "Cinsiyet", rightColX, rightY, false);
            rightY += 20;
            this.cmbGender = new ComboBoxEdit();
            this.cmbGender.Location = new Point(rightColX, rightY);
            this.cmbGender.Size = new Size(columnWidth, fieldHeight);
            this.cmbGender.Properties.Items.AddRange(new[] { "Erkek", "Kadın", "Diğer" });
            this.cmbGender.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbGender.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            this.cmbGender.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.cmbGender.Properties.NullText = "Seçiniz";
            pnlForm.Controls.Add(this.cmbGender);
            rightY += 45;

            // FULL WIDTH
            int maxColumnY = Math.Max(leftY, rightY);
            panelY = maxColumnY + 30;

            AddFieldLabel(pnlForm, "Adres", panelLeft, panelY, false);
            panelY += 20;
            this.txtAddress = new MemoEdit();
            this.txtAddress.Location = new Point(panelLeft, panelY);
            this.txtAddress.Size = new Size(contentWidth - 70, 90);
            this.txtAddress.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            this.txtAddress.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.txtAddress.Properties.NullText = "Tam adres bilgisi";
            pnlForm.Controls.Add(this.txtAddress);
            panelY += 110;

            // ========== ACIL DURUM BİLGİLERİ SECTION ==========
            panelY += 30;

            var lblSectionEmergency = new LabelControl();
            lblSectionEmergency.Text = "📞 Acil Durum Bilgileri";
            lblSectionEmergency.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblSectionEmergency.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblSectionEmergency.Location = new Point(panelLeft, panelY);
            pnlForm.Controls.Add(lblSectionEmergency);
            panelY += 40;

            AddFieldLabel(pnlForm, "Acil Durum Kişi Adı", leftColX, panelY, false);
            panelY += 20;
            this.txtEmergencyContact = AddTextEdit(pnlForm, leftColX, panelY, columnWidth, fieldHeight);
            panelY += 45;

            AddFieldLabel(pnlForm, "Acil Durum Telefon", rightColX, panelY - 65, false);
            this.txtEmergencyPhone = AddTextEdit(pnlForm, rightColX, panelY - 45, columnWidth, fieldHeight);
            this.txtEmergencyPhone.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Simple;
            this.txtEmergencyPhone.Properties.Mask.EditMask = "0000 000 00 00";
            panelY += 30;

            // ========== BİLDİRİM TERCIHLERİ SECTION ==========
            panelY += 30;

            var lblSectionNotifications = new LabelControl();
            lblSectionNotifications.Text = "🔔 Bildirim Tercihleri";
            lblSectionNotifications.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblSectionNotifications.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblSectionNotifications.Location = new Point(panelLeft, panelY);
            pnlForm.Controls.Add(lblSectionNotifications);
            panelY += 40;

            this.chkEmailNotifications = new CheckEdit();
            this.chkEmailNotifications.Text = "E-posta Bildirimleri";
            this.chkEmailNotifications.Location = new Point(panelLeft, panelY);
            this.chkEmailNotifications.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            pnlForm.Controls.Add(this.chkEmailNotifications);
            panelY += 35;

            this.chkSMSNotifications = new CheckEdit();
            this.chkSMSNotifications.Text = "SMS Bildirimleri";
            this.chkSMSNotifications.Location = new Point(panelLeft, panelY);
            this.chkSMSNotifications.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            pnlForm.Controls.Add(this.chkSMSNotifications);
            panelY += 50;

            // ========== ŞİFRE DEĞİŞTİRME SECTION ==========
            panelY += 30;

            var lblSectionPassword = new LabelControl();
            lblSectionPassword.Text = "🔒 Şifre Değiştir";
            lblSectionPassword.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblSectionPassword.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblSectionPassword.Location = new Point(panelLeft, panelY);
            pnlForm.Controls.Add(lblSectionPassword);
            panelY += 40;

            AddFieldLabel(pnlForm, "Mevcut Şifre", panelLeft, panelY, false);
            panelY += 20;
            this.txtCurrentPassword = AddPasswordEdit(pnlForm, panelLeft, panelY, contentWidth - 70, fieldHeight);
            panelY += 45;

            AddFieldLabel(pnlForm, "Yeni Şifre", panelLeft, panelY, false);
            panelY += 20;
            this.txtNewPassword = AddPasswordEdit(pnlForm, panelLeft, panelY, contentWidth - 70, fieldHeight);
            panelY += 45;

            AddFieldLabel(pnlForm, "Yeni Şifre Tekrar", panelLeft, panelY, false);
            panelY += 20;
            this.txtNewPasswordRepeat = AddPasswordEdit(pnlForm, panelLeft, panelY, contentWidth - 70, fieldHeight);
            panelY += 50;

            // Şifre Değiştir Butonu
            this.btnChangePassword = new SimpleButton();
            this.btnChangePassword.Text = "Şifreyi Değiştir";
            this.btnChangePassword.Size = new Size(160, 38);
            this.btnChangePassword.Location = new Point(panelLeft, panelY);
            this.btnChangePassword.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            this.btnChangePassword.Appearance.BackColor = Color.FromArgb(234, 179, 8);
            this.btnChangePassword.Appearance.ForeColor = Color.White;
            this.btnChangePassword.Appearance.Options.UseBackColor = true;
            this.btnChangePassword.Appearance.Options.UseForeColor = true;
            this.btnChangePassword.Cursor = Cursors.Hand;
            this.btnChangePassword.Click += BtnChangePassword_Click;
            pnlForm.Controls.Add(this.btnChangePassword);
            panelY += 50;

            // ========== BUTTONS ==========
            panelY += 20;

            this.btnCancel = new SimpleButton();
            this.btnCancel.Text = "İptal";
            this.btnCancel.Size = new Size(130, 38);
            this.btnCancel.Location = new Point(panelLeft, panelY);
            this.btnCancel.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            this.btnCancel.Appearance.BackColor = Color.FromArgb(241, 245, 249);
            this.btnCancel.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            this.btnCancel.Appearance.BorderColor = Color.FromArgb(226, 232, 240);
            this.btnCancel.Appearance.Options.UseBackColor = true;
            this.btnCancel.Appearance.Options.UseForeColor = true;
            this.btnCancel.Appearance.Options.UseBorderColor = true;
            this.btnCancel.Cursor = Cursors.Hand;
            this.btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            pnlForm.Controls.Add(this.btnCancel);

            this.btnSave = new SimpleButton();
            this.btnSave.Text = "Kaydet";
            this.btnSave.Size = new Size(160, 38);
            this.btnSave.Location = new Point(contentWidth - 70 - 160, panelY);
            this.btnSave.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            this.btnSave.Appearance.BackColor = Color.FromArgb(59, 130, 246);
            this.btnSave.Appearance.ForeColor = Color.White;
            this.btnSave.Appearance.Options.UseBackColor = true;
            this.btnSave.Appearance.Options.UseForeColor = true;
            this.btnSave.Cursor = Cursors.Hand;
            this.btnSave.Click += BtnSave_Click;
            pnlForm.Controls.Add(this.btnSave);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Alan etiketi ekler
        /// </summary>
        private void AddFieldLabel(Control parent, string text, int x, int y, bool required)
        {
            var lbl = new LabelControl();
            lbl.Text = text;
            if (required)
            {
                lbl.Appearance.ForeColor = Color.FromArgb(239, 68, 68);
            }
            else
            {
                lbl.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
            }
            lbl.Appearance.Font = new Font("Tahoma", 8.5F, FontStyle.Regular);
            lbl.Location = new Point(x, y);
            parent.Controls.Add(lbl);
        }

        /// <summary>
        /// TextEdit kontrolü ekler
        /// </summary>
        private TextEdit AddTextEdit(Control parent, int x, int y, int width, int height)
        {
            var txt = new TextEdit();
            txt.Location = new Point(x, y);
            txt.Size = new Size(width, height);
            txt.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            txt.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            txt.Properties.Appearance.BorderColor = Color.FromArgb(226, 232, 240);
            txt.Properties.Appearance.Options.UseBorderColor = true;
            parent.Controls.Add(txt);
            return txt;
        }

        /// <summary>
        /// Password TextEdit kontrolü ekler
        /// </summary>
        private TextEdit AddPasswordEdit(Control parent, int x, int y, int width, int height)
        {
            var txt = AddTextEdit(parent, x, y, width, height);
            txt.Properties.PasswordChar = '•';
            txt.Properties.UseSystemPasswordChar = true;
            return txt;
        }

        /// <summary>
        /// Verileri yükler
        /// </summary>
        private void LoadData()
        {
            if (_currentUser != null)
            {
                // Kullanıcıyı veritabanından yeniden yükle (güncel bilgiler için)
                var user = _userService.GetById(_currentUser.Id);
                if (user == null) return;

                txtFirstName.Text = user.FirstName ?? "";
                txtLastName.Text = user.LastName ?? "";
                txtEmail.Text = user.Email ?? "";
                txtPhone.Text = user.Phone ?? "";
                txtTcNo.Text = user.TcNo ?? "";
                cmbGender.EditValue = user.Gender ?? "";
                if (user.BirthDate.HasValue)
                {
                    dtBirthDate.EditValue = user.BirthDate.Value;
                }
                txtAddress.Text = user.Address ?? "";

                // Acil Durum Bilgileri
                txtEmergencyContact.Text = user.EmergencyContact ?? "";
                txtEmergencyPhone.Text = user.EmergencyPhone ?? "";

                // Bildirim Tercihleri
                chkEmailNotifications.Checked = user.EmailNotifications;
                chkSMSNotifications.Checked = user.SMSNotifications;
            }
        }

        /// <summary>
        /// Kaydet butonuna tıklandığında
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                Swal.Warning("Lütfen Ad ve Soyad alanlarını doldurun.");
                return;
            }

            try
            {
                // Kullanıcıyı veritabanından yeniden yükle
                var user = _userService.GetById(_currentUser.Id);
                if (user == null)
                {
                    Swal.Error("Kullanıcı bulunamadı.");
                    return;
                }

                // Bilgileri güncelle
                user.FirstName = txtFirstName.Text.Trim();
                user.LastName = txtLastName.Text.Trim();
                user.Phone = txtPhone.Text.Trim();
                user.TcNo = txtTcNo.Text.Trim();
                user.Gender = cmbGender.EditValue?.ToString() ?? "";
                user.BirthDate = dtBirthDate.EditValue as DateTime?;
                user.Address = txtAddress.Text.Trim();

                // Acil Durum Bilgileri
                user.EmergencyContact = txtEmergencyContact.Text.Trim();
                user.EmergencyPhone = txtEmergencyPhone.Text.Trim();

                // Bildirim Tercihleri
                user.EmailNotifications = chkEmailNotifications.Checked;
                user.SMSNotifications = chkSMSNotifications.Checked;

                string result = _userService.Update(user);
                if (!string.IsNullOrEmpty(result))
                {
                    Swal.Error("Güncelleme hatası: " + result);
                    return;
                }

                // Current user'ı güncelle
                _currentUser = user;

                Swal.Success("Profil bilgileriniz başarıyla güncellendi.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Swal.Error("İşlem hatası: " + ex.Message);
            }
        }

        /// <summary>
        /// Şifre değiştir butonuna tıklandığında
        /// </summary>
        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text))
            {
                Swal.Warning("Lütfen mevcut şifrenizi girin.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                Swal.Warning("Lütfen yeni şifre girin.");
                return;
            }

            if (txtNewPassword.Text.Length < 8)
            {
                Swal.Warning("Yeni şifre en az 8 karakter olmalıdır.");
                return;
            }

            if (txtNewPassword.Text != txtNewPasswordRepeat.Text)
            {
                Swal.Warning("Yeni şifreler eşleşmiyor.");
                return;
            }

            try
            {
                // Mevcut şifreyi kontrol et
                var user = _userService.GetById(_currentUser.Id);
                if (user == null)
                {
                    Swal.Error("Kullanıcı bulunamadı.");
                    return;
                }

                if (!PasswordHelper.VerifyPassword(txtCurrentPassword.Text, user.PasswordHash))
                {
                    Swal.Error("Mevcut şifre hatalı.");
                    return;
                }

                // Yeni şifreyi hashle ve kaydet
                user.PasswordHash = PasswordHelper.HashPassword(txtNewPassword.Text);
                string result = _userService.Update(user);
                if (!string.IsNullOrEmpty(result))
                {
                    Swal.Error("Şifre değiştirme hatası: " + result);
                    return;
                }

                // Şifre alanlarını temizle
                txtCurrentPassword.Text = "";
                txtNewPassword.Text = "";
                txtNewPasswordRepeat.Text = "";

                Swal.Success("Şifreniz başarıyla değiştirildi.");
            }
            catch (Exception ex)
            {
                Swal.Error("İşlem hatası: " + ex.Message);
            }
        }
    }
}

