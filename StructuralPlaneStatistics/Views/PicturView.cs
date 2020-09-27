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
    public class PicturView : View
    {
        private Matrix matrix;
        private MainActivity a1;
        public PicturView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            a1 = context as MainActivity;
            Initialize();
        }

        public PicturView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
        }

        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            if (matrix != null)
            {
                canvas.DrawBitmap(App.bitmap, matrix, null);
            }
            base.OnDraw(canvas);
        }
        public void SetMatrix(Matrix matrix)
        {
            
            this.matrix = matrix;
        }
    }
}