using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using StructuralPlaneStatistics.Activities;
using StructuralPlaneStatistics.Classes;

namespace StructuralPlaneStatistics.Views
{
    public class StatisticView : View
    {
        Paint paint;
        RelativeLayout rl;
        MainActivity main;

        public StatisticView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            main = context as MainActivity;
            paint = new Paint() { Color = Color.Black, AntiAlias = true, StrokeWidth = 5,TextSize=32 };
            rl = main.rl;
            Initialize();
        }

        public StatisticView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
        }
        protected override void OnDraw(Canvas canvas)
        {
            Clean(canvas);
            //DarwResults(canvas);
            base.OnDraw(canvas);
        }

        private void Clean(Canvas canvas)
        {
            Paint p = new Paint() { Color = Color.White };
            p.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Clear));
            canvas.DrawPaint(p);
            p = new Paint() { Color = Color.White };
            p.SetStyle(Paint.Style.Fill);
            canvas.DrawRect(0, 0, rl.Width, rl.Height, p);
        }

        //private void DarwResults(Canvas canvas)
        //{
        //    Statistic statistic = new Statistic();
        //    double meanlength = statistic.GetMeanLength();
        //    double connectivity = statistic.GetConnectivity();
        //    double meanspace = statistic.GetMeanSpace();

        //    string text = $"测窗大小：{App.InputWidth}x{App.InputHeight}m\n\r结构面总数：{App.planes.Count}\n\r平均迹长：{meanlength}m\n\r连通率：{connectivity}\n\r平均间距：{meanspace}m";

        //    canvas.DrawText(text, 50, 50, paint);
        //}

    }
}