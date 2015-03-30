using Android.App;
using Android.Content.PM;
using Cirrious.MvvmCross.Droid.Views;
using CoolBeans.Droid.MvxDroidAdaptation;

namespace RATBVFormsX.Droid
{
    [Activity(
		Label = "RATBVFormsX.Droid"
		, MainLauncher = true
		, Icon = "@drawable/icon"
		, Theme = "@style/Theme.Splash"
		, NoHistory = true
		, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxFormsSplashScreenActivity
    {
        public SplashScreen()
            : base(Resource.Layout.SplashScreen)
        {
        }
    }
}