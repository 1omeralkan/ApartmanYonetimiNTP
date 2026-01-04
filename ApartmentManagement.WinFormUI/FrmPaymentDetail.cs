#nullable disable
// FrmPaymentDetail.cs
// Ödeme Detay Formu - Ödeme detaylarını gösterir
// Standart: Tahoma 8.25pt
using ApartmentManagement.Business.Services;
using ApartmentManagement.Business.Interfaces;
using ApartmentManagement.DataAccess.Entities;
using ApartmentManagement.WinFormUI.Helpers;
using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI
{
    /// <summary>
    /// Ödeme detay formu - Ödeme bilgilerini gösterir
    /// </summary>
    public partial class FrmPaymentDetail : DevExpress.XtraEditors.XtraForm
    {
        private Payment _payment;

        /// <summary>
        /// FrmPaymentDetail constructor
        /// </summary>
        public FrmPaymentDetail(Payment payment)
        {
            _payment = payment;
            InitializeComponent();
            LoadData();
        }

        /// <summary>
        /// Form bileşenlerini başlatır
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Settings
            this.Text = "Ödeme Detayı";
            this.ClientSize = new Size(600, 500);
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

            // ========== HEADER SECTION ==========
            var lblTitle = new LabelControl();
            lblTitle.Text = "💳 Ödeme Detayı";
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblTitle.Location = new Point(leftMargin, currentY);
            this.Controls.Add(lblTitle);

            currentY += 60;

            // ========== DETAIL PANEL ==========
            var pnlDetail = new Helpers.RoundedPanel();
            pnlDetail.Size = new Size(contentWidth, 350);
            pnlDetail.Location = new Point(leftMargin, currentY);
            pnlDetail.BackColor = Color.White;
            pnlDetail.BorderRadius = 12;
            pnlDetail.BorderThickness = 1;
            pnlDetail.BorderColor = Color.FromArgb(226, 232, 240);
            pnlDetail.Padding = new Padding(35);
            this.Controls.Add(pnlDetail);

            int panelY = 30;
            int panelLeft = 35;

            // Daire Bilgileri
            AddDetailRow(pnlDetail, "Site", panelLeft, panelY, ref panelY);
            AddDetailRow(pnlDetail, "Blok", panelLeft, panelY, ref panelY);
            AddDetailRow(pnlDetail, "Apartman", panelLeft, panelY, ref panelY);
            AddDetailRow(pnlDetail, "Daire No", panelLeft, panelY, ref panelY);
            AddDetailRow(pnlDetail, "Kat", panelLeft, panelY, ref panelY);

            panelY += 20;

            // Ödeme Bilgileri
            AddDetailRow(pnlDetail, "Ödeme Tipi", panelLeft, panelY, ref panelY);
            AddDetailRow(pnlDetail, "Tutar", panelLeft, panelY, ref panelY);
            AddDetailRow(pnlDetail, "Tarih", panelLeft, panelY, ref panelY);

            currentY += 380;

            // ========== BUTTON ==========
            var btnClose = new SimpleButton();
            btnClose.Text = "Kapat";
            btnClose.Size = new Size(160, 38);
            btnClose.Location = new Point((this.Width - 160) / 2, currentY);
            btnClose.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btnClose.Appearance.BackColor = Color.FromArgb(59, 130, 246);
            btnClose.Appearance.ForeColor = Color.White;
            btnClose.Appearance.Options.UseBackColor = true;
            btnClose.Appearance.Options.UseForeColor = true;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => { this.Close(); };
            this.Controls.Add(btnClose);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Detay satırı ekler
        /// </summary>
        private void AddDetailRow(Control parent, string label, int x, int y, ref int nextY)
        {
            var lbl = new LabelControl();
            lbl.Text = label + ":";
            lbl.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lbl.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
            lbl.Location = new Point(x, y);
            parent.Controls.Add(lbl);

            var lblValue = new LabelControl();
            lblValue.Name = "lbl" + label.Replace(" ", "");
            lblValue.Appearance.Font = new Font("Tahoma", 9F);
            lblValue.Appearance.ForeColor = Color.FromArgb(30, 30, 46);
            lblValue.Location = new Point(x + 150, y);
            lblValue.Size = new Size(400, 20);
            parent.Controls.Add(lblValue);

            nextY += 35;
        }

        /// <summary>
        /// Verileri yükler
        /// </summary>
        private void LoadData()
        {
            if (_payment == null) return;

            // Payment'i yeniden yükle (ilişkiler için)
            var paymentService = new SPayment();
            var payment = paymentService.GetById(_payment.Id);
            if (payment == null) return;

            // Değerleri set et
            var lblSite = this.Controls.Find("lblSite", true).FirstOrDefault() as LabelControl;
            if (lblSite != null) lblSite.Text = payment.Flat?.Apartment?.Block?.Site?.Name ?? "-";

            var lblBlok = this.Controls.Find("lblBlok", true).FirstOrDefault() as LabelControl;
            if (lblBlok != null) lblBlok.Text = payment.Flat?.Apartment?.Block?.Name ?? "-";

            var lblApartman = this.Controls.Find("lblApartman", true).FirstOrDefault() as LabelControl;
            if (lblApartman != null) lblApartman.Text = payment.Flat?.Apartment?.Name ?? "-";

            var lblDaireNo = this.Controls.Find("lblDaireNo", true).FirstOrDefault() as LabelControl;
            if (lblDaireNo != null) lblDaireNo.Text = payment.Flat?.DoorNumber.ToString() ?? "-";

            var lblKat = this.Controls.Find("lblKat", true).FirstOrDefault() as LabelControl;
            if (lblKat != null) lblKat.Text = payment.Flat?.Floor.ToString() ?? "-";

            var lblÖdemeTipi = this.Controls.Find("lblÖdemeTipi", true).FirstOrDefault() as LabelControl;
            if (lblÖdemeTipi != null) lblÖdemeTipi.Text = payment.Type ?? "Aidat";

            var lblTutar = this.Controls.Find("lblTutar", true).FirstOrDefault() as LabelControl;
            if (lblTutar != null) lblTutar.Text = payment.Amount.ToString("N2") + " TL";

            var lblTarih = this.Controls.Find("lblTarih", true).FirstOrDefault() as LabelControl;
            if (lblTarih != null) lblTarih.Text = payment.Date.ToString("dd.MM.yyyy HH:mm");
        }
    }
}

