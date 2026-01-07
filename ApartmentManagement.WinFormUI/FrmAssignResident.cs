#nullable disable
// FrmAssignResident.cs
// Daireye sakin (ev sahibi / kiracı) atama formu
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    public class FrmAssignResident : XtraForm
    {
        private readonly int _flatId;
        private readonly IFlat _flatService;
        private readonly IUser _userService;
        private readonly IFlatResident _flatResidentService;

        // Controls
        private ComboBoxEdit cmbUsers;
        private ComboBoxEdit cmbType;
        private ComboBoxEdit cmbStatus;
        private DateEdit dtStart;
        private DateEdit dtEnd;
        private TextEdit txtRent;
        private MemoEdit txtNote;
        private SimpleButton btnSave;
        private SimpleButton btnCancel;

        public FrmAssignResident(int flatId)
        {
            _flatId = flatId;
            _flatService = new SFlat();
            _userService = new SUser();
            _flatResidentService = new SFlatResident();

            InitializeComponent();
            LoadUsers();
            LoadFlatInfo();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "Sakin Ata";
            this.Size = new Size(1100, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 252);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int leftMargin = 25;
            int topMargin = 20;
            int fieldHeight = 34;
            int labelSpacing = 6;
            int columnWidth = 340;
            int colGap = 20;

            // Title
            var lblTitle = new LabelControl();
            lblTitle.Text = "Sakin Ata";
            lblTitle.Appearance.Font = new Font("Tahoma", 16F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftMargin, topMargin);
            this.Controls.Add(lblTitle);

            topMargin += 40;

            // Kullanıcı
            AddLabel("Kullanıcı", leftMargin, topMargin);
            cmbUsers = new ComboBoxEdit();
            cmbUsers.Location = new Point(leftMargin, topMargin + labelSpacing + 16);
            cmbUsers.Size = new Size(columnWidth, fieldHeight);
            cmbUsers.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbUsers.Properties.NullText = "Seçiniz";
            this.Controls.Add(cmbUsers);

            // Tip
            AddLabel("Tip", leftMargin + columnWidth + colGap, topMargin);
            cmbType = new ComboBoxEdit();
            cmbType.Location = new Point(leftMargin + columnWidth + colGap, topMargin + labelSpacing + 16);
            cmbType.Size = new Size(columnWidth, fieldHeight);
            cmbType.Properties.Items.AddRange(new[] { "Ev Sahibi", "Kiracı" });
            cmbType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbType.SelectedIndex = 0;
            this.Controls.Add(cmbType);

            // Durum
            AddLabel("Durum", leftMargin + (columnWidth + colGap) * 2, topMargin);
            cmbStatus = new ComboBoxEdit();
            cmbStatus.Location = new Point(leftMargin + (columnWidth + colGap) * 2, topMargin + labelSpacing + 16);
            cmbStatus.Size = new Size(columnWidth, fieldHeight);
            cmbStatus.Properties.Items.AddRange(new[] { "Aktif", "Pasif" });
            cmbStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbStatus.SelectedIndex = 0;
            this.Controls.Add(cmbStatus);

            topMargin += 70;

            // Kira
            AddLabel("Kira (₺)", leftMargin, topMargin);
            txtRent = new TextEdit();
            txtRent.Location = new Point(leftMargin, topMargin + labelSpacing + 16);
            txtRent.Size = new Size(columnWidth, fieldHeight);
            txtRent.Properties.Mask.EditMask = "n2";
            txtRent.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            txtRent.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.Controls.Add(txtRent);

            // Giriş Tarihi
            AddLabel("Giriş Tarihi", leftMargin + columnWidth + colGap, topMargin);
            dtStart = new DateEdit();
            dtStart.Location = new Point(leftMargin + columnWidth + colGap, topMargin + labelSpacing + 16);
            dtStart.Size = new Size(columnWidth, fieldHeight);
            dtStart.Properties.DisplayFormat.FormatString = "dd.MM.yyyy";
            dtStart.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dtStart.Properties.EditFormat.FormatString = "dd.MM.yyyy";
            dtStart.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dtStart.Properties.Mask.EditMask = "dd.MM.yyyy";
            dtStart.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
            dtStart.EditValue = DateTime.Today;
            this.Controls.Add(dtStart);

            // Çıkış Tarihi
            AddLabel("Çıkış Tarihi", leftMargin + (columnWidth + colGap) * 2, topMargin);
            dtEnd = new DateEdit();
            dtEnd.Location = new Point(leftMargin + (columnWidth + colGap) * 2, topMargin + labelSpacing + 16);
            dtEnd.Size = new Size(columnWidth, fieldHeight);
            dtEnd.Properties.DisplayFormat.FormatString = "dd.MM.yyyy";
            dtEnd.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dtEnd.Properties.EditFormat.FormatString = "dd.MM.yyyy";
            dtEnd.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dtEnd.Properties.Mask.EditMask = "dd.MM.yyyy";
            dtEnd.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
            dtEnd.Properties.NullText = "gg.aa.yyyy";
            this.Controls.Add(dtEnd);

            topMargin += 70;

            // Not
            AddLabel("Not", leftMargin, topMargin);
            txtNote = new MemoEdit();
            txtNote.Location = new Point(leftMargin, topMargin + labelSpacing + 16);
            txtNote.Size = new Size(columnWidth * 3 + colGap * 2, 80);
            txtNote.Properties.MaxLength = 500;
            this.Controls.Add(txtNote);

            topMargin += 120;

            // Buttons
            btnCancel = new SimpleButton();
            btnCancel.Text = "Vazgeç";
            btnCancel.Size = new Size(120, 36);
            btnCancel.Location = new Point(leftMargin, topMargin);
            btnCancel.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnCancel.Appearance.BackColor = Color.FromArgb(241, 245, 249);
            btnCancel.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancel);

            btnSave = new SimpleButton();
            btnSave.Text = "Kaydet";
            btnSave.Size = new Size(140, 36);
            btnSave.Location = new Point(leftMargin + 140, topMargin);
            btnSave.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnSave.Appearance.BackColor = Color.FromArgb(59, 130, 246);
            btnSave.Appearance.ForeColor = Color.White;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            this.ResumeLayout(false);
        }

        private void AddLabel(string text, int x, int y)
        {
            var lbl = new LabelControl();
            lbl.Text = text;
            lbl.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lbl.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
            lbl.Location = new Point(x, y);
            this.Controls.Add(lbl);
        }

        private void LoadUsers()
        {
            var users = _userService.GetAll()
                .Where(u => u.IsApproved)
                .OrderBy(u => u.FirstName)
                .Select(u => new
                {
                    u.Id,
                    Display = $"{u.FirstName} {u.LastName} ({u.Email})"
                }).ToList();

            cmbUsers.Properties.Items.Clear();
            foreach (var user in users)
            {
                cmbUsers.Properties.Items.Add(new UserItem { Id = user.Id, Name = user.Display });
            }
        }

        private void LoadFlatInfo()
        {
            var flat = _flatService.GetById(_flatId);
            if (flat != null)
            {
                this.Text = $"Sakin Ata – Daire {flat.DoorNumber}";
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validation
            UserItem selectedUser = null;
            if (cmbUsers.EditValue is UserItem eu) selectedUser = eu;
            else if (cmbUsers.SelectedItem is UserItem si) selectedUser = si;
            if (selectedUser == null)
            {
                Swal.Warning("Lütfen bir kullanıcı seçin.");
                return;
            }

            if (cmbType.SelectedItem == null)
            {
                Swal.Warning("Lütfen tip seçin (Ev Sahibi/Kiracı).");
                return;
            }
            if (cmbStatus.SelectedItem == null)
            {
                Swal.Warning("Lütfen durum seçin (Aktif/Pasif).");
                return;
            }

            DateTime startDate = dtStart.EditValue is DateTime dt ? dt.Date : DateTime.Today;
            DateTime? endDate = dtEnd.EditValue as DateTime?;
            string status = cmbStatus.SelectedItem.ToString();
            bool isOwner = cmbType.SelectedItem.ToString() == "Ev Sahibi";

            if (status == "Pasif" && !endDate.HasValue)
            {
                endDate = DateTime.Today;
            }

            decimal? rent = null;
            if (!string.IsNullOrWhiteSpace(txtRent.Text))
            {
                if (decimal.TryParse(txtRent.Text, out var r))
                {
                    rent = r;
                }
                else
                {
                    Swal.Warning("Kira tutarı geçersiz.");
                    return;
                }
            }

            var flatResident = new FlatResident
            {
                FlatId = _flatId,
                UserId = selectedUser.Id,
                IsOwner = isOwner,
                StartDate = startDate.ToUniversalTime(),
                EndDate = endDate?.ToUniversalTime(),
                Status = status,
                Rent = rent,
                Note = txtNote.Text?.Trim()
            };

            string result = _flatResidentService.Add(flatResident);
            if (!string.IsNullOrEmpty(result))
            {
                Swal.Error("Kayıt hatası: " + result);
                return;
            }

            Swal.Success("Sakin atama başarıyla kaydedildi.");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private class UserItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public override string ToString() => Name;
        }
    }
}


