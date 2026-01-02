#nullable disable
// FrmApartmentManagement.cs
// Apartman Yönetimi Formu - Apartman ekleme/düzenleme işlemleri
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
    /// Apartman yönetimi formu
    /// </summary>
    public partial class FrmApartmentManagement : DevExpress.XtraEditors.XtraForm
    {
        private IApartment _apartmentService;
        private ISite _siteService;
        private IBlock _blockService;
        private IFlat _flatService;
        private Apartment _currentApartment;
        private bool _isEditMode = false;

        // Input Fields
        private LookUpEdit lueSite;
        private LookUpEdit lueBlock;
        private TextEdit txtName;
        private TextEdit txtAddress;
        private ComboBoxEdit cmbStatus;
        private SpinEdit spnFlatsPerFloor;
        private SpinEdit spnTotalFloors;
        private ComboBoxEdit cmbDefaultFlatType;
        private TextEdit txtDefaultMonthlyDue;
        private ComboBoxEdit cmbHasBalcony;
        private TextEdit txtDefaultGrossArea;
        private TextEdit txtDefaultNetArea;
        private ComboBoxEdit cmbHasElevator;
        private ComboBoxEdit cmbHasParking;
        
        private LabelControl lblTitle;
        private SimpleButton btnSave;

        public FrmApartmentManagement(Apartment apartment = null)
        {
            _apartmentService = new SApartment();
            _siteService = new SSite();
            _blockService = new SBlock();
            _flatService = new SFlat();
            _currentApartment = apartment;
            _isEditMode = apartment != null;
            
            InitializeComponent();
            LoadSites();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings - Standart: Max 770x700, AutoScroll = true
            this.Text = _isEditMode ? "Apartman Düzenle" : "Yeni Apartman";
            this.ClientSize = new Size(750, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            int col1 = 30;
            int col2 = 270;
            int col3 = 510;
            int labelOffset = 0;
            int fieldOffset = 22;
            int rowHeight = 75;
            int fieldWidth = 210;
            int currentY = 70;

            // === HEADER ===
            this.lblTitle = new LabelControl();
            this.lblTitle.Text = _isEditMode ? "Apartman Düzenle" : "Yeni Apartman";
            this.lblTitle.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            this.lblTitle.Location = new Point(30, 20);
            this.Controls.Add(this.lblTitle);

            var lblBack = new LabelControl();
            lblBack.Text = "Geri";
            lblBack.Appearance.Font = new Font("Tahoma", 11F);
            lblBack.Appearance.ForeColor = Color.Gray;
            lblBack.Cursor = Cursors.Hand;
            lblBack.Location = new Point(700, 25);
            lblBack.Click += (s, e) => this.Close();
            this.Controls.Add(lblBack);

            // === ROW 1: Site & Blok ===
            AddLabel("Site", col1, currentY + labelOffset);
            this.lueSite = new LookUpEdit();
            this.lueSite.Location = new Point(col1, currentY + fieldOffset);
            this.lueSite.Size = new Size(fieldWidth, 28);
            this.lueSite.Properties.Appearance.Font = new Font("Tahoma", 10F);
            this.lueSite.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Site"));
            this.lueSite.Properties.DisplayMember = "Name";
            this.lueSite.Properties.ValueMember = "Id";
            this.lueSite.Properties.NullText = "Seçiniz";
            this.lueSite.EditValueChanged += LueSite_EditValueChanged;
            this.Controls.Add(this.lueSite);

            AddLabel("Blok", col2, currentY + labelOffset);
            this.lueBlock = new LookUpEdit();
            this.lueBlock.Location = new Point(col2, currentY + fieldOffset);
            this.lueBlock.Size = new Size(fieldWidth, 28);
            this.lueBlock.Properties.Appearance.Font = new Font("Tahoma", 10F);
            this.lueBlock.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Blok"));
            this.lueBlock.Properties.DisplayMember = "Name";
            this.lueBlock.Properties.ValueMember = "Id";
            this.lueBlock.Properties.NullText = "Önce site seçiniz";
            this.Controls.Add(this.lueBlock);

            currentY += rowHeight;

            // === ROW 2: Ad & Adres ===
            AddLabel("Ad", col1, currentY + labelOffset);
            this.txtName = AddTextEdit(col1, currentY + fieldOffset, fieldWidth);

            AddLabel("Adres", col2, currentY + labelOffset);
            this.txtAddress = AddTextEdit(col2, currentY + fieldOffset, fieldWidth);

            currentY += rowHeight;

            // === ROW 3: Durum, Kat Başına Daire, Varsayılan Daire Tipi ===
            AddLabel("Durum", col1, currentY + labelOffset);
            this.cmbStatus = AddComboBox(col1, currentY + fieldOffset, 130, new[] { "Aktif", "Pasif" }, "Aktif");

            AddLabel("Kat Başına Daire", col2, currentY + labelOffset);
            this.spnFlatsPerFloor = AddSpinEdit(col2, currentY + fieldOffset, 100);

            AddLabel("Varsayılan Daire Tipi", col3, currentY + labelOffset);
            this.cmbDefaultFlatType = AddComboBox(col3, currentY + fieldOffset, 130, new[] { "1+0", "1+1", "2+1", "3+1", "4+1" }, "2+1");

            currentY += rowHeight;

            // === ROW 4: Varsayılan Aylık Aidat, Balkon, Brüt Alan ===
            AddLabel("Varsayılan Aylık Aidat (₺)", col1, currentY + labelOffset);
            this.txtDefaultMonthlyDue = AddTextEdit(col1, currentY + fieldOffset, 130);

            AddLabel("Balkon", col2, currentY + labelOffset);
            this.cmbHasBalcony = AddComboBox(col2, currentY + fieldOffset, 100, new[] { "Yok", "Var" }, "Yok");

            AddLabel("Varsayılan Brüt Alan (m²)", col3, currentY + labelOffset);
            this.txtDefaultGrossArea = AddTextEdit(col3, currentY + fieldOffset, 130);

            currentY += rowHeight;

            // === ROW 5: Net Alan, Asansör, Otopark ===
            AddLabel("Varsayılan Net Alan (m²)", col1, currentY + labelOffset);
            this.txtDefaultNetArea = AddTextEdit(col1, currentY + fieldOffset, 130);

            AddLabel("Asansör", col2, currentY + labelOffset);
            this.cmbHasElevator = AddComboBox(col2, currentY + fieldOffset, 100, new[] { "Yok", "Var" }, "Yok");

            AddLabel("Otopark", col3, currentY + labelOffset);
            this.cmbHasParking = AddComboBox(col3, currentY + fieldOffset, 100, new[] { "Yok", "Var" }, "Yok");

            currentY += rowHeight + 10;

            // === SAVE BUTTON ===
            this.btnSave = new SimpleButton();
            this.btnSave.Text = "Kaydet";
            this.btnSave.Size = new Size(100, 40);
            this.btnSave.Location = new Point(620, currentY);
            this.btnSave.Appearance.Font = new Font("Tahoma", 11F, FontStyle.Bold);
            this.btnSave.Appearance.BackColor = Color.FromArgb(66, 133, 244);
            this.btnSave.Appearance.ForeColor = Color.White;
            this.btnSave.Appearance.Options.UseBackColor = true;
            this.btnSave.Appearance.Options.UseForeColor = true;
            this.btnSave.Cursor = Cursors.Hand;
            this.btnSave.Click += BtnSave_Click;
            this.Controls.Add(this.btnSave);

            this.ResumeLayout(false);
        }

        private void AddLabel(string text, int x, int y)
        {
            var lbl = new LabelControl();
            lbl.Text = text;
            lbl.Appearance.Font = new Font("Tahoma", 9F);
            lbl.Appearance.ForeColor = Color.FromArgb(80, 80, 80);
            lbl.Location = new Point(x, y);
            this.Controls.Add(lbl);
        }

        private TextEdit AddTextEdit(int x, int y, int width)
        {
            var txt = new TextEdit();
            txt.Location = new Point(x, y);
            txt.Size = new Size(width, 28);
            txt.Properties.Appearance.Font = new Font("Tahoma", 10F);
            txt.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.Controls.Add(txt);
            return txt;
        }

        private SpinEdit AddSpinEdit(int x, int y, int width)
        {
            var spn = new SpinEdit();
            spn.Location = new Point(x, y);
            spn.Size = new Size(width, 28);
            spn.Properties.Appearance.Font = new Font("Tahoma", 10F);
            spn.Properties.MinValue = 0;
            spn.Properties.MaxValue = 1000;
            spn.EditValue = 2;
            this.Controls.Add(spn);
            return spn;
        }

        private ComboBoxEdit AddComboBox(int x, int y, int width, string[] items, string defaultValue)
        {
            var cmb = new ComboBoxEdit();
            cmb.Location = new Point(x, y);
            cmb.Size = new Size(width, 28);
            cmb.Properties.Items.AddRange(items);
            cmb.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmb.Properties.Appearance.Font = new Font("Tahoma", 10F);
            cmb.EditValue = defaultValue;
            this.Controls.Add(cmb);
            return cmb;
        }

        private void LoadSites()
        {
            var sites = _siteService.GetAll();
            lueSite.Properties.DataSource = sites;
        }

        private void LueSite_EditValueChanged(object sender, EventArgs e)
        {
            if (lueSite.EditValue != null)
            {
                int siteId = (int)lueSite.EditValue;
                var blocks = _blockService.GetAllBySiteId(siteId);
                lueBlock.Properties.DataSource = blocks;
                lueBlock.Properties.NullText = "Blok seçiniz";
            }
            else
            {
                lueBlock.Properties.DataSource = null;
                lueBlock.Properties.NullText = "Önce site seçiniz";
            }
            lueBlock.EditValue = null;
        }

        private void LoadData()
        {
            if (_isEditMode && _currentApartment != null)
            {
                if (_currentApartment.Block != null)
                {
                    lueSite.EditValue = _currentApartment.Block.SiteId;
                    var blocks = _blockService.GetAllBySiteId(_currentApartment.Block.SiteId);
                    lueBlock.Properties.DataSource = blocks;
                    lueBlock.EditValue = _currentApartment.BlockId;
                }
                
                txtName.Text = _currentApartment.Name ?? "";
                txtAddress.Text = _currentApartment.Address ?? "";
                cmbStatus.EditValue = _currentApartment.Status ?? "Aktif";
                spnFlatsPerFloor.EditValue = _currentApartment.FlatsPerFloor;
                cmbDefaultFlatType.EditValue = _currentApartment.DefaultFlatType ?? "2+1";
                txtDefaultMonthlyDue.Text = _currentApartment.DefaultMonthlyDue.ToString("0.##");
                cmbHasBalcony.EditValue = _currentApartment.HasBalcony ? "Var" : "Yok";
                txtDefaultGrossArea.Text = _currentApartment.DefaultGrossArea.ToString("0.##");
                txtDefaultNetArea.Text = _currentApartment.DefaultNetArea.ToString("0.##");
                cmbHasElevator.EditValue = _currentApartment.HasElevator ? "Var" : "Yok";
                cmbHasParking.EditValue = _currentApartment.HasParking ? "Var" : "Yok";
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (lueBlock.EditValue == null)
            {
                Swal.Warning("Lütfen bir blok seçin.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                Swal.Warning("Apartman adı zorunludur.");
                return;
            }

            try
            {
                decimal.TryParse(txtDefaultMonthlyDue.Text, out decimal monthlyDue);
                decimal.TryParse(txtDefaultGrossArea.Text, out decimal grossArea);
                decimal.TryParse(txtDefaultNetArea.Text, out decimal netArea);
                int flatsPerFloor = Convert.ToInt32(spnFlatsPerFloor.EditValue);

                if (_isEditMode)
                {
                    _currentApartment.BlockId = (int)lueBlock.EditValue;
                    _currentApartment.Name = txtName.Text;
                    _currentApartment.Address = txtAddress.Text;
                    _currentApartment.Status = cmbStatus.EditValue?.ToString() ?? "Aktif";
                    _currentApartment.FlatsPerFloor = flatsPerFloor;
                    _currentApartment.DefaultFlatType = cmbDefaultFlatType.EditValue?.ToString() ?? "2+1";
                    _currentApartment.DefaultMonthlyDue = monthlyDue;
                    _currentApartment.HasBalcony = cmbHasBalcony.EditValue?.ToString() == "Var";
                    _currentApartment.DefaultGrossArea = grossArea;
                    _currentApartment.DefaultNetArea = netArea;
                    _currentApartment.HasElevator = cmbHasElevator.EditValue?.ToString() == "Var";
                    _currentApartment.HasParking = cmbHasParking.EditValue?.ToString() == "Var";
                    _currentApartment.TotalFlats = _currentApartment.TotalFloors * flatsPerFloor;
                    
                    string res = _apartmentService.Update(_currentApartment);
                    if (!string.IsNullOrEmpty(res))
                    {
                        Swal.Error("Güncelleme hatası: " + res);
                        return;
                    }
                    Swal.Success("Apartman başarıyla güncellendi.");
                }
                else
                {
                    // Block'u ve Site bilgisini al
                    int blockId = (int)lueBlock.EditValue;
                    var block = _blockService.GetById(blockId);
                    int totalFloors = block?.Site?.FloorsPerApartment ?? 8;
                    
                    var newApartment = new Apartment
                    {
                        BlockId = blockId,
                        Name = txtName.Text,
                        Address = txtAddress.Text,
                        Status = cmbStatus.EditValue?.ToString() ?? "Aktif",
                        FlatsPerFloor = flatsPerFloor,
                        DefaultFlatType = cmbDefaultFlatType.EditValue?.ToString() ?? "2+1",
                        DefaultMonthlyDue = monthlyDue,
                        HasBalcony = cmbHasBalcony.EditValue?.ToString() == "Var",
                        DefaultGrossArea = grossArea,
                        DefaultNetArea = netArea,
                        HasElevator = cmbHasElevator.EditValue?.ToString() == "Var",
                        HasParking = cmbHasParking.EditValue?.ToString() == "Var",
                        TotalFloors = totalFloors,
                        TotalFlats = totalFloors * flatsPerFloor
                    };
                    
                    string res = _apartmentService.Add(newApartment);
                    if (!string.IsNullOrEmpty(res))
                    {
                        Swal.Error("Ekleme hatası: " + res);
                        return;
                    }
                    
                    // Otomatik olarak daireleri oluştur
                    // CreateFlatsForApartment requires apartment ID which is set after Add (EF Core tracks it)
                    string flatRes = _flatService.CreateFlatsForApartment(newApartment);
                    if (!string.IsNullOrEmpty(flatRes))
                    {
                        Swal.Warning("Apartman oluşturuldu ancak daireler oluşturulurken hata alındı: " + flatRes);
                    }
                    else
                    {
                        Swal.Success($"Apartman ve {newApartment.TotalFlats} daire başarıyla oluşturuldu.");
                    }
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
