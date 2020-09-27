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
    public class MessageBox
    {
        private static AlertDialog.Builder CreateDialog(Context ctx, string title, string message)
        {
            AlertDialog.Builder dlg = new AlertDialog.Builder(ctx);
            return dlg.SetTitle(title).SetMessage(message);
        }
        public static void Show(Context ctx, string title, string message)
        {
            CreateDialog(ctx, title, message).SetPositiveButton("确定", delegate { }).Show();
        }
        public static void Confirm(Context ctx, string title, string message, EventHandler<DialogClickEventArgs> okHandler, EventHandler<DialogClickEventArgs> cancelHandler)
        {
            CreateDialog(ctx, title, message).SetPositiveButton("确定", okHandler).SetNegativeButton("取消", cancelHandler).Show();
        }
        public static void ShowErrorMessage(Context ctx, Exception ex)
        {
            Show(ctx, "错误", ex.Message);
        }
    }
}