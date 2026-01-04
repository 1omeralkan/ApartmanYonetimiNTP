#nullable disable
// FrmComplaintManagement.cs
// Şikayet/Talep Yönetim Formu - Şikayet/talep ekleme ve düzenleme
// Standart: Tahoma 8.25pt, AutoScroll = true
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
    /// <summary>
    /// Şikayet/Talep yönetim formu - Şikayet/talep ekleme ve görüntüleme
    /// </summary>
    public partial class FrmComplaintManagement : DevExpress.XtraEditors.XtraForm
    {
        private IComplaint _complaintService;
        private IFlat _flatService;
        private User _currentUser;
        private Complaint _currentComplaint;
        private bool _isEditMode;

        // Controls
        private ComboBoxEdit cmbType;
        private TextEdit txtTitle;
        private MemoEdit txtDescription;
        private SimpleButton btnSave;
        private SimpleButton btnCancel;
        private LabelControl lblManagerComment;

        /// <summary>
        /// FrmComplaintManagement constructor
        /// </summary>
        public FrmComplaintManagement(Complaint complaint, User user)
        {
            _currentComplaint = complaint;
            _currentUser = user;
            _isEditMode = complaint != null;
            _complaintService = new SComplaint();
            _flatService = new SFlat();
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
            this.Text = _isEditMode ? "Şikayet/Talep Detayı" : "Yeni Şikayet/Talep";
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            int formHeight = Math.Min((int)(screenHeight * 0.8), 800);
            this.ClientSize = new Size(900, formHeight);
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
            int fieldHeight = 32;

            // ========== HEADER SECTION ==========
            var lblTitle = new LabelControl();
            lblTitle.Text = _isEditMode ? "Şikayet/Talep Detayı" : "Yeni Şikayet/Talep";
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblTitle);

            var lblSubtitle = new LabelControl();
            lblSubtitle.Text = _isEditMode ? "Şikayet/talep detaylarını görüntüleyin" : "Yeni bir şikayet/talep oluşturun";
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
            pnlForm.Size = new Size(contentWidth, _isEditMode ? 700 : 600);
            pnlForm.Location = new Point(leftMargin, currentY);
            pnlForm.BackColor = Color.White;
            pnlForm.BorderRadius = 12;
            pnlForm.BorderThickness = 1;
            pnlForm.BorderColor = Color.FromArgb(226, 232, 240);
            pnlForm.Padding = new Padding(35);
            this.Controls.Add(pnlForm);

            int panelY = 30;
            int panelLeft = 35;

            // ========== ŞİKAYET/TALEP BİLGİLERİ SECTION ==========
            var lblSectionComplaint = new LabelControl();
            lblSectionComplaint.Text = "Şikayet/Talep Bilgileri";
            lblSectionComplaint.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblSectionComplaint.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblSectionComplaint.Location = new Point(panelLeft, panelY);
            pnlForm.Controls.Add(lblSectionComplaint);
            panelY += 40;

            // Type (sadece yeni eklemede)
            if (!_isEditMode)
            {
                AddFieldLabel(pnlForm, "Tip *", panelLeft, panelY, true);
                panelY += 20;
                cmbType = new ComboBoxEdit();
                cmbType.Location = new Point(panelLeft, panelY);
                cmbType.Size = new Size(contentWidth - 70, fieldHeight);
                cmbType.Properties.Items.AddRange(new[] { "Şikayet", "Talep" });
                cmbType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                cmbType.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
                cmbType.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
                cmbType.Properties.NullText = "Tip seçiniz";
                pnlForm.Controls.Add(cmbType);
                panelY += 45;
            }

            // Title
            AddFieldLabel(pnlForm, "Başlık *", panelLeft, panelY, true);
            panelY += 20;
            txtTitle = new TextEdit();
            txtTitle.Location = new Point(panelLeft, panelY);
            txtTitle.Size = new Size(contentWidth - 70, fieldHeight);
            txtTitle.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            txtTitle.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            txtTitle.Properties.NullText = "Başlık";
            txtTitle.Enabled = !_isEditMode; // Edit modda sadece görüntüleme
            pnlForm.Controls.Add(txtTitle);
            panelY += 45;

            // Description
            AddFieldLabel(pnlForm, "Açıklama *", panelLeft, panelY, true);
            panelY += 20;
            txtDescription = new MemoEdit();
            txtDescription.Location = new Point(panelLeft, panelY);
            txtDescription.Size = new Size(contentWidth - 70, 150);
            txtDescription.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            txtDescription.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            txtDescription.Properties.NullText = "Detaylı açıklama";
            txtDescription.Enabled = !_isEditMode; // Edit modda sadece görüntüleme
            pnlForm.Controls.Add(txtDescription);
            panelY += 170;

            // Edit modda yönetici yorumunu göster
            if (_isEditMode)
            {
                AddFieldLabel(pnlForm, "Yönetici Yorumu", panelLeft, panelY, false);
                panelY += 20;
                lblManagerComment = new LabelControl();
                lblManagerComment.Location = new Point(panelLeft, panelY);
                lblManagerComment.Size = new Size(contentWidth - 70, 100);
                lblManagerComment.Appearance.Font = new Font("Tahoma", 8.5F);
                lblManagerComment.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
                lblManagerComment.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                pnlForm.Controls.Add(lblManagerComment);
                panelY += 120;
            }

            // ========== BUTTONS ==========
            btnCancel = new SimpleButton();
            btnCancel.Text = _isEditMode ? "Kapat" : "İptal";
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

            if (!_isEditMode)
            {
                btnSave = new SimpleButton();
                btnSave.Text = "Kaydet";
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
            }

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
        /// Verileri yükler
        /// </summary>
        private void LoadData()
        {
            if (_currentComplaint == null) return;

            var complaint = _complaintService.GetById(_currentComplaint.Id);
            if (complaint == null) return;

            txtTitle.Text = complaint.Title ?? "";
            txtDescription.Text = complaint.Description ?? "";
            
            if (lblManagerComment != null)
            {
                lblManagerComment.Text = !string.IsNullOrEmpty(complaint.ManagerComment) 
                    ? complaint.ManagerComment 
                    : "Henüz yorum yapılmadı.";
            }
        }

        /// <summary>
        /// Kaydet butonuna tıklandığında
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validation
            if (cmbType?.SelectedItem == null)
            {
                Swal.Warning("Lütfen bir tip seçin.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                Swal.Warning("Lütfen başlık girin.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                Swal.Warning("Lütfen açıklama girin.");
                return;
            }

            try
            {
                // ApartmentManager için otomatik apartman
                int? apartmentId = null;
                if (_currentUser?.Role == "ApartmentManager")
                {
                    var userFlat = _flatService.GetResidentFlat(_currentUser.Id);
                    if (userFlat != null)
                    {
                        apartmentId = userFlat.ApartmentId;
                    }
                }

                // Add
                var newComplaint = new Complaint
                {
                    Type = cmbType.SelectedItem.ToString(),
                    Title = txtTitle.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Status = "Beklemede",
                    ApartmentId = apartmentId,
                    CreatedByUserId = _currentUser.Id
                };

                string result = _complaintService.Add(newComplaint);
                if (!string.IsNullOrEmpty(result))
                {
                    // Hata mesajını tam göster
                    string fullError = "Ekleme hatası: " + result;
                    if (fullError.Length > 500)
                    {
                        fullError = fullError.Substring(0, 500) + "...";
                    }
                    Swal.Error(fullError);
                    return;
                }

                Swal.Success("Şikayet/Talep başarıyla oluşturuldu.");
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

