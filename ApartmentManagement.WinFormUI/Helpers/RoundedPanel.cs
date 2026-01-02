#nullable disable
// RoundedPanel.cs
// Yuvarlak köşeli panel - Modern UI için
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ApartmentManagement.WinFormUI.Helpers
{
    /// <summary>
    /// Yuvarlak köşeli panel kontrolü
    /// </summary>
    public class RoundedPanel : Panel
    {
        private int _borderRadius = 12;
        private Color _borderColor = Color.Transparent;
        private int _borderThickness = 0;

        /// <summary>
        /// Köşe yuvarlaklık yarıçapı (pixel)
        /// </summary>
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        /// <summary>
        /// Kenarlık rengi
        /// </summary>
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        /// <summary>
        /// Kenarlık kalınlığı
        /// </summary>
        public int BorderThickness
        {
            get => _borderThickness;
            set { _borderThickness = value; Invalidate(); }
        }

        public RoundedPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | 
                     ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            using (GraphicsPath path = GetRoundedPath(ClientRectangle, _borderRadius))
            {
                // Arka plan
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    e.Graphics.FillPath(brush, path);
                }
                
                // Kenarlık
                if (_borderThickness > 0 && _borderColor != Color.Transparent)
                {
                    using (Pen pen = new Pen(_borderColor, _borderThickness))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }
            
            // Region ayarla (köşelerin dışına tıklanamaz)
            this.Region = new Region(GetRoundedPath(ClientRectangle, _borderRadius));
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            
            // Sol üst köşe
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            // Sağ üst köşe
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            // Sağ alt köşe
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            // Sol alt köşe
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            
            path.CloseFigure();
            return path;
        }
    }
}
