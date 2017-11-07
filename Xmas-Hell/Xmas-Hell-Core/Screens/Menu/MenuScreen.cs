using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System;
using XmasHell.BulletML;
using XnaMediaPlayer = Microsoft.Xna.Framework.Media.MediaPlayer;

namespace XmasHell.Screens.Menu
{
    public class MenuScreen : Screen
    {
        private string _patternFile = "MainMenu/snowflake";
        private TimeSpan _shootFrequency;

        public MenuScreen(XmasHell game) : base(game)
        {
        }

        public override void Initialize()
        {
            _shootFrequency = TimeSpan.Zero;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (Game.GameManager.MoverManager.FindPattern(_patternFile) == null)
            {
                var pattern = new BulletPattern();
                pattern.ParseStream(_patternFile, Assets.GetPattern(_patternFile));

                Game.GameManager.MoverManager.AddPattern(_patternFile, pattern);
            }
        }

        public override void Show(bool reset)
        {
            base.Show(reset);

            if (XnaMediaPlayer.State == MediaState.Stopped)
            {
                XnaMediaPlayer.Volume = 1f;
                XnaMediaPlayer.IsRepeating = true;
                XnaMediaPlayer.Play(Assets.GetMusic("main-menu"));
            }

            XnaMediaPlayer.MediaStateChanged += MediaPlayerOnMediaStateChanged;
            XnaMediaPlayer.ActiveSongChanged += MediaPlayerOnActiveSongChanged;
        }

        public override void Hide()
        {
            base.Hide();

            XnaMediaPlayer.MediaStateChanged -= MediaPlayerOnMediaStateChanged;
            XnaMediaPlayer.ActiveSongChanged -= MediaPlayerOnActiveSongChanged;
        }

        private void MediaPlayerOnActiveSongChanged(object sender, EventArgs eventArgs)
        {
            //throw new NotImplementedException();
        }

        private void MediaPlayerOnMediaStateChanged(object sender, EventArgs eventArgs)
        {
            //if (XnaMediaPlayer.Queue.ActiveSong == null)
            //    return;

            //if (XnaMediaPlayer.Queue.ActiveSong.Name == _introSong.Name &&
            //    XnaMediaPlayer.Queue.ActiveSong.Position >= _introSong.Duration)
            //{
            //    XnaMediaPlayer.IsRepeating = true;
            //    XnaMediaPlayer.Play(_mainSong);
            //}
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_shootFrequency.TotalMilliseconds < 0)
            {
                var randomX = Game.GameManager.Random.Next(0, Game.ViewportAdapter.VirtualWidth);
                var position = new Vector2(randomX, -500);

                Game.GameManager.MoverManager.TriggerPattern(_patternFile, BulletType.Type1, false, position);
                _shootFrequency = TimeSpan.FromSeconds(0.01);
            }
            else
            {
                _shootFrequency -= gameTime.ElapsedGameTime;
            }
        }
    }
}
