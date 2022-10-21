using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TSkin
{
    public class LoadingMaterialHorizontal : LoadingCore
    {
        #region 属性

        [Category("进度"), Description("是否等待最后一次动画结束"), DefaultValue(true)]
        public bool EndStop { get; set; } = true;

        [Category("进度"), Description("进度颜色"), DefaultValue(typeof(Color), "100, 0, 120, 220")]
        public Color Color { get; set; } = Color.FromArgb(100, 0, 120, 220);

        [Category("进度"), Description("动画方向状态"), DefaultValue(false)]
        public bool isLeft { get; set; } = false;

        #endregion

        #region 动画开关

        ThreadOne thread = null;
        bool _state = false, _real_state = false, isMo = false;
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
                        _real_state = true;
                        if (thread != null)
                            thread.Dispose();

                        thread = new ThreadOne(() =>
                        {
                            if (prog_x > _width)
                            {
                                if (!_state)
                                {
                                    if (thread != null)
                                        thread.Dispose();
                                    return;
                                }

                                isMo = !isMo;

                                prog_width = _width;
                                prog_x = -_width;
                                prog_width = isMo ? _width : _width / 2 + _width_min;
                            }
                            else
                            {
                                prog_x += prog;
                                if (isMo)
                                    prog_width -= prog_min;
                                else
                                    prog_x += prog / 2;
                            }
                            Invalidate();
                        }, () =>
                        {
                            _state = _real_state = false;
                            Invalidate();
                        }, () =>
                        {
                            return _state || EndStop;
                        }, 10);
                    }
                    else
                    {
                        if (!EndStop)
                        {
                            if (thread != null)
                                thread.Dispose();
                        }
                    }
                }
            }
        }

        #endregion

        #region 坐标计算

        float prog_x = 0, prog_width = 0;
        int _width = 0, _width_min = 0;
        float prog = 0, prog_min = 0;
        protected override void OnSizeChanged(EventArgs e)
        {
            _width = Width;
            prog = ((float)_width) / 40;
            prog_min = (float)Math.Floor(prog / 2.1);
            _width_min = _width / 3;
            base.OnSizeChanged(e);
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            //抗锯齿
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality; //高像素偏移质量
            Rectangle rect = ClientRectangle;
            PaintProg(g, rect);
            if (_real_state)
            {
                if (isLeft)
                {
                    g.TranslateTransform(_width, rect.Height);
                    g.RotateTransform(180);
                }
                using (var brush = new SolidBrush(Color))
                {
                    g.FillRectangle(brush, new RectangleF(prog_x, 0, prog_width, rect.Height));
                }
            }
            base.OnPaint(e);
        }
        protected override void Dispose(bool disposing)
        {
            State = false;
            base.Dispose(disposing);
        }
    }
}
