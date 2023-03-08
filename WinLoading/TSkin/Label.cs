using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TSkin
{
    public partial class Label : Control
    {
        public Label()
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

        [Category("外观"), Description("是否纵向"), DefaultValue(false)]
        public bool Vertical { get; set; } = false;

        [Category("外观"), Description("透明度0-255"), DefaultValue(30)]
        public int Alpha { get; set; } = 30;

        bool _MultiLine = false;
        [Category("外观"), Description("是否多行"), DefaultValue(false)]
        public bool MultiLine
        {
            get { return _MultiLine; }
            set
            {
                if (_MultiLine != value)
                {
                    _MultiLine = value;
                    stringFormat.FormatFlags = value ? 0 : StringFormatFlags.NoWrap;
                    Invalidate();
                }
            }
        }

        ContentAlignment _TextAlign = ContentAlignment.MiddleCenter;
        [Category("外观"), Description("文本的位置"), DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment TextAlign
        {
            get => _TextAlign;
            set
            {
                if (_TextAlign != value)
                {
                    _TextAlign = value;
                    switch (_TextAlign)
                    {
                        case ContentAlignment.TopLeft:
                            stringFormat.Alignment = StringAlignment.Near;
                            stringFormat.LineAlignment = StringAlignment.Near;
                            //内容在垂直方向上顶部对齐，在水平方向上左边对齐
                            break;
                        case ContentAlignment.TopCenter:
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Near;
                            //内容在垂直方向上顶部对齐，在水平方向上居中对齐

                            break;
                        case ContentAlignment.TopRight:
                            //内容在垂直方向上顶部对齐，在水平方向上右边对齐
                            stringFormat.Alignment = StringAlignment.Far;
                            stringFormat.LineAlignment = StringAlignment.Near;
                            break;
                        case ContentAlignment.MiddleLeft:
                            //内容在垂直方向上中间对齐，在水平方向上左边对齐
                            stringFormat.Alignment = StringAlignment.Near;
                            stringFormat.LineAlignment = StringAlignment.Center;

                            break;
                        case ContentAlignment.MiddleCenter:
                            //内容在垂直方向上中间对齐，在水平方向上居中对齐
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Center;
                            break;
                        case ContentAlignment.MiddleRight:
                            //内容在垂直方向上中间对齐，在水平方向上右边对齐
                            stringFormat.Alignment = StringAlignment.Far;
                            stringFormat.LineAlignment = StringAlignment.Center;

                            break;
                        case ContentAlignment.BottomLeft:
                            //内容在垂直方向上底边对齐，在水平方向上左边对齐
                            stringFormat.Alignment = StringAlignment.Near;
                            stringFormat.LineAlignment = StringAlignment.Far;
                            break;
                        case ContentAlignment.BottomCenter:
                            //内容在垂直方向上底边对齐，在水平方向上居中对齐
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Far;

                            break;
                        case ContentAlignment.BottomRight:
                            //内容在垂直方向上底边对齐，在水平方向上右边对齐
                            stringFormat.Alignment = StringAlignment.Far;
                            stringFormat.LineAlignment = StringAlignment.Far;
                            break;
                    }
                }
            }
        }


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
                        if (Vertical)
                        {
                            _value = -Height;
                        }
                        else
                        {
                            _value = -Width;
                        }
                        thread = new ThreadOne(() =>
                        {
                            _value += 2;
                            if (Vertical)
                            {
                                if (_value > Height * 2) _value = -Height;
                            }
                            else
                            {
                                if (_value > Width * 2) _value = -Width;
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

        StringFormat stringFormat = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center, FormatFlags = StringFormatFlags.NoWrap };

        int _value = 0;
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            var recto = ClientRectangle;
            var rect_ = new RectangleF(Padding.Left, Padding.Top, recto.Width - Padding.Right - Padding.Left, recto.Height - Padding.Bottom - Padding.Top);
            if (_state)
            {
                if (Vertical)
                {
                    var rect = new Rectangle(0, _value, recto.Width, recto.Height * 3);
                    using (var brush = new LinearGradientBrush(rect, ForeColor, Color.FromArgb(Alpha, ForeColor), LinearGradientMode.Vertical))
                    {
                        brush.SetSigmaBellShape(0.5f);
                        g.DrawString(Text, Font, brush, rect_, stringFormat);
                    }
                }
                else
                {
                    var rect = new Rectangle(_value, 0, recto.Width * 3, recto.Height);
                    using (var brush = new LinearGradientBrush(rect, ForeColor, Color.FromArgb(Alpha, ForeColor), LinearGradientMode.Horizontal))
                    {
                        brush.SetSigmaBellShape(0.5f);
                        g.DrawString(Text, Font, brush, rect_, stringFormat);
                    }
                }
            }
            else
            {
                using (var brush = new SolidBrush(ForeColor))
                {
                    g.DrawString(Text, Font, brush, rect_, stringFormat);
                }
            }
            base.OnPaint(e);
        }
    }
}
