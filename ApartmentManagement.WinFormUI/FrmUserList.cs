#nullable disable
// FrmUserList.cs
// Kullanıcı Listesi Formu - Kayıtlı kullanıcıları listeler
// Standart: Tahoma 8.25pt, AutoScroll = true
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Menu;
using DevExpress.Utils.Menu;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Kullanıcı listesi formu - Görsel tasarıma göre yeniden tasarlandı
    /// </summary>
    public partial class FrmUserList : DevExpress.XtraEditors.XtraForm
    {
        private IUser _userService;
        
        // Header Controls
        private LabelControl lblTitle;
        private LabelControl lblSubtitle;
        private SimpleButton btnNewUser;
        private SimpleButton btnEdit;
        private SimpleButton btnDelete;
        
        // Stat Cards
        private Panel pnlCardTotal;
        private Panel pnlCardActive;
        private Panel pnlCardAdmin;
        private Panel pnlCardLast7Days;
        private LabelControl lblTotalCount;
        private LabelControl lblActiveCount;
        private LabelControl lblAdminCount;
        private LabelControl lblLast7DaysCount;
        
        // Filter Controls
        private Panel pnlFilters;
        private TextEdit txtSearch;
        private ComboBoxEdit cmbRole;
        private ComboBoxEdit cmbStatus;
        private SimpleButton btnSearch;
        private SimpleButton btnClear;
        
        // Grid Controls
        private GridControl gcUsers;
        private GridView gvUsers;
        
        /// <summary>
        /// FrmUserList constructor - Formu başlatır
        /// </summary>
        public FrmUserList()
        {
            _userService = new SUser();
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
            this.Text = "Kullanıcı Yönetimi";
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
            this.lblTitle.Text = "Kullanıcı Yönetimi";
            this.lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            this.lblTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(this.lblTitle);

            // Subtitle
            this.lblSubtitle = new LabelControl();
            this.lblSubtitle.Text = "Sistem kullanıcılarını yönetin ve takip edin";
            this.lblSubtitle.Appearance.Font = new Font("Tahoma", 9F);
            this.lblSubtitle.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            this.lblSubtitle.Location = new Point(leftMargin, currentY + 30);
            this.Controls.Add(this.lblSubtitle);

            // Edit Button (top right, before New User)
            this.btnEdit = new SimpleButton();
            this.btnEdit.Text = "Düzenle";
            this.btnEdit.Size = new Size(100, 35);
            this.btnEdit.Location = new Point(this.Width - rightMargin - 240, currentY);
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
            this.btnDelete.Location = new Point(this.Width - rightMargin - 330, currentY);
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

            // New User Button (Top Right)
            this.btnNewUser = new SimpleButton();
            this.btnNewUser.Text = "+ Yeni Kullanıcı";
            this.btnNewUser.Size = new Size(130, 35);
            this.btnNewUser.Location = new Point(this.Width - rightMargin - 130, currentY);
            this.btnNewUser.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnNewUser.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            this.btnNewUser.Appearance.BackColor = Color.FromArgb(66, 133, 244);
            this.btnNewUser.Appearance.ForeColor = Color.White;
            this.btnNewUser.Appearance.Options.UseBackColor = true;
            this.btnNewUser.Appearance.Options.UseForeColor = true;
            this.btnNewUser.Cursor = Cursors.Hand;
            this.btnNewUser.Click += BtnNewUser_Click;
            this.Controls.Add(this.btnNewUser);

            currentY += 70;

            // ========== STAT CARDS SECTION ==========
            int cardWidth = (contentWidth - 60) / 4; // 4 kart, 3 boşluk
            int cardHeight = 100;
            int cardSpacing = 20;

            // TOPLAM KULLANICI Card
            this.pnlCardTotal = CreateStatCard("TOPLAM KULLANICI", "0", Color.FromArgb(99, 102, 241), 
                leftMargin, currentY, cardWidth, cardHeight);
            this.lblTotalCount = (LabelControl)this.pnlCardTotal.Controls[1];
            this.Controls.Add(this.pnlCardTotal);

            // AKTİF KULLANICI Card
            this.pnlCardActive = CreateStatCard("AKTİF KULLANICI", "0", Color.FromArgb(34, 197, 94), 
                leftMargin + cardWidth + cardSpacing, currentY, cardWidth, cardHeight);
            this.lblActiveCount = (LabelControl)this.pnlCardActive.Controls[1];
            this.Controls.Add(this.pnlCardActive);

            // ADMİN KULLANICI Card
            this.pnlCardAdmin = CreateStatCard("ADMİN KULLANICI", "0", Color.FromArgb(234, 179, 8), 
                leftMargin + (cardWidth + cardSpacing) * 2, currentY, cardWidth, cardHeight);
            this.lblAdminCount = (LabelControl)this.pnlCardAdmin.Controls[1];
            this.Controls.Add(this.pnlCardAdmin);

            // SON 7 GÜN Card
            this.pnlCardLast7Days = CreateStatCard("SON 7 GÜN", "0", Color.FromArgb(239, 68, 68), 
                leftMargin + (cardWidth + cardSpacing) * 3, currentY, cardWidth, cardHeight);
            this.lblLast7DaysCount = (LabelControl)this.pnlCardLast7Days.Controls[1];
            this.Controls.Add(this.pnlCardLast7Days);

            currentY += cardHeight + 30;

            // ========== FILTERS SECTION ==========
            this.pnlFilters = new Panel();
            this.pnlFilters.Size = new Size(contentWidth, 80);
            this.pnlFilters.Location = new Point(leftMargin, currentY);
            this.pnlFilters.BackColor = Color.White;
            this.pnlFilters.Padding = new Padding(15);
            this.Controls.Add(this.pnlFilters);

            // Filter Title
            var lblFilterTitle = new LabelControl();
            lblFilterTitle.Text = "Gelişmiş Filtreler";
            lblFilterTitle.Appearance.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            lblFilterTitle.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            lblFilterTitle.Location = new Point(15, 10);
            this.pnlFilters.Controls.Add(lblFilterTitle);

            int filterY = 35;

            // Search TextBox
            this.txtSearch = new TextEdit();
            this.txtSearch.Properties.NullText = "Ad, soyad, email veya telefon ile ara...";
            this.txtSearch.Size = new Size(300, 28);
            this.txtSearch.Location = new Point(15, filterY);
            this.txtSearch.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            this.txtSearch.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.pnlFilters.Controls.Add(this.txtSearch);

            // Role ComboBox
            this.cmbRole = new ComboBoxEdit();
            this.cmbRole.Properties.Items.AddRange(new[] { "Tüm Roller", "SuperAdmin", "Admin", "SiteManager", "ApartmentManager", "Resident" });
            this.cmbRole.SelectedIndex = 0;
            this.cmbRole.Size = new Size(150, 28);
            this.cmbRole.Location = new Point(330, filterY);
            this.cmbRole.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            this.cmbRole.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.pnlFilters.Controls.Add(this.cmbRole);

            // Status ComboBox
            this.cmbStatus = new ComboBoxEdit();
            this.cmbStatus.Properties.Items.AddRange(new[] { "Tüm Durumlar", "Aktif", "Pasif" });
            this.cmbStatus.SelectedIndex = 0;
            this.cmbStatus.Size = new Size(150, 28);
            this.cmbStatus.Location = new Point(495, filterY);
            this.cmbStatus.Properties.Appearance.Font = new Font("Tahoma", 8.25F);
            this.cmbStatus.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.pnlFilters.Controls.Add(this.cmbStatus);

            // Search Button
            this.btnSearch = new SimpleButton();
            this.btnSearch.Text = "Ara";
            this.btnSearch.Size = new Size(80, 28);
            this.btnSearch.Location = new Point(660, filterY);
            this.btnSearch.Appearance.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            this.btnSearch.Appearance.BackColor = Color.FromArgb(66, 133, 244);
            this.btnSearch.Appearance.ForeColor = Color.White;
            this.btnSearch.Appearance.Options.UseBackColor = true;
            this.btnSearch.Appearance.Options.UseForeColor = true;
            this.btnSearch.Cursor = Cursors.Hand;
            this.btnSearch.Click += BtnSearch_Click;
            this.pnlFilters.Controls.Add(this.btnSearch);

            // Clear Button
            this.btnClear = new SimpleButton();
            this.btnClear.Text = "Temizle";
            this.btnClear.Size = new Size(80, 28);
            this.btnClear.Location = new Point(750, filterY);
            this.btnClear.Appearance.Font = new Font("Tahoma", 8.25F);
            this.btnClear.Appearance.BackColor = Color.FromArgb(240, 240, 240);
            this.btnClear.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            this.btnClear.Appearance.Options.UseBackColor = true;
            this.btnClear.Appearance.Options.UseForeColor = true;
            this.btnClear.Cursor = Cursors.Hand;
            this.btnClear.Click += BtnClear_Click;
            this.pnlFilters.Controls.Add(this.btnClear);

            currentY += 100;

            // ========== GRID SECTION ==========
            // Grid Title
            var lblGridTitle = new LabelControl();
            lblGridTitle.Text = "Kullanıcı Listesi";
            lblGridTitle.Appearance.Font = new Font("Tahoma", 11F, FontStyle.Bold);
            lblGridTitle.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            lblGridTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblGridTitle);

            // User Count Label (Top Right of Grid)
            var lblUserCount = new LabelControl();
            lblUserCount.Text = "0 Kullanıcı";
            lblUserCount.Appearance.Font = new Font("Tahoma", 9F);
            lblUserCount.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            lblUserCount.Location = new Point(this.Width - rightMargin - 100, currentY);
            lblUserCount.Tag = "userCount"; // Tag ile sonra güncelleyebiliriz
            this.Controls.Add(lblUserCount);

            currentY += 30;

            // Grid Control
            this.gcUsers = new GridControl();
            this.gvUsers = new GridView(this.gcUsers);
            this.gcUsers.MainView = this.gvUsers;
            this.gcUsers.Location = new Point(leftMargin, currentY);
            this.gcUsers.Size = new Size(contentWidth, this.Height - currentY - 40);
            this.gcUsers.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            
            // Grid View Settings
            this.gvUsers.OptionsBehavior.Editable = false;
            this.gvUsers.OptionsView.ShowGroupPanel = false;
            this.gvUsers.OptionsView.ShowIndicator = false;
            this.gvUsers.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvUsers.RowHeight = 60;
            this.gvUsers.Appearance.HeaderPanel.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            this.gvUsers.Appearance.HeaderPanel.BackColor = Color.FromArgb(248, 249, 250);
            this.gvUsers.Appearance.HeaderPanel.ForeColor = Color.FromArgb(60, 60, 60);
            this.gvUsers.Appearance.Row.Font = new Font("Tahoma", 8.25F);
            this.gvUsers.Appearance.Row.BackColor = Color.White;
            this.gvUsers.Appearance.OddRow.BackColor = Color.FromArgb(250, 250, 250);
            this.gvUsers.OptionsView.EnableAppearanceOddRow = true;
            this.gvUsers.OptionsView.EnableAppearanceEvenRow = false;
            this.gvUsers.FocusedRowChanged += GvUsers_FocusedRowChanged;
            
            this.Controls.Add(this.gcUsers);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Stat card oluşturur
        /// </summary>
        private Panel CreateStatCard(string title, string value, Color accentColor, int x, int y, int width, int height)
        {
            var card = new RoundedPanel();
            card.Size = new Size(width, height);
            card.Location = new Point(x, y);
            card.BackColor = Color.White;
            card.BorderRadius = 10;
            card.BorderThickness = 0;

            // Title
            var lblTitle = new LabelControl();
            lblTitle.Text = title;
            lblTitle.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            lblTitle.Location = new Point(20, 15);
            card.Controls.Add(lblTitle);

            // Value
            var lblValue = new LabelControl();
            lblValue.Text = value;
            lblValue.Appearance.Font = new Font("Tahoma", 24F, FontStyle.Bold);
            lblValue.Appearance.ForeColor = accentColor;
            lblValue.Location = new Point(20, 40);
            card.Controls.Add(lblValue);

            return card;
        }

        /// <summary>
        /// Kullanıcı verilerini yükler
        /// </summary>
        private void LoadData()
        {
            try
            {
                var users = _userService.GetAll();
                
                // İstatistikleri hesapla
                int totalUsers = users.Count;
                int activeUsers = users.Count; // Şimdilik hepsi aktif
                int adminUsers = users.Count(u => u.Role == "SuperAdmin" || u.Role == "Admin");
                int last7Days = users.Count(u => u.Id > 0); // Basit hesaplama, gerçekte CreatedDate kontrol edilmeli

                // Stat kartlarını güncelle
                if (lblTotalCount != null) lblTotalCount.Text = totalUsers.ToString();
                if (lblActiveCount != null) lblActiveCount.Text = activeUsers.ToString();
                if (lblAdminCount != null) lblAdminCount.Text = adminUsers.ToString();
                if (lblLast7DaysCount != null) lblLast7DaysCount.Text = last7Days.ToString();

                // Create display data
                var displayData = users.Select(u => new
                {
                    u.Id,
                    KullaniciAd = $"{u.FirstName} {u.LastName}",
                    KullaniciId = u.Id,
                    KullaniciTarih = DateTime.Now.ToString("dd.MM.yyyy"), // Gerçekte CreatedDate olmalı
                    Email = u.Email,
                    Telefon = u.Phone ?? "-",
                    Rol = u.Role,
                    Durum = "Aktif",
                    SonGiris = u.LastLoginDate.HasValue 
                        ? u.LastLoginDate.Value.ToString("dd.MM.yyyy HH:mm") 
                        : "Hiç giriş yapmamış",
                    LastLoginDate = u.LastLoginDate, // For sorting/filtering
                    UserObject = u // İşlemler için kullanıcı objesini sakla
                }).ToList();
                
                gcUsers.DataSource = displayData;
                gvUsers.PopulateColumns();
                
                // Hide Id and unnecessary columns
                if (gvUsers.Columns["Id"] != null) gvUsers.Columns["Id"].Visible = false;
                if (gvUsers.Columns["KullaniciId"] != null) gvUsers.Columns["KullaniciId"].Visible = false;
                if (gvUsers.Columns["KullaniciTarih"] != null) gvUsers.Columns["KullaniciTarih"].Visible = false;
                if (gvUsers.Columns["UserObject"] != null) gvUsers.Columns["UserObject"].Visible = false;
                if (gvUsers.Columns["LastLoginDate"] != null) gvUsers.Columns["LastLoginDate"].Visible = false;
                
                // Configure columns
                ConfigureGridColumns();
                
                // Update user count label
                var lblUserCount = this.Controls.Cast<Control>()
                    .FirstOrDefault(c => c.Tag?.ToString() == "userCount") as LabelControl;
                if (lblUserCount != null)
                {
                    lblUserCount.Text = $"{users.Count} Kullanıcı";
                }
            }
            catch (Exception ex)
            {
                Swal.Error("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Grid kolonlarını yapılandırır
        /// </summary>
        private void ConfigureGridColumns()
        {
            // Kullanıcı kolonu (Ad + ID + Tarih) - Custom format
            if (gvUsers.Columns["KullaniciAd"] != null)
            {
                var col = gvUsers.Columns["KullaniciAd"];
                col.Caption = "Kullanıcı";
                col.VisibleIndex = 0;
                col.Width = 250;
                col.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            }

            // Email kolonu
            if (gvUsers.Columns["Email"] != null)
            {
                var col = gvUsers.Columns["Email"];
                col.Caption = "Email";
                col.VisibleIndex = 1;
                col.Width = 200;
            }

            // Telefon kolonu
            if (gvUsers.Columns["Telefon"] != null)
            {
                var col = gvUsers.Columns["Telefon"];
                col.Caption = "Telefon";
                col.VisibleIndex = 2;
                col.Width = 130;
            }

            // Roller kolonu
            if (gvUsers.Columns["Rol"] != null)
            {
                var col = gvUsers.Columns["Rol"];
                col.Caption = "Roller";
                col.VisibleIndex = 3;
                col.Width = 120;
            }

            // Durum kolonu
            if (gvUsers.Columns["Durum"] != null)
            {
                var col = gvUsers.Columns["Durum"];
                col.Caption = "Durum";
                col.VisibleIndex = 4;
                col.Width = 80;
            }

            // Son Giriş kolonu
            if (gvUsers.Columns["SonGiris"] != null)
            {
                var col = gvUsers.Columns["SonGiris"];
                col.Caption = "Son Giriş";
                col.VisibleIndex = 5;
                col.Width = 150;
            }

            // Custom Draw Cell for formatting
            gvUsers.CustomDrawCell += GvUsers_CustomDrawCell;
            gvUsers.DoubleClick += GvUsers_DoubleClick;
        }

        /// <summary>
        /// Grid hücrelerini özel çizer
        /// </summary>
        private void GvUsers_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            // Kullanıcı kolonu için özel format (Ad + ID + Tarih)
            if (e.Column?.FieldName == "KullaniciAd")
            {
                var row = gvUsers.GetRow(e.RowHandle);
                if (row != null)
                {
                    var nameProp = row.GetType().GetProperty("KullaniciAd");
                    var idProp = row.GetType().GetProperty("KullaniciId");
                    var dateProp = row.GetType().GetProperty("KullaniciTarih");

                    string name = nameProp?.GetValue(row)?.ToString() ?? "";
                    int id = idProp != null ? (int)idProp.GetValue(row) : 0;
                    string date = dateProp?.GetValue(row)?.ToString() ?? "";

                    e.DisplayText = $"{name}\n#{id} • {date}";
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
                    e.Appearance.BackColor = Color.FromArgb(240, 240, 240);
                    e.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
                }
            }

            // Durum kolonu için badge görünümü
            if (e.Column?.FieldName == "Durum")
            {
                var row = gvUsers.GetRow(e.RowHandle);
                if (row != null)
                {
                    var statusProp = row.GetType().GetProperty("Durum");
                    string status = statusProp?.GetValue(row)?.ToString() ?? "";
                    
                    e.DisplayText = status;
                    if (status == "Aktif")
                    {
                        e.Appearance.BackColor = Color.FromArgb(220, 252, 231);
                        e.Appearance.ForeColor = Color.FromArgb(22, 163, 74);
                    }
                    else
                    {
                        e.Appearance.BackColor = Color.FromArgb(254, 226, 226);
                        e.Appearance.ForeColor = Color.FromArgb(220, 38, 38);
                    }
                }
            }
        }

        /// <summary>
        /// Satırdan kullanıcı ID'sini alır
        /// </summary>
        private int? GetUserIdFromRow(int rowHandle)
        {
            var row = gvUsers.GetRow(rowHandle);
            if (row != null)
            {
                var idProp = row.GetType().GetProperty("Id");
                if (idProp != null)
                {
                    return (int)idProp.GetValue(row);
                }
            }
            return null;
        }

        /// <summary>
        /// Yeni kullanıcı butonuna tıklandığında
        /// </summary>
        private void BtnNewUser_Click(object sender, EventArgs e)
        {
            var frm = new FrmUserManagement(null);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        /// <summary>
        /// Ara butonuna tıklandığında
        /// </summary>
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            // Filtreleme işlemi yapılacak
            LoadData(); // Şimdilik tüm verileri yükle
        }

        /// <summary>
        /// Temizle butonuna tıklandığında
        /// </summary>
        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            cmbRole.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 0;
            LoadData();
        }

        /// <summary>
        /// Görüntüle butonuna tıklandığında
        /// </summary>
        private void BtnView_Click(int? userId)
        {
            if (userId.HasValue)
            {
                var user = _userService.GetById(userId.Value);
                if (user != null)
                {
                    var frm = new FrmUserManagement(user);
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
        }

        /// <summary>
        /// Rol kodunu görüntüleme adına çevirir
        /// </summary>
        private string GetRoleDisplayName(string role)
        {
            return role switch
            {
                "SuperAdmin" => "Super admin",
                "Admin" => "Admin",
                "SiteManager" => "Site manager",
                "ApartmentManager" => "Apartment manager",
                "Resident" => "Resident",
                _ => role
            };
        }

        /// <summary>
        /// Seçili kullanıcının ID'sini döndürür
        /// </summary>
        private int? GetSelectedUserId()
        {
            var row = gvUsers.GetFocusedRow();
            if (row != null)
            {
                var idProp = row.GetType().GetProperty("Id");
                if (idProp != null)
                {
                    return (int)idProp.GetValue(row);
                }
            }
            return null;
        }

        /// <summary>
        /// Satıra çift tıklandığında çalışır
        /// </summary>
        private void GvUsers_DoubleClick(object sender, EventArgs e)
        {
            var userId = GetSelectedUserId();
            if (userId.HasValue)
            {
                var user = _userService.GetById(userId.Value);
                if (user != null)
                {
                    var frm = new FrmUserManagement(user);
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
        }

        /// <summary>
        /// Grid'de seçim değiştiğinde butonları aktif/pasif yapar
        /// </summary>
        private void GvUsers_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            // Enable/disable edit and delete buttons based on selection
            bool hasSelection = gvUsers.FocusedRowHandle >= 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        /// <summary>
        /// Düzenle butonuna tıklandığında
        /// </summary>
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                        var userId = GetSelectedUserId();
                if (!userId.HasValue)
                {
                    Swal.Warning("Lütfen düzenlemek için bir kullanıcı seçin.");
                    return;
                }

                            var user = _userService.GetById(userId.Value);
                if (user == null)
                            {
                    Swal.Warning("Kullanıcı bulunamadı.");
                    return;
                }

                                var frm = new FrmUserManagement(user);
                                if (frm.ShowDialog() == DialogResult.OK)
                                {
                                    LoadData();
                                }
                            }
            catch (Exception ex)
            {
                Swal.Error("Düzenleme işlemi sırasında hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Sil butonuna tıklandığında
        /// </summary>
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                        var userId = GetSelectedUserId();
                if (!userId.HasValue)
                        {
                    Swal.Warning("Lütfen silmek için bir kullanıcı seçin.");
                    return;
                }

                            var row = gvUsers.GetFocusedRow();
                            var nameProp = row?.GetType().GetProperty("KullaniciAd");
                            string userName = nameProp?.GetValue(row)?.ToString() ?? "Bu kullanıcı";

                            if (Swal.Confirm($"'{userName}' kullanıcısını silmek istediğinize emin misiniz?"))
                                {
                                    string result = _userService.Delete(userId.Value);
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        Swal.Error("Silme başarısız: " + result);
                                        return;
                                    }
                                    
                                    LoadData();
                                    Swal.Success("Kullanıcı başarıyla silindi.");
                }
                                }
                                catch (Exception ex)
                                {
                Swal.Error("Silme işlemi sırasında hata oluştu: " + ex.Message);
                                }
        }
    }
}
