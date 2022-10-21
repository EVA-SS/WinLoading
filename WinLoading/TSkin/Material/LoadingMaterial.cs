using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TSkin
{
    public class LoadingMaterial : Control
    {
        #region 属性

        [Category("进度"), Description("进度颜色"), DefaultValue(typeof(Color), "0, 120, 220")]
        public Color Color { get; set; } = Color.FromArgb(0, 120, 220);

        [Category("进度"), Description("进度填充颜色"), DefaultValue(typeof(Color), "Transparent")]
        public Color FillColor { get; set; } = Color.Transparent;

        [Category("进度"), Description("是否圆角"), DefaultValue(true)]
        public bool Round { get; set; } = true;

        float _progWidth = 4f;
        [Category("进度"), Description("宽度"), DefaultValue(4F)]
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

        [Category("进度"), Description("速度"), DefaultValue(1)]
        public int Speed { get; set; } = 1;

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
                        LineWidth = (float)Math.Round(_Value / _MaxValue * 100.0, 4);
                        Invalidate();
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
                        LineWidth = (float)Math.Round(_Value / _MaxValue * 100.0, 4);
                    }
                }
            }
        }

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

                        bool ProgState = false;
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

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;

            if (FillColor != Color.Transparent)
            {
                using (var brush2 = new Pen(FillColor, _progWidth))
                {
                    g.DrawEllipse(brush2, rect);
                }
            }
            if (State || (_MaxValue > 0 && _Value > 0))
            {
                using (var brush = new Pen(Color, _progWidth))
                {
                    if (Round)
                        brush.StartCap = brush.EndCap = LineCap.Round;
                    g.DrawArc(brush, rect, LineAngle, (float)(LineWidth * 3.6));
                }
            }
        }

        public LoadingMaterial()
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
