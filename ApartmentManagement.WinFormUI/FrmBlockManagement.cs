#nullable disable
// FrmBlockManagement.cs
// Blok Yönetimi Formu - Blok ekleme/düzenleme işlemleri
// Standart: Tahoma 8.25pt, AutoScroll = true
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using System.Drawing;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Blok yönetimi formu
    /// </summary>
    public partial class FrmBlockManagement : DevExpress.XtraEditors.XtraForm
    {
        private IBlock _blockService;
        private ISite _siteService;
        private Block _currentBlock;
        private bool _isEditMode = false;

        // Input Fields
        private LookUpEdit lueSite;
        private TextEdit txtName;
        private ComboBoxEdit cmbStatus;
        private SpinEdit spnFlatsPerFloor;
        
        // Labels
        private LabelControl lblTitle;
        private LabelControl lblBack;
        private LabelControl lblInfo;
        private SimpleButton btnSave;

        public FrmBlockManagement(Block block = null)
        {
            _blockService = new SBlock();
            _siteService = new SSite();
            _currentBlock = block;
            _isEditMode = block != null;
            
            InitializeComponent();
            LoadSites();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings - Standart: Max 770x700, AutoScroll = true
            this.Text = _isEditMode ? "Blok Düzenle" : "Yeni Blok";
            this.ClientSize = new Size(700, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.Padding = new Padding(30);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            int leftColumn = 30;
            int rightColumn = 360;
            int fieldWidth = 300;
            int currentY = 80;
            int rowHeight = 70;

            // === HEADER ROW ===
            // Title Label
            this.lblTitle = new LabelControl();
            this.lblTitle.Text = _isEditMode ? "Blok Düzenle" : "Yeni Blok";
            this.lblTitle.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            this.lblTitle.Location = new Point(30, 25);
            this.Controls.Add(this.lblTitle);

            // Back Label
            this.lblBack = new LabelControl();
            this.lblBack.Text = "Geri";
            this.lblBack.Appearance.Font = new Font("Tahoma", 8.25F);
            this.lblBack.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            this.lblBack.Cursor = Cursors.Hand;
            this.lblBack.Location = new Point(640, 32);
            this.lblBack.Click += (s, e) => this.Close();
            this.Controls.Add(this.lblBack);

            // === ROW 1: Site & Ad ===
            CreateLabel("Site", leftColumn, currentY);
            this.lueSite = new LookUpEdit();
            this.lueSite.Location = new Point(leftColumn, currentY + 22);
            this.lueSite.Size = new Size(fieldWidth, 28);
            this.lueSite.Properties.Appearance.Font = new Font("Tahoma", 10F);
            this.lueSite.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { 
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) 
            });
            this.lueSite.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { 
                new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Site Adı") 
            });
            this.lueSite.Properties.DisplayMember = "Name";
            this.lueSite.Properties.ValueMember = "Id";
            this.lueSite.Properties.NullText = "Seçiniz";
            this.Controls.Add(this.lueSite);

            CreateLabel("Ad", rightColumn, currentY);
            this.txtName = CreateTextEdit(rightColumn, currentY + 22, fieldWidth);

            currentY += rowHeight;

            // === ROW 2: Durum & Kat Başına Daire ===
            CreateLabel("Durum", leftColumn, currentY);
            this.cmbStatus = new ComboBoxEdit();
            this.cmbStatus.Location = new Point(leftColumn, currentY + 22);
            this.cmbStatus.Size = new Size(180, 28);
            this.cmbStatus.Properties.Items.AddRange(new string[] { "Aktif", "Pasif" });
            this.cmbStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbStatus.Properties.Appearance.Font = new Font("Tahoma", 10F);
            this.cmbStatus.EditValue = "Aktif";
            this.Controls.Add(this.cmbStatus);

            CreateLabel("Kat Başına Daire", leftColumn + 200, currentY);
            this.spnFlatsPerFloor = CreateSpinEdit(leftColumn + 200, currentY + 22, 120);

            currentY += rowHeight;

            // === INFO LABEL (altta) ===
            this.lblInfo = new LabelControl();
            this.lblInfo.Text = "Not: Seçilen sitedeki ayarlardan toplamlar otomatik hesaplanır.";
            this.lblInfo.Appearance.Font = new Font("Tahoma", 8.5F, FontStyle.Italic);
            this.lblInfo.Appearance.ForeColor = Color.FromArgb(140, 140, 140);
            this.lblInfo.Location = new Point(leftColumn, currentY);
            this.Controls.Add(this.lblInfo);

            currentY += 35;

            // === SAVE BUTTON ===
            this.btnSave = new SimpleButton();
            this.btnSave.Text = "Kaydet";
            this.btnSave.Size = new Size(100, 40);
            this.btnSave.Location = new Point(570, currentY);
            this.btnSave.Appearance.Font = new Font("Tahoma", 11F, FontStyle.Bold);
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
            label.Appearance.Font = new Font("Tahoma", 9F);
            label.Appearance.ForeColor = Color.FromArgb(80, 80, 80);
            this.Controls.Add(label);
            return label;
        }

        private TextEdit CreateTextEdit(int x, int y, int width)
        {
            var txt = new TextEdit();
            txt.Location = new Point(x, y);
            txt.Size = new Size(width, 28);
            txt.Properties.Appearance.Font = new Font("Tahoma", 10F);
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
            spin.Properties.Appearance.Font = new Font("Tahoma", 10F);
            spin.Properties.Appearance.Options.UseFont = true;
            spin.EditValue = 0;
            this.Controls.Add(spin);
            return spin;
        }

        private void LoadSites()
        {
            var sites = _siteService.GetAll();
            lueSite.Properties.DataSource = sites;
        }

        private void LoadData()
        {
            if (_isEditMode && _currentBlock != null)
            {
                lueSite.EditValue = _currentBlock.SiteId;
                txtName.Text = _currentBlock.Name ?? "";
                cmbStatus.EditValue = _currentBlock.Status ?? "Aktif";
                spnFlatsPerFloor.EditValue = _currentBlock.FlatsPerFloor;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (lueSite.EditValue == null)
            {
                Swal.Warning("Lütfen bir site seçin.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                Swal.Warning("Blok adı zorunludur.");
                return;
            }

            try
            {
                int siteId = (int)lueSite.EditValue;
                var site = _siteService.GetAll().FirstOrDefault(s => s.Id == siteId);

                // Calculate totals from site configuration
                int totalApartments = site?.ApartmentsPerBlock ?? 0;
                int totalFloors = site?.FloorsPerApartment ?? 0;
                int flatsPerFloor = Convert.ToInt32(spnFlatsPerFloor.EditValue);
                int totalFlats = totalFloors * flatsPerFloor;

                if (_isEditMode)
                {
                    _currentBlock.SiteId = siteId;
                    _currentBlock.Name = txtName.Text;
                    _currentBlock.Status = cmbStatus.EditValue?.ToString() ?? "Aktif";
                    _currentBlock.TotalApartments = totalApartments;
                    _currentBlock.TotalFloors = totalFloors;
                    _currentBlock.FlatsPerFloor = flatsPerFloor;
                    _currentBlock.TotalFlats = totalFlats;
                    
                    _currentBlock.TotalFlats = totalFlats;
                    
                    string res = _blockService.Update(_currentBlock);
                    if (!string.IsNullOrEmpty(res))
                    {
                        Swal.Error("Güncelleme hatası: " + res);
                        return;
                    }
                    Swal.Success("Blok başarıyla güncellendi.");
                }
                else
                {
                    var newBlock = new Block
                    {
                        SiteId = siteId,
                        Name = txtName.Text,
                        Status = cmbStatus.EditValue?.ToString() ?? "Aktif",
                        TotalApartments = totalApartments,
                        TotalFloors = totalFloors,
                        FlatsPerFloor = flatsPerFloor,
                        TotalFlats = totalFlats
                    };
                    
                    string res = _blockService.Add(newBlock);
                    if (!string.IsNullOrEmpty(res))
                    {
                        Swal.Error("Ekleme hatası: " + res);
                        return;
                    }
                    Swal.Success("Blok başarıyla eklendi.");
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
