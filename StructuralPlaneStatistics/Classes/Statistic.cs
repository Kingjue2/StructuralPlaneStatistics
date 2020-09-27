using System;
using System.Collections.Generic;
using System.Drawing;
using static StructuralPlaneStatistics.Classes.App;

namespace StructuralPlaneStatistics.Classes
{
    public class Statistic
    {
        /// <summary>
        /// 结构面分组
        /// </summary>
        /// <param name="structPlanes"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public List<StructPlane> Classification(List<StructPlane> structPlanes, PlaneGroup group)
        {
            List<StructPlane> list = new List<StructPlane>();
            foreach(StructPlane plane in structPlanes)
            {
                if (plane.Group == group)
                {
                    list.Add(plane);
                }
            }
            return list;
        }

        /// <summary>
        /// 结构面可见性计数
        /// </summary>
        /// <param name="structPlanes"></param>
        /// <param name="group"></param>
        /// <param name="n0"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        private void VisibilityCount(List<StructPlane> structPlanes, PlaneGroup group, ref int n0, ref int n1, ref int n2)
        {
            List<StructPlane> list = Classification(structPlanes, group);
            VisibilityCount(list, ref n0, ref n1, ref n2);
        }

        private void VisibilityCount(List<StructPlane> structPlanes, ref int n0, ref int n1, ref int n2)
        {
            //假设一端可见的不连续面条数为 n1,两端可见的不连续面条数为 n2，两端均不可见的不连续面条数为 n0
            n0 = 0;
            n1 = 0;
            n2 = 0;

            for (int i = 0; i < structPlanes.Count; i++)
            {
                switch (structPlanes[i].Visiblecount)
                {
                    case 0:
                        n0++;
                        break;
                    case 1:
                        n1++;
                        break;
                    case 2:
                        n2++;
                        break;
                }
            }
        }

        /// <summary>
        /// 计算平均迹长
        /// </summary>
        /// <param name="structPlanes">要统计的结构面</param>
        /// <param name="group">要统计的结构面分组</param>
        /// <returns>返回平均迹长</returns>
        public double GetMeanLength(List<StructPlane> structPlanes, PlaneGroup group)
        {
            int n0 = 0, n1 = 0, n2 = 0;
            VisibilityCount(structPlanes, group,ref n0,ref n1,ref n2);
            return ((double)n1 + 2 * (double)n0) * (Math.PI * App.InputWidth * App.InputHeight) / (2 * (double)structPlanes.Count) / (App.InputWidth + App.InputHeight);
        }
        /// <summary>
        /// 计算平均迹长
        /// </summary>
        /// <param name="structPlanes">要统计的结构面</param>
        /// <returns>返回平均迹长</returns>
        public double GetMeanLength(List<StructPlane> structPlanes)
        {
            int n0 = 0, n1 = 0, n2 = 0;
            VisibilityCount(structPlanes, ref n0, ref n1, ref n2);
            if (n1==0 && n0 == 0)
            {
                n1 = 1;
            }
            return ((double)n1 + 2 * (double)n0) * (Math.PI * (double)App.InputWidth * (double)App.InputHeight) / (2 * (double)structPlanes.Count) / (double)(App.InputWidth + App.InputHeight);
        }
        /// <summary>
        /// 计算连通率
        /// </summary>
        /// <param name="structPlanes">要统计的结构面</param>
        /// <param name="group">要统计的结构面分组</param>
        /// <returns>返回连通率</returns>
        public double GetConnectivity(List<StructPlane> structPlanes, PlaneGroup group)
        {
            int n0 = 0, n1 = 0, n2 = 0;
            VisibilityCount(structPlanes, group, ref n0, ref n1, ref n2);
            return ((double)n1 + 2 * (double)n0) / (2 * (double)structPlanes.Count + (double)n2);
        }
        /// <summary>
        /// 计算连通率
        /// </summary>
        /// <param name="structPlanes">要统计的结构面</param>
        /// <returns>返回连通率</returns>
        public double GetConnectivity(List<StructPlane> structPlanes)
        {
            int n0 = 0, n1 = 0, n2 = 0;
            VisibilityCount(structPlanes, ref n0, ref n1, ref n2);
            return ((double)n1 + 2 * (double)n0) / (2 * (double)structPlanes.Count + (double)n2);
        }
        /// <summary>
        /// 计算平均间距
        /// </summary>
        /// <param name="structPlanes">要统计的结构面</param>
        /// <param name="group">要统计的结构面分组</param>
        /// <returns>返回平均间距</returns>
        public double GetMeanSpace(List<StructPlane> structPlanes, PlaneGroup group)
        {
            List<StructPlane> list = Classification(structPlanes, group);
            return GetMeanSpace(list);
        }
        /// <summary>
        /// 计算平均间距
        /// </summary>
        /// <param name="structPlanes">要统计的结构面</param>
        /// <returns>返回平均间距</returns>
        public double GetMeanSpace(List<StructPlane> structPlanes)
        {
            double minspace = 0, space = 0;
            float x1, x2, y1, y2;
            //横向
            x1 = App.left;
            x2 = App.right;
            y1 = (App.top + App.bottom) / 2;
            y2 = (App.top + App.bottom) / 2;
            space = App.InputWidth / (float)PlaneCount(structPlanes, x1, y1, x2, y2);
            minspace = space;

            //纵向
            x1 = (App.left + App.right) / 2;
            x2 = (App.left + App.right) / 2;
            y1 = App.top;
            y2 = App.bottom;
            space = App.InputHeight / (float)PlaneCount(structPlanes, x1, y1, x2, y2);
            if (space!= 0)
            {
                minspace = space < minspace ? space : minspace;
            }

            //斜 K+
            x1 = App.left;
            x2 = App.right;
            y1 = App.bottom;
            y2 = App.top;
            space = Math.Sqrt(App.InputHeight * App.InputHeight + App.InputWidth * App.InputWidth) / (double)PlaneCount(structPlanes, x1, y1, x2, y2);
            if (space != 0)
            {
                minspace = space < minspace ? space : minspace;
            }

            //斜 K-
            x1 = App.left;
            x2 = App.right;
            y1 = App.top;
            y2 = App.bottom;
            space = Math.Sqrt(App.InputHeight * App.InputHeight + App.InputWidth * App.InputWidth) / (double)PlaneCount(structPlanes, x1, y1, x2, y2);
            if (space != 0)
            {
                minspace = space < minspace ? space : minspace;
            }
            return minspace;
            ////先计算横向测线
            //int planeCount = PlaneCountHorizontal(structPlanes);

            //if (planeCount == 0)
            //{
            //    //没有相交，则计算竖向测线
            //    planeCount = PlaneCountVertical(structPlanes);
            //    if (planeCount == 0)
            //    {
            //        //竖线也没有相交，返回0
            //        return 0;
            //    }
            //    else
            //    {
            //        return App.InputHeight / (float)planeCount;
            //    }
            //}
            //else
            //{
            //    return App.InputWidth / (float)planeCount;
            //}

        }


        /// <summary>
        /// 计算横向三条测线相交结构面数量
        /// </summary>
        /// <param name="structPlanes"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        private int PlaneCountHorizontal(List<StructPlane> structPlanes)
        {
            //设置三条测线，分别为
            //x1,y0,x2,y0 中间横测线
            //x1,y1,x2,y1 上部横测线
            //x1,y2,x2,y2 底部横测线
            float x1, x2, y0, y1, y2;
            x1 = App.left;
            x2 = App.right;
            y0 = (App.top + App.bottom) / 2;
            y1 = (App.top + y0) / 2;
            y2 = (App.bottom + y0) / 2;

            //三条测线与结构面的交点数
            int Count0 = 0;
            int Count1 = 0;
            int Count2 = 0;

            PointF sp1, sp2, cx1p1, cx1p2, cx2p1, cx2p2, cx0p1, cx0p2, point;
            for (int i = 0; i < structPlanes.Count; i++)
            {
       
                sp1 = new PointF(structPlanes[i].X1, structPlanes[i].Y1);
                sp2 = new PointF(structPlanes[i].X2, structPlanes[i].Y2);
                cx0p1 = new PointF(x1, y0);
                cx0p2 = new PointF(x2, y0);
                cx1p1 = new PointF(x1, y1);
                cx1p2 = new PointF(x2, y1);
                cx2p1 = new PointF(x1, y2);
                cx2p2 = new PointF(x2, y2);
                point = new PointF();
                if (Intersection(sp1, sp2, cx0p1, cx0p2, ref point) > 0) Count0++;
                if (Intersection(sp1, sp2, cx1p1, cx1p2, ref point) > 0) Count1++;
                if (Intersection(sp1, sp2, cx2p1, cx2p2, ref point) > 0) Count2++;

            }
            return Count0 + Count1 + Count2;
        }
        /// <summary>
        /// 计算竖向三条测线相交结构面数量
        /// </summary>
        /// <param name="structPlanes"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        private int PlaneCountVertical(List<StructPlane> structPlanes)
        {
            //设置三条测线，分别为
            //x0,y1,x0,y2 中间竖测线
            //x1,y1,x1,y2 左侧竖测线
            //x2,y1,x2,y2 右侧竖测线

            float x1, x2, x0, y1, y2;
            y1 = App.top;
            y2 = App.bottom;
            x0 = (App.left + App.right) / 2;
            x1 = (App.left + x0) / 2;
            x2 = (App.right + x0) / 2;

            //三条测线与结构面的交点数
            int Count0 = 0;
            int Count1 = 0;
            int Count2 = 0;

            PointF sp1, sp2, cx1p1, cx1p2, cx2p1, cx2p2, cx0p1, cx0p2, point;
            for (int i = 0; i < structPlanes.Count; i++)
            {
                sp1 = new PointF(structPlanes[i].X1, structPlanes[i].Y1);
                sp2 = new PointF(structPlanes[i].X2, structPlanes[i].Y2);
                cx0p1 = new PointF(x0, y1);
                cx0p2 = new PointF(x0, y2);
                cx1p1 = new PointF(x1, y1);
                cx1p2 = new PointF(x1, y2);
                cx2p1 = new PointF(x2, y1);
                cx2p2 = new PointF(x2, y2);
                point = new PointF();
                if (Intersection(sp1, sp2, cx0p1, cx0p2, ref point) > 0) Count0++;
                if (Intersection(sp1, sp2, cx1p1, cx1p2, ref point) > 0) Count1++;
                if (Intersection(sp1, sp2, cx2p1, cx2p2, ref point) > 0) Count2++;
          
            }
            return Count0 + Count1 + Count2;
        }

        /// <summary>
        /// 计算与测线相交迹线的数量
        /// </summary>
        /// <param name="structPlanes"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        private int PlaneCount(List<StructPlane> structPlanes,float x1,float y1,float x2, float y2)
        {
            int Count = 0;
            PointF sp1, sp2, cxp1, cxp2, point;
            for (int i = 0; i < structPlanes.Count; i++)
            {
                sp1 = new PointF(structPlanes[i].X1, structPlanes[i].Y1);
                sp2 = new PointF(structPlanes[i].X2, structPlanes[i].Y2);
                cxp1 = new PointF(x1, y1);
                cxp2 = new PointF(x2, y2);
                point = new PointF();
                if (Intersection(sp1, sp2, cxp1, cxp2, ref point) > 0) Count++;
            }
            return Count;
        }

        //原理
        //https://www.cnblogs.com/tuyang1129/p/9390376.html

        //判定两线段位置关系，并求出交点(如果存在)。返回值列举如下：
        //[有重合] 完全重合(6)，1个端点重合且共线(5)，部分重合(4)
        //[无重合] 两端点相交(3)，交于线上(2)，正交(1)，无交(0)，参数错误(-1)
        public int Intersection(PointF p1, PointF p2, PointF p3, PointF p4, ref PointF point)
        {
            //保证参数p1!=p2，p3!=p4
            if (p1 == p2 || p3 == p4)
            {
                return -1; //返回-1代表至少有一条线段首尾重合，不能构成线段
            }
            //为方便运算，保证各线段的起点在前，终点在后。
            if (IsBigger(p1, p2))
            {
                PointF pTemp = p1;
                p1 = p2;
                p2 = pTemp;
                // swap(p1, p2);
            }
            if (IsBigger(p3, p4))
            {
                PointF pTemp = p3;
                p3 = p4;
                p4 = pTemp;
                //swap(p3, p4);
            }
            //判定两线段是否完全重合
            if (p1 == p3 && p2 == p4)
            {
                return 6;
            }
            //求出两线段构成的向量
            PointF v1 = new PointF(p2.X - p1.X, p2.Y - p1.Y), v2 = new PointF(p4.X - p3.X, p4.Y - p3.Y);
            //求两向量外积，平行时外积为0
            float Corss = Product(v1, v2);
            //如果起点重合
            if (Equal(p1, p3))
            {
                point = p1;
                //起点重合且共线(平行)返回5；不平行则交于端点，返回3
                return (Equal(Corss, 0) ? 5 : 3);
            }
            //如果终点重合
            if (Equal(p2, p4))
            {
                point = p2;
                //终点重合且共线(平行)返回5；不平行则交于端点，返回3
                return (Equal(Corss, 0) ? 5 : 3);
            }
            //如果两线端首尾相连
            if (Equal(p1, p4))
            {
                point = p1;
                return 3;
            }
            if (Equal(p2, p3))
            {
                point = p2;
                return 3;
            }//经过以上判断，首尾点相重的情况都被排除了
             //将线段按起点坐标排序。若线段1的起点较大，则将两线段交换
            if (IsBigger(p1, p3))
            {
                PointF pTemp = p1;
                p1 = p3;
                p3 = pTemp;

                pTemp = p2;
                p2 = p4;
                p4 = pTemp;

                pTemp = v1;
                v1 = v2;
                v2 = pTemp;
                //swap(p1, p3);
                //swap(p2, p4);
                //更新原先计算的向量及其外积
                //swap(v1, v2);
                Corss = Product(v1, v2);
            }
            //处理两线段平行的情况
            if (Equal(Corss, 0))
            {
                //做向量v1(p1, p2)和vs(p1,p3)的外积，判定是否共线
                PointF vs = new PointF(p3.X - p1.X, p3.Y - p1.Y);
                //外积为0则两平行线段共线，下面判定是否有重合部分
                if (Equal(Product(v1, vs), 0))
                {
                    //前一条线的终点大于后一条线的起点，则判定存在重合
                    if (IsBigger(p2, p3))
                    {
                        point = p3;
                        return 4;//返回值4代表线段部分重合
                    }
                }//若三点不共线，则这两条平行线段必不共线。
                 //不共线或共线但无重合的平行线均无交点
                return 0;
            }//以下为不平行的情况，先进行快速排斥试验
             //x坐标已有序，可直接比较。y坐标要先求两线段的最大和最小值
            float ymax1 = p1.Y, ymin1 = p2.Y, ymax2 = p3.Y, ymin2 = p4.Y;
            if (ymax1 < ymin1)
            {
                float fTemp = ymax1;
                ymax1 = ymin1;
                ymin1 = fTemp;
                //swap(ymax1, ymin1);
            }
            if (ymax2 < ymin2)
            {
                //swap(ymax2, ymin2);
                float fTemp = ymax2;
                ymax2 = ymin2;
                ymin2 = fTemp;
            }
            //如果以两线段为对角线的矩形不相交，则无交点
            if (p1.X > p4.X || p2.X < p3.X || ymax1 < ymin2 || ymin1 > ymax2)
            {
                return 0;
            }//下面进行跨立试验
            PointF vs1 = new PointF(p1.X - p3.X, p1.Y - p3.Y), vs2 = new PointF(p2.X - p3.X, p2.Y - p3.Y);
            PointF vt1 = new PointF(p3.X - p1.X, p3.Y - p1.Y), vt2 = new PointF(p4.X - p1.X, p4.Y - p1.Y);
            float s1v2, s2v2, t1v1, t2v1;
            //根据外积结果判定否交于线上
            if (Equal(s1v2 = Product(vs1, v2), 0) && IsBigger(p4, p1) && IsBigger(p1, p3))
            {
                point = p1;
                return 2;
            }
            if (Equal(s2v2 = Product(vs2, v2), 0) && IsBigger(p4, p2) && IsBigger(p2, p3))
            {
                point = p2;
                return 2;
            }
            if (Equal(t1v1 = Product(vt1, v1), 0) && IsBigger(p2, p3) && IsBigger(p3, p1))
            {
                point = p3;
                return 2;
            }
            if (Equal(t2v1 = Product(vt2, v1), 0) && IsBigger(p2, p4) && IsBigger(p4, p1))
            {
                point = p4;
                return 2;
            }//未交于线上，则判定是否相交
            if (s1v2 * s2v2 > 0 || t1v1 * t2v1 > 0)
            {
                return 0;
            }
            //https://blog.csdn.net/miao0967020148/article/details/48088871
            //以下为相交的情况，算法详见文档
            //计算二阶行列式的两个常数项
            float ConA = p1.X * v1.Y - p1.Y * v1.X;
            float ConB = p3.X * v2.Y - p3.Y * v2.X;
            //计算行列式D1和D2的值，除以系数行列式的值，得到交点坐标
            point.X = ((ConB * v1.X - ConA * v2.X) / Corss);
            point.Y = ((ConB * v1.Y - ConA * v2.Y) / Corss);
            //正交返回1
            return 1;
        }

        bool Equal(float f1, float f2)
        {
            return (Math.Abs(f1 - f2) < 1f);
        }
        bool IsBigger(PointF p1, PointF p2)////比较两点坐标大小，先比较x坐标，若相同则比较y坐标
        {
            return (p1.X > p2.X || (Equal(p1.X, p2.X) && p1.Y > p2.Y));
        }
        bool Equal(PointF p1, PointF p2)////判断两点是否相等
        {
            return (Equal(p1.X, p2.X) && Equal(p1.Y, p2.Y));
        }
        float Product(PointF p1, PointF p2)////计算两向量外积
        {
            return (p1.X * p2.Y - p1.Y * p2.X);
        }


        /// <summary>
        /// 计算平均倾向
        /// </summary>
        /// <param name="structPlanes"></param>
        /// <returns></returns>
        public int GetMeanDipDirection(List<StructPlane> structPlanes)
        {
            int count = 0;
            foreach(StructPlane plane in structPlanes)
            {
                count = count + plane.DipDirection;
            }
            return (int)((double)count / structPlanes.Count + 0.5);
        }
        /// <summary>
        /// 计算平均倾角
        /// </summary>
        /// <param name="structPlanes"></param>
        /// <returns></returns>
        public int GetMeanDipAngle(List<StructPlane> structPlanes)
        {
            int count = 0;
            foreach (StructPlane plane in structPlanes)
            {
                count = count + plane.DipAngle;
            }
            return (int)((double)count / structPlanes.Count + 0.5);
        }

    }

}