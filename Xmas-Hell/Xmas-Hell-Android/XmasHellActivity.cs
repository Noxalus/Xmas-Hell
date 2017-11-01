using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace XmasHellAndroid
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
        private XmasHell.XmasHell _game;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _game = new XmasHell.XmasHell(this);
            View vw = (View)_game.Services.GetService(typeof(View));

            SetContentView(vw);
            _game.Run();

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

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_game != null)
                _game.OnDestroy();
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (_game != null)
                _game.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (_game != null)
                _game.OnResume();
        }
    }
}

