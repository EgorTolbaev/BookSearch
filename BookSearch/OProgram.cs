using Android.App;
using Android.OS;

namespace BookSearch
{
    [Activity(Label = "О программе", Theme = "@android:style/Theme.NoTitleBar")]
    class OProgram : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.OProgram);
        }
    }
}