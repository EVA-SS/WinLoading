using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TSkin
{
    [ToolboxItem(false)]
    public class LoadingCore : Control
    {
        public LoadingCore()
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

        #region 属性

        [Category("进度"), Description("进度颜色"), DefaultValue(typeof(Color), "0, 120, 220")]
        public Color ProgColor { get; set; } = Color.FromArgb(0, 120, 220);

        double _Value = 0.0, _MaxValue = 100.0;
        [Category("进度"), Description("当前值"), DefaultValue(0.0)]
        public double Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    if (value > _MaxValue)
                    { _Value = _MaxValue; }
                    else
                    {
                        _Value = value;
                    }
                    Invalidate();
                }
            }
        }

        [Category("进度"), Description("最大值"), DefaultValue(100.0)]
        public double MaxValue
        {
            get { return _MaxValue; }
            set
            {
                if (_MaxValue != value)
                {
                    _MaxValue = value;
                    this.Invalidate();
                }
            }
        }

        #endregion

        public void PaintProg(Graphics g, Rectangle rect)
        {
            if (_Value > 0 && _MaxValue > 0)
            {
                var prog = Math.Round(rect.Width * (_Value / _MaxValue), 3);
                if (prog > 0)
                {
                    using (var brush = new SolidBrush(ProgColor))
                    {
                        g.FillRectangle(brush, new RectangleF(rect.X, rect.Y, (float)prog, rect.Height));
                    }
                }
            }
        }
    }
}
