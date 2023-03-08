using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TSkin
{
    public partial class LoadingLine : Control
    {
        public LoadingLine()
        {
            SetStyle(
                 ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor |
                 ControlStyles.DoubleBuffer, true);
            UpdateStyles();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        [Category("外观"), Description("颜色"), DefaultValue(typeof(Color), "0, 120, 220")]
        public Color Color { get; set; } = Color.FromArgb(0, 120, 220);

        [Category("外观"), Description("颜色2"), DefaultValue(typeof(Color), "Transparent")]
        public Color Color2 { get; set; } = Color.Transparent;

        [Category("外观"), Description("是否纵向"), DefaultValue(false)]
        public bool Vertical { get; set; } = false;

        #region 动画开关

        ThreadOne thread = null;
        bool _state = false;
        [Category("进度"), Description("动画状态"), DefaultValue(false)]
        public bool State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    if (_state)
                    {
                        if (thread != null)
                            thread.Dispose();
                        thread = new ThreadOne(() =>
                        {
                            _value += 2F;
                            if (Vertical)
                            {
                                if (_value > Height) _value = 0F;
                            }
                            else
                            {
                                if (_value > Width) _value = 0F;
                            }
                            Invalidate();
                        }, () =>
                        {
                            _state = false;
                            Invalidate();
                        }, () =>
                        {
                            return _state;
                        }, 10);
                    }
                    else
                    {
                        if (thread != null)
                            thread.Dispose();
                    }
                }
            }
        }

        #endregion

        float _value = 0;
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            var rect = new RectangleF(0, 0, Width, Height);
            if (Vertical)
            {
                rect.Y = _value;
                using (var brush = new LinearGradientBrush(rect, Color2, Color, LinearGradientMode.Vertical))
                {
                    brush.SetSigmaBellShape(0.5f);
                    g.FillRectangle(brush, ClientRectangle);
                }
            }
            else
            {
                rect.X = _value;
                using (var brush = new LinearGradientBrush(rect, Color2, Color, LinearGradientMode.Horizontal))
                {
                    brush.SetSigmaBellShape(0.5f);
                    g.FillRectangle(brush, ClientRectangle);
                }
            }
            base.OnPaint(e);
        }
    }
}
