#nullable disable
// FrmSystemLogs.cs
// Sistem Logları ekranı - Sadece SuperAdmin için
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.Business.Services;
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
    public class FrmSystemLogs : XtraForm
    {
        private readonly ISystemLog _logService;
        private readonly IUser _userService;
        private readonly User _currentUser;

        private GridControl gcLogs;
        private GridView gvLogs;
        private ComboBoxEdit cmbLevelFilter;
        private DateEdit dtStart;
        private DateEdit dtEnd;
        private SimpleButton btnFilter;
        private SimpleButton btnClear;

        public FrmSystemLogs(User user)
        {
            _currentUser = user;
            _logService = new SSystemLog();
            _userService = new SUser();

            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "Sistem Logları";
            this.Size = new Size(1300, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            int leftMargin = 20;
            int currentY = 20;
            int contentWidth = this.Width - (leftMargin * 2);

            // Header
            var lblTitle = new LabelControl();
            lblTitle.Text = "🧾 Sistem Logları";
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblTitle);

            var lblSubtitle = new LabelControl();
            lblSubtitle.Text = "Uygulama içi önemli olayların listesi (sadece SuperAdmin).";
            lblSubtitle.Appearance.Font = new Font("Tahoma", 9F);
            lblSubtitle.Appearance.ForeColor = Color.FromArgb(120, 120, 130);
            lblSubtitle.Location = new Point(leftMargin, currentY + 35);
            this.Controls.Add(lblSubtitle);

            currentY += 60;

            // Filter panel
            var pnlFilter = new Panel();
            pnlFilter.Location = new Point(leftMargin, currentY);
            pnlFilter.Size = new Size(contentWidth, 80);
            pnlFilter.BackColor = Color.FromArgb(240, 242, 245);
            this.Controls.Add(pnlFilter);

            int filterX = 10;
            int filterY = 10;
            int filterSpacing = 15;

            // Level filter
            var lblLevel = new LabelControl();
            lblLevel.Text = "Seviye";
            lblLevel.Location = new Point(filterX, filterY);
            lblLevel.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblLevel);

            cmbLevelFilter = new ComboBoxEdit();
            cmbLevelFilter.Location = new Point(filterX, filterY + 18);
            cmbLevelFilter.Size = new Size(120, 26);
            cmbLevelFilter.Properties.Items.AddRange(new[] { "Tümü", "INFO", "WARNING", "ERROR", "SECURITY" });
            cmbLevelFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbLevelFilter.Properties.Appearance.Font = new Font("Tahoma", 9F);
            cmbLevelFilter.SelectedIndex = 0;
            pnlFilter.Controls.Add(cmbLevelFilter);
            filterX += 120 + filterSpacing;

            // Start date
            var lblStart = new LabelControl();
            lblStart.Text = "Başlangıç";
            lblStart.Location = new Point(filterX, filterY);
            lblStart.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblStart);

            dtStart = new DateEdit();
            dtStart.Location = new Point(filterX, filterY + 18);
            dtStart.Size = new Size(140, 26);
            dtStart.Properties.Appearance.Font = new Font("Tahoma", 9F);
            dtStart.Properties.DisplayFormat.FormatString = "dd.MM.yyyy";
            dtStart.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dtStart.Properties.EditFormat.FormatString = "dd.MM.yyyy";
            dtStart.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dtStart.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            dtStart.Properties.Mask.EditMask = "dd.MM.yyyy";
            dtStart.Properties.NullText = "Başlangıç";
            pnlFilter.Controls.Add(dtStart);
            filterX += 140 + filterSpacing;

            // End date
            var lblEnd = new LabelControl();
            lblEnd.Text = "Bitiş";
            lblEnd.Location = new Point(filterX, filterY);
            lblEnd.Appearance.Font = new Font("Tahoma", 8F);
            pnlFilter.Controls.Add(lblEnd);

            dtEnd = new DateEdit();
            dtEnd.Location = new Point(filterX, filterY + 18);
            dtEnd.Size = new Size(140, 26);
            dtEnd.Properties.Appearance.Font = new Font("Tahoma", 9F);
            dtEnd.Properties.DisplayFormat.FormatString = "dd.MM.yyyy";
            dtEnd.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dtEnd.Properties.EditFormat.FormatString = "dd.MM.yyyy";
            dtEnd.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dtEnd.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            dtEnd.Properties.Mask.EditMask = "dd.MM.yyyy";
            dtEnd.Properties.NullText = "Bitiş";
            pnlFilter.Controls.Add(dtEnd);
            filterX += 140 + filterSpacing;

            // Buttons
            btnFilter = new SimpleButton();
            btnFilter.Text = "🔍 Filtrele";
            btnFilter.Size = new Size(110, 30);
            btnFilter.Location = new Point(contentWidth - 230, filterY + 18);
            btnFilter.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnFilter.Appearance.BackColor = Color.FromArgb(59, 130, 246);
            btnFilter.Appearance.ForeColor = Color.White;
            btnFilter.Appearance.Options.UseBackColor = true;
            btnFilter.Appearance.Options.UseForeColor = true;
            btnFilter.Click += (s, e) => LoadData();
            pnlFilter.Controls.Add(btnFilter);

            btnClear = new SimpleButton();
            btnClear.Text = "⊗ Temizle";
            btnClear.Size = new Size(110, 30);
            btnClear.Location = new Point(contentWidth - 115, filterY + 18);
            btnClear.Appearance.Font = new Font("Tahoma", 9F);
            btnClear.Click += (s, e) =>
            {
                cmbLevelFilter.SelectedIndex = 0;
                dtStart.EditValue = null;
                dtEnd.EditValue = null;
                LoadData();
            };
            pnlFilter.Controls.Add(btnClear);

            currentY += 100;

            // Grid
            gcLogs = new GridControl();
            gvLogs = new GridView(gcLogs);
            gcLogs.MainView = gvLogs;
            gcLogs.Location = new Point(leftMargin, currentY);
            gcLogs.Size = new Size(contentWidth, 550);
            gcLogs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            gvLogs.OptionsBehavior.Editable = false;
            gvLogs.OptionsView.ShowGroupPanel = false;
            gvLogs.OptionsView.ShowIndicator = false;
            gvLogs.RowHeight = 30;
            gvLogs.Appearance.HeaderPanel.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            gvLogs.Appearance.Row.Font = new Font("Tahoma", 8.5F);

            this.Controls.Add(gcLogs);

            this.ResumeLayout(false);
        }

        private void LoadData()
        {
            try
            {
                var logs = _logService.GetLast(500);
                var users = _userService.GetAll();

                // Filtreler
                string levelFilter = cmbLevelFilter?.EditValue?.ToString();
                DateTime? start = dtStart.EditValue as DateTime?;
                DateTime? end = dtEnd.EditValue as DateTime?;

                var query = logs.AsQueryable();

                if (!string.IsNullOrEmpty(levelFilter) && levelFilter != "Tümü")
                {
                    query = query.Where(l => l.Level == levelFilter);
                }

                if (start.HasValue)
                {
                    query = query.Where(l => l.CreatedDate >= start.Value.ToUniversalTime());
                }

                if (end.HasValue)
                {
                    var endDate = end.Value.Date.AddDays(1).ToUniversalTime();
                    query = query.Where(l => l.CreatedDate < endDate);
                }

                // Expression tree ile uyumsuzluk yaşamamak için önce listeye al, sonra join yap
                var logList = query
                    .OrderByDescending(l => l.CreatedDate)
                    .ToList();

                var data = (from l in logList
                            join u in users on l.UserId equals (int?)u.Id into lu
                            from u in lu.DefaultIfEmpty()
                            select new
                            {
                                Tarih = l.CreatedDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss"),
                                Seviye = l.Level,
                                Kategori = l.Category,
                                Mesaj = l.Message,
                                Kullanıcı = u != null ? $"{u.FirstName} {u.LastName}" : string.Empty
                            }).ToList();

                gcLogs.DataSource = data;
                gvLogs.BestFitColumns();
            }
            catch (Exception ex)
            {
                Swal.Error("Loglar yüklenirken hata oluştu: " + ex.Message);
            }
        }
    }
}


