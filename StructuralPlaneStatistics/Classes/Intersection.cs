using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace StructuralPlaneStatistics.Classes
{

    /// <summary>
    /// 计算交点
    /// </summary>
    class Intersection
    {
        /// <summary>
        /// 计算直线参数
        /// </summary>
        /// <param name="P1">点1</param>
        /// <param name="P2">点2</param>
        /// <returns>返回直线参数</returns>
        public TdLine CalLine(TdPoint P1, TdPoint P2)
        {
            TdLine L = new TdLine();
            if (P1.X != P2.X)
            {
                L.A = (P2.Y - P1.Y) / (P2.X - P1.X);
                L.C = 1;
                L.B = P1.Y - L.A * P1.X;
            }
            else
            {
                L.A = 1;
                L.C = 0;
                L.B = -P1.X;
            }
            return L;
        }

        /// <summary>
        /// 计算直线参数
        /// </summary>
        /// <param name="X1"></param>
        /// <param name="Y1"></param>
        /// <param name="X2"></param>
        /// <param name="Y2"></param>
        /// <returns>返回直线参数</returns>
        public TdLine CalLine(float X1, float Y1, float X2, float Y2)
        {
            TdPoint P1 = new TdPoint(X1, Y1);
            TdPoint P2 = new TdPoint(X2, Y2);
            TdLine L = new TdLine();
            if (P1.X != P2.X)
            {
                L.A = (P2.Y - P1.Y) / (P2.X - P1.X);
                L.C = 1;
                L.B = P1.Y - L.A * P1.X;
            }
            else
            {
                L.A = 1;
                L.C = 0;
                L.B = -P1.X;
            }
            return L;
        }


        /// <summary>
        /// 计算中垂线参数
        /// </summary>
        /// <param name="P1">点1</param>
        /// <param name="P2">点2</param>
        /// <returns>返回中垂线参数</returns>
        public TdLine CalMidLine(TdPoint P1, TdPoint P2)
        {
            TdLine line = new TdLine();//返回的中垂线

            TdLine L = new TdLine();//两点所在直线
            if (P1.X != P2.X)
            {
                L.A = (P2.Y - P1.Y) / (P2.X - P1.X);
                L.C = 1;
                L.B = P1.Y - L.A * P1.X;
            }
            else
            {
                L.A = 1;
                L.C = 0;
                L.B = -P1.X;
            }

            TdPoint MidPoint = new TdPoint();//两点的中点
            MidPoint.X = (P2.X + P1.X) / 2;
            MidPoint.Y = (P2.Y + P1.Y) / 2;

            if (L.C == 1)//直线有斜率
            {
                if (L.A != 0)//斜率不为0
                {
                    line.A = -1 / L.A;
                    line.B = MidPoint.Y - line.A * MidPoint.X;
                    line.C = 1;
                }
                else
                {
                    line.C = 0;
                    line.A = 1;
                    line.B = -MidPoint.X;
                }
            }
            else
            {
                line.C = 1;
                line.A = 0;
                line.B = MidPoint.Y;
            }
            return line;
        }

        /// <summary>
        /// 计算量直线交点
        /// </summary>
        /// <param name="L1">直线1</param>
        /// <param name="L2">直线2</param>
        /// <returns>返回交点</returns>
        public TdPoint CalIPoint(TdLine L1, TdLine L2)
        {
            TdPoint p = new TdPoint();
            if (L1.C == 1 && L2.C == 1)//两直线都有有斜率
            {
                p.X = (L1.B - L2.B) / (L2.A - L1.A);
                p.Y = (L1.B * L2.A - L2.B * L1.A) / (L2.A - L1.A);
            }
            else
            {
                if (L1.C == 0)
                {
                    p.X = -L1.B;
                    p.Y = L2.A * p.X + L2.B;
                }
                else
                {
                    if (L2.C == 0)
                    {
                        p.X = -L2.B;
                        p.Y = L1.A * p.X + L1.B;
                    }
                }
            }
            return p;
        }

        /// <summary>
        /// 计算两点之间的距离
        /// </summary>
        /// <param name="P1">点1</param>
        /// <param name="P2">点2</param>
        /// <returns>返回距离</returns>
        public double Distance(TdPoint P1, TdPoint P2)
        {
            double dis;
            dis = Math.Sqrt((P2.Y - P1.Y) * (P2.Y - P1.Y) + (P2.X - P1.X) * (P2.X - P1.X));
            return dis;
        }

        /// <summary>
        /// 计算两个面的持平投影交点（设定大圆圆心为600,600半径480，上半球投影）
        /// </summary>
        /// <param name="trend1">面1倾向</param>
        /// <param name="dip1">面1倾角</param>
        /// <param name="trend2">面2倾向</param>
        /// <param name="dip2">面2倾角</param>
        /// <returns>返回两个面在赤平投影交点</returns>
        public CrossTdLine CalArcPoint(double trend1, double dip1, double trend2, double dip2)
        {
            Circle C1 = CalArc(trend1, dip1);
            Circle C2 = CalArc(trend2, dip2);
            //a b 为计算中间参数，经转换交点X=a+bY
            //double a = (C1.R * C1.R - C2.R * C2.R - C1.X * C1.X + C2.X * C2.X - C1.Y * C1.Y + C2.Y * C2.Y) / (2 * C2.X - 2 * C1.X);
            //double b = (C1.Y + C2.Y) / (C2.X - C1.X);

            //double Aa = b * b + 1;
            //double Ab =2*a*b -2 * C1.X * b - 2 * C1.Y;
            //double Ac = a * a - 2 * C1.X * a + C1.X * C1.X + C1.Y * C1.Y - C1.R * C1.R;

            TdPoint p1 = new TdPoint();
            TdPoint p2 = new TdPoint();
            TdPoint pc = new TdPoint();
            pc.X = 600; pc.Y = 600;

            //p1.X = ((-Ab + Math.Sqrt(Ab * Ab - 4 * Aa * Ac)) / (2 * Aa));
            //p2.X = ((-Ab - Math.Sqrt(Ab * Ab - 4 * Aa * Ac)) / (2 * Aa));
            //p1.Y = (p1.X - a) / b;
            //p2.Y = (p2.X - a) / b;
            double a = (C2.Y - C1.Y) / (C2.X - C1.X);
            double b = (C1.R * C1.R - C2.R * C2.R - C1.X * C1.X + C2.X * C2.X - C1.Y * C1.Y + C2.Y * C2.Y) / 2 / (C2.X - C1.X);
            double c = 2 * (a * (C1.X - b) - C1.Y) / (1 + a * a);
            double d = (b * b - 2 * C1.X * b + C1.Y * C1.Y + C1.X * C1.X - C1.R * C1.R) / (1 + a * a);

            p1.Y = (-1 * c + Math.Sqrt(c * c - 4 * d)) / 2;
            p1.X = b - a * p1.Y;
            p2.Y = (-1 * c - Math.Sqrt(c * c - 4 * d)) / 2;
            p2.X = b - a * p2.Y;

            if (Distance(p1, pc) < Distance(p2, pc))
            {
                return CalPlane(p1);
            }
            else
            {
                return CalPlane(p2);
            }
        }

        private CrossTdLine CalPlane(TdPoint p)
        {
            CrossTdLine pl = new CrossTdLine();
            TdPoint pc = new TdPoint();
            pc.X = 600; pc.Y = 600;
            if ((p.X - 600) == 0)//0/180
            {
                if ((p.Y - 600) > 0)
                {
                    pl.trend = 180;
                    pl.dip = (1080 - p.Y) / 480 * 90;
                }
                else
                {
                    pl.trend = 0;
                    pl.dip = (p.Y - 120) / 480 * 90;
                }
            }
            else
            {
                if ((p.Y - 600) == 0)//90/270
                {
                    if ((p.X - 600) > 0)
                    {
                        pl.trend = 90;
                        pl.dip = (1080 - p.X) / 480 * 90;
                    }
                    else
                    {
                        pl.trend = 270;
                        pl.dip = (p.X - 120) / 480 * 90;
                    }
                }
                else
                {
                    if ((p.X - 600) > 0)//1 4 象限
                    {
                        if ((p.Y - 600) > 0)//4象限
                        {
                            pl.trend = Lim360(0 - Math.Atan((p.X - 600) / (p.Y - 600)) / Math.PI * 180);
                        }
                        else//1象限
                        {
                            pl.trend = Lim360(Math.Atan((p.X - 600) / (600 - p.Y)) / Math.PI * 180 + 180);
                        }
                    }
                    else//2 3象限
                    {
                        if ((p.Y - 600) > 0)//3象限
                        {
                            pl.trend = Lim360(Math.Atan((600 - p.X) / (p.Y - 600)) / Math.PI * 180);
                        }
                        else//2象限
                        {
                            pl.trend = Lim360(180 - Math.Atan((600 - p.X) / (600 - p.Y)) / Math.PI * 180);
                        }

                    }

                    pl.dip = (480 - Distance(pc, p)) / 480 * 90;
                }
            }
            return pl;
        }

        /// <summary>
        /// 计算赤平投影中，面投影圆弧的圆心（设定大圆圆心为600,600半径480，上半球投影）
        /// </summary>
        /// <param name="trend">面倾向</param>
        /// <param name="dip">面倾角</param>
        /// <returns>返回圆弧圆心点</returns>
        public Circle CalArc(double trend, double dip)
        {
            TdPoint P1 = new TdPoint();
            TdPoint P2 = new TdPoint();
            TdPoint Ps = new TdPoint();
            P1.X = (600 + 480 * Math.Sin((trend + 90) * 3.14 / 180));
            P1.Y = (600 - 480 * Math.Cos((trend + 90) * 3.14 / 180));
            P2.X = (600 + 480 * Math.Sin((trend - 90) * 3.14 / 180));
            P2.Y = (600 - 480 * Math.Cos((trend - 90) * 3.14 / 180));
            Ps.X = (600 + 480 * Math.Sin((trend + 180) * 3.14 / 180) * (1 - dip / 90.0));
            Ps.Y = (600 - 480 * Math.Cos((trend + 180) * 3.14 / 180) * (1 - dip / 90.0));

            TdLine l1 = CalMidLine(P1, Ps);
            TdLine l2 = CalMidLine(P2, Ps);
            TdPoint Pc = CalIPoint(l1, l2);
            double dist = Distance(Pc, P1);

            Circle c = new Circle();
            c.X = Pc.X;
            c.Y = Pc.Y;
            c.R = dist;
            return c;
        }
        /// <summary>
        /// 将度数限制在0~360之间
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns>返回计算后的角度</returns>
        private double Lim360(double angle)
        {
            //while (angle > 360)
            //{
            //    angle -= 360;
            //}
            //while (angle < 0)
            //{
            //    angle += 360;
            //}
            return (angle + 720) % 360;
        }



    }


    /// <summary>
    /// 直线类
    /// </summary>
    public class TdLine
    {
        //Cy=Ax+B
        /// <summary>
        /// 斜率
        /// </summary>
        public double A;
        /// <summary>
        /// 与Y轴交点
        /// </summary>
        public double B;
        /// <summary>
        /// 是否有斜率，1有，0无
        /// 当没有斜率时，B为与X轴交点的相反数
        /// </summary>
        public int C;
    }
    /// <summary>
    /// 二维点类
    /// </summary>
    public class TdPoint
    {
        /// <summary>
        /// X坐标
        /// </summary>
        public double X;
        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y;

        public TdPoint() { }
        public TdPoint(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
    /// <summary>
    /// 圆类
    /// </summary>
    public class Circle
    {
        /// <summary>
        /// 圆心X坐标
        /// </summary>
        public double X;
        /// <summary>
        /// 圆心Y坐标
        /// </summary>
        public double Y;
        /// <summary>
        /// 半径
        /// </summary>
        public double R;
    }
    /// <summary>
    /// 两面交线类
    /// </summary>
    public class CrossTdLine
    {
        /// <summary>
        /// 倾向
        /// </summary>
        public double trend;
        /// <summary>
        /// 倾角
        /// </summary>
        public double dip;
    }
}