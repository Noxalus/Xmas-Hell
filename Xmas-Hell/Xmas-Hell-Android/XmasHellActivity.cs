using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Content;

[assembly: MetaData("com.google.android.gms.games.APP_ID", Value = "@string/app_id")]

namespace XmasHellAndroid
{
    [Activity(Label = "Xmas Hell"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , Theme = "@style/Theme.Splash"
        , AlwaysRetainTaskState = true
        , ScreenOrientation = ScreenOrientation.SensorPortrait
        , ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize)]
    public class XmasHellActivity : Microsoft.Xna.Framework.AndroidGameActivity, View.IOnSystemUiVisibilityChangeListener
    {
        private XmasHell.XmasHell _game;
        private GameHelper _helper;
        const string TAG = "XmasHell";

        void Log(string message)
        {
            Android.Util.Log.Debug(TAG, message);
        }

        protected override void OnCreate(Bundle bundle)
        {
            Log("onCreate()");
            HideSystemUi();
            InitializeServices();

            if (_helper != null && _helper.SignedOut)
                _helper.SignIn();

            base.OnCreate(bundle);
            _game = new XmasHell.XmasHell(this);
            View vw = (View)_game.Services.GetService(typeof(View));

            SetContentView(vw);
            _game.Run();

            vw.SetOnSystemUiVisibilityChangeListener(this);
        }

        void InitializeServices()
        {
            // Setup Google Play Services Helper
            _helper = new GameHelper(this);
            _helper.Initialize();
        }

        public void OnSystemUiVisibilityChange(StatusBarVisibility visibility)
        {
            HideSystemUi();
        }

        private void HideSystemUi()
        {
            SystemUiFlags flags = SystemUiFlags.HideNavigation | SystemUiFlags.Fullscreen | SystemUiFlags.ImmersiveSticky;
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)flags;
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

        protected override void OnStart()
        {
            base.OnStart();

            if (_helper != null)
                _helper.Start();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (_helper != null)
                _helper.OnActivityResult(requestCode, resultCode, data);

            base.OnActivityResult(requestCode, resultCode, data);
        }

        protected override void OnStop()
        {
            if (_helper != null)
                _helper.Stop();

            base.OnStop();
        }

        public void ShowAchievements()
        {
            if (_helper != null && !_helper.SignedOut)
                _helper.ShowAchievements();
        }

        public void ShowLeaderboards(string leaderboardName = "")
        {
            if (_helper != null && !_helper.SignedOut)
            {
                if (leaderboardName == "")
                    _helper.ShowAllLeaderBoardsIntent();
                else
                    _helper.ShowLeaderBoardIntentForLeaderboard(leaderboardName);
            }
        }
    }
}

