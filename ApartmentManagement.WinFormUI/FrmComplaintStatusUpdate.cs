#nullable disable
// FrmComplaintStatusUpdate.cs
// Şikayet/Talep Durum Güncelleme Formu
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
    /// Şikayet/Talep durum güncelleme formu - Manager rolleri için
    /// </summary>
    public partial class FrmComplaintStatusUpdate : DevExpress.XtraEditors.XtraForm
    {
        private IComplaint _complaintService;
        private Complaint _currentComplaint;
        private User _currentUser;

        // Controls
        private ComboBoxEdit cmbStatus;
        private MemoEdit txtComment;
        private SimpleButton btnSave;
        private SimpleButton btnCancel;

        /// <summary>
        /// FrmComplaintStatusUpdate constructor
        /// </summary>
        public FrmComplaintStatusUpdate(Complaint complaint, User user)
        {
            _currentComplaint = complaint;
            _currentUser = user;
            _complaintService = new SComplaint();
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
            this.Text = "Durum Güncelle";
            this.ClientSize = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.Font = new Font("Tahoma", 8.25F);

            int leftMargin = 40;
            int rightMargin = 40;
            int topMargin = 25;
            int currentY = topMargin;
            int contentWidth = this.Width - leftMargin - rightMargin;
            int fieldHeight = 32;

            // ========== HEADER SECTION ==========
            var lblTitle = new LabelControl();
            lblTitle.Text = "Durum Güncelle";
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblTitle);

            currentY += 50;

            // ========== FORM PANEL ==========
            var pnlForm = new Helpers.RoundedPanel();
            pnlForm.Size = new Size(contentWidth, 300);
            pnlForm.Location = new Point(leftMargin, currentY);
            pnlForm.BackColor = Color.White;
            pnlForm.BorderRadius = 12;
            pnlForm.BorderThickness = 1;
            pnlForm.BorderColor = Color.FromArgb(226, 232, 240);
            pnlForm.Padding = new Padding(35);
            this.Controls.Add(pnlForm);

            int panelY = 30;
            int panelLeft = 35;

            // Status
            var lblStatus = new LabelControl();
            lblStatus.Text = "Durum *";
            lblStatus.Appearance.ForeColor = Color.FromArgb(239, 68, 68);
            lblStatus.Appearance.Font = new Font("Tahoma", 8.5F);
            lblStatus.Location = new Point(panelLeft, panelY);
            pnlForm.Controls.Add(lblStatus);
            panelY += 20;

            cmbStatus = new ComboBoxEdit();
            cmbStatus.Location = new Point(panelLeft, panelY);
            cmbStatus.Size = new Size(contentWidth - 70, fieldHeight);
            cmbStatus.Properties.Items.AddRange(new[] { "Beklemede", "İnceleniyor", "Çözüldü", "Reddedildi" });
            cmbStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbStatus.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            cmbStatus.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            pnlForm.Controls.Add(cmbStatus);
            panelY += 45;

            // Comment
            var lblComment = new LabelControl();
            lblComment.Text = "Yorum";
            lblComment.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
            lblComment.Appearance.Font = new Font("Tahoma", 8.5F);
            lblComment.Location = new Point(panelLeft, panelY);
            pnlForm.Controls.Add(lblComment);
            panelY += 20;

            txtComment = new MemoEdit();
            txtComment.Location = new Point(panelLeft, panelY);
            txtComment.Size = new Size(contentWidth - 70, 100);
            txtComment.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            txtComment.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            txtComment.Properties.NullText = "Yorum (Opsiyonel)";
            pnlForm.Controls.Add(txtComment);
            panelY += 120;

            // ========== BUTTONS ==========
            btnCancel = new SimpleButton();
            btnCancel.Text = "İptal";
            btnCancel.Size = new Size(130, 38);
            btnCancel.Location = new Point(panelLeft, panelY);
            btnCancel.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnCancel.Appearance.BackColor = Color.FromArgb(241, 245, 249);
            btnCancel.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            btnCancel.Appearance.BorderColor = Color.FromArgb(226, 232, 240);
            btnCancel.Appearance.Options.UseBackColor = true;
            btnCancel.Appearance.Options.UseForeColor = true;
            btnCancel.Appearance.Options.UseBorderColor = true;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            pnlForm.Controls.Add(btnCancel);

            btnSave = new SimpleButton();
            btnSave.Text = "Güncelle";
            btnSave.Size = new Size(160, 38);
            btnSave.Location = new Point(contentWidth - 70 - 160, panelY);
            btnSave.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnSave.Appearance.BackColor = Color.FromArgb(59, 130, 246);
            btnSave.Appearance.ForeColor = Color.White;
            btnSave.Appearance.Options.UseBackColor = true;
            btnSave.Appearance.Options.UseForeColor = true;
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += BtnSave_Click;
            pnlForm.Controls.Add(btnSave);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Verileri yükler
        /// </summary>
        private void LoadData()
        {
            if (_currentComplaint == null) return;

            var complaint = _complaintService.GetById(_currentComplaint.Id);
            if (complaint == null) return;

            cmbStatus.SelectedItem = complaint.Status ?? "Beklemede";
            txtComment.Text = complaint.ManagerComment ?? "";
        }

        /// <summary>
        /// Kaydet butonuna tıklandığında
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cmbStatus?.SelectedItem == null)
            {
                Swal.Warning("Lütfen bir durum seçin.");
                return;
            }

            try
            {
                string status = cmbStatus.SelectedItem.ToString();
                string comment = txtComment.Text.Trim();

                string result = _complaintService.UpdateStatus(_currentComplaint.Id, status, comment);
                if (!string.IsNullOrEmpty(result))
                {
                    Swal.Error("Güncelleme hatası: " + result);
                    return;
                }

                Swal.Success("Durum başarıyla güncellendi.");
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

