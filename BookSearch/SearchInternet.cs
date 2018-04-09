using Android.Widget;
using Android.App;
using Android.OS;
using Android.Webkit;
using System;

namespace BookSearch
{
    [Activity(Label = "Поиск", Theme = "@android:style/Theme.NoTitleBar")]
    class SearchInternet : Activity
    {
        public static EditText textURL;
        WebView webView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SearchInternet);

            //инициализация элементов
            webView = FindViewById<Android.Webkit.WebView>(Resource.Id.webView1);
            textURL = FindViewById<EditText>(Resource.Id.editText1);

            webView.SetWebViewClient(new ExtendWebViewClient());
            //продключаем js
            WebSettings webSettings = webView.Settings;
            webSettings.JavaScriptEnabled = true;

            //предзагрузка ссылки на сайт с данными из динамичской строки
            base.OnStart();
            if (!textURL.Text.Contains("https://www.ozon.ru/?context=search&text="))
            {
                string address = textURL.Text;
                textURL.Text = string.Format("https://www.ozon.ru/?context=search&text={0}", MainActivity.sv);
            }
            webView.LoadUrl(textURL.Text);



        }

    }
    //позволяет открыть ссылку прям в приложении
    internal class ExtendWebViewClient : WebViewClient
    {
        //переопределить загрузку URL
        public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, string url)
        {
            view.LoadUrl(url);
            return true;

        }
    }

}