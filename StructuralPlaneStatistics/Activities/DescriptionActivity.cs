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
using StructuralPlaneStatistics.Classes;

namespace StructuralPlaneStatistics.Activities
{
    [Activity(Label = "DescriptionActivity")]
    public class DescriptionActivity : Activity
    {
        Spinner spn_group, spn_rough, spn_opening, spn_cementation, spn_groundwater, spn_waviness;
        EditText etxt_dipdirection, etxt_dipangle, etxt_filler;
        Button btn_descripOK, btn_measure;
        int index = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Description);

            #region 绑定控件
            spn_group = FindViewById<Spinner>(Resource.Id.spn_group);
            spn_waviness = FindViewById<Spinner>(Resource.Id.spn_waviness);
            spn_rough = FindViewById<Spinner>(Resource.Id.spn_rough);
            spn_opening = FindViewById<Spinner>(Resource.Id.spn_opening);
            spn_cementation = FindViewById<Spinner>(Resource.Id.spn_cementation);
            spn_groundwater = FindViewById<Spinner>(Resource.Id.spn_groundwater);
            etxt_dipdirection = FindViewById<EditText>(Resource.Id.etxt_dipdirection);
            etxt_dipangle = FindViewById<EditText>(Resource.Id.etxt_dipangle);
            etxt_filler = FindViewById<EditText>(Resource.Id.etxt_filler);
            btn_descripOK = FindViewById<Button>(Resource.Id.btn_descripOK);
            btn_measure = FindViewById<Button>(Resource.Id.btn_measure);

            Utils.BindList(this, spn_group, Resource.Array.arr_group);
            Utils.BindList(this, spn_waviness, Resource.Array.arr_waviness);
            Utils.BindList(this, spn_rough, Resource.Array.arr_rough);
            Utils.BindList(this, spn_opening, Resource.Array.arr_opening);
            Utils.BindList(this, spn_cementation, Resource.Array.arr_cementation);
            Utils.BindList(this, spn_groundwater, Resource.Array.arr_groundwater);

            btn_measure.Click += Btn_measure_Click;
            btn_descripOK.Click += Btn_descripOK_Click;
            #endregion

            try
            {
                Bundle b = this.Intent.Extras;
                index = b.GetInt("position");
            }
            catch
            {
                MessageBox.Show(this, "非法调用", "非法调用");
                this.Finish();
            }

        }

        private void Btn_descripOK_Click(object sender, EventArgs e)
        {
            App.planes[index].DipDirection = int.Parse(etxt_dipdirection.Text);
            App.planes[index].DipAngle = int.Parse(etxt_dipangle.Text);
            App.planes[index].Group = (PlaneGroup)spn_group.SelectedItemPosition;
            App.planes[index].Waviness = spn_waviness.SelectedItem.ToString();
            App.planes[index].Roughness = spn_rough.SelectedItem.ToString();
            App.planes[index].Opening = spn_opening.SelectedItem.ToString();
            App.planes[index].Filler = etxt_filler.Text;
            App.planes[index].Cementation = spn_cementation.SelectedItem.ToString();
            App.planes[index].Groundwater = spn_groundwater.SelectedItem.ToString();
            this.Finish();
        }

        private void Btn_measure_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(AutoDipActivity));
            Bundle b = new Bundle();
            b.PutInt("show", 1);
            intent.AddFlags(ActivityFlags.TaskOnHome);
            intent.PutExtras(b);
            StartActivityForResult(intent, App.DipMeasureRequestCode);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == App.DipMeasureRequestCode)//产状测量
            {
                if (resultCode == Android.App.Result.Ok && null != data)
                {
                    //int position = (int)data.Extras.Get("position");
                    int dipdirection = (int)data.Extras.Get("dipdirection");
                    int dipangle = (int)data.Extras.Get("dipangle");
                    etxt_dipdirection.Text = dipdirection.ToString();
                    etxt_dipangle.Text = dipangle.ToString();
                    //Toast.MakeText(this, position.ToString() + ";" + dipdirection.ToString() + ";" + dipangle.ToString() + splanes[splanes.Count-1].Strike.ToString(), ToastLength.Long);
                }
            }

        }
    }
}