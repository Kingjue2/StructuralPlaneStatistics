using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using StructuralPlaneStatistics.Activities;
using StructuralPlaneStatistics.Classes;

namespace StructuralPlaneStatistics.Views
{
    public class StructPlaneView : View
    {
        private float X1, Y1, X2, Y2;
        private Paint r_paint = new Paint() { Color = Color.Red, AntiAlias = true, StrokeWidth = 5 };      
        private Rect rect;
        private MainActivity a1;
        int index;

        public StructPlaneView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            a1 = context as MainActivity;
            r_paint.SetStyle(Paint.Style.Stroke);
            Initialize();
        }

        public StructPlaneView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
        }

        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            //Clean(canvas);

            rect = new Rect(App.DrawOffset, App.DrawOffset, App.DrawOffset + App.DrawWidth, App.DrawOffset + App.DrawHeight);
            canvas.DrawRect(rect, r_paint);
            if (App.planes != null && App.planes.Count > 0)
            {
                foreach (StructPlane plane in App.planes)
                {
                    plane.Draw(canvas);
                }
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

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (App.planeState)
            {
                case App.PlaneState.Add:
                    AddPlane(e);
                    break;
                case App.PlaneState.Modify:
                    ModifyPlane(e);
                    break;
            }
            return true;
        }

        private void AddPlane(MotionEvent e)
        {
            StructPlane plane;
            switch ((int)e.Action)
            {
                case (int)MotionEventActions.Down:
                    X1 = e.GetX();
                    Y1 = e.GetY();
                    plane = new StructPlane(X1, Y1, X1, Y1);
                    App.planes.Add(plane);
                    index = App.planes.IndexOf(plane);
                    Invalidate();
                    break;
                case (int)MotionEventActions.Move:
                    App.planes[index].Move2To(e.GetX(), e.GetY());
                    Invalidate();
                    break;
                case (int)MotionEventActions.Up:
                    App.planes[index] = CutStructPlane(App.planes[index]);
                    Invalidate();
                    //a1.AddPlane(index);
                    Intent intent = new Intent(a1, typeof(DescriptionActivity));
                    Bundle b = new Bundle();
                    //b.PutInt("show", 1);
                    b.PutInt("position", index);
                    intent.AddFlags(ActivityFlags.TaskOnHome);
                    intent.PutExtras(b);
                    a1.StartActivity(intent);

                    break;
            }
        }


        private void ModifyPlane(MotionEvent e)
        {
            int Action = (int)e.Action;
            switch (Action)
            {
                case (int)MotionEventActions.Down:
                    for (int i = App.planes.Count-1; i >= 0; i--)
                    {
                        if (App.planes[i].SetState(e.GetX(), e.GetY(), MotionEventActions.Down))
                        {
                            break;
                        }
                    }
                    //foreach (StructPlane plane in App.planes)
                    //{
                    //    if (plane.SetState(e.GetX(), e.GetY(), MotionEventActions.Down))
                    //    {
                    //        break;
                    //    }
                    //}
                    break;
                case (int)MotionEventActions.Move:
                    for (int i = App.planes.Count - 1; i >= 0; i--)
                    {
                        if (App.planes[i].Modify(e.GetX(), e.GetY()))
                        {
                            break;
                        }
                    }

                    //foreach (StructPlane plane in App.planes)
                    //{
                    //    if (plane.Modify(e.GetX(), e.GetY()))
                    //    {
                    //        break;
                    //    }
                    //}
                    Invalidate();
                    break;
                case (int)MotionEventActions.Up:
                    for (int i = App.planes.Count - 1; i >= 0; i--)
                    {
                        if (App.planes[i].SetState(e.GetX(), e.GetY(), MotionEventActions.Up))
                        {
                            //int index = App.planes.IndexOf(plane);
                            //App.planes[index] = CutStructPlane(plane);
                            App.planes[i] = CutStructPlane(App.planes[i]);
                            Invalidate();
                            break;

                        }
                    }

                    //foreach (StructPlane plane in App.planes)
                    //{
                    //    if (plane.SetState(e.GetX(), e.GetY(), MotionEventActions.Up))
                    //    {
                    //        int index = App.planes.IndexOf(plane);
                    //        App.planes[index] = CutStructPlane(plane);
                    //        Invalidate();
                    //        break;
                    //    }
                    //}
                    break;
            }

        }

        public StructPlane CutStructPlane(StructPlane structPlane)
        {
            Intersection intersection = new Intersection();
            TdLine line = intersection.CalLine(structPlane.X1, structPlane.Y1, structPlane.X2, structPlane.Y2);
            TdLine line2;
            TdPoint point;
            bool p1out = false;
            bool p2out = false;
            //structPlane.visible1 = 1;
            //structPlane.visible2 = 1;

            if (structPlane.X1 < App.left)
            {
                line2 = new TdLine
                {
                    C = 0,
                    B = -1 * App.left
                };
                point = intersection.CalIPoint(line, line2);
                structPlane.X1 = (float)point.X;
                structPlane.Y1 = (float)point.Y;
                structPlane.visible1 = 0;
                p1out = true;
            }
                if (structPlane.X1 > App.right)
            {
                line2 = new TdLine
                {
                    C = 0,
                    B = -1 * App.right
                };
                point = intersection.CalIPoint(line, line2);
                structPlane.X1 = (float)point.X;
                structPlane.Y1 = (float)point.Y;
                structPlane.visible1 = 0;
                p1out = true;
            }
            if ( Math.Abs( structPlane.X1 - App.left) < 0.01f || Math.Abs(structPlane.X1 - App.right) < 0.01f)
            {
                p1out = true;
            }
            if (structPlane.Y1 < App.top)
            {
                line2 = new TdLine
                {
                    C = 1,
                    B = App.top,
                    A = 0
                };
                point = intersection.CalIPoint(line, line2);
                structPlane.X1 = (float)point.X;
                structPlane.Y1 = (float)point.Y;
                structPlane.visible1 = 0;
                p1out = true;
            }
            if (structPlane.Y1 > App.bottom)
            {
                line2 = new TdLine
                {
                    C = 1,
                    B = App.bottom,
                    A = 0
                };
                point = intersection.CalIPoint(line, line2);
                structPlane.X1 = (float)point.X;
                structPlane.Y1 = (float)point.Y;
                structPlane.visible1 = 0;
                p1out = true;
            }
            if (Math.Abs(structPlane.Y1 - App.top) < 0.01f || Math.Abs(structPlane.Y1 - App.bottom) < 0.01f)
            {
                p1out = true;
            }

            if (structPlane.X2 < App.left)
            {
                line2 = new TdLine
                {
                    C = 0,
                    B = -1 * App.left
                };
                point = intersection.CalIPoint(line, line2);
                structPlane.X2 = (float)point.X;
                structPlane.Y2 = (float)point.Y;
                structPlane.visible2 = 0;
                p2out = true;
            }
            if (structPlane.X2 > App.right)
            {
                line2 = new TdLine
                {
                    C = 0,
                    B = -1 * App.right
                };
                point = intersection.CalIPoint(line, line2);
                structPlane.X2 = (float)point.X;
                structPlane.Y2 = (float)point.Y;
                structPlane.visible2 = 0;
                p2out = true;
            }
            if (Math.Abs(structPlane.X2 - App.left) < 0.01f || Math.Abs(structPlane.X2 - App.right) < 0.01f)
            {
                p2out = true;
            }

            if (structPlane.Y2 < App.top)
            {
                line2 = new TdLine
                {
                    C = 1,
                    B = App.top,
                    A = 0
                };
                point = intersection.CalIPoint(line, line2);
                structPlane.X2 = (float)point.X;
                structPlane.Y2 = (float)point.Y;
                structPlane.visible2 = 0;
                p2out = true;
            }
            if (structPlane.Y2 > App.bottom)
            {
                line2 = new TdLine
                {
                    C = 1,
                    B = App.bottom,
                    A = 0
                };
                point = intersection.CalIPoint(line, line2);
                structPlane.X2 = (float)point.X;
                structPlane.Y2 = (float)point.Y;
                structPlane.visible2 = 0;
                p2out = true;
            }
            if (Math.Abs(structPlane.Y2 - App.top) < 0.01f || Math.Abs(structPlane.Y2 - App.bottom) < 0.01f)
            {
                p2out = true;
            }

            if (p1out)
            {
                structPlane.visible1 = 0;
            }
            else
            {
                structPlane.visible1 = 1;
            }
            if (p2out)
            {
                structPlane.visible2 = 0;
            }
            else
            {
                structPlane.visible2 = 1;
            }

            return structPlane;
        }
    }

   
}