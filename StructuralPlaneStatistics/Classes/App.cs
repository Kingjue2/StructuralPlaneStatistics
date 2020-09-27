using Android.Graphics;
using Java.IO;
using System.Collections.Generic;

namespace StructuralPlaneStatistics.Classes
{
    public class App
    {
        public static int CameraRequestCode = 233;
        public static int StorageRequestCode = 234;
        public static int DipMeasureRequestCode = 235;
        public static Java.IO.File _file;
        public static Java.IO.File _dir;
        public static Bitmap bitmap;
        /// <summary>
        /// 用于绘图的控件宽度，像素
        /// </summary>
        public static int LayoutWidth;
        /// <summary>
        /// 用于绘图的控件高度，像素
        /// </summary>
        public static int LayoutHeight;
        /// <summary>
        /// 输入的测窗宽度，米
        /// </summary>
        public static float InputWidth;
        /// <summary>
        /// 输入的测窗高度，米
        /// </summary>
        public static float InputHeight;
        /// <summary>
        /// 绘图时的测窗宽度，像素
        /// </summary>
        public static int DrawWidth;
        /// <summary>
        /// 绘图时的测窗宽度，像素
        /// </summary>
        public static int DrawHeight;
        /// <summary>
        /// 绘图时测窗与边缘的偏移距离，像素
        /// </summary>
        public static int DrawOffset = 50;
        /// <summary>
        /// 结构面
        /// </summary>
        public static List<StructPlane> planes = new List<StructPlane>();
        /// <summary>
        /// 结构面编辑状态
        /// </summary>
        public enum PlaneState { Add, Modify }
        /// <summary>
        /// 结构面编辑状态
        /// </summary>
        public static PlaneState planeState;
        /// <summary>
        /// 矩形框左
        /// </summary>
        public static float left;
        /// <summary>
        /// 矩形框右
        /// </summary>
        public static float right;
        /// <summary>
        /// 矩形框顶
        /// </summary>
        public static float top;
        /// <summary>
        /// 矩形框底
        /// </summary>
        public static float bottom;

    }
}