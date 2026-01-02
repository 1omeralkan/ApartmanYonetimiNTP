#nullable disable
// FrmApartmentList.cs
// Apartman Listesi Formu - Kayıtlı apartmanları listeler
// Standart: Tahoma 8.25pt, AutoScroll = true
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.Drawing;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Apartman listesi formu
    /// </summary>
    public partial class FrmApartmentList : DevExpress.XtraEditors.XtraForm
    {
        private IApartment _apartmentService;
        private ISite _siteService;
        private IBlock _blockService;
        
        // Controls
        private GridControl gcApartments;
        private GridView gvApartments;
        private ComboBoxEdit cmbSiteFilter;
        private ComboBoxEdit cmbBlockFilter;
        private SimpleButton btnFilter;
        private SimpleButton btnClear;
        private SimpleButton btnAdd;
        private SimpleButton btnEdit;
        private SimpleButton btnDelete;
        private LabelControl lblTitle;

        public FrmApartmentList()
        {
            _apartmentService = new SApartment();
            _siteService = new SSite();
            _blockService = new SBlock();
            InitializeComponent();
            LoadSites();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings - Standart: Max 770x700, AutoScroll = true
            this.Text = "Apartman Yönetimi";
            this.Size = new Size(770, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            // Title
            this.lblTitle = new LabelControl();
            this.lblTitle.Text = "🏠 Apartmanlar";
            this.lblTitle.Appearance.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            this.lblTitle.Location = new Point(20, 15);
            this.Controls.Add(this.lblTitle);

            // Button Panel (top right)
            var pnlButtons = new Panel();
            pnlButtons.Location = new Point(850, 10);
            pnlButtons.Size = new Size(340, 40);
            this.Controls.Add(pnlButtons);

            // Add Button
            this.btnAdd = new SimpleButton();
            this.btnAdd.Text = "+ Yeni Apartman";
            this.btnAdd.Size = new Size(130, 35);
            this.btnAdd.Location = new Point(0, 0);
            this.btnAdd.Appearance.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            this.btnAdd.Appearance.BackColor = Color.FromArgb(66, 133, 244);
            this.btnAdd.Appearance.ForeColor = Color.White;
            this.btnAdd.Appearance.Options.UseBackColor = true;
            this.btnAdd.Appearance.Options.UseForeColor = true;
            this.btnAdd.Cursor = Cursors.Hand;
            this.btnAdd.Click += BtnAdd_Click;
            pnlButtons.Controls.Add(this.btnAdd);

            // Edit Button
            this.btnEdit = new SimpleButton();
            this.btnEdit.Text = "Düzenle";
            this.btnEdit.Size = new Size(90, 35);
            this.btnEdit.Location = new Point(140, 0);
            this.btnEdit.Appearance.Font = new Font("Tahoma", 8.25F);
            this.btnEdit.Appearance.BackColor = Color.FromArgb(16, 185, 129);
            this.btnEdit.Appearance.ForeColor = Color.White;
            this.btnEdit.Appearance.Options.UseBackColor = true;
            this.btnEdit.Appearance.Options.UseForeColor = true;
            this.btnEdit.Cursor = Cursors.Hand;
            this.btnEdit.Click += BtnEdit_Click;
            pnlButtons.Controls.Add(this.btnEdit);

            // Delete Button
            this.btnDelete = new SimpleButton();
            this.btnDelete.Text = "Sil";
            this.btnDelete.Size = new Size(70, 35);
            this.btnDelete.Location = new Point(240, 0);
            this.btnDelete.Appearance.Font = new Font("Tahoma", 8.25F);
            this.btnDelete.Appearance.BackColor = Color.FromArgb(220, 53, 69);
            this.btnDelete.Appearance.ForeColor = Color.White;
            this.btnDelete.Appearance.Options.UseBackColor = true;
            this.btnDelete.Appearance.Options.UseForeColor = true;
            this.btnDelete.Cursor = Cursors.Hand;
            this.btnDelete.Click += BtnDelete_Click;
            pnlButtons.Controls.Add(this.btnDelete);

            // Filter Panel
            var pnlFilter = new Panel();
            pnlFilter.Location = new Point(20, 55);
            pnlFilter.Size = new Size(1160, 50);
            pnlFilter.BackColor = Color.FromArgb(240, 242, 245);
            this.Controls.Add(pnlFilter);

            // Site Filter ComboBox
            this.cmbSiteFilter = new ComboBoxEdit();
            this.cmbSiteFilter.Location = new Point(10, 10);
            this.cmbSiteFilter.Size = new Size(200, 28);
            this.cmbSiteFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbSiteFilter.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            this.cmbSiteFilter.Properties.NullText = "Tüm Siteler";
            this.cmbSiteFilter.SelectedIndexChanged += CmbSiteFilter_SelectedIndexChanged;
            pnlFilter.Controls.Add(this.cmbSiteFilter);

            // Block Filter ComboBox
            this.cmbBlockFilter = new ComboBoxEdit();
            this.cmbBlockFilter.Location = new Point(220, 10);
            this.cmbBlockFilter.Size = new Size(200, 28);
            this.cmbBlockFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbBlockFilter.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            this.cmbBlockFilter.Properties.NullText = "Tüm Bloklar";
            pnlFilter.Controls.Add(this.cmbBlockFilter);

            // Filter Button
            this.btnFilter = new SimpleButton();
            this.btnFilter.Text = "🔍 Uygula";
            this.btnFilter.Size = new Size(120, 30);
            this.btnFilter.Location = new Point(440, 9);
            this.btnFilter.Appearance.Font = new Font("Tahoma", 8.25F);
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
            this.btnClear.Location = new Point(570, 9);
            this.btnClear.Appearance.Font = new Font("Tahoma", 8.25F);
            this.btnClear.Click += BtnClear_Click;
            pnlFilter.Controls.Add(this.btnClear);

            // Grid Control
            this.gcApartments = new GridControl();
            this.gvApartments = new GridView(this.gcApartments);
            this.gcApartments.MainView = this.gvApartments;
            this.gcApartments.Location = new Point(20, 115);
            this.gcApartments.Size = new Size(1160, 530);
            this.gcApartments.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            
            // Grid View Settings
            this.gvApartments.OptionsBehavior.Editable = false;
            this.gvApartments.OptionsView.ShowGroupPanel = false;
            this.gvApartments.OptionsView.ShowIndicator = false;
            this.gvApartments.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvApartments.RowHeight = 40;
            this.gvApartments.Appearance.HeaderPanel.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            this.gvApartments.Appearance.Row.Font = new Font("Tahoma", 8.25F);
            this.gvApartments.DoubleClick += GvApartments_DoubleClick;
            
            this.Controls.Add(this.gcApartments);

            // Bottom info label
            var lblInfo = new LabelControl();
            lblInfo.Text = "Kayıtlı apartman listesi. Satıra tıklayarak detay sayfasına gidebilirsiniz.";
            lblInfo.Appearance.Font = new Font("Tahoma", 8.25F);
            lblInfo.Appearance.ForeColor = Color.Gray;
            lblInfo.Location = new Point(20, 655);
            this.Controls.Add(lblInfo);

            this.ResumeLayout(false);
        }

        // Helper classes for filter dropdowns
        private class FilterItem
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        private void LoadSites()
        {
            var sites = _siteService.GetAll();
            cmbSiteFilter.Properties.Items.Clear();
            cmbSiteFilter.Properties.Items.Add(new FilterItem { Id = null, Name = "Tüm Siteler" });
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
            cmbBlockFilter.Properties.Items.Add(new FilterItem { Id = null, Name = "Tüm Bloklar" });
            
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
        }

        private void CmbSiteFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSiteFilter.SelectedItem is FilterItem filter)
            {
                LoadBlocks(filter.Id);
            }
        }

        private void LoadData(int? siteId = null, int? blockId = null)
        {
            try
            {
                List<Apartment> apartments;
                if (blockId.HasValue)
                {
                    apartments = _apartmentService.GetAllByBlockId(blockId.Value);
                }
                else if (siteId.HasValue)
                {
                    apartments = _apartmentService.GetAllBySiteId(siteId.Value);
                }
                else
                {
                    apartments = _apartmentService.GetAll();
                }
                
                // Create display data
                var displayData = apartments.Select(a => new
                {
                    a.Id,
                    Ad = a.Name,
                    Site = a.Block?.Site?.Name ?? "-",
                    Blok = a.Block?.Name ?? "-",
                    Durum = a.Status,
                    Kat = a.TotalFloors,
                    KatBasinaDaire = a.FlatsPerFloor,
                    ToplamDaire = a.TotalFlats
                }).ToList();
                
                gcApartments.DataSource = displayData;
                gvApartments.PopulateColumns();
                
                if (gvApartments.Columns["Id"] != null) gvApartments.Columns["Id"].Visible = false;
            }
            catch (Exception ex)
            {
                Swal.Error("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            int? siteId = null;
            int? blockId = null;
            
            if (cmbSiteFilter.SelectedItem is FilterItem siteFilter && siteFilter.Id.HasValue)
            {
                siteId = siteFilter.Id;
            }
            if (cmbBlockFilter.SelectedItem is FilterItem blockFilter && blockFilter.Id.HasValue)
            {
                blockId = blockFilter.Id;
            }
            
            LoadData(siteId, blockId);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            cmbSiteFilter.SelectedIndex = 0;
            cmbBlockFilter.SelectedIndex = 0;
            LoadData();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var frm = new FrmApartmentManagement();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void GvApartments_DoubleClick(object sender, EventArgs e)
        {
            BtnEdit_Click(sender, e);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var row = gvApartments.GetFocusedRow();
            if (row != null)
            {
                var idProperty = row.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    int apartmentId = (int)idProperty.GetValue(row);
                    var apartment = _apartmentService.GetById(apartmentId);
                    if (apartment != null)
                    {
                        var frm = new FrmApartmentManagement(apartment);
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            LoadData();
                        }
                    }
                }
            }
            else
            {
                Swal.Warning("Lütfen düzenlemek için bir apartman seçin.");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var row = gvApartments.GetFocusedRow();
            if (row != null)
            {
                var idProperty = row.GetType().GetProperty("Id");
                var nameProperty = row.GetType().GetProperty("Ad");
                if (idProperty != null)
                {
                    int apartmentId = (int)idProperty.GetValue(row);
                    string apartmentName = nameProperty?.GetValue(row)?.ToString() ?? "Bu apartman";
                    
                    if (Swal.Confirm($"'{apartmentName}' apartmanını silmek istediğinize emin misiniz?"))
                    {
                        try
                    {
                        string result = _apartmentService.Delete(apartmentId);
                        if (!string.IsNullOrEmpty(result))
                        {
                            Swal.Error("Silme başarısız: " + result);
                            return;
                        }

                        LoadData();
                        Swal.Success("Apartman başarıyla silindi.");
                    }
                        catch (Exception ex)
                        {
                            Swal.Error("Silme işlemi sırasında hata oluştu: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                Swal.Warning("Lütfen silmek için bir apartman seçin.");
            }
        }
    }
}
