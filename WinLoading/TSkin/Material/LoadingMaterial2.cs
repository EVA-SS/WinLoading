using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace TSkin
{
    public class LoadingMaterial2 : Control
    {
        #region 属性

        [Category("进度"), Description("进度颜色"), DefaultValue(typeof(Color), "0, 120, 220")]
        public Color Color { get; set; } = Color.FromArgb(0, 120, 220);

        [Category("进度"), Description("进度填充颜色"), DefaultValue(typeof(Color), "Transparent")]
        public Color FillColor { get; set; } = Color.Transparent;

        [Category("进度"), Description("是否圆角"), DefaultValue(true)]
        public bool Round { get; set; } = true;

        float _progWidth = 6f;
        [Category("进度"), Description("宽度"), DefaultValue(6F)]
        public float ProgWidth
        {
            get { return _progWidth; }
            set
            {
                if (_progWidth != value)
                {
                    _progWidth = value;
                    OnSizeChanged(null);
                }
            }
        }

        [Category("进度"), Description("速度"), DefaultValue(2)]
        public int Speed { get; set; } = 2;

        double _Value = 0;
        [Category("进度"), Description("当前值"), DefaultValue(0D)]
        public double Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    if (value > _MaxValue)
                    {
                        _Value = _MaxValue;
                    }
                    else
                    {
                        _Value = value;
                    }
                    if (_Value > 0 && _MaxValue > 0)
                    {
                        LineWidth = (float)(_Value / _MaxValue * 100.0);
                    }
                }
            }
        }


        double _MaxValue = 100;
        [Category("进度"), Description("最大值"), DefaultValue(100D)]
        public double MaxValue
        {
            get { return _MaxValue; }
            set
            {
                if (_MaxValue != value)
                {
                    _MaxValue = value;
                    if (_Value > 0 && _MaxValue > 0)
                    {
                        LineWidth = (float)(_Value / _MaxValue * 100.0);
                    }
                }
            }
        }

        [Description("图片"), Category("外观"), DefaultValue(null)]
        public Image Img { get; set; } = null;

        #endregion

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

                        bool ProgState = false, ProgState2 = false;
                        thread = new ThreadOne(() =>
                        {
                            if (_Value > 0 && _MaxValue > 0)
                            {
                                LineAngle += Speed;
                                if (LineAngle >= 360)
                                { LineAngle = 0; }
                            }
                            else
                            {
                                LineAngle += Speed;
                                if (LineAngle >= 360)
                                { LineAngle = 0; }
                                if (ProgState)
                                {
                                    if (LineWidth >= 98) ProgState = false;
                                    else LineWidth++;
                                }
                                else
                                {
                                    if (LineWidth <= 2) ProgState = true;
                                    else
                                    {
                                        LineWidth--;
                                        LineAngle += Speed;
                                        if (Speed < 2) LineAngle += 3;
                                        else LineAngle += Speed;
                                    }
                                }
                            }
                            if (ProgState2)
                            {
                                if (_enabledOpacity1 >= 1) ProgState2 = false;
                                else _enabledOpacity1 += 0.01f;
                            }
                            else
                            {
                                if (_enabledOpacity1 <= 0.4f) ProgState2 = true;
                                else _enabledOpacity1 -= 0.01f;
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

        float LineWidth = 2;
        int LineAngle = 0;

        #region 坐标计算

        protected override void OnResize(EventArgs e)
        {
            Height = Width;
            base.OnResize(e);
        }
        RectangleF rect = new RectangleF();
        protected override void OnSizeChanged(EventArgs e)
        {
            var rectf = this.ClientRectangle;
            if (rectf.Width > 0 && rectf.Height > 0)
            {
                rect = new RectangleF(_progWidth / 2, _progWidth / 2, rectf.Width - _progWidth, rectf.Height - _progWidth);
            }
            base.OnSizeChanged(e);
        }

        #endregion

        float _enabledOpacity1 = 1f;
        int wi = 80;
        protected override void OnPaint(PaintEventArgs e)
        {
            if (FillColor != Color.Transparent || State || (_MaxValue > 0 && _Value > 0))
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                if (FillColor != Color.Transparent)
                {
                    using (var p = new Pen(FillColor, ProgWidth))
                    {
                        g.DrawArc(p, rect, 0, 360);
                    }
                }
                if (State || (_MaxValue > 0 && _Value > 0))
                {
                    var lwi = (float)(LineWidth * 3.6);
                    float cw = rect.Width / 4;
                    var _j2 = new RectangleF(rect.X + cw / 2, rect.Y + cw / 2, rect.Width - cw, rect.Height - cw);
                    using (var p2 = new Pen(Color.FromArgb(100, Color), ProgWidth))
                    {
                        if (Round)
                        {
                            p2.StartCap = LineCap.Round;
                            p2.EndCap = LineCap.Round;
                            p2.LineJoin = LineJoin.Round;
                        }
                        g.DrawArc(p2, _j2, 90 + LineAngle, lwi);
                    }
                    using (var p = new Pen(Color, ProgWidth))
                    {
                        if (Round)
                        {
                            p.StartCap = LineCap.Round;
                            p.EndCap = LineCap.Round;
                            p.LineJoin = LineJoin.Round;
                        }
                        g.DrawArc(p, rect, LineAngle, lwi);
                    }
                }

                if (Img != null)
                {
                    using (var attributes = new ImageAttributes())
                    {
                        ColorMatrix matrix = new ColorMatrix();
                        matrix.Matrix33 = _enabledOpacity1;
                        attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                        Rectangle rect = ClientRectangle;
                        g.DrawImage(Img, new Rectangle((rect.Width - wi) / 2, (rect.Height - wi) / 2, wi, wi), 0, 0, Img.Width, Img.Height, GraphicsUnit.Pixel, attributes);
                    }
                }
            }
        }

        public LoadingMaterial2()
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
            State = false;
            base.Dispose(disposing);
        }
    }
}
