using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using Java.IO;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;

using System.Collections.Generic;
using System.IO;
using Com.Microsoft.Projectoxford.Vision;
using Com.Microsoft.Projectoxford.Vision.Contract;
using GoogleGson;
using BookSearch.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BookSearch
{
    public static class App
    {
        public static Java.IO.File _file;
        public static Java.IO.File _dir;
        public static Bitmap bitmap;
    }

    [Activity(Label = "Book Search", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private ImageView _imageView;

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Делаем доступным в галерее

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);

            /*
             Отображаем в ImageView и изменяем размер растрового изображения что бы соответствовало
             дисплею
             Загрузка полноразмерного изображения будет потреблять много памяти и приведет к сбою
             */

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = _imageView.Height;
            App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
            
            if (App.bitmap != null)
            {
                Stream imageStream = BitmapHelpers.BitmapToStream(App.bitmap);
                var task = new RecognizeTextTask(this).Execute(imageStream);
                
                _imageView.SetImageBitmap(App.bitmap);
                App.bitmap = null;
            }
            
            // Утилизируем растровое изображение на стороне Java
            GC.Collect();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Button button = FindViewById<Button>(Resource.Id.myButton);
                _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
                button.Click += TakeAPicture;
            }    
        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new Java.IO.File(
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), "CameraAppDemo");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        //делаем фото
        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            //захват изображения
            Intent intent = new Intent(MediaStore.ActionImageCapture);

            App._file = new Java.IO.File(App._dir, System.String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));

            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));

            StartActivityForResult(intent, 0);
        }

        //создаем меню
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        //определяем действие при нажатии кнопок меню
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.aboutprogram)
            {
                Intent intent = new Intent(this, typeof(OProgram));
                StartActivity(intent);

                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

    }



    internal class RecognizeTextTask : AsyncTask<Stream, string, string>
    {
        private MainActivity mainActivity;

        public RecognizeTextTask(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }

        protected override string RunInBackground(params Stream[] @params)
        {
            try
            {
                OCRHelpers oCRHelpers = new OCRHelpers();
                List<string> result = oCRHelpers.RecognizeText(@params[0]);
                return "";
            }
            catch (Java.Lang.Exception ex)
            {
                return "";
            }
            
        }
    }
}

