using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace TSkin
{
    public partial class LoadingMetroHorizontal : LoadingCore
    {
        #region 属性

        int _DotCount = 5;
        [Category("进度"), Description("点数量"), DefaultValue(5)]
        public int DotCount
        {
            get { return _DotCount; }
            set
            {
                if (_DotCount != value)
                {
                    _DotCount = value;
                    OnSizeChanged(null);
                }
            }
        }

        int _DotSize = 6;
        [Category("进度"), Description("点大小"), DefaultValue(6)]
        public int DotSize
        {
            get
            {
                return _DotSize;
            }
            set
            {
                if (_DotSize != value)
                {
                    _DotSize = value;
                    OnSizeChanged(null);
                }
            }
        }

        [Category("进度"), Description("是否等待最后一次动画结束"), DefaultValue(true)]
        public bool EndStop { get; set; } = true;

        [Category("进度"), Description("进度颜色"), DefaultValue(typeof(Color), "0, 120, 220")]
        public Color Color { get; set; } = Color.FromArgb(0, 120, 220);

        #endregion

        #region 动画开关

        ThreadOne thread = null;
        bool _state = false, real_state = false;
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
                        real_state = true;
                        if (thread != null)
                            thread.Dispose();

                        int DotCount = this.DotCount;
                        List<int> Cirular_OK = new List<int>();
                        Clear(10);
                        thread = new ThreadOne(() =>
                        {
                            if (Cirular_OK.Count == Cirular.Count)
                            {
                                if (!_state)
                                {
                                    if (thread != null)
                                        thread.Dispose();
                                    return;
                                }

                                Thread.Sleep(500);

                                Clear(80);
                                Cirular_OK.Clear();
                            }
                            else
                            {
                                for (int i = 0; i < DotCount; i++)
                                {
                                    Getint(Cirular_OK, i, rect, rect_down, _temp_down, f_down);
                                }
                            }
                            Invalidate();
                        }, () =>
                        {
                            _state = real_state = false;
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
                            real_state = false;
                            if (thread != null)
                                thread.Dispose();
                        }
                    }
                }
            }
        }

        #endregion

        #region 坐标计算

        Rectangle rect;
        Rectangle progrect;
        RectangleF rect_down;
        int _cirulary = 0;
        int width_down = 0;
        double _temp_down = 0;
        double f_down = 0;
        protected override void OnSizeChanged(EventArgs e)
        {
            rect = ClientRectangle;
            progrect = new Rectangle(rect.X, rect.Y + ((rect.Height - _DotSize) / 2), rect.Width, _DotSize);
            _cirulary = (rect.Height - _DotSize) / 2;
            width_down = rect.Width / 4;
            f_down = width_down / 3.0;
            rect_down = new RectangleF((rect.Width - width_down) / 2, 0, width_down, rect.Height);
            _temp_down = rect_down.X + rect_down.Width;

            base.OnSizeChanged(e);
        }

        #endregion

        #region 辅助方法

        void Clear(float val, float speed = 70)
        {
            List<float> Cirular = new List<float>();
            for (int i = 0; i < DotCount; i++)
            {
                Cirular.Add(-(val + (speed * i)));
            }
            this.Cirular = Cirular;
        }
        void Getint(List<int> Cirular_OK, int i, Rectangle rect, RectangleF rect_down, double _temp_down, double f_down)
        {
            if (!Cirular_OK.Contains(i))
            {
                double value = Cirular[i];
                if (value > rect_down.X && value < _temp_down)
                {
                    if (Cirular[i] > 1)
                    {
                        double bb = (rect.Width - value) / (100.0 + f_down);
                        Cirular[i] += (float)bb;
                    }
                }
                else
                {
                    if (rect.Width > value)
                    {
                        if (value > _temp_down)
                        {
                            double bb = (rect.Width - (rect.Width - value)) / 100.0;
                            Cirular[i] += (float)bb;
                        }
                        else
                        {
                            double bb = (rect.Width - value) / 100.0;
                            Cirular[i] += (float)bb;
                        }
                    }
                    else
                    {
                        Cirular[i] = -10;
                        Cirular_OK.Add(i);
                    }
                }
            }
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            //抗锯齿
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality; //高像素偏移质量
            PaintProg(g, progrect);
            if (real_state && Cirular.Count > 0)
            {
                for (int i = 0; i < Cirular.Count; i++)
                {
                    var rect = new RectangleF(new PointF((Cirular[i] - (_DotSize / 2)), _cirulary), new SizeF(_DotSize, _DotSize));
                    using (var brush = new SolidBrush(Color))
                    {
                        g.FillEllipse(brush, rect);
                    }
                }
            }
            base.OnPaint(e);
        }

        List<float> Cirular = new List<float>();
        protected override void Dispose(bool disposing)
        {
            State = false;
            base.Dispose(disposing);
        }
    }
}
