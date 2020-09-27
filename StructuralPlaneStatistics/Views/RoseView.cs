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
    public class RoseView : View
    {
        Paint paint;
        Paint text;
        Paint strike;
        Paint dipdirection;
        Paint dipangle;
        RelativeLayout rl;
        MainActivity main;

        List<List<StructPlane>> planes;
        List<List<StructPlane>> strike_planes;
        int[] dipdirection_count;
        int[] dipdirection_sum;
        int dipdirection_maxcount;
        float[] dipdirection_average;

        int[] dipangle_sum;
        int dipangle_maxcount;
        float[] dipangle_average;

        int[] strike_count;
        int[] strike_sum;
        int strike_maxcount;
        float[] strike_average;

        float Current_X, Current_Y;
        float Previous_X, Previous_Y;
        public RoseView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            main = context as MainActivity;
            paint = new Paint() { Color = Color.Black, AntiAlias = true, StrokeWidth = 5 };
            paint.SetStyle(Paint.Style.Stroke);

            strike = new Paint() { Color = Color.LightPink, Alpha = 128, AntiAlias = true, StrokeWidth = 5 };
            dipdirection = new Paint() { Color = Color.LightGreen, Alpha = 128, AntiAlias = true, StrokeWidth = 5 };
            dipangle = new Paint() { Color = Color.Orange, Alpha = 128, AntiAlias = true, StrokeWidth = 5 };

            text = new Paint() { Color = Color.Black, AntiAlias = true, StrokeWidth = 1 };
            text.TextSize = 32;
            text.TextAlign = Paint.Align.Center;
            rl = main.rl;
            Initialize();
        }

        public RoseView(Context context, IAttributeSet attrs, int defStyle) :
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
            SetData();
            DrawStrike(canvas);
            DrawDip(canvas);
            DrawLegend(canvas);
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

        private void DrawLegend(Canvas canvas)
        {
            float rect_half_width = (float)rl.Width / 2 > rl.Height ? (float)rl.Height * 0.4f : (float)rl.Width / 2 * 0.4f;

            float height = (float)rl.Height / 12;
            float x1 = (float)rl.Width / 4 - rect_half_width;
            float x2 = height * 1.6f + x1;

            Paint paint_text = new Paint() { Color = Color.Black, AntiAlias = true, StrokeWidth = 5 };
            paint_text.TextSize = 54;

            RectF rectF_strike = new RectF(x1, (float)rl.Height / 2 + height * 2, x2, (float)rl.Height / 2 + height * 2.8f);
            canvas.DrawRect(rectF_strike, strike);
            canvas.DrawRect(rectF_strike, paint);
            canvas.DrawText("Strike", x2 + 20, (float)rl.Height / 2 + height * 2.65f,paint_text);
            RectF rectF_dipdirection = new RectF(x1, (float)rl.Height / 2 + height * 3, x2, (float)rl.Height / 2 + height * 3.8f);
            canvas.DrawRect(rectF_dipdirection, dipdirection);
            canvas.DrawRect(rectF_dipdirection, paint);
            canvas.DrawText("Dip Direction", x2 + 20, (float)rl.Height / 2 + height * 3.65f,paint_text);
            RectF rectF_dipangle = new RectF(x1, (float)rl.Height / 2 + height * 4, x2, (float)rl.Height / 2 + height * 4.8f);
            canvas.DrawRect(rectF_dipangle, dipangle);
            canvas.DrawRect(rectF_dipangle, paint);
            canvas.DrawText("Dip Angle", x2 + 20, (float)rl.Height / 2 + height * 4.65f,paint_text);
        }


        private void DrawStrike(Canvas canvas)
        {
            float rect_half_width = (float)rl.Width / 2 > rl.Height ? (float)rl.Height * 0.4f : (float)rl.Width / 2 * 0.4f;
            RectF rectF_strike = new RectF((float)rl.Width / 4 - rect_half_width, (float)rl.Height / 2 - rect_half_width, (float)rl.Width / 4 + rect_half_width, (float)rl.Height / 2 + rect_half_width);

            canvas.DrawArc(rectF_strike, -180, 180, false, paint);
            canvas.DrawLine((float)rl.Width / 4 - rect_half_width, (float)rl.Height / 2, (float)rl.Width / 4 + rect_half_width, (float)rl.Height / 2, paint);

            Previous_X = (float)rl.Width / 4 ;
            Previous_Y = (float)rl.Height / 2;
            int max = 0;
            int interval = 0;
            //比例尺
            GetMaxandInterval(strike_maxcount,out  max, out interval);
            DrawScale(canvas, (float)rl.Width / 4, (float)rl.Height / 2 + 50, rect_half_width, max, interval);
            //角度
            DrawAngleLine(canvas, -90, 90, 30, (float)rl.Width / 4, (float)rl.Height / 2, rect_half_width);

            Path path = new Path();
            path.MoveTo(Previous_X, Previous_Y);
            for (int i = 0; i < 18; i++)
            {
                CalculateXY((float)rl.Width / 4, (float)rl.Height / 2, strike_average[i], (float)strike_count[i] / max * rect_half_width, out Current_X, out Current_Y);

                canvas.DrawLine(Previous_X, Previous_Y, Current_X, Current_Y, paint);
                path.QuadTo(Previous_X, Previous_Y, Current_X, Current_Y);
                Previous_X = Current_X;
                Previous_Y = Current_Y;
            }
            path.Close();
            canvas.DrawPath(path, strike);
            canvas.DrawLine(Previous_X, Previous_Y, (float)rl.Width / 4, (float)rl.Height / 2, paint);



        }

        private void DrawDip(Canvas canvas)
        {
            float rect_half_width = (float)rl.Width / 2 > rl.Height ? (float)rl.Height * 0.4f : (float)rl.Width / 2 * 0.4f;
            RectF rectF_dip = new RectF((float)rl.Width / 4 * 3 - rect_half_width, (float)rl.Height / 2 - rect_half_width, (float)rl.Width / 4 * 3 + rect_half_width, (float)rl.Height / 2 + rect_half_width);

            canvas.DrawArc(rectF_dip, 0, 360, false, paint);
            int max = 0;
            int interval = 0;//比例尺
            GetMaxandInterval(dipdirection_maxcount, out max, out interval);
            DrawScale(canvas, (float)rl.Width / 4 * 3, (float)rl.Height / 2 + rect_half_width + 80, rect_half_width, max, interval);

            //角度
            DrawAngleLine(canvas, 0, 360, 30, (float)rl.Width / 4 * 3, (float)rl.Height / 2, rect_half_width);


            Previous_X = (float)rl.Width / 4 * 3;
            Previous_Y = (float)rl.Height / 2;

            Path path = new Path();
            path.MoveTo(Previous_X, Previous_Y);

            for (int i = 0; i < 36; i++)
            {
                CalculateXY((float)rl.Width / 4 * 3, (float)rl.Height / 2, dipdirection_average[i], (float)dipdirection_count[i] / max * rect_half_width, out Current_X, out Current_Y);
                canvas.DrawLine(Previous_X, Previous_Y, Current_X, Current_Y, paint);
                path.QuadTo(Previous_X, Previous_Y, Current_X, Current_Y);
                Previous_X = Current_X;
                Previous_Y = Current_Y;
            }
            path.Close();
            canvas.DrawPath(path, dipdirection);
            canvas.DrawLine(Previous_X, Previous_Y, (float)rl.Width / 4 * 3, (float)rl.Height / 2, paint);

            Previous_X = (float)rl.Width / 4 * 3;
            Previous_Y = (float)rl.Height / 2;
            path = new Path();
            path.MoveTo(Previous_X, Previous_Y);
            for (int i = 0; i < 36; i++)
            {
                CalculateXY((float)rl.Width / 4 * 3, (float)rl.Height / 2, dipdirection_average[i], (float)dipangle_average[i] / 90 * rect_half_width, out Current_X, out Current_Y);
                canvas.DrawLine(Previous_X, Previous_Y, Current_X, Current_Y, paint);
                path.QuadTo(Previous_X, Previous_Y, Current_X, Current_Y);
                Previous_X = Current_X;
                Previous_Y = Current_Y;
            }
            path.Close();
            canvas.DrawPath(path, dipangle);
            canvas.DrawLine(Previous_X, Previous_Y, (float)rl.Width / 4 * 3, (float)rl.Height / 2, paint);
        }

        public void SetData()
        {
            //App.planes.Clear();
            //Random rnd = new Random();
            //for (int i = 0; i < 100; i++)
            //{
            //    App.planes.Add(new StructPlane(rnd.Next(0, 360), rnd.Next(0, 90)));
            //}
            planes = new List<List<StructPlane>>();
            strike_planes = new List<List<StructPlane>>();
            dipdirection_count = new int[36];
            dipdirection_sum = new int[36];
            dipdirection_maxcount = 0;
            dipdirection_average = new float[36];
            dipangle_sum = new int[36];
            dipangle_average = new float[36];
            strike_count = new int[18];
            strike_sum = new int[18];
            strike_average = new float[18];
            strike_maxcount = 0;

            for (int i = 0; i < 36; i++)
            {
                planes.Add(new List<StructPlane>());
            }
            for (int i = 0; i < 18; i++)
            {
                strike_planes.Add(new List<StructPlane>());
            }
            for (int i = 0; i < App.planes.Count; i++)
            {
                planes[GetIndex(App.planes[i].DipDirection, 10)].Add(App.planes[i]);
                
            }
            for (int i = 0; i < 36; i++)
            {
                dipdirection_count[i] = planes[i].Count;
                dipdirection_sum[i] = 0;
                dipangle_sum[i] = 0;

                for (int j = 0; j < planes[i].Count; j++)
                {
                    //count[i]++;
                    dipdirection_sum[i] += planes[i][j].DipDirection;
                    dipangle_sum[i] += planes[i][j].DipAngle;
                }

                dipdirection_maxcount = planes[i].Count > dipdirection_maxcount ? planes[i].Count : dipdirection_maxcount;

                if (planes[i].Count > 0)
                {
                    dipdirection_average[i] = dipdirection_sum[i] / planes[i].Count;
                    dipangle_average[i] = dipangle_sum[i] / planes[i].Count;
                }
                else
                {
                    dipdirection_average[i] = 0;
                    dipangle_average[i] = 0;
                }
            }

            //走向
            for (int i = 0; i < App.planes.Count; i++)
            {
                  strike_planes[GetStrikeIndex(App.planes[i].Strike, 10)].Add(App.planes[i]);
            }
            for (int i = 0; i< 18;i++)
            {
                strike_count[i] = strike_planes[i].Count;
                strike_sum[i] = 0;
                for (int j = 0; j < strike_planes[i].Count; j++)
                {
                    //count[i]++;
                    strike_sum[i] += strike_planes[i][j].Strike;
                }

                strike_maxcount = strike_planes[i].Count > strike_maxcount ? strike_planes[i].Count : strike_maxcount;

                if (strike_planes[i].Count > 0)
                {
                    strike_average[i] = strike_sum[i] / strike_planes[i].Count;
                }
                else
                {
                    strike_average[i] = 0;
                }
            }


        }
        private int GetIndex(int dipangle, int interval)
        {
            return dipangle / interval;
        }

        private int GetStrikeIndex(int strike, int interval)
        {
            int x = strike / interval;
            if(x < 360/interval/4)
            {
                return x + 360 / interval / 4;
            }
            else
            {
                if (x == 360 / interval / 4)
                {
                    return x + 360 / interval / 4 - 1;
                }
                else
                {
                    return x - 360 / interval / 4 * 3;
                }
            }
        }

        private void CalculateXY(float O_X, float O_Y, float azumth, float length, out float X, out float Y)
        {
            X = (float)(O_X + Math.Sin(radians(azumth)) * length);
            Y = (float)(O_Y - Math.Cos(radians(azumth)) * length);
        }
        private double radians(double degrees)
        {
            return degrees / 180 * Math.PI;
        }
        private double degrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        private void DrawScale(Canvas canvas, float center_X, float center_Y, float length, int max,int interval)
        {
            //比例尺
            canvas.DrawLine(center_X - length / 2, center_Y, center_X + length / 2, center_Y, paint);
            int count = max / interval;

            for (int i = 0; i <= count; i++)
            {
                canvas.DrawLine(center_X - length / 2 + i * length / count, center_Y, center_X - length / 2 + i * length / count, center_Y - 10, paint);
                canvas.DrawText((i * interval).ToString(), center_X - length / 2 + i * length / count, center_Y + 30, text);
            }
        }

        private void DrawAngleLine(Canvas canvas,int start,int end,int interval, float center_X, float center_Y,float radius)
        {
            int angle = 0;
            float x1,y1,x2,y2,x3,y3;

            for (int i = start; i <= end; i += 10)
            {
                angle = (i + 720) % 360;
                CalculateXY(center_X, center_Y, angle, radius, out x1, out y1);
                CalculateXY(center_X, center_Y, angle, radius * 0.95f, out x2, out y2);
                canvas.DrawLine(x1, y1, x2, y2, paint);

                CalculateXY(center_X, center_Y, angle, radius * 1.05f, out x3, out y3);
                if (angle > 0 && angle < 180)
                {
                    text.TextAlign = Paint.Align.Left;
                }
                else {
                    if (angle > 180 && angle < 360)
                    {
                        text.TextAlign = Paint.Align.Right;
                    }
                    else
                    {
                        text.TextAlign = Paint.Align.Center;
                    }
                }
                if (angle > 90 && angle < 270) { y3 += 32; }
                if (angle % interval == 0)
                {
                    canvas.DrawText(angle.ToString(), x3, y3, text);
                }
            }

        }


        private void GetMaxandInterval(int maxcount, out int max, out int interval)
        {
            max = maxcount;
            interval = 2;
            if ((float)maxcount / 2 <= 5)
            {
                interval = 2;
                max = maxcount % interval == 0 ? maxcount : maxcount / interval * interval + interval;
            }
            else
            {
                if ((float)maxcount / 5 <= 5)
                {
                    interval = 5;
                    max = maxcount % interval == 0 ? maxcount : maxcount / interval * interval + interval;
                }
                else
                {
                    if ((float)maxcount / 10 <= 5)
                    {
                        interval = 10;
                        max = maxcount % interval == 0 ? maxcount : maxcount / interval * interval + interval;
                    }
                    else
                    {
                        if ((float)maxcount / 20 <= 5)
                        {
                            interval = 20;
                            max = maxcount % interval == 0 ? maxcount : maxcount / interval * interval + interval;
                        }
                        else
                        {
                            if ((float)maxcount / 50 <= 5)
                            {
                                interval = 50;
                                max = maxcount % interval == 0 ? maxcount : maxcount / interval * interval + interval;
                            }
                            else
                            {
                                if ((float)maxcount / 100 <= 5)
                                {
                                    interval = 100;
                                    max = maxcount % interval == 0 ? maxcount : maxcount / interval * interval + interval;
                                }
                                else
                                {
                                    interval = 200;
                                    max = maxcount % interval == 0 ? maxcount : maxcount / interval * interval + interval;
                                }
                            }
                        }
                    }
                }

            }

        }
    }
}