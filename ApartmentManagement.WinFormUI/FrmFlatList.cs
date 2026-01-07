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
        private TextEdit txtKatMin;
        private TextEdit txtKatMax;
        private TextEdit txtDaireNo;
        private ComboBoxEdit cmbDaireTipi;
        private SimpleButton btnFilter;
        private SimpleButton btnClear;
        private SimpleButton btnDetay;
        private SimpleButton btnSakinAta;
        private LabelControl lblTitle;


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

            // Filter Panel - 2 rows
            var pnlFilter = new Panel();
            pnlFilter.Location = new Point(20, 55);
            pnlFilter.Size = new Size(1210, 110);
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

            // Kat Min Filter
            var lblKatMin = new LabelControl();
            lblKatMin.Text = "Kat Min";
            lblKatMin.Location = new Point(630, 5);
            lblKatMin.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblKatMin);
            
            this.txtKatMin = new TextEdit();
            this.txtKatMin.Location = new Point(630, 22);
            this.txtKatMin.Size = new Size(80, 26);
            this.txtKatMin.Properties.Appearance.Font = new Font("Tahoma", 9F);
            this.txtKatMin.EditValue = "0";
            pnlFilter.Controls.Add(this.txtKatMin);

            // Kat Max Filter
            var lblKatMax = new LabelControl();
            lblKatMax.Text = "Kat Max";
            lblKatMax.Location = new Point(720, 5);
            lblKatMax.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblKatMax);
            
            this.txtKatMax = new TextEdit();
            this.txtKatMax.Location = new Point(720, 22);
            this.txtKatMax.Size = new Size(80, 26);
            this.txtKatMax.Properties.Appearance.Font = new Font("Tahoma", 9F);
            this.txtKatMax.EditValue = "999";
            pnlFilter.Controls.Add(this.txtKatMax);

            // Daire No Filter
            var lblDaireNo = new LabelControl();
            lblDaireNo.Text = "Daire No";
            lblDaireNo.Location = new Point(810, 5);
            lblDaireNo.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblDaireNo);
            
            this.txtDaireNo = new TextEdit();
            this.txtDaireNo.Location = new Point(810, 22);
            this.txtDaireNo.Size = new Size(100, 26);
            this.txtDaireNo.Properties.Appearance.Font = new Font("Tahoma", 9F);
            pnlFilter.Controls.Add(this.txtDaireNo);

            // Daire Tipi Filter
            var lblDaireTipi = new LabelControl();
            lblDaireTipi.Text = "Daire Tipi";
            lblDaireTipi.Location = new Point(920, 5);
            lblDaireTipi.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblDaireTipi);
            
            this.cmbDaireTipi = new ComboBoxEdit();
            this.cmbDaireTipi.Location = new Point(920, 22);
            this.cmbDaireTipi.Size = new Size(120, 26);
            this.cmbDaireTipi.Properties.Items.AddRange(new[] { "Tümü", "1+1", "2+1", "3+1", "4+1", "5+1" });
            this.cmbDaireTipi.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbDaireTipi.Properties.Appearance.Font = new Font("Tahoma", 9F);
            this.cmbDaireTipi.SelectedIndex = 0;
            pnlFilter.Controls.Add(this.cmbDaireTipi);

            // Filter Button
            this.btnFilter = new SimpleButton();
            this.btnFilter.Text = "🔍 Ara";
            this.btnFilter.Size = new Size(100, 30);
            this.btnFilter.Location = new Point(1050, 19);
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
            this.btnClear.Location = new Point(1050, 55);
            this.btnClear.Appearance.Font = new Font("Tahoma", 10F);
            this.btnClear.Click += BtnClear_Click;
            pnlFilter.Controls.Add(this.btnClear);

            // Detay Button - positioned to the right of filter panel, aligned with Ara button
            // Filtre paneli: Location = (20, 55), Ara butonu panel içinde (1050, 19)
            // Form koordinatlarında: X = 20 + 1050 + 100 (buton genişliği) + 80 (daha fazla boşluk) = 1250, Y = 55 + 19 = 74
            this.btnDetay = new SimpleButton();
            this.btnDetay.Text = "✓ Detay";
            this.btnDetay.Size = new Size(100, 30);
            this.btnDetay.Location = new Point(1250, 74);
            this.btnDetay.Appearance.Font = new Font("Tahoma", 10F);
            this.btnDetay.Appearance.BackColor = Color.FromArgb(59, 130, 246);
            this.btnDetay.Appearance.ForeColor = Color.White;
            this.btnDetay.Appearance.Options.UseBackColor = true;
            this.btnDetay.Appearance.Options.UseForeColor = true;
            this.btnDetay.Enabled = false;
            this.btnDetay.Click += BtnDetay_Click;
            this.Controls.Add(this.btnDetay);

            // Sakin Ata Button - positioned below Detay button, aligned with Temizle button
            // Temizle butonu panel içinde (1050, 55), Form koordinatlarında: X = 20 + 1050 + 100 + 80 = 1250, Y = 55 + 55 = 110
            this.btnSakinAta = new SimpleButton();
            this.btnSakinAta.Text = "+ Sakin Ata";
            this.btnSakinAta.Size = new Size(120, 30);
            this.btnSakinAta.Location = new Point(1250, 110);
            this.btnSakinAta.Appearance.Font = new Font("Tahoma", 10F);
            this.btnSakinAta.Appearance.BackColor = Color.FromArgb(34, 197, 94);
            this.btnSakinAta.Appearance.ForeColor = Color.White;
            this.btnSakinAta.Appearance.Options.UseBackColor = true;
            this.btnSakinAta.Appearance.Options.UseForeColor = true;
            this.btnSakinAta.Enabled = false;
            this.btnSakinAta.Click += BtnSakinAta_Click;
            this.Controls.Add(this.btnSakinAta);

            // Grid Control
            this.gcFlats = new GridControl();
            this.gvFlats = new GridView(this.gcFlats);
            this.gcFlats.MainView = this.gvFlats;
            this.gcFlats.Location = new Point(20, 175);
            this.gcFlats.Size = new Size(1210, 475);
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
            this.gvFlats.CustomDrawCell += GvFlats_CustomDrawCell;
            this.gvFlats.FocusedRowChanged += GvFlats_FocusedRowChanged;

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
                
                // Kat Min/Max filter
                if (int.TryParse(txtKatMin.EditValue?.ToString() ?? "0", out int katMin))
                {
                    flats = flats.Where(f => f.Floor >= katMin).ToList();
                }
                if (int.TryParse(txtKatMax.EditValue?.ToString() ?? "999", out int katMax))
                {
                    flats = flats.Where(f => f.Floor <= katMax).ToList();
                }
                
                // Daire No filter
                string daireNo = txtDaireNo.EditValue?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(daireNo) && int.TryParse(daireNo, out int daireNoValue))
                {
                    flats = flats.Where(f => f.DoorNumber == daireNoValue).ToList();
                }
                
                // Daire Tipi filter
                string daireTipi = cmbDaireTipi.EditValue?.ToString() ?? "Tümü";
                if (daireTipi != "Tümü")
                {
                    flats = flats.Where(f => f.Type == daireTipi).ToList();
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
                    SakinSayisi = f.FlatResidents?.Count ?? 0,
                    IsEmpty = f.IsEmpty
                }).OrderBy(f => f.Site).ThenBy(f => f.Blok).ThenBy(f => f.Apartman).ThenBy(f => f.Kat).ThenBy(f => f.No).ToList();
                
                gcFlats.DataSource = displayData;
                gvFlats.PopulateColumns();
                
                // Hide Id column
                if (gvFlats.Columns["Id"] != null) gvFlats.Columns["Id"].Visible = false;
                
                // Configure columns
                ConfigureColumns();
            }
            catch (Exception ex)
            {
                Swal.Error("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void ConfigureColumns()
        {
            // Set column order and properties
            int visibleIndex = 0;
            
            if (gvFlats.Columns["Site"] != null)
            {
                gvFlats.Columns["Site"].VisibleIndex = visibleIndex++;
            }
            
            if (gvFlats.Columns["Blok"] != null)
            {
                gvFlats.Columns["Blok"].VisibleIndex = visibleIndex++;
            }
            
            if (gvFlats.Columns["Apartman"] != null)
            {
                gvFlats.Columns["Apartman"].VisibleIndex = visibleIndex++;
            }
            
            if (gvFlats.Columns["Kat"] != null)
            {
                gvFlats.Columns["Kat"].VisibleIndex = visibleIndex++;
                gvFlats.Columns["Kat"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            if (gvFlats.Columns["No"] != null)
            {
                gvFlats.Columns["No"].VisibleIndex = visibleIndex++;
                gvFlats.Columns["No"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            if (gvFlats.Columns["Tip"] != null)
            {
                gvFlats.Columns["Tip"].VisibleIndex = visibleIndex++;
            }
            
            if (gvFlats.Columns["Durum"] != null)
            {
                gvFlats.Columns["Durum"].VisibleIndex = visibleIndex++;
                gvFlats.Columns["Durum"].Width = 100;
            }
            
            // Aktif Sakin column - right after Durum
            if (gvFlats.Columns["SakinSayisi"] != null)
            {
                gvFlats.Columns["SakinSayisi"].VisibleIndex = visibleIndex++;
                gvFlats.Columns["SakinSayisi"].Caption = "Aktif Sakin";
                gvFlats.Columns["SakinSayisi"].Width = 120;
                gvFlats.Columns["SakinSayisi"].Visible = true;
                gvFlats.Columns["SakinSayisi"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            // Hide IsEmpty column
            if (gvFlats.Columns["IsEmpty"] != null)
            {
                gvFlats.Columns["IsEmpty"].Visible = false;
            }
            
            // Hide any other columns that might exist
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gvFlats.Columns)
            {
                if (col.FieldName != "Site" && col.FieldName != "Blok" && col.FieldName != "Apartman" && 
                    col.FieldName != "Kat" && col.FieldName != "No" && col.FieldName != "Tip" && 
                    col.FieldName != "Durum" && col.FieldName != "SakinSayisi" && col.FieldName != "Id" &&
                    col.FieldName != "IsEmpty")
                {
                    col.Visible = false;
                }
            }
        }
        
        private void GvFlats_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            // Draw status badge
            if (e.Column != null && e.Column.FieldName == "Durum")
            {
                string status = e.CellValue?.ToString() ?? "Boş";
                bool isDolu = status.Equals("Dolu", StringComparison.OrdinalIgnoreCase);
                
                // Draw badge background
                Color badgeColor = isDolu ? Color.FromArgb(34, 197, 94) : Color.FromArgb(59, 130, 246); // Green for Dolu, Blue for Boş
                using (var brush = new SolidBrush(badgeColor))
                {
                    // Rounded rectangle for badge
                    int padding = 8;
                    int height = 24;
                    int width = Math.Min(70, e.Bounds.Width - padding * 2);
                    int x = e.Bounds.X + (e.Bounds.Width - width) / 2;
                    int y = e.Bounds.Y + (e.Bounds.Height - height) / 2;
                    
                    var rect = new Rectangle(x, y, width, height);
                    e.Graphics.FillRectangle(brush, rect);
                }
                
                // Draw text
                using (var textBrush = new SolidBrush(Color.White))
                using (var font = new Font("Tahoma", 8F, FontStyle.Bold))
                {
                    var textRect = e.Bounds;
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(status, font, textBrush, textRect, sf);
                }
                
                e.Handled = true;
            }
        }
        
        private void GvFlats_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            // Enable/disable buttons based on selection
            bool hasSelection = gvFlats.FocusedRowHandle >= 0;
            btnDetay.Enabled = hasSelection;
            btnSakinAta.Enabled = hasSelection;
        }
        
        private void BtnDetay_Click(object sender, EventArgs e)
        {
            try
            {
                var row = gvFlats.GetFocusedRow();
                if (row == null)
                {
                    Swal.Warning("Lütfen detaylarını görmek için bir daire seçin.");
                    return;
                }

                var idProp = row.GetType().GetProperty("Id");
                if (idProp == null) return;

                int flatId = (int)idProp.GetValue(row);
                ShowFlatDetails(flatId);
            }
            catch (Exception ex)
            {
                Swal.Error("Detay gösterilirken hata oluştu: " + ex.Message);
            }
        }
        
        private void BtnSakinAta_Click(object sender, EventArgs e)
        {
            try
            {
                var row = gvFlats.GetFocusedRow();
                if (row == null)
                {
                    Swal.Warning("Lütfen sakin atamak için bir daire seçin.");
                    return;
                }

                var idProp = row.GetType().GetProperty("Id");
                if (idProp == null) return;

                int flatId = (int)idProp.GetValue(row);
                AssignResident(flatId);
            }
            catch (Exception ex)
            {
                Swal.Error("Sakin atama işlemi sırasında hata oluştu: " + ex.Message);
            }
        }
        
        private void ShowFlatDetails(int flatId)
        {
            // TODO: Implement flat details form
            Swal.Info($"Daire detayları gösterilecek (ID: {flatId})");
        }
        
        private void AssignResident(int flatId)
        {
            var frm = new FrmAssignResident(flatId);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
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
            txtKatMin.EditValue = "0";
            txtKatMax.EditValue = "999";
            txtDaireNo.EditValue = "";
            cmbDaireTipi.SelectedIndex = 0;
            LoadData();
        }
        
        private void GvFlats_DoubleClick(object sender, EventArgs e)
        {
            // Double click functionality can be added here if needed
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
