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
    public class Utils
    {
        /// <summary>
        /// 绑定Spinner
        /// </summary>
        /// <param name="context">上下文，即当前的Activity</param>
        /// <param name="Spinner1">下拉列表控件</param>
        /// <param name="ArrayId">下拉列表内容</param>
        public static void BindList(Context context, Spinner Spinner1, int ArrayId)
        {
            //ArrayAdapter adapter = ArrayAdapter.CreateFromResource(context, ArrayId, Android.Resource.Layout.SimpleSpinnerItem);
            ArrayAdapter adapter = ArrayAdapter.CreateFromResource(context, ArrayId, Resource.Layout.spitem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            Spinner1.Adapter = adapter;
        }
    }
}