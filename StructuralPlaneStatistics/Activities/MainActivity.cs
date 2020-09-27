using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using StructuralPlaneStatistics.Views;
using Android.Content;
using Android.Provider;
using System.Collections.Generic;
using Android.Content.PM;
using Android.Support.V4.Content;
using Android;
using Android.Support.V4.App;
using System;
using StructuralPlaneStatistics.Classes;
using Java.IO;
using Uri = Android.Net.Uri;
using Android.Graphics;
using System.IO;

namespace StructuralPlaneStatistics.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public RelativeLayout rl;
        public ImageView imageView;
        public Button btn_cancel, btn_line, btn_rect, btn_img, btn_reform, btn_modify, btn_rose, btn_statistic;
        RectView rectView;
        StructPlaneView planeView;
        PicturView picturView;
        RoseView roseView;
        StatisticView statisticView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);//设置无标题
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);//设置全屏
            this.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape; //横屏

            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            btn_line = FindViewById<Button>(Resource.Id.btn_line);
            btn_rect = FindViewById<Button>(Resource.Id.btn_rect);
            btn_img = FindViewById<Button>(Resource.Id.btn_img);
            btn_reform = FindViewById<Button>(Resource.Id.btn_reform);
            btn_modify = FindViewById<Button>(Resource.Id.btn_modify);
            btn_cancel = FindViewById<Button>(Resource.Id.btn_cancel);
            btn_rose = FindViewById<Button>(Resource.Id.btn_rose);
            btn_statistic = FindViewById<Button>(Resource.Id.btn_statistic);

            rl = FindViewById<RelativeLayout>(Resource.Id.relativeLayout1);
            imageView = FindViewById<ImageView>(Resource.Id.imageView1);

            rectView = new RectView(this, null);
            planeView = new StructPlaneView(this, null);
            picturView = new PicturView(this, null);
            roseView = new RoseView(this, null);
            //statisticView = new StatisticView(this, null);




            CreateDirectoryForPictures();

            btn_rect.Click += Btn_rect_Click;
            btn_img.Click += Btn_img_Click;
            btn_reform.Click += Btn_reform_Click;
            btn_modify.Click += Btn_modify_Click;
            btn_line.Click += Btn_line_Click;
            btn_cancel.Click += Btn_cancel_Click;
            btn_rose.Click += Btn_rose_Click;
            btn_statistic.Click += Btn_statistic_Click;
        }
        public void AddPlane(int index)
        {
            LayoutInflater inflater = LayoutInflater.From(this);
            View view = inflater.Inflate(Resource.Layout.Input, null, false);
            Android.App.AlertDialog.Builder dlg = new Android.App.AlertDialog.Builder(this);
            dlg.SetTitle("输入测窗尺寸").SetMessage("输入测窗尺寸").SetView(view).SetPositiveButton("OK", delegate
            {
                int dipdirection = int.Parse(view.FindViewById<EditText>(Resource.Id.et_length).Text);
                int dipangle = int.Parse(view.FindViewById<EditText>(Resource.Id.et_width).Text);
                App.planes[index].DipDirection = dipdirection;
                App.planes[index].DipAngle = dipangle;
            }).SetNegativeButton("Cancel", delegate { }).Show();
        }


        private void Btn_statistic_Click(object sender, EventArgs e)
        {
            SetButtonEnabled();
            btn_statistic.Enabled = false;

            EditText et = new EditText(this);
            Android.App.AlertDialog.Builder dlg = new Android.App.AlertDialog.Builder(this);
            dlg.SetTitle("保存").SetMessage("请输入测窗名称").SetView(et).SetPositiveButton("确定", delegate { Save(et.Text); }).SetNegativeButton("取消", delegate { }).Show();

        }

        private void Save (string windowname)
        {
            if (windowname != "")
            {
                try
                {
                    Java.IO.File path = new Java.IO.File(App._dir, windowname);
                    if (!path.Exists())
                    {
                        path.Mkdirs();
                    }
                    else
                    {
                        MessageBox.Confirm(this, "警告", "该文件夹已存在，是否覆盖？", delegate
                        {
                            path.Mkdirs();
                        }, delegate { });
                    }

                    SavePics(path.ToString());

                    SaveResult.SaveGeologicalData(path.ToString());

                    SaveResult.SaveGeometricalData(path.ToString());

                    SaveResult.SaveStatisticalResult(path.ToString());

                    MessageBox.Show(this, "成功", "保存成功");
                }
                catch
                {
                    MessageBox.Show(this, "失败", "保存失败");

                }
            }
            else
            {
                MessageBox.Show(this, "注意", "测窗名称不能为空！");
            }
        }

        private void SavePics(string path)
        {
            Bitmap bitmap = Bitmap.CreateBitmap(rl.Width, rl.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            picturView.Draw(canvas);
            if (SaveResult.SavePic(path.ToString(), "1 - Transformed Photo", bitmap))
            {
                planeView.Draw(canvas);
                SaveResult.SavePic(path.ToString(), "2 - Photo whith Sketch", bitmap);
            }

            bitmap = Bitmap.CreateBitmap(rl.Width, rl.Height, Bitmap.Config.Argb8888);
            canvas = new Canvas(bitmap);
            canvas.DrawColor(Color.White);
            planeView.Draw(canvas);
            SaveResult.SavePic(path.ToString(), "3 - Sketch", bitmap);

            bitmap = Bitmap.CreateBitmap(rl.Width, rl.Height, Bitmap.Config.Argb8888);
            canvas = new Canvas(bitmap);
            roseView.Draw(canvas);
            SaveResult.SavePic(path.ToString(), "4 - Rose Diagram", bitmap);
        }

        private void Btn_rose_Click(object sender, EventArgs e)
        {
            SetButtonEnabled();
            btn_rose.Enabled = false;
            try { rl.RemoveView(roseView); } catch { }
            rl.AddView(roseView);
            //roseView.SetData();
            rl.Invalidate();
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            //SetButtonEnabled();
            if (App.planes.Count > 0)
            {
                App.planes.Remove(App.planes[App.planes.Count - 1]);
            }
            planeView.Invalidate();
        }

        private void Btn_line_Click(object sender, EventArgs e)
        {
            SetButtonEnabled();
            try { rl.RemoveView(rectView); } catch { }
            try { rl.RemoveView(planeView); } catch { }
            //try { rl.RemoveView(picturView); } catch { }
            rl.AddView(planeView);
            App.planeState = App.PlaneState.Add;
            btn_line.Enabled = false;
        }

        private void Btn_modify_Click(object sender, EventArgs e)
        {
            SetButtonEnabled();
            try { rl.RemoveView(rectView); } catch { }
            try { rl.RemoveView(planeView); } catch { }
            //try { rl.RemoveView(picturView); } catch { }
            rl.AddView(planeView);
            App.planeState = App.PlaneState.Modify;
            btn_modify.Enabled = false;
        }

        private void Btn_reform_Click(object sender, EventArgs e)
        {
            SetButtonEnabled();
            try { rl.RemoveView(rectView); } catch { }
            try { rl.RemoveView(planeView); } catch { }
            try { rl.RemoveView(picturView); } catch { }
            float length, width;

            LayoutInflater inflater = LayoutInflater.From(this);
            View view = inflater.Inflate(Resource.Layout.Input, null, false);
            Android.App.AlertDialog.Builder dlg = new Android.App.AlertDialog.Builder(this);
            dlg.SetTitle("Input the window's size").SetMessage("Input the window's size").SetView(view).SetPositiveButton("OK", delegate
            {
                length = float.Parse(view.FindViewById<EditText>(Resource.Id.et_length).Text);
                width = float.Parse(view.FindViewById<EditText>(Resource.Id.et_width).Text);
                Reform(length ,width);
                btn_reform.Enabled = false;//Toast.MakeText(a1, "length:" + length, ToastLength.Short).Show();
            }).SetNegativeButton("Cancel", delegate { }).Show();
            
        }

        private void Btn_img_Click(object sender, System.EventArgs e)
        {
            SetButtonEnabled();
            if (IsThereAnAppToTakePictures())
            {
                GetCameraPermission();
            }
            
        }

        private void Btn_rect_Click(object sender, System.EventArgs e)
        {
            SetButtonEnabled();
            //imageView.SetImageBitmap(App.bitmap);
            try { rl.RemoveView(rectView); } catch { }
            try { rl.RemoveView(planeView); } catch { }
            try { rl.RemoveView(picturView); } catch { }
            rl.AddView(rectView);
            rectView.SetRects();
            rectView.Invalidate();
            btn_rect.Enabled = false;
        }
        private void Reform(float width, float height)
        {
            Bitmap bitmap = Bitmap.CreateBitmap(1920, 1080, Bitmap.Config.Argb8888); //App.bitmap.Copy(Bitmap.Config.Argb8888, true); //
            Canvas canvas = new Canvas(bitmap);

            float scale = 1;

            if ((float)rl.Width / (float)App.bitmap.Width * (float)App.bitmap.Height > (float)rl.Height)
            {
                //scale = (float)App.bitmap.Width / (float)rl.Width;
                scale = (float)App.bitmap.Height / (float)rl.Height;
            }
            else
            {
                scale = (float)App.bitmap.Width / (float)rl.Width;
                //scale = (float)App.bitmap.Height / (float)rl.Height;
            }


            Matrix matrix = new Matrix();
            
            float[] src = new float[8] {rectView.circles[0].Current_X,rectView.circles[0].Current_Y, rectView.circles[1].Current_X,rectView.circles[1].Current_Y,
                                        rectView.circles[2].Current_X,rectView.circles[2].Current_Y, rectView.circles[3].Current_X,rectView.circles[3].Current_Y};
            for (int i = 0; i < src.Length; i++)
            {
                src[i] = src[i] * scale;
            }

            float out_width = 0, out_height = 0;
            ReSize(width, height,out out_width,out out_height);

            float[] dst = new float[8] {App.DrawOffset, App.DrawOffset,             App.DrawOffset + out_width, App.DrawOffset,
                                        App.DrawOffset, App.DrawOffset + out_height, App.DrawOffset + out_width, App.DrawOffset + out_height};

            App.left = App.DrawOffset;
            App.top = App.DrawOffset;
            App.right = App.DrawOffset + out_width;
            App.bottom = App.DrawOffset + out_height;

            matrix.SetPolyToPoly(src, 0, dst, 0, 4);
            
            try { rl.RemoveView(rectView); } catch { }
            try { rl.RemoveView(planeView); } catch { }
            try { rl.RemoveView(picturView); } catch { }
            picturView.SetMatrix(matrix);
            rl.AddView(picturView);
            picturView.Invalidate();
        }

        private void ReSize(float width, float height, out float out_width, out float out_height)
        {
            //先以长来设置
            out_width = Math.Abs(rl.Width) - 100;
            out_height = out_width / width * height;
            if (out_height > Math.Abs(rl.Height) - 100)
            {
                out_height = Math.Abs(rl.Height) - 100;
                out_width = out_height * width / height;
            }
            App.InputWidth = width;
            App.InputHeight = height;
            App.DrawWidth = (int)out_width;
            App.DrawHeight = (int)out_height;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == App.CameraRequestCode) //拍照
            {
                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                var contentUri = Android.Net.Uri.FromFile(App._file);
                mediaScanIntent.SetData(contentUri);
                SendBroadcast(mediaScanIntent);

                App.bitmap = App._file.Path.LoadBitmap();

                if (App.bitmap != null)
                {
                    //将图片绑定到控件上
                    imageView.SetScaleType(ImageView.ScaleType.FitStart);
                    imageView.SetImageBitmap(App.bitmap);
                    btn_img.Enabled = false;
                    App.LayoutWidth = rl.Width;
                    App.LayoutHeight = rl.Height;
                    //清空bitmap 否则会出现oom问题
                    //App.bitmap = null;
                }

                // Dispose of the Java side bitmap.
                GC.Collect();

            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
        private void CreateDirectoryForPictures()
        {
            App._dir = new Java.IO.File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), this.GetString(Resource.String.app_name));   
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }
        private void GetCameraPermission()
        {
            //判断是否有相机权限
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == (int)Permission.Granted)
            {
                //判断是否有存储权限
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
                {
                    //有权限，就调用相机
                    callcamera();
                }
                else
                {
                    //没权限，请求权限
                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.WriteExternalStorage }, App.StorageRequestCode);
                }
            }
            else
            {
                //没权限，请求权限
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, App.CameraRequestCode);
            }
        }
        public void callcamera()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            //Intent intent = new Intent(Intent.ActionPick,MediaStore.Images.Media.ExternalContentUri);
            //Intent intent = new Intent(Intent.ActionPick, null);

            intent.SetFlags(ActivityFlags.GrantReadUriPermission);

            intent.SetFlags(ActivityFlags.GrantWriteUriPermission);

            App._file = new Java.IO.File(App._dir, string.Format("myPhoto_{0}.jpg", Guid.NewGuid()));

            Uri uri = FileProvider.GetUriForFile(this, "com.StructuralPlaneStatistics.fileProvider", App._file);

            intent.PutExtra(MediaStore.ExtraOutput, uri);

            StartActivityForResult(intent, App.CameraRequestCode);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == App.CameraRequestCode)
            {
                // Check if the only required permission has been granted
                if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
                {
                    // Location permission has been granted, okay to retrieve the location of the device.
                    //获得权限
                    GetCameraPermission();
                }
                else
                {
                    //没有权限
                    Toast.MakeText(this, "没有相机权限，请重试", ToastLength.Long).Show();
                }
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
            if (requestCode == App.StorageRequestCode)
            {
                // Check if the only required permission has been granted
                if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
                {
                    // Location permission has been granted, okay to retrieve the location of the device.
                    //获得权限
                    callcamera();
                }
                else
                {
                    //没有权限
                    Toast.MakeText(this, "没有存储权限，请重试", ToastLength.Long).Show();
                }
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }

        }

        public void SetButtonEnabled()
        {
            btn_img.Enabled = true;
            btn_line.Enabled = true;
            btn_modify.Enabled = true;
            btn_rect.Enabled = true;
            btn_reform.Enabled = true;
            btn_rose.Enabled = true;
        }

    }

}

