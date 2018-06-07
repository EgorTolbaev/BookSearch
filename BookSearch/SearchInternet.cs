using Android.Widget;
using Android.App;
using Android.OS;
using Android.Webkit;
using System;

namespace BookSearch
{
    [Activity(Label = "Поиск")]
    class SearchInternet : Activity
    {
        public static EditText textURL;
        RadioGroup SelectionUrl;
        WebView webView;
        CheckBox ShowSearchBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SearchInternet);

            //инициализация элементов
            webView = FindViewById<Android.Webkit.WebView>(Resource.Id.webView1);
            textURL = FindViewById<EditText>(Resource.Id.editText1);
            textURL.Visibility = Android.Views.ViewStates.Invisible;

            SelectionUrl = FindViewById<RadioGroup>(Resource.Id.radioGroup1);
            SelectionUrl.CheckedChange += VibrUrl_CheckedChange;

            ShowSearchBar = FindViewById<CheckBox>(Resource.Id.checkBox1);
            ShowSearchBar.CheckedChange += ShowSearchBar_CheckedChange;

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
        //Показать строку поиска
        private void ShowSearchBar_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (ShowSearchBar.Checked)
            {
                textURL.Visibility = Android.Views.ViewStates.Visible;
            }
            else
            {
                textURL.Visibility = Android.Views.ViewStates.Invisible;
            }
            
        }

        //Выбор Магазина
        private void VibrUrl_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            switch (SelectionUrl.CheckedRadioButtonId)
            {

                case Resource.Id.radioButton1:
                    //предзагрузка ссылки на сайт с данными из динамичской строки
                    base.OnStart();
                    if (!textURL.Text.Contains("https://www.ozon.ru/context/div_book/"))
                    {
                        string address = textURL.Text;
                        textURL.Text = string.Format("https://www.ozon.ru/?context=search&text={0}", MainActivity.sv);
                    }
                    webView.LoadUrl(textURL.Text);

                    break;

                case Resource.Id.radioButton2:
                    //предзагрузка ссылки на сайт с данными из динамичской строки
                    base.OnStart();
                    if (!textURL.Text.Contains("https://market.yandex.ru/catalog--knigi/54510"))
                    {
                        string address = textURL.Text;
                        textURL.Text = string.Format("https://market.yandex.ru/catalog/71726/list?text={0}&hid=15561854&rt=9&was_redir=1&srnum=3&rs=eJwzMjIy4NLj4uXYeJRVgEmCQfXL0sP7gdyzQC6DBKNqzl-N_Vx8HP9ebGUXYJRgUq310NwPAHJODxY%2C&local-offers-first=0", MainActivity.sv);
                    }
                    webView.LoadUrl(textURL.Text);
                    break;

                case Resource.Id.radioButton3:
                    //предзагрузка ссылки на сайт с данными из динамичской строки
                    base.OnStart();
                    if (!textURL.Text.Contains("https://books.google.ru/"))
                    {
                        string address = textURL.Text;
                        textURL.Text = string.Format("https://www.google.ru/search?q={0}&source=univ&tbm=shop&tbo=u&sa=X&ved=0ahUKEwjh4YfL__raAhUziaYKHVFNBqYQsxgIkAE&biw=1536&bih=735", MainActivity.sv);
                    }
                    webView.LoadUrl(textURL.Text);
                    break;
            }

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