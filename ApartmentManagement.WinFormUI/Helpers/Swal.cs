using DevExpress.XtraEditors;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ApartmentManagement.WinFormUI.Helpers
{
    public enum SwalType
    {
        Success,
        Error,
        Warning,
        Info,
        Question
    }

    public static class Swal
    {
        /// <summary>
        /// Show a success message
        /// </summary>
        public static void Success(string message, string title = "Başarılı!")
        {
            ShowAlert(SwalType.Success, title, message);
        }

        /// <summary>
        /// Show an error message
        /// </summary>
        public static void Error(string message, string title = "Hata!")
        {
            ShowAlert(SwalType.Error, title, message);
        }

        /// <summary>
        /// Show a warning message
        /// </summary>
        public static void Warning(string message, string title = "Uyarı!")
        {
            ShowAlert(SwalType.Warning, title, message);
        }

        /// <summary>
        /// Show an info message
        /// </summary>
        public static void Info(string message, string title = "Bilgi")
        {
            ShowAlert(SwalType.Info, title, message);
        }

        /// <summary>
        /// Show a confirmation dialog
        /// </summary>
        public static bool Confirm(string message, string title = "Emin misiniz?")
        {
            return ShowConfirm(title, message);
        }

        private static void ShowAlert(SwalType type, string title, string message)
        {
            var frm = new FrmSwalAlert(type, title, message);
            frm.ShowDialog();
        }

        private static bool ShowConfirm(string title, string message)
        {
            var frm = new FrmSwalConfirm(title, message);
            return frm.ShowDialog() == DialogResult.Yes;
        }
    }

    public class FrmSwalAlert : XtraForm
    {
        private SwalType _type;
        private string _title;
        private string _message;

        public FrmSwalAlert(SwalType type, string title, string message)
        {
            _type = type;
            _title = title;
            _message = message;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(400, 280);
            this.BackColor = Color.White;
            this.ShowInTaskbar = false;
            this.TopMost = true;

            // Add shadow effect via region
            this.Region = CreateRoundedRegion(this.Width, this.Height, 15);

            // Main panel with rounded corners
            var pnlMain = new Panel();
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.BackColor = Color.White;
            pnlMain.Padding = new Padding(30);
            this.Controls.Add(pnlMain);

            // Icon Circle
            var pnlIcon = new Panel();
            pnlIcon.Size = new Size(80, 80);
            pnlIcon.Location = new Point((this.Width - 80) / 2, 25);
            pnlIcon.Paint += (s, e) => DrawIconCircle(e.Graphics);
            pnlMain.Controls.Add(pnlIcon);

            // Title
            var lblTitle = new LabelControl();
            lblTitle.Text = _title;
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(50, 50, 50);
            lblTitle.AutoSizeMode = LabelAutoSizeMode.None;
            lblTitle.Size = new Size(340, 35);
            lblTitle.Location = new Point(30, 115);
            lblTitle.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            pnlMain.Controls.Add(lblTitle);

            // Message
            var lblMessage = new LabelControl();
            lblMessage.Text = _message;
            lblMessage.Appearance.Font = new Font("Tahoma", 11F);
            lblMessage.Appearance.ForeColor = Color.FromArgb(120, 120, 120);
            lblMessage.AutoSizeMode = LabelAutoSizeMode.None;
            lblMessage.Size = new Size(340, 50);
            lblMessage.Location = new Point(30, 155);
            lblMessage.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lblMessage.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            pnlMain.Controls.Add(lblMessage);

            // OK Button
            var btnOk = new SimpleButton();
            btnOk.Text = "Tamam";
            btnOk.Size = new Size(120, 45);
            btnOk.Location = new Point((this.Width - 120) / 2, 215);
            btnOk.Appearance.Font = new Font("Tahoma", 11F, FontStyle.Bold);
            btnOk.Appearance.ForeColor = Color.White;
            btnOk.Appearance.BackColor = GetButtonColor();
            btnOk.Appearance.Options.UseFont = true;
            btnOk.Appearance.Options.UseForeColor = true;
            btnOk.Appearance.Options.UseBackColor = true;
            btnOk.Cursor = Cursors.Hand;
            btnOk.Click += (s, e) => this.Close();
            pnlMain.Controls.Add(btnOk);

            // Close on Escape
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter) this.Close(); };
        }

        private void DrawIconCircle(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            var circleColor = GetCircleColor();
            var iconColor = circleColor;
            var bgColor = Color.FromArgb(40, circleColor);

            // Background circle
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillEllipse(brush, 0, 0, 78, 78);
            }

            // Icon
            using (var pen = new Pen(iconColor, 4))
            using (var font = new Font("Tahoma", 32F, FontStyle.Bold))
            {
                string icon = GetIconChar();
                var size = g.MeasureString(icon, font);
                g.DrawString(icon, font, new SolidBrush(iconColor), 
                    (78 - size.Width) / 2, (78 - size.Height) / 2);
            }
        }

        private Color GetCircleColor()
        {
            return _type switch
            {
                SwalType.Success => Color.FromArgb(40, 167, 69),
                SwalType.Error => Color.FromArgb(220, 53, 69),
                SwalType.Warning => Color.FromArgb(255, 193, 7),
                SwalType.Info => Color.FromArgb(23, 162, 184),
                SwalType.Question => Color.FromArgb(108, 117, 125),
                _ => Color.FromArgb(66, 133, 244)
            };
        }

        private Color GetButtonColor()
        {
            return _type switch
            {
                SwalType.Success => Color.FromArgb(40, 167, 69),
                SwalType.Error => Color.FromArgb(220, 53, 69),
                SwalType.Warning => Color.FromArgb(255, 193, 7),
                SwalType.Info => Color.FromArgb(23, 162, 184),
                SwalType.Question => Color.FromArgb(66, 133, 244),
                _ => Color.FromArgb(66, 133, 244)
            };
        }

        private string GetIconChar()
        {
            return _type switch
            {
                SwalType.Success => "✓",
                SwalType.Error => "✕",
                SwalType.Warning => "!",
                SwalType.Info => "i",
                SwalType.Question => "?",
                _ => "i"
            };
        }

        private Region CreateRoundedRegion(int width, int height, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
            path.AddArc(width - radius * 2, 0, radius * 2, radius * 2, 270, 90);
            path.AddArc(width - radius * 2, height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(0, height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseAllFigures();
            return new Region(path);
        }
    }

    public class FrmSwalConfirm : XtraForm
    {
        private string _title;
        private string _message;

        public FrmSwalConfirm(string title, string message)
        {
            _title = title;
            _message = message;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(420, 300);
            this.BackColor = Color.White;
            this.ShowInTaskbar = false;
            this.TopMost = true;

            // Rounded region
            this.Region = CreateRoundedRegion(this.Width, this.Height, 15);

            // Main panel
            var pnlMain = new Panel();
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.BackColor = Color.White;
            this.Controls.Add(pnlMain);

            // Question Icon
            var pnlIcon = new Panel();
            pnlIcon.Size = new Size(80, 80);
            pnlIcon.Location = new Point((this.Width - 80) / 2, 25);
            pnlIcon.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var color = Color.FromArgb(220, 53, 69);
                using (var brush = new SolidBrush(Color.FromArgb(40, color)))
                    e.Graphics.FillEllipse(brush, 0, 0, 78, 78);
                using (var font = new Font("Tahoma", 32F, FontStyle.Bold))
                {
                    var size = e.Graphics.MeasureString("?", font);
                    e.Graphics.DrawString("?", font, new SolidBrush(color), 
                        (78 - size.Width) / 2, (78 - size.Height) / 2);
                }
            };
            pnlMain.Controls.Add(pnlIcon);

            // Title
            var lblTitle = new LabelControl();
            lblTitle.Text = _title;
            lblTitle.Appearance.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(50, 50, 50);
            lblTitle.AutoSizeMode = LabelAutoSizeMode.None;
            lblTitle.Size = new Size(380, 35);
            lblTitle.Location = new Point(20, 115);
            lblTitle.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            pnlMain.Controls.Add(lblTitle);

            // Message
            var lblMessage = new LabelControl();
            lblMessage.Text = _message;
            lblMessage.Appearance.Font = new Font("Tahoma", 11F);
            lblMessage.Appearance.ForeColor = Color.FromArgb(120, 120, 120);
            lblMessage.AutoSizeMode = LabelAutoSizeMode.None;
            lblMessage.Size = new Size(380, 50);
            lblMessage.Location = new Point(20, 155);
            lblMessage.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lblMessage.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            pnlMain.Controls.Add(lblMessage);

            // Button Panel
            var pnlButtons = new Panel();
            pnlButtons.Size = new Size(280, 50);
            pnlButtons.Location = new Point((this.Width - 280) / 2, 220);
            pnlMain.Controls.Add(pnlButtons);

            // Cancel Button
            var btnCancel = new SimpleButton();
            btnCancel.Text = "İptal";
            btnCancel.Size = new Size(130, 45);
            btnCancel.Location = new Point(0, 0);
            btnCancel.Appearance.Font = new Font("Tahoma", 11F, FontStyle.Bold);
            btnCancel.Appearance.ForeColor = Color.FromArgb(80, 80, 80);
            btnCancel.Appearance.BackColor = Color.FromArgb(230, 230, 230);
            btnCancel.Appearance.Options.UseFont = true;
            btnCancel.Appearance.Options.UseForeColor = true;
            btnCancel.Appearance.Options.UseBackColor = true;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.No; this.Close(); };
            pnlButtons.Controls.Add(btnCancel);

            // Confirm Button
            var btnConfirm = new SimpleButton();
            btnConfirm.Text = "Evet, Sil";
            btnConfirm.Size = new Size(130, 45);
            btnConfirm.Location = new Point(150, 0);
            btnConfirm.Appearance.Font = new Font("Tahoma", 11F, FontStyle.Bold);
            btnConfirm.Appearance.ForeColor = Color.White;
            btnConfirm.Appearance.BackColor = Color.FromArgb(220, 53, 69);
            btnConfirm.Appearance.Options.UseFont = true;
            btnConfirm.Appearance.Options.UseForeColor = true;
            btnConfirm.Appearance.Options.UseBackColor = true;
            btnConfirm.Cursor = Cursors.Hand;
            btnConfirm.Click += (s, e) => { this.DialogResult = DialogResult.Yes; this.Close(); };
            pnlButtons.Controls.Add(btnConfirm);

            // Close on Escape
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { 
                if (e.KeyCode == Keys.Escape) { this.DialogResult = DialogResult.No; this.Close(); }
            };
        }

        private Region CreateRoundedRegion(int width, int height, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
            path.AddArc(width - radius * 2, 0, radius * 2, radius * 2, 270, 90);
            path.AddArc(width - radius * 2, height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(0, height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseAllFigures();
            return new Region(path);
        }
    }
}
