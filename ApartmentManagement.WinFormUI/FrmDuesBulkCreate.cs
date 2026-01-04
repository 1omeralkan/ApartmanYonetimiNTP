#nullable disable
// FrmDuesBulkCreate.cs
// Toplu Aidat Oluşturma Formu - Bir apartman için tüm dairelere aidat oluşturur
// Standart: Tahoma 8.25pt
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Toplu aidat oluşturma formu
    /// </summary>
    public partial class FrmDuesBulkCreate : DevExpress.XtraEditors.XtraForm
    {
        private IDues _duesService;
        private IApartment _apartmentService;
        private ISite _siteService;
        private User _currentUser;

        // Controls
        private ComboBoxEdit cmbSite;
        private ComboBoxEdit cmbApartment;
        private SpinEdit spnAmount;
        private ComboBoxEdit cmbMonth;
        private ComboBoxEdit cmbYear;
        private LabelControl lblInfo;
        private SimpleButton btnCreate;
        private SimpleButton btnCancel;

        /// <summary>
        /// FrmDuesBulkCreate constructor
        /// </summary>
        public FrmDuesBulkCreate(User user)
        {
            _currentUser = user;
            _duesService = new SDues();
            _apartmentService = new SApartment();
            _siteService = new SSite();
            InitializeComponent();
            LoadFilters();
        }

        /// <summary>
        /// Form bileşenlerini başlatır
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = "Toplu Aidat Oluştur";
            this.ClientSize = new Size(600, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.Font = new Font("Tahoma", 8.25F);

            int leftMargin = 40;
            int rightMargin = 40;
            int topMargin = 25;
            int currentY = topMargin;
            int contentWidth = this.Width - leftMargin - rightMargin;
            int fieldHeight = 32;

            // ========== HEADER SECTION ==========
            var lblTitle = new LabelControl();
            lblTitle.Text = "📦 Toplu Aidat Oluştur";
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblTitle);

            var lblSubtitle = new LabelControl();
            lblSubtitle.Text = "Bir apartman için tüm dairelere aynı aidatı oluşturun";
            lblSubtitle.Appearance.Font = new Font("Tahoma", 9F);
            lblSubtitle.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            lblSubtitle.Location = new Point(leftMargin, currentY + 30);
            this.Controls.Add(lblSubtitle);

            currentY += 70;

            // ========== FORM PANEL ==========
            var pnlForm = new Helpers.RoundedPanel();
            pnlForm.Size = new Size(contentWidth, 300);
            pnlForm.Location = new Point(leftMargin, currentY);
            pnlForm.BackColor = Color.White;
            pnlForm.BorderRadius = 12;
            pnlForm.BorderThickness = 1;
            pnlForm.BorderColor = Color.FromArgb(226, 232, 240);
            pnlForm.Padding = new Padding(35);
            this.Controls.Add(pnlForm);

            int panelY = 30;
            int panelLeft = 35;

            // Site (SiteManager için)
            if (_currentUser?.Role == "SiteManager" || _currentUser?.Role == "SuperAdmin" || _currentUser?.Role == "Admin")
            {
                AddFieldLabel(pnlForm, "Site *", panelLeft, panelY, true);
                panelY += 20;
                cmbSite = new ComboBoxEdit();
                cmbSite.Location = new Point(panelLeft, panelY);
                cmbSite.Size = new Size(contentWidth - 70, fieldHeight);
                cmbSite.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                cmbSite.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
                cmbSite.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
                cmbSite.Properties.NullText = "Site seçiniz";
                cmbSite.SelectedIndexChanged += CmbSite_SelectedIndexChanged;
                pnlForm.Controls.Add(cmbSite);
                panelY += 45;
            }

            // Apartment
            AddFieldLabel(pnlForm, "Apartman *", panelLeft, panelY, true);
            panelY += 20;
            cmbApartment = new ComboBoxEdit();
            cmbApartment.Location = new Point(panelLeft, panelY);
            cmbApartment.Size = new Size(contentWidth - 70, fieldHeight);
            cmbApartment.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbApartment.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            cmbApartment.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            cmbApartment.Properties.NullText = "Apartman seçiniz";
            pnlForm.Controls.Add(cmbApartment);
            panelY += 45;

            // Amount
            AddFieldLabel(pnlForm, "Tutar (TL) *", panelLeft, panelY, true);
            panelY += 20;
            spnAmount = new SpinEdit();
            spnAmount.Location = new Point(panelLeft, panelY);
            spnAmount.Size = new Size(contentWidth - 70, fieldHeight);
            spnAmount.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            spnAmount.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            spnAmount.Properties.MinValue = 0;
            spnAmount.Properties.MaxValue = 999999;
            spnAmount.Properties.IsFloatValue = true;
            spnAmount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            spnAmount.Properties.EditFormat.FormatString = "N2";
            spnAmount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            spnAmount.Properties.DisplayFormat.FormatString = "N2";
            pnlForm.Controls.Add(spnAmount);
            panelY += 45;

            // Month
            AddFieldLabel(pnlForm, "Ay *", panelLeft, panelY, true);
            panelY += 20;
            cmbMonth = new ComboBoxEdit();
            cmbMonth.Location = new Point(panelLeft, panelY);
            cmbMonth.Size = new Size((contentWidth - 80) / 2, fieldHeight);
            cmbMonth.Properties.Items.AddRange(new[] { "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
                "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" });
            cmbMonth.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbMonth.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            cmbMonth.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            cmbMonth.Properties.NullText = "Ay seçiniz";
            pnlForm.Controls.Add(cmbMonth);

            // Year
            AddFieldLabel(pnlForm, "Yıl *", panelLeft + (contentWidth - 80) / 2 + 20, panelY - 20, true);
            cmbYear = new ComboBoxEdit();
            cmbYear.Location = new Point(panelLeft + (contentWidth - 80) / 2 + 20, panelY);
            cmbYear.Size = new Size((contentWidth - 80) / 2, fieldHeight);
            var years = new System.Collections.Generic.List<string>();
            for (int year = DateTime.Now.Year + 1; year >= DateTime.Now.Year - 2; year--)
            {
                years.Add(year.ToString());
            }
            cmbYear.Properties.Items.AddRange(years);
            cmbYear.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbYear.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            cmbYear.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            cmbYear.Properties.NullText = "Yıl seçiniz";
            cmbYear.SelectedIndex = 1; // Current year
            pnlForm.Controls.Add(cmbYear);
            panelY += 50;

            // Info Label
            lblInfo = new LabelControl();
            lblInfo.Text = "Seçilen apartmandaki tüm daireler için aidat oluşturulacaktır.";
            lblInfo.Appearance.Font = new Font("Tahoma", 8F);
            lblInfo.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            lblInfo.Location = new Point(panelLeft, panelY);
            lblInfo.Size = new Size(contentWidth - 70, 30);
            pnlForm.Controls.Add(lblInfo);
            panelY += 40;

            // ========== BUTTONS ==========
            btnCancel = new SimpleButton();
            btnCancel.Text = "İptal";
            btnCancel.Size = new Size(130, 38);
            btnCancel.Location = new Point(panelLeft, panelY);
            btnCancel.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnCancel.Appearance.BackColor = Color.FromArgb(241, 245, 249);
            btnCancel.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            btnCancel.Appearance.BorderColor = Color.FromArgb(226, 232, 240);
            btnCancel.Appearance.Options.UseBackColor = true;
            btnCancel.Appearance.Options.UseForeColor = true;
            btnCancel.Appearance.Options.UseBorderColor = true;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            pnlForm.Controls.Add(btnCancel);

            btnCreate = new SimpleButton();
            btnCreate.Text = "Oluştur";
            btnCreate.Size = new Size(160, 38);
            btnCreate.Location = new Point(contentWidth - 70 - 160, panelY);
            btnCreate.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnCreate.Appearance.BackColor = Color.FromArgb(34, 197, 94);
            btnCreate.Appearance.ForeColor = Color.White;
            btnCreate.Appearance.Options.UseBackColor = true;
            btnCreate.Appearance.Options.UseForeColor = true;
            btnCreate.Cursor = Cursors.Hand;
            btnCreate.Click += BtnCreate_Click;
            pnlForm.Controls.Add(btnCreate);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Alan etiketi ekler
        /// </summary>
        private void AddFieldLabel(Control parent, string text, int x, int y, bool required)
        {
            var lbl = new LabelControl();
            lbl.Text = text;
            if (required)
            {
                lbl.Appearance.ForeColor = Color.FromArgb(239, 68, 68);
            }
            else
            {
                lbl.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
            }
            lbl.Appearance.Font = new Font("Tahoma", 8.5F, FontStyle.Regular);
            lbl.Location = new Point(x, y);
            parent.Controls.Add(lbl);
        }

        /// <summary>
        /// Filtreleri yükler
        /// </summary>
        private void LoadFilters()
        {
            // Site Filter (SiteManager için)
            if (cmbSite != null)
            {
                var sites = _siteService.GetAll();
                cmbSite.Properties.Items.Clear();
                cmbSite.Properties.Items.Add("Tümü");
                foreach (var site in sites)
                {
                    cmbSite.Properties.Items.Add(site.Name);
                }
                if (cmbSite.Properties.Items.Count > 0)
                {
                    cmbSite.SelectedIndex = 0;
                }
            }

            // Apartment Filter
            var apartments = _apartmentService.GetAll();
            cmbApartment.Properties.Items.Clear();
            cmbApartment.Properties.Items.Add("Tümü");
            foreach (var apartment in apartments)
            {
                cmbApartment.Properties.Items.Add(apartment.Name);
            }
            if (cmbApartment.Properties.Items.Count > 0)
            {
                cmbApartment.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Site seçimi değiştiğinde
        /// </summary>
        private void CmbSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSite?.SelectedItem == null || cmbSite.SelectedIndex == 0)
            {
                LoadFilters();
                return;
            }

            var selectedSiteName = cmbSite.SelectedItem.ToString();
            var sites = _siteService.GetAll();
            var selectedSite = sites.FirstOrDefault(s => s.Name == selectedSiteName);
            if (selectedSite == null) return;

            var apartments = _apartmentService.GetAllBySiteId(selectedSite.Id);
            cmbApartment.Properties.Items.Clear();
            cmbApartment.Properties.Items.Add("Tümü");
            foreach (var apartment in apartments)
            {
                cmbApartment.Properties.Items.Add(apartment.Name);
            }
            cmbApartment.SelectedIndex = 0;
        }

        /// <summary>
        /// Oluştur butonuna tıklandığında
        /// </summary>
        private void BtnCreate_Click(object sender, EventArgs e)
        {
            // Validation
            if (cmbApartment?.SelectedItem == null || cmbApartment.SelectedIndex == 0)
            {
                Swal.Warning("Lütfen bir apartman seçin.");
                return;
            }

            if (spnAmount.Value <= 0)
            {
                Swal.Warning("Lütfen geçerli bir tutar girin.");
                return;
            }

            if (cmbMonth?.SelectedIndex < 0)
            {
                Swal.Warning("Lütfen bir ay seçin.");
                return;
            }

            if (cmbYear?.SelectedItem == null)
            {
                Swal.Warning("Lütfen bir yıl seçin.");
                return;
            }

            try
            {
                var apartmentName = cmbApartment.SelectedItem.ToString();
                var apartments = _apartmentService.GetAll();
                var selectedApartment = apartments.FirstOrDefault(a => a.Name == apartmentName);
                if (selectedApartment == null)
                {
                    Swal.Error("Apartman bulunamadı.");
                    return;
                }

                int month = cmbMonth.SelectedIndex + 1;
                int year = int.Parse(cmbYear.SelectedItem.ToString());
                decimal amount = spnAmount.Value;

                if (Swal.Confirm(
                    $"Seçilen apartmandaki tüm daireler için {GetMonthName(month)} {year} ayı için {amount:N2} TL tutarında aidat oluşturulacak. Devam etmek istiyor musunuz?",
                    "Toplu Aidat Oluştur",
                    "Evet, Oluştur",
                    Color.FromArgb(34, 197, 94)))
                {
                    string result = _duesService.CreateBulkDues(selectedApartment.Id, month, year, amount);
                    if (string.IsNullOrEmpty(result))
                    {
                        Swal.Success("Aidatlar başarıyla oluşturuldu.");
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        Swal.Error("Oluşturma hatası: " + result);
                    }
                }
            }
            catch (Exception ex)
            {
                Swal.Error("İşlem hatası: " + ex.Message);
            }
        }

        /// <summary>
        /// Ay adını döndürür
        /// </summary>
        private string GetMonthName(int month)
        {
            string[] months = { "", "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
                "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };
            return month >= 1 && month <= 12 ? months[month] : month.ToString();
        }
    }
}

