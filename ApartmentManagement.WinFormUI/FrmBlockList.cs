#nullable disable
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
    public partial class FrmBlockList : DevExpress.XtraEditors.XtraForm
    {
        private IBlock _blockService;
        private ISite _siteService;
        
        // Controls
        private GridControl gcBlocks;
        private GridView gvBlocks;
        private ComboBoxEdit cmbSiteFilter;
        private SimpleButton btnFilter;
        private SimpleButton btnClear;
        private SimpleButton btnAdd;
        private SimpleButton btnEdit;
        private SimpleButton btnDelete;
        private LabelControl lblTitle;

        public FrmBlockList()
        {
            _blockService = new SBlock();
            _siteService = new SSite();
            InitializeComponent();
            LoadSites();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = "Blok Yönetimi";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Title
            this.lblTitle = new LabelControl();
            this.lblTitle.Text = "🏢 Bloklar";
            this.lblTitle.Appearance.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            this.lblTitle.Location = new Point(20, 15);
            this.Controls.Add(this.lblTitle);

            // Button Panel (top right)
            var pnlButtons = new Panel();
            pnlButtons.Location = new Point(750, 10);
            pnlButtons.Size = new Size(340, 40);
            this.Controls.Add(pnlButtons);

            // Add Button
            this.btnAdd = new SimpleButton();
            this.btnAdd.Text = "+ Yeni Blok";
            this.btnAdd.Size = new Size(100, 35);
            this.btnAdd.Location = new Point(0, 0);
            this.btnAdd.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
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
            this.btnEdit.Location = new Point(110, 0);
            this.btnEdit.Appearance.Font = new Font("Segoe UI", 10F);
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
            this.btnDelete.Location = new Point(210, 0);
            this.btnDelete.Appearance.Font = new Font("Segoe UI", 10F);
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
            pnlFilter.Size = new Size(1060, 50);
            pnlFilter.BackColor = Color.FromArgb(240, 242, 245);
            this.Controls.Add(pnlFilter);

            // Site Filter ComboBox
            this.cmbSiteFilter = new ComboBoxEdit();
            this.cmbSiteFilter.Location = new Point(10, 10);
            this.cmbSiteFilter.Size = new Size(250, 28);
            this.cmbSiteFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbSiteFilter.Properties.Appearance.Font = new Font("Segoe UI", 10F);
            this.cmbSiteFilter.Properties.NullText = "Tüm Siteler";
            pnlFilter.Controls.Add(this.cmbSiteFilter);

            // Filter Button
            this.btnFilter = new SimpleButton();
            this.btnFilter.Text = "🔍 Uygula";
            this.btnFilter.Size = new Size(120, 30);
            this.btnFilter.Location = new Point(280, 9);
            this.btnFilter.Appearance.Font = new Font("Segoe UI", 10F);
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
            this.btnClear.Location = new Point(410, 9);
            this.btnClear.Appearance.Font = new Font("Segoe UI", 10F);
            this.btnClear.Click += BtnClear_Click;
            pnlFilter.Controls.Add(this.btnClear);

            // Grid Control
            this.gcBlocks = new GridControl();
            this.gvBlocks = new GridView(this.gcBlocks);
            this.gcBlocks.MainView = this.gvBlocks;
            this.gcBlocks.Location = new Point(20, 115);
            this.gcBlocks.Size = new Size(1060, 480);
            this.gcBlocks.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            
            // Grid View Settings
            this.gvBlocks.OptionsBehavior.Editable = false;
            this.gvBlocks.OptionsView.ShowGroupPanel = false;
            this.gvBlocks.OptionsView.ShowIndicator = false;
            this.gvBlocks.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvBlocks.RowHeight = 40;
            this.gvBlocks.Appearance.HeaderPanel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.gvBlocks.Appearance.Row.Font = new Font("Segoe UI", 10F);
            this.gvBlocks.DoubleClick += GvBlocks_DoubleClick;
            
            this.Controls.Add(this.gcBlocks);

            // Bottom info label
            var lblInfo = new LabelControl();
            lblInfo.Text = "Kayıtlı blok listesi. Satıra tıklayarak detay sayfasına gidebilirsiniz.";
            lblInfo.Appearance.Font = new Font("Segoe UI", 9F);
            lblInfo.Appearance.ForeColor = Color.Gray;
            lblInfo.Location = new Point(20, 600);
            this.Controls.Add(lblInfo);

            this.ResumeLayout(false);
        }

        private void LoadSites()
        {
            var sites = _siteService.GetAll();
            cmbSiteFilter.Properties.Items.Clear();
            cmbSiteFilter.Properties.Items.Add(new SiteFilterItem { Id = null, Name = "Tüm Siteler" });
            foreach (var site in sites)
            {
                cmbSiteFilter.Properties.Items.Add(new SiteFilterItem { Id = site.Id, Name = site.Name });
            }
            cmbSiteFilter.SelectedIndex = 0;
        }

        // Helper class for filter dropdown
        private class SiteFilterItem
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        private void LoadData(int? siteId = null)
        {
            try
            {
                List<Block> blocks;
                if (siteId.HasValue)
                {
                    blocks = _blockService.GetAllBySiteId(siteId.Value);
                }
                else
                {
                    blocks = _blockService.GetAll();
                }
                
                // Create display data
                var displayData = blocks.Select(b => new
                {
                    b.Id,
                    Ad = b.Name,
                    Site = b.Site?.Name ?? "-",
                    ToplamApartman = b.TotalApartments,
                    ApartmanBasinaKat = b.TotalFloors > 0 ? b.TotalFloors : (b.Site?.FloorsPerApartment ?? 0),
                    ToplamKat = b.TotalApartments * (b.TotalFloors > 0 ? b.TotalFloors : (b.Site?.FloorsPerApartment ?? 0)),
                    KatBasinaDaire = b.FlatsPerFloor,
                    ToplamDaire = b.TotalApartments * (b.TotalFloors > 0 ? b.TotalFloors : (b.Site?.FloorsPerApartment ?? 0)) * b.FlatsPerFloor,
                    Durum = b.Status,
                    Olusturulma = b.CreatedDate.ToString("dd.MM.yyyy")
                }).ToList();
                
                gcBlocks.DataSource = displayData;
                gvBlocks.PopulateColumns();
                
                if (gvBlocks.Columns["Id"] != null) gvBlocks.Columns["Id"].Visible = false;
            }
            catch (Exception ex)
            {
                Swal.Error("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            if (cmbSiteFilter.SelectedItem is SiteFilterItem filter && filter.Id.HasValue)
            {
                LoadData(filter.Id.Value);
            }
            else
            {
                LoadData();
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            cmbSiteFilter.SelectedIndex = 0;
            LoadData();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var frm = new FrmBlockManagement();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void GvBlocks_DoubleClick(object sender, EventArgs e)
        {
            BtnEdit_Click(sender, e);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var row = gvBlocks.GetFocusedRow();
            if (row != null)
            {
                var idProperty = row.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    int blockId = (int)idProperty.GetValue(row);
                    var block = _blockService.GetById(blockId);
                    if (block != null)
                    {
                        var frm = new FrmBlockManagement(block);
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            LoadData();
                        }
                    }
                }
            }
            else
            {
                Swal.Warning("Lütfen düzenlemek için bir blok seçin.");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var row = gvBlocks.GetFocusedRow();
            if (row != null)
            {
                var idProperty = row.GetType().GetProperty("Id");
                var nameProperty = row.GetType().GetProperty("Ad");
                if (idProperty != null)
                {
                    int blockId = (int)idProperty.GetValue(row);
                    string blockName = nameProperty?.GetValue(row)?.ToString() ?? "Bu blok";
                    
                    if (Swal.Confirm($"'{blockName}' bloğunu silmek istediğinize emin misiniz?"))
                    {
                        try
                        {
                            string result = _blockService.Delete(blockId);
                            if (!string.IsNullOrEmpty(result))
                            {
                                Swal.Error("Silme başarısız: " + result);
                                return;
                            }

                            LoadData();
                            Swal.Success("Blok başarıyla silindi.");
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
                Swal.Warning("Lütfen silmek için bir blok seçin.");
            }
        }
    }
}
