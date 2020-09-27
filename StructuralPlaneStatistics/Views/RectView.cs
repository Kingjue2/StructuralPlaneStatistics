using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using StructuralPlaneStatistics.Activities;
using StructuralPlaneStatistics.Classes;

namespace StructuralPlaneStatistics.Views
{
    public class RectView : View
    {
        private float distanceX;
        private float distanceY;
        private float Previous_X;
        private float Previous_Y;
        public List<CornerCircle> circles;
        private Paint line = new Paint() { Color = Color.Red, AntiAlias = true, StrokeWidth = 5 };
        private Rect r;
        private MainActivity a1;

        public RectView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            a1 = context as MainActivity;
            r = new Rect(0, 0, 1080, 1920);
            Initialize();
        }

        public RectView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            //载入时生成一个矩形
            //AddRects();
        }
        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            //Clean(canvas);
            //清空画布 
            if (circles != null && circles.Count == 4)
            {
                circles[0].Draw(canvas);
                circles[1].Draw(canvas);
                circles[2].Draw(canvas);
                circles[3].Draw(canvas);

                canvas.DrawLine(circles[0].Current_X, circles[0].Current_Y, circles[1].Current_X, circles[1].Current_Y, line);
                canvas.DrawLine(circles[1].Current_X, circles[1].Current_Y, circles[3].Current_X, circles[3].Current_Y, line);
                canvas.DrawLine(circles[2].Current_X, circles[2].Current_Y, circles[3].Current_X, circles[3].Current_Y, line);
                canvas.DrawLine(circles[2].Current_X, circles[2].Current_Y, circles[0].Current_X, circles[0].Current_Y, line);
            }

            base.OnDraw(canvas);
        }
        private void Clean(Canvas canvas)
        {
            Paint p = new Paint() { Color = Color.White };
            p.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Clear));
            canvas.DrawPaint(p);
            p = new Paint() { Color = Color.White };
            p.SetStyle(Paint.Style.Fill);
            canvas.DrawRect(0, 0, a1.rl.Width, a1.rl.Height, p);
        }

        public void SetRects()
        {
            circles = new List<CornerCircle>();
            //c1 c2 c3 c4 分别为 左上 右上 左下 右下

            CornerCircle c1 = new CornerCircle(App.LayoutWidth / 2 - 200, App.LayoutHeight / 2 - 200);
            CornerCircle c2 = new CornerCircle(App.LayoutWidth / 2 + 200, App.LayoutHeight / 2 - 200);
            CornerCircle c3 = new CornerCircle(App.LayoutWidth / 2 - 200, App.LayoutHeight / 2 + 200);
            CornerCircle c4 = new CornerCircle(App.LayoutWidth / 2 + 200, App.LayoutHeight / 2 + 200);
            circles.Add(c1);
            circles.Add(c2);
            circles.Add(c3);
            circles.Add(c4);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            int Action = (int)e.Action;
            switch (Action)
            {
                case (int)MotionEventActions.Down:
                    Previous_X = e.GetX();
                    Previous_Y = e.GetY();
                    foreach (CornerCircle circle in circles)
                    {
                        if (circle.SetState(Previous_X, Previous_Y, MotionEventActions.Down))
                        {
                            return true;
                        }
                    }
                    break;
                case (int)MotionEventActions.Move:
                    distanceX = e.GetX() - Previous_X;
                    distanceY = e.GetY() - Previous_Y;
                    Previous_X = e.GetX();
                    Previous_Y = e.GetY();
                    foreach (CornerCircle circle in circles)
                    {
                        if (circle.Move(distanceX, distanceY))
                        {
                            break;
                        }
                    }
                    Invalidate();
                    break;
                case (int)MotionEventActions.Up:
                    foreach (CornerCircle circle in circles)
                    {
                        if (circle.SetState(Previous_X, Previous_Y, MotionEventActions.Up))
                        {
                            JudgeCirclePosition();
                            return true;
                        }
                    }
                    break;
                //case (int)MotionEventActions.Pointer2Down:
                //    foreach (Circle circle in circles)
                //    {
                //        if (circle.SetState(e.GetX(1), e.GetY(1), MotionEventActions.Pointer2Down))
                //        {
                //            circles.Remove(circle);
                //            break;
                //        }
                //    }
                //    Invalidate();
                //    break;
                //case (int)MotionEventActions.Pointer3Down:
                //    {
                //        circles.Add(new Circle() { Current_X = e.GetX(2), Current_Y = e.GetY(2) });
                //        Invalidate();
                //        break;
                //    }
            }
            return true;
        }

        /// <summary>
        /// 判断矩形各角点位置
        /// </summary>
        private void JudgeCirclePosition()
        {
            if (circles[0].Current_X > circles[1].Current_X)
            {
                circles[0].Current_X = circles[1].Current_X - 10;
            }
            if (circles[0].Current_Y > circles[2].Current_Y)
            {
                circles[0].Current_Y = circles[2].Current_Y - 10;
            }
            if (circles[1].Current_Y > circles[3].Current_Y)
            {
                circles[1].Current_Y = circles[3].Current_Y - 10;
            }
            if (circles[2].Current_X > circles[3].Current_X)
            {
                circles[2].Current_X = circles[3].Current_X - 10;
            }
        }
    }
}