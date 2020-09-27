using System;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using StructuralPlaneStatistics.Classes;

namespace StructuralPlaneStatistics.Views
{
    /// <summary>
    /// 赤平投影
    /// </summary>
    class StereographicView : View
    {
        public StereographicView(Context context)
           : base(context)
        {
            Initialize();
            init();
        }
        public StereographicView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
            init();
        }

        public StereographicView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
            init();
        }

        private float direction = 0;
        private float dip = 0;
        private Android.Graphics.Paint paint = new Paint(PaintFlags.AntiAlias);
        private bool firstDraw;

        private void Initialize()
        {
            init();
        }
        private void init()
        {

            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = 3;
            paint.Color = Color.Black;
            paint.TextSize = 30;

            firstDraw = true;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            SetMeasuredDimension(MeasureSpec.GetSize(widthMeasureSpec), MeasureSpec.GetSize(heightMeasureSpec));
        }

        protected override void OnDraw(Canvas canvas)
        {

            int cxCompass = MeasuredWidth / 2;
            int cyCompass = MeasuredHeight / 2;
            float radiusCompass;

            if (cxCompass > cyCompass)
            {
                radiusCompass = (float)(cyCompass * 0.9);
            }
            else
            {
                radiusCompass = (float)(cxCompass * 0.9);
            }
            canvas.DrawCircle(cxCompass, cyCompass, radiusCompass, paint);
            canvas.DrawLine(cxCompass - 10, cyCompass, cxCompass + 10, cyCompass, paint);
            canvas.DrawLine(cxCompass, cyCompass - 10, cxCompass, cyCompass + 10, paint);
            canvas.DrawRect(0, 0, MeasuredWidth, MeasuredHeight, paint);

            canvas.DrawText("N", cxCompass, cyCompass - radiusCompass - 10, paint);
            canvas.DrawText("S", cxCompass, cyCompass + radiusCompass + 30, paint);
            canvas.DrawText("E", cxCompass + radiusCompass + 5, cyCompass, paint);
            canvas.DrawText("W", cxCompass - radiusCompass - 30, cyCompass, paint);

            if (!firstDraw)
            {

                TdPoint P1 = new TdPoint();
                TdPoint P2 = new TdPoint();
                TdPoint P0 = new TdPoint();
                TdPoint Ps = new TdPoint();
                P0.X = (cxCompass + radiusCompass * Math.Sin((double)(direction - 180 * Values.tyfs) * 3.14 / 180) * (float)(dip / 90));
                P0.Y = (cyCompass - radiusCompass * Math.Cos((double)(direction - 180 * Values.tyfs) * 3.14 / 180) * (float)(dip / 90));
                P1.X = (cxCompass + radiusCompass * Math.Sin((double)(direction + 90) * 3.14 / 180));
                P1.Y = (cyCompass - radiusCompass * Math.Cos((double)(direction + 90) * 3.14 / 180));
                P2.X = (cxCompass + radiusCompass * Math.Sin((double)(direction - 90) * 3.14 / 180));
                P2.Y = (cyCompass - radiusCompass * Math.Cos((double)(direction - 90) * 3.14 / 180));

                Ps.X = (cxCompass + radiusCompass * Math.Sin((double)(Lim360(direction + 180 * Values.tyfs)) * 3.14 / 180) * (1 - dip / 90.0));
                Ps.Y = (cyCompass - radiusCompass * Math.Cos((double)(Lim360(direction + 180 * Values.tyfs)) * 3.14 / 180) * (1 - dip / 90.0));
                Intersection intersection = new Intersection();
                TdLine l1 = intersection.CalMidLine(P1, Ps);
                TdLine l2 = intersection.CalMidLine(P2, Ps);
                TdPoint Pc = intersection.CalIPoint(l1, l2);
                double dist = intersection.Distance(Pc, P1);

                //canvas.DrawCircle((float)Pc.X, (float)Pc.Y, (float)dist, paint);//赤平投影圆弧
                //canvas.DrawLine((float)Ps.X, (float)Ps.Y, (float)P0.X, (float)P0.Y, paint);//倾角倾向线
                canvas.DrawLine((float)P1.X, (float)P1.Y, (float)P2.X, (float)P2.Y, paint);//走向线

                RectF re = new RectF((float)(Pc.X - dist), (float)(Pc.Y - dist), (float)(Pc.X + dist), (float)(Pc.Y + dist));
                double[] angle = new double[2];
                angle[0] = GetAngle(P2, Pc);
                angle[1] = GetAngle(P1, Pc);
                paint.SetStyle(Paint.Style.Stroke);
                if (Math.Abs(angle[1] - angle[0]) >= 180) //层面赤平投影
                {
                    canvas.DrawArc(re, (float)angle[Values.tyfs], (float)(360 - Math.Abs(angle[0] - angle[1])), false, paint);
                }
                else
                {
                    canvas.DrawArc(re, (float)angle[Values.tyfs], (float)(Math.Abs(angle[1] - angle[0])), false, paint);
                }
            }
        }

        public void updateDirection(float dir, float d)
        {
            firstDraw = false;
            direction = dir;
            dip = d;
            this.Invalidate();
        }

        private double Lim360(double angle)
        {
            return (angle + 720) % 360;
        }

        private double GetAngle(TdPoint P, TdPoint O)
        {
            double angle = Math.Atan((P.Y - O.Y) / (P.X - O.X)) * 180 / Math.PI;
            if ((P.X - O.X) >= 0)
            {
                return angle;
            }
            else
            {
                if ((P.Y - O.Y) >= 0)
                {
                    return angle + 180;
                }
                else
                {
                    return angle - 180;
                }
            }
        }

        public class Values
        {
            public static int tyfs = 1;//投影方式:1上半球投影 0下半球投影
        }

    }
}