using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace XmasHell
{
    [Activity(Label = "Xmas-Hell"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , Theme = "@style/Theme.Splash"
        , AlwaysRetainTaskState = true
        , LaunchMode = Android.Content.PM.LaunchMode.SingleInstance
        , ScreenOrientation = ScreenOrientation.SensorLandscape
        , ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize)]
    public class XmasHellActivity : Microsoft.Xna.Framework.AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var g = new XmasHell(this);
            SetContentView((View)g.Services.GetService(typeof(View)));
            g.Run();
        }
    }
}

