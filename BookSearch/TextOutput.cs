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

namespace BookSearch
{
    [Activity(Label = "Вывод текста")]
    class TextOutput : Activity
    {
        Button Search;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TextOutput);

            if (MainActivity.listTextOutput.Count != 0)
            {
                MainActivity.listText = new List<string>();
                MainActivity.listText = MainActivity.listTextOutput;
                MainActivity.listView = FindViewById<ListView>(Resource.Id.listView1);
                //привязка массива к списку
                ArrayAdapter<string> arrayAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, MainActivity.listText);
                MainActivity.listView.Adapter = arrayAdapter;


            }
            else
            {
                Toast.MakeText(this, "Текст не распознан, возможно плохое качество фото", ToastLength.Long).Show();
            }

            Search = FindViewById<Button>(Resource.Id.button1);
            Search.Click += Search_Click;
        }
        //переход на поиск activity SearchInternet
        private void Search_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SearchInternet));
            StartActivity(intent);
        }
    }
}