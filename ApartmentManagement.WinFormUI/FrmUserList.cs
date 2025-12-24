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
    public partial class FrmUserList : DevExpress.XtraEditors.XtraForm
    {
        private IUser _userService;
        
        // Controls
        private GridControl gcUsers;
        private GridView gvUsers;
        private LabelControl lblTitle;
        private LabelControl lblCount;
        private SimpleButton btnEdit;
        private SimpleButton btnDelete;

        public FrmUserList()
        {
            _userService = new SUser();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = "Kullanıcı Listesi";
            this.Size = new Size(1200, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Title
            this.lblTitle = new LabelControl();
            this.lblTitle.Text = "☰ Kullanıcı Listesi";
            this.lblTitle.Appearance.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            this.lblTitle.Location = new Point(20, 15);
            this.Controls.Add(this.lblTitle);

            // Buttons Panel (top right)
            this.btnEdit = new SimpleButton();
            this.btnEdit.Text = "Düzenle";
            this.btnEdit.Size = new Size(90, 32);
            this.btnEdit.Location = new Point(950, 15);
            this.btnEdit.Appearance.Font = new Font("Segoe UI", 10F);
            this.btnEdit.Appearance.BackColor = Color.FromArgb(66, 133, 244);
            this.btnEdit.Appearance.ForeColor = Color.White;
            this.btnEdit.Appearance.Options.UseBackColor = true;
            this.btnEdit.Appearance.Options.UseForeColor = true;
            this.btnEdit.Cursor = Cursors.Hand;
            this.btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(this.btnEdit);

            this.btnDelete = new SimpleButton();
            this.btnDelete.Text = "Sil";
            this.btnDelete.Size = new Size(70, 32);
            this.btnDelete.Location = new Point(1050, 15);
            this.btnDelete.Appearance.Font = new Font("Segoe UI", 10F);
            this.btnDelete.Appearance.BackColor = Color.FromArgb(234, 67, 53);
            this.btnDelete.Appearance.ForeColor = Color.White;
            this.btnDelete.Appearance.Options.UseBackColor = true;
            this.btnDelete.Appearance.Options.UseForeColor = true;
            this.btnDelete.Cursor = Cursors.Hand;
            this.btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(this.btnDelete);

            // User Count Badge
            this.lblCount = new LabelControl();
            this.lblCount.Text = "0 Kullanıcı";
            this.lblCount.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblCount.Appearance.BackColor = Color.FromArgb(52, 168, 83);
            this.lblCount.Appearance.ForeColor = Color.White;
            this.lblCount.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblCount.Size = new Size(100, 28);
            this.lblCount.Location = new Point(830, 18);
            this.lblCount.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Controls.Add(this.lblCount);

            // Grid Control
            this.gcUsers = new GridControl();
            this.gvUsers = new GridView(this.gcUsers);
            this.gcUsers.MainView = this.gvUsers;
            this.gcUsers.Location = new Point(20, 60);
            this.gcUsers.Size = new Size(1160, 540);
            this.gcUsers.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            
            // Grid View Settings
            this.gvUsers.OptionsBehavior.Editable = false;
            this.gvUsers.OptionsView.ShowGroupPanel = false;
            this.gvUsers.OptionsView.ShowIndicator = false;
            this.gvUsers.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvUsers.RowHeight = 50;
            this.gvUsers.Appearance.HeaderPanel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.gvUsers.Appearance.Row.Font = new Font("Segoe UI", 10F);
            this.gvUsers.DoubleClick += GvUsers_DoubleClick;
            
            this.Controls.Add(this.gcUsers);

            // Bottom info label
            var lblInfo = new LabelControl();
            lblInfo.Text = "Kayıtlı kullanıcı listesi. Çift tıklayarak düzenleyebilirsiniz.";
            lblInfo.Appearance.Font = new Font("Segoe UI", 9F);
            lblInfo.Appearance.ForeColor = Color.Gray;
            lblInfo.Location = new Point(20, 608);
            this.Controls.Add(lblInfo);

            this.ResumeLayout(false);
        }

        private void LoadData()
        {
            try
            {
                var users = _userService.GetAll();
                
                // Create display data
                var displayData = users.Select(u => new
                {
                    u.Id,
                    Kullanici = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    Telefon = u.Phone ?? "-",
                    Rol = GetRoleDisplayName(u.Role),
                    Durum = "Aktif",
                    SonGiris = "-"
                }).ToList();
                
                gcUsers.DataSource = displayData;
                gvUsers.PopulateColumns();
                
                // Hide Id
                if (gvUsers.Columns["Id"] != null) gvUsers.Columns["Id"].Visible = false;
                
                // Set column captions and widths
                if (gvUsers.Columns["Kullanici"] != null) { gvUsers.Columns["Kullanici"].Caption = "Kullanıcı"; gvUsers.Columns["Kullanici"].Width = 180; gvUsers.Columns["Kullanici"].VisibleIndex = 0; }
                if (gvUsers.Columns["Email"] != null) { gvUsers.Columns["Email"].Width = 220; gvUsers.Columns["Email"].VisibleIndex = 1; }
                if (gvUsers.Columns["Telefon"] != null) { gvUsers.Columns["Telefon"].Width = 130; gvUsers.Columns["Telefon"].VisibleIndex = 2; }
                if (gvUsers.Columns["Rol"] != null) { gvUsers.Columns["Rol"].Caption = "Roller"; gvUsers.Columns["Rol"].Width = 130; gvUsers.Columns["Rol"].VisibleIndex = 3; }
                if (gvUsers.Columns["Durum"] != null) { gvUsers.Columns["Durum"].Width = 80; gvUsers.Columns["Durum"].VisibleIndex = 4; }
                if (gvUsers.Columns["SonGiris"] != null) { gvUsers.Columns["SonGiris"].Caption = "Son Giriş"; gvUsers.Columns["SonGiris"].Width = 150; gvUsers.Columns["SonGiris"].VisibleIndex = 5; }
                
                // Update count
                lblCount.Text = $"{users.Count} Kullanıcı";
            }
            catch (Exception ex)
            {
                Swal.Error("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

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

        private void BtnEdit_Click(object sender, EventArgs e)
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
            else
            {
                Swal.Warning("Lütfen düzenlemek için bir kullanıcı seçin.");
            }
        }

        private void GvUsers_DoubleClick(object sender, EventArgs e)
        {
            BtnEdit_Click(sender, e);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var userId = GetSelectedUserId();
            if (userId.HasValue)
            {
                var row = gvUsers.GetFocusedRow();
                var nameProp = row?.GetType().GetProperty("Kullanici");
                string userName = nameProp?.GetValue(row)?.ToString() ?? "Bu kullanıcı";

                if (Swal.Confirm($"'{userName}' kullanıcısını silmek istediğinize emin misiniz?"))
                {
                    try
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
                    catch (Exception ex)
                    {
                        Swal.Error("Silme hatası: " + ex.Message);
                    }
                }
            }
            else
            {
                Swal.Warning("Lütfen silmek için bir kullanıcı seçin.");
            }
        }

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
    }
}
