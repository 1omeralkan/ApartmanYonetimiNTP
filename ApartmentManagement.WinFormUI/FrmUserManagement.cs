#nullable disable
// FrmUserManagement.cs
// Kullanıcı Yönetim Formu - Yeni kullanıcı ekleme ve düzenleme
// Standart: Tahoma 8.25pt, AutoScroll = true
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Kullanıcı yönetim formu - Yeni kullanıcı ekleme ve düzenleme
    /// </summary>
    public partial class FrmUserManagement : DevExpress.XtraEditors.XtraForm
    {
        private IUser _userService;
        private IAuth _authService;
        private User _currentUser;
        private bool _isEditMode;

        // Controls
        private TextEdit txtFirstName;
        private TextEdit txtLastName;
        private TextEdit txtEmail;
        private TextEdit txtPhone;
        private TextEdit txtTcNo;
        private ComboBoxEdit cmbGender;
        private DateEdit dtBirthDate;
        private TextEdit txtPassword;
        private TextEdit txtPasswordRepeat;
        private MemoEdit txtAddress;
        private SimpleButton btnSave;
        private SimpleButton btnCancel;

        /// <summary>
        /// FrmUserManagement constructor - Formu başlatır
        /// </summary>
        /// <param name="user">Düzenlenecek kullanıcı (null ise yeni kullanıcı)</param>
        public FrmUserManagement(User user)
        {
            _userService = new SUser();
            _authService = new SAuth();
            _currentUser = user;
            _isEditMode = user != null;
            InitializeComponent();
            if (_isEditMode)
            {
            LoadData();
            }
        }

        /// <summary>
        /// Form bileşenlerini başlatır
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = _isEditMode ? "Kullanıcı Düzenle" : "Yeni Kullanıcı Ekle";
            this.ClientSize = new Size(950, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            int leftMargin = 40;
            int rightMargin = 40;
            int topMargin = 25;
            int currentY = topMargin;
            int contentWidth = this.Width - leftMargin - rightMargin;
            int columnWidth = (contentWidth - 30) / 2; // İki kolon, aralarında 30px boşluk
            int fieldHeight = 32; // Daha yüksek input field'lar

            // ========== HEADER SECTION ==========
            // Title
            var lblTitle = new LabelControl();
            lblTitle.Text = _isEditMode ? "Kullanıcı Düzenle" : "Yeni Kullanıcı Ekle";
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblTitle);

            // Subtitle
            var lblSubtitle = new LabelControl();
            lblSubtitle.Text = _isEditMode ? "Kullanıcı bilgilerini düzenleyin" : "Sisteme yeni bir kullanıcı ekleyin";
            lblSubtitle.Appearance.Font = new Font("Tahoma", 9F);
            lblSubtitle.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            lblSubtitle.Location = new Point(leftMargin, currentY + 30);
            this.Controls.Add(lblSubtitle);

            // Back Button (Top Right)
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
            var pnlForm = new RoundedPanel();
            pnlForm.Size = new Size(contentWidth, this.Height - currentY - 80);
            pnlForm.Location = new Point(leftMargin, currentY);
            pnlForm.BackColor = Color.White;
            pnlForm.BorderRadius = 12;
            pnlForm.BorderThickness = 1;
            pnlForm.BorderColor = Color.FromArgb(226, 232, 240);
            pnlForm.Padding = new Padding(35);
            this.Controls.Add(pnlForm);

            int panelY = 30;
            int panelLeft = 35;

            // Section Title: Kişisel Bilgiler
            var lblSectionTitle = new LabelControl();
            lblSectionTitle.Text = "Kişisel Bilgiler";
            lblSectionTitle.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblSectionTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblSectionTitle.Location = new Point(panelLeft, panelY);
            pnlForm.Controls.Add(lblSectionTitle);

            panelY += 40;

            // ========== COLUMNS SETUP ==========
            int leftColX = panelLeft;
            int rightColX = panelLeft + columnWidth + 30;
            int startFieldY = panelY; // Başlangıç Y pozisyonu (her iki kolon için aynı)

            // ========== LEFT COLUMN ==========
            int leftY = startFieldY;

            // Ad* (Left)
            AddFieldLabel(pnlForm, "Ad *", leftColX, leftY, true);
            leftY += 20;
            this.txtFirstName = AddTextEdit(pnlForm, leftColX, leftY, columnWidth, fieldHeight);
            leftY += 45;

            // E-posta* (Left)
            AddFieldLabel(pnlForm, "E-posta *", leftColX, leftY, true);
            leftY += 20;
            this.txtEmail = AddTextEdit(pnlForm, leftColX, leftY, columnWidth, fieldHeight);
            leftY += 45;

            // TC Kimlik No (Left)
            AddFieldLabel(pnlForm, "TC Kimlik No", leftColX, leftY, false);
            leftY += 20;
            this.txtTcNo = AddTextEdit(pnlForm, leftColX, leftY, columnWidth, fieldHeight);
            this.txtTcNo.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtTcNo.Properties.Mask.EditMask = "00000000000";
            leftY += 45;

            // Doğum Tarihi (Left)
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

            // Şifre Tekrar* (Left) - Only for new user
            if (!_isEditMode)
            {
                AddFieldLabel(pnlForm, "Şifre Tekrar *", leftColX, leftY, true);
                leftY += 20;
                this.txtPasswordRepeat = AddPasswordEdit(pnlForm, leftColX, leftY, columnWidth, fieldHeight);
                leftY += 45;
            }

            // ========== RIGHT COLUMN ==========
            int rightY = startFieldY; // Sağ kolon da aynı başlangıç pozisyonundan başlar

            // Soyad* (Right)
            AddFieldLabel(pnlForm, "Soyad *", rightColX, rightY, true);
            rightY += 20;
            this.txtLastName = AddTextEdit(pnlForm, rightColX, rightY, columnWidth, fieldHeight);
            rightY += 45;

            // Telefon (Right)
            AddFieldLabel(pnlForm, "Telefon", rightColX, rightY, false);
            rightY += 20;
            this.txtPhone = AddTextEdit(pnlForm, rightColX, rightY, columnWidth, fieldHeight);
            this.txtPhone.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Simple;
            this.txtPhone.Properties.Mask.EditMask = "0000 000 00 00";
            rightY += 45;

            // Cinsiyet (Right)
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

            // Şifre* (Right) - Only for new user
            if (!_isEditMode)
            {
                AddFieldLabel(pnlForm, "Şifre *", rightColX, rightY, true);
                rightY += 20;
                this.txtPassword = AddPasswordEdit(pnlForm, rightColX, rightY, columnWidth, fieldHeight);
                rightY += 45;
            }

            // ========== FULL WIDTH FIELDS ==========
            // Calculate max Y from both columns
            int maxColumnY = Math.Max(leftY, rightY);
            panelY = maxColumnY + 30; // Add spacing after columns

            // Adres (Full Width)
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

            // ========== BUTTONS ==========
            panelY += 20; // Add spacing before buttons
            
            // Cancel Button
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

            // Save Button
            this.btnSave = new SimpleButton();
            this.btnSave.Text = _isEditMode ? "Güncelle" : "Kullanıcı Oluştur";
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
                lbl.Appearance.ForeColor = Color.FromArgb(239, 68, 68); // Red for required
            }
            else
            {
                lbl.Appearance.ForeColor = Color.FromArgb(71, 85, 105); // Darker gray for better readability
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
        /// Kullanıcı verilerini forma yükler
        /// </summary>
        private void LoadData()
        {
            if (_currentUser != null)
            {
                txtFirstName.Text = _currentUser.FirstName ?? "";
                txtLastName.Text = _currentUser.LastName ?? "";
                txtEmail.Text = _currentUser.Email ?? "";
                txtPhone.Text = _currentUser.Phone ?? "";
                txtTcNo.Text = _currentUser.TcNo ?? "";
                cmbGender.EditValue = _currentUser.Gender ?? "";
                if (_currentUser.BirthDate.HasValue)
                {
                    dtBirthDate.EditValue = _currentUser.BirthDate.Value;
                }
                txtAddress.Text = _currentUser.Address ?? "";
            }
        }

        /// <summary>
        /// Kaydet butonuna tıklandığında çalışır
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                Swal.Warning("Lütfen zorunlu alanları doldurun (Ad, Soyad, E-posta).");
                return;
            }

            if (!_isEditMode)
            {
                // New user validation
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    Swal.Warning("Lütfen şifre girin.");
                    return;
                }

                if (txtPassword.Text.Length < 8)
                {
                    Swal.Warning("Şifre en az 8 karakter olmalıdır.");
                    return;
                }

                if (txtPassword.Text != txtPasswordRepeat.Text)
                {
                    Swal.Warning("Şifreler eşleşmiyor.");
                    return;
                }
            }

            try
            {
                if (_isEditMode)
                {
                    // Update existing user
                _currentUser.FirstName = txtFirstName.Text.Trim();
                _currentUser.LastName = txtLastName.Text.Trim();
                _currentUser.Email = txtEmail.Text.Trim();
                _currentUser.Phone = txtPhone.Text.Trim();
                    _currentUser.TcNo = txtTcNo.Text.Trim();
                    _currentUser.Gender = cmbGender.EditValue?.ToString() ?? "";
                    _currentUser.BirthDate = dtBirthDate.EditValue as DateTime?;
                    _currentUser.Address = txtAddress.Text.Trim();
                    // Role remains unchanged in edit mode

                string result = _userService.Update(_currentUser);
                if (!string.IsNullOrEmpty(result))
                {
                    Swal.Error("Güncelleme hatası: " + result);
                    return;
                }
                
                Swal.Success("Kullanıcı başarıyla güncellendi.");
                }
                else
                {
                    // Create new user (default role: Resident)
                    var newUser = _authService.RegisterFull(
                        txtFirstName.Text.Trim(),
                        txtLastName.Text.Trim(),
                        txtTcNo.Text.Trim(),
                        cmbGender.EditValue?.ToString() ?? "",
                        dtBirthDate.EditValue as DateTime?,
                        txtEmail.Text.Trim(),
                        txtPhone.Text.Trim(),
                        txtAddress.Text.Trim(),
                        "", // EmergencyContact
                        "", // EmergencyPhone
                        txtPassword.Text
                    );
                    // Role is set to "Resident" by default in RegisterFull

                    Swal.Success("Kullanıcı başarıyla oluşturuldu.");
                }
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Swal.Error("İşlem hatası: " + ex.Message);
            }
        }
    }
}
