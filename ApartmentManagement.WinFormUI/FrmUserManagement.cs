#nullable disable
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ApartmentManagement.WinFormUI
{
    public partial class FrmUserManagement : DevExpress.XtraEditors.XtraForm
    {
        private IUser _userService;
        private User _currentUser;

        // Controls
        private TextEdit txtFirstName;
        private TextEdit txtLastName;
        private TextEdit txtEmail;
        private TextEdit txtPhone;
        private TextEdit txtTcNo;
        private ComboBoxEdit cmbRole;
        private SimpleButton btnSave;

        public FrmUserManagement(User user)
        {
            _userService = new SUser();
            _currentUser = user;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = "Kullanıcı Düzenle";
            this.ClientSize = new Size(450, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);

            int fieldWidth = 380;
            int startX = 30;
            int currentY = 30;

            // Title
            var lblTitle = new LabelControl();
            lblTitle.Text = "Kullanıcı Düzenle";
            lblTitle.Appearance.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(startX, currentY);
            this.Controls.Add(lblTitle);

            currentY += 50;

            // First Name
            AddFieldLabel("Ad", startX, currentY);
            currentY += 22;
            this.txtFirstName = AddTextEdit(startX, currentY, fieldWidth);
            currentY += 45;

            // Last Name
            AddFieldLabel("Soyad", startX, currentY);
            currentY += 22;
            this.txtLastName = AddTextEdit(startX, currentY, fieldWidth);
            currentY += 45;

            // Email
            AddFieldLabel("E-posta", startX, currentY);
            currentY += 22;
            this.txtEmail = AddTextEdit(startX, currentY, fieldWidth);
            currentY += 45;

            // Phone
            AddFieldLabel("Telefon", startX, currentY);
            currentY += 22;
            this.txtPhone = AddTextEdit(startX, currentY, fieldWidth);
            currentY += 45;

            // Role
            AddFieldLabel("Rol", startX, currentY);
            currentY += 22;
            this.cmbRole = new ComboBoxEdit();
            this.cmbRole.Location = new Point(startX, currentY);
            this.cmbRole.Size = new Size(fieldWidth, 32);
            this.cmbRole.Properties.Items.AddRange(SAuth.Roles);
            this.cmbRole.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbRole.Properties.Appearance.Font = new Font("Segoe UI", 11F);
            this.Controls.Add(this.cmbRole);
            currentY += 55;

            // Save Button
            this.btnSave = new SimpleButton();
            this.btnSave.Text = "Kaydet";
            this.btnSave.Size = new Size(fieldWidth, 45);
            this.btnSave.Location = new Point(startX, currentY);
            this.btnSave.Appearance.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.btnSave.Appearance.BackColor = Color.FromArgb(66, 133, 244);
            this.btnSave.Appearance.ForeColor = Color.White;
            this.btnSave.Appearance.Options.UseBackColor = true;
            this.btnSave.Appearance.Options.UseForeColor = true;
            this.btnSave.Cursor = Cursors.Hand;
            this.btnSave.Click += BtnSave_Click;
            this.Controls.Add(this.btnSave);

            this.ResumeLayout(false);
        }

        private void AddFieldLabel(string text, int x, int y)
        {
            var lbl = new LabelControl();
            lbl.Text = text;
            lbl.Appearance.Font = new Font("Segoe UI", 9F);
            lbl.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            lbl.Location = new Point(x, y);
            this.Controls.Add(lbl);
        }

        private TextEdit AddTextEdit(int x, int y, int width)
        {
            var txt = new TextEdit();
            txt.Location = new Point(x, y);
            txt.Size = new Size(width, 32);
            txt.Properties.Appearance.Font = new Font("Segoe UI", 11F);
            txt.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.Controls.Add(txt);
            return txt;
        }

        private void LoadData()
        {
            if (_currentUser != null)
            {
                txtFirstName.Text = _currentUser.FirstName ?? "";
                txtLastName.Text = _currentUser.LastName ?? "";
                txtEmail.Text = _currentUser.Email ?? "";
                txtPhone.Text = _currentUser.Phone ?? "";
                cmbRole.EditValue = _currentUser.Role ?? "Resident";
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                Swal.Warning("Lütfen zorunlu alanları doldurun.");
                return;
            }

            try
            {
                _currentUser.FirstName = txtFirstName.Text.Trim();
                _currentUser.LastName = txtLastName.Text.Trim();
                _currentUser.Email = txtEmail.Text.Trim();
                _currentUser.Phone = txtPhone.Text.Trim();
                _currentUser.Role = cmbRole.EditValue?.ToString() ?? "Resident";

                string result = _userService.Update(_currentUser);
                if (!string.IsNullOrEmpty(result))
                {
                    Swal.Error("Güncelleme hatası: " + result);
                    return;
                }
                
                Swal.Success("Kullanıcı başarıyla güncellendi.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Swal.Error("Güncelleme hatası: " + ex.Message);
            }
        }
    }
}
