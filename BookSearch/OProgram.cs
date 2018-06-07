using Android.App;
using Android.Content.PM;
using Android.OS;

namespace BookSearch
{
    [Activity(Label = "О программе")]
    class OProgram : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.OProgram);
            this.RequestedOrientation = ScreenOrientation.Portrait;
        }
    }
}