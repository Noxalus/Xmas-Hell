using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Content;

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
        public GameHelper GameHelper;
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

            if (GameHelper != null && GameHelper.SignedOut)
                GameHelper.SignIn();

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
            GameHelper = new GameHelper(this);
            GameHelper.Initialize();
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

            if (GameHelper != null)
                GameHelper.Start();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (GameHelper != null)
                GameHelper.OnActivityResult(requestCode, resultCode, data);

            base.OnActivityResult(requestCode, resultCode, data);
        }

        protected override void OnStop()
        {
            if (GameHelper != null)
                GameHelper.Stop();

            base.OnStop();
        }

        public void ShowAchievements()
        {
            if (GameHelper != null && !GameHelper.SignedOut)
                GameHelper.ShowAchievements();
        }

        public void ShowLeaderboards(string leaderboardName = "")
        {
            if (GameHelper != null && !GameHelper.SignedOut)
            {
                if (leaderboardName == "")
                    GameHelper.ShowAllLeaderBoardsIntent();
                else
                    GameHelper.ShowLeaderBoardIntentForLeaderboard(leaderboardName);
            }
        }
    }
}

