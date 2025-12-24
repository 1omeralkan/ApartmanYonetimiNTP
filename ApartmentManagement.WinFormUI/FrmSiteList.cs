#nullable disable
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System.Drawing;

namespace ApartmentManagement.WinFormUI
{
    public partial class FrmSiteList : DevExpress.XtraEditors.XtraForm
    {
        private ISite _siteService;
        
        // Controls
        private GridControl gcSites;
        private GridView gvSites;
        private SimpleButton btnAdd;
        private SimpleButton btnEdit;
        private SimpleButton btnDelete;
        private LabelControl lblTitle;

        public FrmSiteList()
        {
            _siteService = new SSite();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = "Site Yönetimi";
            this.Size = new Size(1150, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Title
            this.lblTitle = new LabelControl();
            this.lblTitle.Text = "🏗️ Site Yönetimi";
            this.lblTitle.Appearance.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            this.lblTitle.Location = new Point(20, 15);
            this.Controls.Add(this.lblTitle);

            // Button Panel
            var pnlButtons = new Panel();
            pnlButtons.Location = new Point(800, 10);
            pnlButtons.Size = new Size(340, 40);
            this.Controls.Add(pnlButtons);

            // Add Button
            this.btnAdd = new SimpleButton();
            this.btnAdd.Text = "+ Yeni Site";
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

            // Grid Control
            this.gcSites = new GridControl();
            this.gvSites = new GridView(this.gcSites);
            this.gcSites.MainView = this.gvSites;
            this.gcSites.Location = new Point(20, 60);
            this.gcSites.Size = new Size(1110, 520);
            this.gcSites.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            
            // Grid View Settings
            this.gvSites.OptionsBehavior.Editable = false;
            this.gvSites.OptionsView.ShowGroupPanel = false;
            this.gvSites.OptionsView.ShowIndicator = false;
            this.gvSites.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvSites.RowHeight = 40;
            this.gvSites.Appearance.HeaderPanel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.gvSites.Appearance.Row.Font = new Font("Segoe UI", 10F);
            this.gvSites.DoubleClick += GvSites_DoubleClick;
            
            this.Controls.Add(this.gcSites);

            this.ResumeLayout(false);
        }

        private void LoadData()
        {
            try
            {
                var sites = _siteService.GetAll();
                
                // Create display data with calculated fields
                var displayData = sites.Select(s => new
                {
                    s.Id,
                    Ad = s.Name,
                    Kod = s.Code,
                    Durum = s.Status,
                    Blok = s.TotalBlocks,
                    BlokBasinaApartman = s.ApartmentsPerBlock,
                    ApartmanBasinaKat = s.FloorsPerApartment,
                    ToplamKat = s.TotalBlocks * s.ApartmentsPerBlock * s.FloorsPerApartment,
                    KatBasinaDaire = s.FlatsPerFloor,
                    ToplamDaire = s.TotalBlocks * s.ApartmentsPerBlock * s.FloorsPerApartment * s.FlatsPerFloor
                }).ToList();
                
                gcSites.DataSource = displayData;
                gvSites.PopulateColumns();
                
                // Hide Id
                if (gvSites.Columns["Id"] != null) gvSites.Columns["Id"].Visible = false;
                
                // Set column order and widths
                if (gvSites.Columns["Ad"] != null) { gvSites.Columns["Ad"].Width = 100; gvSites.Columns["Ad"].VisibleIndex = 0; }
                if (gvSites.Columns["Kod"] != null) { gvSites.Columns["Kod"].Width = 60; gvSites.Columns["Kod"].VisibleIndex = 1; }
                if (gvSites.Columns["Durum"] != null) { gvSites.Columns["Durum"].Width = 60; gvSites.Columns["Durum"].VisibleIndex = 2; }
                if (gvSites.Columns["Blok"] != null) { gvSites.Columns["Blok"].Width = 50; gvSites.Columns["Blok"].VisibleIndex = 3; }
                if (gvSites.Columns["BlokBasinaApartman"] != null) { gvSites.Columns["BlokBasinaApartman"].Caption = "Blok Başına Apartman"; gvSites.Columns["BlokBasinaApartman"].Width = 130; gvSites.Columns["BlokBasinaApartman"].VisibleIndex = 4; }
                if (gvSites.Columns["ApartmanBasinaKat"] != null) { gvSites.Columns["ApartmanBasinaKat"].Caption = "Apartman Başına Kat"; gvSites.Columns["ApartmanBasinaKat"].Width = 120; gvSites.Columns["ApartmanBasinaKat"].VisibleIndex = 5; }
                if (gvSites.Columns["ToplamKat"] != null) { gvSites.Columns["ToplamKat"].Caption = "Toplam Kat"; gvSites.Columns["ToplamKat"].Width = 80; gvSites.Columns["ToplamKat"].VisibleIndex = 6; }
                if (gvSites.Columns["KatBasinaDaire"] != null) { gvSites.Columns["KatBasinaDaire"].Caption = "Kat Başına Daire"; gvSites.Columns["KatBasinaDaire"].Width = 100; gvSites.Columns["KatBasinaDaire"].VisibleIndex = 7; }
                if (gvSites.Columns["ToplamDaire"] != null) { gvSites.Columns["ToplamDaire"].Caption = "Toplam Daire"; gvSites.Columns["ToplamDaire"].Width = 90; gvSites.Columns["ToplamDaire"].VisibleIndex = 8; }
            }
            catch (Exception ex)
            {
                Swal.Error("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var frm = new FrmSiteManagement();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var row = gvSites.GetFocusedRow();
            if (row != null)
            {
                var idProp = row.GetType().GetProperty("Id");
                if (idProp != null)
                {
                    int siteId = (int)idProp.GetValue(row);
                    var site = _siteService.GetAll().FirstOrDefault(s => s.Id == siteId);
                    if (site != null)
                    {
                        var frm = new FrmSiteManagement(site);
                        if (frm.ShowDialog() == DialogResult.OK) LoadData();
                    }
                }
            }
            else
            {
                Swal.Warning("Lütfen düzenlemek için bir site seçin.");
            }
        }

        private void GvSites_DoubleClick(object sender, EventArgs e)
        {
            BtnEdit_Click(sender, e);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var row = gvSites.GetFocusedRow();
            if (row != null)
            {
                var idProp = row.GetType().GetProperty("Id");
                var nameProp = row.GetType().GetProperty("Ad");
                if (idProp != null)
                {
                    int siteId = (int)idProp.GetValue(row);
                    string siteName = nameProp?.GetValue(row)?.ToString() ?? "Bu site";
                    
                    if (Swal.Confirm($"'{siteName}' sitesini silmek istediğinize emin misiniz?"))
                    {
                        try
                        {
                            string result = _siteService.Delete(siteId);
                            if (!string.IsNullOrEmpty(result))
                            {
                                Swal.Error("Silme başarısız: " + result);
                                return;
                            }

                            LoadData();
                            Swal.Success("Site başarıyla silindi.");
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
                Swal.Warning("Lütfen silmek için bir site seçin.");
            }
        }
    }
}
