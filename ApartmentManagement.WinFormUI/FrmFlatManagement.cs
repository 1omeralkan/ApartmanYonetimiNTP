#nullable disable
// FrmFlatManagement.cs
// Daire Yönetimi Formu - Daire ekleme/silme işlemleri
// Standart: Tahoma 8.25pt, AutoScroll = true
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using System.Drawing;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Daire yönetimi formu
    /// </summary>
    public partial class FrmFlatManagement : DevExpress.XtraEditors.XtraForm
    {
        private LookUpEdit lueSites;
        private LookUpEdit lueBlocks;
        private LookUpEdit lueApartments;
        private DevExpress.XtraGrid.GridControl gcFlats;
        private DevExpress.XtraGrid.Views.Grid.GridView gvFlats;
        
        private TextEdit txtDoorNumber;
        private TextEdit txtFloor;
        private ComboBoxEdit cmbType;

        private ISite _siteService;
        private IBlock _blockService;
        private IApartment _apartmentService;
        private IFlat _flatService;

        private int _selectedSiteId = -1;
        private int _selectedBlockId = -1;
        private int _selectedApartmentId = -1;

        public FrmFlatManagement()
        {
            _siteService = new SSite();
            _blockService = new SBlock();
            _apartmentService = new SApartment();
            _flatService = new SFlat();

            try
            {
                InitializeComponent();
                LoadSites();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Form yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.layoutControl = new LayoutControl();
            this.lueSites = new LookUpEdit();
            this.lueBlocks = new LookUpEdit();
            this.lueApartments = new LookUpEdit();
            this.gcFlats = new DevExpress.XtraGrid.GridControl();
            this.gvFlats = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtDoorNumber = new TextEdit();
            this.txtFloor = new TextEdit();
            this.cmbType = new ComboBoxEdit();
            // this.Root assignment removed, using layoutControl.Root directly
            if (this.layoutControl.Root == null)
                this.layoutControl.Root = new LayoutControlGroup();
            
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
            this.layoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lueSites.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueBlocks.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueApartments.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcFlats)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvFlats)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDoorNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFloor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl.Root)).BeginInit();
            this.SuspendLayout();

            this.layoutControl.Dock = DockStyle.Fill;
            this.Text = "Daire Yönetimi";
            this.Size = new Size(770, 700);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            // Filter Controls
            lueSites.Properties.DisplayMember = "Name"; lueSites.Properties.ValueMember = "Id";
            lueSites.EditValueChanged += (s, e) => { _selectedSiteId = (int?)lueSites.EditValue ?? -1; LoadBlocks(); };
            
            lueBlocks.Properties.DisplayMember = "Name"; lueBlocks.Properties.ValueMember = "Id";
            lueBlocks.EditValueChanged += (s, e) => { _selectedBlockId = (int?)lueBlocks.EditValue ?? -1; LoadApartments(); };
            
            lueApartments.Properties.DisplayMember = "Name"; lueApartments.Properties.ValueMember = "Id";
            lueApartments.EditValueChanged += (s, e) => { _selectedApartmentId = (int?)lueApartments.EditValue ?? -1; LoadFlats(); };

            // Input Controls
            cmbType.Properties.Items.AddRange(new object[] { "1+0", "1+1", "2+1", "3+1", "4+1" });
            cmbType.SelectedIndex = 1;

            // Simple Buttons
            SimpleButton btnAdd = new SimpleButton { Text = "Daire Ekle" };
            btnAdd.Click += (s, e) => AddFlat();
            SimpleButton btnDel = new SimpleButton { Text = "Seçiliyi Sil" };
            btnDel.Click += (s, e) => DeleteFlat();

            // Grid
            gcFlats.MainView = gvFlats;
            gvFlats.GridControl = gcFlats;
            gvFlats.OptionsBehavior.Editable = false;
            gvFlats.OptionsView.ShowGroupPanel = false;

            // Add Controls to Layout
            this.layoutControl.Controls.Add(lueSites);
            this.layoutControl.Controls.Add(lueBlocks);
            this.layoutControl.Controls.Add(lueApartments);
            this.layoutControl.Controls.Add(txtDoorNumber);
            this.layoutControl.Controls.Add(txtFloor);
            this.layoutControl.Controls.Add(cmbType);
            this.layoutControl.Controls.Add(btnAdd);
            this.layoutControl.Controls.Add(btnDel);
            this.layoutControl.Controls.Add(gcFlats);

            // Layout Hierarchy
            // Layout Hierarchy
            this.layoutControl.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControl.Root.GroupBordersVisible = false;

            var grpFilter = this.layoutControl.Root.AddGroup("Konum Seçimi");
            grpFilter.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
            grpFilter.AddItem("Site", lueSites);
            grpFilter.AddItem("Blok", lueBlocks);
            grpFilter.AddItem("Apartman", lueApartments);

            var grpContent = this.layoutControl.Root.AddGroup("Daire Listesi ve İşlemleri");
            grpContent.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Regular;
            
            // Grid takes most space
            var itemGrid = grpContent.AddItem();
            itemGrid.Control = gcFlats;
            itemGrid.TextVisible = false;

            // Side Panel for Adding
            var grpAdd = grpContent.AddGroup("Yeni Daire Ekle");
            grpAdd.AddItem("Kapı No", txtDoorNumber);
            grpAdd.AddItem("Kat", txtFloor);
            grpAdd.AddItem("Tip", cmbType);
            grpAdd.AddItem("", btnAdd);
            grpAdd.AddItem("", btnDel);

            this.Controls.Add(this.layoutControl);

            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
            this.layoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lueSites.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueBlocks.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueApartments.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcFlats)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvFlats)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDoorNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFloor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl.Root)).EndInit();
            this.ResumeLayout(false);
        }

        private LayoutControl layoutControl;
        
        private void LoadSites() { lueSites.Properties.DataSource = _siteService.GetAll(); }
        private void LoadBlocks() { lueBlocks.Properties.DataSource = _blockService.GetAllBySiteId(_selectedSiteId); }
        private void LoadApartments() { lueApartments.Properties.DataSource = _apartmentService.GetAllByBlockId(_selectedBlockId); }
        
        private void LoadFlats() {
            if (_selectedApartmentId == -1) return;
            gcFlats.DataSource = _flatService.GetAllByApartmentId(_selectedApartmentId);
            if (gvFlats.Columns["Apartment"] != null) gvFlats.Columns["Apartment"].Visible = false;
            if (gvFlats.Columns["FlatResidents"] != null) gvFlats.Columns["FlatResidents"].Visible = false;
            if (gvFlats.Columns["Id"] != null) gvFlats.Columns["Id"].Visible = false;
        }

        private void AddFlat() {
            if (_selectedApartmentId == -1) return;
            if (!int.TryParse(txtDoorNumber.Text, out int door) || !int.TryParse(txtFloor.Text, out int floor)) return;
            
            string res = _flatService.Add(new Flat { ApartmentId = _selectedApartmentId, DoorNumber = door, Floor = floor, Type = cmbType.Text, IsEmpty = true });
            if (!string.IsNullOrEmpty(res)) {
                 XtraMessageBox.Show("Ekleme hatası: " + res, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 return;
            }
            LoadFlats();
        }

        private void DeleteFlat() {
            var flat = gvFlats.GetFocusedRow() as Flat;
            if (flat != null && XtraMessageBox.Show("Silinsin mi?", "Onay", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                string res = _flatService.Delete(flat.Id);
                if (!string.IsNullOrEmpty(res)) {
                     XtraMessageBox.Show("Silme hatası: " + res, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                     return;
                }
                LoadFlats();
            }
        }
    }
}
