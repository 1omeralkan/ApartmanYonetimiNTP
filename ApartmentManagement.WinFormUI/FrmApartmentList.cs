#nullable disable
// FrmApartmentList.cs
// Apartman Listesi Formu - Kayıtlı apartmanları listeler
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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
        private SimpleButton btnAdd;
        private SimpleButton btnEdit;
        private SimpleButton btnDelete;
        private LabelControl lblTitle;
        private LabelControl lblInfo;
        
        // Filter Controls
        private ComboBoxEdit cmbSiteFilter;
        private ComboBoxEdit cmbBlockFilter;
        private SimpleButton btnApplyFilter;
        private SimpleButton btnClearFilter;
        
        private int? _selectedSiteId = null;
        private int? _selectedBlockId = null;
        private Dictionary<string, int> _siteFilterMap = new Dictionary<string, int>();
        private Dictionary<string, int> _blockFilterMap = new Dictionary<string, int>();

        public FrmApartmentList()
        {
            _apartmentService = new SApartment();
            _siteService = new SSite();
            _blockService = new SBlock();
            InitializeComponent();
            LoadSitesForFilter();
            LoadBlocksForFilter();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = "Apartman Yönetimi";
            this.Size = new Size(1400, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            // Title
            this.lblTitle = new LabelControl();
            this.lblTitle.Text = "Apartmanlar";
            this.lblTitle.Appearance.Font = new Font("Tahoma", 14F, FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            this.lblTitle.Location = new Point(20, 20);
            this.Controls.Add(this.lblTitle);

            // Add Button (top right)
            this.btnAdd = new SimpleButton();
            this.btnAdd.Text = "+ Yeni Apartman";
            this.btnAdd.Size = new Size(140, 35);
            this.btnAdd.Location = new Point(this.Width - 150, 15);
            this.btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnAdd.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            this.btnAdd.Appearance.BackColor = Color.FromArgb(59, 130, 246); // Blue
            this.btnAdd.Appearance.ForeColor = Color.White;
            this.btnAdd.Appearance.Options.UseBackColor = true;
            this.btnAdd.Appearance.Options.UseForeColor = true;
            this.btnAdd.Cursor = Cursors.Hand;
            this.btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(this.btnAdd);

            // Edit Button (top right, before Add)
            this.btnEdit = new SimpleButton();
            this.btnEdit.Text = "Düzenle";
            this.btnEdit.Size = new Size(100, 35);
            this.btnEdit.Location = new Point(this.Width - 280, 15);
            this.btnEdit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnEdit.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            this.btnEdit.Appearance.BackColor = Color.FromArgb(59, 130, 246); // Blue
            this.btnEdit.Appearance.ForeColor = Color.White;
            this.btnEdit.Appearance.Options.UseBackColor = true;
            this.btnEdit.Appearance.Options.UseForeColor = true;
            this.btnEdit.Cursor = Cursors.Hand;
            this.btnEdit.Enabled = false; // Initially disabled
            this.btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(this.btnEdit);

            // Delete Button (top right, before Edit)
            this.btnDelete = new SimpleButton();
            this.btnDelete.Text = "Sil";
            this.btnDelete.Size = new Size(80, 35);
            this.btnDelete.Location = new Point(this.Width - 370, 15);
            this.btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnDelete.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            this.btnDelete.Appearance.BackColor = Color.FromArgb(239, 68, 68); // Red
            this.btnDelete.Appearance.ForeColor = Color.White;
            this.btnDelete.Appearance.Options.UseBackColor = true;
            this.btnDelete.Appearance.Options.UseForeColor = true;
            this.btnDelete.Cursor = Cursors.Hand;
            this.btnDelete.Enabled = false; // Initially disabled
            this.btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(this.btnDelete);

            // Filter Section
            int filterY = 60;
            
            // Site Filter ComboBox
            this.cmbSiteFilter = new ComboBoxEdit();
            this.cmbSiteFilter.Location = new Point(20, filterY);
            this.cmbSiteFilter.Size = new Size(200, 30);
            this.cmbSiteFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbSiteFilter.Properties.NullText = "Tüm Siteler";
            this.cmbSiteFilter.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            this.cmbSiteFilter.SelectedIndexChanged += CmbSiteFilter_SelectedIndexChanged;
            this.Controls.Add(this.cmbSiteFilter);

            // Block Filter ComboBox
            this.cmbBlockFilter = new ComboBoxEdit();
            this.cmbBlockFilter.Location = new Point(230, filterY);
            this.cmbBlockFilter.Size = new Size(200, 30);
            this.cmbBlockFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbBlockFilter.Properties.NullText = "Tüm Bloklar";
            this.cmbBlockFilter.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            this.Controls.Add(this.cmbBlockFilter);

            // Apply Filter Button
            this.btnApplyFilter = new SimpleButton();
            this.btnApplyFilter.Text = "Uygula";
            this.btnApplyFilter.Size = new Size(80, 30);
            this.btnApplyFilter.Location = new Point(440, filterY);
            this.btnApplyFilter.Appearance.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            this.btnApplyFilter.Appearance.BackColor = Color.FromArgb(59, 130, 246);
            this.btnApplyFilter.Appearance.ForeColor = Color.White;
            this.btnApplyFilter.Appearance.Options.UseBackColor = true;
            this.btnApplyFilter.Appearance.Options.UseForeColor = true;
            this.btnApplyFilter.Cursor = Cursors.Hand;
            this.btnApplyFilter.Click += BtnApplyFilter_Click;
            this.Controls.Add(this.btnApplyFilter);

            // Clear Filter Button
            this.btnClearFilter = new SimpleButton();
            this.btnClearFilter.Text = "Temizle";
            this.btnClearFilter.Size = new Size(80, 30);
            this.btnClearFilter.Location = new Point(530, filterY);
            this.btnClearFilter.Appearance.Font = new Font("Tahoma", 8.25F);
            this.btnClearFilter.Appearance.BackColor = Color.White;
            this.btnClearFilter.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
            this.btnClearFilter.Appearance.BorderColor = Color.FromArgb(226, 232, 240);
            this.btnClearFilter.Appearance.Options.UseBackColor = true;
            this.btnClearFilter.Appearance.Options.UseForeColor = true;
            this.btnClearFilter.Appearance.Options.UseBorderColor = true;
            this.btnClearFilter.Cursor = Cursors.Hand;
            this.btnClearFilter.Click += BtnClearFilter_Click;
            this.Controls.Add(this.btnClearFilter);

            // Grid Control
            this.gcApartments = new GridControl();
            this.gvApartments = new GridView(this.gcApartments);
            this.gcApartments.MainView = this.gvApartments;
            this.gcApartments.Location = new Point(20, filterY + 45);
            this.gcApartments.Size = new Size(this.Width - 40, this.Height - (filterY + 45 + 100));
            this.gcApartments.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            
            // Grid View Settings
            this.gvApartments.OptionsBehavior.Editable = false;
            this.gvApartments.OptionsView.ShowGroupPanel = false;
            this.gvApartments.OptionsView.ShowIndicator = false;
            this.gvApartments.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvApartments.OptionsSelection.MultiSelect = false;
            this.gvApartments.RowHeight = 50;
            this.gvApartments.Appearance.HeaderPanel.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            this.gvApartments.Appearance.HeaderPanel.BackColor = Color.FromArgb(241, 245, 249);
            this.gvApartments.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gvApartments.Appearance.Row.Font = new Font("Tahoma", 8.25F);
            this.gvApartments.Appearance.Row.Options.UseFont = true;
            this.gvApartments.DoubleClick += GvApartments_DoubleClick;
            this.gvApartments.CustomDrawCell += GvApartments_CustomDrawCell;
            this.gvApartments.FocusedRowChanged += GvApartments_FocusedRowChanged;
            
            this.Controls.Add(this.gcApartments);

            // Bottom info label
            this.lblInfo = new LabelControl();
            this.lblInfo.Text = "Kayıtlı apartman listesi. Satıra tıklayarak detay sayfasına gidebilirsiniz.";
            this.lblInfo.Appearance.Font = new Font("Tahoma", 8.25F);
            this.lblInfo.Appearance.ForeColor = Color.Gray;
            this.lblInfo.Location = new Point(20, this.Height - 60);
            this.lblInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(this.lblInfo);

            this.ResumeLayout(false);
        }

        private void LoadSitesForFilter()
        {
            try
            {
                var sites = _siteService.GetAll();
                cmbSiteFilter.Properties.Items.Clear();
                _siteFilterMap.Clear();
                
                cmbSiteFilter.Properties.Items.Add("Tüm Siteler");
                
                foreach (var site in sites)
                {
                    string displayText = site.Name;
                    cmbSiteFilter.Properties.Items.Add(displayText);
                    _siteFilterMap[displayText] = site.Id;
                }
            }
            catch (Exception ex)
            {
                Swal.Error("Siteler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void LoadBlocksForFilter()
        {
            try
            {
                List<Block> blocks;
                
                if (_selectedSiteId.HasValue)
                {
                    blocks = _blockService.GetAllBySiteId(_selectedSiteId.Value);
                }
                else
                {
                    blocks = _blockService.GetAll();
                }
                
                cmbBlockFilter.Properties.Items.Clear();
                _blockFilterMap.Clear();
                
                cmbBlockFilter.Properties.Items.Add("Tüm Bloklar");
                
                foreach (var block in blocks)
                {
                    string displayText = block.Name;
                    cmbBlockFilter.Properties.Items.Add(displayText);
                    _blockFilterMap[displayText] = block.Id;
                }
            }
            catch (Exception ex)
            {
                Swal.Error("Bloklar yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void CmbSiteFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedText = cmbSiteFilter.Text;
            if (string.IsNullOrEmpty(selectedText) || selectedText == "Tüm Siteler")
            {
                _selectedSiteId = null;
            }
            else
            {
                if (_siteFilterMap.ContainsKey(selectedText))
                {
                    _selectedSiteId = _siteFilterMap[selectedText];
                }
                else
                {
                    _selectedSiteId = null;
                }
            }
            
            LoadBlocksForFilter();
        }

        private void LoadData()
        {
            try
            {
                List<Apartment> apartments;
                
                if (_selectedBlockId.HasValue)
                {
                    apartments = _apartmentService.GetAllByBlockId(_selectedBlockId.Value);
                }
                else if (_selectedSiteId.HasValue)
                {
                    apartments = _apartmentService.GetAllBySiteId(_selectedSiteId.Value);
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
                    Durum = a.Status ?? "Aktif",
                    Kat = a.TotalFloors,
                    KatBasinaDaire = a.FlatsPerFloor,
                    ToplamDaire = a.TotalFlats
                }).ToList();
                
                gcApartments.DataSource = displayData;
                gvApartments.PopulateColumns();
                
                // Hide Id column
                if (gvApartments.Columns["Id"] != null)
                    gvApartments.Columns["Id"].Visible = false;

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
            // Ad column - blue, clickable
            if (gvApartments.Columns["Ad"] != null)
            {
                gvApartments.Columns["Ad"].Width = 150;
                gvApartments.Columns["Ad"].Caption = "Ad";
                gvApartments.Columns["Ad"].AppearanceCell.ForeColor = Color.FromArgb(59, 130, 246);
                gvApartments.Columns["Ad"].AppearanceCell.Options.UseForeColor = true;
            }
            
            // Site column
            if (gvApartments.Columns["Site"] != null)
            {
                gvApartments.Columns["Site"].Width = 150;
                gvApartments.Columns["Site"].Caption = "Site";
            }
            
            // Blok column
            if (gvApartments.Columns["Blok"] != null)
            {
                gvApartments.Columns["Blok"].Width = 120;
                gvApartments.Columns["Blok"].Caption = "Blok";
            }
            
            // Durum column - badge
            if (gvApartments.Columns["Durum"] != null)
            {
                gvApartments.Columns["Durum"].Width = 100;
                gvApartments.Columns["Durum"].Caption = "Durum";
                gvApartments.Columns["Durum"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            // Kat column
            if (gvApartments.Columns["Kat"] != null)
            {
                gvApartments.Columns["Kat"].Width = 80;
                gvApartments.Columns["Kat"].Caption = "Kat";
                gvApartments.Columns["Kat"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            // Kat Başına Daire
            if (gvApartments.Columns["KatBasinaDaire"] != null)
            {
                gvApartments.Columns["KatBasinaDaire"].Width = 130;
                gvApartments.Columns["KatBasinaDaire"].Caption = "Kat Başına Daire";
                gvApartments.Columns["KatBasinaDaire"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            // Toplam Daire
            if (gvApartments.Columns["ToplamDaire"] != null)
            {
                gvApartments.Columns["ToplamDaire"].Width = 120;
                gvApartments.Columns["ToplamDaire"].Caption = "Toplam Daire";
                gvApartments.Columns["ToplamDaire"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
        }

        private void GvApartments_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            // Draw status badge
            if (e.Column != null && e.Column.FieldName == "Durum")
            {
                string status = e.CellValue?.ToString() ?? "Aktif";
                bool isActive = status.Equals("Aktif", StringComparison.OrdinalIgnoreCase) || 
                               status.Equals("Active", StringComparison.OrdinalIgnoreCase);
                
                // Display text - show "Aktif" if status is "Aktif"
                string displayText = status.Equals("Aktif", StringComparison.OrdinalIgnoreCase) ? "Aktif" : status;
                
                // Draw badge background
                Color badgeColor = isActive ? Color.FromArgb(34, 197, 94) : Color.FromArgb(148, 163, 184); // Green or Gray
                using (var brush = new SolidBrush(badgeColor))
                {
                    // Rounded rectangle for badge
                    int padding = 8;
                    int height = 24;
                    int width = Math.Min(80, e.Bounds.Width - padding * 2);
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
                    e.Graphics.DrawString(displayText, font, textBrush, textRect, sf);
                }
                
                e.Handled = true;
            }
        }

        private void GvApartments_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            // Enable/disable edit and delete buttons based on selection
            bool hasSelection = gvApartments.FocusedRowHandle >= 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private void BtnApplyFilter_Click(object sender, EventArgs e)
        {
            try
            {
                // Get selected site
                var selectedSiteText = cmbSiteFilter.Text;
                if (string.IsNullOrEmpty(selectedSiteText) || selectedSiteText == "Tüm Siteler")
                {
                    _selectedSiteId = null;
                }
                else
                {
                    if (_siteFilterMap.ContainsKey(selectedSiteText))
                    {
                        _selectedSiteId = _siteFilterMap[selectedSiteText];
                    }
                    else
                    {
                        _selectedSiteId = null;
                    }
                }
                
                // Get selected block
                var selectedBlockText = cmbBlockFilter.Text;
                if (string.IsNullOrEmpty(selectedBlockText) || selectedBlockText == "Tüm Bloklar")
                {
                    _selectedBlockId = null;
                }
                else
                {
                    if (_blockFilterMap.ContainsKey(selectedBlockText))
                    {
                        _selectedBlockId = _blockFilterMap[selectedBlockText];
                    }
                    else
                    {
                        _selectedBlockId = null;
                    }
                }
                
                LoadData();
            }
            catch (Exception ex)
            {
                Swal.Error("Filtre uygulanırken hata oluştu: " + ex.Message);
            }
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            cmbSiteFilter.SelectedIndex = 0; // Select "Tüm Siteler"
            cmbBlockFilter.SelectedIndex = 0; // Select "Tüm Bloklar"
            _selectedSiteId = null;
            _selectedBlockId = null;
            LoadBlocksForFilter();
            LoadData();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var row = gvApartments.GetFocusedRow();
                if (row == null)
                {
                    Swal.Warning("Lütfen düzenlemek için bir apartman seçin.");
                    return;
                }

                var idProperty = row.GetType().GetProperty("Id");
                if (idProperty == null) return;

                int apartmentId = (int)idProperty.GetValue(row);
                var apartment = _apartmentService.GetById(apartmentId);
                if (apartment == null)
                {
                    Swal.Warning("Apartman bulunamadı.");
                    return;
                }

                EditApartment(apartment);
            }
            catch (Exception ex)
            {
                Swal.Error("Düzenleme işlemi sırasında hata oluştu: " + ex.Message);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var row = gvApartments.GetFocusedRow();
                if (row == null)
                {
                    Swal.Warning("Lütfen silmek için bir apartman seçin.");
                    return;
                }

                var idProperty = row.GetType().GetProperty("Id");
                var nameProperty = row.GetType().GetProperty("Ad");
                if (idProperty == null) return;

                int apartmentId = (int)idProperty.GetValue(row);
                string apartmentName = nameProperty?.GetValue(row)?.ToString() ?? "Bu apartman";
                var apartment = _apartmentService.GetById(apartmentId);
                if (apartment == null)
                {
                    Swal.Warning("Apartman bulunamadı.");
                    return;
                }

                DeleteApartment(apartment);
            }
            catch (Exception ex)
            {
                Swal.Error("Silme işlemi sırasında hata oluştu: " + ex.Message);
            }
        }

        private void EditApartment(Apartment apartment)
        {
            try
            {
                if (apartment == null)
                {
                    Swal.Warning("Düzenlenecek apartman bulunamadı.");
                    return;
                }

                var frm = new FrmApartmentManagement(apartment);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                Swal.Error("Düzenleme formu açılırken hata oluştu: " + ex.Message);
            }
        }

        private void DeleteApartment(Apartment apartment)
        {
            try
            {
                if (apartment == null)
                {
                    Swal.Warning("Silinecek apartman bulunamadı.");
                    return;
                }

                if (Swal.Confirm($"'{apartment.Name}' apartmanını silmek istediğinize emin misiniz?"))
                {
                    string result = _apartmentService.Delete(apartment.Id);
                    if (!string.IsNullOrEmpty(result))
                    {
                        Swal.Error("Silme başarısız: " + result);
                        return;
                    }

                    LoadData();
                    Swal.Success("Apartman başarıyla silindi.");
                }
            }
            catch (Exception ex)
            {
                Swal.Error("Silme işlemi sırasında hata oluştu: " + ex.Message);
            }
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
                        EditApartment(apartment);
                    }
                }
            }
        }
    }
}
