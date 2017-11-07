using System;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Games;
using Android.Gms.Games.Achievement;
using Android.Gms.Games.LeaderBoard;
using Android.App;
using Android.Content;
using Android.Views;
using System.Collections.Generic;
using System.Threading.Tasks;
using XmasHell.Entities.Bosses;

namespace XmasHellAndroid
{
    /// <summary>
    /// Basic wrapper for interfacing with the GooglePlayServices Game API's
    /// </summary>
    public class GameHelper : Java.Lang.Object, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        GoogleApiClient client;
        Activity activity;
        bool signedOut = true;
        bool signingIn = false;
        bool resolving = false;
        List<IAchievement> achievments = new List<IAchievement>();
        Dictionary<string, List<ILeaderboardScore>> scores = new Dictionary<string, List<ILeaderboardScore>>();

        const int REQUEST_LEADERBOARD = 9002;
        const int REQUEST_ALL_LEADERBOARDS = 9003;
        const int REQUEST_ACHIEVEMENTS = 9004;
        const int RC_RESOLVE = 9001;

        /// <summary>
        /// Gets or sets a value indicating whether the user is signed out or not.
        /// </summary>
        /// <value><c>true</c> if signed out; otherwise, <c>false</c>.</value>
        public bool SignedOut
        {
            get { return signedOut; }
            set
            {
                if (signedOut != value)
                    signedOut = value;
            }
        }

        /// <summary>
        /// Gets or sets the gravity for the GooglePlay Popups. 
        /// Defaults to Bottom|Center
        /// </summary>
        /// <value>The gravity for popups.</value>
        public GravityFlags GravityForPopups { get; set; }

        /// <summary>
        /// The View on which the Popups should show
        /// </summary>
        /// <value>The view for popups.</value>
        public View ViewForPopups { get; set; }

        /// <summary>
        /// This event is fired when a user successfully signs in
        /// </summary>
        public event EventHandler OnSignedIn;

        /// <summary>
        /// This event is fired when the Sign in fails for any reason
        /// </summary>
        public event EventHandler OnSignInFailed;

        /// <summary>
        /// This event is fired when the user Signs out
        /// </summary>
        public event EventHandler OnSignedOut;

        /// <summary>
        /// List of Achievements. Populated by LoadAchievements
        /// </summary>
        /// <value>The achievements.</value>
        public List<IAchievement> Achievements
        {
            get { return achievments; }
        }

        public GameHelper(Activity activity)
        {
            this.activity = activity;
            this.GravityForPopups = GravityFlags.Bottom | GravityFlags.Center;
        }

        public void Initialize()
        {
            CreateClient();
        }

        private void CreateClient()
        {
            var builder = new GoogleApiClient.Builder(activity, this, this);
            builder.AddApi(GamesClass.API);
            builder.AddScope(GamesClass.ScopeGames);
            builder.SetGravityForPopups((int)GravityForPopups);

            if (ViewForPopups != null)
                builder.SetViewForPopups(ViewForPopups);

            client = builder.Build();
        }

        /// <summary>
        /// Start the GooglePlayClient. This should be called from your Activity Start
        /// </summary>
        public void Start()
        {
            if (SignedOut && !signingIn)
                return;

            if (client != null && !client.IsConnected)
            {
                client.Connect();
            }
        }

        /// <summary>
        /// Disconnects from the GooglePlayClient. This should be called from your Activity Stop
        /// </summary>
        public void Stop()
        {

            if (client != null && client.IsConnected)
            {
                client.Disconnect();
            }
        }

        /// <summary>
        /// Reconnect to google play.
        /// </summary>
        public void Reconnect()
        {
            if (client != null)
                client.Reconnect();
        }

        /// <summary>
        /// Sign out of Google Play and make sure we don't try to auto sign in on the next startup
        /// </summary>
        public void SignOut()
        {
            SignedOut = true;
            if (client.IsConnected)
            {
                GamesClass.SignOut(client);
                Stop();
                client.Dispose();
                client = null;
                OnSignedOut?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Attempt to Sign in to Google Play
        /// </summary>
        public void SignIn()
        {
            signingIn = true;
            if (client == null)
                CreateClient();

            if (client.IsConnected)
                return;

            if (client.IsConnecting)
                return;

            var result = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(activity);
            if (result != ConnectionResult.Success)
                return;

            Start();
        }

        /// <summary>
        /// Unlocks the achievement.
        /// </summary>
        /// <param name="achievementCode">Achievement code from you applications Google Play Game Services Achievements Page</param>
        public void UnlockAchievement(string achievementCode)
        {
            GamesClass.Achievements.Unlock(client, achievementCode);
        }

        public void IncrementAchievement(string achievementCode, int progress)
        {
            GamesClass.Achievements.Increment(client, achievementCode, progress);
        }

        /// <summary>
        /// Show the built in google Achievements Activity. This will cause your application to go into a Paused State
        /// </summary>
        public void ShowAchievements()
        {
            var intent = GamesClass.Achievements.GetAchievementsIntent(client);
            activity.StartActivityForResult(intent, REQUEST_ACHIEVEMENTS);
        }

        /// <summary>
        /// Submit a score to google play. The score will only be updated if it is greater than the existing score. 
        /// This is not immediate but will occur at the next sync of the google play client.
        /// </summary>
        /// <param name="leaderboardCode">Leaderboard code from you applications Google Play Game Services Leaderboards Page</param>
        /// <param name="value">The value of the score</param>
        public void SubmitScore(string leaderboardCode, long value)
        {
            GamesClass.Leaderboards.SubmitScore(client, leaderboardCode, value);
        }

        /// <summary>
        /// Submit a score to google play. The score will only be updated if it is greater than the existing score. 
        /// This is not immediate but will occur at the next sync of the google play client.
        /// </summary>
        /// <param name="leaderboardCode">Leaderboard code from you applications Google Play Game Services Leaderboards Page</param>
        /// <param name="value">The value of the score</param>
        /// <param name="value">Additional MetaData to attach. Must be a URI safe string with a max length of 64 characters</param>
        public void SubmitScore(string leaderboardCode, long value, string metadata)
        {
            GamesClass.Leaderboards.SubmitScore(client, leaderboardCode, value, metadata);
        }

        /// <summary>
        /// Show the built in leaderboard activity for the leaderboard code.
        /// </summary>
        /// <param name="leaderboardCode">Leaderboard code from you applications Google Play Game Services Leaderboards Page</param>
        public void ShowLeaderBoardIntentForLeaderboard(string leaderboardCode)
        {
            var intent = GamesClass.Leaderboards.GetLeaderboardIntent(client, leaderboardCode);
            activity.StartActivityForResult(intent, REQUEST_LEADERBOARD);
        }

        /// <summary>
        /// Show the built in leaderboard activity for all the leaderboards setup for your application
        /// </summary>
        public void ShowAllLeaderBoardsIntent()
        {
            var intent = GamesClass.Leaderboards.GetAllLeaderboardsIntent(client);
            activity.StartActivityForResult(intent, REQUEST_ALL_LEADERBOARDS);
        }

        /// <summary>
        /// Load the Achievments. This populates the Achievements property
        /// </summary>
        public async Task LoadAchievements()
        {
            var ar = await GamesClass.Achievements.LoadAsync(client, false);
            if (ar != null)
            {
                achievments.Clear();
                achievments.AddRange(ar.Achievements);
            }
        }

        public async Task LoadTopScores(string leaderboardCode)
        {
            var ar = await GamesClass.Leaderboards.LoadTopScoresAsync(client, leaderboardCode, 2, 0, 25);
            if (ar != null)
            {
                var id = ar.Leaderboard.LeaderboardId;
                if (!scores.ContainsKey(id))
                {
                    scores.Add(id, new List<ILeaderboardScore>());
                }
                scores[id].Clear();
                scores[id].AddRange(ar.Scores);
            }
        }

        #region IGoogleApiClientConnectionCallbacks implementation

        public void OnConnected(Android.OS.Bundle connectionHint)
        {
            resolving = false;
            SignedOut = false;
            signingIn = false;

            OnSignedIn?.Invoke(this, EventArgs.Empty);
        }

        public void OnConnectionSuspended(int resultCode)
        {
            resolving = false;
            SignedOut = false;
            signingIn = false;
            client.Disconnect();
            OnSignInFailed?.Invoke(this, EventArgs.Empty);
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            if (resolving)
                return;

            if (result.HasResolution)
            {
                resolving = true;
                result.StartResolutionForResult(activity, RC_RESOLVE);
                return;
            }

            resolving = false;
            SignedOut = false;
            signingIn = false;
            OnSignInFailed?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        /// <summary>
        /// Processes the Activity Results from the Signin process. MUST be called from your activity OnActivityResult override.
        /// </summary>
        /// <param name="requestCode">Request code.</param>
        /// <param name="resultCode">Result code.</param>
        /// <param name="data">Data.</param>
        public void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {

            if (requestCode == RC_RESOLVE)
            {
                if (resultCode == Result.Ok)
                    Start();
                else
                    OnSignInFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        #region Xmas Hell specific code

        public string BossTypeToAchievementCode(BossType bossType)
        {
            switch (bossType)
            {
                case BossType.XmasBall:
                    return activity.GetString(Resource.String.achievement_defeat_xmas_ball);
                case BossType.XmasBell:
                    return activity.GetString(Resource.String.achievement_defeat_xmas_bell);
                case BossType.XmasCandy:
                    return activity.GetString(Resource.String.achievement_defeat_xmas_candy);
                case BossType.XmasSnowflake:
                    return activity.GetString(Resource.String.achievement_defeat_xmas_snowflake);
                case BossType.XmasLog:
                    return activity.GetString(Resource.String.achievement_defeat_xmas_log);
                case BossType.XmasTree:
                    return activity.GetString(Resource.String.achievement_defeat_xmas_tree);
                case BossType.XmasGift:
                    return activity.GetString(Resource.String.achievement_defeat_xmas_gift);
                case BossType.XmasReindeer:
                    return activity.GetString(Resource.String.achievement_defeat_xmas_reindeer);
                case BossType.XmasSnowman:
                    return activity.GetString(Resource.String.achievement_defeat_xmas_snowman);
                case BossType.XmasSanta:
                    return activity.GetString(Resource.String.achievement_defeat_xmas_santa);
                default:
                    return "";
            }
        }

        public string BossTypeToLeaderboardCode(BossType bossType)
        {
            switch (bossType)
            {
                case BossType.XmasBall:
                    return activity.GetString(Resource.String.leaderboard_xmas_ball);
                case BossType.XmasBell:
                    return activity.GetString(Resource.String.leaderboard_xmas_bell);
                case BossType.XmasCandy:
                    return activity.GetString(Resource.String.leaderboard_xmas_candy);
                case BossType.XmasSnowflake:
                    return activity.GetString(Resource.String.leaderboard_xmas_snowflake);
                case BossType.XmasLog:
                    return activity.GetString(Resource.String.leaderboard_xmas_log);
                case BossType.XmasTree:
                    return activity.GetString(Resource.String.leaderboard_xmas_tree);
                case BossType.XmasGift:
                    return activity.GetString(Resource.String.leaderboard_xmas_gift);
                case BossType.XmasReindeer:
                    return activity.GetString(Resource.String.leaderboard_xmas_reindeer);
                case BossType.XmasSnowman:
                    return activity.GetString(Resource.String.leaderboard_xmas_snowman);
                case BossType.XmasSanta:
                    return activity.GetString(Resource.String.leaderboard_xmas_santa);
                default:
                    return "";
            }
        }

        #endregion
    }
}
