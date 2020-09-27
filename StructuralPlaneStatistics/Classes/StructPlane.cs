using System;
using Android.Graphics;
using Android.Views;

namespace StructuralPlaneStatistics.Classes
{
    public class StructPlane
    {
        enum State { In1,In2, Out }
        State state = State.Out;

        public StructPlane() { }
        public StructPlane(int DipDirection, int DipAngle)
        {
            this.DipDirection = DipDirection;
            this.DipAngle = DipAngle;
        }

        /// <summary>
        /// 倾向
        /// </summary>
        public int DipDirection { get; set; }
        /// <summary>
        /// 倾角
        /// </summary>
        public int DipAngle { get; set; }
        /// <summary>
        /// 走向
        /// </summary>
        public int Strike
        {
            get
            {
                if (DipDirection > 180)
                {
                    return (DipDirection + 90 + 720) % 360;
                }
                else
                {
                    return (DipDirection - 90 + 720) % 360;
                }
            }
        }
        /// <summary>
        /// 迹长
        /// </summary>
        public float Lengh
        {
            get
            {
                return (float)Math.Sqrt((X2 - X1) * (X2 - X1) + (Y2 - Y1) * (Y2 - Y1)) / App.DrawWidth * App.InputWidth;
            }
        }
        public float P1X
        {
            get
            {
                return (X1 - App.left) / App.DrawWidth * App.InputWidth;
            }
        }
        public float P2X
        {
            get
            {
                return (X2 - App.left) / App.DrawWidth * App.InputWidth;
            }
        }
        public float P1Y
        {
            get
            {
                return (-1 * Y1 + App.DrawHeight + App.top) / App.DrawWidth * App.InputWidth;
            }
        }
        public float P2Y
        {
            get
            {
                return (-1 * Y2 + App.DrawHeight + App.top) / App.DrawWidth * App.InputWidth;
            }
        }
        /// <summary>
        /// 结构面分组
        /// </summary>
        public PlaneGroup Group { get; set; }
        /// <summary>
        /// 起伏度
        /// </summary>
        public string Waviness { get; set; }
        /// <summary>
        /// 粗糙度
        /// </summary>
        public string Roughness { get; set; }
        /// <summary>
        /// 张开度
        /// </summary>
        public string Opening { get; set; }
        /// <summary>
        /// 充填物
        /// </summary>
        public string Filler { get; set; }
        /// <summary>
        /// 胶结程度
        /// </summary>
        public string Cementation { get; set; }
        /// <summary>
        /// 地下水
        /// </summary>
        public string Groundwater { get; set; }

        public int visible1 = 1;
        public int visible2 = 1;
        public int Visiblecount
        {
            get
            {
                return visible1 + visible2;
            }
        }

        public float X1, X2, Y1, Y2;
        private Paint paint;

        public StructPlane(float x1, float y1, float x2, float y2)
        {
            X1 = x1;
            X2 = x2;
            Y1 = y1;
            Y2 = y2;
            paint = new Paint() { Color = Color.Yellow, AntiAlias = true, Alpha = 196, StrokeWidth = 4 };
            paint.SetStyle(Paint.Style.Stroke);
        }

        public void Draw(Android.Graphics.Canvas canvas)
        {
            switch (Group)
            {
                case PlaneGroup.Bed:
                    paint.Color = new Color(20, 84, 140);
                    break;
                case PlaneGroup.Group1:
                    paint.Color = new Color(243, 111, 99);
                    break;
                case PlaneGroup.Group2:
                    paint.Color = new Color(80, 176, 174);
                    break;
                case PlaneGroup.Group3:
                    paint.Color = new Color(241, 192, 89);
                    break;
                case PlaneGroup.Group4:
                    paint.Color = new Color(16, 150, 117);
                    break;
                case PlaneGroup.Group5:
                    paint.Color = new Color(156, 27, 49);
                    break;
            }

            canvas.DrawLine(X1, Y1, X2, Y2, paint);
        }
        public void Move1(float x, float y)
        {
            X1 += x;
            Y1 += y;
        }
        public void Move1To(float x, float y)
        {
            X1 = x;
            Y1 = y;
        }
        public void Move2(float x, float y)
        {
            X2 += x;
            Y2 += y;
        }
        public void Move2To(float x, float y)
        {
            X2 = x;
            Y2 = y;
        }
        public bool Modify(float x, float y)
        {
            bool re = false;
            switch (state)
            {
                case State.In1:
                    X1 = x;
                    Y1 = y;
                    re = true;
                    break;
                case State.In2:
                    X2 = x;
                    Y2 = y;
                    re = true;
                    break;
                default:
                    re = false;
                    break;
            }
            return re;
        }



        public bool SetState(float x, float y, MotionEventActions mea)
        {
            if (Math.Sqrt((X1 - x) * (X1 - x) + (Y1 - y) * (Y1 - y)) < 50)
            {
                if (mea == MotionEventActions.Down)
                {
                    state = State.In1;
                }
                else
                {
                    state = State.Out;
                }
                return true;
            }
            else
            {
                if (Math.Sqrt((X2 - x) * (X2 - x) + (Y2 - y) * (Y2 - y)) < 50)
                {
                    if (mea == MotionEventActions.Down)
                    {
                        state = State.In2;
                    }
                    else
                    {
                        state = State.Out;
                    }
                    return true;
                }
                else
                {
                    state = State.Out;
                    return false;
                }
            }
        }
    }

    /// <summary>
    /// 结构面分组
    /// </summary>
    public enum PlaneGroup { Group1, Group2, Group3, Group4, Group5, Bed }
}