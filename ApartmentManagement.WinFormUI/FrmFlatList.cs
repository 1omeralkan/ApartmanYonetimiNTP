#nullable disable
// FrmFlatList.cs
// Daire Listesi Formu - Kayıtlı daireleri listeler
// Standart: Tahoma 8.25pt, AutoScroll = true
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System;

namespace ApartmentManagement.WinFormUI
{
    public class FrmFlatList : DevExpress.XtraEditors.XtraForm
    {
        private IFlat _flatService;
        private ISite _siteService;
        private IBlock _blockService;
        private IApartment _apartmentService;
        
        // Controls
        private GridControl gcFlats;
        private GridView gvFlats;
        private ComboBoxEdit cmbSiteFilter;
        private ComboBoxEdit cmbBlockFilter;
        private ComboBoxEdit cmbApartmentFilter;
        private ComboBoxEdit cmbStatusFilter;
        private SimpleButton btnFilter;
        private SimpleButton btnClear;
        private LabelControl lblTitle;

        // Missing buttons from backup, adding them now
        private SimpleButton btnAdd;
        private SimpleButton btnEdit;
        private SimpleButton btnDelete;

        public FrmFlatList()
        {
            _flatService = new SFlat();
            _siteService = new SSite();
            _blockService = new SBlock();
            _apartmentService = new SApartment();
            InitializeComponent();
            LoadSites();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings - Standart: Max 770x700, AutoScroll = true
            this.Text = "Daire Yönetimi";
            this.Size = new Size(770, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            // Title
            this.lblTitle = new LabelControl();
            this.lblTitle.Text = "🚪 Daireler";
            this.lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            this.lblTitle.Location = new Point(20, 15);
            this.Controls.Add(this.lblTitle);

            // Filter Panel
            var pnlFilter = new Panel();
            pnlFilter.Location = new Point(20, 55);
            pnlFilter.Size = new Size(1210, 55);
            pnlFilter.BackColor = Color.FromArgb(240, 242, 245);
            this.Controls.Add(pnlFilter);

            // Site Filter
            var lblSite = new LabelControl();
            lblSite.Text = "Site";
            lblSite.Location = new Point(10, 5);
            lblSite.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblSite);
            
            this.cmbSiteFilter = new ComboBoxEdit();
            this.cmbSiteFilter.Location = new Point(10, 22);
            this.cmbSiteFilter.Size = new Size(180, 26);
            this.cmbSiteFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbSiteFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
            this.cmbSiteFilter.SelectedIndexChanged += CmbSiteFilter_SelectedIndexChanged;
            pnlFilter.Controls.Add(this.cmbSiteFilter);

            // Block Filter
            var lblBlock = new LabelControl();
            lblBlock.Text = "Blok";
            lblBlock.Location = new Point(200, 5);
            lblBlock.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblBlock);
            
            this.cmbBlockFilter = new ComboBoxEdit();
            this.cmbBlockFilter.Location = new Point(200, 22);
            this.cmbBlockFilter.Size = new Size(150, 26);
            this.cmbBlockFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbBlockFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
            this.cmbBlockFilter.SelectedIndexChanged += CmbBlockFilter_SelectedIndexChanged;
            pnlFilter.Controls.Add(this.cmbBlockFilter);

            // Apartment Filter
            var lblApartment = new LabelControl();
            lblApartment.Text = "Apartman";
            lblApartment.Location = new Point(360, 5);
            lblApartment.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblApartment);
            
            this.cmbApartmentFilter = new ComboBoxEdit();
            this.cmbApartmentFilter.Location = new Point(360, 22);
            this.cmbApartmentFilter.Size = new Size(150, 26);
            this.cmbApartmentFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbApartmentFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
            pnlFilter.Controls.Add(this.cmbApartmentFilter);

            // Status Filter
            var lblStatus = new LabelControl();
            lblStatus.Text = "Durum";
            lblStatus.Location = new Point(520, 5);
            lblStatus.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblStatus);
            
            this.cmbStatusFilter = new ComboBoxEdit();
            this.cmbStatusFilter.Location = new Point(520, 22);
            this.cmbStatusFilter.Size = new Size(100, 26);
            this.cmbStatusFilter.Properties.Items.AddRange(new[] { "Tümü", "Boş", "Dolu" });
            this.cmbStatusFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbStatusFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
            this.cmbStatusFilter.SelectedIndex = 0;
            pnlFilter.Controls.Add(this.cmbStatusFilter);

            // Filter Button
            this.btnFilter = new SimpleButton();
            this.btnFilter.Text = "🔍 Ara";
            this.btnFilter.Size = new Size(100, 30);
            this.btnFilter.Location = new Point(640, 19);
            this.btnFilter.Appearance.Font = new Font("Tahoma", 10F);
            this.btnFilter.Appearance.BackColor = Color.FromArgb(66, 133, 244);
            this.btnFilter.Appearance.ForeColor = Color.White;
            this.btnFilter.Appearance.Options.UseBackColor = true;
            this.btnFilter.Appearance.Options.UseForeColor = true;
            this.btnFilter.Click += BtnFilter_Click;
            pnlFilter.Controls.Add(this.btnFilter);

            // Clear Button
            this.btnClear = new SimpleButton();
            this.btnClear.Text = "⊗ Temizle";
            this.btnClear.Size = new Size(100, 30);
            this.btnClear.Location = new Point(750, 19);
            this.btnClear.Appearance.Font = new Font("Tahoma", 10F);
            this.btnClear.Click += BtnClear_Click;
            pnlFilter.Controls.Add(this.btnClear);

            // CRUD Buttons (Top Right of Filter Panel)
            this.btnAdd = new SimpleButton { Text = "+ Yeni Daire", Size = new Size(100, 30), Location = new Point(880, 19) };
            this.btnAdd.Appearance.BackColor = Color.FromArgb(40, 167, 69);
            this.btnAdd.Appearance.ForeColor = Color.White;
            this.btnAdd.Appearance.Options.UseBackColor = true;
            this.btnAdd.Appearance.Options.UseForeColor = true;
            this.btnAdd.Click += BtnAdd_Click;
            pnlFilter.Controls.Add(this.btnAdd);

            this.btnEdit = new SimpleButton { Text = "✎ Düzenle", Size = new Size(100, 30), Location = new Point(990, 19) };
            this.btnEdit.Click += BtnEdit_Click;
            pnlFilter.Controls.Add(this.btnEdit);

            this.btnDelete = new SimpleButton { Text = "🗑 Sil", Size = new Size(100, 30), Location = new Point(1100, 19) };
            this.btnDelete.Appearance.BackColor = Color.FromArgb(220, 53, 69);
            this.btnDelete.Appearance.ForeColor = Color.White;
            this.btnDelete.Appearance.Options.UseBackColor = true;
            this.btnDelete.Appearance.Options.UseForeColor = true;
            this.btnDelete.Click += BtnDelete_Click;
            pnlFilter.Controls.Add(this.btnDelete);

            // Grid Control
            this.gcFlats = new GridControl();
            this.gvFlats = new GridView(this.gcFlats);
            this.gcFlats.MainView = this.gvFlats;
            this.gcFlats.Location = new Point(20, 120);
            this.gcFlats.Size = new Size(1210, 530);
            this.gcFlats.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            
            // Grid View Settings
            this.gvFlats.OptionsBehavior.Editable = false;
            this.gvFlats.OptionsView.ShowGroupPanel = false;
            this.gvFlats.OptionsView.ShowIndicator = false;
            this.gvFlats.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvFlats.RowHeight = 38;
            this.gvFlats.Appearance.HeaderPanel.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            this.gvFlats.Appearance.Row.Font = new Font("Tahoma", 10F);
            this.gvFlats.DoubleClick += GvFlats_DoubleClick;

            this.Controls.Add(this.gcFlats);

            // Bottom info label
            var lblInfo = new LabelControl();
            lblInfo.Text = "Kayıtlı daire listesi. Apartman oluşturulduğunda daireler otomatik oluşturulur.";
            lblInfo.Appearance.Font = new Font("Tahoma", 9F);
            lblInfo.Appearance.ForeColor = Color.Gray;
            lblInfo.Location = new Point(20, 658);
            this.Controls.Add(lblInfo);

            this.ResumeLayout(false);
        }

        // Helper class for filter dropdowns
        private class FilterItem
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        private void LoadFilters()
        {
            // Load Sites
            var sites = _siteService.GetAll();
            cmbSiteFilter.Properties.Items.Clear();
            cmbSiteFilter.Properties.Items.Add(new FilterItem { Id = null, Name = "Tümü" });
            foreach (var site in sites)
            {
                cmbSiteFilter.Properties.Items.Add(new FilterItem { Id = site.Id, Name = site.Name });
            }
            cmbSiteFilter.SelectedIndex = 0;
            
            LoadBlocks(null);
        }

        private void LoadBlocks(int? siteId)
        {
            cmbBlockFilter.Properties.Items.Clear();
            cmbBlockFilter.Properties.Items.Add(new FilterItem { Id = null, Name = "Tümü" });
            
            List<Block> blocks;
            if (siteId.HasValue)
            {
                blocks = _blockService.GetAllBySiteId(siteId.Value);
            }
            else
            {
                blocks = _blockService.GetAll();
            }
            
            foreach (var block in blocks)
            {
                cmbBlockFilter.Properties.Items.Add(new FilterItem { Id = block.Id, Name = block.Name });
            }
            cmbBlockFilter.SelectedIndex = 0;
            
            LoadApartments(null);
        }

        private void LoadApartments(int? blockId)
        {
            cmbApartmentFilter.Properties.Items.Clear();
            cmbApartmentFilter.Properties.Items.Add(new FilterItem { Id = null, Name = "Tümü" });
            
            List<Apartment> apartments;
            if (blockId.HasValue)
            {
                apartments = _apartmentService.GetAllByBlockId(blockId.Value);
            }
            else
            {
                apartments = _apartmentService.GetAll();
            }
            
            foreach (var apt in apartments)
            {
                cmbApartmentFilter.Properties.Items.Add(new FilterItem { Id = apt.Id, Name = apt.Name });
            }
            cmbApartmentFilter.SelectedIndex = 0;
        }

        private void LoadSites()
        {
            LoadFilters(); // Alias for LoadFilters
        }

        private void CmbSiteFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSiteFilter.SelectedItem is FilterItem filter)
            {
                LoadBlocks(filter.Id);
            }
        }

        private void CmbBlockFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBlockFilter.SelectedItem is FilterItem filter)
            {
                LoadApartments(filter.Id);
            }
        }

        private void DeleteFlat(int? flatId)
        {
            if (!flatId.HasValue) return;

            try
            {
                string result = _flatService.Delete(flatId.Value);
                if (!string.IsNullOrEmpty(result))
                {
                    Swal.Error("Silme başarısız: " + result);
                    return;
                }

                LoadData();
                Swal.Success("Daire başarıyla silindi.");
            }
            catch (Exception ex)
            {
                Swal.Error("Daire silinirken bir hata oluştu: " + ex.Message);
            }
        }

        private void LoadData()
        {
            try
            {
                var flats = _flatService.GetAll();
                
                // Apply filters if set
                if (cmbSiteFilter.SelectedItem is FilterItem siteFilter && siteFilter.Id.HasValue)
                {
                    flats = flats.Where(f => f.Apartment?.Block?.SiteId == siteFilter.Id.Value).ToList();
                }
                if (cmbBlockFilter.SelectedItem is FilterItem blockFilter && blockFilter.Id.HasValue)
                {
                    flats = flats.Where(f => f.Apartment?.BlockId == blockFilter.Id.Value).ToList();
                }
                if (cmbApartmentFilter.SelectedItem is FilterItem aptFilter && aptFilter.Id.HasValue)
                {
                    flats = flats.Where(f => f.ApartmentId == aptFilter.Id.Value).ToList();
                }
                
                string statusFilter = cmbStatusFilter.EditValue?.ToString() ?? "Tümü";
                if (statusFilter == "Boş")
                {
                    flats = flats.Where(f => f.IsEmpty).ToList();
                }
                else if (statusFilter == "Dolu")
                {
                    flats = flats.Where(f => !f.IsEmpty).ToList();
                }
                
                // Create display data
                var displayData = flats.Select(f => new
                {
                    f.Id,
                    Site = f.Apartment?.Block?.Site?.Name ?? "-",
                    Blok = f.Apartment?.Block?.Name ?? "-",
                    Apartman = f.Apartment?.Name ?? "-",
                    Kat = f.Floor,
                    No = f.DoorNumber,
                    Tip = f.Type ?? "-",
                    Durum = f.IsEmpty ? "Boş" : "Dolu",
                    SakinSayisi = f.FlatResidents?.Count ?? 0
                }).OrderBy(f => f.Site).ThenBy(f => f.Blok).ThenBy(f => f.Apartman).ThenBy(f => f.Kat).ThenBy(f => f.No).ToList();
                
                gcFlats.DataSource = displayData;
                gvFlats.PopulateColumns();
                
                if (gvFlats.Columns["Id"] != null) gvFlats.Columns["Id"].Visible = false;
                if (gvFlats.Columns["SakinSayisi"] != null) gvFlats.Columns["SakinSayisi"].Caption = "Aktif Sakin";
            }
            catch (Exception ex)
            {
                Swal.Error("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            cmbSiteFilter.SelectedIndex = 0;
            cmbBlockFilter.SelectedIndex = 0;
            cmbApartmentFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndex = 0;
            LoadData();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var frm = new FrmFlatManagement();
            frm.ShowDialog();
            LoadData();
        }
        
        private void GvFlats_DoubleClick(object sender, EventArgs e)
        {
            var frm = new FrmFlatManagement();
            frm.ShowDialog();
            LoadData();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var frm = new FrmFlatManagement();
            frm.ShowDialog();
            LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var row = gvFlats.GetFocusedRow();
            if (row != null)
            {
                var idProperty = row.GetType().GetProperty("Id");
                var noProperty = row.GetType().GetProperty("No");
                if (idProperty != null)
                {
                    int flatId = (int)idProperty.GetValue(row);
                    string flatNo = noProperty?.GetValue(row)?.ToString() ?? "Bu daire";
                    
                    if (Swal.Confirm($"'{flatNo}' numaralı daireyi silmek istediğinize emin misiniz?"))
                    {
                        DeleteFlat(flatId);
                    }
                }
            }
            else
            {
                Swal.Warning("Lütfen silmek için bir daire seçin.");
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private System.ComponentModel.IContainer components = null;
    }
}
