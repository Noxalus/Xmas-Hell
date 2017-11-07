using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Games;
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
    public class XmasHellActivity : Microsoft.Xna.Framework.AndroidGameActivity, View.IOnSystemUiVisibilityChangeListener, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        private XmasHell.XmasHell _game;

        // Google Play Services
        const string TAG = "XmasHell";

        // Default lifetime of a request, 1 week.
        const int DEFAULT_LIFETIME = 7;

        // Request code used to invoke sign in user interactions.
        const int RC_SIGN_IN = 9001;

        // Client used to interact with Google APIs.
        GoogleApiClient mGoogleApiClient;

        // Are we currently resolving a connection failure?
        bool mResolvingConnectionFailure = false;

        // Set to true to automatically start the sign in flow when the Activity starts.
        // Set to false to require the user to click the button in order to sign in.
        bool mAutoStartSignInFlow = true;

        void Log(string message)
        {
            Android.Util.Log.Debug(TAG, message);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _game = new XmasHell.XmasHell(this);
            View vw = (View)_game.Services.GetService(typeof(View));

            SetContentView(vw);
            _game.Run();

            vw.SetOnSystemUiVisibilityChangeListener(this);
            HideSystemUi();

            Log("onCreate()");

            // Create the Google Api Client with access to Plus and Games
            mGoogleApiClient = new GoogleApiClient.Builder(this)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .AddApi(GamesClass.API).AddScope(GamesClass.ScopeGames)
                .Build();

            mGoogleApiClient.Connect();
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

        public void OnConnectionFailed(ConnectionResult result)
        {
            Log("onConnectionFailed() called, result: " + result);

            if (mResolvingConnectionFailure)
            {
                Log("onConnectionFailed() ignoring connection failure; already resolving.");
                return;
            }

            if (mAutoStartSignInFlow)
            {
                mAutoStartSignInFlow = false;
                mResolvingConnectionFailure = ResolveConnectionFailure(
                    this, mGoogleApiClient, result, RC_SIGN_IN, GetString(Resource.String.signin_other_error)
                );
            }
        }

        bool ResolveConnectionFailure(Activity activity, GoogleApiClient client, ConnectionResult result, int requestCode, string fallbackErrorMessage)
        {
            if (result.HasResolution)
            {
                try
                {
                    result.StartResolutionForResult(activity, requestCode);
                    return true;
                }
                catch (IntentSender.SendIntentException e)
                {
                    // The intent was canceled before it was sent.  Return to the default
                    // state and attempt to connect to get an updated ConnectionResult.
                    client.Connect();
                    return false;
                }
            }
            else
            {
                // not resolvable... so show an error message
                int errorCode = result.ErrorCode;
                var dialog = GooglePlayServicesUtil.GetErrorDialog(errorCode, activity, requestCode);
                if (dialog != null)
                {
                    dialog.Show();
                }
                else
                {
                    // no built-in dialog: show the fallback error message
                    //ShowAlert (activity, fallbackErrorMessage);
                    (new AlertDialog.Builder(activity)).SetMessage(fallbackErrorMessage)
                        .SetNeutralButton(Android.Resource.String.Ok, delegate { }).Create().Show();
                }
                return false;
            }
        }

        public void OnConnected(Bundle connectionHint)
        {
            Log("onConnected() called. Sign in successful!");

            var player = GamesClass.Players.GetCurrentPlayer(mGoogleApiClient);
            var xmasBallLeaderboard = GamesClass.Leaderboards.GetLeaderboardIntent(mGoogleApiClient, "@string/leaderboard_xmas_ball");
            var achievementsIntent = GamesClass.Achievements.GetAchievementsIntent(mGoogleApiClient);

            const int REQUEST_ACHIEVEMENTS = 9004;
            StartActivityForResult(achievementsIntent, REQUEST_ACHIEVEMENTS);
        }

        public void OnConnectionSuspended(int cause)
        {
            Log("onConnectionSuspended() called. Trying to reconnect.");
            mGoogleApiClient.Connect();
        }
    }
}

