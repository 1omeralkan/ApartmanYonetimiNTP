#nullable disable
// FrmExpenseManagement.cs
// Gider Yönetim Formu - Gider ekleme ve düzenleme
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
    /// Gider yönetim formu - Gider ekleme ve düzenleme
    /// </summary>
    public partial class FrmExpenseManagement : DevExpress.XtraEditors.XtraForm
    {
        private IExpense _expenseService;
        private IApartment _apartmentService;
        private ISite _siteService;
        private IFlat _flatService;
        private Expense _currentExpense;
        private User _currentUser;
        private bool _isEditMode;

        // Controls
        private ComboBoxEdit cmbSite;
        private ComboBoxEdit cmbApartment;
        private ComboBoxEdit cmbCategory;
        private MemoEdit txtDescription;
        private SpinEdit spnAmount;
        private DateEdit dtDate;
        private SimpleButton btnSave;
        private SimpleButton btnCancel;

        /// <summary>
        /// FrmExpenseManagement constructor
        /// </summary>
        public FrmExpenseManagement(Expense expense, User user)
        {
            _currentExpense = expense;
            _currentUser = user;
            _isEditMode = expense != null;
            _expenseService = new SExpense();
            _apartmentService = new SApartment();
            _siteService = new SSite();
            _flatService = new SFlat();
            InitializeComponent();
            LoadFilters();
            if (_isEditMode)
            {
                LoadData();
            }
            else
            {
                // Yeni gider için varsayılan değerler
                dtDate.EditValue = DateTime.Now;
            }
        }

        /// <summary>
        /// Form bileşenlerini başlatır
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = _isEditMode ? "Gider Düzenle" : "Yeni Gider Ekle";
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
            int fieldHeight = 32;

            // ========== HEADER SECTION ==========
            var lblTitle = new LabelControl();
            lblTitle.Text = _isEditMode ? "Gider Düzenle" : "Yeni Gider Ekle";
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblTitle);

            var lblSubtitle = new LabelControl();
            lblSubtitle.Text = _isEditMode ? "Gider bilgilerini düzenleyin" : "Yeni bir gider kaydı oluşturun";
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
            pnlForm.Size = new Size(contentWidth, 500);
            pnlForm.Location = new Point(leftMargin, currentY);
            pnlForm.BackColor = Color.White;
            pnlForm.BorderRadius = 12;
            pnlForm.BorderThickness = 1;
            pnlForm.BorderColor = Color.FromArgb(226, 232, 240);
            pnlForm.Padding = new Padding(35);
            this.Controls.Add(pnlForm);

            int panelY = 30;
            int panelLeft = 35;

            // ========== GİDER BİLGİLERİ SECTION ==========
            var lblSectionExpense = new LabelControl();
            lblSectionExpense.Text = "Gider Bilgileri";
            lblSectionExpense.Appearance.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            lblSectionExpense.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblSectionExpense.Location = new Point(panelLeft, panelY);
            pnlForm.Controls.Add(lblSectionExpense);
            panelY += 40;

            // Site (SiteManager için)
            if (_currentUser?.Role == "SiteManager" || _currentUser?.Role == "SuperAdmin" || _currentUser?.Role == "Admin")
            {
                AddFieldLabel(pnlForm, "Site", panelLeft, panelY, false);
                panelY += 20;
                cmbSite = new ComboBoxEdit();
                cmbSite.Location = new Point(panelLeft, panelY);
                cmbSite.Size = new Size(contentWidth - 70, fieldHeight);
                cmbSite.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                cmbSite.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
                cmbSite.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
                cmbSite.Properties.NullText = "Site seçiniz (Opsiyonel)";
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

            // Category
            AddFieldLabel(pnlForm, "Kategori *", panelLeft, panelY, true);
            panelY += 20;
            cmbCategory = new ComboBoxEdit();
            cmbCategory.Location = new Point(panelLeft, panelY);
            cmbCategory.Size = new Size(contentWidth - 70, fieldHeight);
            cmbCategory.Properties.Items.AddRange(new[] { "Bakım", "Temizlik", "Elektrik", "Su", "Güvenlik", "Yakıt", "Diğer" });
            cmbCategory.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbCategory.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            cmbCategory.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            cmbCategory.Properties.NullText = "Kategori seçiniz";
            pnlForm.Controls.Add(cmbCategory);
            panelY += 45;

            // Description
            AddFieldLabel(pnlForm, "Açıklama *", panelLeft, panelY, true);
            panelY += 20;
            txtDescription = new MemoEdit();
            txtDescription.Location = new Point(panelLeft, panelY);
            txtDescription.Size = new Size(contentWidth - 70, 90);
            txtDescription.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            txtDescription.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            txtDescription.Properties.NullText = "Gider açıklaması";
            pnlForm.Controls.Add(txtDescription);
            panelY += 110;

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

            // Date
            AddFieldLabel(pnlForm, "Tarih *", panelLeft, panelY, true);
            panelY += 20;
            dtDate = new DateEdit();
            dtDate.Location = new Point(panelLeft, panelY);
            dtDate.Size = new Size(contentWidth - 70, fieldHeight);
            dtDate.Properties.Appearance.Font = new Font("Tahoma", 8.5F);
            dtDate.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            dtDate.Properties.DisplayFormat.FormatString = "dd.MM.yyyy";
            dtDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dtDate.Properties.EditFormat.FormatString = "dd.MM.yyyy";
            dtDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dtDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            dtDate.Properties.Mask.EditMask = "dd.MM.yyyy";
            dtDate.Properties.NullText = "gg.aa.yyyy";
            pnlForm.Controls.Add(dtDate);
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
            // Site Filter
            if (cmbSite != null)
            {
                var sites = _siteService.GetAll();
                cmbSite.Properties.Items.Clear();
                cmbSite.Properties.Items.Add("Seçiniz");
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
            cmbApartment.Properties.Items.Add("Seçiniz");
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
            cmbApartment.Properties.Items.Add("Seçiniz");
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
        /// Verileri yükler
        /// </summary>
        private void LoadData()
        {
            if (_currentExpense == null) return;

            var expense = _expenseService.GetById(_currentExpense.Id);
            if (expense == null) return;

            // Site seçimi
            if (cmbSite != null && expense.SiteId.HasValue)
            {
                var site = _siteService.GetAll().FirstOrDefault(s => s.Id == expense.SiteId.Value);
                if (site != null)
                {
                    for (int i = 0; i < cmbSite.Properties.Items.Count; i++)
                    {
                        if (cmbSite.Properties.Items[i].ToString() == site.Name)
                        {
                            cmbSite.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }

            // Apartment seçimi
            if (expense.ApartmentId.HasValue)
            {
                var apartment = _apartmentService.GetAll().FirstOrDefault(a => a.Id == expense.ApartmentId.Value);
                if (apartment != null)
                {
                    for (int i = 0; i < cmbApartment.Properties.Items.Count; i++)
                    {
                        if (cmbApartment.Properties.Items[i].ToString() == apartment.Name)
                        {
                            cmbApartment.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }

            // Diğer alanlar
            cmbCategory.SelectedItem = expense.Category ?? "Diğer";
            txtDescription.Text = expense.Description ?? "";
            spnAmount.Value = (decimal)expense.Amount;
            dtDate.EditValue = expense.Date;
        }

        /// <summary>
        /// Kaydet butonuna tıklandığında
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validation
            if (cmbApartment?.SelectedItem == null || cmbApartment.SelectedIndex == 0)
            {
                Swal.Warning("Lütfen bir apartman seçin.");
                return;
            }

            if (cmbCategory?.SelectedItem == null)
            {
                Swal.Warning("Lütfen bir kategori seçin.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                Swal.Warning("Lütfen açıklama girin.");
                return;
            }

            if (spnAmount.Value <= 0)
            {
                Swal.Warning("Lütfen geçerli bir tutar girin.");
                return;
            }

            if (dtDate.EditValue == null)
            {
                Swal.Warning("Lütfen bir tarih seçin.");
                return;
            }

            try
            {
                // Apartment seçimi
                var apartmentName = cmbApartment.SelectedItem.ToString();
                var apartments = _apartmentService.GetAll();
                var selectedApartment = apartments.FirstOrDefault(a => a.Name == apartmentName);
                if (selectedApartment == null)
                {
                    Swal.Error("Apartman bulunamadı.");
                    return;
                }

                // Site seçimi (opsiyonel)
                int? siteId = null;
                if (cmbSite != null && cmbSite.SelectedItem != null && cmbSite.SelectedIndex > 0)
                {
                    var siteName = cmbSite.SelectedItem.ToString();
                    var sites = _siteService.GetAll();
                    var selectedSite = sites.FirstOrDefault(s => s.Name == siteName);
                    if (selectedSite != null)
                    {
                        siteId = selectedSite.Id;
                    }
                }

                if (_isEditMode)
                {
                    // Update
                    _currentExpense.SiteId = siteId;
                    _currentExpense.ApartmentId = selectedApartment.Id;
                    _currentExpense.Category = cmbCategory.SelectedItem.ToString();
                    _currentExpense.Description = txtDescription.Text.Trim();
                    _currentExpense.Amount = spnAmount.Value;
                    _currentExpense.Date = (DateTime)dtDate.EditValue;

                    string result = _expenseService.Update(_currentExpense);
                    if (!string.IsNullOrEmpty(result))
                    {
                        Swal.Error("Güncelleme hatası: " + result);
                        return;
                    }

                    Swal.Success("Gider başarıyla güncellendi.");
                }
                else
                {
                    // Add
                    var newExpense = new Expense
                    {
                        SiteId = siteId,
                        ApartmentId = selectedApartment.Id,
                        Category = cmbCategory.SelectedItem.ToString(),
                        Description = txtDescription.Text.Trim(),
                        Amount = spnAmount.Value,
                        Date = (DateTime)dtDate.EditValue
                    };

                    string result = _expenseService.Add(newExpense);
                    if (!string.IsNullOrEmpty(result))
                    {
                        Swal.Error("Ekleme hatası: " + result);
                        return;
                    }

                    Swal.Success("Gider başarıyla oluşturuldu.");
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

