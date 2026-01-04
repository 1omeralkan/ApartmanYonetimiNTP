#nullable disable
// FrmDuesManagement.cs
// Aidat Yönetim Formu - Aidat ekleme ve düzenleme
// Standart: Tahoma 8.25pt, AutoScroll = true
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
    /// Aidat yönetim formu - Aidat ekleme ve düzenleme
    /// </summary>
    public partial class FrmDuesManagement : DevExpress.XtraEditors.XtraForm
    {
        private IDues _duesService;
        private IFlat _flatService;
        private IApartment _apartmentService;
        private ISite _siteService;
        private Dues _currentDues;
        private User _currentUser;
        private bool _isEditMode;

        // Controls
        private ComboBoxEdit cmbSite;
        private ComboBoxEdit cmbApartment;
        private ComboBoxEdit cmbFlat;
        private SpinEdit spnAmount;
        private ComboBoxEdit cmbMonth;
        private ComboBoxEdit cmbYear;
        private CheckEdit chkIsPaid;
        private SimpleButton btnSave;
        private SimpleButton btnCancel;

        /// <summary>
        /// FrmDuesManagement constructor
        /// </summary>
        public FrmDuesManagement(Dues dues, User user)
        {
            _currentDues = dues;
            _currentUser = user;
            _isEditMode = dues != null;
            _duesService = new SDues();
            _flatService = new SFlat();
            _apartmentService = new SApartment();
            _siteService = new SSite();
            InitializeComponent();
            LoadFilters();
            if (_isEditMode)
            {
                LoadData();
            }
        }

        /// <summary>
        /// Form bileşenlerini başlatır
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = _isEditMode ? "Aidat Düzenle" : "Yeni Aidat Ekle";
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            int formHeight = Math.Min((int)(screenHeight * 0.7), 700);
            this.ClientSize = new Size(800, formHeight);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.AutoScroll = true;
            this.Font = new Font("Tahoma", 8.25F);

            int leftMargin = 40;
            int rightMargin = 40;
            int topMargin = 25;
            int currentY = topMargin;
            int contentWidth = this.Width - leftMargin - rightMargin;
            int columnWidth = (contentWidth - 30) / 2;
            int fieldHeight = 32;

            // ========== HEADER SECTION ==========
            var lblTitle = new LabelControl();
            lblTitle.Text = _isEditMode ? "Aidat Düzenle" : "Yeni Aidat Ekle";
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblTitle);

            var lblSubtitle = new LabelControl();
            lblSubtitle.Text = _isEditMode ? "Aidat bilgilerini düzenleyin" : "Yeni bir aidat kaydı oluşturun";
            lblSubtitle.Appearance.Font = new Font("Tahoma", 9F);
            lblSubtitle.Appearance.ForeColor = Color.FromArgb(100, 100, 100);
            lblSubtitle.Location = new Point(leftMargin, currentY + 30);
            this.Controls.Add(lblSubtitle);

            var btnBack = new SimpleButton();
            btnBack.Text = "Geri Dön";
            btnBack.Size = new Size(110, 36);
            btnBack.Location = new Point(this.Width - rightMargin - 110, currentY + 5);
            btnBack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBack.Appearance.Font = new Font("Tahoma", 9F);
            btnBack.Appearance.BackColor = Color.FromArgb(241, 245, 249);
            btnBack.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
            btnBack.Appearance.BorderColor = Color.FromArgb(226, 232, 240);
            btnBack.Appearance.Options.UseBackColor = true;
            btnBack.Appearance.Options.UseForeColor = true;
            btnBack.Appearance.Options.UseBorderColor = true;
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnBack);

            currentY += 75;

            // ========== FORM PANEL ==========
            var pnlForm = new Helpers.RoundedPanel();
            pnlForm.Size = new Size(contentWidth, 600);
            pnlForm.Location = new Point(leftMargin, currentY);
            pnlForm.BackColor = Color.White;
            pnlForm.BorderRadius = 12;
            pnlForm.BorderThickness = 1;
            pnlForm.BorderColor = Color.FromArgb(226, 232, 240);
            pnlForm.Padding = new Padding(35);
            this.Controls.Add(pnlForm);

            int panelY = 30;
            int panelLeft = 35;

            // ========== DAİRE SEÇİMİ SECTION ==========
            var lblSectionDaire = new LabelControl();
            lblSectionDaire.Text = "Daire Bilgileri";
            lblSectionDaire.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblSectionDaire.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblSectionDaire.Location = new Point(panelLeft, panelY);
            pnlForm.Controls.Add(lblSectionDaire);
            panelY += 40;

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
            cmbApartment.SelectedIndexChanged += CmbApartment_SelectedIndexChanged;
            pnlForm.Controls.Add(cmbApartment);
            panelY += 45;

            // Flat
            AddFieldLabel(pnlForm, "Daire *", panelLeft, panelY, true);
            panelY += 20;
            cmbFlat = new ComboBoxEdit();
            cmbFlat.Location = new Point(panelLeft, panelY);
            cmbFlat.Size = new Size(contentWidth - 70, fieldHeight);
            cmbFlat.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbFlat.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            cmbFlat.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            cmbFlat.Properties.NullText = "Daire seçiniz";
            pnlForm.Controls.Add(cmbFlat);
            panelY += 50;

            // ========== AİDAT BİLGİLERİ SECTION ==========
            var lblSectionDues = new LabelControl();
            lblSectionDues.Text = "Aidat Bilgileri";
            lblSectionDues.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblSectionDues.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblSectionDues.Location = new Point(panelLeft, panelY);
            pnlForm.Controls.Add(lblSectionDues);
            panelY += 40;

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
            cmbMonth.Size = new Size(columnWidth, fieldHeight);
            cmbMonth.Properties.Items.AddRange(new[] { "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
                "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" });
            cmbMonth.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbMonth.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            cmbMonth.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            cmbMonth.Properties.NullText = "Ay seçiniz";
            pnlForm.Controls.Add(cmbMonth);
            panelY += 45;

            // Year
            AddFieldLabel(pnlForm, "Yıl *", panelLeft + columnWidth + 30, panelY - 65, true);
            cmbYear = new ComboBoxEdit();
            cmbYear.Location = new Point(panelLeft + columnWidth + 30, panelY - 45);
            cmbYear.Size = new Size(columnWidth, fieldHeight);
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
            if (!_isEditMode)
            {
                cmbYear.SelectedIndex = 1; // Current year
            }
            pnlForm.Controls.Add(cmbYear);
            panelY += 30;

            // IsPaid
            chkIsPaid = new CheckEdit();
            chkIsPaid.Text = "Ödendi olarak işaretle";
            chkIsPaid.Location = new Point(panelLeft, panelY);
            chkIsPaid.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            pnlForm.Controls.Add(chkIsPaid);
            panelY += 50;

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

            btnSave = new SimpleButton();
            btnSave.Text = _isEditMode ? "Güncelle" : "Kaydet";
            btnSave.Size = new Size(160, 38);
            btnSave.Location = new Point(contentWidth - 70 - 160, panelY);
            btnSave.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnSave.Appearance.BackColor = Color.FromArgb(59, 130, 246);
            btnSave.Appearance.ForeColor = Color.White;
            btnSave.Appearance.Options.UseBackColor = true;
            btnSave.Appearance.Options.UseForeColor = true;
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += BtnSave_Click;
            pnlForm.Controls.Add(btnSave);

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
            if (cmbSite?.SelectedItem == null || cmbSite.SelectedIndex == 0) return;

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
            cmbFlat.Properties.Items.Clear();
        }

        /// <summary>
        /// Apartment seçimi değiştiğinde
        /// </summary>
        private void CmbApartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbApartment?.SelectedItem == null || cmbApartment.SelectedIndex == 0)
            {
                cmbFlat.Properties.Items.Clear();
                return;
            }

            var selectedApartmentName = cmbApartment.SelectedItem.ToString();
            var apartments = _apartmentService.GetAll();
            var selectedApartment = apartments.FirstOrDefault(a => a.Name == selectedApartmentName);
            if (selectedApartment == null) return;

            var flats = _flatService.GetAllByApartmentId(selectedApartment.Id);
            cmbFlat.Properties.Items.Clear();
            foreach (var flat in flats)
            {
                cmbFlat.Properties.Items.Add($"Daire {flat.DoorNumber} - Kat {flat.Floor}");
            }
        }

        /// <summary>
        /// Verileri yükler
        /// </summary>
        private void LoadData()
        {
            if (_currentDues == null) return;

            var dues = _duesService.GetById(_currentDues.Id);
            if (dues == null) return;

            var flat = dues.Flat;
            if (flat == null) return;

            var apartment = flat.Apartment;
            if (apartment == null) return;

            // Site seçimi
            if (cmbSite != null && apartment.Block?.Site != null)
            {
                var siteName = apartment.Block.Site.Name;
                for (int i = 0; i < cmbSite.Properties.Items.Count; i++)
                {
                    if (cmbSite.Properties.Items[i].ToString() == siteName)
                    {
                        cmbSite.SelectedIndex = i;
                        break;
                    }
                }
            }

            // Apartment seçimi
            var apartmentName = apartment.Name;
            for (int i = 0; i < cmbApartment.Properties.Items.Count; i++)
            {
                if (cmbApartment.Properties.Items[i].ToString() == apartmentName)
                {
                    cmbApartment.SelectedIndex = i;
                    break;
                }
            }

            // Flat seçimi
            var flatText = $"Daire {flat.DoorNumber} - Kat {flat.Floor}";
            for (int i = 0; i < cmbFlat.Properties.Items.Count; i++)
            {
                if (cmbFlat.Properties.Items[i].ToString() == flatText)
                {
                    cmbFlat.SelectedIndex = i;
                    break;
                }
            }

            // Diğer alanlar
            spnAmount.Value = (decimal)dues.Amount;
            cmbMonth.SelectedIndex = dues.Month - 1; // 0-based index
            cmbYear.SelectedItem = dues.Year.ToString();
            chkIsPaid.Checked = dues.IsPaid;
        }

        /// <summary>
        /// Kaydet butonuna tıklandığında
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validation
            if (cmbFlat?.SelectedItem == null)
            {
                Swal.Warning("Lütfen bir daire seçin.");
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
                // Flat seçimi
                var selectedFlatText = cmbFlat.SelectedItem.ToString();
                var apartmentName = cmbApartment.SelectedItem?.ToString();
                var apartments = _apartmentService.GetAll();
                var selectedApartment = apartments.FirstOrDefault(a => a.Name == apartmentName);
                if (selectedApartment == null)
                {
                    Swal.Error("Apartman bulunamadı.");
                    return;
                }

                var flats = _flatService.GetAllByApartmentId(selectedApartment.Id);
                var selectedFlat = flats.FirstOrDefault(f => $"Daire {f.DoorNumber} - Kat {f.Floor}" == selectedFlatText);
                if (selectedFlat == null)
                {
                    Swal.Error("Daire bulunamadı.");
                    return;
                }

                if (_isEditMode)
                {
                    // Update
                    _currentDues.FlatId = selectedFlat.Id;
                    _currentDues.Amount = spnAmount.Value;
                    _currentDues.Month = cmbMonth.SelectedIndex + 1;
                    _currentDues.Year = int.Parse(cmbYear.SelectedItem.ToString());
                    _currentDues.IsPaid = chkIsPaid.Checked;

                    string result = _duesService.Update(_currentDues);
                    if (!string.IsNullOrEmpty(result))
                    {
                        Swal.Error("Güncelleme hatası: " + result);
                        return;
                    }

                    Swal.Success("Aidat başarıyla güncellendi.");
                }
                else
                {
                    // Add
                    var newDues = new Dues
                    {
                        FlatId = selectedFlat.Id,
                        Amount = spnAmount.Value,
                        Month = cmbMonth.SelectedIndex + 1,
                        Year = int.Parse(cmbYear.SelectedItem.ToString()),
                        IsPaid = chkIsPaid.Checked
                    };

                    string result = _duesService.Add(newDues);
                    if (!string.IsNullOrEmpty(result))
                    {
                        Swal.Error("Ekleme hatası: " + result);
                        return;
                    }

                    Swal.Success("Aidat başarıyla oluşturuldu.");
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Swal.Error("İşlem hatası: " + ex.Message);
            }
        }
    }
}

