#nullable disable
// FrmSiteManagement.cs
// Site Yönetimi Formu - Site ekleme/düzenleme işlemleri
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
    /// Site yönetimi formu
    /// </summary>
    public partial class FrmSiteManagement : DevExpress.XtraEditors.XtraForm
    {
        private ISite _siteService;
        private Site _currentSite;
        private bool _isEditMode = false;

        // Input Fields
        private TextEdit txtName;
        private TextEdit txtCode;
        private MemoEdit txtAddress;
        private MemoEdit txtDescription;
        private ComboBoxEdit cmbStatus;
        private SpinEdit spnTotalBlocks;
        private SpinEdit spnApartmentsPerBlock;
        private SpinEdit spnFloorsPerApartment;
        private SpinEdit spnFlatsPerFloor;
        
        // Labels and Buttons
        private LabelControl lblTitle;
        private LabelControl lblBack;
        private SimpleButton btnSave;

        public FrmSiteManagement(Site site = null)
        {
            _siteService = new SSite();
            _currentSite = site;
            _isEditMode = site != null;
            
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = _isEditMode ? "Site Düzenle" : "Yeni Site";
            this.ClientSize = new Size(800, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            int leftMargin = 30;
            int fieldWidth = 350;
            int fieldWidthSmall = 160;
            int labelOffset = 0;
            int fieldOffset = 22;
            int rowHeight = 75;
            int currentY = 70;

            // === HEADER ===
            this.lblTitle = new LabelControl();
            this.lblTitle.Text = _isEditMode ? "Site Düzenle" : "Yeni Site";
            this.lblTitle.Appearance.Font = new Font("Tahoma", 14F, FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            this.lblTitle.Location = new Point(leftMargin, 20);
            this.Controls.Add(this.lblTitle);

            this.lblBack = new LabelControl();
            this.lblBack.Text = "Geri";
            this.lblBack.Appearance.Font = new Font("Tahoma", 11F);
            this.lblBack.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            this.lblBack.Cursor = Cursors.Hand;
            this.lblBack.Location = new Point(this.Width - 80, 25);
            this.lblBack.Click += (s, e) => this.Close();
            this.Controls.Add(this.lblBack);

            // === ROW 1: Ad (Name) & Kod (Code) ===
            AddLabel("Ad", leftMargin, currentY + labelOffset);
            this.txtName = CreateTextEdit(leftMargin, currentY + fieldOffset, fieldWidthSmall);
            
            AddLabel("Kod", leftMargin + fieldWidthSmall + 20, currentY + labelOffset);
            this.txtCode = CreateTextEdit(leftMargin + fieldWidthSmall + 20, currentY + fieldOffset, fieldWidthSmall);
            
            currentY += rowHeight;

            // === ROW 2: Adres (Address) ===
            AddLabel("Adres", leftMargin, currentY + labelOffset);
            this.txtAddress = CreateMemoEdit(leftMargin, currentY + fieldOffset, fieldWidth * 2 + 20, 80);
            
            currentY += 120;

            // === ROW 3: Açıklama (Description) ===
            AddLabel("Açıklama", leftMargin, currentY + labelOffset);
            this.txtDescription = CreateMemoEdit(leftMargin, currentY + fieldOffset, fieldWidth * 2 + 20, 80);
            
            currentY += 120;

            // === ROW 4: Durum (Status) & Toplam Blok (Total Blocks) ===
            AddLabel("Durum", leftMargin, currentY + labelOffset);
            this.cmbStatus = CreateComboBoxEdit(leftMargin, currentY + fieldOffset, fieldWidthSmall);
            this.cmbStatus.Properties.Items.AddRange(new[] { "Aktif", "Pasif" });
            this.cmbStatus.EditValue = "Aktif";
            
            AddLabel("Toplam Blok", leftMargin + fieldWidthSmall + 20, currentY + labelOffset);
            this.spnTotalBlocks = CreateSpinEdit(leftMargin + fieldWidthSmall + 20, currentY + fieldOffset, fieldWidthSmall);
            
            currentY += rowHeight;

            // === ROW 5: Blok Başına Apartman & Apartman Başına Kat ===
            AddLabel("Blok Başına Apartman", leftMargin, currentY + labelOffset);
            this.spnApartmentsPerBlock = CreateSpinEdit(leftMargin, currentY + fieldOffset, fieldWidthSmall);
            
            AddLabel("Apartman Başına Kat", leftMargin + fieldWidthSmall + 20, currentY + labelOffset);
            this.spnFloorsPerApartment = CreateSpinEdit(leftMargin + fieldWidthSmall + 20, currentY + fieldOffset, fieldWidthSmall);
            
            currentY += rowHeight;

            // === ROW 6: Kat Başına Daire ===
            AddLabel("Kat Başına Daire", leftMargin, currentY + labelOffset);
            this.spnFlatsPerFloor = CreateSpinEdit(leftMargin, currentY + fieldOffset, fieldWidthSmall);
            
            currentY += 100;

            // === SAVE BUTTON ===
            this.btnSave = new SimpleButton();
            this.btnSave.Text = "Kaydet";
            this.btnSave.Size = new Size(120, 40);
            this.btnSave.Location = new Point(this.Width - 150, currentY);
            this.btnSave.Appearance.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            this.btnSave.Appearance.BackColor = Color.FromArgb(59, 130, 246); // Blue
            this.btnSave.Appearance.ForeColor = Color.White;
            this.btnSave.Appearance.Options.UseBackColor = true;
            this.btnSave.Appearance.Options.UseForeColor = true;
            this.btnSave.Cursor = Cursors.Hand;
            this.btnSave.Click += BtnSave_Click;
            this.Controls.Add(this.btnSave);

            this.ResumeLayout(false);
        }

        private LabelControl AddLabel(string text, int x, int y)
        {
            var label = new LabelControl();
            label.Text = text;
            label.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            label.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            label.Location = new Point(x, y);
            this.Controls.Add(label);
            return label;
        }

        private TextEdit CreateTextEdit(int x, int y, int width)
        {
            var txt = new TextEdit();
            txt.Location = new Point(x, y);
            txt.Size = new Size(width, 32);
            txt.Properties.Appearance.Font = new Font("Tahoma", 10F);
            txt.Properties.Appearance.Options.UseFont = true;
            txt.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.Controls.Add(txt);
            return txt;
        }

        private MemoEdit CreateMemoEdit(int x, int y, int width, int height)
        {
            var memo = new MemoEdit();
            memo.Location = new Point(x, y);
            memo.Size = new Size(width, height);
            memo.Properties.Appearance.Font = new Font("Tahoma", 10F);
            memo.Properties.Appearance.Options.UseFont = true;
            memo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.Controls.Add(memo);
            return memo;
        }

        private ComboBoxEdit CreateComboBoxEdit(int x, int y, int width)
        {
            var cmb = new ComboBoxEdit();
            cmb.Location = new Point(x, y);
            cmb.Size = new Size(width, 32);
            cmb.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmb.Properties.Appearance.Font = new Font("Tahoma", 10F);
            cmb.Properties.Appearance.Options.UseFont = true;
            cmb.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.Controls.Add(cmb);
            return cmb;
        }

        private SpinEdit CreateSpinEdit(int x, int y, int width)
        {
            var spin = new SpinEdit();
            spin.Location = new Point(x, y);
            spin.Size = new Size(width, 32);
            spin.Properties.MinValue = 0;
            spin.Properties.MaxValue = 1000;
            spin.Properties.IsFloatValue = false;
            spin.Properties.Appearance.Font = new Font("Tahoma", 10F);
            spin.Properties.Appearance.Options.UseFont = true;
            spin.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            spin.EditValue = 0;
            this.Controls.Add(spin);
            return spin;
        }

        private void LoadData()
        {
            if (_isEditMode && _currentSite != null)
            {
                this.txtName.Text = _currentSite.Name ?? "";
                this.txtCode.Text = _currentSite.Code ?? "";
                this.txtAddress.Text = _currentSite.Address ?? "";
                this.txtDescription.Text = _currentSite.Description ?? "";
                this.cmbStatus.EditValue = _currentSite.Status ?? "Aktif";
                this.spnTotalBlocks.EditValue = _currentSite.TotalBlocks;
                this.spnApartmentsPerBlock.EditValue = _currentSite.ApartmentsPerBlock;
                this.spnFloorsPerApartment.EditValue = _currentSite.FloorsPerApartment;
                this.spnFlatsPerFloor.EditValue = _currentSite.FlatsPerFloor;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                Swal.Warning("Site adı zorunludur.");
                txtName.Focus();
                return;
            }

            try
            {
                Site site;
                if (_isEditMode && _currentSite != null)
                {
                    site = _currentSite;
                }
                else
                {
                    site = new Site();
                }

                site.Name = txtName.Text.Trim();
                site.Code = txtCode.Text.Trim();
                site.Address = txtAddress.Text.Trim();
                site.Description = txtDescription.Text.Trim();
                site.Status = cmbStatus.EditValue?.ToString() ?? "Aktif";
                site.TotalBlocks = Convert.ToInt32(spnTotalBlocks.EditValue ?? 0);
                site.ApartmentsPerBlock = Convert.ToInt32(spnApartmentsPerBlock.EditValue ?? 0);
                site.FloorsPerApartment = Convert.ToInt32(spnFloorsPerApartment.EditValue ?? 0);
                site.FlatsPerFloor = Convert.ToInt32(spnFlatsPerFloor.EditValue ?? 0);

                string result;
                if (_isEditMode)
                {
                    result = _siteService.Update(site);
                    if (string.IsNullOrEmpty(result))
                    {
                        Swal.Success("Site başarıyla güncellendi.");
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        Swal.Error("Güncelleme başarısız: " + result);
                    }
                }
                else
                {
                    result = _siteService.Add(site);
                    if (string.IsNullOrEmpty(result))
                    {
                        Swal.Success("Site başarıyla eklendi.");
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        Swal.Error("Ekleme başarısız: " + result);
                    }
                }
            }
            catch (Exception ex)
            {
                Swal.Error("İşlem sırasında hata oluştu: " + ex.Message);
            }
        }
    }
}

