using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using StructuralPlaneStatistics.Views;

namespace StructuralPlaneStatistics.Activities
{
    [Activity(Label = "AutoDipActivity")]
    public class AutoDipActivity : Activity, Android.Hardware.ISensorEventListener
    {
        TextView tv0;
        TextView tv1;
        TextView tv2;
        Button btn_DipOk;
        SensorManager sensManager;
        Sensor aSensor, mSensor, oSensor;//加速度传感器和磁场传感器，方向传感器
        float[] accelerometerValues = new float[3];
        float[] magneticFieldValues = new float[3];
        float[] orientaionValues = new float[3];
        //double phi, thita, psi;
        private StereographicView sView;

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {

        }

        public void OnSensorChanged(SensorEvent e)
        {
            //if (e.Sensor.Type == SensorType.Accelerometer)
            //{
            //    var aa = e.Values;
            //    accelerometerValues[0] = aa[0];
            //    accelerometerValues[1] = aa[1];
            //    accelerometerValues[2] = aa[2];
            //}
            //if (e.Sensor.Type == SensorType.MagneticField)
            //{
            //    var mm = e.Values;
            //    magneticFieldValues[0] = mm[0];
            //    magneticFieldValues[1] = mm[1];
            //    magneticFieldValues[2] = mm[2];
            //}
            if (e.Sensor.Type == SensorType.Orientation)
            {
                orientaionValues[0] = e.Values[0];
                orientaionValues[1] = e.Values[1];
                orientaionValues[2] = e.Values[2];
            }

            CalTrendDip();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AutoDip);

            tv0 = FindViewById<TextView>(Resource.Id.tv0);
            tv1 = FindViewById<TextView>(Resource.Id.tv1);
            tv2 = FindViewById<TextView>(Resource.Id.tv2);
            btn_DipOk = FindViewById<Button>(Resource.Id.btn_dipok);
            btn_DipOk.Click += Btn_DipOk_Click;
            try
            {
                Bundle b = this.Intent.Extras;
                if (b.GetInt("show") == 1)
                {
                    btn_DipOk.Visibility = ViewStates.Visible;
                }
            }
            catch
            {
                btn_DipOk.Visibility = ViewStates.Invisible;
            }

            String service_name = Context.SensorService;
            sensManager = (SensorManager)GetSystemService(service_name);

            //aSensor = sensManager.GetDefaultSensor(SensorType.Accelerometer);
            //sensManager.RegisterListener(this, aSensor, SensorDelay.Ui);
            //mSensor = sensManager.GetDefaultSensor(SensorType.MagneticField);
            //sensManager.RegisterListener(this, mSensor, SensorDelay.Ui);
            oSensor = sensManager.GetDefaultSensor(SensorType.Orientation);
            sensManager.RegisterListener(this, oSensor, SensorDelay.Ui);

            sView = new StereographicView(this, null);
            LinearLayout layout = FindViewById<LinearLayout>(Resource.Id.ll);
            var Params = new Android.Widget.LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
            layout.AddView(sView, Params);

        }

        private void Btn_DipOk_Click(object sender, EventArgs e)
        {
            Bundle b = this.Intent.Extras;
            Intent aintent = new Intent();
            aintent.PutExtra("dipdirection", int.Parse(tv0.Text));
            aintent.PutExtra("dipangle", int.Parse(tv1.Text));
            //aintent.PutExtra("position", b.GetInt("position"));
            SetResult(Android.App.Result.Ok, aintent);
            this.Finish();
        }

        void CalTrendDip()
        {
            double dipdirection = 0;
            double dipangle = 0;

            double sino1 = Math.Sin(orientaionValues[1] * Math.PI / 180);
            double sino2 = Math.Sin(orientaionValues[2] * Math.PI / 180);
            dipangle = Math.Asin(Math.Sqrt(sino1 * sino1 + sino2 * sino2)) * 180 / Math.PI;
            double epsilon = Math.Acos(sino1 / Math.Sqrt(sino1 * sino1 + sino2 * sino2)) * 180 / Math.PI;

            if (orientaionValues[2] > 0)
            {
                dipdirection = (orientaionValues[0] - epsilon + 720) % 360;
            }
            else
            {
                dipdirection = (orientaionValues[0] + epsilon + 720) % 360;
            }

            sView.updateDirection((float)dipdirection, (float)dipangle);
            RunOnUiThread(delegate
            {
                tv0.Text = dipdirection.ToString("0");
                tv1.Text = dipangle.ToString("0");
            });

        }

        protected override void OnDestroy()
        {
            //sensManager.UnregisterListener(this, aSensor);
            //sensManager.UnregisterListener(this, mSensor);
            sensManager.UnregisterListener(this, oSensor);
            base.OnDestroy();
        }
        protected override void OnPause()
        {
            sensManager.UnregisterListener(this);
            base.OnPause();
        }
        protected override void OnResume()
        {
            base.OnResume();
            //sensManager.RegisterListener(this, aSensor, SensorDelay.Ui);
            //sensManager.RegisterListener(this, mSensor, SensorDelay.Ui);
            sensManager.RegisterListener(this, oSensor, SensorDelay.Ui);
        }
    }
}