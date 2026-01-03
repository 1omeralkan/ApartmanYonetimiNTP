#nullable disable
// FrmPendingApprovals.cs
// Onay Bekleyen Kullanıcılar Formu - Admin onayı bekleyen kullanıcıları yönetir
// Standart: Tahoma 8.25pt, AutoScroll = true
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Onay bekleyen kullanıcılar formu
    /// </summary>
    public partial class FrmPendingApprovals : DevExpress.XtraEditors.XtraForm
    {
        private IUser _userService;
        private User _currentUser; // Admin user for approval tracking
        
        // Header Controls
        private LabelControl lblTitle;
        private LabelControl lblSubtitle;
        private SimpleButton btnApproveSelected;
        private SimpleButton btnRejectSelected;
        
        // Stat Cards
        private Panel pnlCardPending;
        private Panel pnlCardLast7Days;
        private Panel pnlCardApprovedToday;
        private LabelControl lblPendingCount;
        private LabelControl lblLast7DaysCount;
        private LabelControl lblApprovedTodayCount;
        
        // Grid Controls
        private GridControl gcUsers;
        private GridView gvUsers;
        private CheckEdit chkSelectAll;
        private LabelControl lblSelectionInfo;
        
        /// <summary>
        /// FrmPendingApprovals constructor
        /// </summary>
        public FrmPendingApprovals(User currentUser = null)
        {
            _userService = new SUser();
            _currentUser = currentUser;
            InitializeComponent();
            LoadData();
        }

        /// <summary>
        /// Form bileşenlerini başlatır
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings - Standart: Max 770x700, AutoScroll = true
            this.Text = "Onay Bekleyen Kullanıcılar";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            int currentY = 20;
            int leftMargin = 30;
            int rightMargin = 30;
            int contentWidth = this.Width - leftMargin - rightMargin;

            // ========== HEADER SECTION ==========
            // Title
            this.lblTitle = new LabelControl();
            this.lblTitle.Text = "⏳ Onay Bekleyen Kullanıcılar";
            this.lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            this.lblTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(this.lblTitle);

            // Subtitle
            this.lblSubtitle = new LabelControl();
            this.lblSubtitle.Text = "Admin onayı bekleyen kullanıcıları yönetin";
            this.lblSubtitle.Appearance.Font = new Font("Tahoma", 9F);
            this.lblSubtitle.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            this.lblSubtitle.Location = new Point(leftMargin, currentY + 30);
            this.Controls.Add(this.lblSubtitle);

            // Action Buttons (Top Right)
            this.btnApproveSelected = new SimpleButton();
            this.btnApproveSelected.Text = "✓ Seçilenleri Onayla";
            this.btnApproveSelected.Size = new Size(180, 35);
            this.btnApproveSelected.Location = new Point(this.Width - rightMargin - 380, currentY);
            this.btnApproveSelected.Appearance.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            this.btnApproveSelected.Appearance.BackColor = Color.FromArgb(34, 197, 94);
            this.btnApproveSelected.Appearance.ForeColor = Color.White;
            this.btnApproveSelected.Appearance.Options.UseBackColor = true;
            this.btnApproveSelected.Appearance.Options.UseForeColor = true;
            this.btnApproveSelected.Click += BtnApproveSelected_Click;
            this.Controls.Add(this.btnApproveSelected);

            this.btnRejectSelected = new SimpleButton();
            this.btnRejectSelected.Text = "⊗ Seçilenleri Reddet";
            this.btnRejectSelected.Size = new Size(180, 35);
            this.btnRejectSelected.Location = new Point(this.Width - rightMargin - 180, currentY);
            this.btnRejectSelected.Appearance.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            this.btnRejectSelected.Appearance.BackColor = Color.FromArgb(220, 38, 38);
            this.btnRejectSelected.Appearance.ForeColor = Color.White;
            this.btnRejectSelected.Appearance.Options.UseBackColor = true;
            this.btnRejectSelected.Appearance.Options.UseForeColor = true;
            this.btnRejectSelected.Click += BtnRejectSelected_Click;
            this.Controls.Add(this.btnRejectSelected);

            currentY += 70;

            // ========== STAT CARDS SECTION ==========
            int cardSpacing = 20;
            int totalCardsWidth = contentWidth - (cardSpacing * 2); // Toplam kart genişliği (boşluklar dahil)
            int cardWidth = totalCardsWidth / 3; // 3 kart için eşit genişlik
            int cardHeight = 140;
            
            // Kartları ortalamak için başlangıç X pozisyonunu hesapla
            int totalWidth = (cardWidth * 3) + (cardSpacing * 2);
            int startX = leftMargin + (contentWidth - totalWidth) / 2;

            // Pending Card (Yellow)
            this.pnlCardPending = CreateStatCard(startX, currentY, cardWidth, cardHeight, Color.FromArgb(234, 179, 8), "⏳", "Onay Bekleyen", ref this.lblPendingCount);
            
            // Last 7 Days Card (Blue)
            this.pnlCardLast7Days = CreateStatCard(startX + cardWidth + cardSpacing, currentY, cardWidth, cardHeight, Color.FromArgb(59, 130, 246), "📅", "Son 7 Gün", ref this.lblLast7DaysCount);
            
            // Approved Today Card (Green)
            this.pnlCardApprovedToday = CreateStatCard(startX + (cardWidth + cardSpacing) * 2, currentY, cardWidth, cardHeight, Color.FromArgb(34, 197, 94), "👤", "Bugün Onaylanan", ref this.lblApprovedTodayCount);

            currentY += cardHeight + 30;

            // ========== TABLE SECTION ==========
            // Table Title
            var lblTableTitle = new LabelControl();
            lblTableTitle.Text = "📋 Onay Bekleyen Kullanıcı Listesi";
            lblTableTitle.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblTableTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTableTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblTableTitle);

            // Selection Info and Select All
            this.lblSelectionInfo = new LabelControl();
            this.lblSelectionInfo.Text = "0 Kullanıcı";
            this.lblSelectionInfo.Appearance.Font = new Font("Tahoma", 9F);
            this.lblSelectionInfo.Appearance.ForeColor = Color.FromArgb(234, 179, 8);
            this.lblSelectionInfo.Location = new Point(leftMargin + 200, currentY + 3);
            this.Controls.Add(this.lblSelectionInfo);

            this.chkSelectAll = new CheckEdit();
            this.chkSelectAll.Text = "Tümünü Seç";
            this.chkSelectAll.Location = new Point(leftMargin + 300, currentY);
            this.chkSelectAll.Size = new Size(100, 20);
            this.chkSelectAll.CheckedChanged += ChkSelectAll_CheckedChanged;
            this.Controls.Add(this.chkSelectAll);

            currentY += 35;

            // Grid Control
            this.gcUsers = new GridControl();
            this.gvUsers = new GridView(this.gcUsers);
            this.gcUsers.MainView = this.gvUsers;
            this.gcUsers.Location = new Point(leftMargin, currentY);
            this.gcUsers.Size = new Size(contentWidth, this.Height - currentY - 50);
            this.gcUsers.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            
            // Grid View Settings
            this.gvUsers.OptionsBehavior.Editable = false;
            this.gvUsers.OptionsView.ShowGroupPanel = false;
            this.gvUsers.OptionsView.ShowIndicator = false;
            this.gvUsers.OptionsSelection.MultiSelect = true;
            this.gvUsers.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gvUsers.RowHeight = 50;
            this.gvUsers.Appearance.HeaderPanel.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            this.gvUsers.Appearance.Row.Font = new Font("Tahoma", 10F);
            this.gvUsers.CustomDrawCell += GvUsers_CustomDrawCell;
            this.gvUsers.SelectionChanged += GvUsers_SelectionChanged;

            this.Controls.Add(this.gcUsers);

            this.ResumeLayout(false);
        }

        private Panel CreateStatCard(int x, int y, int width, int height, Color barColor, string icon, string title, ref LabelControl countLabel)
        {
            var panel = new Panel();
            panel.Location = new Point(x, y);
            panel.Size = new Size(width, height);
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.None;

            // Top colored bar
            var bar = new Panel();
            bar.Location = new Point(0, 0);
            bar.Size = new Size(width, 4);
            bar.BackColor = barColor;
            panel.Controls.Add(bar);

            // Icon - Sol üstte, daha iyi görünüm için
            var lblIcon = new LabelControl();
            lblIcon.Text = icon;
            lblIcon.Appearance.Font = new Font("Segoe UI Emoji", 32F, FontStyle.Regular);
            lblIcon.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
            lblIcon.Location = new Point(20, 25);
            lblIcon.Size = new Size(60, 60);
            lblIcon.AutoSizeMode = LabelAutoSizeMode.None;
            lblIcon.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lblIcon.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            panel.Controls.Add(lblIcon);

            // Count Label - Sağ üstte, büyük sayı
            countLabel = new LabelControl();
            countLabel.Text = "0";
            countLabel.Appearance.Font = new Font("Tahoma", 36F, FontStyle.Bold);
            countLabel.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            countLabel.Location = new Point(width - 120, 15);
            countLabel.Size = new Size(100, 50);
            countLabel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            panel.Controls.Add(countLabel);

            // Title - Alt kısımda, ortalanmış
            var lblTitle = new LabelControl();
            lblTitle.Text = title;
            lblTitle.Appearance.Font = new Font("Tahoma", 11F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            lblTitle.Location = new Point(20, height - 35);
            lblTitle.Size = new Size(width - 40, 25);
            panel.Controls.Add(lblTitle);

            this.Controls.Add(panel);
            return panel;
        }

        private void LoadData()
        {
            try
            {
                var pendingUsers = _userService.GetPendingApprovals();
                var allUsers = _userService.GetAll();
                
                // Calculate statistics
                int pendingCount = pendingUsers.Count;
                int last7Days = pendingUsers.Count(u => u.CreatedDate >= DateTime.UtcNow.AddDays(-7));
                int approvedToday = allUsers.Count(u => u.IsApproved && u.ApprovedDate.HasValue && 
                    u.ApprovedDate.Value.Date == DateTime.UtcNow.Date);

                // Update stat cards
                this.lblPendingCount.Text = pendingCount.ToString();
                this.lblLast7DaysCount.Text = last7Days.ToString();
                this.lblApprovedTodayCount.Text = approvedToday.ToString();

                // Create display data
                var displayData = pendingUsers.Select(u => new
                {
                    u.Id,
                    IsSelected = false,
                    KullaniciAd = $"{u.FirstName} {u.LastName}",
                    KullaniciId = u.Id,
                    Email = u.Email,
                    Telefon = u.Phone ?? "-",
                    KayitTarihi = u.CreatedDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                    KayitTarihiRelative = GetRelativeTime(u.CreatedDate),
                    Rol = u.Role,
                    CreatedDate = u.CreatedDate
                }).ToList();
                
                gcUsers.DataSource = displayData;
                gvUsers.PopulateColumns();
                
                // Hide Id column
                if (gvUsers.Columns["Id"] != null) gvUsers.Columns["Id"].Visible = false;
                if (gvUsers.Columns["IsSelected"] != null) gvUsers.Columns["IsSelected"].Visible = false;
                if (gvUsers.Columns["CreatedDate"] != null) gvUsers.Columns["CreatedDate"].Visible = false;
                
                ConfigureColumns();
                UpdateSelectionInfo();
            }
            catch (Exception ex)
            {
                Swal.Error("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void ConfigureColumns()
        {
            int visibleIndex = 0;
            
            // Checkbox column (handled by MultiSelectMode)
            
            if (gvUsers.Columns["KullaniciAd"] != null)
            {
                gvUsers.Columns["KullaniciAd"].VisibleIndex = visibleIndex++;
                gvUsers.Columns["KullaniciAd"].Caption = "Kullanıcı";
                gvUsers.Columns["KullaniciAd"].Width = 200;
            }
            
            if (gvUsers.Columns["Email"] != null)
            {
                gvUsers.Columns["Email"].VisibleIndex = visibleIndex++;
                gvUsers.Columns["Email"].Width = 250;
            }
            
            if (gvUsers.Columns["Telefon"] != null)
            {
                gvUsers.Columns["Telefon"].VisibleIndex = visibleIndex++;
                gvUsers.Columns["Telefon"].Width = 150;
            }
            
            if (gvUsers.Columns["KayitTarihi"] != null)
            {
                gvUsers.Columns["KayitTarihi"].VisibleIndex = visibleIndex++;
                gvUsers.Columns["KayitTarihi"].Caption = "Kayıt Tarihi";
                gvUsers.Columns["KayitTarihi"].Width = 180;
            }
            
            if (gvUsers.Columns["Rol"] != null)
            {
                gvUsers.Columns["Rol"].VisibleIndex = visibleIndex++;
                gvUsers.Columns["Rol"].Caption = "Roller";
                gvUsers.Columns["Rol"].Width = 120;
            }
        }

        private void GvUsers_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            // Kullanıcı kolonu için özel format (Ad + ID)
            if (e.Column?.FieldName == "KullaniciAd")
            {
                var row = gvUsers.GetRow(e.RowHandle);
                if (row != null)
                {
                    var nameProp = row.GetType().GetProperty("KullaniciAd");
                    var idProp = row.GetType().GetProperty("KullaniciId");

                    string name = nameProp?.GetValue(row)?.ToString() ?? "";
                    int id = idProp != null ? (int)idProp.GetValue(row) : 0;

                    e.DisplayText = $"{name}\n# ID: {id}";
                    e.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                }
            }

            // Email kolonu için uyarı ikonu
            if (e.Column?.FieldName == "Email")
            {
                var row = gvUsers.GetRow(e.RowHandle);
                if (row != null)
                {
                    var emailProp = row.GetType().GetProperty("Email");
                    string email = emailProp?.GetValue(row)?.ToString() ?? "";
                    e.DisplayText = email;
                }
            }

            // Kayıt Tarihi kolonu için relative time
            if (e.Column?.FieldName == "KayitTarihi")
            {
                var row = gvUsers.GetRow(e.RowHandle);
                if (row != null)
                {
                    var dateProp = row.GetType().GetProperty("KayitTarihi");
                    var relativeProp = row.GetType().GetProperty("KayitTarihiRelative");
                    string date = dateProp?.GetValue(row)?.ToString() ?? "";
                    string relative = relativeProp?.GetValue(row)?.ToString() ?? "";
                    e.DisplayText = $"{date}\n{relative}";
                    e.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                }
            }

            // Roller kolonu için badge görünümü
            if (e.Column?.FieldName == "Rol")
            {
                var row = gvUsers.GetRow(e.RowHandle);
                if (row != null)
                {
                    var roleProp = row.GetType().GetProperty("Rol");
                    string role = roleProp?.GetValue(row)?.ToString() ?? "";
                    string displayRole = GetRoleDisplayName(role);
                    
                    e.DisplayText = displayRole;
                    e.Appearance.BackColor = Color.FromArgb(71, 85, 105);
                    e.Appearance.ForeColor = Color.White;
                    e.Appearance.Options.UseBackColor = true;
                    e.Appearance.Options.UseForeColor = true;
                }
            }
        }

        private string GetRoleDisplayName(string role)
        {
            switch (role)
            {
                case "SuperAdmin": return "Süper Admin";
                case "Admin": return "Admin";
                case "SiteManager": return "Site Yöneticisi";
                case "ApartmentManager": return "Apartman Yöneticisi";
                case "Resident": return "Sakin";
                default: return role;
            }
        }

        private string GetRelativeTime(DateTime dateTime)
        {
            var localTime = dateTime.ToLocalTime();
            var now = DateTime.Now;
            var diff = now - localTime;

            if (diff.TotalSeconds < 60)
                return $"{(int)diff.TotalSeconds} saniye önce";
            else if (diff.TotalMinutes < 60)
                return $"{(int)diff.TotalMinutes} dakika önce";
            else if (diff.TotalHours < 24)
                return $"{(int)diff.TotalHours} saat önce";
            else if (diff.TotalDays < 7)
                return $"{(int)diff.TotalDays} gün önce";
            else
                return localTime.ToString("dd.MM.yyyy");
        }

        private void ChkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelectAll.Checked)
            {
                gvUsers.SelectAll();
            }
            else
            {
                gvUsers.ClearSelection();
            }
            UpdateSelectionInfo();
        }

        private void GvUsers_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            UpdateSelectionInfo();
        }

        private void UpdateSelectionInfo()
        {
            int selectedCount = gvUsers.SelectedRowsCount;
            this.lblSelectionInfo.Text = $"{selectedCount} Kullanıcı";
        }

        private void BtnApproveSelected_Click(object sender, EventArgs e)
        {
            var selectedRows = gvUsers.GetSelectedRows();
            if (selectedRows.Length == 0)
            {
                Swal.Warning("Lütfen onaylamak için en az bir kullanıcı seçin.");
                return;
            }

            if (Swal.Confirm($"Seçilen {selectedRows.Length} kullanıcıyı onaylamak istediğinize emin misiniz?", "Emin misiniz?", "Evet, Onayla", Color.FromArgb(34, 197, 94)))
            {
                int approvedCount = 0;
                int errorCount = 0;

                foreach (int rowHandle in selectedRows)
                {
                    var row = gvUsers.GetRow(rowHandle);
                    if (row != null)
                    {
                        var idProp = row.GetType().GetProperty("Id");
                        int userId = idProp != null ? (int)idProp.GetValue(row) : 0;
                        int approvedBy = _currentUser?.Id ?? 0;

                        string result = _userService.ApproveUser(userId, approvedBy);
                        if (string.IsNullOrEmpty(result))
                            approvedCount++;
                        else
                            errorCount++;
                    }
                }

                if (approvedCount > 0)
                {
                    Swal.Success($"{approvedCount} kullanıcı başarıyla onaylandı.");
                    LoadData();
                }
                if (errorCount > 0)
                {
                    Swal.Error($"{errorCount} kullanıcı onaylanırken hata oluştu.");
                }
            }
        }

        private void BtnRejectSelected_Click(object sender, EventArgs e)
        {
            var selectedRows = gvUsers.GetSelectedRows();
            if (selectedRows.Length == 0)
            {
                Swal.Warning("Lütfen reddetmek için en az bir kullanıcı seçin.");
                return;
            }

            if (Swal.Confirm($"Seçilen {selectedRows.Length} kullanıcıyı reddetmek istediğinize emin misiniz? Bu işlem geri alınamaz!", "Emin misiniz?", "Evet, Reddet", Color.FromArgb(220, 38, 38)))
            {
                int rejectedCount = 0;
                int errorCount = 0;

                foreach (int rowHandle in selectedRows)
                {
                    var row = gvUsers.GetRow(rowHandle);
                    if (row != null)
                    {
                        var idProp = row.GetType().GetProperty("Id");
                        int userId = idProp != null ? (int)idProp.GetValue(row) : 0;

                        string result = _userService.RejectUser(userId);
                        if (string.IsNullOrEmpty(result))
                            rejectedCount++;
                        else
                            errorCount++;
                    }
                }

                if (rejectedCount > 0)
                {
                    Swal.Success($"{rejectedCount} kullanıcı başarıyla reddedildi.");
                    LoadData();
                }
                if (errorCount > 0)
                {
                    Swal.Error($"{errorCount} kullanıcı reddedilirken hata oluştu.");
                }
            }
        }
    }
}

