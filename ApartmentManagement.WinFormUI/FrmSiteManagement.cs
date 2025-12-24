#nullable disable
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using System.Drawing;

namespace ApartmentManagement.WinFormUI
{
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
        
        // Labels
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
            this.ClientSize = new Size(850, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.Padding = new Padding(30);

            // === HEADER ROW ===
            // Title Label
            this.lblTitle = new LabelControl();
            this.lblTitle.Text = _isEditMode ? "Site Düzenle" : "Yeni Site";
            this.lblTitle.Appearance.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            this.lblTitle.Location = new Point(30, 25);
            this.lblTitle.AutoSizeMode = LabelAutoSizeMode.Default;
            this.Controls.Add(this.lblTitle);

            // Back Label
            this.lblBack = new LabelControl();
            this.lblBack.Text = "Geri";
            this.lblBack.Appearance.Font = new Font("Segoe UI", 11F);
            this.lblBack.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            this.lblBack.Cursor = Cursors.Hand;
            this.lblBack.Location = new Point(790, 32);
            this.lblBack.Click += (s, e) => this.Close();
            this.Controls.Add(this.lblBack);

            int leftColumn = 30;
            int rightColumn = 440;
            int fieldWidth = 380;
            int currentY = 80;
            int rowHeight = 70;

            // === ROW 1: Ad & Kod ===
            CreateLabel("Ad", leftColumn, currentY);
            this.txtName = CreateTextEdit(leftColumn, currentY + 22, fieldWidth);
            
            CreateLabel("Kod", rightColumn, currentY);
            this.txtCode = CreateTextEdit(rightColumn, currentY + 22, fieldWidth);

            currentY += rowHeight;

            // === ROW 2: Adres ===
            CreateLabel("Adres", leftColumn, currentY);
            this.txtAddress = new MemoEdit();
            this.txtAddress.Location = new Point(leftColumn, currentY + 22);
            this.txtAddress.Size = new Size(790, 70);
            this.txtAddress.Properties.Appearance.Font = new Font("Segoe UI", 10F);
            this.txtAddress.Properties.Appearance.Options.UseFont = true;
            this.Controls.Add(this.txtAddress);

            currentY += 100;

            // === ROW 3: Açıklama ===
            CreateLabel("Açıklama", leftColumn, currentY);
            this.txtDescription = new MemoEdit();
            this.txtDescription.Location = new Point(leftColumn, currentY + 22);
            this.txtDescription.Size = new Size(790, 70);
            this.txtDescription.Properties.Appearance.Font = new Font("Segoe UI", 10F);
            this.txtDescription.Properties.Appearance.Options.UseFont = true;
            this.Controls.Add(this.txtDescription);

            currentY += 100;

            // === ROW 4: Durum, Toplam Blok, Blok Başına Apartman, Apartman Başına Kat, Kat Başına Daire ===
            int smallFieldWidth = 145;
            int smallGap = 163;

            // Durum
            CreateLabel("Durum", leftColumn, currentY);
            this.cmbStatus = new ComboBoxEdit();
            this.cmbStatus.Location = new Point(leftColumn, currentY + 22);
            this.cmbStatus.Size = new Size(smallFieldWidth, 28);
            this.cmbStatus.Properties.Items.AddRange(new string[] { "Aktif", "Pasif" });
            this.cmbStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbStatus.Properties.Appearance.Font = new Font("Segoe UI", 10F);
            this.cmbStatus.EditValue = "Aktif";
            this.Controls.Add(this.cmbStatus);

            // Toplam Blok
            CreateLabel("Toplam Blok", leftColumn + smallGap, currentY);
            this.spnTotalBlocks = CreateSpinEdit(leftColumn + smallGap, currentY + 22, smallFieldWidth);

            // Blok Başına Apartman
            CreateLabel("Blok Başına Apartman", leftColumn + smallGap * 2, currentY);
            this.spnApartmentsPerBlock = CreateSpinEdit(leftColumn + smallGap * 2, currentY + 22, smallFieldWidth);

            // Apartman Başına Kat
            CreateLabel("Apartman Başına Kat", leftColumn + smallGap * 3, currentY);
            this.spnFloorsPerApartment = CreateSpinEdit(leftColumn + smallGap * 3, currentY + 22, smallFieldWidth);

            // Kat Başına Daire
            CreateLabel("Kat Başına Daire", leftColumn + smallGap * 4, currentY);
            this.spnFlatsPerFloor = CreateSpinEdit(leftColumn + smallGap * 4, currentY + 22, smallFieldWidth);

            currentY += rowHeight + 20;

            // === SAVE BUTTON ===
            this.btnSave = new SimpleButton();
            this.btnSave.Text = "Kaydet";
            this.btnSave.Size = new Size(100, 40);
            this.btnSave.Location = new Point(720, currentY);
            this.btnSave.Appearance.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.btnSave.Appearance.ForeColor = Color.White;
            this.btnSave.Appearance.BackColor = Color.FromArgb(66, 133, 244);
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.Appearance.Options.UseForeColor = true;
            this.btnSave.Appearance.Options.UseBackColor = true;
            this.btnSave.Cursor = Cursors.Hand;
            this.btnSave.Click += BtnSave_Click;
            this.Controls.Add(this.btnSave);

            this.ResumeLayout(false);
        }

        private LabelControl CreateLabel(string text, int x, int y)
        {
            var label = new LabelControl();
            label.Text = text;
            label.Location = new Point(x, y);
            label.Appearance.Font = new Font("Segoe UI", 9F);
            label.Appearance.ForeColor = Color.FromArgb(80, 80, 80);
            this.Controls.Add(label);
            return label;
        }

        private TextEdit CreateTextEdit(int x, int y, int width)
        {
            var txt = new TextEdit();
            txt.Location = new Point(x, y);
            txt.Size = new Size(width, 28);
            txt.Properties.Appearance.Font = new Font("Segoe UI", 10F);
            txt.Properties.Appearance.Options.UseFont = true;
            this.Controls.Add(txt);
            return txt;
        }

        private SpinEdit CreateSpinEdit(int x, int y, int width)
        {
            var spin = new SpinEdit();
            spin.Location = new Point(x, y);
            spin.Size = new Size(width, 28);
            spin.Properties.MinValue = 0;
            spin.Properties.MaxValue = 100;
            spin.Properties.IsFloatValue = false;
            spin.Properties.Appearance.Font = new Font("Segoe UI", 10F);
            spin.Properties.Appearance.Options.UseFont = true;
            spin.EditValue = 0;
            this.Controls.Add(spin);
            return spin;
        }

        private void LoadData()
        {
            if (_isEditMode && _currentSite != null)
            {
                txtName.Text = _currentSite.Name ?? "";
                txtCode.Text = _currentSite.Code ?? "";
                txtAddress.Text = _currentSite.Address ?? "";
                txtDescription.Text = _currentSite.Description ?? "";
                cmbStatus.EditValue = _currentSite.Status ?? "Aktif";
                spnTotalBlocks.EditValue = _currentSite.TotalBlocks;
                spnApartmentsPerBlock.EditValue = _currentSite.ApartmentsPerBlock;
                spnFloorsPerApartment.EditValue = _currentSite.FloorsPerApartment;
                spnFlatsPerFloor.EditValue = _currentSite.FlatsPerFloor;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                Swal.Warning("Site adı zorunludur.");
                return;
            }

            try
            {
                if (_isEditMode)
                {
                    _currentSite.Name = txtName.Text;
                    _currentSite.Code = txtCode.Text;
                    _currentSite.Address = txtAddress.Text;
                    _currentSite.Description = txtDescription.Text;
                    _currentSite.Status = cmbStatus.EditValue?.ToString() ?? "Aktif";
                    _currentSite.TotalBlocks = Convert.ToInt32(spnTotalBlocks.EditValue);
                    _currentSite.ApartmentsPerBlock = Convert.ToInt32(spnApartmentsPerBlock.EditValue);
                    _currentSite.FloorsPerApartment = Convert.ToInt32(spnFloorsPerApartment.EditValue);
                    _currentSite.FlatsPerFloor = Convert.ToInt32(spnFlatsPerFloor.EditValue);
                    
                    _currentSite.FlatsPerFloor = Convert.ToInt32(spnFlatsPerFloor.EditValue);
                    
                    string res = _siteService.Update(_currentSite);
                    if (!string.IsNullOrEmpty(res))
                    {
                        Swal.Error("Güncelleme hatası: " + res);
                        return;
                    }
                    Swal.Success("Site başarıyla güncellendi.");
                }
                else
                {
                    var newSite = new Site
                    {
                        Name = txtName.Text,
                        Code = txtCode.Text,
                        Address = txtAddress.Text,
                        Description = txtDescription.Text,
                        Status = cmbStatus.EditValue?.ToString() ?? "Aktif",
                        TotalBlocks = Convert.ToInt32(spnTotalBlocks.EditValue),
                        ApartmentsPerBlock = Convert.ToInt32(spnApartmentsPerBlock.EditValue),
                        FloorsPerApartment = Convert.ToInt32(spnFloorsPerApartment.EditValue),
                        FlatsPerFloor = Convert.ToInt32(spnFlatsPerFloor.EditValue)
                    };
                    
                    string res = _siteService.Add(newSite);
                    if (!string.IsNullOrEmpty(res))
                    {
                        Swal.Error("Ekleme hatası: " + res);
                        return;
                    }
                    Swal.Success("Site başarıyla eklendi.");
                }
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Swal.Error("Kayıt sırasında hata oluştu: " + ex.Message);
            }
        }
    }
}
