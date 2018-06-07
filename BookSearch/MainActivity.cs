using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Net;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;


namespace BookSearch
{
    public static class App
    {
        public static Java.IO.File _file;
        public static Java.IO.File _dir;
        public static Bitmap bitmap;
    }
    
    [Activity(Label = "Book Search", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private ImageView _imageView;
        public static List<string> listTextOutput;
        public static ListView listView;
        public static List<string> listText;
        //флаг для определения нажатой кнопки
        public static bool ButtonFlag = false;

        public static StringBuilder sv;

        //метод для возвращения результата
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            //распознает по выбранному фото
            if (ButtonFlag == true)
            {

                if (resultCode == Result.Ok)
                {
                    _imageView.SetImageURI(data.Data);

                }

                int height = Resources.DisplayMetrics.HeightPixels;
                int width = _imageView.Height;
                //App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
                _imageView.BuildDrawingCache(true);
                App.bitmap = _imageView.GetDrawingCache(true);


                if (App.bitmap != null)
                {
                    Stream imageStream = BitmapHelpers.BitmapToStream(App.bitmap);
                    var task = new RecognizeTextTask(this).Execute(imageStream);

                    _imageView.SetImageBitmap(App.bitmap);
                    App.bitmap = null;
                }

                // Утилизируем растровое изображение на стороне Java
                GC.Collect();


                ButtonFlag = false;
                
            }//распознает по сделанному фото
            else
            {
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

                int height = Resources.DisplayMetrics.HeightPixels; //получение размера экрана
                int width = _imageView.Height;
                App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
                //App.bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.abc);

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
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            this.RequestedOrientation = ScreenOrientation.Portrait;


            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Button TakePhoto = FindViewById<Button>(Resource.Id.myButton);
                _imageView = FindViewById<ImageView>(Resource.Id.imageView1);

                //кнопка сделать фото
                TakePhoto.Click += TakeAPicture;  
            }
            //кнопка выбрать фото
            var SelectPhoto = FindViewById<Button>(Resource.Id.button1);            
            SelectPhoto.Click += delegate
            {
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 0);
                ButtonFlag = true;

                _imageView.Visibility = ViewStates.Visible;
            };
        }

        //создание каталога для файлов
        private void CreateDirectoryForPictures()
        {
            App._dir = new Java.IO.File(
                //доступ к хранилищу
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), "CameraAppDemo");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        //проверка на наличие камеры
        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            //извлекаем информацию о камере
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
           // _imageView.Visibility = ViewStates.Visible;
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
            //Опрограмме
            if (id == Resource.Id.aboutprogram)
            {
                Intent intent = new Intent(this, typeof(OProgram));
                StartActivity(intent);

                return true;
            }
            else
            //Выбор фото
            if (id == Resource.Id.SelectPhoto)
            {
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 0);
                ButtonFlag = true;

              //  _imageView.Visibility = ViewStates.Visible;
            }
            else
            //вывод на экран
            if (id == Resource.Id.TextOutput)
            {
                if (listTextOutput.Count != 0)
                {
                    Intent intent = new Intent(this, typeof(TextOutput));
                    StartActivity(intent);
                }
                else
                {
                    Toast.MakeText(this, "Текст не распознан, возможно плохое качество фото", ToastLength.Long).Show();
                }

            }
            else
            //переход на поиск activity SearchInternet
            if (id == Resource.Id.Search)
            {
                Intent intent = new Intent(this, typeof(SearchInternet));
                StartActivity(intent);
            }
            else
            if (id == Resource.Id.openCamera)
            {
                //захват изображения
                Intent intent = new Intent(MediaStore.ActionImageCapture);
                App._file = new Java.IO.File(App._dir, System.String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
                intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));
                StartActivityForResult(intent, 0);
                _imageView.Visibility = ViewStates.Visible;
            }
            return base.OnOptionsItemSelected(item);
        }
    }

    //распознание текста
    internal class RecognizeTextTask : AsyncTask<Stream, string, string>
    {
        private MainActivity mainActivity;
        //инициализация диалога
        ProgressDialog mDialog = new ProgressDialog(Application.Context);
        

        public RecognizeTextTask(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }

        //запуск в фоновом режиме
        protected override string RunInBackground(params Stream[] @params)
        {
            try
            {
                
                // выводим промежуточные результаты
                PublishProgress("Recognizing...");
                
                OCRHelpers oCRHelpers = new OCRHelpers();
                List<string> result = oCRHelpers.RecognizeText(@params[0]);
                MainActivity.listTextOutput = result;

                //передаем данные в одну переменную (динамическую строку)
                StringBuilder sb = new StringBuilder();
                foreach (string ch in MainActivity.listTextOutput)
                    sb.Append(ch.ToString() + "+");
                MainActivity.sv = sb;

                return "";
            }
            catch (Java.Lang.Exception ex)
            {
                return "";
            }
            
        }
        //открывает диалог
        protected override void OnPreExecute()
        {
            mDialog.Window.SetType(Android.Views.WindowManagerTypes.SystemAlert);
            mDialog.Show();
        }
        //для вывода промежуточного результата
        protected override void OnProgressUpdate(params string[] values)
        {
            mDialog.SetMessage(values[0]);
        }
        //закрываем диалог
        protected override void OnPostExecute(string result)
        {
            mDialog.Dismiss(); 
        }   
    }
    
}

