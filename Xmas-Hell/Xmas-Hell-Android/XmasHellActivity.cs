using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace Xmas_Hell_Android
{
    [Activity(Label = "Xmas Hell"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , Theme = "@style/Theme.Splash"
        , AlwaysRetainTaskState = true
        , LaunchMode = Android.Content.PM.LaunchMode.SingleInstance
        , ScreenOrientation = ScreenOrientation.SensorPortrait
        , ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize)]
    public class XmasHellActivity : Microsoft.Xna.Framework.AndroidGameActivity, View.IOnSystemUiVisibilityChangeListener
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var g = new XmasHell.XmasHell(this);
            View vw = (View)g.Services.GetService(typeof(View));

            SetContentView(vw);
            g.Run();

            vw.SetOnSystemUiVisibilityChangeListener(this);
            HideSystemUi();
        }

        public void OnSystemUiVisibilityChange(StatusBarVisibility visibility)
        {
            HideSystemUi();
        }

        private void HideSystemUi()
        {
            SystemUiFlags flags = SystemUiFlags.HideNavigation | SystemUiFlags.Fullscreen | SystemUiFlags.ImmersiveSticky;
            this.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)flags;
        }
    }
}

