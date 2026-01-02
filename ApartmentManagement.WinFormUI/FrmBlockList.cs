#nullable disable
// FrmBlockList.cs
// Blok Listesi Formu - Kayıtlı blokları listeler
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
    /// Blok listesi formu
    /// </summary>
    public partial class FrmBlockList : DevExpress.XtraEditors.XtraForm
    {
        private IBlock _blockService;
        private ISite _siteService;
        
        // Controls
        private GridControl gcBlocks;
        private GridView gvBlocks;
        private SimpleButton btnAdd;
        private SimpleButton btnEdit;
        private SimpleButton btnDelete;
        private LabelControl lblTitle;
        private LabelControl lblInfo;
        private LabelControl lblPagination;
        
        // Filter Controls
        private ComboBoxEdit cmbSiteFilter;
        private SimpleButton btnApplyFilter;
        private SimpleButton btnClearFilter;
        
        private int? _selectedSiteId = null;
        private Dictionary<string, int> _siteFilterMap = new Dictionary<string, int>();

        public FrmBlockList()
        {
            _blockService = new SBlock();
            _siteService = new SSite();
            InitializeComponent();
            LoadSitesForFilter();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = "Blok Yönetimi";
            this.Size = new Size(1400, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            // Title
            this.lblTitle = new LabelControl();
            this.lblTitle.Text = "Bloklar";
            this.lblTitle.Appearance.Font = new Font("Tahoma", 14F, FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            this.lblTitle.Location = new Point(20, 20);
            this.Controls.Add(this.lblTitle);

            // Add Button (top right)
            this.btnAdd = new SimpleButton();
            this.btnAdd.Text = "Yeni Blok";
            this.btnAdd.Size = new Size(120, 35);
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
            this.Controls.Add(this.cmbSiteFilter);

            // Apply Filter Button
            this.btnApplyFilter = new SimpleButton();
            this.btnApplyFilter.Text = "Uygula";
            this.btnApplyFilter.Size = new Size(80, 30);
            this.btnApplyFilter.Location = new Point(230, filterY);
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
            this.btnClearFilter.Location = new Point(320, filterY);
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
            this.gcBlocks = new GridControl();
            this.gvBlocks = new GridView(this.gcBlocks);
            this.gcBlocks.MainView = this.gvBlocks;
            this.gcBlocks.Location = new Point(20, filterY + 45);
            this.gcBlocks.Size = new Size(this.Width - 40, this.Height - (filterY + 45 + 100));
            this.gcBlocks.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            
            // Grid View Settings
            this.gvBlocks.OptionsBehavior.Editable = false;
            this.gvBlocks.OptionsView.ShowGroupPanel = false;
            this.gvBlocks.OptionsView.ShowIndicator = false;
            this.gvBlocks.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvBlocks.OptionsSelection.MultiSelect = false;
            this.gvBlocks.RowHeight = 50;
            this.gvBlocks.Appearance.HeaderPanel.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            this.gvBlocks.Appearance.HeaderPanel.BackColor = Color.FromArgb(241, 245, 249);
            this.gvBlocks.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gvBlocks.Appearance.Row.Font = new Font("Tahoma", 8.25F);
            this.gvBlocks.Appearance.Row.Options.UseFont = true;
            this.gvBlocks.DoubleClick += GvBlocks_DoubleClick;
            this.gvBlocks.CustomDrawCell += GvBlocks_CustomDrawCell;
            this.gvBlocks.FocusedRowChanged += GvBlocks_FocusedRowChanged;
            
            this.Controls.Add(this.gcBlocks);

            // Bottom info label
            this.lblInfo = new LabelControl();
            this.lblInfo.Text = "Kayıtlı blok listesi. Satıra tıklayarak detay sayfasına gidebilirsiniz.";
            this.lblInfo.Appearance.Font = new Font("Tahoma", 8.25F);
            this.lblInfo.Appearance.ForeColor = Color.Gray;
            this.lblInfo.Location = new Point(20, this.Height - 60);
            this.lblInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(this.lblInfo);

            // Pagination label
            this.lblPagination = new LabelControl();
            this.lblPagination.Appearance.Font = new Font("Tahoma", 8.25F);
            this.lblPagination.Appearance.ForeColor = Color.Gray;
            this.lblPagination.Location = new Point(20, this.Height - 40);
            this.lblPagination.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(this.lblPagination);

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
                    string displayText = $"{site.Name} ({site.Code ?? site.Id.ToString("D5")})";
                    cmbSiteFilter.Properties.Items.Add(displayText);
                    _siteFilterMap[displayText] = site.Id;
                }
            }
            catch (Exception ex)
            {
                Swal.Error("Siteler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void LoadData()
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
                
                // Create display data
                var displayData = blocks.Select(b => new
                {
                    b.Id,
                    Ad = b.Name,
                    Site = b.Site != null ? $"{b.Site.Name} ({b.Site.Code ?? b.Site.Id.ToString("D5")})" : "-",
                    ToplamApartman = b.TotalApartments,
                    ToplamKat = b.TotalFloors,
                    KatBasinaDaire = b.FlatsPerFloor,
                    ToplamDaire = b.TotalFlats,
                    Durum = b.Status ?? "Aktif",
                    Olusturulma = b.CreatedDate.ToString("dd.MM.yyyy")
                }).ToList();
                
                gcBlocks.DataSource = displayData;
                gvBlocks.PopulateColumns();
                
                // Hide Id column
                if (gvBlocks.Columns["Id"] != null)
                    gvBlocks.Columns["Id"].Visible = false;

                // Configure columns
                ConfigureColumns();

                // Update pagination
                UpdatePagination(blocks.Count);
            }
            catch (Exception ex)
            {
                Swal.Error("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void ConfigureColumns()
        {
            // Ad column - blue, clickable
            if (gvBlocks.Columns["Ad"] != null)
            {
                gvBlocks.Columns["Ad"].Width = 120;
                gvBlocks.Columns["Ad"].Caption = "Ad";
                gvBlocks.Columns["Ad"].AppearanceCell.ForeColor = Color.FromArgb(59, 130, 246);
                gvBlocks.Columns["Ad"].AppearanceCell.Options.UseForeColor = true;
            }
            
            // Site column
            if (gvBlocks.Columns["Site"] != null)
            {
                gvBlocks.Columns["Site"].Width = 200;
                gvBlocks.Columns["Site"].Caption = "Site";
            }
            
            // Toplam Apartman
            if (gvBlocks.Columns["ToplamApartman"] != null)
            {
                gvBlocks.Columns["ToplamApartman"].Width = 120;
                gvBlocks.Columns["ToplamApartman"].Caption = "Toplam Apartman";
                gvBlocks.Columns["ToplamApartman"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            // Toplam Kat
            if (gvBlocks.Columns["ToplamKat"] != null)
            {
                gvBlocks.Columns["ToplamKat"].Width = 100;
                gvBlocks.Columns["ToplamKat"].Caption = "Toplam Kat";
                gvBlocks.Columns["ToplamKat"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            // Kat Başına Daire
            if (gvBlocks.Columns["KatBasinaDaire"] != null)
            {
                gvBlocks.Columns["KatBasinaDaire"].Width = 130;
                gvBlocks.Columns["KatBasinaDaire"].Caption = "Kat Başına Daire";
                gvBlocks.Columns["KatBasinaDaire"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            // Toplam Daire
            if (gvBlocks.Columns["ToplamDaire"] != null)
            {
                gvBlocks.Columns["ToplamDaire"].Width = 120;
                gvBlocks.Columns["ToplamDaire"].Caption = "Toplam Daire";
                gvBlocks.Columns["ToplamDaire"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            // Durum column - badge
            if (gvBlocks.Columns["Durum"] != null)
            {
                gvBlocks.Columns["Durum"].Width = 100;
                gvBlocks.Columns["Durum"].Caption = "Durum";
                gvBlocks.Columns["Durum"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            // Oluşturulma
            if (gvBlocks.Columns["Olusturulma"] != null)
            {
                gvBlocks.Columns["Olusturulma"].Width = 120;
                gvBlocks.Columns["Olusturulma"].Caption = "Oluşturulma";
                gvBlocks.Columns["Olusturulma"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
        }

        private void GvBlocks_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
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

        private void GvBlocks_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            // Enable/disable edit and delete buttons based on selection
            bool hasSelection = gvBlocks.FocusedRowHandle >= 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private void BtnApplyFilter_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedText = cmbSiteFilter.Text;
                if (string.IsNullOrEmpty(selectedText) || selectedText == "Tüm Siteler")
                {
                    _selectedSiteId = null;
                }
                else
                {
                    // Get SiteId from dictionary
                    if (_siteFilterMap.ContainsKey(selectedText))
                    {
                        _selectedSiteId = _siteFilterMap[selectedText];
                    }
                    else
                    {
                        _selectedSiteId = null;
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
            _selectedSiteId = null;
            LoadData();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var row = gvBlocks.GetFocusedRow();
                if (row == null)
                {
                    Swal.Warning("Lütfen düzenlemek için bir blok seçin.");
                    return;
                }

                var idProperty = row.GetType().GetProperty("Id");
                if (idProperty == null) return;

                int blockId = (int)idProperty.GetValue(row);
                var block = _blockService.GetById(blockId);
                if (block == null)
                {
                    Swal.Warning("Blok bulunamadı.");
                    return;
                }

                EditBlock(block);
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
                var row = gvBlocks.GetFocusedRow();
                if (row == null)
                {
                    Swal.Warning("Lütfen silmek için bir blok seçin.");
                    return;
                }

                var idProperty = row.GetType().GetProperty("Id");
                if (idProperty == null) return;

                int blockId = (int)idProperty.GetValue(row);
                var block = _blockService.GetById(blockId);
                if (block == null)
                {
                    Swal.Warning("Blok bulunamadı.");
                    return;
                }

                DeleteBlock(block);
            }
            catch (Exception ex)
            {
                Swal.Error("Silme işlemi sırasında hata oluştu: " + ex.Message);
            }
        }

        private void EditBlock(Block block)
        {
            try
            {
                if (block == null)
                {
                    Swal.Warning("Düzenlenecek blok bulunamadı.");
                    return;
                }

                var frm = new FrmBlockManagement(block);
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

        private void DeleteBlock(Block block)
        {
            try
            {
                if (block == null)
                {
                    Swal.Warning("Silinecek blok bulunamadı.");
                    return;
                }

                if (Swal.Confirm($"'{block.Name}' blokunu silmek istediğinize emin misiniz?"))
                {
                    string result = _blockService.Delete(block.Id);
                    if (!string.IsNullOrEmpty(result))
                    {
                        Swal.Error("Silme başarısız: " + result);
                        return;
                    }

                    LoadData();
                    Swal.Success("Blok başarıyla silindi.");
                }
            }
            catch (Exception ex)
            {
                Swal.Error("Silme işlemi sırasında hata oluştu: " + ex.Message);
            }
        }

        private void UpdatePagination(int totalRecords)
        {
            int pageSize = 10; // Default page size
            int startRecord = 1;
            int endRecord = Math.Min(startRecord + pageSize - 1, totalRecords);
            
            lblPagination.Text = $"{startRecord}-{endRecord} / {totalRecords} kayıt";
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
                        EditBlock(block);
                    }
                }
            }
        }
    }
}

