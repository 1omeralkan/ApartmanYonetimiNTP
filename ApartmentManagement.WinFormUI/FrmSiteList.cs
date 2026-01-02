#nullable disable
// FrmSiteList.cs
// Site Listesi Formu - Kayıtlı siteleri listeler
// Görseldeki kolonlar: Ad, Kod, Durum, Blok, Apartman, Blok Başına Apartman, Apartman Başına Kat, Toplam Kat, Kat Başına Daire, Toplam Daire, İşlemler
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
    /// Site listesi formu
    /// </summary>
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
        private LabelControl lblInfo;
        private LabelControl lblPagination;

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
            this.Size = new Size(1400, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            // Title
            this.lblTitle = new LabelControl();
            this.lblTitle.Text = "Siteler";
            this.lblTitle.Appearance.Font = new Font("Tahoma", 14F, FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            this.lblTitle.Location = new Point(20, 20);
            this.Controls.Add(this.lblTitle);

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

            // Add Button (top right)
            this.btnAdd = new SimpleButton();
            this.btnAdd.Text = "Yeni Site";
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

            // Grid Control
            this.gcSites = new GridControl();
            this.gvSites = new GridView(this.gcSites);
            this.gcSites.MainView = this.gvSites;
            this.gcSites.Location = new Point(20, 70);
            this.gcSites.Size = new Size(this.Width - 40, this.Height - 150);
            this.gcSites.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            
            // Grid View Settings
            this.gvSites.OptionsBehavior.Editable = false;
            this.gvSites.OptionsView.ShowGroupPanel = false;
            this.gvSites.OptionsView.ShowIndicator = false;
            this.gvSites.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvSites.OptionsSelection.MultiSelect = false;
            this.gvSites.RowHeight = 50;
            this.gvSites.Appearance.HeaderPanel.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            this.gvSites.Appearance.HeaderPanel.BackColor = Color.FromArgb(241, 245, 249);
            this.gvSites.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gvSites.Appearance.Row.Font = new Font("Tahoma", 8.25F);
            this.gvSites.Appearance.Row.Options.UseFont = true;
            this.gvSites.DoubleClick += GvSites_DoubleClick;
            this.gvSites.CustomDrawCell += GvSites_CustomDrawCell;
            this.gvSites.FocusedRowChanged += GvSites_FocusedRowChanged;
            
            this.Controls.Add(this.gcSites);

            // Bottom info label
            this.lblInfo = new LabelControl();
            this.lblInfo.Text = "Kayıtlı site listesi. Satıra tıklayarak detay sayfasına gidebilirsiniz.";
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
                    Kod = s.Code ?? "-",
                    Durum = s.Status ?? "Aktif",
                    Blok = s.TotalBlocks,
                    Apartman = s.TotalBlocks * s.ApartmentsPerBlock,
                    BlokBasinaApartman = s.ApartmentsPerBlock,
                    ApartmanBasinaKat = s.FloorsPerApartment,
                    ToplamKat = s.TotalBlocks * s.ApartmentsPerBlock * s.FloorsPerApartment,
                    KatBasinaDaire = s.FlatsPerFloor,
                    ToplamDaire = s.TotalBlocks * s.ApartmentsPerBlock * s.FloorsPerApartment * s.FlatsPerFloor
                }).ToList();
                
                gcSites.DataSource = displayData;
                gvSites.PopulateColumns();
                
                // Hide Id column
                if (gvSites.Columns["Id"] != null)
                    gvSites.Columns["Id"].Visible = false;

                // Configure columns
                ConfigureColumns();

                // Update pagination
                UpdatePagination(sites.Count);
            }
            catch (Exception ex)
            {
                Swal.Error("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void ConfigureColumns()
        {
            // Set column widths and alignments
            if (gvSites.Columns["Ad"] != null)
            {
                gvSites.Columns["Ad"].Width = 150;
                gvSites.Columns["Ad"].Caption = "Ad";
            }
            
            if (gvSites.Columns["Kod"] != null)
            {
                gvSites.Columns["Kod"].Width = 100;
                gvSites.Columns["Kod"].Caption = "Kod";
            }
            
            if (gvSites.Columns["Durum"] != null)
            {
                gvSites.Columns["Durum"].Width = 100;
                gvSites.Columns["Durum"].Caption = "Durum";
                // Format status with badge
                gvSites.Columns["Durum"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            if (gvSites.Columns["Blok"] != null)
            {
                gvSites.Columns["Blok"].Width = 80;
                gvSites.Columns["Blok"].Caption = "Blok";
                gvSites.Columns["Blok"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            if (gvSites.Columns["Apartman"] != null)
            {
                gvSites.Columns["Apartman"].Width = 100;
                gvSites.Columns["Apartman"].Caption = "Apartman";
                gvSites.Columns["Apartman"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            if (gvSites.Columns["BlokBasinaApartman"] != null)
            {
                gvSites.Columns["BlokBasinaApartman"].Width = 150;
                gvSites.Columns["BlokBasinaApartman"].Caption = "Blok Başına Apartman";
                gvSites.Columns["BlokBasinaApartman"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            if (gvSites.Columns["ApartmanBasinaKat"] != null)
            {
                gvSites.Columns["ApartmanBasinaKat"].Width = 150;
                gvSites.Columns["ApartmanBasinaKat"].Caption = "Apartman Başına Kat";
                gvSites.Columns["ApartmanBasinaKat"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            if (gvSites.Columns["ToplamKat"] != null)
            {
                gvSites.Columns["ToplamKat"].Width = 100;
                gvSites.Columns["ToplamKat"].Caption = "Toplam Kat";
                gvSites.Columns["ToplamKat"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            if (gvSites.Columns["KatBasinaDaire"] != null)
            {
                gvSites.Columns["KatBasinaDaire"].Width = 120;
                gvSites.Columns["KatBasinaDaire"].Caption = "Kat Başına Daire";
                gvSites.Columns["KatBasinaDaire"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            
            if (gvSites.Columns["ToplamDaire"] != null)
            {
                gvSites.Columns["ToplamDaire"].Width = 120;
                gvSites.Columns["ToplamDaire"].Caption = "Toplam Daire";
                gvSites.Columns["ToplamDaire"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
        }


        private void GvSites_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            // Draw status badge
            if (e.Column != null && e.Column.FieldName == "Durum")
            {
                string status = e.CellValue?.ToString() ?? "Aktif";
                bool isActive = status.Equals("Aktif", StringComparison.OrdinalIgnoreCase) || 
                               status.Equals("Active", StringComparison.OrdinalIgnoreCase);
                
                // Display text - show "Active" if status is "Aktif" for consistency with image
                string displayText = status.Equals("Aktif", StringComparison.OrdinalIgnoreCase) ? "Active" : status;
                
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

        private void GvSites_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            // Enable/disable edit and delete buttons based on selection
            bool hasSelection = gvSites.FocusedRowHandle >= 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var row = gvSites.GetFocusedRow();
                if (row == null)
                {
                    Swal.Warning("Lütfen düzenlemek için bir site seçin.");
                    return;
                }

                var idProperty = row.GetType().GetProperty("Id");
                if (idProperty == null) return;

                int siteId = (int)idProperty.GetValue(row);
                var site = _siteService.GetAll().FirstOrDefault(s => s.Id == siteId);
                if (site == null)
                {
                    Swal.Warning("Site bulunamadı.");
                    return;
                }

                EditSite(site);
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
                var row = gvSites.GetFocusedRow();
                if (row == null)
                {
                    Swal.Warning("Lütfen silmek için bir site seçin.");
                    return;
                }

                var idProperty = row.GetType().GetProperty("Id");
                if (idProperty == null) return;

                int siteId = (int)idProperty.GetValue(row);
                var site = _siteService.GetAll().FirstOrDefault(s => s.Id == siteId);
                if (site == null)
                {
                    Swal.Warning("Site bulunamadı.");
                    return;
                }

                DeleteSite(site);
            }
            catch (Exception ex)
            {
                Swal.Error("Silme işlemi sırasında hata oluştu: " + ex.Message);
            }
        }

        private void EditSite(Site site)
        {
            try
            {
                if (site == null)
                {
                    Swal.Warning("Düzenlenecek site bulunamadı.");
                    return;
                }

                var frm = new FrmSiteManagement(site);
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

        private void DeleteSite(Site site)
        {
            try
            {
                if (site == null)
                {
                    Swal.Warning("Silinecek site bulunamadı.");
                    return;
                }

                if (Swal.Confirm($"'{site.Name}' sitesini silmek istediğinize emin misiniz?"))
                {
                    string result = _siteService.Delete(site.Id);
                    if (!string.IsNullOrEmpty(result))
                    {
                        Swal.Error("Silme başarısız: " + result);
                        return;
                    }

                    LoadData();
                    Swal.Success("Site başarıyla silindi.");
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
            var frm = new FrmSiteManagement();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void GvSites_DoubleClick(object sender, EventArgs e)
        {
            var row = gvSites.GetFocusedRow();
            if (row != null)
            {
                var idProperty = row.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    int siteId = (int)idProperty.GetValue(row);
                    var site = _siteService.GetAll().FirstOrDefault(s => s.Id == siteId);
                    if (site != null)
                    {
                        EditSite(site);
                    }
                }
            }
        }
    }
}

