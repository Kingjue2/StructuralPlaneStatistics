using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace StructuralPlaneStatistics.Classes
{
    public class SaveResult
    {
        /// <summary>
        /// 将bitmap保存为文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="filename">文件名</param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static bool SavePic(string path, string filename, Bitmap bitmap)
        {
            try
            {
                string output_filename = path + $"/{filename}.jpg";
                FileStream output = new FileStream(output_filename, FileMode.Create);
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, output);
                //MessageBox.Show(context, "成功", $"{filename}保存成功！");
                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// 保存统计结果
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static bool SaveStatisticalResult(string path)
        {
            //创建文件
            string outtextname = path + "/7 - Statistical Data.csv";
            StreamWriter SW;
            SW = File.CreateText(outtextname);
            SW.WriteLine("Index,Group,Count,Mean Dip Direction,Mean Dip Angle,Mean Trace Length,Mean Space,Connectivity");
            try
            {
                Statistic statistic = new Statistic();

                double meanlength = 0;
                double connectivity = 0;
                double meanspace = 0;
                int meandipdirection = 0;
                int meandipangle = 0;
                List<StructPlane> planes = new List<StructPlane>();
                int index = 1;
                foreach (PlaneGroup group in Enum.GetValues(typeof(PlaneGroup)))
                {
                    planes = statistic.Classification(App.planes, group);
                    if(planes.Count > 0)
                    {
                        //index = 1;
                        meanlength = statistic.GetMeanLength(planes);
                        connectivity = statistic.GetConnectivity(planes);
                        meanspace = statistic.GetMeanSpace(planes);
                        meandipdirection = statistic.GetMeanDipDirection(planes);
                        meandipangle = statistic.GetMeanDipAngle(planes);

                        SW.WriteLine($"{index},{group.ToString()},{planes.Count},{meandipdirection},{meandipangle},{meanlength},{meanspace},{connectivity}");
                        index++;
                    }

                }


                //string report = $"测窗尺寸{App.InputWidth}x{App.InputHeight}m，共测量结构面{App.planes.Count}个，平均迹长{meanlength.ToString("0.000")}m，平均间距{meanspace.ToString("0.000")}m，连通率{connectivity.ToString("0.000")}。";

                //SW.Write(report);
                //关闭文件
                SW.Close();
                return true;
            }
            catch
            {
                //关闭文件
                SW.Close();
                return false;
            }
        }

        /// <summary>
        /// 保存地质数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static bool SaveGeologicalData(string path)
        {
            try
            {
                string outtextname = path + "/6 - Geological Data.csv";
                StreamWriter SW;
                SW = File.CreateText(outtextname);

                SW.WriteLine("Index,Group,Dip Direction,Dip Angle,Strike,Waviness,Roughness,Opening,Filler,Cementation,Groundwater");
                for(int i = 0; i < App.planes.Count; i++)
                {
                    SW.WriteLine($"{i + 1},{App.planes[i].Group},{App.planes[i].DipDirection},{App.planes[i].DipAngle},{App.planes[i].Strike},{App.planes[i].Waviness},{App.planes[i].Roughness},{App.planes[i].Opening},{App.planes[i].Filler},{App.planes[i].Cementation},{App.planes[i].Groundwater}");
                }
                SW.Close();
                return true;
            }
            catch
            {
                return false;
            }

        }
        /// <summary>
        /// 保存几何数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static bool SaveGeometricalData(string path)
        {
            try
            {
                string outtextname = path + "/5 - Geometrical Data.csv";
                StreamWriter SW;
                SW = File.CreateText(outtextname);
                SW.WriteLine("Index,Group,Visible Trace Length,Endpoint 1 X,Endpoint 1 Y,Endpoint 2 X,Endpoint 2 Y,Visible EndPoint");

                for (int i = 0; i < App.planes.Count; i++)
                {
                    SW.WriteLine($"{i + 1},{App.planes[i].Group},{App.planes[i].Lengh},{App.planes[i].P1X},{App.planes[i].P1Y},{App.planes[i].P2X},{App.planes[i].P2Y},{App.planes[i].Visiblecount}");
                }
                SW.Close();
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}